using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Tests.TestExtensions
{
    public static class RxActionExtensions
    {
        public static IObservable<TOutput> ExecuteSequentally<TInput, TOutput>(this RxAction<TInput, TOutput> action,
            IEnumerable<TInput> inputs)
            => action.executeSequentally(inputs);

        public static IObservable<TOutput> ExecuteSequentally<TInput, TOutput>(this RxAction<TInput, TOutput> action,
            params TInput[] inputs)
            => action.executeSequentally(inputs);

        public static IObservable<Unit> ExecuteSequentally(this RxAction<Unit, Unit> action, int times)
            => action.executeSequentally(Enumerable.Range(0, times).Select(_ => default(Unit)));

        private static IObservable<TOutput> executeSequentally<TInput, TOutput>(this RxAction<TInput, TOutput> action,
            IEnumerable<TInput> inputs)
        {
            var observables = inputs
                .Select(input => Observable.Defer(() => action.ExecuteWithCompletion(input)));

            return Observable.Concat(
                observables
            );
        }

        public static IObservable<Unit> PrependAction(this IObservable<Unit> observable, RxAction<Unit, Unit> action)
            => observable.prependAction(action, default(Unit));

        public static IObservable<Unit> PrependAction<TInput>(this IObservable<Unit> observable, RxAction<TInput, Unit> action, TInput input)
            => observable.prependAction(action, input);

        private static IObservable<TOutput> prependAction<TElement, TInput, TOutput>(this IObservable<TElement> observable, RxAction<TInput, TOutput> action, TInput input)
        {
            return observable
                .SelectValue(default(TOutput))
                .IgnoreElements()
                .Concat(
                    Observable.Defer(() => action.ExecuteWithCompletion(input))
            );
        }
    }
}
