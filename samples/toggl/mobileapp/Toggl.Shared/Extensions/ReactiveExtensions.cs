using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Toggl.Shared.Extensions
{
    public static class ReactiveExtensions
    {
        private static Action<string> outputDebugInfo = Console.WriteLine;

        private class Observer<T> : IObserver<T>
        {
            private readonly Action<Exception> onError;
            private readonly Action onCompleted;

            public Observer(Action<Exception> onError, Action onCompleted)
            {
                this.onError = onError;
                this.onCompleted = onCompleted;
            }

            public void OnCompleted()
                => onCompleted();

            public void OnError(Exception error)
                => onError(error);

            public void OnNext(T value) { }
        }

        public static IDisposable Subscribe<T1, T2>(this IObservable<(T1, T2)> observable, Action<T1, T2> action)
            => observable.Subscribe(tuple => action(tuple.Item1, tuple.Item2));

        public static IDisposable SubscribeToErrorsAndCompletion<T>(this IObservable<T> observable, Action<Exception> onError, Action onCompleted)
        {
            var observer = new Observer<T>(onError, onCompleted);
            return observable.Subscribe(observer);
        }

        public static IDisposable SubscribeToErrors<T>(this IObservable<T> observable, Action<Exception> onError)
        {
            var observer = new Observer<T>(onError, () => { });
            return observable.Subscribe(observer);
        }

        public static IObservable<T> ConnectedReplay<T>(this IObservable<T> observable)
        {
            var replayed = observable.Replay();
            replayed.Connect();
            return replayed;
        }

        public static IObservable<T> DelayIf<T>(this IObservable<T> observable, Predicate<T> predicate, TimeSpan delay)
            => observable.SelectMany(value => predicate(value)
                ? Observable.Return(value).Delay(delay)
                : Observable.Return(value));

        public static IObservable<T> Share<T>(this IObservable<T> observable)
            => observable.Publish().RefCount();

        public static IObservable<T> ShareReplay<T>(this IObservable<T> observable, int bufferSize = 1)
            => observable.Replay(bufferSize).RefCount();

        public static IObservable<T> AsDriver<T>(this IObservable<T> observable, T onErrorJustReturn, ISchedulerProvider schedulerProvider)
        {
            if (schedulerProvider.MainScheduler == null)
                throw new InvalidOperationException("You need to set the MainThreadScheduler property before using the AsDriver extension");

            return observable
                .Replay(1).RefCount()
                .Catch(Observable.Return(onErrorJustReturn))
                .ObserveOn(schedulerProvider.MainScheduler);
        }

        public static IObservable<TValue> NotNullable<TValue>(this IObservable<TValue?> observable)
            where TValue : struct
            => observable.Where(x => x != null).Select(x => x.Value);

        public static IObservable<U> SelectValue<T, U>(this IObservable<T> observable, U u)
            => observable.Select(_ => u);

        public static IObservable<Unit> SelectUnit<T>(this IObservable<T> observable)
            => observable.SelectValue(Unit.Default);

        public static IObservable<T> Debug<T>(this IObservable<T> observable, string tag = "")
        {
            return Observable.Defer(() => Observable.Create<T>(observer =>
            {
                outputDebugInfo($"Subscribed {tag}");

                var disposable = observable.Do(
                        x => outputDebugInfo($"OnNext {tag}: {x}"),
                        ex => outputDebugInfo($"OnError {tag}: {ex}"),
                        () => outputDebugInfo($"OnCompleted {tag}")
                    )
                    .Subscribe(observer);

                return Disposable.Create(() =>
                {
                    disposable.Dispose();
                    outputDebugInfo($"Disposed {tag}");
                });
            }));
        }

        public static IObservable<T> DoIf<T>(this IObservable<T> observable, Predicate<T> predicate, Action<T> action)
            => observable.Do(value =>
            {
                if (predicate(value))
                    action(value);
            });

        public static IObservable<T> ThrowIf<T>(
            this IObservable<T> observable,
            Predicate<T> predicate,
            Exception exception)
            => observable.DoIf(predicate, _ => throw exception);

        public static IObservable<bool> Invert(this IObservable<bool> observable) => observable.Select(b => !b);

        public static IObservable<T> WhereAsync<T>(this IObservable<T> observable, Func<T, IObservable<bool>> asyncPredicate)
            => observable.SelectMany(item =>
                asyncPredicate(item)
                    .SingleAsync()
                    .SelectMany(include => include ? Observable.Return(item) : Observable.Empty<T>()));

        public static void DisposedBy(this IDisposable disposable, CompositeDisposable disposeBag)
        {
            disposeBag.Add(disposable);
        }

        public static IObservable<T> ConditionalRetryWithBackoffStrategy<T>(
            this IObservable<T> source,
            int maxRetries,
            Func<int, TimeSpan> backOffStrategy,
            Func<Exception, bool> shouldRetryOn,
            IScheduler scheduler)
        {
            return source.RetryWhen(errorSignal =>
            {
                return errorSignal.SelectMany((error, retryCount) =>
                {
                    var currentTry = retryCount + 1;
                    if (!shouldRetryOn(error) || currentTry > maxRetries)
                    {
                        throw error;
                    }

                    return Observable.Return(Unit.Default).Delay(backOffStrategy(currentTry), scheduler);
                });
            });
        }

        public static IObservable<T> Do<T>(this IObservable<T> observable, Action action)
            => observable.Do(_ => action());

        public static void CompleteWith<T>(this IObserver<T> observer, T item)
        {
            observer.OnNext(item);
            observer.OnCompleted();
        }

        public static void CompleteWithUnit(this IObserver<Unit> observer)
            => observer.CompleteWith(Unit.Default);

        public static IObservable<Unit> ToUnitObservable<T>(this Task<T> task)
            => Observable
            .FromAsync(async () => await task)
            .SelectUnit();

        public static IObservable<string> SelectToString<T>(this IObservable<T> observable)
            => observable.Select(item => item.ToString());

        public static IObservable<TOther> SelectLatestFrom<TFirst, TOther>(this IObservable<TFirst> observable, IObservable<TOther> otherObservable)
            => observable.WithLatestFrom(otherObservable, (first, other) => other);

        public static IObservable<T> Flatten<T>(this IObservable<IObservable<T>> observable)
            => observable.SelectMany(CommonFunctions.Identity);

        public static IObservable<T> Flatten<T>(this IObservable<IEnumerable<T>> observable)
            => observable.SelectMany(CommonFunctions.Identity);

        public static IObservable<T> OnErrorResumeEmpty<T>(this IObservable<T> observable)
            => observable.OnErrorResumeNext(Observable.Empty<T>());
    }
}
