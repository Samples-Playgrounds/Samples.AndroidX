using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Toggl.Shared
{
    public struct Option<T> : IEquatable<Option<T>>
    {
        private readonly T value;
        private readonly bool hasValue;

        private Option(T value)
        {
            hasValue = true;
            this.value = value;
        }

        public static Option<T> None
            => default;

        internal static Option<T> Some(T value)
            => new Option<T>(value);

        public Option<TOut> Select<TOut>(Func<T, TOut> selector)
            => hasValue ? Option.Some(selector(value)) : Option<TOut>.None;

        public Option<TOut> SelectMany<TOut>(Func<T, Option<TOut>> selector)
            => hasValue ? selector(value) : Option<TOut>.None;

        public Option<T> Where(Func<T, bool> predicate)
            => hasValue && predicate(value) ? this : None;

        public void Match(Action<T> onSome)
        {
            if (hasValue)
                onSome(value);
        }

        public void Match(Action<T> onSome, Action onNone)
        {
            if (hasValue)
            {
                onSome(value);
            }
            else
            {
                onNone();
            }
        }

        public TResult Match<TResult>(Func<T, TResult> onSome, Func<TResult> onNone)
            => hasValue ? onSome(value) : onNone();

        public bool Equals(Option<T> other)
            => hasValue == other.hasValue
            && EqualityComparer<T>.Default.Equals(value, other.value);

        public override bool Equals(object obj)
            => obj is Option<T> other && Equals(other);

        public override int GetHashCode()
            => hasValue ? EqualityComparer<T>.Default.GetHashCode(value) : 0;

        public override string ToString()
            => hasValue ? $"Some {value}" : "None";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Option<T>(NothingOption _) => None;
    }

    public static class Option
    {
        public static Option<T> FromNullable<T>(T value) where T : class =>
            value == null ? None : Option<T>.Some(value);

        public static Option<T> FromNullable<T>(T? value) where T : struct =>
            value.HasValue ? Option<T>.Some(value.Value) : None;

        public static Option<T> Some<T>(T value) => Option<T>.Some(value);

        public static NothingOption None => default;
    }

    public struct NothingOption
    {
    }
}
