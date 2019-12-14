using FluentAssertions;
using FsCheck;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.Exceptions;
using Toggl.Core.Interactors;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Networking.Exceptions;
using Toggl.Networking.Network;
using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Storage.Settings;
using Xunit;
using static Toggl.Core.UI.ViewModels.SignupViewModel;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SignupViewModelTests
    {
        public abstract class SignupViewModelTest : BaseViewModelWithInputTests<SignupViewModel, CredentialsParameter>
        {
            protected CredentialsParameter DefaultParameters { get; } = CredentialsParameter.Empty;

            protected Email ValidEmail { get; } = Email.From("susancalvin@psychohistorian.museum");
            protected Email InvalidEmail { get; } = Email.From("foo@");

            protected Password ValidPassword { get; } = Password.From("123456");
            protected Password InvalidPassword { get; } = Password.Empty;

            protected ILocation Location { get; } = Substitute.For<ILocation>();
            protected ILastTimeUsageStorage LastTimeUsageStorage { get; } = Substitute.For<ILastTimeUsageStorage>();

            protected override SignupViewModel CreateViewModel()
                => new SignupViewModel(
                    ApiFactory,
                    UserAccessManager,
                    AnalyticsService,
                    OnboardingStorage,
                    NavigationService,
                    ErrorHandlingService,
                    LastTimeUsageStorage,
                    TimeService,
                    SchedulerProvider,
                    RxActionFactory,
                    PlatformInfo);

            protected override void AdditionalSetup()
            {
                Location.CountryCode.Returns("LV");
                Location.CountryName.Returns("Latvia");

                Api.Location.Get().ReturnsTaskOf(Location);

                ApiFactory.CreateApiWith(Arg.Any<Credentials>()).Returns(Api);

                var container = new TestDependencyContainer { MockSyncManager = SyncManager };
                TestDependencyContainer.Initialize(container);
            }
        }

        public sealed class TheConstructor : SignupViewModelTest
        {
            [Xunit.Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useApiFactory,
                bool useUserAccessManager,
                bool useAnalyticsService,
                bool useOnboardingStorage,
                bool userNavigationService,
                bool useApiErrorHandlingService,
                bool useLastTimeUsageStorage,
                bool useTimeService,
                bool useSchedulerProvider,
                bool useRxActionFactory,
                bool usePlatformInfo)
            {
                var apiFactory = useApiFactory ? ApiFactory : null;
                var userAccessManager = useUserAccessManager ? UserAccessManager : null;
                var analyticsSerivce = useAnalyticsService ? AnalyticsService : null;
                var onboardingStorage = useOnboardingStorage ? OnboardingStorage : null;
                var navigationService = userNavigationService ? NavigationService : null;
                var apiErrorHandlingService = useApiErrorHandlingService ? ErrorHandlingService : null;
                var lastTimeUsageService = useLastTimeUsageStorage ? LastTimeUsageStorage : null;
                var timeService = useTimeService ? TimeService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var timezoneService = usePlatformInfo ? PlatformInfo : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SignupViewModel(
                        apiFactory,
                        userAccessManager,
                        analyticsSerivce,
                        onboardingStorage,
                        navigationService,
                        apiErrorHandlingService,
                        lastTimeUsageService,
                        timeService,
                        schedulerProvider,
                        rxActionFactory,
                        timezoneService);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheInitializeMethod : SignupViewModelTest
        {
            [FsCheck.Xunit.Property]
            public void SetsTheEmail(NonEmptyString emailString)
            {
                var viewModel = CreateViewModel();
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

            [Fact, LogIfTooSlow]
            public async Task GetstheCurrentLocation()
            {
                await ViewModel.Initialize(DefaultParameters);

                await Api.Location.Received().Get();
            }

            [Fact, LogIfTooSlow]
            public async Task SetsTheCountryButtonTitleToCountryNameWhenApiCallSucceeds()
            {
                var observer = TestScheduler.CreateObserver<string>();
                ViewModel.CountryButtonTitle.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameters);

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(1, Resources.SelectCountry),
                    ReactiveTest.OnNext(2, Location.CountryName)
                );
            }

            [Fact, LogIfTooSlow]
            public async Task SetsTheCountryButtonTitleToSelectCountryWhenApiCallFails()
            {
                var observer = TestScheduler.CreateObserver<string>();
                ViewModel.CountryButtonTitle.Subscribe(observer);

                Api.Location.Get().ReturnsThrowingTaskOf(new Exception());

                await ViewModel.Initialize(DefaultParameters);

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(1, Resources.SelectCountry)
                );
            }

            [Fact, LogIfTooSlow]
            public async Task SetsFailedToGetCountryToTrueWhenApiCallFails()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsCountryErrorVisible.Subscribe(observer);

                Api.Location.Get().ReturnsThrowingTaskOf(new Exception());

                await ViewModel.Initialize(DefaultParameters);

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(1, false),
                    ReactiveTest.OnNext(2, true)
                );
            }
        }

        public sealed class ThePickCountryMethod : SignupViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task NavigatesToSelectCountryViewModelPassingNullIfLocationApiFailed()
            {
                Api.Location.Get().ReturnsThrowingTaskOf(new Exception());
                await ViewModel.Initialize(DefaultParameters);

                ViewModel.PickCountry.Execute();

                TestScheduler.Start();
                await NavigationService.Received().Navigate<SelectCountryViewModel, long?, long?>(null, ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public async Task NavigatesToSelectCountryViewModelPassingCountryIdIfLocationApiSucceeded()
            {
                await ViewModel.Initialize(DefaultParameters);
                var selectedCountryId = await new GetAllCountriesInteractor()
                    .Execute()
                    .Select(countries => countries.Single(country => country.CountryCode == Location.CountryCode))
                    .Select(country => country.Id);

                ViewModel.PickCountry.Execute();

                TestScheduler.Start();
                await NavigationService.Received().Navigate<SelectCountryViewModel, long?, long?>(selectedCountryId, ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public async Task UpdatesTheCountryButtonTitle()
            {
                var observer = TestScheduler.CreateObserver<string>();
                ViewModel.CountryButtonTitle.Subscribe(observer);

                var selectedCountry = await new GetAllCountriesInteractor()
                    .Execute()
                    .Select(countries => countries.Single(country => country.Id == 1));
                NavigationService
                    .Navigate<SelectCountryViewModel, long?, long?>(Arg.Any<long?>(), ViewModel.View)
                    .Returns(selectedCountry.Id);
                await ViewModel.Initialize(DefaultParameters);

                ViewModel.PickCountry.Execute();

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(1, Resources.SelectCountry),
                    ReactiveTest.OnNext(2, Location.CountryName),
                    ReactiveTest.OnNext(3, selectedCountry.Name)
                );
            }

            [Fact, LogIfTooSlow]
            public async Task SetsFailedToGetCountryToFalse()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.IsCountryErrorVisible.Subscribe(observer);

                Api.Location.Get().ReturnsThrowingTaskOf(new Exception());
                NavigationService
                    .Navigate<SelectCountryViewModel, long?, long?>(Arg.Any<long?>(), ViewModel.View)
                    .Returns(1);
                await ViewModel.Initialize(DefaultParameters);

                ViewModel.PickCountry.Execute();

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(1, false),
                    ReactiveTest.OnNext(2, true),
                    ReactiveTest.OnNext(3, false)
                );
            }
        }

        public sealed class TheGoogleSignupMethod : SignupViewModelTest
        {
            protected override void AdditionalViewModelSetup()
            {
                base.AdditionalViewModelSetup();
                ViewModel.Initialize(DefaultParameters).Wait();
            }

            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheTermsOfServiceViewModel()
            {
                ViewModel.GoogleSignup.Execute();

                TestScheduler.Start();
                await NavigationService.Received().Navigate<TermsOfServiceViewModel, bool>(ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotTryToSignUpIfUserDoesNotAcceptTermsOfService()
            {
                NavigationService.Navigate<TermsOfServiceViewModel, bool>(ViewModel.View).Returns(false);

                ViewModel.GoogleSignup.Execute();

                TestScheduler.Start();
                UserAccessManager.DidNotReceive().SignUpWithGoogle(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<string>());
            }

            [Fact, LogIfTooSlow]
            public void ShowsTheTermsOfServiceViewModelOnlyOnceIfUserAcceptsTheTerms()
            {
                NavigationService.Navigate<TermsOfServiceViewModel, bool>(ViewModel.View).Returns(true);
                UserAccessManager
                    .SignUpWithGoogle(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<string>())
                    .Returns(Observable.Throw<Unit>(new Exception()));

                ViewModel.GoogleSignup.Execute();
                ViewModel.GoogleSignup.Execute();

                TestScheduler.Start();
                NavigationService.Received(1).Navigate<TermsOfServiceViewModel, bool>(ViewModel.View);
            }

            public sealed class WhenUserAcceptsTheTermsOfService : SignupViewModelTest
            {
                protected override void AdditionalSetup()
                {
                    base.AdditionalSetup();

                    NavigationService.Navigate<TermsOfServiceViewModel, bool>(View).Returns(true);
                }

                protected override void AdditionalViewModelSetup()
                {
                    base.AdditionalViewModelSetup();
                    ViewModel.Initialize(DefaultParameters).Wait();
                }

                [Fact, LogIfTooSlow]
                public void CallsTheUserAccessManager()
                {
                    ViewModel.GoogleSignup.Execute();

                    TestScheduler.Start();
                    UserAccessManager.Received().SignUpWithGoogle(Arg.Any<string>(), true, Arg.Any<int>(), Arg.Any<string>());
                }

                [Fact, LogIfTooSlow]
                public void DoesNothingWhenThePageIsCurrentlyLoading()
                {
                    var never = Observable.Never<Unit>();
                    UserAccessManager.SignUpWithGoogle(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<string>()).Returns(never);
                    ViewModel.GoogleSignup.Execute();
                    ViewModel.GoogleSignup.Execute();

                    TestScheduler.Start();
                    UserAccessManager.Received(1).SignUpWithGoogle(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<string>());
                }

                [Fact, LogIfTooSlow]
                public void SetsTheIsLoadingPropertyToTrue()
                {
                    var observer = TestScheduler.CreateObserver<bool>();
                    ViewModel.IsLoading.Subscribe(observer);

                    UserAccessManager.SignUpWithGoogle(Arg.Any<string>(), true, Arg.Any<int>(), Arg.Any<string>()).Returns(
                        Observable.Never<Unit>());

                    ViewModel.GoogleSignup.Execute();

                    TestScheduler.Start();
                    observer.Messages.AssertEqual(
                        ReactiveTest.OnNext(1, false),
                        ReactiveTest.OnNext(2, true)
                    );
                }

                [Fact, LogIfTooSlow]
                public void TracksGoogleSignupEvent()
                {
                    UserAccessManager.SignUpWithGoogle(Arg.Any<string>(), true, Arg.Any<int>(), Arg.Any<string>()).Returns(
                        Observable.Return(Unit.Default));

                    ViewModel.GoogleSignup.Execute();

                    TestScheduler.Start();
                    AnalyticsService.SignUp.Received().Track(AuthenticationMethod.Google);
                }

                [Fact, LogIfTooSlow]
                public void StopsTheViewModelLoadStateWhenItErrors()
                {
                    var observer = TestScheduler.CreateObserver<bool>();
                    ViewModel.IsLoading.Subscribe(observer);

                    UserAccessManager.SignUpWithGoogle(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<string>()).Returns(
                        Observable.Throw<Unit>(new GoogleLoginException(false)));

                    ViewModel.GoogleSignup.Execute();

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
                    UserAccessManager.SignUpWithGoogle(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<string>()).Returns(
                        Observable.Throw<Unit>(new GoogleLoginException(false)));

                    ViewModel.GoogleSignup.Execute();

                    TestScheduler.Start();
                    NavigationService.DidNotReceive().Navigate<MainViewModel>(ViewModel.View);
                }

                [Fact, LogIfTooSlow]
                public void DoesNotDisplayAnErrorMessageWhenTheUserCancelsTheRequestOnTheGoogleService()
                {
                    var hasErrorObserver = TestScheduler.CreateObserver<bool>();
                    ViewModel.HasError.Subscribe(hasErrorObserver);
                    var errorTextObserver = TestScheduler.CreateObserver<string>();
                    ViewModel.ErrorMessage.Subscribe(errorTextObserver);

                    UserAccessManager.SignUpWithGoogle(Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<string>()).Returns(
                        Observable.Throw<Unit>(new GoogleLoginException(true)));

                    ViewModel.GoogleSignup.Execute();

                    TestScheduler.Start();
                    errorTextObserver.Messages.AssertEqual(
                        ReactiveTest.OnNext(1, "")
                    );

                    hasErrorObserver.Messages.AssertEqual(
                        ReactiveTest.OnNext(2, false)
                    );
                }

                public sealed class WhenSignupSucceeds : SuccessfulSignupTest
                {
                    protected override void ExecuteCommand()
                    {
                        ViewModel.GoogleSignup.Execute();
                    }

                    protected override void AdditionalViewModelSetup()
                    {
                        base.AdditionalViewModelSetup();

                        ViewModel.Initialize(DefaultParameters).Wait();

                        UserAccessManager
                            .SignUpWithGoogle(Arg.Any<string>(), true, Arg.Any<int>(), Arg.Any<string>())
                            .Returns(Observable.Return(Unit.Default));

                        NavigationService.Navigate<TermsOfServiceViewModel, bool>(ViewModel.View).Returns(true);
                    }

                    [FsCheck.Xunit.Property]
                    public void SavesTheTimeOfLastLogin(DateTimeOffset now)
                    {
                        TimeService.CurrentDateTime.Returns(now);

                        var viewModel = CreateViewModel();
                        viewModel.AttachView(View);

                        viewModel.Initialize(DefaultParameters).Wait();
                        viewModel.GoogleSignup.Execute();

                        TestScheduler.Start();
                        LastTimeUsageStorage.Received().SetLogin(now);
                    }

                    [Fact, LogIfTooSlow]
                    public void FiresSuccessfulSignupEvent()
                    {
                        var viewModel = CreateViewModel();
                        viewModel.AttachView(View);
                        var observer = TestScheduler.CreateObserver<Unit>();
                        viewModel.SuccessfulSignup.Subscribe(observer);

                        viewModel.Initialize(DefaultParameters).Wait();
                        viewModel.GoogleSignup.Execute();

                        TestScheduler.Start();
                        observer.Messages.Count.Should().Be(1);
                    }
                }
            }
        }

        public sealed class TheTogglePasswordVisibilityMethod : SignupViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void SetsIsPasswordMaskedToFalseWhenItIsTrue()
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
            public void SetsIsPasswordMaskedToTrueWhenItIsFalse()
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

        public sealed class TheLoginMethod : SignupViewModelTest
        {
            [FsCheck.Xunit.Property]
            public void NavigatesToLoginViewModel(
                NonEmptyString email, NonEmptyString password)
            {
                ViewModel.SetEmail(Email.From(email.Get));
                ViewModel.SetPassword(Password.From(password.Get));

                ViewModel.Login.Execute();

                TestScheduler.Start();
                NavigationService
                    .Received()
                    .Navigate<LoginViewModel, CredentialsParameter>(
                        Arg.Is<CredentialsParameter>(parameter
                             => parameter.Email.Equals(Email.From(email.Get))
                             && parameter.Password.Equals(Password.From(password.Get))),
                        ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public void PassesTheCredentialsToLoginViewModelIfUserTriedToSignUpWithExistingEmail()
            {
                var request = Substitute.For<IRequest>();
                request.Endpoint.Returns(new Uri("http://any.url.com"));
                var exception = new EmailIsAlreadyUsedException(
                    new BadRequestException(
                        request, Substitute.For<IResponse>()
                    )
                );
                UserAccessManager
                    .SignUp(Arg.Any<Email>(), Arg.Any<Password>(), true, Arg.Any<int>(), Arg.Any<string>())
                    .Returns(
                        Observable.Throw<Unit>(exception)
                    );
                NavigationService.Navigate<TermsOfServiceViewModel, bool>(ViewModel.View);
                ViewModel.SetEmail(ValidEmail);
                ViewModel.SetPassword(ValidPassword);
                ViewModel.Signup.Execute();
                TestScheduler.Start();
                TestScheduler.Stop();
                ViewModel.Login.Execute();

                TestScheduler.Start();
                NavigationService
                    .Received()
                    .Navigate<LoginViewModel, CredentialsParameter>(
                        Arg.Is<CredentialsParameter>(
                            parameter => parameter.Email.Equals(ValidEmail)
                            && parameter.Password.Equals(ValidPassword)
                        ),
                        ViewModel.View
                    );
            }
        }

        public sealed class TheShakeFlags : SignupViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsNothingWhenEmailPasswordAndCountryAreValid()
            {
                var observer = TestScheduler.CreateObserver<ShakeTargets>();
                ViewModel.Shake.Subscribe(observer);

                await ViewModel.Initialize(DefaultParameters);

                ViewModel.SetEmail(ValidEmail);
                ViewModel.SetPassword(ValidPassword);

                ViewModel.Signup.Execute();

                TestScheduler.Start();
                observer.Messages.AssertEqual();
            }

            [Xunit.Theory]
            [InlineData("not an email", "123", true, ShakeTargets.Email | ShakeTargets.Password)]
            [InlineData("not an email", "1234567", true, ShakeTargets.Email)]
            [InlineData("this@is.email", "123", true, ShakeTargets.Password)]
            [InlineData("not an email", "123", false, ShakeTargets.Country | ShakeTargets.Email | ShakeTargets.Password)]
            [InlineData("not an email", "1234567", false, ShakeTargets.Country | ShakeTargets.Email)]
            [InlineData("this@is.email", "123", false, ShakeTargets.Country | ShakeTargets.Password)]
            public async Task ReturnsTheCorrectFlagForInvalidEmailPasswordOrCountry(string email, string password, bool countryIsSelected, ShakeTargets shakeTargets)
            {
                var observer = TestScheduler.CreateObserver<ShakeTargets>();
                ViewModel.Shake.Subscribe(observer);

                var currentEmail = Email.From(email);
                var currentPassword = Password.From(password);
                var expectedShakeTargets = shakeTargets;

                if (!countryIsSelected)
                {
                    Api.Location.Get().ReturnsThrowingTaskOf(new Exception());
                }

                await ViewModel.Initialize(DefaultParameters);

                ViewModel.SetEmail(currentEmail);
                ViewModel.SetPassword(currentPassword);

                ViewModel.Signup.Execute();

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(1, expectedShakeTargets)
                );
            }
        }

        public sealed class TheSignupMethod : SignupViewModelTest
        {
            protected override void AdditionalViewModelSetup()
            {
                base.AdditionalViewModelSetup();

                ViewModel.Initialize(DefaultParameters).Wait();

                ViewModel.SetEmail(ValidEmail);
                ViewModel.SetPassword(ValidPassword);
            }

            [Fact, LogIfTooSlow]
            public async Task NavigatesToTheTermsOfServiceViewModel()
            {
                ViewModel.Signup.Execute();

                TestScheduler.Start();
                await NavigationService.Received().Navigate<TermsOfServiceViewModel, bool>(ViewModel.View);
            }

            [Fact, LogIfTooSlow]
            public void DoesNotTryToSignUpIfUserDoesNotAcceptTermsOfService()
            {
                NavigationService.Navigate<TermsOfServiceViewModel, bool>(ViewModel.View).Returns(false);

                ViewModel.Signup.Execute();

                TestScheduler.Start();
                UserAccessManager.DidNotReceive().SignUp(
                    Arg.Any<Email>(),
                    Arg.Any<Password>(),
                    Arg.Any<bool>(),
                    Arg.Any<int>(),
                    Arg.Any<string>());
            }

            [Fact, LogIfTooSlow]
            public void ShowsTheTermsOfServiceViewModelOnlyOnceIfUserAcceptsTheTerms()
            {
                NavigationService.Navigate<TermsOfServiceViewModel, bool>(ViewModel.View).Returns(true);
                UserAccessManager
                    .SignUp(Arg.Any<Email>(), Arg.Any<Password>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<string>())
                    .Returns(Observable.Throw<Unit>(new Exception()));

                ViewModel.Signup.Execute();
                ViewModel.Signup.Execute();

                TestScheduler.Start();
                NavigationService.Received(1).Navigate<TermsOfServiceViewModel, bool>(ViewModel.View);
            }

            public sealed class WhenUserAcceptsTheTermsOfService : SignupViewModelTest
            {
                protected override void AdditionalSetup()
                {
                    base.AdditionalSetup();

                    NavigationService.Navigate<TermsOfServiceViewModel, bool>(View).Returns(true);
                }

                protected override void AdditionalViewModelSetup()
                {
                    base.AdditionalViewModelSetup();

                    ViewModel.Initialize(DefaultParameters).Wait();

                    ViewModel.SetEmail(ValidEmail);
                    ViewModel.SetPassword(ValidPassword);
                    ViewModel.Signup.Execute();
                }

                [Fact, LogIfTooSlow]
                public void SetsIsLoadingToTrue()
                {
                    var observer = TestScheduler.CreateObserver<bool>();
                    ViewModel.IsLoading.Subscribe(observer);

                    TestScheduler.Start();
                    observer.Messages.AssertEqual(
                        ReactiveTest.OnNext(1, true)
                    );
                }

                [Fact, LogIfTooSlow]
                public void TriesToSignUp()
                {
                    UserAccessManager.Received().SignUp(
                        ValidEmail,
                        ValidPassword,
                        true,
                        Arg.Any<int>(),
                        Arg.Any<string>());
                }

                [Fact, LogIfTooSlow]
                public void TracksSignupEvent()
                {
                    AnalyticsService.SignUp.Received().Track(AuthenticationMethod.EmailAndPassword);
                }

                public sealed class WhenSignupSucceeds : SuccessfulSignupTest
                {
                    protected override void ExecuteCommand()
                    {
                        ViewModel.Signup.Execute();
                    }

                    protected override void AdditionalViewModelSetup()
                    {
                        base.AdditionalViewModelSetup();

                        ViewModel.Initialize(DefaultParameters).Wait();

                        ViewModel.SetEmail(ValidEmail);
                        ViewModel.SetPassword(ValidPassword);
                        UserAccessManager
                            .SignUp(Arg.Any<Email>(), Arg.Any<Password>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<string>())
                            .Returns(Observable.Return(Unit.Default));
                        NavigationService.Navigate<TermsOfServiceViewModel, bool>(ViewModel.View).Returns(true);
                    }

                    [FsCheck.Xunit.Property]
                    public void SavesTheTimeOfLastLogin(DateTimeOffset now)
                    {
                        var viewModel = CreateViewModel();
                        viewModel.AttachView(View);
                        viewModel.Initialize(DefaultParameters).Wait();
                        viewModel.SetEmail(ValidEmail);
                        viewModel.SetPassword(ValidPassword);
                        TimeService.CurrentDateTime.Returns(now);

                        viewModel.Signup.Execute();

                        TestScheduler.Start();
                        LastTimeUsageStorage.Received().SetLogin(now);
                    }

                    [Fact, LogIfTooSlow]
                    public void FiresSuccessfulSignupEvent()
                    {
                        var viewModel = CreateViewModel();
                        viewModel.AttachView(View);
                        var observer = TestScheduler.CreateObserver<Unit>();
                        viewModel.SuccessfulSignup.Subscribe(observer);

                        viewModel.Initialize(DefaultParameters).Wait();
                        viewModel.SetEmail(ValidEmail);
                        viewModel.SetPassword(ValidPassword);

                        viewModel.Signup.Execute();

                        TestScheduler.Start();
                        observer.Messages.Count.Should().Be(1);
                    }
                }

                public sealed class WhenSignupFails : SignupViewModelTest
                {
                    private void prepareException(Exception exception)
                    {
                        UserAccessManager
                            .SignUp(Arg.Any<Email>(), Arg.Any<Password>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<string>())
                            .Returns(Observable.Throw<Unit>(exception));
                    }

                    protected override void AdditionalViewModelSetup()
                    {
                        base.AdditionalViewModelSetup();

                        ViewModel.Initialize(DefaultParameters).Wait();

                        ViewModel.SetEmail(ValidEmail);
                        ViewModel.SetPassword(ValidPassword);

                        NavigationService.Navigate<TermsOfServiceViewModel, bool>(ViewModel.View).Returns(true);
                    }

                    [Fact, LogIfTooSlow]
                    public void SetsIsLoadingToFalse()
                    {
                        var observer = TestScheduler.CreateObserver<bool>();
                        ViewModel.IsLoading.Subscribe(observer);

                        prepareException(new Exception());

                        ViewModel.Signup.Execute();

                        TestScheduler.Start();
                        observer.Messages.AssertEqual(
                            ReactiveTest.OnNext(1, false),
                            ReactiveTest.OnNext(2, true),
                            ReactiveTest.OnNext(3, false)
                        );
                    }

                    [Fact, LogIfTooSlow]
                    public void SetsIncorrectEmailOrPasswordErrorIfReceivedUnautghorizedException()
                    {
                        var observer = TestScheduler.CreateObserver<string>();
                        ViewModel.ErrorMessage.Subscribe(observer);

                        prepareException(new UnauthorizedException(
                            Substitute.For<IRequest>(),
                            Substitute.For<IResponse>()));

                        ViewModel.Signup.Execute();

                        TestScheduler.Start();
                        observer.Messages.AssertEqual(
                            ReactiveTest.OnNext(1, ""),
                            ReactiveTest.OnNext(2, Resources.IncorrectEmailOrPassword)
                        );
                    }

                    [Fact, LogIfTooSlow]
                    public void SetsEmailAlreadyUsedErrorIfReceivedEmailIsAlreadyusedException()
                    {
                        var observer = TestScheduler.CreateObserver<string>();
                        ViewModel.ErrorMessage.Subscribe(observer);

                        var request = Substitute.For<IRequest>();
                        request.Endpoint.Returns(new Uri("https://any.url.com"));
                        prepareException(new EmailIsAlreadyUsedException(
                            new BadRequestException(
                                request,
                                Substitute.For<IResponse>()
                            )
                        ));

                        ViewModel.Signup.Execute();

                        TestScheduler.Start();
                        observer.Messages.AssertEqual(
                            ReactiveTest.OnNext(1, ""),
                            ReactiveTest.OnNext(2, Resources.EmailIsAlreadyUsedError)
                        );
                    }

                    [Fact, LogIfTooSlow]
                    public void SetsGenereicErrorForAnyOtherException()
                    {
                        var observer = TestScheduler.CreateObserver<string>();
                        ViewModel.ErrorMessage.Subscribe(observer);

                        prepareException(new Exception());

                        ViewModel.Signup.Execute();

                        TestScheduler.Start();
                        observer.Messages.AssertEqual(
                            ReactiveTest.OnNext(1, ""),
                            ReactiveTest.OnNext(2, Resources.GenericSignUpError)
                        );
                    }

                    [Fact, LogIfTooSlow]
                    public void TracksTheEventAndException()
                    {
                        var exception = new Exception();
                        prepareException(exception);

                        ViewModel.Signup.Execute();

                        TestScheduler.Start();
                        AnalyticsService.UnknownSignUpFailure.Received()
                            .Track(exception.GetType().FullName, exception.Message);
                        AnalyticsService.Received().TrackAnonymized(exception);
                    }

                    [Fact, LogIfTooSlow]
                    public void DoesNotFiresSuccessfulSignupEvent()
                    {
                        var observer = TestScheduler.CreateObserver<Unit>();
                        ViewModel.SuccessfulSignup.Subscribe(observer);

                        prepareException(new Exception());

                        ViewModel.Signup.Execute();

                        TestScheduler.Start();
                        observer.Messages.Count.Should().Be(0);
                    }
                }
            }
        }

        public abstract class SuccessfulSignupTest : SignupViewModelTest
        {
            protected abstract void ExecuteCommand();

            [Fact, LogIfTooSlow]
            public void SetsIsNewUserToTrue()
            {
                ExecuteCommand();

                TestScheduler.Start();
                OnboardingStorage.Received().SetIsNewUser(true);
            }

            [Fact, LogIfTooSlow]
            public void SetsUserSignedUp()
            {
                ExecuteCommand();

                TestScheduler.Start();
                OnboardingStorage.Received().SetUserSignedUp();
            }

            [Fact, LogIfTooSlow]
            public void NavigatesToMainViewModel()
            {
                ExecuteCommand();

                TestScheduler.Start();
                NavigationService.Received().Navigate<MainTabBarViewModel>(ViewModel.View);
            }
        }

        public sealed class TheSignupEnabledProperty : SignupViewModelTest
        {
            protected override void AdditionalViewModelSetup()
            {
                ViewModel.Initialize(DefaultParameters).Wait();

                base.AdditionalViewModelSetup();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTrueWhenEmailAndPasswordAreValidAndIsNotLoading()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.SignupEnabled.Subscribe(observer);

                ViewModel.SetEmail(ValidEmail);
                ViewModel.SetPassword(ValidPassword);

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    ReactiveTest.OnNext(2, true)
                );
            }

            [Xunit.Theory]
            [InlineData("not an email", "123")]
            [InlineData("not an email", "1234567")]
            [InlineData("this@is.email", "123")]
            public void ReturnsFalseWhenEmailOrPasswordIsInvalid(string email, string password)
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.SignupEnabled.Subscribe(observer);

                ViewModel.SetEmail(Email.From(email));
                ViewModel.SetPassword(Password.From(password));

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    OnNext(2, false)
                );
            }

            [Fact]
            public void ReturnsFalseWhenIsLoading()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                ViewModel.SignupEnabled.Subscribe(observer);

                ViewModel.SetEmail(ValidEmail);
                ViewModel.SetPassword(ValidPassword);
                NavigationService.Navigate<TermsOfServiceViewModel, bool>(ViewModel.View).Returns(true);
                UserAccessManager
                    .SignUp(Arg.Any<Email>(), Arg.Any<Password>(), Arg.Any<bool>(), Arg.Any<int>(), Arg.Any<string>())
                    .Returns(Observable.Never<Unit>());
                ViewModel.Signup.Execute();

                TestScheduler.Start();
                observer.Messages.AssertEqual(
                    OnNext(2, true),
                    OnNext(3, false)
                );
            }
        }
    }
}
