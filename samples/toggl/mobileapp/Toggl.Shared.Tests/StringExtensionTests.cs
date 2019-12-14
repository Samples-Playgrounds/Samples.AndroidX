using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using System;
using System.Linq;
using Toggl.Shared.Extensions;
using Xunit;
using static System.Math;

namespace Toggl.Shared.Tests
{
    public sealed class StringExtensionTests
    {
        public sealed class TheContainsIgnoringCaseMethod
        {
            [Property]
            public void MatchesSubstringsEvenIfTheyAreAllSearchedInAllCaps(string testString)
            {
                if (string.IsNullOrEmpty(testString)) return;

                var length = testString.Length;
                var startIndex = Gen.Choose(0, length - 1).Sample(1, 1).Single();
                var substring = testString.Substring(startIndex, length - startIndex);

                testString.ContainsIgnoringCase(substring.ToUpper()).Should().BeTrue();
            }

            [Property]
            public void MatchesSubstringsEvenIfTheyAreAllSearchedInAllLowerCase(string testString)
            {
                if (string.IsNullOrEmpty(testString)) return;

                var length = testString.Length;
                var startIndex = Gen.Choose(0, length - 1).Sample(1, 1).Single();
                var substring = testString.Substring(startIndex, length - startIndex);

                testString.ContainsIgnoringCase(substring.ToLower()).Should().BeTrue();
            }
        }

        public sealed class TheUnicodeSafeSubstringMethod
        {
            [Property]
            public void ThrowsIfTheStringIsNull(int startIndex, int length)
            {
                string str = null;

                Action tryingToGetSubstringFromNull =
                    () => str.UnicodeSafeSubstring(startIndex, length);

                tryingToGetSubstringFromNull.Should().Throw<ArgumentNullException>();
            }

            [Property]
            public void ReturnsEmptyStringIfTheStringIsEmpty(int startIndex)
            {
                "".UnicodeSafeSubstring(startIndex, 0).Should().Be("");
            }

            [Property]
            public void ThrowsIfStartIndexIsLessThanZero(
                NonEmptyString str, NonZeroInt nonZeroInt)
            {
                var startIndex = nonZeroInt.Get > 0 ? -nonZeroInt.Get : nonZeroInt.Get;

                Action tryingToGetSubstringWithNegativeStartIndex =
                    () => str.Get.UnicodeSafeSubstring(startIndex, 1);

                tryingToGetSubstringWithNegativeStartIndex
                    .Should().Throw<ArgumentOutOfRangeException>();
            }

            [Property]
            public void ThrowsIfStartIndexIsGreaterThanStringLength(
                NonEmptyString nonEmptyString, NonZeroInt startIndexOffset)
            {
                var str = nonEmptyString.Get;
                var stringLength = str.LengthInGraphemes();
                var startIndex = stringLength + Abs(startIndexOffset.Get);

                Action tryingToGetSubstringWithStartIndexGreaterThanStringLength
                    = () => str.Substring(startIndex, 1);

                tryingToGetSubstringWithStartIndexGreaterThanStringLength
                    .Should().Throw<ArgumentOutOfRangeException>();
            }

            [Theory, LogIfTooSlow]
            [InlineData("Hello", 0, 6)]
            [InlineData("Hello", 3, 3)]
            [InlineData("Hello", 4, 2)]
            [InlineData("🐶👻👨🤷", 0, 5)]
            [InlineData("👻👻❤️👻", 2, 3)]
            [InlineData("👻😂👻👻", 2, 6)]
            [InlineData("Mixed ݗ ྒྷ ᯼ č 㡨", 0, 16)]
            [InlineData("Mixed ݗ ྒྷ ᯼ č 㡨", 15, 2)]
            [InlineData("Mixed ݗ ྒྷ ᯼ č 㡨", 5, 30)]
            public void ThrowsIfLengthPlusStartIsGreaterThanStringLength(
                string str, int start, int length)
            {
                Action tryingToGetSubstringThatGoesOutOfStringBounds =
                    () => str.UnicodeSafeSubstring(start, length);

                tryingToGetSubstringThatGoesOutOfStringBounds
                    .Should().Throw<ArgumentOutOfRangeException>();
            }

            [Theory, LogIfTooSlow]
            [InlineData("Hello", 0, 5, "Hello")]
            [InlineData("Hello", 3, 2, "lo")]
            [InlineData("Hello", 0, 3, "Hel")]
            [InlineData("🍌 🍄 🍆", 0, 5, "🍌 🍄 🍆")]
            [InlineData("🍌 🍄 🍆", 0, 1, "🍌")]
            [InlineData("🍌 🍄 🍆", 1, 1, " ")]
            [InlineData("🍌 🍄 🍆", 2, 1, "🍄")]
            [InlineData("🍌 🍄 🍆", 3, 1, " ")]
            [InlineData("🍌 🍄 🍆", 4, 1, "🍆")]
            [InlineData("🍌 🍄 🍆", 2, 3, "🍄 🍆")]
            public void ReturnsExpectedSubstring(
                string original, int start, int length, string substring)
            {
                original.UnicodeSafeSubstring(start, length).Should().Be(substring);
            }
        }
    }
}
