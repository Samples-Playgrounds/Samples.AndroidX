using FluentAssertions;
using NSubstitute;
using System;
using Toggl.Core.Calendar;
using Toggl.Core.UI.Calendar;
using Xunit;

namespace Toggl.Core.Tests.Calendar
{
    public sealed class CalendarLayoutCalculatorTests
    {

        [Fact, LogIfTooSlow]
        public void WhenTheCalendarItemsDoNotOverlapWithEachOther()
        {
            var calendarItems = new[]
            {
                new CalendarItem("1", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 8, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(30), "Item 1",
                    CalendarIconKind.None),
                new CalendarItem("2", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 9, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(30), "Item 2",
                    CalendarIconKind.None),
                new CalendarItem("3", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 10, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(30), "Item 3",
                    CalendarIconKind.None),
            };

            var timeService = Substitute.For<ITimeService>();
            var calculator = new CalendarLayoutCalculator(timeService);

            var layoutAttributes = calculator.CalculateLayoutAttributes(calendarItems);

            layoutAttributes.Should().HaveSameCount(calendarItems)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[0].StartTime && attributes.TotalColumns == 1)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[1].StartTime && attributes.TotalColumns == 1)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[2].StartTime && attributes.TotalColumns == 1);
        }

        [Fact, LogIfTooSlow]
        public void WhenTwoItemsOverlap()
        {
            var calendarItems = new[]
            {
                new CalendarItem("1", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 8, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(90), "Item 1",
                    CalendarIconKind.None),
                new CalendarItem("2", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 9, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(30), "Item 2",
                    CalendarIconKind.None),
                new CalendarItem("3", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 10, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(30), "Item 3",
                    CalendarIconKind.None),
            };

            var timeService = Substitute.For<ITimeService>();
            var calculator = new CalendarLayoutCalculator(timeService);

            var layoutAttributes = calculator.CalculateLayoutAttributes(calendarItems);

            layoutAttributes.Should().HaveSameCount(calendarItems)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[0].StartTime && attributes.TotalColumns == 2 && attributes.ColumnIndex == 0)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[1].StartTime && attributes.TotalColumns == 2 && attributes.ColumnIndex == 1)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[2].StartTime && attributes.TotalColumns == 1);
        }

        [Fact, LogIfTooSlow]
        public void WhenTwoItemsShouldOverlapBecauseOfMinimumDurationForUIPurposes()
        {
            var calendarItems = new[]
            {
                new CalendarItem("1", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 8, 0, 0, TimeSpan.Zero), TimeSpan.FromSeconds(10), "Item 1",
                    CalendarIconKind.None),
                new CalendarItem("2", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 8, 0, 11, TimeSpan.Zero), TimeSpan.FromSeconds(10), "Item 2",
                    CalendarIconKind.None)
            };

            var timeService = Substitute.For<ITimeService>();
            var calculator = new CalendarLayoutCalculator(timeService);

            var layoutAttributes = calculator.CalculateLayoutAttributes(calendarItems);

            layoutAttributes.Should().HaveSameCount(calendarItems)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[0].StartTime && attributes.TotalColumns == 2 && attributes.ColumnIndex == 0)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[1].StartTime && attributes.TotalColumns == 2 && attributes.ColumnIndex == 1);
        }

        [Fact, LogIfTooSlow]
        public void WhenThreeItemsOverlapButOnlyTwoColumnsAreRequired()
        {
            var calendarItems = new[]
            {
                new CalendarItem("1", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 8, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(90), "Item 1",
                    CalendarIconKind.None),
                new CalendarItem("2", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 9, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(90), "Item 2",
                    CalendarIconKind.None),
                new CalendarItem("3", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 10, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(30), "Item 3",
                    CalendarIconKind.None),
            };

            var timeService = Substitute.For<ITimeService>();
            var calculator = new CalendarLayoutCalculator(timeService);

            var layoutAttributes = calculator.CalculateLayoutAttributes(calendarItems);

            layoutAttributes.Should().HaveSameCount(calendarItems)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[0].StartTime && attributes.TotalColumns == 2 && attributes.ColumnIndex == 0)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[1].StartTime && attributes.TotalColumns == 2 && attributes.ColumnIndex == 1)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[2].StartTime && attributes.TotalColumns == 2 && attributes.ColumnIndex == 0);
        }

        [Fact, LogIfTooSlow]
        public void WhenItemsOverlapInTwoDifferentGroupsWithDifferentNumberOfColumns()
        {
            var calendarItems = new[]
            {
                new CalendarItem("1", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 8, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(180), "Item 1",
                    CalendarIconKind.None),
                new CalendarItem("2", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 9, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(90), "Item 2",
                    CalendarIconKind.None),
                new CalendarItem("3", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 10, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(90), "Item 3",
                    CalendarIconKind.None),
                new CalendarItem("4", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 14, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(90), "Item 4",
                    CalendarIconKind.None),
                new CalendarItem("5", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 15, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(90), "Item 5",
                    CalendarIconKind.None),
            };

            var timeService = Substitute.For<ITimeService>();
            var calculator = new CalendarLayoutCalculator(timeService);

            var layoutAttributes = calculator.CalculateLayoutAttributes(calendarItems);

            layoutAttributes.Should().HaveSameCount(calendarItems)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[0].StartTime && attributes.TotalColumns == 3 && attributes.ColumnIndex == 0)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[1].StartTime && attributes.TotalColumns == 3 && attributes.ColumnIndex == 1)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[2].StartTime && attributes.TotalColumns == 3 && attributes.ColumnIndex == 2)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[3].StartTime && attributes.TotalColumns == 2 && attributes.ColumnIndex == 0)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[4].StartTime && attributes.TotalColumns == 2 && attributes.ColumnIndex == 1);
        }

        [Fact, LogIfTooSlow]
        public void CalendarEventsHaveTheirOwnColumnsToTheLeft()
        {
            var calendarItems = new[]
            {
                new CalendarItem("1", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 8, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(90), "Item 1",
                    CalendarIconKind.None),
                new CalendarItem("2", CalendarItemSource.Calendar,
                    new DateTimeOffset(2018, 11, 21, 9, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(60), "Item 2",
                    CalendarIconKind.None),
                new CalendarItem("3", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 9, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(30), "Item 3",
                    CalendarIconKind.None),
                new CalendarItem("4", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 11, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(120), "Item 4",
                    CalendarIconKind.None),
                new CalendarItem("5", CalendarItemSource.Calendar,
                    new DateTimeOffset(2018, 11, 21, 11, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(60), "Item 5",
                    CalendarIconKind.None),
            };

            var timeService = Substitute.For<ITimeService>();
            var calculator = new CalendarLayoutCalculator(timeService);

            var layoutAttributes = calculator.CalculateLayoutAttributes(calendarItems);

            layoutAttributes.Should().HaveSameCount(calendarItems)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[0].StartTime && attributes.TotalColumns == 3 && attributes.ColumnIndex == 1)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[1].StartTime && attributes.TotalColumns == 3 && attributes.ColumnIndex == 0)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[2].StartTime && attributes.TotalColumns == 3 && attributes.ColumnIndex == 2)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[3].StartTime &&
                    attributes.TotalColumns == 2 &&
                    calendarItems[3].Duration == attributes.Duration &&
                    attributes.ColumnIndex == 1)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[4].StartTime &&
                    attributes.TotalColumns == 2 &&
                    calendarItems[4].Duration == attributes.Duration &&
                    attributes.ColumnIndex == 0);
        }

        [Fact, LogIfTooSlow]
        public void OverlapingCalendarEventsAreAlwaysToTheLeft()
        {
            var calendarItems = new[]
            {
                new CalendarItem("1", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2018, 11, 21, 8, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(180), "Item 1",
                    CalendarIconKind.None),
                new CalendarItem("2", CalendarItemSource.Calendar,
                    new DateTimeOffset(2018, 11, 21, 9, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(180), "Item 2",
                    CalendarIconKind.None),
                new CalendarItem("3", CalendarItemSource.Calendar,
                    new DateTimeOffset(2018, 11, 21, 10, 0, 0, TimeSpan.Zero), TimeSpan.FromMinutes(180), "Item 3",
                    CalendarIconKind.None),
            };

            var timeService = Substitute.For<ITimeService>();
            var calculator = new CalendarLayoutCalculator(timeService);

            var layoutAttributes = calculator.CalculateLayoutAttributes(calendarItems);

            layoutAttributes.Should().HaveSameCount(calendarItems)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[0].StartTime && attributes.TotalColumns == 3 && attributes.ColumnIndex == 2)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[1].StartTime && attributes.TotalColumns == 3 && attributes.ColumnIndex == 0)
                .And.Contain((attributes) =>
                    attributes.StartTime == calendarItems[2].StartTime && attributes.TotalColumns == 3 && attributes.ColumnIndex == 1);
        }

        [Fact, LogIfTooSlow]
        public void GapsAreCalculatedFromTheBeginingOfTheDay()
        {
            var calendarItems = new[]
            {
                new CalendarItem("1", CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2019, 11, 21, 1, 30, 0, ((DateTimeOffset)DateTimeOffset.Now.LocalDateTime.Date).Offset), TimeSpan.FromMinutes(60), "Item 1",
                    CalendarIconKind.None)
            };

            var timeService = Substitute.For<ITimeService>();
            var calculator = new CalendarLayoutCalculator(timeService);

            var layoutAttributes = calculator.CalculateTwoHoursOrLessGapsLayoutAttributes(calendarItems);

            var dayStart = (DateTimeOffset)calendarItems[0].StartTime.LocalDateTime.Date;

            layoutAttributes.Should().HaveCount(1)
                .And.Contain((attributes) =>
                    attributes.StartTime == dayStart &&
                    attributes.Duration == TimeSpan.FromMinutes(90) &&
                    attributes.TotalColumns == 1 &&
                    attributes.ColumnIndex == 0);
        }

        [Fact, LogIfTooSlow]
        public void GapsAreCalculatedFromTheEndOfTheDay()
        {
            var calendarItems = new[]
            {
                new CalendarItem(
                    "1",
                    CalendarItemSource.TimeEntry,
                    new DateTimeOffset(2019, 11, 21, 21, 30, 0, DateTimeOffset.Now.Offset),
                    TimeSpan.FromMinutes(60),
                    "Item 1",
                    CalendarIconKind.None)
            };

            var timeService = Substitute.For<ITimeService>();
            var calculator = new CalendarLayoutCalculator(timeService);

            var layoutAttributes = calculator.CalculateTwoHoursOrLessGapsLayoutAttributes(calendarItems);

            var date = calendarItems[0].StartTime;
            var gapStart = date + TimeSpan.FromMinutes(60);

            layoutAttributes.Should().HaveCount(1)
                .And.Contain((attributes) =>
                    attributes.StartTime == gapStart &&
                    Math.Abs(attributes.Duration.TotalMinutes - 90.0) < 0.1 &&
                    attributes.TotalColumns == 1 &&
                    attributes.ColumnIndex == 0);
        }

        [Fact, LogIfTooSlow]
        public void GapsAreCalculatedBetweenCalendarItems()
        {
            var calendarItems = new[]
            {
                new CalendarItem("1", CalendarItemSource.TimeEntry, new DateTimeOffset(2019, 11, 21, 1, 30, 0, DateTimeOffset.Now.Offset), TimeSpan.FromMinutes(30), "Item 1", CalendarIconKind.None),
                new CalendarItem("1", CalendarItemSource.TimeEntry, new DateTimeOffset(2019, 11, 21, 3, 30, 0, DateTimeOffset.Now.Offset), TimeSpan.FromMinutes(30), "Item 1", CalendarIconKind.None),
            };

            var timeService = Substitute.For<ITimeService>();
            var calculator = new CalendarLayoutCalculator(timeService);

            var layoutAttributes = calculator.CalculateTwoHoursOrLessGapsLayoutAttributes(calendarItems);

            var date = calendarItems[0].StartTime;
            var gapStart = date + TimeSpan.FromMinutes(30);

            layoutAttributes.Should().HaveCount(2)
                .And.Contain((attributes) =>
                    attributes.StartTime == gapStart &&
                    Math.Abs(attributes.Duration.TotalMinutes - 90.0) < 0.1 &&
                    attributes.TotalColumns == 1 &&
                    attributes.ColumnIndex == 0);
        }

        [Fact, LogIfTooSlow]
        public void GapsAreCalculatedBetweenCalendarItemsThatAreInTwoOrMoreColumns()
        {
            var calendarItems = new[]
            {
                new CalendarItem("1", CalendarItemSource.TimeEntry, new DateTimeOffset(2019, 11, 21, 1, 30, 0, DateTimeOffset.Now.Offset), TimeSpan.FromMinutes(30), "Item 1", CalendarIconKind.None),
                new CalendarItem("2", CalendarItemSource.TimeEntry, new DateTimeOffset(2019, 11, 21, 1, 35, 0, DateTimeOffset.Now.Offset), TimeSpan.FromMinutes(20), "Item 2", CalendarIconKind.None),
                new CalendarItem("3", CalendarItemSource.TimeEntry, new DateTimeOffset(2019, 11, 21, 3, 30, 0, DateTimeOffset.Now.Offset), TimeSpan.FromMinutes(30), "Item 3", CalendarIconKind.None),
            };

            var timeService = Substitute.For<ITimeService>();
            var calculator = new CalendarLayoutCalculator(timeService);

            var layoutAttributes = calculator.CalculateTwoHoursOrLessGapsLayoutAttributes(calendarItems);

            var date = calendarItems[0].StartTime;
            var gapStart = date + TimeSpan.FromMinutes(30);

            layoutAttributes.Should().HaveCount(2)
                .And.Contain((attributes) =>
                    attributes.StartTime == gapStart &&
                    Math.Abs(attributes.Duration.TotalMinutes - 90.0) < 0.1 &&
                    attributes.TotalColumns == 1 &&
                    attributes.ColumnIndex == 0);
        }

        [Fact, LogIfTooSlow]
        public void GapsLongerThanTwoHoursAreNotCalculatedBetweenCalendarItemsOrEdges()
        {
            var calendarItems = new[]
            {
                new CalendarItem("1", CalendarItemSource.TimeEntry, new DateTimeOffset(2019, 11, 21, 2, 1, 0, DateTimeOffset.Now.Offset), TimeSpan.FromMinutes(30), "Item 1", CalendarIconKind.None),
                new CalendarItem("1", CalendarItemSource.TimeEntry, new DateTimeOffset(2019, 11, 21, 15, 00, 0, DateTimeOffset.Now.Offset), TimeSpan.FromMinutes(30), "Item 1", CalendarIconKind.None),
                new CalendarItem("1", CalendarItemSource.TimeEntry, new DateTimeOffset(2019, 11, 21, 21, 29, 0, DateTimeOffset.Now.Offset), TimeSpan.FromMinutes(30), "Item 1", CalendarIconKind.None),
            };

            var timeService = Substitute.For<ITimeService>();
            var calculator = new CalendarLayoutCalculator(timeService);

            var layoutAttributes = calculator.CalculateTwoHoursOrLessGapsLayoutAttributes(calendarItems);

            var date = calendarItems[0].StartTime;
            var gapStart = date + TimeSpan.FromMinutes(30);

            layoutAttributes.Should().BeEmpty();
        }
    }
}
