using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class UISwitchExtensions
    {
        public static IDisposable BindToggleAction<TElement>(this IReactive<UISwitch> reactive, RxAction<bool, TElement> action)
            => reactive.BindAction(action, s => s.On);

        public static IDisposable BindTapAction<TElement>(this IReactive<UISwitch> reactive, RxAction<Unit, TElement> action)
            => reactive.BindAction(action, _ => Unit.Default);

        public static IDisposable BindAction<TInput, TElement>(this IReactive<UISwitch> reactive,
            RxAction<TInput, TElement> action, Func<UISwitch, TInput> inputTransform)
        {
            IObservable<Unit> eventObservable = reactive.Base.Rx().Changed();

            return Observable.Using(
                    () => action.Enabled.Subscribe(e => { reactive.Base.Enabled = e; }),
                    _ => eventObservable
                )
                .Select(_ => inputTransform(reactive.Base))
                .Subscribe(action.Inputs);
        }

        public static IDisposable BindAction(this IReactive<UISwitch> reactive, Action action)
        {
            Ensure.Argument.IsNotNull(action, nameof(action));

            return reactive.Base.Rx().Changed().Subscribe(_ => action());
        }

        public static Action<bool> CheckedObserver(this IReactive<UISwitch> reactive)
        {
            return isChecked =>
            {
                if (reactive.Base.On != isChecked)
                {
                    reactive.Base.SetState(isChecked, false);
                }
            };
        }

        public static Action<bool> On(this IReactive<UISwitch> reactive)
            => isOn => reactive.Base.SetState(isOn, true);
    }
}
