using FluentAssertions;
using System;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class EnsureTests
    {
        public sealed class TheArgumentIsNotNullMethod
        {
            [Fact, LogIfTooSlow]
            public void ThrowsWhenTheArgumentIsNull()
            {
                const string argumentName = "argument";

                Action whenTheCalledArgumentIsNull =
                    () => Ensure.Argument.IsNotNull<string>(null, argumentName);

                whenTheCalledArgumentIsNull
                    .Should().Throw<ArgumentException>()
                    .WithMessage("Value cannot be null. (Parameter 'argument')");
            }

            [Fact, LogIfTooSlow]
            public void DoesNotThrowWhenTheArgumentIsNotNull()
            {
                Action whenTheCalledArgumentIsNull =
                    () => Ensure.Argument.IsNotNull("something", "argument");

                whenTheCalledArgumentIsNull.Should().NotThrow();
            }

            [Fact, LogIfTooSlow]
            public void WorksForValueTypes()
            {
                Action whenTheCalledArgumentIsNull =
                    () => Ensure.Argument.IsNotNull(0, "argument");

                whenTheCalledArgumentIsNull.Should().NotThrow();
            }
        }

        public sealed class TheArgumentIsNotNullOrWhiteSpaceMethod
        {
            [Fact, LogIfTooSlow]
            public void ThrowsWhenTheArgumentIsAnEmptyString()
            {
                Action whenTheCalledArgumentIsNull =
                    () => Ensure.Argument.IsNotNullOrWhiteSpaceString("", "argument");

                whenTheCalledArgumentIsNull
                    .Should().Throw<ArgumentException>()
                    .WithMessage("String cannot be empty. (Parameter 'argument')");
            }

            [Fact, LogIfTooSlow]
            public void ThrowsWhenTheArgumentIsABlankString()
            {
                Action whenTheCalledArgumentIsNull =
                    () => Ensure.Argument.IsNotNullOrWhiteSpaceString(" ", "argument");

                whenTheCalledArgumentIsNull
                    .Should().Throw<ArgumentException>()
                    .WithMessage("String cannot be empty. (Parameter 'argument')");
            }

            [Fact, LogIfTooSlow]
            public void ThrowsWhenTheArgumentIsNull()
            {
                const string argumentName = "argument";

                Action whenTheCalledArgumentIsNull =
                    () => Ensure.Argument.IsNotNullOrWhiteSpaceString(null, argumentName);

                whenTheCalledArgumentIsNull
                    .Should().Throw<ArgumentException>()
                    .WithMessage("Value cannot be null. (Parameter 'argument')");
            }

            [Fact, LogIfTooSlow]
            public void DoesNotThrowWhenTheArgumentIsNotNull()
            {
                Action whenTheCalledArgumentIsNull =
                    () => Ensure.Argument.IsNotNullOrWhiteSpaceString("something", "argument");

                whenTheCalledArgumentIsNull.Should().NotThrow();
            }
        }

        public sealed class TheUriIsAbsoluteMethod
        {
            [Fact, LogIfTooSlow]
            public void ThrowsWhenTheUriIsNotAbsolute()
            {
                const string argumentName = "argument";

                Action whenTheCalledArgumentIsNull =
                    () => Ensure.Argument.IsAbsoluteUri(new Uri("/something", UriKind.Relative), argumentName);

                whenTheCalledArgumentIsNull
                    .Should().Throw<ArgumentException>()
                    .WithMessage("Uri must be absolute. (Parameter 'argument')");
            }

            [Fact, LogIfTooSlow]
            public void DoesNotThrowWhenUriIsAbsolute()
            {
                Action whenTheCalledArgumentIsNull =
                    () => Ensure.Argument.IsAbsoluteUri(new Uri("http://www.toggl.com", UriKind.Absolute), "argument");

                whenTheCalledArgumentIsNull.Should().NotThrow();
            }

            [Fact, LogIfTooSlow]
            public void ThrowsIfTheUriIsNull()
            {
                Action whenTheCalledArgumentIsNull =
                    () => Ensure.Argument.IsAbsoluteUri(null, "argument");

                whenTheCalledArgumentIsNull
                    .Should().Throw<ArgumentException>()
                    .WithMessage("Value cannot be null. (Parameter 'argument')");
            }
        }

        public sealed class TheArgumentIsADefinedEnumValueMethod
        {
            [Fact, LogIfTooSlow]
            public void ThrowsWhenTheArgumentIsNotADefinedEnumValue()
            {
                const string argumentName = "argument";

                Action whenTheCalledArgumentIsNotADefinedEnumValue =
                    () => Ensure.Argument.IsADefinedEnumValue<BeginningOfWeek>((BeginningOfWeek)10, argumentName);

                whenTheCalledArgumentIsNotADefinedEnumValue
                    .Should().Throw<ArgumentException>()
                    .WithMessage("Invalid enum value. (Parameter 'argument')");
            }

            [Fact, LogIfTooSlow]
            public void DoesNotThrowWhenTheArgumentIsNotNull()
            {
                Action whenTheCalledArgumentIsNotADefinedEnumValue =
                    () => Ensure.Argument.IsADefinedEnumValue<BeginningOfWeek>(BeginningOfWeek.Monday, "argument");

                whenTheCalledArgumentIsNotADefinedEnumValue.Should().NotThrow();
            }
        }
    }
}
