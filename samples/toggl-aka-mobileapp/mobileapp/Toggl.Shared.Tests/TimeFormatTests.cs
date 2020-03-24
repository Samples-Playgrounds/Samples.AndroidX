using FluentAssertions;
using FsCheck.Xunit;
using System;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class TimeFormatTests
    {
        public sealed class TheFromLocalizedTimeFormatMethod
        {
            [Theory]
            [InlineData("h:mm A")]
            [InlineData("H:mm")]
            public void SetsTheCorrectLocalizedFormat(string localizedFormat)
            {
                var timeFormat = TimeFormat.FromLocalizedTimeFormat(localizedFormat);

                timeFormat.Localized.Should().Be(localizedFormat);
            }

            [Theory]
            [InlineData("h:mm A", "hh:mm tt")]
            [InlineData("H:mm", "H:mm")]
            public void SetsTheCorrectFormat(
                string localizedFormat, string expectedFormat)
            {
                var timeFormat = TimeFormat.FromLocalizedTimeFormat(localizedFormat);

                timeFormat.Format.Should().Be(expectedFormat);
            }

            [Property]
            public void ThrowsForUnsupportedFormats(string format)
            {
                if (format == "h:mm A" || format == "H:mm") return;

                Action fromFormat = () => TimeFormat.FromLocalizedTimeFormat(format);

                fromFormat.Should().Throw<ArgumentException>();
            }
        }
    }
}
