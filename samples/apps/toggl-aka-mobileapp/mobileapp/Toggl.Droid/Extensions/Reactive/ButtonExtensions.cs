using Android.Widget;
using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Extensions.Reactive
{
    public static class ButtonExtensions
    {
        public static IObservable<Unit> Tap(this IReactive<Button> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.Click += e, e => reactive.Base.Click -= e)
                .SelectUnit();

        public static IDisposable BindAction(this IReactive<Button> reactive, ViewAction action,
            ButtonEventType eventType = ButtonEventType.Tap) =>
            reactive.BindAction(action, _ => Unit.Default, eventType);

        public static IDisposable BindAction<TInput>(this IReactive<Button> reactive, InputAction<TInput> action,
            Func<Button, TInput> inputTransform, ButtonEventType eventType = ButtonEventType.Tap) =>
            reactive.BindAction<TInput, Unit>(action, inputTransform, eventType);

        public static IDisposable BindAction<TElement>(this IReactive<Button> reactive,
            RxAction<Unit, TElement> action, ButtonEventType eventType = ButtonEventType.Tap) =>
            reactive.BindAction(action, _ => Unit.Default, eventType);

        public static IDisposable BindAction<TInput, TElement>(this IReactive<Button> reactive,
            RxAction<TInput, TElement> action, Func<Button, TInput> inputTransform, ButtonEventType eventType = ButtonEventType.Tap)
        {
            IObservable<Unit> eventObservable = Observable.Empty<Unit>();
            switch (eventType)
            {
                case ButtonEventType.Tap:
                    eventObservable = reactive.Base.Rx().Tap();
                    break;
                case ButtonEventType.LongPress:
                    eventObservable = reactive.Base.Rx().LongPress();
                    break;
            }

            return Observable.Using(
                    () => action.Enabled.Subscribe(e => { reactive.Base.Enabled = e; }),
                    _ => eventObservable
                )
                .Select(_ => inputTransform(reactive.Base))
                .Subscribe(action.Inputs);
        }
    }
}
