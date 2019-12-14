using System;
using System.Collections.Generic;
using Toggl.Core.UI.Reactive;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Extensions.Reactive
{
    public static class UIStackViewExtensions
    {
        public static Action<IEnumerable<UIView>> ArrangedViews(this IReactive<UIStackView> reactive)
            => views =>
            {
                reactive.Base.ArrangedSubviews.ForEach(subview => subview.RemoveFromSuperview());
                views.ForEach(reactive.Base.AddArrangedSubview);
            };

        public static Action<nfloat> Spacing(this IReactive<UIStackView> reactive)
            => spacing => reactive.Base.Spacing = spacing;
    }
}
