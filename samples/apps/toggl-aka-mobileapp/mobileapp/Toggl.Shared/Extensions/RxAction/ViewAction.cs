using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace Toggl.Shared.Extensions
{
    public sealed class ViewAction : RxAction<Unit, Unit>
    {
        private ViewAction(Func<IObservable<Unit>> workFactory, IScheduler mainScheduler, IObservable<bool> enabledIf)
            : base(_ => workFactory(), mainScheduler, enabledIf)
        {
        }

        public void Execute()
            => Execute(Unit.Default);

        public IObservable<Unit> ExecuteWithCompletion()
            => ExecuteWithCompletion(Unit.Default);

        public static ViewAction FromAction(Action action, IScheduler mainScheduler, IObservable<bool> enabledIf = null)
        {
            IObservable<Unit> workFactory()
                => Observable.Create<Unit>(observer =>
                {
                    action();
                    observer.CompleteWith(Unit.Default);
                    return Disposable.Empty;
                });

            return new ViewAction(workFactory, mainScheduler, enabledIf);
        }

        public static ViewAction FromAsync(Func<Task> asyncAction, IScheduler mainScheduler, IObservable<bool> enabledIf = null)
        {
            IObservable<Unit> workFactory()
                => asyncAction().ToObservable();

            return new ViewAction(workFactory, mainScheduler, enabledIf);
        }

        public static ViewAction FromObservable(Func<IObservable<Unit>> workFactory, IScheduler mainScheduler, IObservable<bool> enabledIf = null)
            => new ViewAction(workFactory, mainScheduler, enabledIf);
    }

    public static class RxActionExtensions
    {
        public static void Execute(this ViewAction action)
        {
            action.Execute(Unit.Default);
        }
    }
}
