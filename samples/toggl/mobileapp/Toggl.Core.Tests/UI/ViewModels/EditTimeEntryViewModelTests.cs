using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.DTOs;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xunit;
using static Toggl.Core.Helper.Constants;
using ProjectClientTaskInfo = Toggl.Core.UI.ViewModels.EditTimeEntryViewModel.ProjectClientTaskInfo;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class EditTimeEntryViewModelTests
    {
        public abstract class EditTimeEntryViewModelTest : BaseViewModelWithInputTests<EditTimeEntryViewModel, long[]>
        {
            protected static readonly string WorkspaceName = "The best workspace ever";
            protected static readonly string ProjectName = "Very nice project";
            protected static readonly string ProjectColor = "#000000";
            protected static readonly string ClientName = "Bobby tables";
            protected static readonly string TaskName = "It's compiling.";
            protected static readonly long WorkspaceIdWithBillableAvailable = 100;
            protected static readonly long WorkspaceIdWithoutBillableAvailable = 200;
            protected static readonly string Description = "Sample description";
            protected static readonly string StringSample = "Lorem ipsum.";
            protected static readonly long[] EmptyArray = new long[] { };
            protected static readonly long[] NullArray = null;
            protected static readonly long[] TimeEntriesGroupIds = new long[] { 123, 456, 789 };
            protected static long[] SingleTimeEntryId => TimeEntriesGroupIds.Take(1).ToArray();
            protected static readonly DateTimeOffset Now = DateTimeOffset.Now;
            protected static readonly TimeSpan OneHour = TimeSpan.FromHours(1);
            protected static readonly int TagCount = 4;

            protected static string TagNameFromId(long id) => $"Tag {id}";
            protected static IThreadSafeTag TagFromId(long id, IThreadSafeWorkspace workspace)
                => new MockTag(id, workspace) { Name = TagNameFromId(id) };

            protected static readonly MockWorkspace InaccessibleWorkspace
                = new MockWorkspace(WorkspaceIdWithBillableAvailable, true);

            protected static readonly MockWorkspace WorkspaceWithBillableAvailable
                = new MockWorkspace(WorkspaceIdWithBillableAvailable);

            protected static readonly MockWorkspace WorkspaceWithoutBillableAvailable
                = new MockWorkspace(WorkspaceIdWithoutBillableAvailable);

            protected override void AdditionalSetup()
            {
                var nowObservable = Observable.Return(Now);
                TimeService.CurrentDateTime.Returns(Now);
                TimeService.CurrentDateTimeObservable.Returns(nowObservable);
            }

            protected override EditTimeEntryViewModel CreateViewModel()
                => new EditTimeEntryViewModel(
                    TimeService,
                    DataSource,
                    SyncManager,
                    InteractorFactory,
                    NavigationService,
                    OnboardingStorage,
                    AnalyticsService,
                    RxActionFactory,
                    SchedulerProvider);
        }

        public abstract class InitializableEditTimeEntryViewModelTest : EditTimeEntryViewModelTest
        {
            protected IEnumerable<MockTimeEntry> Entries { get; set; }
            protected long[] TimeEntriesIds { get; set; }

            public InitializableEditTimeEntryViewModelTest() : this(SingleTimeEntryId)
            {
            }

            public InitializableEditTimeEntryViewModelTest(long[] ids)
            {
                TimeEntriesIds = ids;

                AdjustTimeEntries(TimeEntriesIds, te => te);

                SetupPreferences(
                    false,
                    DateFormat.FromLocalizedDateFormat("DD.MM.YYYY"),
                    TimeFormat.TwelveHoursFormat,
                    DurationFormat.Improved);

                SetupBillableAvailabilityInteractors();

                SetupDataSource();
            }

            protected virtual void AdjustTimeEntries(long[] ids, Func<MockTimeEntry, MockTimeEntry> timeEntryModifier)
            {
                ViewModel.Initialize(ids);
                SetupTimeEntries(ids, (te, index) => timeEntryModifier(te));
            }

            protected virtual void SetupTimeEntries(long[] ids, Func<MockTimeEntry, int, MockTimeEntry> timeEntryModifier)
            {
                Entries = ids
                   .Select(id => CreateTimeEntry(Now, id, false))
                   .Select(timeEntryModifier)
                   .ToList();
                var observable = Observable.Return(Entries);

                InteractorFactory
                    .GetMultipleTimeEntriesById(Arg.Any<long[]>())
                    .Execute()
                    .Returns(observable);
            }

            protected virtual void SetupPreferences(bool collapse, DateFormat dateFormat, TimeFormat timeFormat, DurationFormat durationFormat)
            {
                var preferences = new MockPreferences
                {
                    Id = 1,
                    CollapseTimeEntries = collapse,
                    DateFormat = dateFormat,
                    TimeOfDayFormat = timeFormat,
                    DurationFormat = durationFormat
                };

                var observable = Observable.Return(preferences);

                InteractorFactory
                    .GetPreferences()
                    .Execute()
                    .Returns(observable);
            }

            protected virtual void SetupBillableAvailabilityInteractors()
            {
                var trueObservable = Observable.Return(true);
                var falseObservable = Observable.Return(false);

                InteractorFactory
                    .IsBillableAvailableForWorkspace(Arg.Is(WorkspaceIdWithBillableAvailable))
                    .Execute()
                    .Returns(trueObservable);

                InteractorFactory
                    .IsBillableAvailableForWorkspace(Arg.Is(WorkspaceIdWithoutBillableAvailable))
                    .Execute()
                    .Returns(falseObservable);
            }

            protected virtual void SetupDataSource()
            {
            }

            protected MockTimeEntry CreateTimeEntry(DateTimeOffset time, long id, bool isRunning)
            {
                var user = new MockUser { Id = 1 };

                var workspace = new MockWorkspace(WorkspaceIdWithBillableAvailable)
                {
                    Name = WorkspaceName
                };

                var client = new MockClient(2, workspace)
                {
                    Name = ClientName
                };

                var project = new MockProject(3, workspace, client)
                {
                    Name = ProjectName,
                    Color = ProjectColor,
                    Active = true
                };

                var task = new MockTask(4, workspace, project)
                {
                    Name = TaskName
                };

                var tags = Enumerable.Range(0, TagCount)
                    .Select(tagId => TagFromId(tagId, workspace))
                    .ToArray();

                return new MockTimeEntry
                {
                    Id = id,
                    Task = task,
                    TaskId = task.Id,
                    UserId = user.Id,
                    Project = project,
                    Workspace = workspace,
                    ProjectId = project.Id,
                    At = time.AddHours(-2),
                    Start = time.AddHours(-2),
                    Description = Description,
                    WorkspaceId = workspace.Id,
                    Tags = tags,
                    TagIds = tags.Select(tag => tag.Id).ToArray(),
                    Duration = isRunning ? (long?)null : (long)OneHour.TotalSeconds
                };
            }

            protected static EditTimeEntryDto CreateDtoFromTimeEntry(IThreadSafeTimeEntry timeEntry)
            {
                return new EditTimeEntryDto
                {
                    Id = timeEntry.Id,
                    Description = timeEntry.Description,
                    StartTime = timeEntry.Start,
                    StopTime = CalculateStopTime(timeEntry.Start, timeEntry.TimeSpanDuration()),
                    ProjectId = timeEntry.ProjectId,
                    TaskId = timeEntry.TaskId,
                    Billable = timeEntry.Billable,
                    WorkspaceId = timeEntry.WorkspaceId,
                    TagIds = timeEntry.TagIds
                };
            }

            protected static bool DtosEqualTimeEntries(IEnumerable<EditTimeEntryDto> dtos, IEnumerable<IThreadSafeTimeEntry> timeEntries)
            {
                var orderedDtos = dtos.OrderBy(dto => dto.Id);
                var orderedTimeEntries = timeEntries.OrderBy(te => te.Id);

                var areEqual = Enumerable
                    .Zip(orderedDtos, orderedTimeEntries, DtoEqualsTimeEntry)
                    .Select(equal => !equal)
                    .None(CommonFunctions.Identity);

                return areEqual;
            }

            protected static bool DtoEqualsTimeEntry(EditTimeEntryDto dto, IThreadSafeTimeEntry timeEntry)
                => dto.Equals(CreateDtoFromTimeEntry(timeEntry));

            protected static DateTimeOffset? CalculateStopTime(DateTimeOffset start, TimeSpan? duration)
                => duration.HasValue ? start + duration : null;

            protected IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>> SetupUpdateInteractor(
                IEnumerable<IThreadSafeTimeEntry> entries)
            {
                var timeEntryObservable = Observable.Return(entries);
                var interactor = Substitute.For<IInteractor<IObservable<IEnumerable<IThreadSafeTimeEntry>>>>();
                interactor
                    .Execute()
                    .Returns(timeEntryObservable);
                InteractorFactory
                     .UpdateMultipleTimeEntries(Arg.Any<EditTimeEntryDto[]>())
                     .Returns(interactor);

                return interactor;
            }
        }

        public abstract class GroupedTimeEntriesEditTimeEntryViewModelTest : InitializableEditTimeEntryViewModelTest
        {
            public GroupedTimeEntriesEditTimeEntryViewModelTest() : base(TimeEntriesGroupIds) { }
        }

        public sealed class TheConstructor : EditTimeEntryViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useDataSource,
                bool useSyncManager,
                bool useNavigationService,
                bool useTimeService,
                bool useInteractorFactory,
                bool useOnboardingStorage,
                bool useAnalyticsService,
                bool useRxActionFactory,
                bool useSchedulerProvider)
            {
                var dataSource = useDataSource ? DataSource : null;
                var syncManager = useSyncManager ? SyncManager : null;
                var timeService = useTimeService ? TimeService : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var onboardingStorage = useOnboardingStorage ? OnboardingStorage : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new EditTimeEntryViewModel(
                        timeService,
                        dataSource,
                        syncManager,
                        interactorFactory,
                        navigationService,
                        onboardingStorage,
                        analyticsService,
                        rxActionFactory,
                        schedulerProvider);

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheInitializeMethod : EditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ThrowsIfReceivedEmptyArray()
            {
                Func<Task> work = async () => await ViewModel.Initialize(EmptyArray);

                work.Should().Throw<ArgumentException>();
            }

            [Fact, LogIfTooSlow]
            public void ThrowsIfReceivedNullArray()
            {
                Func<Task> work = async () => await ViewModel.Initialize(NullArray);

                work.Should().Throw<ArgumentException>();
            }
        }

        public sealed class TheTimeEntryIdsProperty : EditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ContainsCorrectIdsAfterPrepareStep()
            {
                ViewModel.Initialize(TimeEntriesGroupIds);

                ViewModel.TimeEntryIds.Should().BeEquivalentTo(TimeEntriesGroupIds);
            }
        }

        public sealed class TheTimeEntryIdProperty : EditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ContainsCorrectIdAfterPrepareStep()
            {
                ViewModel.Initialize(TimeEntriesGroupIds);

                ViewModel.TimeEntryId.Should().Be(TimeEntriesGroupIds.First());
            }
        }

        public sealed class TheIsEditingGroupProperty : EditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTrueForTimeEntriesGroup()
            {
                ViewModel.Initialize(TimeEntriesGroupIds);

                ViewModel.IsEditingGroup.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForSingleTimeEntry()
            {
                ViewModel.Initialize(SingleTimeEntryId);

                ViewModel.IsEditingGroup.Should().BeFalse();
            }
        }

        public sealed class TheGroupCountProperty : EditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ReturnsCorrectValueForTimeEntriesGroup()
            {
                ViewModel.Initialize(TimeEntriesGroupIds);

                ViewModel.GroupCount.Should().Be(TimeEntriesGroupIds.Length);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsCorrectValueForSingleTimeEntry()
            {
                ViewModel.Initialize(SingleTimeEntryId);

                ViewModel.GroupCount.Should().Be(SingleTimeEntryId.Length);
            }
        }

        public sealed class TheGroupDurationProperty : InitializableEditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsCorrectSumForTimeEntries()
            {
                var expectedDuration = OneHour * (TimeEntriesGroupIds.Length * (TimeEntriesGroupIds.Length + 1) / 2.0);

                int i = 0;
                AdjustTimeEntries(TimeEntriesGroupIds, te =>
                {
                    te.Duration = ++i * (long)OneHour.TotalSeconds;
                    return te;
                });

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                ViewModel.GroupDuration.Should().Be(expectedDuration);
            }
        }

        public sealed class TheDescriptionProperty : InitializableEditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsStringFromLoadedTimeEntry()
            {
                var observer = TestScheduler.CreateObserverFor(ViewModel.Description);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(Description);
            }

            [Fact, LogIfTooSlow]
            public async Task EmitsChangesToRelay()
            {
                var observer = TestScheduler.CreateObserverFor(ViewModel.Description);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();
                ViewModel.Description.Accept(StringSample);

                observer.LastEmittedValue().Should().Be(StringSample);
            }

            [Fact, LogIfTooSlow]
            public async Task TrimsValueOnChange()
            {
                var dirtyString = $" \t{StringSample}  ";
                var observer = TestScheduler.CreateObserverFor(ViewModel.Description);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();
                ViewModel.Description.Accept(dirtyString);

                observer.LastEmittedValue().Should().Be(StringSample);
            }
        }

        public sealed class TheIsSyncErrorMessageVisibleProperty : InitializableEditTimeEntryViewModelTest
        {
            private const string lastSyncError = "This time entry been naughty!";

            [Theory, LogIfTooSlow]
            [InlineData(false, false, false, false)]
            [InlineData(false, false, true, true)]
            [InlineData(false, true, false, true)]
            [InlineData(false, true, true, true)]
            [InlineData(true, false, false, false)]
            [InlineData(true, false, true, true)]
            [InlineData(true, true, false, true)]
            [InlineData(true, true, true, true)]
            public async Task ReturnsExpectedIsSyncErrorMessageVisibleValue(bool isGrouped, bool hasError, bool isInaccessible, bool expectedValue)
            {
                var ids = isGrouped ? TimeEntriesGroupIds : SingleTimeEntryId;
                await ViewModel.Initialize(ids);
                SetupTimeEntries(ids, (te, index) =>
                {
                    te.LastSyncErrorMessage = index == 0 && hasError
                        ? lastSyncError
                        : te.LastSyncErrorMessage;
                    te.Workspace = index == 0 && isInaccessible
                        ? InaccessibleWorkspace
                        : te.Workspace;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.IsSyncErrorMessageVisible);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(expectedValue);
            }
        }

        public sealed class TheIsBillableAvailableProperty : InitializableEditTimeEntryViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async Task ReturnsExpectedIsBillableAvailableValue(bool isBillableAvailable)
            {
                AdjustTimeEntries(TimeEntriesIds, te =>
                {
                    te.WorkspaceId = isBillableAvailable
                        ? WorkspaceIdWithBillableAvailable
                        : WorkspaceIdWithoutBillableAvailable;
                    te.Workspace = isBillableAvailable
                        ? WorkspaceWithBillableAvailable
                        : WorkspaceWithoutBillableAvailable;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.IsBillableAvailable);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(isBillableAvailable);
            }
        }

        public sealed class TheIsBillableProperty : InitializableEditTimeEntryViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(false)]
            [InlineData(true)]
            public async Task ReturnsExpectedValueAfterInitialization(bool isBillable)
            {
                AdjustTimeEntries(TimeEntriesIds, te =>
                {
                    te.Billable = isBillable;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.IsBillable);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(isBillable);
            }
        }

        public sealed class TheStartTimeProperty : InitializableEditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsExpectedValueAfterInitialization()
            {
                var startTime = DateTimeOffset.Now;
                AdjustTimeEntries(TimeEntriesIds, te =>
                {
                    te.Start = startTime;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.StartTime);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(startTime);
            }
        }

        public sealed class TheDurationProperty : InitializableEditTimeEntryViewModelTest
        {
            private readonly TimeSpan oneSecond = TimeSpan.FromSeconds(1);

            protected override void AdditionalSetup()
            {
                base.AdditionalSetup();

                TimeService.CurrentDateTimeObservable.Returns(
                    Observable.Interval(TimeSpan.FromSeconds(1), TestScheduler).Select(n =>
                    {
                        var now = Now + (n + 1) * TimeSpan.FromSeconds(1);
                        TimeService.CurrentDateTime.Returns(now);
                        return now;
                    }));
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsValidDurationForRunningTimeEntryWithoutAnyDelay()
            {
                AdjustTimeEntries(TimeEntriesIds, te =>
                {
                    te.Start = Now - OneHour;
                    te.Duration = null;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.Duration);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.AdvanceBy(TimeSpan.FromSeconds(3).Ticks + 1);

                observer.Messages.AssertEqual(
                    OnNext(1, OneHour),
                    OnNext(oneSecond.Ticks + 1, OneHour + oneSecond),
                    OnNext(2 * oneSecond.Ticks + 1, OneHour + 2 * oneSecond),
                    OnNext(3 * oneSecond.Ticks + 1, OneHour + 3 * oneSecond));
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsValidDurationForStoppedTimeEntry()
            {
                var observer = TestScheduler.CreateObserverFor(ViewModel.Duration);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.AdvanceBy(TimeSpan.FromSeconds(3).Ticks);

                observer.Messages.AssertEqual(OnNext(1, OneHour));
            }

            [Fact, LogIfTooSlow]
            public async Task StopsTickingWhenTheTimeEntryIsStopped()
            {
                AdjustTimeEntries(TimeEntriesIds, te =>
                {
                    te.Start = Now - OneHour;
                    te.Duration = null;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.Duration);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.AdvanceBy(TimeSpan.FromSeconds(3).Ticks);
                ViewModel.StopTimeEntry.Execute();
                TestScheduler.AdvanceBy(TimeSpan.FromSeconds(3).Ticks);

                observer.Messages.AssertEqual(
                    OnNext(1, OneHour),
                    OnNext(oneSecond.Ticks + 1, OneHour + oneSecond),
                    OnNext(2 * oneSecond.Ticks + 1, OneHour + 2 * oneSecond),
                    OnNext(3 * oneSecond.Ticks + 1, OneHour + 3 * oneSecond));
            }
        }

        public sealed class TheStopTimeProperty : InitializableEditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsValidTimeForRunningTimeEntry()
            {
                AdjustTimeEntries(TimeEntriesIds, te =>
                {
                    te.Duration = null;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.StopTime);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(null);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsValidTimeForStoppedTimeEntry()
            {
                AdjustTimeEntries(TimeEntriesIds, te =>
                {
                    te.Start = Now - OneHour;
                    te.Duration = (long)OneHour.TotalSeconds;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.StopTime);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(Now);
            }
        }

        public sealed class TheIsTimeEntryRunningProperty : InitializableEditTimeEntryViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(false)]
            [InlineData(true)]
            public async Task ReturnsTrueForRunningTimeEntry(bool isRunning)
            {
                AdjustTimeEntries(TimeEntriesIds, te =>
                {
                    te.Start = Now - OneHour;
                    te.Duration = isRunning
                        ? (long?)null
                        : (long)OneHour.TotalSeconds;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.IsTimeEntryRunning);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(isRunning);
            }
        }

        public sealed class TheProjectClientTaskProperty : InitializableEditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsExpectedValueAfterInitialization()
            {
                var expectedValue = new ProjectClientTaskInfo(ProjectName, ProjectColor, ClientName, TaskName, false, false);

                var observer = TestScheduler.CreateObserverFor(ViewModel.ProjectClientTask);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(expectedValue);
            }

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async Task HasProjectHasCorrectValueDependingOnProjectBeingSet(bool isProjectSet)
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    if (!isProjectSet)
                    {
                        te.ProjectId = null;
                        te.Project = null;
                        te.Task = null;
                        te.TaskId = null;
                    }
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.ProjectClientTask);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().HasProject.Should().Be(isProjectSet);
            }

            [Fact, LogIfTooSlow]
            public async Task TaskIsNullWhenItIsNotSet()
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    te.Task = null;
                    te.TaskId = null;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.ProjectClientTask);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Task.Should().BeNull();
            }

            [Fact, LogIfTooSlow]
            public async Task ClientIsNullWhenItIsNotSet()
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    te.Project = new MockProject(te.Project.Id, te.Project.Workspace, client: null) { Active = true };
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.ProjectClientTask);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Client.Should().BeNull();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsEmptyProjectClientTaskInfoWhenProjectAndTaskAreNotSet()
            {
                var expectedValue = ProjectClientTaskInfo.Empty;
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    te.Project = new MockProject(te.Project.Id, te.Project.Workspace, client: null) { Active = true };
                    te.Task = null;
                    te.TaskId = null;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.ProjectClientTask);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(expectedValue);
            }
        }

        public sealed class TheTagsProperty : InitializableEditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsExpectedValueAfterInitialization()
            {
                var tagNames = Enumerable.Range(0, TagCount)
                    .Select(id => (long)id)
                    .Select(TagNameFromId)
                    .ToArray();
                var observer = TestScheduler.CreateObserverFor(ViewModel.Tags);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeEquivalentTo(tagNames);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsEllipsizedTagsAccordingToMaxValue()
            {
                var veryLongTagName = "This text is much more than just 30 characters.";
                var expectedTagName = veryLongTagName.Substring(0, EditTimeEntryViewModel.MaxTagLength);
                var expectedTagNames = new[] { $"{expectedTagName}..." };
                AdjustTimeEntries(TimeEntriesIds, te =>
                {
                    var tag = new MockTag(1, te.Workspace) { Name = veryLongTagName };
                    te.Tags = new[] { tag };
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.Tags);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeEquivalentTo(expectedTagNames);
            }
        }

        public sealed class TheCloseWithDefaultResultMethod : InitializableEditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModelIfNothingChanged()
            {
                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheConfirmationDialogIfDescriptionChanges()
            {
                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Description.Accept("Something Else");

                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                await View.Received().ConfirmDestructiveAction(ActionType.DiscardEditingChanges);
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheConfirmationDialogIfProjectChanges()
            {
                long? newProjectId = null, newTaskId = null;
                long newWorkspaceId = -1;
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    newProjectId = te.ProjectId + 1;
                    newTaskId = te.TaskId;
                    newWorkspaceId = te.WorkspaceId;
                    return te;
                });
                var selectProjectParameter = new SelectProjectParameter(newProjectId, newTaskId, newWorkspaceId);
                NavigationService
                    .Navigate<SelectProjectViewModel, SelectProjectParameter, SelectProjectParameter>(Arg.Any<SelectProjectParameter>(), ViewModel.View)
                    .Returns(selectProjectParameter);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectProject.Execute();
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                await View.Received().ConfirmDestructiveAction(ActionType.DiscardEditingChanges);
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheConfirmationDialogIfTaskChanges()
            {
                long? newProjectId = null, newTaskId = null;
                long newWorkspaceId = -1;
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    newProjectId = te.ProjectId;
                    newTaskId = te.TaskId + 1;
                    newWorkspaceId = te.WorkspaceId;
                    return te;
                });
                var selectProjectParameter = new SelectProjectParameter(newProjectId, newTaskId, newWorkspaceId);
                NavigationService
                    .Navigate<SelectProjectViewModel, SelectProjectParameter, SelectProjectParameter>(Arg.Any<SelectProjectParameter>(), ViewModel.View)
                    .Returns(selectProjectParameter);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectProject.Execute();
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                await View.Received().ConfirmDestructiveAction(ActionType.DiscardEditingChanges);
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheConfirmationDialogIfWorkspaceChanges()
            {
                long? newProjectId = null, newTaskId = null;
                long newWorkspaceId = -1;
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    newProjectId = te.ProjectId;
                    newTaskId = te.TaskId;
                    newWorkspaceId = te.WorkspaceId + 1;
                    return te;
                });
                var selectProjectParameter = new SelectProjectParameter(newProjectId, newTaskId, newWorkspaceId);
                NavigationService
                    .Navigate<SelectProjectViewModel, SelectProjectParameter, SelectProjectParameter>(Arg.Any<SelectProjectParameter>(), ViewModel.View)
                    .Returns(selectProjectParameter);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectProject.Execute();
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                await View.Received().ConfirmDestructiveAction(ActionType.DiscardEditingChanges);
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheConfirmationDialogIfStartTimeChanges()
            {
                var newStartTime = default(DateTimeOffset);
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    newStartTime = te.Start - TimeSpan.FromHours(1);
                    return te;
                });
                var newDurationParameter = DurationParameter.WithStartAndDuration(newStartTime, null);
                NavigationService
                    .Navigate<EditDurationViewModel, EditDurationParameters, DurationParameter>(Arg.Any<EditDurationParameters>(), ViewModel.View)
                    .Returns(newDurationParameter);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.EditTimes.Execute(EditViewTapSource.StartTime);
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                await View.Received().ConfirmDestructiveAction(ActionType.DiscardEditingChanges);
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheConfirmationDialogIfDurationChanges()
            {
                var newStartTime = default(DateTimeOffset);
                var newDuration = default(TimeSpan?);
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    newStartTime = te.Start;
                    newDuration = TimeSpan.FromSeconds(te.Duration.Value) + TimeSpan.FromHours(1);
                    return te;
                });
                var newDurationParameter = DurationParameter.WithStartAndDuration(newStartTime, newDuration);
                NavigationService
                    .Navigate<EditDurationViewModel, EditDurationParameters, DurationParameter>(Arg.Any<EditDurationParameters>(), ViewModel.View)
                    .Returns(newDurationParameter);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.EditTimes.Execute(EditViewTapSource.Duration);
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                await View.Received().ConfirmDestructiveAction(ActionType.DiscardEditingChanges);
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheConfirmationDialogIfUserStopsTimeEntry()
            {
                var newStartTime = default(DateTimeOffset);
                var newDuration = default(TimeSpan?);
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    newStartTime = te.Start;
                    newDuration = TimeSpan.FromSeconds(te.Duration.Value) + TimeSpan.FromHours(1);
                    te.Duration = null;
                    return te;
                });

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.StopTimeEntry.Execute();
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                await View.Received().ConfirmDestructiveAction(ActionType.DiscardEditingChanges);
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheConfirmationDialogIfBillableChanges()
            {
                var newBillable = false;
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    newBillable = !te.Billable;
                    return te;
                });

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.ToggleBillable.Execute();
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                await View.Received().ConfirmDestructiveAction(ActionType.DiscardEditingChanges);
            }

            [Fact, LogIfTooSlow]
            public async Task ShowsTheConfirmationDialogIfTagsChange()
            {
                var newTags = new long[0];
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    newTags = te.Tags.Select(tag => tag.Id + 1).ToArray();
                    return te;
                });
                NavigationService
                    .Navigate<SelectTagsViewModel, SelectTagsParameter, long[]>(Arg.Any<SelectTagsParameter>(), ViewModel.View)
                    .Returns(newTags);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();
                ViewModel.SelectTags.Execute();
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                await View.Received().ConfirmDestructiveAction(ActionType.DiscardEditingChanges);
            }

            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewIfUserClicksOnTheDiscardButton()
            {
                View
                    .ConfirmDestructiveAction(ActionType.DiscardEditingChanges)
                    .Returns(Observable.Return(true));

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();
                ViewModel.Description.Accept("This changes the description.");
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTheCloseReasonForSingleTimeEntry()
            {
                View
                    .ConfirmDestructiveAction(ActionType.DiscardEditingChanges)
                    .Returns(Observable.Return(true));

                await ViewModel.Initialize(new[] { 123L });
                TestScheduler.Start();
                ViewModel.Description.Accept("This changes the description.");
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                AnalyticsService.EditViewClosed.Received().Track(EditViewCloseReason.Close);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTheCloseReasonForGroupedTimeEntries()
            {
                View
                    .ConfirmDestructiveAction(ActionType.DiscardEditingChanges)
                    .Returns(Observable.Return(true));

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();
                ViewModel.Description.Accept("This changes the description.");
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                AnalyticsService.EditViewClosed.Received().Track(EditViewCloseReason.GroupClose);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotCloseTheViewIfUserClicksOnTheContinueEditingButton()
            {
                View
                    .ConfirmDestructiveAction(ActionType.DiscardEditingChanges)
                    .Returns(Observable.Return(false));

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();
                ViewModel.Description.Accept("This changes the description.");
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                View.DidNotReceive().Close();
            }
        }

        public sealed class TheStopTimeEntryAction : InitializableEditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task CannotBeExecutedForAStoppedTimeEntry()
            {
                var observer = TestScheduler.CreateObserverFor(ViewModel.StopTimeEntry.Enabled);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async Task CanBeExecutedForARunningTimeEntry()
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    te.Duration = null;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.StopTimeEntry.Enabled);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task SetsTheCurrentTimeAsTheStopTime()
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    te.Duration = null;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.StopTime);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.StopTimeEntry.Execute();
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(Now);
            }

            [Fact, LogIfTooSlow]
            public async Task ClearsTheIsRunningFlag()
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    te.Duration = null;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.IsTimeEntryRunning);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.StopTimeEntry.Execute();
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTimeEntryStoppedEvent()
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    te.Duration = null;
                    return te;
                });
                await ViewModel.Initialize(SingleTimeEntryId);
                ViewModel.StopTimeEntry.Execute();
                TestScheduler.Start();

                AnalyticsService.Received().TimeEntryStopped.Track(TimeEntryStopOrigin.EditView);
            }

            public async Task TracksEditViewTappedEvent()
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    te.Duration = null;
                    return te;
                });
                await ViewModel.Initialize(SingleTimeEntryId);
                ViewModel.StopTimeEntry.Execute();
                TestScheduler.Start();

                AnalyticsService.Received().EditViewTapped.Track(EditViewTapSource.StopTimeLabel);
            }
        }

        public sealed class TheSelectProjectAction : InitializableEditTimeEntryViewModelTest
        {
            private static readonly long selectedProjectId = 1000;
            private static readonly string selectedProjectName = "Project unlike any other";
            private static readonly string selectedProjectColor = "#FF0000";
            private static readonly long selectedClientId = 1001;
            private static readonly string selectedClientName = "Unforgiving client";
            private static readonly long selectedTaskId = 1002;
            private static readonly string selectedTaskName = "Recursing recursions";
            private static readonly long initialWorkspaceId = WorkspaceIdWithBillableAvailable;
            private static readonly string initialWorkspaceName = WorkspaceWithBillableAvailable.Name;
            private static readonly long changedWorkspaceId = initialWorkspaceId + 1;
            private static readonly string changedWorkspaceName = $"Not {initialWorkspaceName}";

            private void prepare(long? projectId = null, bool hasClient = false, long? taskId = null, bool workspaceChanged = false)
            {
                long workspaceId = workspaceChanged ? changedWorkspaceId : initialWorkspaceId;

                var selectedWorkspace = new MockWorkspace(workspaceId)
                {
                    Name = workspaceChanged ? changedWorkspaceName : initialWorkspaceName
                };

                var selectedClient = new MockClient(selectedClientId, selectedWorkspace)
                {
                    Name = selectedClientName
                };

                var selectedProject = new MockProject(selectedProjectId, selectedWorkspace)
                {
                    Name = selectedProjectName,
                    Color = selectedProjectColor,
                    Client = hasClient ? selectedClient : null,
                    ClientId = hasClient ? selectedClientId : (long?)null,
                    Active = true
                };

                var selectedTask = new MockTask(selectedTaskId, selectedWorkspace, selectedProject)
                {
                    Name = selectedTaskName,
                    Active = true
                };

                var parameter = new SelectProjectParameter(projectId, taskId, WorkspaceIdWithBillableAvailable);
                NavigationService
                    .Navigate<SelectProjectViewModel, SelectProjectParameter, SelectProjectParameter>(Arg.Any<SelectProjectParameter>(), ViewModel.View)
                    .Returns(parameter);

                var selectedProjectObservable = projectId.HasValue ? Observable.Return(selectedProject) : null;
                InteractorFactory
                    .GetProjectById(selectedProjectId)
                    .Execute()
                    .Returns(selectedProjectObservable);

                var selectedTaskObservable = taskId.HasValue ? Observable.Return(selectedTask) : null;
                InteractorFactory
                    .GetTaskById(selectedTaskId)
                    .Execute()
                    .Returns(selectedTaskObservable);
            }

            [Fact, LogIfTooSlow]
            public async Task SetsTheOnboardingStorageFlag()
            {
                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectProject.Execute();
                TestScheduler.Start();

                OnboardingStorage.Received().SelectsProject();
            }

            [Fact, LogIfTooSlow]
            public async Task SetsTheProjectNameAndColor()
            {
                prepare(selectedProjectId);
                var observer = TestScheduler.CreateObserverFor(ViewModel.ProjectClientTask);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectProject.Execute();
                TestScheduler.Start();

                observer.LastEmittedValue().Project.Should().Be(selectedProjectName);
                observer.LastEmittedValue().ProjectColor.Should().Be(selectedProjectColor);
            }

            [Fact, LogIfTooSlow]
            public async Task SetsTheClient()
            {
                prepare(selectedProjectId, true);
                var observer = TestScheduler.CreateObserverFor(ViewModel.ProjectClientTask);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectProject.Execute();
                TestScheduler.Start();

                observer.LastEmittedValue().Client.Should().Be(selectedClientName);
            }

            [Fact, LogIfTooSlow]
            public async Task SetsTheTask()
            {
                prepare(selectedProjectId, false, selectedTaskId);
                var observer = TestScheduler.CreateObserverFor(ViewModel.ProjectClientTask);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectProject.Execute();
                TestScheduler.Start();

                observer.LastEmittedValue().Task.Should().Be(selectedTaskName);
            }

            [Fact, LogIfTooSlow]
            public async Task RemovesTheTaskIfNoTaskWasSelected()
            {
                prepare(selectedProjectId);
                var observer = TestScheduler.CreateObserverFor(ViewModel.ProjectClientTask);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectProject.Execute();
                TestScheduler.Start();

                observer.LastEmittedValue().Task.Should().BeNull();
            }

            [Fact, LogIfTooSlow]
            public async Task RemovesTagsIfProjectFromAnotherWorkspaceWasSelected()
            {
                prepare(selectedProjectId, false, null, true);
                var observer = TestScheduler.CreateObserverFor(ViewModel.Tags);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectProject.Execute();
                TestScheduler.Start();

                observer.LastEmittedValue().Should().HaveCount(0);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksProjectSelectorOpens()
            {
                prepare(selectedProjectId);
                var observer = TestScheduler.CreateObserverFor(ViewModel.Tags);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectProject.Execute();
                TestScheduler.Start();

                AnalyticsService.Received().EditEntrySelectProject.Track();
                AnalyticsService.Received().EditViewTapped.Track(Arg.Is(EditViewTapSource.Project));
            }
        }

        public sealed class TheSelectTagsAction : InitializableEditTimeEntryViewModelTest
        {
            private void prepareInteractorAndNavigationResults(long[] tagsIds = null/*, long[] selectedTagsIds = null*/)
            {
                tagsIds = tagsIds ?? Enumerable.Range(0, TagCount).Select(id => (long)id).ToArray();

                var tags = tagsIds
                    .Select(id => TagFromId(id, WorkspaceWithBillableAvailable))
                    .ToArray();

                var tagsObservable = Observable.Return(tags);

                InteractorFactory
                    .GetMultipleTagsById(Arg.Any<long[]>())
                    .Execute()
                    .Returns(tagsObservable);

                NavigationService
                    .Navigate<SelectTagsViewModel, SelectTagsParameter, long[]>(Arg.Any<SelectTagsParameter>(), ViewModel.View)
                    .Returns(tagsIds);
            }

            [Property]
            public void NavigatesToTheSelectTagsViewModelPassingCurrentTagIds(NonNegativeInt[] nonNegativeInts)
            {
                var tagIds = nonNegativeInts.Select(i => (long)i.Get).Distinct().ToArray();
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    te.TagIds = tagIds;
                    te.Tags = tagIds.Select(tagId => TagFromId(tagId, te.Workspace));
                    return te;
                });
                prepareInteractorAndNavigationResults(tagIds);

                ViewModel.Initialize(TimeEntriesGroupIds).Wait();
                ViewModel.SelectTags.Execute();
                TestScheduler.Start();

                NavigationService
                    .Received()
                    .Navigate<SelectTagsViewModel, SelectTagsParameter, long[]>(
                        Arg.Is<SelectTagsParameter>(tuple => tuple.TagIds.SetEquals(tagIds, null)),
                        ViewModel.View)
                    .Wait();
            }

            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheSelectTagsViewModelPassingWorkspaceId()
            {
                var workspaceId = 0L;
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    workspaceId = te.WorkspaceId;
                    return te;
                });
                prepareInteractorAndNavigationResults();

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectTags.Execute();
                TestScheduler.Start();

                await NavigationService
                    .Received()
                    .Navigate<SelectTagsViewModel, SelectTagsParameter, long[]>(
                        Arg.Is<SelectTagsParameter>(tuple => tuple.WorkspaceId == workspaceId),
                        ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public async Task SetsTheReturnedTags()
            {
                var originalTags = new long[0];
                var expectedTags = new string[0];
                var expectedTagsIds = new long[0];
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    originalTags = te.Tags.Select(tag => tag.Id).ToArray();
                    expectedTags = te.Tags.Take(2).Select(tag => tag.Name).ToArray();
                    expectedTagsIds = te.TagIds.Take(2).ToArray();
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.Tags);
                prepareInteractorAndNavigationResults(expectedTagsIds);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectTags.Execute();
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeEquivalentTo(expectedTags);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTagSelectorOpens()
            {
                prepareInteractorAndNavigationResults();

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectTags.Execute();
                TestScheduler.Start();

                AnalyticsService.Received().EditEntrySelectTag.Track();
                AnalyticsService.Received().EditViewTapped.Track(Arg.Is(EditViewTapSource.Tags));
            }

        }

        public sealed class TheToggleBillableAction : InitializableEditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task TracksBillableTap()
            {
                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.ToggleBillable.Execute();
                TestScheduler.Start();

                AnalyticsService.Received()
                                .EditViewTapped
                                .Track(Arg.Is(EditViewTapSource.Billable));
            }

            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async Task InvertsTheIsBillableProperty(bool initialValue)
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    te.Billable = initialValue;
                    return te;
                });
                var observable = TestScheduler.CreateObserverFor(ViewModel.IsBillable);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.ToggleBillable.Execute();
                TestScheduler.Start();

                observable.LastEmittedValue().Should().Be(!initialValue);
            }
        }

        public sealed class TheDismissSyncErrorMessageCommand : InitializableEditTimeEntryViewModelTest
        {
            [Theory, LogIfTooSlow]
            [InlineData(true)]
            [InlineData(false)]
            public async Task SetsIsSyncErrorMessageVisibleToFalse(bool hasError)
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    te.LastSyncErrorMessage = hasError ? "Bad time entry" : null;
                    return te;
                });
                var observer = TestScheduler.CreateObserverFor(ViewModel.IsSyncErrorMessageVisible);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.DismissSyncErrorMessage.Execute();
                TestScheduler.Start();

                observer.LastEmittedValue().Should().BeFalse();
            }
        }

        public sealed class TheEditTimesAction : InitializableEditTimeEntryViewModelTest
        {
            private DateTimeOffset selectedStartTime = Now - TimeSpan.FromDays(4);
            private TimeSpan? selectedDuration = TimeSpan.FromMinutes(28);

            private void setupNavigation(DateTimeOffset start, TimeSpan? duration)
            {
                var durationParameter = DurationParameter.WithStartAndDuration(start, duration);
                NavigationService
                    .Navigate<EditDurationViewModel, EditDurationParameters, DurationParameter>(Arg.Any<EditDurationParameters>(), ViewModel.View)
                    .Returns(durationParameter);
            }

            [Theory, LogIfTooSlow]
            [InlineData(EditViewTapSource.StartTime)]
            [InlineData(EditViewTapSource.StartDate)]
            [InlineData(EditViewTapSource.StopTime)]
            [InlineData(EditViewTapSource.Duration)]
            public async Task TracksCorrectTapSource(EditViewTapSource tapSource)
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    setupNavigation(te.Start, te.TimeSpanDuration());
                    return te;
                });

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.EditTimes.Execute(tapSource);
                TestScheduler.Start();

                AnalyticsService.Received().EditViewTapped.Track(tapSource);
            }

            [Fact, LogIfTooSlow]
            public async Task NavigatesToEditDurationViewModel()
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    setupNavigation(te.Start, te.TimeSpanDuration());
                    return te;
                });

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.EditTimes.Execute(EditViewTapSource.StartTime);
                TestScheduler.Start();

                await NavigationService
                    .Received()
                    .Navigate<EditDurationViewModel, EditDurationParameters, DurationParameter>(Arg.Any<EditDurationParameters>(), ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public async Task SetsDurationToBeInitiallyFocusedIfDurationWasTapped()
            {
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    setupNavigation(te.Start, te.TimeSpanDuration());
                    return te;
                });

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.EditTimes.Execute(EditViewTapSource.Duration);
                TestScheduler.Start();

                await NavigationService
                    .Received()
                    .Navigate<EditDurationViewModel, EditDurationParameters, DurationParameter>(
                        Arg.Is<EditDurationParameters>(parameter => parameter.IsDurationInitiallyFocused), ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public async Task PassesCorrectStartTimeAndDurationToViewModel()
            {
                var startTime = default(DateTimeOffset);
                TimeSpan? duration = null;
                AdjustTimeEntries(SingleTimeEntryId, te =>
                {
                    startTime = te.Start;
                    duration = te.TimeSpanDuration();
                    setupNavigation(te.Start, te.TimeSpanDuration());
                    return te;
                });

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.EditTimes.Execute(EditViewTapSource.Duration);
                TestScheduler.Start();

                await NavigationService
                    .Received()
                    .Navigate<EditDurationViewModel, EditDurationParameters, DurationParameter>(
                        Arg.Is<EditDurationParameters>(parameter =>
                            parameter.DurationParam.Start == startTime
                            && parameter.DurationParam.Duration == duration), ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesStartTimeAfterTimeSelection()
            {
                setupNavigation(selectedStartTime, selectedDuration);
                var observer = TestScheduler.CreateObserverFor(ViewModel.StartTime);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.EditTimes.Execute(EditViewTapSource.Duration);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(selectedStartTime);
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesDurationAfterTimeSelection()
            {
                setupNavigation(selectedStartTime, selectedDuration);
                var observer = TestScheduler.CreateObserverFor(ViewModel.Duration);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.EditTimes.Execute(EditViewTapSource.Duration);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(selectedDuration.Value);
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesStopTimeAfterTimeSelection()
            {
                var expectedStopTime = selectedStartTime + selectedDuration.Value;
                setupNavigation(selectedStartTime, selectedDuration);
                var observer = TestScheduler.CreateObserverFor(ViewModel.StopTime);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.EditTimes.Execute(EditViewTapSource.Duration);
                TestScheduler.Start();

                observer.LastEmittedValue().Should().Be(expectedStopTime);
            }
        }

        public sealed class TheSelectStartDateAction : InitializableEditTimeEntryViewModelTest
        {
            private MockTimeEntry entry => Entries.Single();

            [Fact]
            public async Task OpensTheSelectDateTimeViewModel()
            {
                ViewModel.Initialize(SingleTimeEntryId);
                await ViewModel.Initialize(TimeEntriesGroupIds);

                ViewModel.SelectStartDate.Execute();
                TestScheduler.Start();

                await NavigationService.Received()
                    .Navigate<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>(
                        Arg.Any<DateTimePickerParameters>(),
                        ViewModel.View);
            }

            [Fact]
            public async Task OpensTheSelectDateTimeViewModelWithCorrectLimitsForARunningTimeEntry()
            {
                entry.Duration = null;

                ViewModel.Initialize(SingleTimeEntryId);
                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectStartDate.Execute();
                TestScheduler.Start();

                await NavigationService.Received()
                    .Navigate<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>(
                        Arg.Is<DateTimePickerParameters>(param => param.MinDate == Now - MaxTimeEntryDuration && param.MaxDate == Now),
                        ViewModel.View);
            }

            [Fact]
            public async Task OpensTheSelectDateTimeViewModelWithCorrectLimitsForAStoppedTimeEntry()
            {
                entry.Duration = 123;

                ViewModel.Initialize(SingleTimeEntryId);
                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectStartDate.Execute();
                TestScheduler.Start();

                await NavigationService.Received()
                    .Navigate<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>(
                        Arg.Is<DateTimePickerParameters>(param => param.MinDate == EarliestAllowedStartTime && param.MaxDate == LatestAllowedStartTime),
                        ViewModel.View);
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async Task ChangesTheStartTimeToTheSelectedStartDate(bool isRunning)
            {
                var startTime = Now.AddMonths(-1);
                entry.Duration = isRunning ? (long?)null : 2 * 60;
                NavigationService
                    .Navigate<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>(Arg.Any<DateTimePickerParameters>(), ViewModel.View)
                    .Returns(startTime);

                ViewModel.Initialize(SingleTimeEntryId);
                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.SelectStartDate.Execute();
                TestScheduler.Start();

                entry.Start.Should().NotBe(startTime);
            }

            [Fact]
            public async Task DoesNotChangeDurationForAStoppedTimeEntry()
            {
                entry.Duration = 2 * 60;
                NavigationService
                    .Navigate<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>(Arg.Any<DateTimePickerParameters>(), ViewModel.View)
                    .Returns(entry.Start - TimeSpan.FromDays(1));

                ViewModel.Initialize(SingleTimeEntryId);
                await ViewModel.Initialize(TimeEntriesGroupIds);
                var durationObserver = TestScheduler.CreateObserver<TimeSpan>();
                ViewModel.Duration.Subscribe(durationObserver);
                ViewModel.SelectStartDate.Execute();
                TestScheduler.Start();

                durationObserver.LastEmittedValue().Should().Be(TimeSpan.FromSeconds(entry.Duration.Value));
            }

            [Fact, LogIfTooSlow]
            public async Task TracksStartDateTap()
            {
                var newStartTime = entry.Start.AddHours(1);
                NavigationService
                    .Navigate<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>(
                        Arg.Any<DateTimePickerParameters>(), ViewModel.View)
                    .Returns(newStartTime);

                ViewModel.Initialize(SingleTimeEntryId);
                ViewModel.Initialize(TimeEntriesGroupIds).Wait();
                ViewModel.SelectStartDate.Execute();
                TestScheduler.Start();

                AnalyticsService.Received()
                    .EditViewTapped
                    .Track(Arg.Is(EditViewTapSource.StartDate));
            }
        }


        public sealed class TheDeleteAction : InitializableEditTimeEntryViewModelTest
        {
            public TheDeleteAction()
            {
                var trueObservable = Observable.Return(true);
                View
                    .ConfirmDestructiveAction(Arg.Any<ActionType>(), Arg.Any<object>())
                    .Returns(trueObservable);

                var unitObservable = Task.FromResult(Unit.Default);
                InteractorFactory
                    .DeleteTimeEntry(Arg.Any<long>())
                    .Execute()
                    .Returns(unitObservable);

                InteractorFactory
                    .DeleteTimeEntry(Arg.Any<long>())
                    .Execute()
                    .Returns(unitObservable);
            }

            [Fact, LogIfTooSlow]
            public async Task AsksForDestructiveActionConfirmationForSingleTimeEntry()
            {
                await ViewModel.Initialize(new[] { 1L });

                ViewModel.Delete.Execute();
                TestScheduler.Start();

                await View.Received().ConfirmDestructiveAction(
                    ActionType.DeleteExistingTimeEntry, 1);
            }

            [Fact, LogIfTooSlow]
            public async Task AsksForDestructiveActionConfirmationForTimeEntriesGroup()
            {
                AdjustTimeEntries(TimeEntriesGroupIds, te => te);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Delete.Execute();
                TestScheduler.Start();

                await View.Received().ConfirmDestructiveAction(
                    ActionType.DeleteMultipleExistingTimeEntries, TimeEntriesGroupIds.Length);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotDeleteIfCancelledSingleTimeEntry()
            {
                var falseObservable = Observable.Return(false);
                View
                    .ConfirmDestructiveAction(Arg.Any<ActionType>(), Arg.Any<object>())
                    .Returns(falseObservable);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Delete.Execute();
                TestScheduler.Start();

                await InteractorFactory.DeleteTimeEntry(Arg.Any<long>()).DidNotReceive().Execute();
                await InteractorFactory.DeleteMultipleTimeEntries(Arg.Any<long[]>()).DidNotReceive().Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotDeleteIfCancelledTimeEntriesGroup()
            {
                var falseObservable = Observable.Return(false);
                View
                    .ConfirmDestructiveAction(Arg.Any<ActionType>())
                    .Returns(falseObservable);
                AdjustTimeEntries(TimeEntriesGroupIds, te => te);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Delete.Execute();
                TestScheduler.Start();

                await InteractorFactory.DeleteTimeEntry(Arg.Any<long>()).DidNotReceive().Execute();
                await InteractorFactory.DeleteMultipleTimeEntries(Arg.Any<long[]>()).DidNotReceive().Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task InitiatesPushSyncOnDeleteConfirmationForSingleTimeEntry()
            {
                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Delete.Execute();
                TestScheduler.Start();

                SyncManager.Received().InitiatePushSync();
            }

            [Fact, LogIfTooSlow]
            public async Task InitiatesPushSyncOnDeleteConfirmationForTimeEntriesGroup()
            {
                AdjustTimeEntries(TimeEntriesGroupIds, te => te);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Delete.Execute();
                TestScheduler.Start();

                SyncManager.Received().InitiatePushSync();
            }

            [Fact, LogIfTooSlow]
            public async Task ClosesViewModelAfterDeletionForSingleTimeEntry()
            {
                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Delete.Execute();
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTheCloseReasonForSingleTimeEntry()
            {
                await ViewModel.Initialize(new[] { 123L });
                ViewModel.Delete.Execute();
                TestScheduler.Start();

                AnalyticsService.EditViewClosed.Received().Track(EditViewCloseReason.Delete);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTheCloseReasonForGroupedTimeEntries()
            {
                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Delete.Execute();
                TestScheduler.Start();

                AnalyticsService.EditViewClosed.Received().Track(EditViewCloseReason.GroupDelete);
            }

            [Fact, LogIfTooSlow]
            public async Task ClosesViewModelAfterDeletionForTimeEntriesGroup()
            {
                AdjustTimeEntries(TimeEntriesGroupIds, te => te);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Delete.Execute();
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task TracksDeletionOfSingleTimeEntryUsingTheAnaltyticsService()
            {
                await ViewModel.Initialize(new[] { 123L });
                ViewModel.Delete.Execute();
                TestScheduler.Start();

                AnalyticsService.DeleteTimeEntry.Received().Track(DeleteTimeEntryOrigin.EditView);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksDeletionOfTimeEntriesGroupUsingTheAnaltyticsService()
            {
                AdjustTimeEntries(TimeEntriesGroupIds, te => te);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Delete.Execute();
                TestScheduler.Start();

                AnalyticsService.DeleteTimeEntry.Received().Track(DeleteTimeEntryOrigin.GroupedEditView);
            }
        }

        public sealed class TheSaveActionForMultipleTimeEntries : InitializableEditTimeEntryViewModelTest
        {
            private static readonly long[] ids = { 1, 2, 3 };

            public TheSaveActionForMultipleTimeEntries()
                : base(ids)
            {
            }

            [Fact, LogIfTooSlow]
            public async Task PreservesStartAndEndTimesWhenEditingAGroupOfTimeEntries()
            {
                NavigationService
                    .Navigate<EditDurationViewModel, EditDurationParameters, DurationParameter>(Arg.Any<EditDurationParameters>(), ViewModel.View)
                    .Returns(new DurationParameter { Start = new DateTimeOffset(), Duration = TimeSpan.FromDays(365) });
                var interactor = SetupUpdateInteractor(Entries);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.EditTimes.Execute(EditViewTapSource.StartDate);
                ViewModel.Save.Execute();
                TestScheduler.Start();

                InteractorFactory
                   .Received()
                   .UpdateMultipleTimeEntries(Arg.Is<EditTimeEntryDto[]>(dtos => DtosEqualTimeEntries(dtos, Entries)));
                await interactor.Received().Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTheCloseReasonWhenSomethingChanged()
            {
                await ViewModel.Initialize(ids);
                ViewModel.Description.Accept("Hello there");
                ViewModel.Save.Execute();
                TestScheduler.Start();

                AnalyticsService.EditViewClosed.Received().Track(EditViewCloseReason.GroupSave);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTheCloseReasonWhenNothingChanged()
            {
                await ViewModel.Initialize(ids);
                ViewModel.Save.Execute();
                TestScheduler.Start();

                AnalyticsService.EditViewClosed.Received().Track(EditViewCloseReason.GroupSaveWithoutChange);
            }
        }

        public sealed class TheSaveAction : InitializableEditTimeEntryViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task SetsTheOnboardingStorageFlag()
            {
                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Save.Execute();
                TestScheduler.Start();

                OnboardingStorage.Received().EditedTimeEntry();
            }

            [Fact, LogIfTooSlow]
            public async Task CallsInteractorWithValidDtoForSingleTimeEntry()
            {
                var interactor = SetupUpdateInteractor(Entries);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Save.Execute();
                TestScheduler.Start();

                InteractorFactory
                   .Received()
                   .UpdateMultipleTimeEntries(Arg.Is<EditTimeEntryDto[]>(dtos => DtosEqualTimeEntries(dtos, Entries)));
                await interactor.Received().Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task AllowsEditingStartAndEndTimesWhenEditingASingleTimeEntry()
            {
                var timeEntry = Entries.Single();
                var interactor = SetupUpdateInteractor(new[] { timeEntry });

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.StopTimeEntry.Execute();
                ViewModel.Save.Execute();
                TestScheduler.Start();

                InteractorFactory
                    .Received()
                    .UpdateMultipleTimeEntries(Arg.Is<EditTimeEntryDto[]>(dtos => DtoEqualsTimeEntry(dtos.First(), timeEntry)));
                await interactor.Received().Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task CallsInteractorWithValidDtoForTimeEntriesGroup()
            {
                AdjustTimeEntries(TimeEntriesGroupIds, te => te);
                var interactor = SetupUpdateInteractor(Entries);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Save.Execute();
                TestScheduler.Start();

                InteractorFactory
                   .Received()
                   .UpdateMultipleTimeEntries(Arg.Is<EditTimeEntryDto[]>(dtos => DtosEqualTimeEntries(dtos, Entries)));
                await interactor.Received().Execute();
            }

            [Fact, LogIfTooSlow]
            public async Task ClosesAfterSuccessfulSave()
            {
                SetupUpdateInteractor(Entries);

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Save.Execute();
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task ClosesEvenAfterFailedSave()
            {
                var interactor = SetupUpdateInteractor(Entries);
                interactor.Execute()
                    .Returns(Observable.Throw<IEnumerable<IThreadSafeTimeEntry>>(new Exception()));

                await ViewModel.Initialize(TimeEntriesGroupIds);
                ViewModel.Save.Execute();
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTheCloseReasonWhenSomethingChanged()
            {
                await ViewModel.Initialize(new[] { 123L });
                ViewModel.Description.Accept("Hello there");
                ViewModel.Save.Execute();
                TestScheduler.Start();

                AnalyticsService.EditViewClosed.Received().Track(EditViewCloseReason.Save);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksTheCloseReasonWhenNothingChanged()
            {
                await ViewModel.Initialize(new[] { 123L });
                ViewModel.Save.Execute();
                TestScheduler.Start();

                AnalyticsService.EditViewClosed.Received().Track(EditViewCloseReason.SaveWithoutChange);
            }
        }
    }
}
