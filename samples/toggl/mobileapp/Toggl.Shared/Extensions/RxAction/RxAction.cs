using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Toggl.Shared.Extensions
{
    public sealed class RxActionNotEnabledException : Exception
    {
    }

    public class
        RxAction<TInput, TElement> : IDisposable
    {
        public IObservable<Exception> Errors { get; }
        public IObservable<TElement> Elements { get; }
        public IObservable<bool> Executing { get; }
        public IObservable<bool> Enabled { get; }
        public ISubject<TInput> Inputs { get; }

        private CompositeDisposable disposeBag;

        private readonly IObservable<IObservable<TElement>> executionObservables;

        public RxAction(Func<TInput, IObservable<TElement>> workFactory, IScheduler mainScheduler, IObservable<bool> enabledIf = null)
        {
            if (enabledIf == null)
            {
                enabledIf = Observable.Return(true);
            }

            disposeBag = new CompositeDisposable();
            Inputs = new Subject<TInput>();

            var enabledSubject = new BehaviorSubject<bool>(false);
            Enabled = enabledSubject.AsObservable();

            var errorsSubject = new Subject<Exception>();
            Errors = errorsSubject.AsObservable();

            executionObservables = Inputs
                .WithLatestFrom(Enabled, (i, e) => (input: i, enabled: e))
                .SelectMany(tuple =>
                {
                    var enabled = tuple.enabled;
                    var input = tuple.input;

                    if (enabled)
                    {
                        var ob = workFactory(input)
                            .ObserveOn(mainScheduler)
                            .Do(CommonFunctions.DoNothing, error => errorsSubject.OnNext(error))
                            .Replay(1).RefCount();

                        return Observable.Return(ob);
                    }
                    else
                    {
                        errorsSubject.OnNext(new RxActionNotEnabledException());
                        return Observable.Empty<IObservable<TElement>>();
                    }
                })
                .Share();

            Elements = executionObservables
                .SelectMany(observable => observable.OnErrorResumeNext(Observable.Empty<TElement>()));

            Executing = executionObservables
                .SelectMany(exec =>
                {
                    var execution = exec
                        .SelectMany(_ => Observable.Empty<bool>())
                        .OnErrorResumeNext(Observable.Empty<bool>());

                    return Observable.Concat(Observable.Return(true), execution, Observable.Return(false));
                })
                .StartWith(false)
                .Replay(1).RefCount();

            Observable.CombineLatest(Executing, enabledIf, (executing, enabled) => !executing && enabled)
                .Subscribe(enabledSubject)
                .DisposedBy(disposeBag);
        }

        public void Execute(TInput value)
        {
            Inputs.OnNext(value);
        }


        public IObservable<TElement> ExecuteWithCompletion(TInput value)
        {
            var subject = new ReplaySubject<TElement>();

            var error = Errors
                .Select(e => Observable.Throw<TElement>(e));

            executionObservables
                .Amb(error)
                .Take(1)
                .Flatten()
                .Subscribe(subject)
                .DisposedBy(disposeBag);

            Inputs.OnNext(value);
            return subject.AsObservable();
        }

        public void Dispose()
        {
            disposeBag?.Dispose();
        }

        public static RxAction<TInput, TElement> FromFunction(Func<TInput, TElement> func, IScheduler mainScheduler)
        {
            IObservable<TElement> workFactory(TInput input)
                => Observable.Create<TElement>(observer =>
                {
                    var result = func(input);
                    observer.CompleteWith(result);
                    return Disposable.Empty;
                });

            return new RxAction<TInput, TElement>(workFactory, mainScheduler);
        }

        public static RxAction<TInput, TElement> FromAsync(Func<TInput, Task<TElement>> asyncFunction, IScheduler mainScheduler, IObservable<bool> enabledIf = null)
        {
            IObservable<TElement> workFactory(TInput input)
                => asyncFunction(input).ToObservable();

            return new RxAction<TInput, TElement>(workFactory, mainScheduler, enabledIf);
        }

        public static RxAction<TInput, TElement> FromObservable(Func<TInput, IObservable<TElement>> workFactory, IScheduler mainScheduler, IObservable<bool> enabledIf = null)
            => new RxAction<TInput, TElement>(workFactory, mainScheduler, enabledIf);
    }
}
