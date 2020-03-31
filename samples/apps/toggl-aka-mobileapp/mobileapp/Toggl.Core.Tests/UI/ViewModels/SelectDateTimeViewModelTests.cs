using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Toggl.Core.Tests.Generators;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public class SelectDateTimeViewModelTests
    {
        public class SelectDateTimeDialogViewModelTest : BaseViewModelTests<SelectDateTimeViewModel, DateTimePickerParameters, DateTimeOffset>
        {
            protected override SelectDateTimeViewModel CreateViewModel()
                => new SelectDateTimeViewModel(RxActionFactory, NavigationService);

            protected DateTimePickerParameters GenerateParameterForTime(DateTimeOffset now)
                => DateTimePickerParameters.WithDates(DateTimePickerMode.DateTime, now, now.AddHours(-1), now.AddHours(+1));
        }

        public class TheConstructor : SelectDateTimeDialogViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useRxActionFactory,
                bool useNavigationService)
            {
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameter
                    = () => new SelectDateTimeViewModel(rxActionFactory, navigationService);

                tryingToConstructWithEmptyParameter.Should().Throw<ArgumentNullException>();
            }
        }

        public class TheCurrentDateTimeCommand : SelectDateTimeDialogViewModelTest
        {
            [Property]
            public void DoesNotAcceptValuesGreaterThanTheMaxValue(DateTimeOffset now)
            {
                if (DateTimeOffset.MinValue.AddHours(1) <= now ||
                    DateTimeOffset.MaxValue.AddHours(-1) >= now) return;

                var parameter = GenerateParameterForTime(now);
                ViewModel.Initialize(parameter);

                ViewModel.CurrentDateTime.Accept(parameter.MaxDate.AddMinutes(3));

                ViewModel.CurrentDateTime.Value.Should().Be(parameter.MaxDate);
            }

            [Property]
            public void DoesNotAcceptValuesSmallerThanTheMinValue(DateTimeOffset now)
            {
                if (DateTimeOffset.MinValue.AddHours(1) <= now ||
                    DateTimeOffset.MaxValue.AddHours(-1) >= now) return;

                var parameter = GenerateParameterForTime(now);
                ViewModel.Initialize(parameter);

                ViewModel.CurrentDateTime.Accept(parameter.MinDate.AddMinutes(-3));

                ViewModel.CurrentDateTime.Value.Should().Be(parameter.MinDate);
            }
        }

        public class TheCloseMethod : SelectDateTimeDialogViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ClosesTheViewModel()
            {
                ViewModel.CloseWithDefaultResult();

                View.Received().Close();
            }

            [Property]
            public void ReturnsTheDefaultParameter(DateTimeOffset now)
            {
                if (DateTimeOffset.MinValue.AddHours(1) <= now ||
                    DateTimeOffset.MaxValue.AddHours(-1) >= now) return;

                var parameter = GenerateParameterForTime(now);
                ViewModel.Initialize(parameter);

                ViewModel.CloseWithDefaultResult();

                ViewModel.Result.GetAwaiter().GetResult().Should().Be(now);
            }
        }

        public class TheSaveCommand : SelectDateTimeDialogViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ClosesTheViewModel()
            {
                ViewModel.Initialize(GenerateParameterForTime(DateTimeOffset.Now));

                ViewModel.Save.Execute();
                
                View.Received().Close();
            }

            [Property]
            public void ReturnsAValueThatReflectsTheChangesToDuration(DateTimeOffset now, DateTimeOffset dateTimeOffset)
            {
                if (DateTimeOffset.MinValue.AddHours(1) <= dateTimeOffset ||
                    DateTimeOffset.MaxValue.AddHours(-1) >= dateTimeOffset) return;

                var parameter = GenerateParameterForTime(dateTimeOffset);
                parameter.CurrentDate = now;
                ViewModel.Initialize(parameter);
                ViewModel.CurrentDateTime.Accept(dateTimeOffset);

                ViewModel.Save.Execute();

                ViewModel.Result.GetAwaiter().GetResult().Should().Be(dateTimeOffset);
            }
        }
    }
}
