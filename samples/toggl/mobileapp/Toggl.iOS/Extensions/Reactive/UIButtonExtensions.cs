using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Reactive;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Extensions.Reactive
{
    public enum ButtonEventType
    {
        Tap,
        LongPress
    }

    public static class UIButtonExtensions
    {
        public static IObservable<Unit> Tap(this IReactive<UIButton> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.TouchUpInside += e, e => reactive.Base.TouchUpInside -= e)
                .SelectUnit();

        public static Action<string> Title(this IReactive<UIButton> reactive)
            => title => reactive.Base.SetTitle(title, UIControlState.Normal);

        public static Action<string> TitleAdaptive(this IReactive<UIButton> reactive)
            => title =>
            {
                reactive.Base.SetTitle(title, UIControlState.Normal);
                reactive.Base.SizeToFit();
            };

        public static Action<UIColor> TitleColor(this IReactive<UIButton> reactive)
            => color => reactive.Base.SetTitleColor(color, UIControlState.Normal);

        public static Action<string> AnimatedTitle(this IReactive<UIButton> reactive)
            => title =>
            {
                UIView.Transition(
                    reactive.Base,
                    Animation.Timings.EnterTiming,
                    UIViewAnimationOptions.TransitionCrossDissolve,
                    () => reactive.Base.SetTitle(title, UIControlState.Normal),
                    null
                );
            };

        public static IDisposable BindAction(this IReactive<UIButton> reactive, ViewAction action,
            ButtonEventType eventType = ButtonEventType.Tap) =>
            reactive.BindAction(action, _ => Unit.Default, eventType);

        public static IDisposable BindAction<TInput>(this IReactive<UIButton> reactive, InputAction<TInput> action,
            Func<UIButton, TInput> inputTransform, ButtonEventType eventType = ButtonEventType.Tap) =>
            reactive.BindAction<TInput, Unit>(action, inputTransform, eventType);

        public static IDisposable BindAction<TElement>(this IReactive<UIButton> reactive,
            RxAction<Unit, TElement> action, ButtonEventType eventType = ButtonEventType.Tap) =>
            reactive.BindAction(action, _ => Unit.Default, eventType);

        public static IDisposable BindAction<TInput, TElement>(this IReactive<UIButton> reactive,
            RxAction<TInput, TElement> action, Func<UIButton, TInput> inputTransform, ButtonEventType eventType = ButtonEventType.Tap, bool useFeedback = false)
        {
            IObservable<Unit> eventObservable = Observable.Empty<Unit>();
            switch (eventType)
            {
                case ButtonEventType.Tap:
                    eventObservable = reactive.Base.Rx().Tap();
                    break;
                case ButtonEventType.LongPress:
                    eventObservable = reactive.Base.Rx().LongPress(useFeedback);
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
