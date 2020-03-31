using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SelectColorViewModelTests
    {
        public abstract class SelectColorViewModelTest : BaseViewModelTests<SelectColorViewModel, ColorParameters, Color>
        {
            protected long EnoughTicksToEmitTheThrottledColor { get; } = TimeSpan.FromSeconds(1).Ticks;

            protected override SelectColorViewModel CreateViewModel()
                => new SelectColorViewModel(NavigationService, RxActionFactory, SchedulerProvider);
        }

        public sealed class TheConstructor : SelectColorViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useNavigationService,
                bool useRxActionFactory,
                bool useSchedulerProvider)
            {
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var schedulerProvider = useSchedulerProvider ? SchedulerProvider : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SelectColorViewModel(navigationService, rxActionFactory, schedulerProvider);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheSelectColorAction : SelectColorViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ChangesTheSelectedColor()
            {
                var initiallySelectedColor = Colors.DefaultProjectColors.First();
                var colorToSelect = Colors.DefaultProjectColors.Last();
                var parameters = ColorParameters.Create(initiallySelectedColor, true);
                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();
                ViewModel.Initialize(parameters);
                ViewModel.SelectableColors.Subscribe(observer);
                TestScheduler.AdvanceBy(EnoughTicksToEmitTheThrottledColor);

                ViewModel.SelectColor.Execute(colorToSelect);
                TestScheduler.AdvanceBy(EnoughTicksToEmitTheThrottledColor);

                observer.LastEmittedValue()
                    .Single(c => c.Selected)
                    .Color.Should().BeEquivalentTo(colorToSelect);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheSelectedColorIfCustomColorsAreNotAllowed()
            {
                var initiallySelectedColor = Colors.DefaultProjectColors.First();
                var colorToSelect = Colors.DefaultProjectColors.Last();
                var parameters = ColorParameters.Create(initiallySelectedColor, false);

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();

                await ViewModel.Initialize(parameters);
                ViewModel.SelectableColors.Subscribe(observer);

                ViewModel.SelectColor.Execute(colorToSelect);
                TestScheduler.AdvanceBy(EnoughTicksToEmitTheThrottledColor);

                (await ViewModel.Result).Should().Be(colorToSelect);
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotReturnIfCustomColorsAreAllowed()
            {
                var initiallySelectedColor = Colors.DefaultProjectColors.First();
                var colorToSelect = Colors.DefaultProjectColors.Last();
                var parameters = ColorParameters.Create(initiallySelectedColor, true);

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();

                ViewModel.Initialize(parameters);
                ViewModel.SelectableColors.Subscribe(observer);
                ViewModel.SelectColor.Execute(colorToSelect);
                TestScheduler.AdvanceBy(EnoughTicksToEmitTheThrottledColor);

                View.DidNotReceive().Close();
            }
        }

        public sealed class TheInitializeMethod : SelectColorViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void AddsFourteenItemsToTheListOfSelectableColorsIfTheUserIsNotPro()
            {
                var someColor = new Color(23, 45, 125);
                var parameters = ColorParameters.Create(someColor, false);

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();

                ViewModel.Initialize(parameters);
                ViewModel.SelectableColors.Subscribe(observer);
                TestScheduler.AdvanceBy(EnoughTicksToEmitTheThrottledColor);

                observer.LastEmittedValue()
                    .Should().HaveCount(14);
            }

            [Fact, LogIfTooSlow]
            public void AddsFifteenItemsToTheListOfSelectableColorsIfTheUserIsPro()
            {
                var someColor = new Color(23, 45, 125);
                var parameters = ColorParameters.Create(someColor, true);

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();

                ViewModel.Initialize(parameters);
                ViewModel.SelectableColors.Subscribe(observer);
                TestScheduler.AdvanceBy(EnoughTicksToEmitTheThrottledColor);

                observer.LastEmittedValue()
                    .Should().HaveCount(15);
            }

            [Fact, LogIfTooSlow]
            public void SelectsTheColorPassedAsTheParameter()
            {
                var passedColor = Colors.DefaultProjectColors.Skip(3).First();
                var parameters = ColorParameters.Create(passedColor, false);
                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();

                ViewModel.Initialize(parameters);
                ViewModel.SelectableColors.Subscribe(observer);
                TestScheduler.AdvanceBy(EnoughTicksToEmitTheThrottledColor);

                observer.LastEmittedValue()
                    .Single(c => c.Selected)
                    .Color.Should().Be(passedColor);
            }

            [Fact, LogIfTooSlow]
            public void SelectsTheFirstColorIfThePassedColorIsNotPartOfTheDefaultColorsAndWorkspaceIsNotPro()
            {
                var someColor = new Color(23, 45, 125);
                var expected = Colors.DefaultProjectColors.First();
                var parameters = ColorParameters.Create(someColor, false);
                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();

                ViewModel.Initialize(parameters);
                ViewModel.SelectableColors.Subscribe(observer);
                TestScheduler.Start();

                observer.LastEmittedValue()
                    .Single(c => c.Selected)
                    .Color.Should().Be(expected);
            }

            [Fact, LogIfTooSlow]
            public void SelectsThePassedColorIfThePassedColorIsNotPartOfTheDefaultColorsAndWorkspaceIsPro()
            {
                var someColor = new Color(23, 45, 125);
                var parameters = ColorParameters.Create(someColor, true);
                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();

                ViewModel.Initialize(parameters);
                ViewModel.SelectableColors.Subscribe(observer);
                TestScheduler.AdvanceBy(EnoughTicksToEmitTheThrottledColor);

                observer.LastEmittedValue()
                    .Single(c => c.Selected)
                    .Color.Should().Be(someColor);
            }
        }

        public class TheCloseWithDefaultResultMethod : SelectColorViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ClosesTheViewModel()
            {
                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheDefaultParameter()
            {
                var color = Colors.DefaultProjectColors.Last();
                var parameters = ColorParameters.Create(color, true);
                await ViewModel.Initialize(parameters);

                ViewModel.CloseWithDefaultResult();
                TestScheduler.Start();

                (await ViewModel.Result).Should().Be(color);
            }
        }

        public sealed class TheSaveCommand : SelectColorViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                ViewModel.Save.Execute();
                TestScheduler.Start();

                View.Received().Close();
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheSelectedColor()
            {
                var someColor = new Color(23, 45, 125);
                var parameters = ColorParameters.Create(someColor, true);
                await ViewModel.Initialize(parameters);
                TestScheduler.AdvanceBy(EnoughTicksToEmitTheThrottledColor);
                var expected = Colors.DefaultProjectColors.First();
                ViewModel.SelectColor.Execute(expected);
                TestScheduler.AdvanceBy(EnoughTicksToEmitTheThrottledColor);

                var toAwait = ViewModel.Save.ExecuteWithCompletion();
                TestScheduler.Start();
                await toAwait;

                (await ViewModel.Result).Should().Be(expected);
            }
        }
    }
}
