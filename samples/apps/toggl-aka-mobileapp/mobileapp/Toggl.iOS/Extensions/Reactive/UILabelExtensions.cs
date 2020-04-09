using Foundation;
using System;
using Toggl.Core.UI.Reactive;
using UIKit;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class UILabelExtensions
    {
        public static Action<string> Text(this IReactive<UILabel> reactive)
            => text => reactive.Base.Text = text;

        public static Action<NSAttributedString> AttributedText(this IReactive<UILabel> reactive)
            => text => reactive.Base.AttributedText = text;

        public static Action<UIColor> TextColor(this IReactive<UILabel> reactive)
            => color => reactive.Base.TextColor = color;
    }
}
