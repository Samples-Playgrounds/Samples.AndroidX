﻿using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.Reactive.Testing;
using NSubstitute;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toggl.Core.Interactors;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SendFeedbackViewModelTests
    {
        public abstract class SendFeedbackViewModelTest : BaseViewModelWithOutputTests<SendFeedbackViewModel, bool>
        {
            protected override SendFeedbackViewModel CreateViewModel()
                => new SendFeedbackViewModel(NavigationService, InteractorFactory, SchedulerProvider, RxActionFactory);
        }

        public sealed class TheConstructor : SendFeedbackViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useNavigationService,
                bool useInteractorFactory,
                bool useSchedulerProvider,
                bool useRxActionFactory
            )
            {
                var navigationService = useNavigationService ? NavigationService : null;
                var interactorFactory = useInteractorFactory ? InteractorFactory : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;

                Action tryingToConstructWithEmptyParameters = ()
                    => new SendFeedbackViewModel(navigationService, interactorFactory, schedulerProvider, rxActionFactory);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheIsFeedbackEmptyObservable : SendFeedbackViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void EmitsValueBasedOnFeedbackText()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                viewModel.IsFeedbackEmpty.Subscribe(observer);
                viewModel.FeedbackText.OnNext(string.Empty);
                viewModel.FeedbackText.OnNext("some value");
                viewModel.FeedbackText.OnNext(string.Empty);

                TestScheduler.Start();
                observer.Messages.Select(r => r.Value.Value).TakeLast(3).AssertEqual(true, false, true);
            }

            [Fact, LogIfTooSlow]
            public void StartWithTrue()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                viewModel.IsFeedbackEmpty.Subscribe(observer);

                TestScheduler.Start();
                observer.Messages.First().Value.Value.Should().BeTrue();
            }
        }

        public sealed class TheSendEnabledProperty : SendFeedbackViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void StartsWithFalse()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                viewModel.SendEnabled.Subscribe(observer);

                TestScheduler.Start();
                observer.SingleEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void EmitsValueBasedOnFeedbackText()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();
                viewModel.SendEnabled.Subscribe(observer);

                viewModel.FeedbackText.OnNext(string.Empty);
                viewModel.FeedbackText.OnNext("some value");
                viewModel.FeedbackText.OnNext(string.Empty);

                TestScheduler.Start();
                observer.Messages.TakeLast(3).Select(r => r.Value.Value).AssertEqual(false, true, false);
            }

            [Fact, LogIfTooSlow]
            public void EmitsFalseWhenFeedbackBeingSent()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();
                viewModel.SendEnabled.Subscribe(observer);

                viewModel.FeedbackText.OnNext("some value");
                viewModel.Send.Execute();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeFalse();
            }
        }

        public sealed class TheIsLoadingProperty : SendFeedbackViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void EmitsTrueWhenFeedbackBeingSent()
            {
                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

                var mockedFeedbackInteractor = Substitute.For<IInteractor<IObservable<Unit>>>();
                InteractorFactory.SendFeedback(Arg.Any<string>()).Returns(mockedFeedbackInteractor);
                mockedFeedbackInteractor.Execute().Returns(Observable.Never<Unit>());
                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                viewModel.IsLoading.StartWith(false).Subscribe(observer);
                viewModel.FeedbackText.OnNext("some value");
                viewModel.Send.Execute();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void EmitsFalseAfterNetworkRequestFailed()
            {
                var mockedFeedbackInteractor = Substitute.For<IInteractor<IObservable<Unit>>>();
                InteractorFactory.SendFeedback(Arg.Any<string>()).Returns(mockedFeedbackInteractor);
                mockedFeedbackInteractor.Execute().Returns(Observable.Throw<Unit>(new Exception()));
                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                viewModel.IsLoading.StartWith(true).Subscribe(observer);
                viewModel.FeedbackText.OnNext("some value");
                viewModel.Send.Execute();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void DoesNotEmitFalseAfterNetworkRequestSucceed()
            {
                var mockedFeedbackInteractor = Substitute.For<IInteractor<IObservable<Unit>>>();
                mockedFeedbackInteractor.Execute().Returns(Observable.Return(Unit.Default));
                InteractorFactory.SendFeedback(Arg.Any<string>()).Returns(mockedFeedbackInteractor);
                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                viewModel.IsLoading.StartWith(true).Subscribe(observer);
                viewModel.FeedbackText.OnNext("some value");
                viewModel.Send.Execute();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void EmitsFalseWhenUserDismissTheView()
            {
                var observer = TestScheduler.CreateObserver<bool>();
                var viewModel = CreateViewModel();

                viewModel.IsLoading.StartWith(true).Subscribe(observer);
                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeFalse();
            }
        }

        public sealed class TheErrorObservable : SendFeedbackViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void EmitsNullWhenUserTapOnErrorView()
            {
                var observer = TestScheduler.CreateObserver<Exception>();
                var viewModel = CreateViewModel();

                viewModel.Error.StartWith(new Exception()).Subscribe(observer);
                viewModel.DismissError.Execute();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeNull();
            }

            [Fact, LogIfTooSlow]
            public void PassTheExceptionsForward()
            {
                var mockedFeedbackInteractor = Substitute.For<IInteractor<IObservable<Unit>>>();
                InteractorFactory.SendFeedback(Arg.Any<string>()).Returns(mockedFeedbackInteractor);
                var observer = TestScheduler.CreateObserver<Exception>();
                var viewModel = CreateViewModel();
                var exception = new Exception("nasty exception");

                viewModel.Error.Subscribe(observer);
                mockedFeedbackInteractor.Execute().Returns(Observable.Throw<Unit>(exception));
                viewModel.FeedbackText.OnNext("some value");
                viewModel.Send.Execute();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().Be(exception);
            }
        }

        public sealed class TheCloseButtonTappedAction : SendFeedbackViewModelTest
        {
            [Fact]
            public async Task SimplyClosesTheViewWhenTextIsEmpty()
            {
                ViewModel.FeedbackText.OnNext(string.Empty);

                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                View.Received().Close();
            }

            [Property]
            public void ShowsConfirmationDialogWhenFeedbackIsNotEmpty(NonEmptyString feedbackText)
            {
                ViewModel.FeedbackText.OnNext(feedbackText.Get);

                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                View.Received().ConfirmDestructiveAction(Arg.Any<ActionType>());
            }

            [Property]
            public void ClosesTheDialogWithoutSendingFeedbackWhenUserConfirmsDestructiveAction(NonEmptyString feedbackText)
            {
                View.ConfirmDestructiveAction(Arg.Any<ActionType>()).Returns(Observable.Return(true));
                ViewModel.FeedbackText.OnNext(feedbackText.Get);

                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                View.Received().Close();
            }

            [Property]
            public void DoesNotCloseTheDialogWhenUserCancelsDestructiveAction(NonEmptyString feedbackText)
            {
                View.ConfirmDestructiveAction(Arg.Any<ActionType>()).Returns(Observable.Return(false));
                ViewModel.FeedbackText.OnNext(feedbackText.Get);

                ViewModel.CloseWithDefaultResult();

                TestScheduler.Start();
                View.DidNotReceive().Close();
            }
        }

        public sealed class TheErrorViewTappedAction : SendFeedbackViewModelTest
        {
            [Fact]
            public void HidesTheErrorView()
            {
                var observer = TestScheduler.CreateObserver<Exception>();
                InteractorFactory.SendFeedback(Arg.Any<string>())
                    .Execute()
                    .Returns(Observable.Throw<Unit>(new Exception()));
                ViewModel.FeedbackText.OnNext("some feedback");
                ViewModel.Error.Subscribe(observer);

                ViewModel.Send.Execute();
                ViewModel.DismissError.Execute();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().BeNull();
            }
        }

        public sealed class TheSendButtonTappedAction : SendFeedbackViewModelTest
        {
            [Fact]
            public async Task SendsFeedback()
            {
                ViewModel.FeedbackText.OnNext("feedback");
                InteractorFactory.SendFeedback(Arg.Any<string>())
                    .Execute()
                    .Returns(Observable.Return(Unit.Default));

                ViewModel.Send.Execute();

                TestScheduler.Start();
                (await ViewModel.Result).Should().BeTrue();
            }

            [Fact]
            public void ShowsErrorWhenSendingFeedbackFails()
            {
                var observer = TestScheduler.CreateObserver<Exception>();
                ViewModel.FeedbackText.OnNext("feedback");
                var expectedException = new Exception("damn boom");
                InteractorFactory.SendFeedback(Arg.Any<string>())
                    .Execute()
                    .Returns(Observable.Throw<Unit>(expectedException));
                ViewModel.Error.Subscribe(observer);

                ViewModel.Send.Execute();

                TestScheduler.Start();
                observer.LastEmittedValue().Should().Be(expectedException);
            }
        }
    }
}
