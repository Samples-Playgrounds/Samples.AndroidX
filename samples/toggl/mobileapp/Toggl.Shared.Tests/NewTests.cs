using FluentAssertions;
using System;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class NewTests
    {
        public sealed class TheValueOrMethodForIntegers : TheValueOrMethodBase<int>
        {
            public TheValueOrMethodForIntegers() : base(1, 2) { }
        }

        public sealed class TheValueOrMethodForNullableIntegers : TheValueOrMethodBase<int?>
        {
            public TheValueOrMethodForNullableIntegers() : base(1, 2) { }
        }

        public sealed class TheValueOrMethodForStrings : TheValueOrMethodBase<string>
        {
            public TheValueOrMethodForStrings() : base("hello", "this is test") { }
        }

        public abstract class TheValueOrMethodBase<T>
        {
            private readonly T newValue;
            private readonly T oldValue;

            protected TheValueOrMethodBase(T newValue, T oldValue)
            {
                if (oldValue.Equals(default(T)))
                    throw new ArgumentException("Argument should not be default value.", nameof(oldValue));

                if (newValue.Equals(default(T)))
                    throw new ArgumentException("Argument should not be default value.", nameof(newValue));

                if (newValue.Equals(oldValue))
                    throw new ArgumentException("Second value should be different from first.");

                this.newValue = newValue;
                this.oldValue = oldValue;
            }

            [Fact, LogIfTooSlow]
            public void ReturnsOldValueForDefault()
            {
                var n = default(New<T>);

                var value = n.ValueOr(oldValue);

                value.Should().Be(oldValue);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsOldValueForNone()
            {
                var n = New<T>.None;

                var value = n.ValueOr(oldValue);

                value.Should().Be(oldValue);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsNewValueForExplicitelyConstructed()
            {
                var n = New<T>.Value(newValue);

                var value = n.ValueOr(oldValue);

                value.Should().Be(newValue);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsNewValueForImplicitelyConstructed()
            {
                var n = (New<T>)newValue;

                var value = n.ValueOr(oldValue);

                value.Should().Be(newValue);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsNewValueForExplicitelyConstructedWithDefaultValue()
            {
                var n = New<T>.Value(default(T));

                var value = n.ValueOr(oldValue);

                value.Should().Be(default(T));
            }

            [Fact, LogIfTooSlow]
            public void ReturnsNewValueForImplicitelyConstructedWithDefaultValue()
            {
                var n = (New<T>)default(T);

                var value = n.ValueOr(oldValue);

                value.Should().Be(default(T));
            }

            [Fact, LogIfTooSlow]
            public void ReturnsOldValueForNoneIfOldValueIsDefault()
            {
                var n = New<T>.None;

                var value = n.ValueOr(default(T));

                value.Should().Be(default(T));
            }
        }
    }
}
