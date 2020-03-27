using Android.Text;
using Android.Widget;
using Java.Lang;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using static Android.Views.View;

namespace Toggl.Droid.Extensions.Reactive
{
    public static class TextViewExtensions
    {
        public static IObservable<string> Text(this IReactive<TextView> reactive)
            => Observable
                .FromEventPattern<TextChangedEventArgs>(e => reactive.Base.TextChanged += e, e => reactive.Base.TextChanged -= e)
                .Select(args => ((EditText)args.Sender).Text);

        public static IObservable<ICharSequence> TextFormatted(this IReactive<TextView> reactive)
            => Observable
                .FromEventPattern<TextChangedEventArgs>(e => reactive.Base.TextChanged += e, e => reactive.Base.TextChanged -= e)
                .Select(args => ((EditText)args.Sender).TextFormatted);

        public static IObservable<bool> FocusChanged(this IReactive<TextView> reactive)
            => Observable
                .FromEventPattern<FocusChangeEventArgs>(e => reactive.Base.FocusChange += e, e => reactive.Base.FocusChange -= e)
                .Select(args => ((EditText)args.Sender).HasFocus);

        public static Action<string> Hint(this IReactive<TextView> reactive)
            => text => reactive.Base.Hint = text;

        public static Action<string> TextObserver(this IReactive<TextView> reactive, bool ignoreUnchanged = false)
        {
            return text =>
            {
                if (!ignoreUnchanged)
                {
                    reactive.Base.Text = text;
                    return;
                }

                if (reactive.Base.Text != text)
                    reactive.Base.Text = text;
            };
        }

        public static Action<ISpannable> TextFormattedObserver(this IReactive<TextView> reactive)
            => text => reactive.Base.TextFormatted = text;
    }
}
