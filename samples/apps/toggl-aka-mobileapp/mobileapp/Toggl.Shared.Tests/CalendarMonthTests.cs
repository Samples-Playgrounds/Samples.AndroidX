using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class CalendarMonthTests
    {
        private class MonthTestData : IEnumerable<object[]>
        {
            private readonly List<object[]> data
                = Enumerable.Range(1, 12)
                    .Select(i => new object[] { i })
                    .ToList();

            public IEnumerator<object[]> GetEnumerator()
                => data.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }

        public sealed class TheConstructor
        {
            [Fact, LogIfTooSlow]
            public void ThrowsIfTheYearIsNegative()
            {
                Action tryingToConstructWithNegativeYear =
                    () => new CalendarMonth(-100, 4);

                tryingToConstructWithNegativeYear
                    .Should().Throw<ArgumentOutOfRangeException>();
            }

            [Theory, LogIfTooSlow]
            [InlineData(0)]
            [InlineData(-1)]
            public void ThrowsIfTheMonthIsSmallerThan1(int month)
            {
                Action tryingToConstructWithNegativeYear =
                    () => new CalendarMonth(2018, month);

                tryingToConstructWithNegativeYear
                    .Should().Throw<ArgumentOutOfRangeException>();
            }

            [Theory, LogIfTooSlow]
            [InlineData(13)]
            [InlineData(120)]
            public void ThrowsIfTheMonthIsGreaterThan12(int month)
            {
                Action tryingToConstructWithNegativeYear =
                    () => new CalendarMonth(2018, month);

                tryingToConstructWithNegativeYear
                    .Should().Throw<ArgumentOutOfRangeException>();
            }
        }

        public sealed class TheNextMethod
        {
            [Theory, LogIfTooSlow]
            [ClassData(typeof(MonthTestData))]
            public void IncrementsTheMonthWhenMonthIsLessThan12(int month)
            {
                if (month == 12) return;
                var calendarMonth = new CalendarMonth(2018, month);

                var nextCalendarMonth = calendarMonth.Next();

                nextCalendarMonth.Month.Should().Be(month + 1);
            }

            [Theory, LogIfTooSlow]
            [ClassData(typeof(MonthTestData))]
            public void DoesNotIncrementTheYearWhenMonthIsLessThan12(int month)
            {
                if (month == 12) return;
                var year = 2020;
                var calendarMonth = new CalendarMonth(year, month);

                var nextCalendarMonth = calendarMonth.Next();

                nextCalendarMonth.Year.Should().Be(year);
            }

            [Property]
            public void IncreasesTheYearIfMonthIs12(NonNegativeInt nonNegativeInt)
            {
                var year = nonNegativeInt.Get;
                var calendarMonth = new CalendarMonth(year, 12);

                var nextCalendarMonth = calendarMonth.Next();

                nextCalendarMonth.Year.Should().Be(year + 1);
            }

            [Property]
            public void SetsTheMonthTo1IfMonthIs12(NonNegativeInt nonNegativeInt)
            {
                var year = nonNegativeInt.Get;
                var calendarMonth = new CalendarMonth(year, 12);

                var nextCalendarMonth = calendarMonth.Next();

                nextCalendarMonth.Month.Should().Be(1);
            }
        }

        public sealed class ThePreviousMethod
        {
            [Theory, LogIfTooSlow]
            [ClassData(typeof(MonthTestData))]
            public void DecrementsTheMonthIfMonthIsGreaterThan1(int month)
            {
                if (month == 1) return;
                var calendarMonth = new CalendarMonth(1234, month);

                var previousCalendarMonth = calendarMonth.Previous();

                previousCalendarMonth.Month.Should().Be(month - 1);
            }

            [Theory, LogIfTooSlow]
            [ClassData(typeof(MonthTestData))]
            public void DoesNotDecrementTheYearWhenMonthIsGreaterThan1(int month)
            {
                if (month == 1) return;
                var year = 2020;
                var calendarMonth = new CalendarMonth(year, month);

                var nextCalendarMonth = calendarMonth.Previous();

                nextCalendarMonth.Year.Should().Be(year);
            }

            [Property]
            public void DecreasesTheYearIfMonthIs1(PositiveInt positiveInt)
            {
                var year = positiveInt.Get;
                var calendarMonth = new CalendarMonth(year, 1);

                var nextCalendarMonth = calendarMonth.Previous();

                nextCalendarMonth.Year.Should().Be(year - 1);
            }

            [Property]
            public void SetsTheMonthTo12IfMonthIs1(PositiveInt positiveInt)
            {
                var year = positiveInt.Get;
                var calendarMonth = new CalendarMonth(year, 1);

                var nextCalendarMonth = calendarMonth.Previous();

                nextCalendarMonth.Month.Should().Be(12);
            }
        }

        public sealed class TheAddMonthsProperty
        {
            [Theory, LogIfTooSlow]
            [InlineData(2018, 1, 5, 2018, 6)]
            [InlineData(2018, 12, 12, 2019, 12)]
            [InlineData(2018, 12, 5, 2019, 5)]
            [InlineData(2018, 11, 50, 2023, 1)]
            [InlineData(2018, 11, 1, 2018, 12)]
            [InlineData(2014, 12, 0, 2014, 12)]
            [InlineData(2020, 1, 13, 2021, 2)]
            public void WorksForPositiveNumberOfMonths(
                int initialYear,
                int initialMonth,
                int monthsToAdd,
                int expectedYear,
                int expectedMonth)
            {
                var initialCalendarMonth
                    = new CalendarMonth(initialYear, initialMonth);

                var result = initialCalendarMonth.AddMonths(monthsToAdd);

                result.Year.Should().Be(expectedYear);
                result.Month.Should().Be(expectedMonth);
            }

            [Theory, LogIfTooSlow]
            [InlineData(2018, 7, -3, 2018, 4)]
            [InlineData(2014, 4, -12, 2013, 4)]
            [InlineData(2015, 1, -1, 2014, 12)]
            [InlineData(2016, 2, -52, 2011, 10)]
            [InlineData(2020, 12, -13, 2019, 11)]
            public void WorksForNegativeNumberOfMonths(
                int initialYear,
                int initialMonth,
                int monthsToAdd,
                int expectedYear,
                int expectedMonth)
            {
                var initialCalendarMonth
                    = new CalendarMonth(initialYear, initialMonth);

                var result = initialCalendarMonth.AddMonths(monthsToAdd);

                result.Year.Should().Be(expectedYear);
                result.Month.Should().Be(expectedMonth);
            }
        }
    }
}
