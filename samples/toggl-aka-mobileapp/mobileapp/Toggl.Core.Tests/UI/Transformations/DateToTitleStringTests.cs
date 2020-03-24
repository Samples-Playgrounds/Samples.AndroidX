using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using System;
using System.Globalization;
using Toggl.Core.UI.Transformations;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.UI.Transformations
{
    public sealed class DateToTitleStringTests
    {
        public sealed class TheConvertMethod
        {
            private readonly DateTimeOffset now = new DateTimeOffset(2018, 10, 01, 23, 0, 0, TimeSpan.FromHours(-3));

            [Fact, LogIfTooSlow]
            public void ReturnsASpecialCaseStringForTheCurrentDay()
            {
                var date = now;

                var result = DateToTitleString.Convert(date, now);

                result.Should().Be(Resources.Today);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsASpecialCaseStringForThePreviousDay()
            {
                var date = now.AddDays(-1);

                var result = DateToTitleString.Convert(date, now);

                result.Should().Be(Resources.Yesterday);
            }

            [Property]
            public Property ReturnsAFormattedStringForAnyOtherDate()
            {
                var arb = Arb.Default.DateTimeOffset().Filter(d => d < now.AddDays(-1));
                return Prop.ForAll(arb, date =>
                {
                    var result = DateToTitleString.Convert(date, now);
                    var expectedCulture = CultureInfo.CreateSpecificCulture("en-US");
                    result.Should().Be(date.ToLocalTime().ToString("ddd, dd MMM", expectedCulture));
                });
            }
        }
    }
}
