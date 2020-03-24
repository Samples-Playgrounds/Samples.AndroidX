using FluentAssertions;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class DateFormatTests
    {
        public sealed class TheFromHumanReadableFormatMethod
        {
            [Theory]
            [InlineData("MM/DD/YYYY")]
            [InlineData("DD-MM-YYYY")]
            [InlineData("MM-DD-YYYY")]
            [InlineData("YYYY-MM-DD")]
            [InlineData("DD/MM/YYYY")]
            [InlineData("DD.MM.YYYY")]
            public void SetsTheCorrectLocalizedFormat(string localizedFormat)
            {
                var dateFormat = DateFormat.FromLocalizedDateFormat(localizedFormat);

                dateFormat.Localized.Should().Be(localizedFormat);
            }

            [Theory]
            [InlineData("MM/DD/YYYY", "MM/dd/yyyy")]
            [InlineData("DD-MM-YYYY", "dd-MM-yyyy")]
            [InlineData("MM-DD-YYYY", "MM-dd-yyyy")]
            [InlineData("YYYY-MM-DD", "yyyy-MM-dd")]
            [InlineData("DD/MM/YYYY", "dd/MM/yyyy")]
            [InlineData("DD.MM.YYYY", "dd.MM.yyyy")]
            public void SetsTheCorrectLongFormat(
                string localizedFormat, string expectedLongFormat)
            {
                var dateFormat = DateFormat.FromLocalizedDateFormat(localizedFormat);

                dateFormat.Long.Should().Be(expectedLongFormat);
            }

            [Theory]
            [InlineData("MM/DD/YYYY", "MM/dd")]
            [InlineData("DD-MM-YYYY", "dd-MM")]
            [InlineData("MM-DD-YYYY", "MM-dd")]
            [InlineData("YYYY-MM-DD", "MM-dd")]
            [InlineData("DD/MM/YYYY", "dd/MM")]
            [InlineData("DD.MM.YYYY", "dd.MM")]
            public void SetsTheCorrectShortFormat(
                string localizedFormat, string expectedShortFormat)
            {
                var dateFormat = DateFormat.FromLocalizedDateFormat(localizedFormat);

                dateFormat.Short.Should().Be(expectedShortFormat);
            }
        }
    }
}
