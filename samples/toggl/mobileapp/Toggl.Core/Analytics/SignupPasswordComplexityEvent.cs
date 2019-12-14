using System;
using System.Collections.Generic;
using Toggl.Shared;

namespace Toggl.Core.Analytics
{
    public sealed class SignupPasswordComplexityEvent : ITrackableEvent
    {
        private const string eventName = "SignupPasswordComplexity";

        private readonly int length;
        private readonly int digitCount;
        private readonly int nonAsciiCount;
        private readonly int lowerCaseCount;
        private readonly int upperCaseCount;
        private readonly int otherAsciiCount;

        public SignupPasswordComplexityEvent(Password password)
        {
            if (!password.IsValid)
                throw new ArgumentException("Can't track the complexity of an invalid password");

            length = password.Length;

            foreach (var character in password.ToString())
            {
                if (character >= 'a' && character <= 'z')
                {
                    lowerCaseCount++;
                }
                else if (character >= 'A' && character <= 'Z')
                {
                    upperCaseCount++;
                }
                else if (character >= '0' && character <= '9')
                {
                    digitCount++;
                }
                else if (character >= ' ' && character <= '~')
                {
                    otherAsciiCount++;
                }
                else
                {
                    nonAsciiCount++;
                }
            }
        }

        public string EventName { get; } = eventName;

        public Dictionary<string, string> ToDictionary()
            => new Dictionary<string, string>
            {
                [nameof(length)] = length.ToString(),
                [nameof(digitCount)] = digitCount.ToString(),
                [nameof(nonAsciiCount)] = nonAsciiCount.ToString(),
                [nameof(upperCaseCount)] = upperCaseCount.ToString(),
                [nameof(lowerCaseCount)] = lowerCaseCount.ToString(),
                [nameof(otherAsciiCount)] = otherAsciiCount.ToString()
            };
    }
}
