using FluentAssertions;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class RatingViewCriterionExtensionsTests
    {
        public sealed class TheToRatingViewCriterionMethod
        {
            [Theory]
            [InlineData("continue")]
            [InlineData("Continue")]
            [InlineData("CONTINUE")]
            [InlineData("coNTinUE")]
            public void ReturnsContinueForContinueArgument(string arg)
            {
                arg.ToRatingViewCriterion().Should()
                    .Be(RatingViewCriterion.Continue);
            }
            
            [Theory]
            [InlineData("stop")]
            [InlineData("Stop")]
            [InlineData("STOP")]
            [InlineData("sTOp")]
            public void ReturnsStopForStopArgument(string arg)
            {
                arg.ToRatingViewCriterion().Should()
                    .Be(RatingViewCriterion.Stop);
            }
            
            [Theory]
            [InlineData("start")]
            [InlineData("Start")]
            [InlineData("START")]
            [InlineData("sTARt")]
            public void ReturnsStartForStartArgument(string arg)
            {
                arg.ToRatingViewCriterion().Should()
                    .Be(RatingViewCriterion.Start);
            }
            
            [Theory]
            [InlineData("startA")]
            [InlineData("sttop")]
            [InlineData("")]
            [InlineData("cnt")]
            [InlineData(" ")]
            [InlineData(null)]
            public void ReturnsNoneForOtherArguments(string arg)
            {
                arg.ToRatingViewCriterion().Should()
                    .Be(RatingViewCriterion.None);
            }

        }
        
    }
}