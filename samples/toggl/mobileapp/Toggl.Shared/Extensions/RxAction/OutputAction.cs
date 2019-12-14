using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Toggl.Shared.Extensions
{
    public sealed class OutputAction<TElement> : RxAction<Unit, TElement>
    {
        private OutputAction(Func<IObservable<TElement>> workFactory, IScheduler mainScheduler, IObservable<bool> enabledIf = null)
            : base(_ => workFactory(), mainScheduler, enabledIf)
        {
        }

        public static OutputAction<TElement> FromFunction(Func<TElement> func, IScheduler mainScheduler)
        {
            IObservable<TElement> workFactory()
                => Observable.Create<TElement>(observer =>
                {
                    var result = func();
                    observer.CompleteWith(result);
                    return Disposable.Empty;
                });

            return new OutputAction<TElement>(workFactory, mainScheduler);
        }

        public static OutputAction<TElement> FromAsync(Func<Task<TElement>> asyncFunction, IScheduler mainScheduler, IObservable<bool> enabledIf = null)
        {
            IObservable<TElement> workFactory()
                => asyncFunction().ToObservable();

            return new OutputAction<TElement>(workFactory, mainScheduler, enabledIf);
        }

        public static OutputAction<TElement> FromObservable(Func<IObservable<TElement>> workFactory, IScheduler mainScheduler, IObservable<bool> enabledIf = null)
            => new OutputAction<TElement>(workFactory, mainScheduler, enabledIf);
    }

    public static class OutputActionExtensions
    {
        public static void Execute<TElement>(this OutputAction<TElement> action)
        {
            action.Execute(Unit.Default);
        }
    }
}
