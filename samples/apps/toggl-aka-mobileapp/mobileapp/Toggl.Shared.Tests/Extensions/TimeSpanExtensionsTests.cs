using FluentAssertions;
using System;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Shared.Tests.Extensions
{
    public sealed class TimeSpanExtensionsTests
    {
        public sealed class TheToFormattedStringMethod
        {
            public sealed class TheDecimalFormat
            {
                [Fact, LogIfTooSlow]
                public void WorksWithZero()
                {
                    var convertedValue = TimeSpan.Zero.ToFormattedString(DurationFormat.Decimal);

                    convertedValue.Should().Be("00.00 h");
                }

                [Fact, LogIfTooSlow]
                public void WorksWithMoreThan24Hours()
                {
                    var timeSpan = TimeSpan.FromHours(25);

                    var convertedValue = timeSpan.ToFormattedString(DurationFormat.Decimal);

                    convertedValue.Should().Be("25.00 h");
                }

                [Fact, LogIfTooSlow]
                public void WorksWithMoreThan99Hours()
                {
                    var timeSpan = TimeSpan.FromHours(125);

                    var convertedValue = timeSpan.ToFormattedString(DurationFormat.Decimal);

                    convertedValue.Should().Be("125.00 h");
                }

                [Fact, LogIfTooSlow]
                public void WorksForNormalCase()
                {
                    var timeSpan = TimeSpan.FromSeconds(5 * 60 * 60 + 6 * 60 + 3);

                    var convertedValue = timeSpan.ToFormattedString(DurationFormat.Decimal);

                    convertedValue.Should().Be("05.10 h");
                }
            }

            public sealed class TheImprovedFormat
            {
                [Fact, LogIfTooSlow]
                public void WorksWithZero()
                {
                    var convertedValue = TimeSpan.Zero.ToFormattedString(DurationFormat.Improved);

                    convertedValue.Should().Be("0:00:00");
                }

                [Fact, LogIfTooSlow]
                public void WorksWithMoreThan24Hours()
                {
                    var timeSpan = TimeSpan.FromHours(25);

                    var convertedValue = timeSpan.ToFormattedString(DurationFormat.Improved);

                    convertedValue.Should().Be("25:00:00");
                }

                [Fact, LogIfTooSlow]
                public void WorksForNormalCase()
                {
                    var timeSpan = TimeSpan.FromSeconds(5 * 60 * 60 + 4 * 60 + 3);

                    var convertedValue = timeSpan.ToFormattedString(DurationFormat.Improved);

                    convertedValue.Should().Be("5:04:03");
                }
            }

            public sealed class TheClassicFormat
            {
                [Fact, LogIfTooSlow]
                public void DoesNotAppendUnitIfTimeSpanIsLongerThanOneHour()
                {
                    var timeSpan = new TimeSpan(12, 32, 42);
                    var expected = "12:32:42";

                    var convertedValue = timeSpan.ToFormattedString(DurationFormat.Classic);

                    convertedValue.Should().Be(expected);
                }

                [Fact, LogIfTooSlow]
                public void AppendsTheMinuteUnitIfTimeSpanIsNotLongerThanOneHour()
                {
                    var timeSpan = new TimeSpan(0, 43, 59);
                    var expected = $"43:59 {Resources.UnitMin}";

                    var convertedValue = timeSpan.ToFormattedString(DurationFormat.Classic);

                    convertedValue.Should().Be(expected);
                }

                [Fact, LogIfTooSlow]
                public void AppendsTheSecondUnitIfTimeSpanIsLessThanOneMinute()
                {
                    var timeSpan = new TimeSpan(0, 0, 42);
                    var expected = $"42 {Resources.UnitSecond}";

                    var convertedValue = timeSpan.ToFormattedString(DurationFormat.Classic);

                    convertedValue.Should().Be(expected);
                }

                [Fact, LogIfTooSlow]
                public void WorksIfMinutesAreZero()
                {
                    var timeSpan = new TimeSpan(12, 0, 12);
                    var expected = $"12:00:12";

                    var convertedValue = timeSpan.ToFormattedString(DurationFormat.Classic);

                    convertedValue.Should().Be(expected);
                }

                [Fact, LogIfTooSlow]
                public void DoesNotRemoveLeadingZeroFromMinutes()
                {
                    var timeSpan = new TimeSpan(0, 6, 12);
                    var expected = $"06:12 {Resources.UnitMin}";

                    var convertedValue = timeSpan.ToFormattedString(DurationFormat.Classic);

                    convertedValue.Should().Be(expected);
                }

                [Fact, LogIfTooSlow]
                public void DoesNotRemoveLeadingZeroFromHours()
                {
                    var timeSpan = new TimeSpan(3, 6, 12);
                    var expected = $"03:06:12";

                    var convertedValue = timeSpan.ToFormattedString(DurationFormat.Classic);

                    convertedValue.Should().Be(expected);
                }

                [Fact, LogIfTooSlow]
                public void WorksForMoreThan24Hours()
                {
                    var timeSpan = new TimeSpan(43, 6, 12);
                    var expected = $"43:06:12";

                    var convertedValue = timeSpan.ToFormattedString(DurationFormat.Classic);

                    convertedValue.Should().Be(expected);
                }
            }
        }
    }
}
