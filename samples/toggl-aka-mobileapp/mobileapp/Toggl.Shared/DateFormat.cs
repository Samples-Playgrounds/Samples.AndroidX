using System;

namespace Toggl.Shared
{
    public struct DateFormat : IEquatable<DateFormat>
    {
        /// <summary>
        /// Intended for displaying in the UI
        /// </summary>
        public string Localized { get; }

        /// <summary>
        /// Intended for  using in dateTime.ToString(useHere)
        /// </summary>
        public string Long { get; }

        /// <summary>
        /// Same as DateFormat.Long, but without the year portion
        /// </summary>
        public string Short { get; }

        private DateFormat(
            string longDateFormat,
            string shortDateFormat,
            string localizedDateFormat)
        {
            Ensure.Argument.IsNotNull(longDateFormat, nameof(longDateFormat));
            Ensure.Argument.IsNotNull(shortDateFormat, nameof(shortDateFormat));
            Ensure.Argument.IsNotNull(localizedDateFormat, nameof(localizedDateFormat));

            Long = longDateFormat;
            Short = shortDateFormat;
            Localized = localizedDateFormat;
        }

        public static DateFormat FromLocalizedDateFormat(string localizedDateFormat)
        {
            var longDateFormat = localizedDateFormat
                .Replace('Y', 'y')
                .Replace('D', 'd');

            var shortDateFormat = longDateFormat
                .Replace("y", "")
                .Trim('.', '-', '/');

            return new DateFormat(
                longDateFormat,
                shortDateFormat,
                localizedDateFormat);
        }

        public bool Equals(DateFormat other)
            => Localized == other.Localized;

        public override bool Equals(object obj)
        {
            if (obj is DateFormat dateFormat)
                return Equals(dateFormat);
            return false;
        }

        public override int GetHashCode() => Localized.GetHashCode();

        public static bool operator ==(DateFormat d1, DateFormat d2)
            => d1.Equals(d2);

        public static bool operator !=(DateFormat d1, DateFormat d2)
            => !d1.Equals(d2);

        public static DateFormat[] ValidDateFormats => new[]
        {
            FromLocalizedDateFormat("MM/DD/YYYY"),
            FromLocalizedDateFormat("DD-MM-YYYY"),
            FromLocalizedDateFormat("MM-DD-YYYY"),
            FromLocalizedDateFormat("YYYY-MM-DD"),
            FromLocalizedDateFormat("DD/MM/YYYY"),
            FromLocalizedDateFormat("DD.MM.YYYY")
        };
    }
}
