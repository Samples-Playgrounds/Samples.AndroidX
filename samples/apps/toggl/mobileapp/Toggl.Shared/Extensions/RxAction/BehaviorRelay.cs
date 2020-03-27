using System;
using System.Reactive.Subjects;

namespace Toggl.Shared.Extensions.Reactive
{
    /// <summary>
    /// A wrapper for BehaviorSubject.
    /// It behaves just like BehaviorSubject, except that it can't terminate or error out.
    /// </summary>
    /// <remarks>
    /// Sanitization function is optionally used to sanitize the input in a meaningful way, 
    /// such as trimming the string or limiting the range of a numeric value.
    /// </remarks>
    /// <typeparam name="T">The type of the elements processed by the subject.</typeparam>
    public class BehaviorRelay<T> : IObservable<T>
    {
        private readonly BehaviorSubject<T> subject;
        private readonly Func<T, T> sanitizationFunction;

        public BehaviorRelay(T value, Func<T, T> sanitizationFunction = null)
        {
            this.sanitizationFunction = sanitizationFunction;
            subject = new BehaviorSubject<T>(value);
        }

        public T Value => subject.Value;

        public void Accept(T value)
        {
            var changedValue = sanitizationFunction == null
                ? value
                : sanitizationFunction(value);

            subject.OnNext(changedValue);
        }

        public IDisposable Subscribe(IObserver<T> observer)
            => subject.Subscribe(observer);
    }
}