using FluentAssertions;
using Toggl.Core.Helper;
using Xunit;

namespace Toggl.Core.Tests.Helpers
{
    public sealed class ColorTests
    {
        public sealed class TheIsValidHexColorMethod
        {
            [Theory, LogIfTooSlow]
            [InlineData("#1f1f1F", true)]
            [InlineData("#AFAFAF", true)]
            [InlineData("#1AFFa1", true)]
            [InlineData("#222fff", true)]
            [InlineData("#F00", true)]
            [InlineData("123456", false)]
            [InlineData("#afafah", false)]
            [InlineData("#123abce", false)]
            [InlineData("aFaE3f", false)]
            [InlineData("F00", false)]
            [InlineData("#afaf", false)]
            [InlineData("#F0h", false)]
            [InlineData(null, false)]
            public void ReturnsAppropriateValue(string color, bool isValid)
            {
                Colors.IsValidHexColor(color).Should().Be(isValid);
            }
        }
    }
}