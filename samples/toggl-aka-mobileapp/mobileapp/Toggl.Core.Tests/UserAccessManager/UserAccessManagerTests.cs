using FluentAssertions;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Interactors;
using Toggl.Core.Login;
using Toggl.Core.Services;
using Toggl.Core.Sync;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Networking;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Network;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage;
using Toggl.Storage.Models;
using Toggl.Storage.Settings;
using Xunit;
using FoundationUser = Toggl.Core.Models.User;
using User = Toggl.Networking.Models.User;

namespace Toggl.Core.Tests.Login
{
    public sealed class UserAccessManagerTests
    {
        public abstract class UserAccessManagerTest
        {
            protected static readonly Password Password = "theirobotmoviesucked123".ToPassword();
            protected static readonly Email Email = "susancalvin@psychohistorian.museum".ToEmail();
            protected static readonly bool TermsAccepted = true;
            protected static readonly int CountryId = 237;
            protected static readonly string Timezone = "Europe/Tallinn";

            protected IUser User { get; } = new User { Id = 10, ApiToken = "ABCDEFG" };
            protected ITogglApi Api { get; } = Substitute.For<ITogglApi>();
            protected IApiFactory ApiFactory { get; } = Substitute.For<IApiFactory>();
            protected ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();
            protected IAccessRestrictionStorage AccessRestrictionStorage { get; } = Substitute.For<IAccessRestrictionStorage>();
            protected ITogglDataSource DataSource { get; } = Substitute.For<ITogglDataSource>();
            protected IUserAccessManager UserAccessManager { get; }
            protected IScheduler Scheduler { get; } = System.Reactive.Concurrency.Scheduler.Default;
            protected ISyncManager SyncManager { get; } = Substitute.For<ISyncManager>();
            protected IInteractorFactory InteractorFactory { get; } = Substitute.For<IInteractorFactory>();
            protected (ISyncManager, IInteractorFactory) Initialize(ITogglApi api) => (SyncManager, InteractorFactory);
            protected virtual IScheduler CreateScheduler => Scheduler;
            protected IAnalyticsService AnalyticsService { get; } = Substitute.For<IAnalyticsService>();
            protected IPrivateSharedStorageService PrivateSharedStorageService { get; } = Substitute.For<IPrivateSharedStorageService>();

            protected UserAccessManagerTest()
            {
                UserAccessManager = new UserAccessManager(
                    new Lazy<IApiFactory>(() => ApiFactory),
                    new Lazy<ITogglDatabase>(() => Database),
                    new Lazy<IPrivateSharedStorageService>(() => PrivateSharedStorageService)
                );

                Api.User.Get().ReturnsTaskOf(User);
                Api.User.SignUp(Email, Password, TermsAccepted, CountryId, Timezone).ReturnsTaskOf(User);
                Api.User.GetWithGoogle().ReturnsTaskOf(User);
                ApiFactory.CreateApiWith(Arg.Any<Credentials>()).Returns(Api);
                Database.Clear().Returns(Observable.Return(Unit.Default));
            }
        }

        public abstract class UserAccessManagerWithTestSchedulerTest : UserAccessManagerTest
        {
            protected TestScheduler TestScheduler { get; } = new TestScheduler();
            protected override IScheduler CreateScheduler => TestScheduler;
        }

        public sealed class Constructor : UserAccessManagerTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useApiFactory,
                bool useDatabase,
                bool usePrivateSharedStorageService)
            {
                var database = useDatabase ? new Lazy<ITogglDatabase>(() => Database) : null;
                var apiFactory = useApiFactory ? new Lazy<IApiFactory>(() => ApiFactory) : null;
                var privateSharedStorageService = usePrivateSharedStorageService ? new Lazy<IPrivateSharedStorageService>(() => PrivateSharedStorageService) : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new UserAccessManager(apiFactory, database, privateSharedStorageService);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheUserAccessMethod : UserAccessManagerTest
        {
            public TheUserAccessMethod()
            {
                var databaseUserSubstitute = Substitute.For<IDatabaseUser>();
                databaseUserSubstitute.ApiToken.Returns("ApiToken");
                Database.User.Create(Arg.Any<IDatabaseUser>())
                    .Returns(Observable.Return(databaseUserSubstitute));
            }

            [Theory, LogIfTooSlow]
            [InlineData("susancalvin@psychohistorian.museum", null)]
            [InlineData("susancalvin@psychohistorian.museum", "")]
            [InlineData("susancalvin@psychohistorian.museum", " ")]
            [InlineData("susancalvin@", null)]
            [InlineData("susancalvin@", "")]
            [InlineData("susancalvin@", " ")]
            [InlineData("susancalvin@", "123456")]
            [InlineData("", null)]
            [InlineData("", "")]
            [InlineData("", " ")]
            [InlineData("", "123456")]
            [InlineData(null, null)]
            [InlineData(null, "")]
            [InlineData(null, " ")]
            [InlineData(null, "123456")]
            public void ThrowsIfYouPassInvalidParameters(string email, string password)
            {
                var actualEmail = email.ToEmail();
                var actualPassword = password.ToPassword();

                Action tryingToConstructWithEmptyParameters =
                    () => UserAccessManager.Login(actualEmail, actualPassword).Wait();

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentException>();
            }

            [Fact, LogIfTooSlow]
            public async Task EmptiesTheDatabaseBeforeTryingToLogin()
            {
                await UserAccessManager.Login(Email, Password);

                Received.InOrder(async () =>
                {
                    await Database.Clear();
                    await Api.User.Get();
                });
            }

            [Fact, LogIfTooSlow]
            public async Task CallsTheGetMethodOfTheUserApi()
            {
                await UserAccessManager.Login(Email, Password);

                await Api.User.Received().Get();
            }

            [Fact, LogIfTooSlow]
            public async Task PersistsTheUserToTheDatabase()
            {
                await UserAccessManager.Login(Email, Password);

                await Database.User.Received().Create(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.Id == User.Id));
            }

            [Fact, LogIfTooSlow]
            public async Task PersistsTheUserWithTheSyncStatusSetToInSync()
            {
                await UserAccessManager.Login(Email, Password);

                await Database.User.Received().Create(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.SyncStatus == SyncStatus.InSync));
            }

            [Fact, LogIfTooSlow]
            public async Task AlwaysReturnsASingleResult()
            {
                await UserAccessManager
                        .Login(Email, Password)
                        .SingleAsync();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotRetryWhenTheApiThrowsSomethingOtherThanUserIsMissingApiTokenException()
            {
                var serverErrorException = Substitute.For<ServerErrorException>(Substitute.For<IRequest>(), Substitute.For<IResponse>(), "Some Exception");
                Api.User.Get().ReturnsThrowingTaskOf(serverErrorException);

                Action tryingToLoginWhenTheApiIsThrowingSomeRandomServerErrorException =
                    () => UserAccessManager.Login(Email, Password).Wait();

                tryingToLoginWhenTheApiIsThrowingSomeRandomServerErrorException
                    .Should().Throw<ServerErrorException>();
            }
        }

        public sealed class TheResetPasswordMethod : UserAccessManagerTest
        {
            [Theory, LogIfTooSlow]
            [InlineData("foo")]
            public void ThrowsWhenEmailIsInvalid(string emailString)
            {
                Action tryingToResetWithInvalidEmail = () => UserAccessManager
                    .ResetPassword(Email.From(emailString))
                    .Wait();

                tryingToResetWithInvalidEmail.Should().Throw<ArgumentException>();
            }

            [Fact, LogIfTooSlow]
            public async Task UsesApiWithoutCredentials()
            {
                await UserAccessManager.ResetPassword(Email.From("some@email.com"));

                ApiFactory.Received().CreateApiWith(Arg.Is<Credentials>(
                    arg => arg.Header.Name == null
                        && arg.Header.Value == null
                        && arg.Header.Type == HttpHeader.HeaderType.None));
            }

            [Theory, LogIfTooSlow]
            [InlineData("example@email.com")]
            [InlineData("john.smith@gmail.com")]
            [InlineData("h4cker123@domain.ru")]
            public async Task CallsApiClientWithThePassedEmailAddress(string address)
            {
                var email = address.ToEmail();

                await UserAccessManager.ResetPassword(email);

                await Api.User.Received().ResetPassword(Arg.Is(email));
            }
        }

        public sealed class TheCheckIfLoggedInMethod : UserAccessManagerTest
        {
            [Fact, LogIfTooSlow]
            public void ReturnsFalseIfTheDatabaseHasNoUsers()
            {
                var observable = Observable.Throw<IDatabaseUser>(new InvalidOperationException());
                Database.User.Single().Returns(observable);
                PrivateSharedStorageService.HasUserDataStored().Returns(false);

                var result = UserAccessManager.CheckIfLoggedIn();

                result.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTrueIfTheUserDataWasStoredInThePrivateStorage()
            {
                PrivateSharedStorageService.HasUserDataStored().Returns(true);
                PrivateSharedStorageService.GetApiToken().Returns("ApiToken");

                var result = UserAccessManager.CheckIfLoggedIn();

                result.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTrueAndStoresTheUserDataIfUserIsInTheDatabaseButNotInThePrivateStorage()
            {
                PrivateSharedStorageService.HasUserDataStored().Returns(false);
                var observable = Observable.Return<IDatabaseUser>(FoundationUser.Clean(User));
                Database.User.Single().Returns(observable);

                var result = UserAccessManager.CheckIfLoggedIn();

                result.Should().BeTrue();
                PrivateSharedStorageService.Received().SaveApiToken(Arg.Any<string>());
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTrueTheUserExistsInTheDatabase()
            {
                var observable = Observable.Return<IDatabaseUser>(FoundationUser.Clean(User));
                Database.User.Single().Returns(observable);
                PrivateSharedStorageService.HasUserDataStored().Returns(false);

                var result = UserAccessManager.CheckIfLoggedIn();

                result.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotCauseTheUserLoggedInObservableToEmit()
            {
                var observer = Substitute.For<IObserver<ITogglApi>>();
                var observable = Observable.Throw<IDatabaseUser>(new InvalidOperationException());
                Database.User.Single().Returns(observable);
                UserAccessManager.UserLoggedIn.Subscribe(observer);

                UserAccessManager.CheckIfLoggedIn();

                observer.DidNotReceive().OnNext(Arg.Any<ITogglApi>());
            }
        }

        public sealed class TheLoginWithSavedCredentialsMethod : UserAccessManagerTest
        {
            [Fact, LogIfTooSlow]
            public void EmitsWhenUserIsLoggedAndDataIsInTheDataBase()
            {
                var observer = Substitute.For<IObserver<ITogglApi>>();
                var observable = Observable.Return<IDatabaseUser>(FoundationUser.Clean(User));
                Database.User.Single().Returns(observable);
                UserAccessManager.UserLoggedIn.Subscribe(observer);

                UserAccessManager.LoginWithSavedCredentials();

                observer.Received().OnNext(Arg.Any<ITogglApi>());
            }

            [Fact, LogIfTooSlow]
            public void EmitsWhenUserIsLoggedInAndDataIsAlreadyStoredInThePrivateStorage()
            {
                var observer = Substitute.For<IObserver<ITogglApi>>();
                UserAccessManager.UserLoggedIn.Subscribe(observer);
                PrivateSharedStorageService.HasUserDataStored().Returns(true);
                PrivateSharedStorageService.GetApiToken().Returns("ApiToken");

                UserAccessManager.LoginWithSavedCredentials();

                observer.Received().OnNext(Arg.Any<ITogglApi>());
            }

            [Fact, LogIfTooSlow]
            public void DoesNotCauseTheUserLoggedInObservableToEmit()
            {
                var observer = Substitute.For<IObserver<ITogglApi>>();
                var observable = Observable.Throw<IDatabaseUser>(new InvalidOperationException());
                Database.User.Single().Returns(observable);
                UserAccessManager.UserLoggedIn.Subscribe(observer);

                UserAccessManager.LoginWithSavedCredentials();

                observer.DidNotReceive().OnNext(Arg.Any<ITogglApi>());
            }
        }

        public sealed class TheSignUpMethod : UserAccessManagerTest
        {

            public TheSignUpMethod()
            {
                var databaseUserSubstitute = Substitute.For<IDatabaseUser>();
                databaseUserSubstitute.ApiToken.Returns("ApiToken");
                Database.User.Create(Arg.Any<IDatabaseUser>())
                    .Returns(Observable.Return(databaseUserSubstitute));
            }

            [Theory, LogIfTooSlow]
            [InlineData("susancalvin@psychohistorian.museum", null)]
            [InlineData("susancalvin@psychohistorian.museum", "")]
            [InlineData("susancalvin@psychohistorian.museum", " ")]
            [InlineData("susancalvin@", null)]
            [InlineData("susancalvin@", "")]
            [InlineData("susancalvin@", " ")]
            [InlineData("susancalvin@", "123456")]
            [InlineData("", null)]
            [InlineData("", "")]
            [InlineData("", " ")]
            [InlineData("", "123456")]
            [InlineData(null, null)]
            [InlineData(null, "")]
            [InlineData(null, " ")]
            [InlineData(null, "123456")]
            public void ThrowsIfYouPassInvalidParameters(string email, string password)
            {
                var actualEmail = email.ToEmail();
                var actualPassword = password.ToPassword();

                Action tryingToConstructWithEmptyParameters =
                    () => UserAccessManager.SignUp(actualEmail, actualPassword, true, 0, null).Wait();

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentException>();
            }

            [Fact, LogIfTooSlow]
            public async Task EmptiesTheDatabaseBeforeTryingToCreateTheUser()
            {
                await UserAccessManager.SignUp(Email, Password, TermsAccepted, CountryId, Timezone);

                Received.InOrder(async () =>
                {
                    await Database.Clear();
                    await Api.User.SignUp(Email, Password, TermsAccepted, CountryId, Timezone);
                });
            }

            [Fact, LogIfTooSlow]
            public async Task CallsTheSignUpMethodOfTheUserApi()
            {
                await UserAccessManager.SignUp(Email, Password, TermsAccepted, CountryId, Timezone);

                await Api.User.Received().SignUp(Email, Password, TermsAccepted, CountryId, Timezone);
            }

            [Fact, LogIfTooSlow]
            public async Task PersistsTheUserToTheDatabase()
            {
                await UserAccessManager.SignUp(Email, Password, TermsAccepted, CountryId, Timezone);

                await Database.User.Received().Create(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.Id == User.Id));
            }

            [Fact, LogIfTooSlow]
            public async Task PersistsTheUserWithTheSyncStatusSetToInSync()
            {
                await UserAccessManager.SignUp(Email, Password, TermsAccepted, CountryId, Timezone);

                await Database.User.Received().Create(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.SyncStatus == SyncStatus.InSync));
            }

            [Fact, LogIfTooSlow]
            public async Task AlwaysReturnsASingleResult()
            {
                await UserAccessManager
                        .SignUp(Email, Password, TermsAccepted, CountryId, Timezone)
                        .SingleAsync();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotRetryWhenTheApiThrowsSomethingOtherThanUserIsMissingApiTokenException()
            {
                var serverErrorException = Substitute.For<ServerErrorException>(Substitute.For<IRequest>(), Substitute.For<IResponse>(), "Some Exception");
                Api.User.SignUp(Email, Password, TermsAccepted, CountryId, Timezone)
                    .ReturnsThrowingTaskOf(serverErrorException);

                Action tryingToSignUpWhenTheApiIsThrowingSomeRandomServerErrorException =
                    () => UserAccessManager.SignUp(Email, Password, TermsAccepted, CountryId, Timezone).Wait();

                tryingToSignUpWhenTheApiIsThrowingSomeRandomServerErrorException
                    .Should().Throw<ServerErrorException>();
            }

            [Fact, LogIfTooSlow]
            public async Task SavesTheApiTokenToPrivateSharedStorage()
            {
                await UserAccessManager.SignUp(Email, Password, TermsAccepted, CountryId, Timezone);

                PrivateSharedStorageService.Received().SaveApiToken(Arg.Any<string>());
            }

            [Fact, LogIfTooSlow]
            public async Task SavesTheUserIdToPrivateSharedStorage()
            {
                await UserAccessManager.SignUp(Email, Password, TermsAccepted, CountryId, Timezone);

                PrivateSharedStorageService.Received().SaveUserId(Arg.Any<long>());
            }
        }

        public sealed class TheRefreshTokenMethod : UserAccessManagerTest
        {
            public TheRefreshTokenMethod()
            {
                var user = Substitute.For<IDatabaseUser>();
                user.Email.Returns(Email);
                user.ApiToken.Returns("ApiToken");
                Database.User.Single().Returns(Observable.Return(user));
                Database.User.Update(Arg.Any<IDatabaseUser>()).Returns(Observable.Return(user));
            }

            [Theory, LogIfTooSlow]
            [InlineData(null)]
            [InlineData("")]
            [InlineData(" ")]
            public void ThrowsIfYouPassInvalidParameters(string password)
            {
                Action tryingToRefreshWithInvalidParameters =
                    () => UserAccessManager.RefreshToken(password.ToPassword()).Wait();

                tryingToRefreshWithInvalidParameters
                    .Should().Throw<ArgumentException>();
            }

            [Fact, LogIfTooSlow]
            public async Task CallsTheGetMethodOfTheUserApi()
            {
                await UserAccessManager.RefreshToken(Password);

                await Api.User.Received().Get();
            }

            [Fact, LogIfTooSlow]
            public async Task PersistsTheUserToTheDatabase()
            {
                await UserAccessManager.RefreshToken(Password);

                await Database.User.Received().Update(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.Id == User.Id));
            }

            [Fact, LogIfTooSlow]
            public async Task PersistsTheUserWithTheSyncStatusSetToInSync()
            {
                await UserAccessManager.RefreshToken(Password);

                await Database.User.Received().Update(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.SyncStatus == SyncStatus.InSync));
            }

            [Fact, LogIfTooSlow]
            public async Task AlwaysReturnsASingleResult()
            {
                await UserAccessManager
                        .RefreshToken(Password)
                        .SingleAsync();
            }

            [Fact, LogIfTooSlow]
            public async Task SavesTheApiTokenToPrivateSharedStorage()
            {
                await UserAccessManager.RefreshToken(Password);

                PrivateSharedStorageService.Received().SaveApiToken(Arg.Any<string>());
            }

            [Fact, LogIfTooSlow]
            public async Task SavesTheUserIdToPrivateSharedStorage()
            {
                await UserAccessManager.RefreshToken(Password);

                PrivateSharedStorageService.Received().SaveUserId(Arg.Any<long>());
            }
        }

        public sealed class TheUserAccessUsingGoogleMethod : UserAccessManagerTest
        {
            public TheUserAccessUsingGoogleMethod()
            {
                var databaseUserSubstitute = Substitute.For<IDatabaseUser>();
                databaseUserSubstitute.ApiToken.Returns("ApiToken");
                Database.User.Create(Arg.Any<IDatabaseUser>())
                    .Returns(Observable.Return(databaseUserSubstitute));
            }

            private const string googleToken = "sometoken";

            [Fact, LogIfTooSlow]
            public async Task EmptiesTheDatabaseBeforeTryingToCreateTheUser()
            {
                await UserAccessManager.LoginWithGoogle(googleToken);

                Received.InOrder(async () =>
                {
                    await Database.Clear();
                    await Api.User.GetWithGoogle();
                });
            }

            [Fact, LogIfTooSlow]
            public async Task CallsTheGetWithGoogleOfTheUserApi()
            {
                await UserAccessManager.LoginWithGoogle(googleToken);

                await Api.User.Received().GetWithGoogle();
            }

            [Fact, LogIfTooSlow]
            public async Task PersistsTheUserToTheDatabase()
            {
                await UserAccessManager.LoginWithGoogle(googleToken);

                await Database.User.Received().Create(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.Id == User.Id));
            }

            [Fact, LogIfTooSlow]
            public async Task PersistsTheUserWithTheSyncStatusSetToInSync()
            {
                await UserAccessManager.LoginWithGoogle(googleToken);

                await Database.User.Received().Create(Arg.Is<IDatabaseUser>(receivedUser => receivedUser.SyncStatus == SyncStatus.InSync));
            }

            [Fact, LogIfTooSlow]
            public async Task AlwaysReturnsASingleResult()
            {
                await UserAccessManager
                        .LoginWithGoogle(googleToken)
                        .SingleAsync();
            }

            [Fact, LogIfTooSlow]
            public async Task SavesTheApiTokenToPrivateSharedStorage()
            {
                await UserAccessManager.LoginWithGoogle(googleToken);

                PrivateSharedStorageService.Received().SaveApiToken(Arg.Any<string>());
            }

            [Fact, LogIfTooSlow]
            public async Task SavesTheUserIdToPrivateSharedStorage()
            {
                await UserAccessManager.LoginWithGoogle(googleToken);

                PrivateSharedStorageService.Received().SaveUserId(Arg.Any<long>());
            }
        }
    }
}
