using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Toggl.Shared.Tests
{
    public sealed class PasswordTests
    {
        public sealed class TheIsValidProperty
        {
            [Fact, LogIfTooSlow]
            public void ReturnsFalseForEmptyPassword()
            {
                Password.Empty.IsValid.Should().BeFalse();
            }

            [Theory, LogIfTooSlow]
            [InlineData("")]
            [InlineData("1")]
            [InlineData("pass")]
            [InlineData("12345")]
            [InlineData("    a")]
            [InlineData("\t a")]
            public void ReturnsFalseWhenPasswordIsShorterThan6Characters(
                string passwordString)
            {
                Password.From(passwordString).IsValid.Should().BeFalse();
            }

            [Theory, LogIfTooSlow]
            [InlineData("123456")]
            [InlineData("qwerty1")]
            [InlineData("12345\t ")]
            public void ReturnsTrueWhenPasswordIsLongerThan6Characters(
                string passwordString)
            {
                Password.From(passwordString).IsValid.Should().BeTrue();
            }
        }

        public sealed class TheEmptyProperty
        {
            [Fact, LogIfTooSlow]
            public void ReturnsEmptyPassword()
            {
                Password.Empty.ToString().Should().Be("");
            }
        }

        public sealed class TheToStringMethod
        {
            [Property]
            public void ReturnsTheSameStringThatPasswordWasCreatedWith(
                NonEmptyString nonEmptyString)
            {
                var passwordString = nonEmptyString.Get;
                var password = Password.From(passwordString);

                password.ToString().Should().Be(passwordString);
            }
        }
    }
}
