using FluentAssertions;
using System;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class EitherTests
    {
        public sealed class TheLeftProperty
        {
            [Fact, LogIfTooSlow]
            public void CannotBeAccessedInAnObjectConstructedWithRight()
            {
                var either = Either<string, bool>.WithRight(true);

                Action accessingLeftInARightObject =
                    () => { var a = either.Left; };

                accessingLeftInARightObject
                    .Should().Throw<InvalidOperationException>();
            }

            [Fact, LogIfTooSlow]
            public void CanBeAccessedInAnObjectConstructedWithLeft()
            {
                var either = Either<string, bool>.WithLeft("");

                Action accessingLeftInALeftObject =
                    () => { var a = either.Left; };

                accessingLeftInALeftObject.Should().NotThrow();
            }
        }

        public sealed class TheIsLeftProperty
        {
            [Fact, LogIfTooSlow]
            public void ShouldBeTrueForAnObjectCreatedWithLeft()
            {
                var either = Either<string, bool>.WithLeft("");

                either.IsLeft.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ShouldBeTrueForAnObjectCreatedWithRight()
            {
                var either = Either<string, bool>.WithRight(true);

                either.IsLeft.Should().BeFalse();
            }
        }

        public sealed class TheRightProperty
        {
            [Fact, LogIfTooSlow]
            public void CannotBeAccessedInAnObjectConstructedWithLeft()
            {
                var either = Either<string, bool>.WithLeft("");

                Action accessingRightInALeftObject =
                    () => { var a = either.Right; };

                accessingRightInALeftObject
                    .Should().Throw<InvalidOperationException>();
            }

            [Fact, LogIfTooSlow]
            public void CanBeAccessedInAnObjectConstructedWithRight()
            {
                var either = Either<string, bool>.WithRight(true);

                Action accessingRightInARightObject =
                    () => { var a = either.Right; };

                accessingRightInARightObject.Should().NotThrow();
            }
        }

        public sealed class TheIsRightProperty
        {
            [Fact, LogIfTooSlow]
            public void ShouldBeTrueForAnObjectCreatedWithRight()
            {
                var either = Either<string, bool>.WithRight(true);

                either.IsRight.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ShouldBeFalseForAnObjectCreatedWithLeft()
            {
                var either = Either<string, bool>.WithLeft("");

                either.IsRight.Should().BeFalse();
            }
        }
    }
}