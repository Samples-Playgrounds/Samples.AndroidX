using System;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Shared.Extensions;
using static Toggl.Core.Helper.Constants;

namespace Toggl.Core
{
    public struct DurationFieldInfo : IEquatable<DurationFieldInfo>
    {
        public static DurationFieldInfo Empty = new DurationFieldInfo(ImmutableStack<int>.Empty);

        private const int maximumNumberOfDigits = 5;

        private static readonly TimeSpan maximumDuration = TimeSpan.FromHours(MaxTimeEntryDurationInHours);

        private readonly ImmutableStack<int> digits;

        public int Minutes => combineDigitsIntoANumber(0, 2);

        public int Hours => combineDigitsIntoANumber(2, 3);

        public bool IsEmpty => digits.IsEmpty;

        private DurationFieldInfo(ImmutableStack<int> digits)
        {
            this.digits = digits;
        }

        public DurationFieldInfo Push(int digit)
        {
            if (digit < 0 || digit > 9)
                throw new ArgumentException($"Digits must be between 0 and 9, value {digit} was rejected.");

            if (digits.Count() == maximumNumberOfDigits) return this;

            if (digits.IsEmpty && digit == 0) return this;

            return new DurationFieldInfo(digits.Push(digit));
        }

        public DurationFieldInfo Pop()
        {
            if (digits.IsEmpty) return this;

            return new DurationFieldInfo(digits.Pop());
        }

        public static DurationFieldInfo FromTimeSpan(TimeSpan duration)
        {
            var stack = ImmutableStack<int>.Empty;
            var totalMinutes = (long)duration.Clamp(TimeSpan.Zero, maximumDuration).TotalMinutes;
            var hoursPart = totalMinutes / 60;
            var minutesPart = totalMinutes % 60;
            var digitsString = (hoursPart * 100 + minutesPart).ToString();
            digitsString.ToCharArray()
                .Select(digit => digit - '0')
                .ForEach(d => stack = stack.Push(d));
            return new DurationFieldInfo(stack);
        }

        public override string ToString()
            => $"{Hours:00}:{Minutes:00}";

        public TimeSpan ToTimeSpan()
            => TimeSpan.FromHours(Hours).Add(TimeSpan.FromMinutes(Minutes)).Clamp(TimeSpan.Zero, maximumDuration);

        public override int GetHashCode() => digits.GetHashCode();

        public bool Equals(DurationFieldInfo other) => other.Hours == Hours && other.Minutes == Minutes;

        private int combineDigitsIntoANumber(int start, int count)
        {
            var digitsArray = digits.ToArray();
            var number = 0;
            var power = 1;
            for (int i = start; i < Math.Min(start + count, digitsArray.Length); i++)
            {
                number += digitsArray[i] * power;
                power *= 10;
            }

            return number;
        }
    }
}
