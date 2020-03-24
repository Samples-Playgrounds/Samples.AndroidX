using FluentAssertions;
using System;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class ClampExtensionTests
    {
        public abstract class TheClampMethodTest<T> where T : IComparable
        {
            public abstract T Min { get; }
            public abstract T Max { get; }

            public abstract T NumInInterval { get; }
            public abstract T NumLessThanMin { get; }
            public abstract T NumGreaterThanMax { get; }

            [Fact, LogIfTooSlow]
            public void ReturnsMinIfValueIsLessThanMin()
                => NumLessThanMin.Clamp(Min, Max).Should().Be(Min);

            [Fact, LogIfTooSlow]
            public void ReturnsMaxIfValueIsGreaterThanMax()
                => NumGreaterThanMax.Clamp(Min, Max).Should().Be(Max);

            [Fact, LogIfTooSlow]
            public void ReturnsTheSameValueIfValueIsInInterval()
                => NumInInterval.Clamp(Min, Max).Should().Be(NumInInterval);
        }

        public sealed class ClampIntTests : TheClampMethodTest<int>
        {
            public override int Min => 10;
            public override int Max => 20;
            public override int NumInInterval => 15;
            public override int NumLessThanMin => 5;
            public override int NumGreaterThanMax => 25;
        }

        public sealed class ClampFloatTests : TheClampMethodTest<float>
        {
            public override float Min => 12.45f;
            public override float Max => 24.898f;
            public override float NumInInterval => 19.999f;
            public override float NumLessThanMin => -20.1f;
            public override float NumGreaterThanMax => 1000.123f;
        }
    }
}
