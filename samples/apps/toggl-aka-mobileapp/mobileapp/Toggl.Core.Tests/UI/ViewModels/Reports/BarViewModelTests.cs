using FluentAssertions;
using System;
using System.Collections.Generic;
using Toggl.Core.UI.ViewModels.Reports;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels.Reports
{
    public sealed class BarViewModelTests
    {
        public sealed class TheConstructor
        {
            [Theory]
            [InlineData(-0.1, 0.3)]
            [InlineData(0.1, -0.3)]
            [InlineData(-0.1, -0.3)]
            public void ThrowsForNegativeValues(double billable, double nonBillable)
            {
                // ReSharper disable once ObjectCreationAsStatement
                Action creatingBar = () => new BarViewModel(billable, nonBillable);

                creatingBar.Should().Throw<ArgumentOutOfRangeException>();
            }

            [Theory]
            [InlineData(1.1, 0.3)]
            [InlineData(0.1, 3.1)]
            public void ThrowsForPositiveValuesGreaterThanOne(double billable, double nonBillable)
            {
                // ReSharper disable once ObjectCreationAsStatement
                Action creatingBar = () => new BarViewModel(billable, nonBillable);

                creatingBar.Should().Throw<ArgumentOutOfRangeException>();
            }

            [Theory]
            [InlineData(0.6, 0.6)]
            [InlineData(0.4, 0.7)]
            public void ThrowsForTheSumBeingMoreThanOne(double billable, double nonBillable)
            {
                // ReSharper disable once ObjectCreationAsStatement
                Action creatingBar = () => new BarViewModel(billable, nonBillable);

                creatingBar.Should().Throw<ArgumentOutOfRangeException>();
            }

            [Theory]
            [MemberData(nameof(BillableAndNonBillablePairs))]
            public void HoldsTheDataWhenTheyAreValid(double billable, double nonBillable)
            {
                var vm = new BarViewModel(billable, nonBillable);

                vm.BillablePercent.Should().Be(billable);
                vm.NonBillablePercent.Should().Be(nonBillable);
            }

            public static IEnumerable<object[]> BillableAndNonBillablePairs()
            {
                double max = 100;
                for (var b = 0; b <= max; b += 5)
                {
                    for (var n = 0; n <= max - b; n += 5)
                    {
                        yield return new object[] { b / max, n / max };
                    }
                }
            }
        }
    }
}
