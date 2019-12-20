using FluentAssertions;
using System;
using System.Collections.Generic;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class DateTimeOffsetExtensionsTests
    {
        public sealed class TheRoundFunction
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(SecondsWhichShouldBeRoundedDown))]
            public void ShouldRoundDownToTheNearestMinute(int second)
            {
                var time = new DateTimeOffset(2018, 02, 02, 6, 12, second, TimeSpan.Zero);

                var rounded = time.RoundToClosestMinute();

                rounded.Minute.Should().Be(time.Minute);
                rounded.Second.Should().Be(0);
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(SecondsWhichShouldBeRoundedUp))]
            public void ShouldRoundUpToTheNearestMinute(int second)
            {
                var time = new DateTimeOffset(2018, 02, 02, 6, 12, second, TimeSpan.Zero);

                var rounded = time.RoundToClosestMinute();

                rounded.Minute.Should().Be(13);
                rounded.Second.Should().Be(0);
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(SecondsWhichShouldBeRoundedUp))]
            public void ShouldRoundUpToTheNearestMinuteAndUpdateHourAndDayAndYearWhenNeeded(int second)
            {
                var time = new DateTimeOffset(2018, 12, 31, 23, 59, second, TimeSpan.Zero);

                var rounded = time.RoundToClosestMinute();

                rounded.Should().Be(new DateTimeOffset(2019, 01, 01, 00, 00, 00, TimeSpan.Zero));
            }

            public static IEnumerable<object[]> SecondsWhichShouldBeRoundedDown()
            {
                for (int i = 0; i < 30; i++)
                    yield return new object[] { i };
            }

            public static IEnumerable<object[]> SecondsWhichShouldBeRoundedUp()
            {
                for (int i = 30; i < 60; i++)
                    yield return new object[] { i };
            }
        }

        public sealed class TheBeginningOfWeekMethod
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(TestData))]
            public void ReturnsTheCorrectDate(
                DateTimeOffset now, BeginningOfWeek beginningOfWeek, DateTimeOffset expectedResult)
            {
                now.BeginningOfWeek(beginningOfWeek).Should().Be(expectedResult);
            }

            public static IEnumerable<object[]> TestData =>
            new List<object[]>
            {
                new object[]
                {
                    new DateTimeOffset(2019, 4, 29, 13, 13, 13, TimeSpan.Zero),
                    BeginningOfWeek.Monday,
                    new DateTimeOffset(2019, 4, 29, 0, 0, 0, DateTimeOffset.Now.Offset),
                },
                new object[]
                {
                    new DateTimeOffset(2019, 4, 29, 13, 13, 13, TimeSpan.Zero),
                    BeginningOfWeek.Tuesday,
                    new DateTimeOffset(2019, 4, 23, 0, 0, 0, DateTimeOffset.Now.Offset),
                },
                new object[]
                {
                    new DateTimeOffset(2019, 4, 29, 13, 13, 13, TimeSpan.Zero),
                    BeginningOfWeek.Wednesday,
                    new DateTimeOffset(2019, 4, 24, 0, 0, 0, DateTimeOffset.Now.Offset),
                },
                new object[]
                {
                    new DateTimeOffset(2019, 4, 29, 13, 13, 13, TimeSpan.Zero),
                    BeginningOfWeek.Thursday,
                    new DateTimeOffset(2019, 4, 25, 0, 0, 0, DateTimeOffset.Now.Offset),
                },
                new object[]
                {
                    new DateTimeOffset(2019, 4, 29, 13, 13, 13, TimeSpan.Zero),
                    BeginningOfWeek.Friday,
                    new DateTimeOffset(2019, 4, 26, 0, 0, 0, DateTimeOffset.Now.Offset),
                },
                new object[]
                {
                    new DateTimeOffset(2019, 4, 29, 13, 13, 13, TimeSpan.Zero),
                    BeginningOfWeek.Saturday,
                    new DateTimeOffset(2019, 4, 27, 0, 0, 0, DateTimeOffset.Now.Offset),
                },
                new object[]
                {
                    new DateTimeOffset(2019, 4, 29, 13, 13, 13, TimeSpan.Zero),
                    BeginningOfWeek.Sunday,
                    new DateTimeOffset(2019, 4, 28, 0, 0, 0, DateTimeOffset.Now.Offset),
                },
            };
        }
    }
}
