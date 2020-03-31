using FluentAssertions;
using FsCheck;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Exceptions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Network;
using Toggl.Shared;
using Toggl.Storage.Settings;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class LoginViewModelTests
    {
        public abstract class LoginViewModelTest : BaseViewModelWithInputTests<LoginViewModel, CredentialsParameter>
        {
            protected Email ValidEmail { get; } = Email.From("person@company.com");
            protected Email InvalidEmail { get; } = Email.From("this is not an email");

            protected Password ValidPassword { get; } = Password.From("T0t4lly s4afe p4$$");
            protected Password InvalidPassword { get; } = Password.From("123");

            protected ILastTimeUsageStorage LastTimeUsageStorage { get; } = Substitute.For<ILastTimeUsageStorage>();

            protected override LoginViewModel CreateViewModel()
                => new LoginViewModel(
                    UserAccessManager,
                    AnalyticsService,
                    OnboardingStorage,
                    NavigationService,
                    ErrorHandlingService,
                    LastTimeUsageStorage,
                    TimeService,
                    SchedulerProvider,
                    RxActionFactory,
                    InteractorFactory);

            protected override void AdditionalSetup()
            {
                var container = new TestDependencyContainer { MockSyncManager = SyncManager };
                TestDependencyContainer.Initialize(container);
            }
        }

        public sealed class TheConstructor : LoginViewModelTest
        {
            [Xunit.Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useUserAccessManager,
                bool useAnalyticsService,
                bool useOnboardingStorage,
                bool userNavigationService,
                bool useApiErrorHandlingService,
                bool useLastTimeUsageStorage,
                bool useTimeService,
                bool useSchedulerProvider,
                bool useRxActionFactory,
                bool useInteractorFactory)
            {
                var userAccessManager = useUserAccessManager ? UserAccessManager : null;
                var analyticsSerivce = useAnalyticsService ? AnalyticsService : null;
                var onboardingStorage = useOnboardingStorage ? OnboardingStorage : null;
                var navigationService = userNavigationService ? NavigationService : null;
                var apiErrorHandlingService = useApiErrorHandlingService ? ErrorHandlingService : null;
                var lastTimeUsageStorage = useLastTimeUsageStorage ? LastTimeUsageStorage : null;
                var timeService = useTimeService ? TimeService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new LoginViewModel(userAccessManager,
                                             analyticsSerivce,
                                             onboardingStorage,
                                             navigationService,
                                             apiErrorHandlingService,
                                             lastTimeUsageStorage,
                                             timeService,
                                             schedulerProvider,
                                             rxActionFactory,
                                             interactorFactory);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheLoginEnabledObservable : LoginViewModelTest
        {
            [Xunit.Theory]
            [InlineData("invalid email address", "123")]
            [InlineData("invalid email address", "T0tally s4afe p4a$$")]
            [InlineData("person@company.com", "123")]
            public void ReturnsFalseWhenEmailOrPasswordIsInvalid(string email, string password)
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.LoginEnabled.Subscribe(observer);
                ViewModel.SetEmail(Email.From(email));
                ViewModel.SetPassword(Password.From(password));

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(2, false)
                );
            }

            [Xunit.Theory]
            [InlineData("invalid email address", "123")]
            [InlineData("invalid email address", "T0tally s4afe p4a$$")]
            [InlineData("person@company.com", "123")]
            public async Task ReturnsFalseWhenIsLoading(string email, string password)
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.LoginEnabled.Subscribe(observer);

                ViewModel.SetEmail(Email.From(email));
                ViewModel.SetPassword(Password.From(password));
                //Make sure isloading is true
                UserAccessManager
                    .Login(Arg.Any<Email>(), Arg.Any<Password>())
                    .Returns(Observable.Never<Unit>());
                ViewModel.Login();

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(2, false)
                );
            }
        }

        public sealed class TheLoginMethod : LoginViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void CallsTheUserAccessManagerWhenTheEmailAndPasswordAreValid()
            {
                ViewModel.SetEmail(ValidEmail);
                ViewModel.SetPassword(ValidPassword);

                ViewModel.Login();

                UserAccessManager.Received().Login(Arg.Is(ValidEmail), Arg.Is(ValidPassword));
            }

            [Fact, LogIfTooSlow]
            public void DoesNothingWhenThePageIsCurrentlyLoading()
            {
                var never = Observable.Never<Unit>();
                UserAccessManager.Login(Arg.Any<Email>(), Arg.Any<Password>()).Returns(never);
                ViewModel.SetEmail(ValidEmail);
                ViewModel.SetPassword(ValidPassword);
                ViewModel.Login();

                ViewModel.Login();

                UserAccessManager.Received(1).Login(Arg.Any<Email>(), Arg.Any<Password>());
            }

            public sealed class WhenLoginSucceeds : LoginViewModelTest
            {
                public WhenLoginSucceeds()
                {
                    ViewModel.SetEmail(ValidEmail);
                    ViewModel.SetPassword(ValidPassword);
                    UserAccessManager.Login(Arg.Any<Email>(), Arg.Any<Password>())
                        .Returns(Observable.Return(Unit.Default));
                }

                [Fact, LogIfTooSlow]
                public void SetsIsNewUserToFalse()
                {
                    ViewModel.Login();

                    OnboardingStorage.Received().SetIsNewUser(false);
                }

                [Fact, LogIfTooSlow]
                public void NavigatesToTheTimeEntriesViewModel()
                {
                    ViewModel.Login();

                    NavigationService.Received().Navigate<MainTabBarViewModel>(ViewModel.View);
                }

                [Fact, LogIfTooSlow]
                public void TracksTheLoginEvent()
                {
                    ViewModel.Login();

                    AnalyticsService.Received().Login.Track(AuthenticationMethod.EmailAndPassword);
                }

                [Fact, LogIfTooSlow]
                public void ReportsUserIdToAppCenter()
                {
                    var id = 1234567890L;
                    var user = Substitute.For<IThreadSafeUser>();
                    user.Id.Returns(id);
                    var observable = Observable.Return(user);
                    InteractorFactory.GetCurrentUser().Execute().Returns(observable);

                    ViewModel.Login();

                    AnalyticsService.Received().SetAppCenterUserId(id);
                }

                [FsCheck.Xunit.Property]
                public void SavesTheTimeOfLastLogin(DateTimeOffset now)
                {
                    TimeService.CurrentDateTime.Returns(now);
                    var viewModel = CreateViewModel();
                    viewModel.AttachView(View);
                    viewModel.SetEmail(ValidEmail);
                    viewModel.SetPassword(ValidPassword);

                    viewModel.Login();

                    LastTimeUsageStorage.Received().SetLogin(Arg.Is(now));
                }
            }

            public sealed class WhenLoginFails : LoginViewModelTest
            {
                public WhenLoginFails()
                {
                    ViewModel.SetEmail(ValidEmail);
                    ViewModel.SetPassword(ValidPassword);
                }

                [Fact, LogIfTooSlow]
                public void SetsIsLoadingToFalse()
                {
                    var observer = TestScheduler.CreateObserver<bool>();
                    ViewModel.IsLoading.Subscribe(observer);
                    UserAccessManager.Login(Arg.Any<Email>(), Arg.Any<Password>())
                        .Returns(Observable.Throw<Unit>(new Exception()));

                    ViewModel.Login();

                    TestScheduler.Start();
                    observer.Messages.AssertEqual(
                        ReactiveTest.OnNext(1, false),
                        ReactiveTest.OnNext(2, true),
                        ReactiveTest.OnNext(3, false)
                    );
                }

                [Fact, LogIfTooSlow]
                public void DoesNotNavigate()
                {
                    UserAccessManager.Login(Arg.Any<Email>(), Arg.Any<Password>())
                        .Returns(Observable.Throw<Unit>(new Exception()));

                    ViewModel.Login();

                    NavigationService.DidNotReceive().Navigate<MainViewModel>(ViewModel.View);
                }

                [Fact, LogIfTooSlow]
                public void SetsTheErrorMessageToIncorrectEmailOrPasswordWhenReceivedUnauthorizedException()
                {
                    var observer = TestScheduler.CreateObserver<string>();
                    ViewModel.ErrorMessage.Subscribe(observer);
                    var exception = new UnauthorizedException(
                        Substitute.For<IRequest>(), Substitute.For<IResponse>());
                    UserAccessManager.Login(Arg.Any<Email>(), Arg.Any<Password>())
                        .Returns(Observable.Throw<Unit>(exception));

                    ViewModel.Login();

                    TestScheduler.Start();
                    observer.Messages.AssertEqual(
                        ReactiveTest.OnNext(1, ""),
                        ReactiveTest.OnNext(2, Resources.IncorrectEmailOrPassword)
                    );
                }

                [Fact, LogIfTooSlow]
                public void SetsTheErrorMessageToNothingWhenGoogleLoginWasCanceled()
                {
                    var observer = TestScheduler.CreateObserver<string>();
                    var exception = new GoogleLoginException(true);
                    ViewModel.ErrorMessage.Subscribe(observer);
                    UserAccessManager.Login(Arg.Any<Email>(), Arg.Any<Password>())
                        .Returns(Observable.Throw<Unit>(exception));

                    ViewModel.Login();

                    TestScheduler.Start();
                    observer.Messages.AssertEqual(
                        ReactiveTest.OnNext(1, "")
                    );
                }

                [Fact, LogIfTooSlow]
                public void SetsTheErrorMessageToGenericLoginErrorForAnyOtherException()
                {
                    var observer = TestScheduler.CreateObserver<string>();
                    var exception = new Exception();
                    ViewModel.ErrorMessage.Subscribe(observer);
                    UserAccessManager.Login(Arg.Any<Email>(), Arg.Any<Password>())
                        .Returns(Observable.Throw<Unit>(exception));

                    ViewModel.Login();

                    TestScheduler.Start();
                    observer.Messages.AssertEqual(
                        ReactiveTest.OnNext(1, ""),
                        ReactiveTest.OnNext(2, Resources.GenericLoginError)
                    );
                }

                [Fact, LogIfTooSlow]
                public void DoesNothingWhenErrorHandlingServiceHandlesTheException()
                {
                    var observer = TestScheduler.CreateObserver<string>();
                    var exception = new Exception();
                    ViewModel.ErrorMessage.Subscribe(observer);
                    UserAccessManager.Login(Arg.Any<Email>(), Arg.Any<Password>())
                        .Returns(Observable.Throw<Unit>(exception));
                    ErrorHandlingService.TryHandleDeprecationError(Arg.Any<Exception>())
                        .Returns(true);

                    ViewModel.Login();

                    TestScheduler.Start();
                    observer.Messages.AssertEqual(
                        ReactiveTest.OnNext(1, "")
                    );
                }

                [Fact, LogIfTooSlow]
                public void TracksTheEventAndException()
                {
                    var exception = new Exception();
                    UserAccessManager.Login(Arg.Any<Email>(), Arg.Any<Password>())
                        .Returns(Observable.Throw<Unit>(exception));

                    ViewModel.Login();

                    AnalyticsService.UnknownLoginFailure.Received()
                        .Track(exception.GetType().FullName, exception.Message);
                    AnalyticsService.Received().TrackAnonymized(exception);
                }
            }
        }

        public sealed class TheGoogleLoginMethod : LoginViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void CallsTheUserAccessManager()
            {
                ViewModel.GoogleLogin();

                UserAccessManager.Received().LoginWithGoogle(Arg.Any<string>());
            }

            [Fact, LogIfTooSlow]
            public void DoesNothingWhenThePageIsCurrentlyLoading()
            {
                var never = Observable.Never<Unit>();
                View.GetGoogleToken().Returns(Observable.Return(""));
                UserAccessManager.LoginWithGoogle(Arg.Any<string>()).Returns(never);
                ViewModel.GoogleLogin();

                ViewModel.GoogleLogin();

                UserAccessManager.Received(1).LoginWithGoogle(Arg.Any<string>());
            }

            [Fact, LogIfTooSlow]
            public void NavigatesToTheTimeEntriesViewModelWhenTheLoginSucceeds()
            {
                View.GetGoogleToken().Returns(Observable.Return(""));
                UserAccessManager.LoginWithGoogle(Arg.Any<string>())
                    .Returns(Observable.Return(Unit.Default));

                ViewModel.GoogleLogin();

                NavigationService.Received().Navigate<MainTabBarViewModel>(ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public void TracksGoogleLoginEvent()
            {
                View.GetGoogleToken().Returns(Observable.Return(""));
                UserAccessManager.LoginWithGoogle(Arg.Any<string>())
                    .Returns(Observable.Return(Unit.Default));

                ViewModel.GoogleLogin();

                AnalyticsService.Received().Login.Track(AuthenticationMethod.Google);
            }

            [Fact, LogIfTooSlow]
            public void StopsTheViewModelLoadStateWhenItErrors()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsLoading.Subscribe(observer);
                View.GetGoogleToken().Returns(Observable.Return(""));
                UserAccessManager.LoginWithGoogle(Arg.Any<string>())
                    .Returns(Observable.Throw<Unit>(new GoogleLoginException(false)));

                ViewModel.GoogleLogin();

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(1, false),
                    ReactiveTest.OnNext(2, true),
                    ReactiveTest.OnNext(3, false)
                );
            }

            [Fact, LogIfTooSlow]
            public void DoesNotNavigateWhenTheLoginFails()
            {
                View.GetGoogleToken().Returns(Observable.Return(""));
                UserAccessManager.LoginWithGoogle(Arg.Any<string>())
                    .Returns(Observable.Throw<Unit>(new GoogleLoginException(false)));

                ViewModel.GoogleLogin();

                NavigationService.DidNotReceive().Navigate<MainViewModel>(ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotDisplayAnErrormessageWhenTheUserCancelsTheRequestOnTheGoogleService()
            {
                var observer = SchedulerProvider.TestScheduler.CreateObserver<string>();
                ViewModel.ErrorMessage.Subscribe(observer);
                View.GetGoogleToken().Returns(Observable.Return(""));
                UserAccessManager.LoginWithGoogle(Arg.Any<string>())
                    .Returns(Observable.Throw<Unit>(new GoogleLoginException(true)));

                ViewModel.GoogleLogin();

                SchedulerProvider.TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(1, "")
                );
            }

            [FsCheck.Xunit.Property]
            public void SavesTheTimeOfLastLogin(DateTimeOffset now)
            {
                TimeService.CurrentDateTime.Returns(now);
                View.GetGoogleToken().Returns(Observable.Return(""));
                UserAccessManager.LoginWithGoogle(Arg.Any<string>())
                    .Returns(Observable.Return(Unit.Default));
                var viewModel = CreateViewModel();
                viewModel.AttachView(View);

                viewModel.GoogleLogin();

                LastTimeUsageStorage.Received().SetLogin(Arg.Is(now));
            }
        }

        public sealed class TheTogglePasswordVisibilityMethod : LoginViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void SetsTheIsPasswordMaskedToFalseWhenItIsTrue()
            {
                var observer = TestScheduler.CreateObserver<bool>();

                ViewModel.IsPasswordMasked.Subscribe(observer);
                ViewModel.TogglePasswordVisibility();

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(1, true),
                    ReactiveTest.OnNext(2, false)
                );
            }

            [Fact, LogIfTooSlow]
            public void SetsTheIsPasswordMaskedToTrueWhenItIsFalse()
            {
                var observer = TestScheduler.CreateObserver<bool>();

                ViewModel.IsPasswordMasked.Subscribe(observer);
                ViewModel.TogglePasswordVisibility();

                ViewModel.TogglePasswordVisibility();

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(1, true),
                    ReactiveTest.OnNext(2, false),
                    ReactiveTest.OnNext(3, true)
                );
            }
        }

        public sealed class TheSignupCommand : LoginViewModelTest
        {
            [FsCheck.Xunit.Property]
            public void NavigatesToTheSignupViewModel(
                NonEmptyString emailString, NonEmptyString passwordString)
            {
                var email = Email.From(emailString.Get);
                var password = Password.From(passwordString.Get);
                ViewModel.SetEmail(email);
                ViewModel.SetPassword(password);

                ViewModel.Signup.Execute();

                TestScheduler.Start();
                NavigationService
                    .Received()
                    .Navigate<SignupViewModel, CredentialsParameter>(
                        Arg.Is<CredentialsParameter>(parameter
                            => parameter.Email.Equals(email)
                            && parameter.Password.Equals(password)
                        ), ViewModel.View
                    ).Wait();
            }
        }

        public sealed class ThePrepareMethod : LoginViewModelTest
        {
            [FsCheck.Xunit.Property]
            public void SetsTheEmail(NonEmptyString emailString)
            {
                var viewModel = CreateViewModel();
                viewModel.AttachView(View);
                var email = Email.From(emailString.Get);
                var password = Password.Empty;
                var parameter = CredentialsParameter.With(email, password);
                var expectedValues = new[] { Email.Empty.ToString(), email.TrimmedEnd().ToString() }.Distinct();
                var actualValues = new List<string>();
                viewModel.Email.Subscribe(actualValues.Add);

                viewModel.Initialize(parameter);

                TestScheduler.Start();
                CollectionAssert.AreEqual(expectedValues, actualValues);
            }

            [FsCheck.Xunit.Property]
            public void SetsThePassword(NonEmptyString passwordString)
            {
                var viewModel = CreateViewModel();
                viewModel.AttachView(View);
                var email = Email.Empty;
                var password = Password.From(passwordString.Get);
                var parameter = CredentialsParameter.With(email, password);
                var expectedValues = new[] { Password.Empty.ToString(), password.ToString() };
                var actualValues = new List<string>();
                viewModel.Password.Subscribe(actualValues.Add);

                viewModel.Initialize(parameter);

                TestScheduler.Start();
                CollectionAssert.AreEqual(expectedValues, actualValues);
            }
        }
    }
}
