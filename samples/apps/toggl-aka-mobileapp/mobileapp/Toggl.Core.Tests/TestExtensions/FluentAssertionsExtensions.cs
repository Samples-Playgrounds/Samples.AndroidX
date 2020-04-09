using FluentAssertions;
using FluentAssertions.Collections;
using NSubstitute;
using NSubstitute.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Toggl.Core.Tests.TestExtensions
{
    public static class FluentAssertionsExtensions
    {
        public static int ToInt(this string x) => int.Parse(x);

        public static AndConstraint<TAssertions> BeSequenceEquivalentTo<TExpectation, TSubject, TAssertions>(
            this CollectionAssertions<TSubject, TAssertions> collectionAssertions,
            IEnumerable<TExpectation> expectation,
            string because = "",
            params object[] becauseArgs)
            where TSubject : IEnumerable
            where TAssertions : CollectionAssertions<TSubject, TAssertions>
        {
            return collectionAssertions.BeEquivalentTo(expectation, options => options.WithStrictOrdering(), because, becauseArgs);
        }

        public static ConfiguredCall ReturnsCompletedTask(this Task value)
            => value.Returns(Task.CompletedTask);

        public static ConfiguredCall ReturnsTaskOf<T>(this Task<T> value, T returnThis, params T[] returnThese)
            => value.Returns(Task.FromResult(returnThis), returnThese.Select(Task.FromResult).ToArray());

        public static ConfiguredCall ReturnsThrowingTask(this Task value, Exception exception)
            => value.Returns(Task.FromException(exception));

        public static ConfiguredCall ReturnsThrowingTaskOf<T>(this Task<T> value, Exception exception)
            => value.Returns(Task.FromException<T>(exception));

        public static ConfiguredCall ReturnsObservableOf<T>(this IObservable<T> value, T returnThis, params T[] returnThese)
            => value.Returns(Observable.Return(returnThis), returnThese.Select(Observable.Return).ToArray());

        public static ConfiguredCall ReturnsObservableOf<T>(this IObservable<T> value, Exception exception)
            => value.Returns(Observable.Throw<T>(exception));
    }
}
