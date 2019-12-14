using FluentAssertions;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class DurationHelperTests
    {
        public sealed class TheLengthOfDurationPrefixMethod
        {
            [Theory]
            [InlineData("0:00:00", 7)]
            [InlineData("0:00:01", 5)]
            [InlineData("0:00:10", 5)]
            [InlineData("0:00:11", 5)]
            [InlineData("0:01:00", 2)]
            [InlineData("0:01:01", 2)]
            [InlineData("0:01:10", 2)]
            [InlineData("0:01:11", 2)]
            [InlineData("0:10:00", 2)]
            [InlineData("0:10:01", 2)]
            [InlineData("0:10:10", 2)]
            [InlineData("0:10:11", 2)]
            [InlineData("0:11:00", 2)]
            [InlineData("0:11:01", 2)]
            [InlineData("0:11:10", 2)]
            [InlineData("0:11:11", 2)]
            [InlineData("1:00:00", 0)]
            [InlineData("1:00:01", 0)]
            [InlineData("1:00:10", 0)]
            [InlineData("1:00:11", 0)]
            [InlineData("1:01:00", 0)]
            [InlineData("1:01:01", 0)]
            [InlineData("1:01:10", 0)]
            [InlineData("1:01:11", 0)]
            [InlineData("1:10:00", 0)]
            [InlineData("1:10:01", 0)]
            [InlineData("1:10:10", 0)]
            [InlineData("1:10:11", 0)]
            [InlineData("1:11:00", 0)]
            [InlineData("1:11:01", 0)]
            [InlineData("1:11:10", 0)]
            [InlineData("1:11:11", 0)]
            [InlineData("11:11:11", 0)]
            [InlineData("111:11:11", 0)]
            public void CalculatesTheLengthOfPrefixCorrectly(string duration, int expectedPrefixLength)
            {
                var prefixLength = DurationHelper.LengthOfDurationPrefix(duration);

                prefixLength.Should().Be(expectedPrefixLength);
            }
        }
    }
}
