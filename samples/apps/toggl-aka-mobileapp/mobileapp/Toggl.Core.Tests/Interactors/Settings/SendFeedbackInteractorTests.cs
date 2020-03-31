using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Interactors.Settings;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.Mocks;
using Toggl.Networking.ApiClients;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Models;
using Toggl.Storage.Settings;
using Xunit;
using static Toggl.Core.Interactors.Settings.SendFeedbackInteractor;

namespace Toggl.Core.Tests.Interactors.Settings
{
    public sealed class SendFeedbackInteractorTests
    {
        public sealed class TheConstructor : BaseInteractorTests
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheParametersIsNullOrInvalid(
                bool useFeedbackApi,
                bool useUserDataSource,
                bool useWorkspacesDataSource,
                bool useTimeEntriesDataSource,
                bool useplatformInfo,
                bool useUserPreferences,
                bool useLastTimeUsageStorage,
                bool useTimeService,
                bool useInteractorFactory,
                bool useMessage)
            {
                // ReSharper disable once ObjectCreationAsStatement
                Action createInstance = () => new SendFeedbackInteractor(
                    useFeedbackApi ? Substitute.For<IFeedbackApi>() : null,
                    useUserDataSource ? DataSource.User : null,
                    useWorkspacesDataSource ? DataSource.Workspaces : null,
                    useTimeEntriesDataSource ? DataSource.TimeEntries : null,
                    useplatformInfo ? Substitute.For<IPlatformInfo>() : null,
                    useUserPreferences ? UserPreferences : null,
                    useLastTimeUsageStorage ? Substitute.For<ILastTimeUsageStorage>() : null,
                    useTimeService ? TimeService : null,
                    useInteractorFactory ? InteractorFactory : null,
                    useMessage ? "some message" : null);

                createInstance.Should().Throw<ArgumentException>();
            }
        }

        public sealed class TheSendMethod : BaseInteractorTests
        {
            private readonly IEnumerable<IThreadSafeTimeEntry> timeEntries = new[]
            {
                new MockTimeEntry { SyncStatus = SyncStatus.InSync },
                new MockTimeEntry { SyncStatus = SyncStatus.SyncNeeded },
                new MockTimeEntry { SyncStatus = SyncStatus.InSync },
                new MockTimeEntry { SyncStatus = SyncStatus.SyncFailed },
                new MockTimeEntry { SyncStatus = SyncStatus.SyncFailed },
                new MockTimeEntry { SyncStatus = SyncStatus.InSync },
            };

            private readonly IEnumerable<IThreadSafeWorkspace> workspaces = new[]
            {
                new MockWorkspace(), new MockWorkspace(), new MockWorkspace(), new MockWorkspace()
            };

            private readonly IFeedbackApi feedbackApi = Substitute.For<IFeedbackApi>();

            private readonly IThreadSafeUser user = Substitute.For<IThreadSafeUser>();

            public TheSendMethod()
            {
                DataSource.User.Get().Returns(Observable.Return(user));
                DataSource.Workspaces.GetAll().Returns(Observable.Return(workspaces));
                DataSource.TimeEntries.GetAll().Returns(Observable.Return(timeEntries));
                DataSource.TimeEntries.GetAll(Arg.Any<Func<IDatabaseTimeEntry, bool>>())
                    .Returns(callInfo => Observable.Return(timeEntries.Where<IThreadSafeTimeEntry>(callInfo.Arg<Func<IDatabaseTimeEntry, bool>>())));
            }

            [Property]
            public void SendsUsersMessage(NonNull<string> message)
            {
                var email = $"{Guid.NewGuid().ToString()}@randomdomain.com".ToEmail();
                user.Email.Returns(email);

                executeInteractor(message: message.Get).Wait();

                feedbackApi.Received().Send(Arg.Is(email), Arg.Is(message.Get), Arg.Any<Dictionary<string, string>>());
            }

            [Fact, LogIfTooSlow]
            public async Task SendsCorrectplatformInfo()
            {
                var operatingSystem = "TogglOS";
                var phoneModel = "TogglPhone";
                PlatformInfo.OperatingSystem.Returns(operatingSystem);
                PlatformInfo.PhoneModel.Returns(phoneModel);

                await executeInteractor();

                await feedbackApi.Received().Send(Arg.Any<Email>(), Arg.Any<string>(), Arg.Is<Dictionary<string, string>>(
                    data => data[SendFeedbackInteractor.OperatingSystem] == operatingSystem
                        && data[PhoneModel] == phoneModel));
            }

            [Fact, LogIfTooSlow]
            public async Task SendsTheAppPlatformSlashAppVersion()
            {
                const string version = "42.2";
                var platform = Platform.Giskard;
                PlatformInfo.Version.Returns(version);
                PlatformInfo.Platform.Returns(platform);
                var formattedUserAgent = $"{platform}/{version}";

                await executeInteractor();

                await feedbackApi.Received().Send(Arg.Any<Email>(), Arg.Any<string>(), Arg.Is<Dictionary<string, string>>(
                    data => data[AppNameAndVersion] == formattedUserAgent));
            }

            [Property]
            public void SendsTimesOfLastUsage(
                DateTimeOffset? login,
                DateTimeOffset? syncAttempt,
                DateTimeOffset? successfulSync)
            {
                LastTimeUsageStorage.LastLogin.Returns(login);
                LastTimeUsageStorage.LastSyncAttempt.Returns(syncAttempt);
                LastTimeUsageStorage.LastSuccessfulSync.Returns(successfulSync);

                executeInteractor().Wait();

                feedbackApi.Received().Send(Arg.Any<Email>(), Arg.Any<string>(), Arg.Is<Dictionary<string, string>>(
                    data => data[LastLogin] == (login.HasValue ? login.ToString() : "never")
                        && data[LastSyncAttempt] == (syncAttempt.HasValue ? syncAttempt.ToString() : "never")
                        && data[LastSuccessfulSync] == (successfulSync.HasValue ? successfulSync.ToString() : "never")));
            }

            [Property]
            public void SendsCurrentDeviceTime(DateTimeOffset now)
            {
                TimeService.CurrentDateTime.Returns(now);

                executeInteractor().Wait();

                feedbackApi.Received().Send(Arg.Any<Email>(), Arg.Any<string>(), Arg.Is<Dictionary<string, string>>(
                    data => data[DeviceTime] == now.ToString()));
            }

            [Property]
            public void SendsUsersPreferences(bool isManualModeEnabled)
            {
                UserPreferences.IsManualModeEnabled.Returns(isManualModeEnabled);

                executeInteractor().Wait();

                feedbackApi.Received().Send(Arg.Any<Email>(), Arg.Any<string>(), Arg.Is<Dictionary<string, string>>(
                    data => data[ManualModeIsOn] == (isManualModeEnabled ? "yes" : "no"))).Wait();
            }

            [Property]
            public void SendsTheUserId(int userId)
            {
                var user = Substitute.For<IThreadSafeUser>();
                user.Id.Returns(userId);
                InteractorFactory.GetCurrentUser().Execute().Returns(Observable.Return(user));

                executeInteractor().Wait();

                feedbackApi.Received().Send(Arg.Any<Email>(), Arg.Any<string>(), Arg.Is<Dictionary<string, string>>(
                    data => data[UserId] == userId.ToString())).Wait();
            }

            [Property]
            public void SendsApplicationInstallLocation(ApplicationInstallLocation installLocation)
            {
                PlatformInfo.InstallLocation.Returns(installLocation);
                PlatformInfo.Platform.Returns(Platform.Giskard);

                executeInteractor().Wait();

                feedbackApi.Received().Send(Arg.Any<Email>(), Arg.Any<string>(), Arg.Is<Dictionary<string, string>>(
                    data => data[InstallLocation] == installLocation.ToString())).Wait();
            }

            [Property]
            public void DoesNotSendApplicationInstallLocationOnIOs(ApplicationInstallLocation installLocation)
            {
                PlatformInfo.InstallLocation.Returns(installLocation);
                PlatformInfo.Platform.Returns(Platform.Daneel);

                executeInteractor().Wait();

                feedbackApi.DidNotReceive().Send(Arg.Any<Email>(), Arg.Any<string>(), Arg.Is<Dictionary<string, string>>(
                    data => data[InstallLocation] == installLocation.ToString())).Wait();
            }

            [Fact, LogIfTooSlow]
            public async Task CountsAllWorkspaces()
            {
                await executeInteractor();

                await feedbackApi.Received().Send(Arg.Any<Email>(), Arg.Any<string>(), Arg.Is<Dictionary<string, string>>(
                    data => data[NumberOfWorkspaces] == "4"));
            }

            [Fact, LogIfTooSlow]
            public async Task CountsTimeEntries()
            {
                await executeInteractor();

                await feedbackApi.Received().Send(Arg.Any<Email>(), Arg.Any<string>(), Arg.Is<Dictionary<string, string>>(
                    data => data[NumberOfTimeEntries] == "6"
                        && data[NumberOfUnsyncedTimeEntries] == "1"
                        && data[NumberOfUnsyncableTimeEntries] == "2"));
            }

            private async Task executeInteractor(string message = "")
            {
                var interactor = new SendFeedbackInteractor(
                    feedbackApi,
                    DataSource.User,
                    DataSource.Workspaces,
                    DataSource.TimeEntries,
                    PlatformInfo,
                    UserPreferences,
                    LastTimeUsageStorage,
                    TimeService,
                    InteractorFactory,
                    message);

                await interactor.Execute();
            }
        }
    }
}
