using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using System;
using System.Collections.Generic;
using Xunit;
using static Toggl.Shared.Math;

namespace Toggl.Shared.Tests
{
    public sealed class MathTests
    {
        public sealed class TheTimeToAngleMethod
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(MathTests.TimeToAngleTestData), MemberType = typeof(MathTests))]
            public void CalculatesCorrectAngleForTheGivenTime(double expectedAngle, int minutes, int seconds)
            {
                var angle = (new TimeSpan(0, minutes, seconds)).ToAngle();

                angle.Should().BeApproximately(expectedAngle, 0.001);
            }
        }

        public sealed class TheTimeToAngleOnTheDialMethod
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(MathTests.TimeToAngleTestData), MemberType = typeof(MathTests))]
            public void CalculatesCorrectAngleForTheGivenTimeAdjustedForUseOnTheClockRotatedBy90DegreesToTheLeft(double expectedAngle, int minutes, int seconds)
            {
                var angle = (new TimeSpan(0, minutes, seconds)).ToAngleOnTheDial();

                angle.Should().BeApproximately(expectedAngle - QuarterOfCircle, 0.001);
            }
        }

        public sealed class TheAngleToTimeMethod
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(MathTests.TimeToAngleTestData), MemberType = typeof(MathTests))]
            public void CalculatesCorrectTimeDayForAGivenAngle(double angle, int minutes, int seconds)
            {
                var twelveHourExpectedTimeOfDay = new TimeSpan(0, minutes, seconds);

                var timeOfDay = angle.AngleToTime();

                timeOfDay.TotalSeconds.Should().BeApproximately(minutes * 60 + seconds, 1);
            }
        }

        public sealed class TheAngleBetweenMethod
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(AngleBetweenTestData))]
            public void CalculatesTheAngleBetweenTwoPointsWell(Point a, Point b, double expectedAngle)
            {
                var angle = AngleBetween(a, b);

                angle.Should().BeApproximately(expectedAngle, 0.001);
            }

            public static IEnumerable<object[]> AngleBetweenTestData()
                => new List<object[]>
                {
                    new object[] { Point.Zero, Point.Zero, 0 },
                    new object[] { new Point { X = 5, Y = 3 }, new Point { X = 5, Y = 3 }, 0 },
                    new object[] { new Point { X = System.Math.Sqrt(3), Y = 1 }, Point.Zero, FullCircle / 12.0 }, // 30 deg
                    new object[] { new Point { X = 2, Y = 3 }, new Point { X = 1, Y = 2 }, FullCircle / 8.0 }, // 45 deg
                    new object[] { new Point { X = 1, Y = System.Math.Sqrt(3) }, Point.Zero, FullCircle / 6.0 }, // 60 deg
                    new object[] { new Point { X = 0, Y = 120 }, Point.Zero, QuarterOfCircle }, // 90 deg
                };
        }

        public sealed class TheDistasnceSqMethod
        {
            [Property]
            public void CalculatesTheSquaredDistanceBetweenTwoPointsCorrectly(NormalFloat nx1, NormalFloat ny1, NormalFloat nx2, NormalFloat ny2)
            {
                var x1 = nx1.Get;
                var x2 = nx2.Get;
                var y1 = ny1.Get;
                var y2 = ny2.Get;

                if (x1 > 10000000 || y1 > 10000000 || x2 > 10000000 || y2 > 10000000
                    || x1 < -10000000 || y1 < -10000000 || x2 < -10000000 || y2 < -10000000) return;

                var a = new Point { X = x1, Y = y1 };
                var b = new Point { X = x2, Y = y2 };
                var dx = x1 - x2;
                var dy = y1 - y2;
                var correctDistance = System.Math.Sqrt(dx * dx + dy * dy);

                var distanceSq = DistanceSq(a, b);

                System.Math.Sqrt(distanceSq).Should().BeApproximately(correctDistance, 0.001);
            }
        }

        public sealed class ThePointOnCircumferenceMethod
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(CircumferenceTestData))]
            public void CalculatesCorrectPositionOnCircumference(double angle, double radius, Point expectedPoint)
            {
                var point = PointOnCircumference(Point.Zero, angle, radius);

                point.X.Should().BeApproximately(expectedPoint.X, 0.001);
                point.Y.Should().BeApproximately(expectedPoint.Y, 0.001);
            }

            public static IEnumerable<object[]> CircumferenceTestData()
                => new List<object[]>
                {
                    new object[] { 0, 0, Point.Zero },
                    new object[] { System.Math.PI, 0, Point.Zero },
                    new object[] { 0, 1, new Point { X = 1, Y = 0 } },
                    new object[] { 0.5 * System.Math.PI, 1, new Point { X = 0, Y = 1 } },
                    new object[] { System.Math.PI, 1, new Point { X = -1, Y = 0 } },
                    new object[] { 1.5 * System.Math.PI, 1, new Point { X = 0, Y = -1 } },
                    new object[] { 2 * System.Math.PI, 1, new Point { X = 1, Y = 0 } },
                    new object[] { 4 * System.Math.PI, 1, new Point { X = 1, Y = 0 } },
                    new object[] { 4 * System.Math.PI, 25, new Point { X = 25, Y = 0 } },
                    new object[] { 1.0 / 6.0 * System.Math.PI, 6, new Point { X = 3 * System.Math.Sqrt(3), Y = 3 } },
                };
        }

        public sealed class TheIsBetweenMethod
        {
            [Theory, LogIfTooSlow]
            [MemberData(nameof(IsBetweenData))]
            public void DeterminesThatTheAngleIsInBetweenTheTwoOtherAngles(double angle, double start, double end)
            {
                var isBetween = angle.IsBetween(start, end);

                isBetween.Should().BeTrue();
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(IsBetweenData))]
            public void DeterminesThatTheAngleIsNotInBetweenTheTwoOtherAngles(double angle, double end, double start)
            {
                if (System.Math.Abs(angle.ToPositiveAngle() - start.ToPositiveAngle()) < 0.001
                    || System.Math.Abs(angle.ToPositiveAngle() - end.ToPositiveAngle()) < 0.001) return;

                var isBetween = angle.IsBetween(start, end);

                isBetween.Should().BeFalse();
            }

            public static IEnumerable<object[]> IsBetweenData()
                => new List<object[]>
                {
                    new object[] { 0, -0.1, 0.1 },
                    new object[] { 0.1, 0, 0.1 },
                    new object[] { 0, 0, 0.1 },
                    new object[] { 0.2, 0.1, 0.3 },
                    new object[] { System.Math.PI, QuarterOfCircle, FullCircle - QuarterOfCircle },
                    new object[] { QuarterOfCircle, -FullCircle + QuarterOfCircle, System.Math.PI },
                    new object[] { -System.Math.PI, QuarterOfCircle, -QuarterOfCircle },
                    new object[] { 2, 1, 3 },
                    new object[] { -2, -3, -1 }
                };
        }

        public sealed class ThePingPongClampMethod
        {
            [Theory]
            [MemberData(nameof(PingPongTestData))]
            public void ConvertsPositiveNumberToPingPongIndex(int number, int length, int expectedIndex)
            {
                var index = number.PingPongClamp(length);

                index.Should().Be(expectedIndex);
            }

            [Property]
            public void DifferenceBetweenTwoConsequentNonNegativeNumbersIsAlwaysOne(NonNegativeInt a, PositiveInt length)
            {
                if (length.Get == 1) return; // for a loop of length 1 all of the indexes will be 0

                var b = a.Get + 1;

                var indexA = a.Get.PingPongClamp(length.Get);
                var indexB = b.PingPongClamp(length.Get);

                System.Math.Abs(indexA - indexB).Should().Be(1);
            }

            [Property(MaxTest = 100000)]
            public void NeverReturnsIndexOutOfTheRange(NonNegativeInt number, PositiveInt length)
            {
                var index = number.Get.PingPongClamp(length.Get);

                index.Should().BeLessThan(length.Get);
            }

            [Property]
            public void ThrowsWhenTheNumberIsLessThanZero(NegativeInt number, PositiveInt length)
            {
                Action withNegativeNumber = () => number.Get.PingPongClamp(length.Get);

                withNegativeNumber.Should().Throw<ArgumentOutOfRangeException>();
            }

            [Property]
            public void ThrowsWhenTheLengthIsAZeroOrLess(NonNegativeInt number, NonNegativeInt length)
            {
                var nonPositiveLength = -1 * length.Get;

                Action withNegativeNumber = () => number.Get.PingPongClamp(nonPositiveLength);

                withNegativeNumber.Should().Throw<ArgumentOutOfRangeException>();
            }

            public static IEnumerable<object[]> PingPongTestData()
                => new[]
                {
                    new object[] { 0, 2, 0 },
                    new object[] { 1, 2, 1 },
                    new object[] { 2, 2, 0 },
                    new object[] { 3, 2, 1 },
                    new object[] { 4, 2, 0 },

                    new object[] { 0, 5, 0 },
                    new object[] { 1, 5, 1 },
                    new object[] { 2, 5, 2 },
                    new object[] { 3, 5, 3 },
                    new object[] { 4, 5, 4 },
                    new object[] { 5, 5, 3 },
                    new object[] { 6, 5, 2 },
                    new object[] { 7, 5, 1 },
                    new object[] { 8, 5, 0 },
                    new object[] { 9, 5, 1 },
                    new object[] { 10, 5, 2 },
                };
        }

        public static IEnumerable<object[]> TimeToAngleTestData()
        {
            var oneSecondAngle = FullCircle / (60.0 * 60.0);
            return new List<object[]>
            {
                new object[] { 0, 0, 0 },
                new object[] { (2 * 60 + 30) * oneSecondAngle, 2, 30 },
                new object[] { 5 * 60 * oneSecondAngle, 5, 0 },
                new object[] { (5 * 60 + 17) * oneSecondAngle, 5, 17 },
                new object[] { 10 * 60 * oneSecondAngle, 10, 0 },
                new object[] { (10 * 60 + 31) * oneSecondAngle, 10, 31 },
                new object[] { 15 * 60 * oneSecondAngle, 15, 0 },
                new object[] { (15 * 60 + 3) * oneSecondAngle, 15, 3 },
                new object[] { 20 * 60 * oneSecondAngle, 20, 0 },
                new object[] { (20 * 60 + 57) * oneSecondAngle, 20, 57 },
                new object[] { 25 * 60 * oneSecondAngle, 25, 0 },
                new object[] { (25 * 60 + 24) * oneSecondAngle, 25, 24 },
                new object[] { 30 * 60 * oneSecondAngle, 30, 0 },
                new object[] { (30 * 60 + 45) * oneSecondAngle, 30, 45 },
                new object[] { 35 * 60 * oneSecondAngle, 35, 0 },
                new object[] { (35 * 60 + 7) * oneSecondAngle, 35, 7 },
                new object[] { 40 * 60 * oneSecondAngle, 40, 0 },
                new object[] { (40 * 60 + 19) * oneSecondAngle, 40, 19 },
                new object[] { 45 * 60 * oneSecondAngle, 45, 0 },
                new object[] { (45 * 60 + 21) * oneSecondAngle, 45, 21 },
                new object[] { 50 * 60 * oneSecondAngle, 50, 0 },
                new object[] { (50 * 60 + 49) * oneSecondAngle, 50, 49 },
                new object[] { 55 * 60 * oneSecondAngle, 55, 0 },
                new object[] { (55 * 60 + 34) * oneSecondAngle, 55, 34 }
            };
        }
    }
}
