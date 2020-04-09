using System.Collections.Immutable;
using System.Reactive.Subjects;
using Toggl.Core.UI.Helper;
using Toggl.Core.Suggestions;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Suggestions
{
    public sealed class SuggestionsView : UIView
    {
        private const float titleSize = 12;
        private const float sideMargin = 16;
        private const float suggestionHeightCompact = 64;
        private const float suggestionHeightRegular = 48;
        private const float distanceAboveTitleLabel = 20;
        private const float distanceBelowTitleLabel = 16;
        private const float distanceBetweenSuggestions = 12;

        private readonly UILabel titleLabel = new UILabel();

        private NSLayoutConstraint heightConstraint;
        private float suggestionHeight;

        public ISubject<Suggestion> SuggestionTapped { get; } = new Subject<Suggestion>();

        public SuggestionsView()
        {
            TranslatesAutoresizingMaskIntoConstraints = false;
            BackgroundColor = UIColor.Clear;
            ClipsToBounds = true;

            heightConstraint = HeightAnchor.ConstraintEqualTo(0);
        }

        public override void MovedToSuperview()
        {
            base.MovedToSuperview();

            TopAnchor.ConstraintEqualTo(Superview.TopAnchor).Active = true;
            WidthAnchor.ConstraintEqualTo(Superview.WidthAnchor).Active = true;
            CenterXAnchor.ConstraintEqualTo(Superview.CenterXAnchor).Active = true;
            //Actual value is set with bindings a few lines below
            heightConstraint.Active = true;

            prepareTitleLabel();

            LayoutIfNeeded();
        }

        public void OnSuggestions(IImmutableList<Suggestion> suggestions)
        {
            foreach (UIView view in Subviews)
            {
                if (view is SuggestionView)
                {
                    view.RemoveFromSuperview();
                }
            }

            suggestionHeight = TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular
                               ? suggestionHeightRegular
                               : suggestionHeightCompact;

            var suggestionCount = suggestions.Count;
            for (int i = 0; i < suggestionCount; i++)
            {
                var suggestionView = SuggestionView.Create();
                suggestionView.Suggestion = suggestions[i];
                AddSubview(suggestionView);
                suggestionView.TranslatesAutoresizingMaskIntoConstraints = false;
                suggestionView.HeightAnchor.ConstraintEqualTo(suggestionHeight).Active = true;
                suggestionView.CenterXAnchor.ConstraintEqualTo(Superview.CenterXAnchor).Active = true;
                suggestionView.WidthAnchor.ConstraintEqualTo(Superview.WidthAnchor, 1, -2 * sideMargin).Active = true;
                suggestionView.TopAnchor.ConstraintEqualTo(titleLabel.BottomAnchor, distanceFromTitleLabel(i)).Active = true;

                suggestionView.AddGestureRecognizer(new UITapGestureRecognizer(() =>
                {
                    SuggestionTapped.OnNext(suggestionView.Suggestion);
                }));
            }
            heightConstraint.Constant = heightForSuggestionCount(suggestionCount);
            heightConstraint.Active = true;
            SetNeedsLayout();
        }

        private void prepareTitleLabel()
        {
            AddSubview(titleLabel);
            titleLabel.TranslatesAutoresizingMaskIntoConstraints = false;
            titleLabel.Text = Resources.SuggestionsHeader;
            titleLabel.IsAccessibilityElement = false;
            titleLabel.Font = UIFont.SystemFontOfSize(titleSize, UIFontWeight.Medium);
            titleLabel.TextColor = Colors.Main.SuggestionsTitle.ToNativeColor();
            titleLabel.TopAnchor.ConstraintEqualTo(Superview.TopAnchor, distanceAboveTitleLabel).Active = true;
            titleLabel.LeadingAnchor.ConstraintEqualTo(Superview.LeadingAnchor, sideMargin).Active = true;
        }

        private float distanceFromTitleLabel(int index)
            => distanceBelowTitleLabel
               + index * distanceBetweenSuggestions
               + index * suggestionHeight;

        private float heightForSuggestionCount(int count)
        {
            if (count == 0)
            {
                return 0;
            }
            return count * (suggestionHeight + distanceBetweenSuggestions) + distanceAboveTitleLabel
                                                                           + distanceBelowTitleLabel
                                                                           + (float)titleLabel.Frame.Height;
        }
    }
}
