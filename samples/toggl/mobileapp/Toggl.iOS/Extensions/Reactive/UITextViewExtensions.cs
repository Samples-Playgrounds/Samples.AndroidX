using Foundation;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using UIKit;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class UITextViewExtensions
    {
        private const string selectedTextRangeChangedKey = "selectedTextRange";

        public static IObservable<string> Text(this IReactive<UITextView> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.Changed += e, e => reactive.Base.Changed -= e)
                .Select(e => ((UITextView)e.Sender).Text);

        public static IObservable<NSAttributedString> AttributedText(this IReactive<UITextView> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.Changed += e, e => reactive.Base.Changed -= e)
                .Select(e => ((UITextView)e.Sender).AttributedText);

        public static IObservable<int> CursorPosition(this IReactive<UITextView> reactive)
            => Observable.Create<int>(observer =>
            {
                var selectedTextRangeDisposable = reactive.Base.AddObserver(
                    selectedTextRangeChangedKey,
                    NSKeyValueObservingOptions.OldNew,
                    _ => observer.OnNext((int)reactive.Base.SelectedRange.Location)
                );

                return selectedTextRangeDisposable;
            })
                .StartWith((int)reactive.Base.SelectedRange.Location);

        public static Action<NSAttributedString> AttributedTextObserver(this IReactive<UITextView> reactive)
            => text => reactive.Base.AttributedText = text;

        public static Action<string> TextObserver(this IReactive<UITextView> reactive)
            => text => reactive.Base.Text = text;
    }
}
