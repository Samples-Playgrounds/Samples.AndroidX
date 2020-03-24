using CoreGraphics;
using Foundation;
using ObjCRuntime;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;
using static Toggl.Shared.Extensions.CommonFunctions;

namespace Toggl.iOS
{
    public partial class RatingView : UIView
    {
        private readonly UIStringAttributes descriptionStringAttributes = new UIStringAttributes
        {
            ParagraphStyle = new NSMutableParagraphStyle
            {
                MaximumLineHeight = 22,
                MinimumLineHeight = 22,
                Alignment = UITextAlignment.Center,
            }
        };

        private RatingViewModel viewModel;
        public RatingViewModel ViewModel
        {
            get => viewModel;
            set
            {
                viewModel = value;
                updateBindings();
            }
        }

        public CompositeDisposable DisposeBag { get; } = new CompositeDisposable();

        public RatingView(IntPtr handle) : base(handle)
        {
        }

        public static RatingView Create()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(RatingView), null, null);
            return Runtime.GetNSObject<RatingView>(arr.ValueAt(0));
        }

        private void updateBindings()
        {
            ViewModel.Impression
                .Select(impression => impression.HasValue)
                .Subscribe(CtaView.Rx().IsVisibleWithFade())
                .DisposedBy(DisposeBag);

            ViewModel.Impression
                .Select(callToActionTitle)
                .Subscribe(CtaTitle.Rx().Text())
                .DisposedBy(DisposeBag);

            ViewModel.Impression
                .Select(callToActionDescription)
                .Select(attributedDescription)
                .Subscribe(CtaDescription.Rx().AttributedText())
                .DisposedBy(DisposeBag);

            ViewModel.Impression
                .Select(callToActionButtonTitle)
                .Subscribe(CtaButton.Rx().Title())
                .DisposedBy(DisposeBag);

            ViewModel.Impression
                .Select(impression => impression.HasValue)
                .Select(Invert)
                .Subscribe(QuestionView.Rx().IsVisibleWithFade())
                .DisposedBy(DisposeBag);

            ViewModel.Impression
                .Select(impression => impression.HasValue)
                .Subscribe(CtaViewBottomConstraint.Rx().Active())
                .DisposedBy(DisposeBag);

            ViewModel.Impression
                .Select(impression => impression.HasValue)
                .Select(Invert)
                .Subscribe(QuestionViewBottomConstraint.Rx().Active())
                .DisposedBy(DisposeBag);

            YesView.Rx().Tap()
                .Subscribe(() => ViewModel.RegisterImpression(true))
                .DisposedBy(DisposeBag);

            NotReallyView.Rx().Tap()
                .Subscribe(() => ViewModel.RegisterImpression(false))
                .DisposedBy(DisposeBag);

            CtaButton.Rx()
                .BindAction(ViewModel.PerformMainAction)
                .DisposedBy(DisposeBag);

            DismissButton.Rx().Tap()
                .Subscribe(ViewModel.Dismiss)
                .DisposedBy(DisposeBag);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            DisposeBag.Dispose();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            TitleLabel.Text = Resources.RatingTitle;
            YesLabel.Text = Resources.RatingYes;
            NotReallyLabel.Text = Resources.RatingNotReally;

            CtaButton.SetTitle(Resources.RatingCallToActionTitle, UIControlState.Normal);
            DismissButton.SetTitle(Resources.NoThanks, UIControlState.Normal);

            SetupAsCard(QuestionView);
            SetupAsCard(CtaView);

            CtaButton.Layer.CornerRadius = 8;
            CtaView.Layer.MasksToBounds = false;
        }

        private NSAttributedString attributedDescription(string text)
            => new NSAttributedString(text, descriptionStringAttributes);

        private void SetupAsCard(UIView view)
        {
            var shadowPath = UIBezierPath.FromRect(view.Bounds);
            view.Layer.ShadowPath?.Dispose();
            view.Layer.ShadowPath = shadowPath.CGPath;

            view.Layer.CornerRadius = 8;
            view.Layer.ShadowRadius = 4;
            view.Layer.ShadowOpacity = 0.1f;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowOffset = new CGSize(0, 2);
            view.Layer.ShadowColor = UIColor.Black.CGColor;
        }

        private string callToActionTitle(bool? impressionIsPositive)
        {
            if (impressionIsPositive == null)
                return string.Empty;

            return impressionIsPositive.Value
                   ? Resources.RatingViewPositiveCallToActionTitle
                   : Resources.RatingViewNegativeCallToActionTitle;
        }

        private string callToActionDescription(bool? impressionIsPositive)
        {
            if (impressionIsPositive == null)
                return string.Empty;

            return impressionIsPositive.Value
                   ? Resources.RatingViewPositiveCallToActionDescriptionIos
                   : Resources.RatingViewNegativeCallToActionDescription;
        }

        private string callToActionButtonTitle(bool? impressionIsPositive)
        {
            if (impressionIsPositive == null)
                return string.Empty;

            return impressionIsPositive.Value
                   ? Resources.RatingViewPositiveCallToActionButtonTitle
                   : Resources.RatingViewNegativeCallToActionButtonTitle;
        }
    }
}
