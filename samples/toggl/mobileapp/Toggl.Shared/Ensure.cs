﻿using System;
using Toggl.Shared.Extensions;

namespace Toggl.Shared
{
    public static class Ensure
    {
        public static class Argument
        {
            public static void IsNotNull<T>(T value, string argumentName)
            {
#pragma warning disable RECS0017 // Possible compare of value type with 'null'
                if (value != null) return;
#pragma warning restore RECS0017 // Possible compare of value type with 'null'

                throw new ArgumentNullException(argumentName);
            }

            public static void IsNotNullOrWhiteSpaceString(string value, string argumentName)
            {
                IsNotNull(value, argumentName);

                if (!string.IsNullOrWhiteSpace(value)) return;

                throw new ArgumentException("String cannot be empty.", argumentName);
            }

            public static void IsNotNullOrEmpty(string value, string argumentName)
            {
                if (!string.IsNullOrEmpty(value)) return;

                throw new ArgumentException("String cannot be null or empty.", argumentName);
            }

            public static void IsNotZero(long value, string argumentName)
            {
                if (value != 0) return;

                throw new ArgumentException("Long cannot be zero.", argumentName);
            }

            public static void IsAbsoluteUri(Uri uri, string argumentName)
            {
                IsNotNull(uri, argumentName);

                if (uri.IsAbsoluteUri) return;

                throw new ArgumentException("Uri must be absolute.", argumentName);
            }

            public static void IsValidEmail(Email email, string argumentName)
            {
                if (email.IsValid) return;

                throw new ArgumentException("Email address must be valid.", argumentName);
            }

            public static void IsADefinedEnumValue<T>(T value, string argumentName) where T : struct
            {
                if (Enum.IsDefined(typeof(T), value)) return;

                throw new ArgumentException("Invalid enum value.", argumentName);
            }

            public static void IsInClosedRange<T>(T value, T lowerBound, T upperBound, string argumentName) where T : IComparable
            {
                if (value.CompareTo(lowerBound) >= 0 && value.CompareTo(upperBound) <= 0) return;

                throw new ArgumentException("Value is in out of bounds.", argumentName);
            }

            public static void TypeImplementsOrInheritsFromType(Type derivedType, Type baseType)
            {
                if (derivedType.ImplementsOrDerivesFrom(baseType))
                    return;

                throw new ArgumentException($"Type {derivedType.Name} is not of type {baseType.Name}");
            }
        }
    }
}
