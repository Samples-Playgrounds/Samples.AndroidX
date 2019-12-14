using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace Toggl.Shared.Tests
{
    public sealed class OptionTests
    {
        public sealed class TheSelectMethod
        {
            [Fact, LogIfTooSlow]
            public void MapsNoneToNone()
            {
                var Option = Option<int>.None;

                Option.Select(i => i * 2).Should().Be(Option<int>.None);
            }

            [Fact, LogIfTooSlow]
            public void MapsValueToSome()
            {
                var option = Option.Some(100);

                option.Select(i => i * 2).Should().Be(Option.Some(200));
            }
        }

        public sealed class TheSelectManyMethod
        {
            [Fact, LogIfTooSlow]
            public void MapsNoneToNone()
            {
                var option = Option<int>.None;

                option.SelectMany(i => Option.Some(i * 2)).Should().Be(Option<int>.None);
            }

            [Fact, LogIfTooSlow]
            public void MapsValueToSome()
            {
                var option = Option.Some(100);

                option.SelectMany(i => Option.Some(i * 2)).Should().Be(Option.Some(200));
            }

            [Fact, LogIfTooSlow]
            public void MapsValueToNone()
            {
                var option = Option.Some(100);

                option.SelectMany(i => Option<int>.None).Should().Be(Option<int>.None);
            }
        }

        public sealed class TheWhereMethod
        {
            [Fact, LogIfTooSlow]
            public void MapsNoneToNoneIfThePredicateReturnsFalse()
            {
                var Option = Option<int>.None;

                Option.Where(_ => false).Should().Be(Option<int>.None);
            }

            [Fact, LogIfTooSlow]
            public void MapsNoneToNoneIfThePredicateReturnsTrue()
            {
                var Option = Option<int>.None;

                Option.Where(_ => true).Should().Be(Option<int>.None);
            }

            [Fact, LogIfTooSlow]
            public void MapsSomeToNoneIfThePredicateReturnsFalse()
            {
                var option = Option.Some(100);

                option.Where(_ => false).Should().Be(Option<int>.None);
            }

            [Fact, LogIfTooSlow]
            public void MapsSomeToSomeIfThePredicateReturnsTrue()
            {
                var option = Option.Some(100);

                option.Where(_ => true).Should().Be(Option.Some(100));
            }
        }

        public sealed class TheMatchMethodWithOneParameter
        {
            [Fact, LogIfTooSlow]
            public void DoesNotCallOnSomeWithNone()
            {
                var option = Option<int>.None;

                option.Match(onSome: _ => throw new XunitException("Wrong method called"));
            }

            [Fact, LogIfTooSlow]
            public void CallsOnSomeWithValueOnSome()
            {
                var option = Option.Some(100);

                var isCalled = false;
                option.Match(
                    onSome: val =>
                    {
                        val.Should().Be(100);
                        isCalled = true;
                    });

                isCalled.Should().BeTrue("onSome should have been called");
            }
        }

        public sealed class TheMatchMethodWithTwoParameters
        {
            [Fact, LogIfTooSlow]
            public void CallsOnNoneOnNone()
            {
                var option = Option<int>.None;

                var isCalled = false;
                option.Match(
                    onSome: _ => throw new XunitException("Wrong method called"),
                    onNone: () => isCalled = true);

                isCalled.Should().BeTrue("onNone should have been called");
            }

            [Fact, LogIfTooSlow]
            public void CallsonSomeWithValueOnSome()
            {
                var option = Option.Some(100);

                var isCalled = false;
                option.Match(
                    onSome: val =>
                    {
                        val.Should().Be(100);
                        isCalled = true;
                    },
                    onNone: () => throw new XunitException("Wrong method called"));

                isCalled.Should().BeTrue("onSome should have been called");
            }
        }

        public sealed class TheMatchMethodThatReturns
        {
            [Fact, LogIfTooSlow]
            public void CallsOnNoneOnNoneAndReturnsItsValue()
            {
                var option = Option<int>.None;
                var expectedResult = "expected result";

                var actualResult = option.Match<string>(
                    onSome: _ => throw new XunitException("Wrong method called"),
                    onNone: () => expectedResult);

                actualResult.Should().Be(expectedResult, "onNone should have been called");
            }

            [Fact, LogIfTooSlow]
            public void CallsonSomeWithValueOnSomeAndReturnsItsValue()
            {
                var option = Option.Some(100);
                var expectedResult = "expected result";

                var actualResult = option.Match(
                    onSome: val => {
                        val.Should().Be(100);
                        return expectedResult;
                    },
                    onNone: () => throw new XunitException("Wrong method called"));

                actualResult.Should().Be(expectedResult, "onSome should have been called");
            }
        }

        public sealed class TheFromNullableMethod
        {
            [Fact, LogIfTooSlow]
            public void ReturnsNoneOnReferenceTypeNull()
            {
                Option.FromNullable((string)null).Should().Be(Option<string>.None);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsSomeOnReferenceTypeSet()
            {
                Option.FromNullable("the game").Should().Be(Option.Some("the game"));
            }

            [Fact, LogIfTooSlow]
            public void ReturnsNoneOnNullableNoValue()
            {
                Option.FromNullable((int?)null).Should().Be(Option<int>.None);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsSomeOnNullableWithValue()
            {
                Option.FromNullable((int?)10).Should().Be(Option.Some(10));
            }
        }
    }
}
