using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Analytics;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;
using Xunit;
using static Toggl.Shared.BeginningOfWeek;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class EditDurationViewModelTests
    {
        public abstract class EditDurationViewModelTest : BaseViewModelTests<EditDurationViewModel, EditDurationParameters, DurationParameter>
        {
            protected override EditDurationViewModel CreateViewModel()
                => new EditDurationViewModel(NavigationService, TimeService, DataSource, AnalyticsService, RxActionFactory, SchedulerProvider);
        }

        public sealed class TheConstructor : EditDurationViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useNavigationService, bool useTimeService, bool useDataSource, bool useAnalyticsService, bool useRxActionFactory, bool useSchedulerProvider)
            {
                var navigationService = useNavigationService ? NavigationService : null;
                var timeService = useTimeService ? TimeService : null;
                var dataSource = useDataSource ? DataSource : null;
                var analyticsService = useAnalyticsService ? AnalyticsService : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new EditDurationViewModel(navigationService, timeService, dataSource, analyticsService, rxActionFactory, schedulerProvider);

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }

        }

        public sealed class TheDurationProperty : EditDurationViewModelTest
        {
            [Property]
            public void WhenChangedWhileUpdatingTheRunningTimeEntryTriggersTheUpdateOfTheStartTime(DateTimeOffset now)
            {
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndDuration(start, null);
                TimeService.CurrentDateTime.Returns(now);
                var observer = TestScheduler.CreateObserver<DateTimeOffset>();
                ViewModel.StartTime.Subscribe(observer);

                ViewModel.Initialize(new EditDurationParameters(parameter));

                ViewModel.ChangeDuration.Execute(TimeSpan.FromHours(4));

                TestScheduler.Start();
                var expectedStart = start.AddHours(-2);
                observer.LastEmittedValue().Should().BeSameDateAs(expectedStart);
            }

            [Property]
            public void WhenChangedWhileUpdatingFinishedTimeEntryTriggersTheUpdateOfTheStopTime(DateTimeOffset now)
            {
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndDuration(start, now - start);
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var observer = TestScheduler.CreateObserver<DateTimeOffset>();
                ViewModel.StopTime.Subscribe(observer);

                ViewModel.ChangeDuration.Execute(TimeSpan.FromHours(4));

                TestScheduler.Start();
                var expectedStop = now.AddHours(2);
                observer.LastEmittedValue().Should().BeSameDateAs(expectedStop);
            }

            [Property]
            public void IsUpdatedAccordingToTimeServiceForRunningTimeEntries(DateTimeOffset now)
            {
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndDuration(start, null);
                var tickSubject = new Subject<DateTimeOffset>();
                var tickObservable = tickSubject.AsObservable().Publish();
                var observer = TestScheduler.CreateObserver<TimeSpan>();
                ViewModel.Duration.Subscribe(observer);
                tickObservable.Connect();
                TimeService.CurrentDateTimeObservable.Returns(tickObservable);
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.Initialize(new EditDurationParameters(parameter));

                tickSubject.OnNext(now.AddHours(2));

                TestScheduler.Start();
                observer.LastEmittedValue().Hours.Should().Be(4);
            }
        }

        public sealed class TheBeginningOfWeekProperty : EditDurationViewModelTest
        {
            [Theory]
            [InlineData(Sunday)]
            [InlineData(Monday)]
            [InlineData(Tuesday)]
            [InlineData(Wednesday)]
            [InlineData(Thursday)]
            [InlineData(Friday)]
            [InlineData(Saturday)]
            public async Task CorrespondsToSettings(BeginningOfWeek beginningOfWeek)
            {
                System.Diagnostics.Debug.WriteLine(beginningOfWeek);
                var now = new DateTimeOffset(2019, 1, 1, 10, 12, 14, TimeSpan.Zero);
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndDuration(start, null);
                TimeService.CurrentDateTime.Returns(now);
                var user = Substitute.For<IThreadSafeUser>();
                user.BeginningOfWeek.Returns(beginningOfWeek);
                user.Id.Returns(123456);
                DataSource.User.Current.Returns(Observable.Return(user));
                var viewModel = CreateViewModel();

                await viewModel.Initialize(new EditDurationParameters(parameter));

                TestScheduler.Start();
                viewModel.BeginningOfWeek.Should().Be(beginningOfWeek);
            }
        }

        public sealed class TheDurationTimeProperty : EditDurationViewModelTest
        {
            [Property]
            public void IsUpdatedAccordingToTimeServiceForRunningTimeEntries(DateTimeOffset now, byte hours)
            {
                var duration = TimeSpan.FromHours(hours);
                var parameter = DurationParameter.WithStartAndDuration(now, null);
                var tickSubject = new Subject<DateTimeOffset>();
                var tickObservable = tickSubject.AsObservable().Publish();
                tickObservable.Connect();
                TimeService.CurrentDateTimeObservable.Returns(tickObservable);
                TimeService.CurrentDateTime.Returns(now);
                var durationObserver = TestScheduler.CreateObserver<TimeSpan>();
                ViewModel.Duration.Subscribe(durationObserver);

                ViewModel.Initialize(new EditDurationParameters(parameter));

                var newCurrentTime = now + duration;
                tickSubject.OnNext(newCurrentTime);

                TestScheduler.Start();
                durationObserver.LastEmittedValue().Should().Be(duration);
            }
        }

        public sealed class ThePrepareMethod : EditDurationViewModelTest
        {
            [Property]
            public void SetsTheStartTime(DateTimeOffset now)
            {
                var start = now;
                var parameter = DurationParameter.WithStartAndDuration(start, null);
                var startObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                ViewModel.StartTime.Subscribe(startObserver);

                ViewModel.Initialize(new EditDurationParameters(parameter));

                TestScheduler.Start();
                startObserver.LastEmittedValue().Should().Be(start);
            }

            [Property]
            public void SetsTheStartTimeToCurrentTimeIfParameterDoesNotHaveStartTime(DateTimeOffset now)
            {
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndDuration(start, null);
                TimeService.CurrentDateTime.Returns(now);
                var observer = TestScheduler.CreateObserver<DateTimeOffset>();
                ViewModel.StartTime.Subscribe(observer);

                ViewModel.Initialize(new EditDurationParameters(parameter));

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeSameDateAs(start);
            }

            [Property]
            public void SetsTheStopTimeToParameterStopTimeIfParameterHasStopTime(DateTimeOffset now)
            {
                var start = now.AddHours(-4);
                var stop = start.AddHours(2);
                var parameter = DurationParameter.WithStartAndDuration(start, stop - now);
                var observer = TestScheduler.CreateObserver<DateTimeOffset>();
                ViewModel.StartTime.Subscribe(observer);

                ViewModel.Initialize(new EditDurationParameters(parameter));

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeSameDateAs(start);
            }

            [Property]
            public void SubscribesToCurrentTimeObservableIfParameterDoesNotHaveStopTime(DateTimeOffset now)
            {
                var parameter = DurationParameter.WithStartAndDuration(now, null);
                TimeService.CurrentDateTimeObservable.Returns(Substitute.For<IObservable<DateTimeOffset>>());
                ViewModel.Initialize(new EditDurationParameters(parameter));

                TimeService.CurrentDateTimeObservable.Received().Subscribe(Arg.Any<AnonymousObserver<DateTimeOffset>>());
            }

            [Fact, LogIfTooSlow]
            public void SetsTheIsRunningPropertyWhenTheDurationIsNull()
            {
                var start = new DateTimeOffset(2018, 01, 15, 12, 34, 56, TimeSpan.Zero);
                var parameter = DurationParameter.WithStartAndDuration(start, null);
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsRunning.Subscribe(observer);

                ViewModel.Initialize(new EditDurationParameters(parameter));

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotSetTheIsRunningPropertyWhenTheDurationIsNotNull()
            {
                var start = new DateTimeOffset(2018, 01, 15, 12, 34, 56, TimeSpan.Zero);
                var duration = TimeSpan.FromMinutes(20);
                var parameter = DurationParameter.WithStartAndDuration(start, duration);
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsRunning.Subscribe(observer);

                ViewModel.Initialize(new EditDurationParameters(parameter));

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeFalse();
            }
        }

        public sealed class TheCloseWithDefaultResultMethod : EditDurationViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ClosesTheViewModel()
            {
                var parameter = DurationParameter.WithStartAndDuration(DateTimeOffset.UtcNow, null);
                ViewModel.Initialize(new EditDurationParameters(parameter));

                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheDefaultParameter()
            {
                var parameter = DurationParameter.WithStartAndDuration(DateTimeOffset.UtcNow, null);
                await ViewModel.Initialize(new EditDurationParameters(parameter));

                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                (await ViewModel.Result).Should().Be(parameter);
            }
        }

        public sealed class TheSaveCommand : EditDurationViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                var parameter = DurationParameter.WithStartAndDuration(DateTimeOffset.UtcNow, null);
                ViewModel.Initialize(new EditDurationParameters(parameter));

                ViewModel.Save.Execute();

                TestScheduler.Start();
                View.Received().Close();
            }

            [Property]
            public void ReturnsAValueThatReflectsTheChangesToDurationForFinishedTimeEntries(DateTimeOffset start, DateTimeOffset stop)
            {
                if (start >= stop) return;

                var now = DateTimeOffset.UtcNow;
                TimeService.CurrentDateTime.Returns(now);
                if (start >= now) return;

                var durationObserver = TestScheduler.CreateObserver<TimeSpan>();
                var startObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var viewModel = CreateViewModel();
                viewModel.AttachView(View);
                viewModel.Duration.Subscribe(durationObserver);
                viewModel.StartTime.Subscribe(startObserver);

                viewModel.Initialize(new EditDurationParameters(DurationParameter.WithStartAndDuration(start, stop - start)));
                viewModel.ChangeDuration.Execute(TimeSpan.FromMinutes(10));

                viewModel.Save.Execute();

                TestScheduler.Start();
                var result = viewModel.Result.GetAwaiter().GetResult();
                result.Start.Should().Be(startObserver.LastEmittedValue());
                result.Duration.Should().Be(durationObserver.LastEmittedValue());
            }

            [Property]
            public void ReturnsAValueThatReflectsTheChangesToDurationForRunningTimeEntries(DateTimeOffset start, DateTimeOffset now)
            {
                if (start > now) return;
                TimeService.CurrentDateTime.Returns(now);
                var startObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var viewModel = CreateViewModel();
                viewModel.AttachView(View);
                viewModel.StartTime.Subscribe(startObserver);

                viewModel.Initialize(new EditDurationParameters(DurationParameter.WithStartAndDuration(start, null)));
                viewModel.ChangeDuration.Execute(TimeSpan.FromMinutes(10));

                viewModel.Save.Execute();

                TestScheduler.Start();
                var result = viewModel.Result.GetAwaiter().GetResult();
                result.Start.Should().Be(startObserver.LastEmittedValue());
                result.Duration.Should().BeNull();
            }
        }

        public sealed class TheEditStartTimeCommand : EditDurationViewModelTest
        {
            private static DurationParameter parameter = DurationParameter.WithStartAndDuration(
                new DateTimeOffset(2018, 01, 13, 0, 0, 0, TimeSpan.Zero),
                TimeSpan.FromMinutes(7));

            [Fact]
            public void SetsTheIsEditingFlagsCorrectlyWhenNothingWasEdited()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var startObserver = TestScheduler.CreateObserver<bool>();
                var stopObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEditingStartTime.Subscribe(startObserver);
                ViewModel.IsEditingStopTime.Subscribe(stopObserver);

                ViewModel.EditStartTime.Execute();

                TestScheduler.Start();
                startObserver.LastEmittedValue().Should().BeTrue();
                stopObserver.LastEmittedValue().Should().BeFalse();
            }

            [Fact]
            public void SetsTheIsEditingFlagsCorrectlyWhenStopTimeWasEdited()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var startObserver = TestScheduler.CreateObserver<bool>();
                var stopObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEditingStartTime.Subscribe(startObserver);
                ViewModel.IsEditingStopTime.Subscribe(stopObserver);
                ViewModel.EditStopTime.Execute();

                ViewModel.EditStartTime.Execute();

                TestScheduler.Start();
                startObserver.LastEmittedValue().Should().BeTrue();
                stopObserver.LastEmittedValue().Should().BeFalse();
            }

            [Fact]
            public void ClosesEditingWhenStartTimeWasBeingEdited()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var startObserver = TestScheduler.CreateObserver<bool>();
                var stopObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEditingStartTime.Subscribe(startObserver);
                ViewModel.IsEditingStopTime.Subscribe(stopObserver);

                ViewModel.EditStartTime.ExecuteSequentally(times: 2)
                    .Subscribe();

                TestScheduler.Start();
                startObserver.LastEmittedValue().Should().BeFalse();
                stopObserver.LastEmittedValue().Should().BeFalse();
            }

            [Fact]
            public void SetsTheMinimumAndMaximumDateForTheDatePicker()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var minTimeObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var maxTimeObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                ViewModel.MinimumDateTime.Subscribe(minTimeObserver);
                ViewModel.MaximumDateTime.Subscribe(maxTimeObserver);

                ViewModel.EditStartTime.Execute();

                TestScheduler.Start();
                minTimeObserver.LastEmittedValue().Should().Be((parameter.Start + parameter.Duration.Value - TimeSpan.FromHours(999)));
                maxTimeObserver.LastEmittedValue().Should().Be((parameter.Start + parameter.Duration.Value));
            }
        }

        public sealed class TheStopTimeEntryCommand : EditDurationViewModelTest
        {
            private static DurationParameter parameter = DurationParameter.WithStartAndDuration(
                 new DateTimeOffset(2018, 01, 13, 0, 0, 0, TimeSpan.Zero),
                 TimeSpan.FromMinutes(7));

            [Fact]
            public void StopsARunningTimeEntry()
            {
                var now = new DateTimeOffset(2018, 02, 20, 0, 0, 0, TimeSpan.Zero);
                var runningTEParameter = DurationParameter.WithStartAndDuration(parameter.Start, null);
                ViewModel.Initialize(new EditDurationParameters(runningTEParameter));
                TimeService.CurrentDateTime.Returns(now);
                var stopObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var isRunningObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.StopTime.Subscribe(stopObserver);
                ViewModel.IsRunning.Subscribe(isRunningObserver);

                ViewModel.StopTimeEntry.Execute();

                TestScheduler.Start();
                isRunningObserver.LastEmittedValue().Should().BeFalse();
                stopObserver.LastEmittedValue().Should().Be(now);
            }

            [Fact]
            public void UnsubscribesFromTheTheRunningTimeEntryObservable()
            {
                var now = new DateTimeOffset(2018, 02, 20, 0, 0, 0, TimeSpan.Zero);
                var runningTEParameter = DurationParameter.WithStartAndDuration(parameter.Start, null);
                var subject = new BehaviorSubject<DateTimeOffset>(now);
                var observable = subject.AsObservable().Publish();
                var stopObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                ViewModel.StopTime.Subscribe(stopObserver);
                ViewModel.Initialize(new EditDurationParameters(runningTEParameter));
                TimeService.CurrentDateTime.Returns(now);
                TimeService.CurrentDateTimeObservable.Returns(observable);

                ViewModel.StopTimeEntry.Execute();
                subject.OnNext(now.AddSeconds(1));

                TestScheduler.Start();
                stopObserver.LastEmittedValue().Should().Be(now);
            }

            [Fact, LogIfTooSlow]
            public void TracksTimeEntryStoppedEvent()
            {
                var runningTEParameter = DurationParameter.WithStartAndDuration(parameter.Start, null);
                ViewModel.Initialize(new EditDurationParameters(runningTEParameter));

                ViewModel.StopTimeEntry.Execute();
                TestScheduler.Start();

                AnalyticsService.Received().TimeEntryStopped.Track(TimeEntryStopOrigin.Wheel);
            }
        }

        public sealed class TheEditStopTimeCommand : EditDurationViewModelTest
        {
            private static DurationParameter parameter = DurationParameter.WithStartAndDuration(
                new DateTimeOffset(2018, 01, 13, 0, 0, 0, TimeSpan.Zero),
                TimeSpan.FromMinutes(7));

            [Fact]
            public void SetsTheIsEditingFlagsCorrectlyWhenNothingWasEdited()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var startObserver = TestScheduler.CreateObserver<bool>();
                var stopObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEditingStartTime.Subscribe(startObserver);
                ViewModel.IsEditingStopTime.Subscribe(stopObserver);

                ViewModel.EditStopTime.Execute();

                TestScheduler.Start();
                startObserver.LastEmittedValue().Should().BeFalse();
                stopObserver.LastEmittedValue().Should().BeTrue();
            }

            [Fact]
            public void SetsTheIsEditingFlagsCorrectlyWhenStopTimeWasEdited()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var startObserver = TestScheduler.CreateObserver<bool>();
                var stopObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEditingStartTime.Subscribe(startObserver);
                ViewModel.IsEditingStopTime.Subscribe(stopObserver);
                ViewModel.EditStartTime.Execute();

                ViewModel.EditStopTime.Execute();

                TestScheduler.Start();
                startObserver.LastEmittedValue().Should().BeFalse();
                stopObserver.LastEmittedValue().Should().BeTrue();
            }

            [Fact]
            public void ClosesEditingWhenStartTimeWasBeingEdited()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var startObserver = TestScheduler.CreateObserver<bool>();
                var stopObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEditingStartTime.Subscribe(startObserver);
                ViewModel.IsEditingStopTime.Subscribe(stopObserver);

                ViewModel.EditStopTime.ExecuteSequentally(2)
                    .Subscribe();

                TestScheduler.Start();
                startObserver.LastEmittedValue().Should().BeFalse();
                stopObserver.LastEmittedValue().Should().BeFalse();
            }

            [Fact]
            public void SetsTheMinimumAndMaximumDateForTheDatePicker()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var minTimeObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var maxTimeObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                ViewModel.MinimumDateTime.Subscribe(minTimeObserver);
                ViewModel.MaximumDateTime.Subscribe(maxTimeObserver);

                ViewModel.EditStopTime.Execute();

                TestScheduler.Start();
                minTimeObserver.LastEmittedValue().Should().Be(parameter.Start);
                maxTimeObserver.LastEmittedValue().Should().Be(parameter.Start + TimeSpan.FromHours(999));
            }
        }

        public sealed class TheStopEditingTimeCommand : EditDurationViewModelTest
        {
            private static DurationParameter parameter = DurationParameter.WithStartAndDuration(
                new DateTimeOffset(2018, 01, 13, 0, 0, 0, TimeSpan.Zero),
                TimeSpan.FromMinutes(7));

            [Fact]
            public void ClearsAllTimeEditingFlagsWhenStartTimeWasEdited()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var editingObserver = TestScheduler.CreateObserver<bool>();
                var startObserver = TestScheduler.CreateObserver<bool>();
                var stopObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEditingTime.Subscribe(editingObserver);
                ViewModel.IsEditingStartTime.Subscribe(startObserver);
                ViewModel.IsEditingStopTime.Subscribe(stopObserver);

                ViewModel.EditStartTime.Execute();
                ViewModel.StopEditingTime.Execute();

                TestScheduler.Start();
                editingObserver.LastEmittedValue().Should().BeFalse();
                startObserver.LastEmittedValue().Should().BeFalse();
                stopObserver.LastEmittedValue().Should().BeFalse();
            }

            [Fact]
            public void ClearsAllTimeEditingFlagsWhenStopTimeWasEdited()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var editingObserver = TestScheduler.CreateObserver<bool>();
                var startObserver = TestScheduler.CreateObserver<bool>();
                var stopObserver = TestScheduler.CreateObserver<bool>();
                ViewModel.IsEditingTime.Subscribe(editingObserver);
                ViewModel.IsEditingStartTime.Subscribe(startObserver);
                ViewModel.IsEditingStopTime.Subscribe(stopObserver);

                ViewModel.EditStopTime.Execute();
                ViewModel.StopEditingTime.Execute();

                TestScheduler.Start();
                editingObserver.LastEmittedValue().Should().BeFalse();
                startObserver.LastEmittedValue().Should().BeFalse();
                stopObserver.LastEmittedValue().Should().BeFalse();
            }
        }

        public sealed class TheChangeActiveTimeAction : EditDurationViewModelTest
        {
            private static DurationParameter parameter = DurationParameter.WithStartAndDuration(
                new DateTimeOffset(2018, 01, 13, 0, 0, 0, TimeSpan.Zero),
                TimeSpan.FromMinutes(7));

            [Fact]
            public void DoesNotAcceptAnyValueWhenNotEditingNeitherStartNorStopTime()
            {
                var editedValue = new DateTimeOffset(2018, 02, 20, 0, 0, 0, TimeSpan.Zero);
                ViewModel.Initialize(new EditDurationParameters(parameter));

                ViewModel.ChangeActiveTime.Execute(editedValue);

                ViewModel.StartTime.Should().NotBe(editedValue);
                ViewModel.StopTime.Should().NotBe(editedValue);
            }

            [Fact]
            public void ChangesJustTheStartTimeWhenEditingStartTime()
            {
                var editedValue = new DateTimeOffset(2018, 01, 07, 0, 0, 0, TimeSpan.Zero);
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var startObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var stopObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var temporalInconsistenciesObserver = TestScheduler.CreateObserver<TemporalInconsistency>();
                ViewModel.StartTime.Subscribe(startObserver);
                ViewModel.StopTime.Subscribe(stopObserver);
                ViewModel.TemporalInconsistencies.Subscribe(temporalInconsistenciesObserver);

                ViewModel.EditStartTime.Execute();
                ViewModel.ChangeActiveTime.Execute(editedValue);

                TestScheduler.Start();
                startObserver.LastEmittedValue().Should().Be(editedValue);
                stopObserver.LastEmittedValue().Should().NotBe(editedValue);
                temporalInconsistenciesObserver.Messages.Should().BeEmpty();
            }

            [Fact]
            public void DoesNotAllowChangingTheStartTimeToMoreThanTheMaximumDate()
            {
                var editedValue = parameter.Start.Add(parameter.Duration.Value).AddHours(1);
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var startObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var stopObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var temporalInconsistenciesObserver = TestScheduler.CreateObserver<TemporalInconsistency>();
                var maxTimeObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                ViewModel.StartTime.Subscribe(startObserver);
                ViewModel.StopTime.Subscribe(stopObserver);
                ViewModel.MaximumDateTime.Subscribe(maxTimeObserver);
                ViewModel.TemporalInconsistencies.Subscribe(temporalInconsistenciesObserver);

                ViewModel.EditStartTime.Execute();
                ViewModel.ChangeActiveTime.Execute(editedValue);

                TestScheduler.Start();
                startObserver.LastEmittedValue().Should().Be(maxTimeObserver.LastEmittedValue());
                stopObserver.LastEmittedValue().Should().Be(maxTimeObserver.LastEmittedValue());
                temporalInconsistenciesObserver.LastEmittedValue().Should().Be(TemporalInconsistency.StartTimeAfterStopTime);
            }

            [Fact]
            public void DoesNotAllowChangingTheStartTimeToLessThanTheMinimumDate()
            {
                var editedValue = parameter.Start.AddHours(-1000);
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var startObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var stopObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var minTimeObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var temporalInconsistenciesObserver = TestScheduler.CreateObserver<TemporalInconsistency>();
                ViewModel.StartTime.Subscribe(startObserver);
                ViewModel.StopTime.Subscribe(stopObserver);
                ViewModel.MinimumDateTime.Subscribe(minTimeObserver);
                ViewModel.TemporalInconsistencies.Subscribe(temporalInconsistenciesObserver);

                ViewModel.EditStartTime.Execute();
                ViewModel.ChangeActiveTime.Execute(editedValue);

                TestScheduler.Start();
                startObserver.LastEmittedValue().Should().Be(minTimeObserver.LastEmittedValue());
                stopObserver.LastEmittedValue().Should().NotBe(minTimeObserver.LastEmittedValue());
                temporalInconsistenciesObserver.LastEmittedValue().Should().Be(TemporalInconsistency.DurationTooLong);
            }

            [Fact]
            public void ChangesJustTheStopTimeWhenEditingTheStopTime()
            {
                var editedValue = new DateTimeOffset(2018, 02, 20, 0, 0, 0, TimeSpan.Zero);
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var startObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var stopObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var temporalInconsistenciesObserver = TestScheduler.CreateObserver<TemporalInconsistency>();
                ViewModel.StartTime.Subscribe(startObserver);
                ViewModel.StopTime.Subscribe(stopObserver);
                ViewModel.TemporalInconsistencies.Subscribe(temporalInconsistenciesObserver);

                ViewModel.EditStopTime.Execute();
                ViewModel.ChangeActiveTime.Execute(editedValue);

                TestScheduler.Start();
                startObserver.LastEmittedValue().Should().NotBe(editedValue);
                stopObserver.LastEmittedValue().Should().Be(editedValue);
                temporalInconsistenciesObserver.Messages.Should().BeEmpty();
            }

            [Fact]
            public void DoesNotAllowChangingTheStopTimeToMoreThanTheMaximumDate()
            {
                var editedValue = parameter.Start.AddHours(1000);
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var stopObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var maxTimeObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var temporalInconsistenciesObserver = TestScheduler.CreateObserver<TemporalInconsistency>();
                ViewModel.StopTime.Subscribe(stopObserver);
                ViewModel.MaximumDateTime.Subscribe(maxTimeObserver);
                ViewModel.TemporalInconsistencies.Subscribe(temporalInconsistenciesObserver);

                ViewModel.EditStopTime.Execute();
                ViewModel.ChangeActiveTime.Execute(editedValue);

                TestScheduler.Start();
                stopObserver.LastEmittedValue().Should().Be(maxTimeObserver.LastEmittedValue());
                temporalInconsistenciesObserver.LastEmittedValue().Should().Be(TemporalInconsistency.DurationTooLong);
            }

            [Fact]
            public void DoesNotAllowChangingTheStopTimeToLessThanTheMinimumDate()
            {
                var editedValue = parameter.Start.AddHours(-1);
                ViewModel.Initialize(new EditDurationParameters(parameter));
                var stopObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var minTimeObserver = TestScheduler.CreateObserver<DateTimeOffset>();
                var temporalInconsistenciesObserver = TestScheduler.CreateObserver<TemporalInconsistency>();
                ViewModel.StopTime.Subscribe(stopObserver);
                ViewModel.MinimumDateTime.Subscribe(minTimeObserver);
                ViewModel.TemporalInconsistencies.Subscribe(temporalInconsistenciesObserver);

                ViewModel.EditStopTime.Execute();
                ViewModel.ChangeActiveTime.Execute(editedValue);

                TestScheduler.Start();
                stopObserver.LastEmittedValue().Should().Be(minTimeObserver.LastEmittedValue());
                temporalInconsistenciesObserver.LastEmittedValue().Should().Be(TemporalInconsistency.StopTimeBeforeStartTime);
            }
        }

        public sealed class TheIsDurationInitiallyFocusedProperty : EditDurationViewModelTest
        {
            private static DurationParameter parameter = DurationParameter.WithStartAndDuration(
                new DateTimeOffset(2018, 01, 13, 0, 0, 0, TimeSpan.Zero),
                TimeSpan.FromMinutes(7));

            [Fact]
            public void DefaultToNone()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));
                ViewModel.IsDurationInitiallyFocused.Should().Be(false);
            }

            [Fact]
            public void ShouldBeSetProperly()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter, isStartingNewEntry: true, isDurationInitiallyFocused: true));
                ViewModel.IsDurationInitiallyFocused.Should().Be(true);
            }
        }

        public sealed class TheAnalyticsService : EditDurationViewModelTest
        {
            private static readonly DurationParameter parameter = DurationParameter.WithStartAndDuration(
                new DateTimeOffset(2018, 01, 13, 0, 0, 0, TimeSpan.Zero),
                TimeSpan.FromMinutes(7));

            [Fact, LogIfTooSlow]
            public void ReceivesEventWhenViewModelCloses()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));

                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                AnalyticsService.Received().Track(
                    Arg.Is<ITrackableEvent>(trackableEvent =>
                        trackableEvent.EventName == "EditDuration"
                        && trackableEvent.ToDictionary().ContainsKey("navigationOrigin")
                        && trackableEvent.ToDictionary().ContainsKey("result")
                        && trackableEvent.ToDictionary()["navigationOrigin"] == EditDurationEvent.NavigationOrigin.Edit.ToString()
                        && trackableEvent.ToDictionary()["result"] == EditDurationEvent.Result.Cancel.ToString()
                    )
                );
            }

            [Fact, LogIfTooSlow]
            public void ReceivesEventWhenViewModelSaves()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter, isStartingNewEntry: true));

                ViewModel.Save.Execute();

                TestScheduler.Start();
                AnalyticsService.Received().Track(
                    Arg.Is<ITrackableEvent>(trackableEvent =>
                        trackableEvent.EventName == "EditDuration"
                        && trackableEvent.ToDictionary().ContainsKey("navigationOrigin")
                        && trackableEvent.ToDictionary().ContainsKey("result")
                        && trackableEvent.ToDictionary()["navigationOrigin"] == EditDurationEvent.NavigationOrigin.Start.ToString()
                        && trackableEvent.ToDictionary()["result"] == EditDurationEvent.Result.Save.ToString()
                    )
                );
            }

            [Fact, LogIfTooSlow]
            public void SetsCorrectParametersOnEdition()
            {
                ViewModel.Initialize(new EditDurationParameters(parameter));

                ViewModel.TimeEditedWithSource(EditTimeSource.WheelBothTimes);
                ViewModel.TimeEditedWithSource(EditTimeSource.BarrelStartDate);
                ViewModel.Save.Execute();

                TestScheduler.Start();
                AnalyticsService.Received().Track(
                    Arg.Is<ITrackableEvent>(trackableEvent =>
                        trackableEvent.EventName == "EditDuration"
                        && trackableEvent.ToDictionary().ContainsKey("changedBothTimesWithWheel")
                        && trackableEvent.ToDictionary().ContainsKey("changedStartDateWithBarrel")
                        && trackableEvent.ToDictionary().ContainsKey("changedEndDateWithBarrel")
                        && trackableEvent.ToDictionary()["changedBothTimesWithWheel"] == true.ToString()
                        && trackableEvent.ToDictionary()["changedStartDateWithBarrel"] == true.ToString()
                        && trackableEvent.ToDictionary()["changedEndDateWithBarrel"] == false.ToString()
                    )
                );
            }
        }
    }
}
