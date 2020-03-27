using FluentAssertions;
using System;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class EnumerableExtensions
    {
        public sealed class TheSumMethod
        {
            [Fact, LogIfTooSlow]
            public void CorrectlySumsTheTimeSpans()
            {
                var timespans = new[]
                {
                    TimeSpan.FromHours(2),
                    TimeSpan.FromMinutes(30),
                    TimeSpan.FromSeconds(30)
                };
                var expectedDuration =
                      TimeSpan.FromHours(2)
                    + TimeSpan.FromMinutes(30)
                    + TimeSpan.FromSeconds(30);

                timespans.Sum(ts => ts).Should().Be(expectedDuration);
            }

            [Fact, LogIfTooSlow]
            public void CorrectlySumsTheNullableTimeSpansWithDefaultNullSelector()
            {
                var timespans = new TimeSpan?[]
                {
                    TimeSpan.FromHours(3),
                    null,
                    null,
                    TimeSpan.FromMinutes(11),
                    null,
                    TimeSpan.FromSeconds(24)
                };
                var expectedDuration =
                      TimeSpan.FromHours(3)
                    + TimeSpan.FromMinutes(11)
                    + TimeSpan.FromSeconds(24);

                timespans.Sum(ts => ts).Should().Be(expectedDuration);
            }
        }
    }
}
