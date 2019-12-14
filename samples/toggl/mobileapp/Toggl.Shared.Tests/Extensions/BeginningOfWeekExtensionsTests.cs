using FluentAssertions;
using System.Globalization;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class BeginningOfWeekExtensionsTests
    {
        [Theory, LogIfTooSlow]
        [InlineData(BeginningOfWeek.Monday, "Monday")]
        [InlineData(BeginningOfWeek.Tuesday, "Tuesday")]
        [InlineData(BeginningOfWeek.Wednesday, "Wednesday")]
        [InlineData(BeginningOfWeek.Thursday, "Thursday")]
        [InlineData(BeginningOfWeek.Friday, "Friday")]
        [InlineData(BeginningOfWeek.Saturday, "Saturday")]
        [InlineData(BeginningOfWeek.Sunday, "Sunday")]
        public void LocalizesToEnglishProperly(BeginningOfWeek beginningOfWeek, string translation)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en");

            beginningOfWeek.ToLocalizedString().Should().Be(translation);
        }
    }
}
