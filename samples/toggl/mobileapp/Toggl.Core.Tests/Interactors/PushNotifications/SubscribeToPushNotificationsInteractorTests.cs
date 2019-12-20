using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Interactors.PushNotifications;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Networking.ApiClients;
using Toggl.Shared;
using Toggl.Storage;
using Xunit;

namespace Toggl.Core.Tests.Interactors.PushNotifications
{
    public class SubscribeToPushNotificationsInteractorTests : BaseInteractorTests
    {
        private static readonly DateTimeOffset now = new DateTimeOffset(2019, 06, 13, 12, 13, 14, TimeSpan.Zero);

        public sealed class TheConstructor : BaseInteractorTests
        {
            private IPushNotificationsTokenStorage pushNotificationsTokenStorage = Substitute.For<IPushNotificationsTokenStorage>();

            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool usePushNotificationsTokenStorage, bool usePushServicesApi, bool usePushNotificationTokenService, bool useTimeService)
            {
                Action tryingToConstructWithNull = () => new SubscribeToPushNotificationsInteractor(
                    usePushNotificationsTokenStorage ? pushNotificationsTokenStorage : null,
                    usePushServicesApi ? Api : null,
                    usePushNotificationTokenService ? PushNotificationsTokenService : null,
                    useTimeService ? TimeService : null);

                tryingToConstructWithNull.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private SubscribeToPushNotificationsInteractor interactor;
            private IPushServicesApi pushServicesApi = Substitute.For<IPushServicesApi>();
            private IPushNotificationsTokenStorage pushNotificationsTokenStorage = Substitute.For<IPushNotificationsTokenStorage>();

            public TheExecuteMethod()
            {
                Api.PushServices.Returns(pushServicesApi);
                TimeService.CurrentDateTime.Returns(now);
                interactor = new SubscribeToPushNotificationsInteractor(pushNotificationsTokenStorage, Api, PushNotificationsTokenService, TimeService);
            }

            [Fact]
            public async Task DoesNothingWhenPushNotificationsTokenServiceTokenIsNull()
            {
                PushNotificationsTokenService.Token?.Returns(null);

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                pushServicesApi.DidNotReceive().Subscribe(Arg.Any<PushNotificationsToken>());
                pushServicesApi.DidNotReceive().Unsubscribe(Arg.Any<PushNotificationsToken>());
            }

            [Fact]
            public async Task DoesNothingWhenPushNotificationsTokenServiceTokenIsEmpty()
            {
                PushNotificationsTokenService.Token.Returns(default(PushNotificationsToken));

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                pushServicesApi.DidNotReceive().Subscribe(Arg.Any<PushNotificationsToken>());
                pushServicesApi.DidNotReceive().Unsubscribe(Arg.Any<PushNotificationsToken>());
            }

            [Fact]
            public async Task DoesNothingWhenTheTokenHasAlreadyBeenRegisteredRecently()
            {
                pushNotificationsTokenStorage.PreviouslyRegisteredToken.Returns(new PushNotificationsToken("tokenA"));
                pushNotificationsTokenStorage.DateOfRegisteringTheToken.Returns(now - TimeSpan.FromDays(2));
                PushNotificationsTokenService.Token.Returns(new PushNotificationsToken("tokenA"));

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                pushServicesApi.DidNotReceive().Subscribe(Arg.Any<PushNotificationsToken>());
                pushServicesApi.DidNotReceive().Unsubscribe(Arg.Any<PushNotificationsToken>());
            }

            [Fact]
            public async Task CallsTheApiToSubscribeForPushNotificationsWhenNoOtherTokenHasBeenStored()
            {
                pushNotificationsTokenStorage.PreviouslyRegisteredToken.Returns(default(PushNotificationsToken));
                var expectedPushNotificationToken = new PushNotificationsToken("tokenA");
                PushNotificationsTokenService.Token.Returns(expectedPushNotificationToken);
                pushServicesApi.Subscribe(Arg.Any<PushNotificationsToken>()).ReturnsCompletedTask();

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                pushServicesApi.Received().Subscribe(expectedPushNotificationToken);
            }

            [Fact]
            public async Task CallsTheApiToSubscribeForPushNotificationsAfterATimePeriod()
            {
                var token = new PushNotificationsToken("tokenA");
                pushNotificationsTokenStorage.PreviouslyRegisteredToken.Returns(token);
                pushNotificationsTokenStorage.DateOfRegisteringTheToken.Returns(now - TimeSpan.FromDays(10));
                PushNotificationsTokenService.Token.Returns(token);
                pushServicesApi.Subscribe(Arg.Any<PushNotificationsToken>()).ReturnsCompletedTask();

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                pushServicesApi.Received().Subscribe(token);
            }

            [Fact]
            public async Task StoresTheTokenWhenSucceedsToRegisterIt()
            {
                pushNotificationsTokenStorage.PreviouslyRegisteredToken.Returns(default(PushNotificationsToken));
                var expectedPushNotificationToken = new PushNotificationsToken("tokenA");
                PushNotificationsTokenService.Token.Returns(expectedPushNotificationToken);
                pushServicesApi.Subscribe(Arg.Any<PushNotificationsToken>()).ReturnsCompletedTask();

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                pushNotificationsTokenStorage.Received().StoreRegisteredToken(expectedPushNotificationToken, now);
            }

            [Fact]
            public async Task DoesNotStoreTheTokenWhenItFailsToRegisterIt()
            {
                pushNotificationsTokenStorage.PreviouslyRegisteredToken.Returns(default(PushNotificationsToken));
                var expectedPushNotificationToken = new PushNotificationsToken("tokenA");
                PushNotificationsTokenService.Token.Returns(expectedPushNotificationToken);
                pushServicesApi.Subscribe(Arg.Any<PushNotificationsToken>()).ReturnsThrowingTask(new Exception());

                (await interactor.Execute().SingleAsync()).Should().Be(Unit.Default);
                pushServicesApi.Received().Subscribe(expectedPushNotificationToken);
                pushNotificationsTokenStorage.DidNotReceive().StoreRegisteredToken(expectedPushNotificationToken, Arg.Any<DateTimeOffset>());
            }
        }
    }
}
