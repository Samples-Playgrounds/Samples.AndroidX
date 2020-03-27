using FluentAssertions;
using FsCheck.Xunit;
using System;
using System.Collections.Generic;
using System.Text;
using Toggl.Core.Analytics;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.AnalyticsService
{
    public sealed class SignupPasswordComplexityEventTests
    {
        public sealed class TheConstructor
        {
            [Theory, LogIfTooSlow]
            [InlineData("1")]
            [InlineData("12")]
            [InlineData("123")]
            [InlineData("1234")]
            [InlineData("12345")]
            public void ThrowsIfYouPassAnInvalidPassword(string passwordString)
            {
                var password = Password.From(passwordString);

                Action tryingToCreateAnEventFromAnInvalidPassword =
                    () => new SignupPasswordComplexityEvent(password);

                tryingToCreateAnEventFromAnInvalidPassword.Should().Throw<ArgumentException>();
            }

        }

        public sealed class TheToDictionaryMethod
        {
            [Property]
            public void AlwaysHasSixParameters(string passwordString)
            {
                var password = Password.From(passwordString);
                if (!password.IsValid)
                    return;
                var complexityEvent = new SignupPasswordComplexityEvent(password);

                var dictionary = complexityEvent.ToDictionary();

                dictionary.Count.Should().Be(6);
            }

            [Property]
            public void HasALengthEntryThatEqualsTheLengthOfThePassword(string passwordString)
            {
                var password = Password.From(passwordString);
                if (!password.IsValid)
                    return;

                var length = password.Length;
                var complexityEvent = new SignupPasswordComplexityEvent(password);
                var dictionary = complexityEvent.ToDictionary();

                dictionary[nameof(length)].ToInt().Should().Be(length);
            }

            [Theory, LogIfTooSlow]
            [MemberData(nameof(PasswordsAndCounts))]
            public void CountsTheNumberOfEachCharTypeCorrectly(
                string passwordString,
                int digitCount,
                int nonAsciiCount,
                int lowerCaseCount,
                int upperCaseCount,
                int otherAsciiCount)
            {
                var password = Password.From(passwordString);
                var complexityEvent = new SignupPasswordComplexityEvent(password);
                var dictionary = complexityEvent.ToDictionary();

                dictionary[nameof(digitCount)].ToInt().Should().Be(digitCount);
                dictionary[nameof(nonAsciiCount)].ToInt().Should().Be(nonAsciiCount);
                dictionary[nameof(lowerCaseCount)].ToInt().Should().Be(lowerCaseCount);
                dictionary[nameof(upperCaseCount)].ToInt().Should().Be(upperCaseCount);
                dictionary[nameof(otherAsciiCount)].ToInt().Should().Be(otherAsciiCount);
            }


            [Property]
            public void TheCountOfAllEntriesEqualsTheLength(string passwordString)
            {
                var password = Password.From(passwordString);
                if (!password.IsValid)
                    return;
                var complexityEvent = new SignupPasswordComplexityEvent(password);
                var dictionary = complexityEvent.ToDictionary();

                var lengthEntry = dictionary["length"].ToInt();
                var sumOfIndividualCounts =
                    dictionary["digitCount"].ToInt() +
                    dictionary["nonAsciiCount"].ToInt() +
                    dictionary["lowerCaseCount"].ToInt() +
                    dictionary["upperCaseCount"].ToInt() +
                    dictionary["otherAsciiCount"].ToInt();

                lengthEntry.Should().Be(sumOfIndividualCounts);
            }

            public static IEnumerable<object[]> PasswordsAndCounts
                => new[]
                {
                    new object[] { "123456", 6, 0, 0, 0, 0 },
                    new object[] { "ٱلْعَرَبِيَّة1E‎", 1, 14, 0, 1, 0 },
                    new object[] { "🔥🔥🔥asd123", 3, 6, 3, 0, 0 },
                    new object[] { "ט31ײַטשspooky", 2, 5, 6, 0, 0 },
                    new object[] { "$J?=f6&([)+7LQAx", 2, 0, 2, 4, 8 },
                    new object[] { "JQR2$PK?=PGzqGyAMxRLB=QC_LZNYLVgZ6_8Ej", 3, 0, 6, 23, 6 },
                    new object[] { "_DK*xF_ZjnZXLAqQPUrkLVn4AJZJM5tG^YxvuE", 2, 0, 11, 21, 4 },
                    new object[] { "&=-DwdzDAh848v&uKpm&XM%MXA!ZF-5fmT9jP5", 6, 0, 11, 13, 8 },
                };
        }
    }
}
