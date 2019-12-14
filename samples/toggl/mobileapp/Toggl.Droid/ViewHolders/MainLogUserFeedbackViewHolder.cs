using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.RecyclerView.Widget;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;
using static Toggl.Shared.Extensions.CommonFunctions;

namespace Toggl.Droid.ViewHolders
{
    public class MainLogUserFeedbackViewHolder : RecyclerView.ViewHolder
    {
        private RatingViewModel ratingViewModel;

        private TextView userFeedbackTitle;
        private ImageView thumbsUpButton;
        private ImageView thumbsDownButton;
        private TextView yesText;
        private TextView noText;
        private TextView impressionTitle;
        private ImageView impressionThumbsImage;
        private TextView impressionDescription;
        private Button rateButton;
        private TextView laterButton;

        private Group questionGroup;
        private Group impressionGroup;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public MainLogUserFeedbackViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MainLogUserFeedbackViewHolder(View itemView, RatingViewModel ratingViewModel) : base(itemView)
        {
            this.ratingViewModel = ratingViewModel;
            initializeViews(itemView);
            bindViews(itemView);
        }

        private void bindViews(View itemView)
        {
            ratingViewModel.Impression
                .Select(callToActionTitle)
                .Subscribe(impressionTitle.Rx().TextObserver())
                .DisposedBy(disposeBag);

            ratingViewModel.Impression
                .Select(callToActionDescription)
                .Subscribe(impressionDescription.Rx().TextObserver())
                .DisposedBy(disposeBag);

            ratingViewModel.Impression
                .Select(callToActionButtonTitle)
                .Subscribe(rateButton.Rx().TextObserver())
                .DisposedBy(disposeBag);

            ratingViewModel.Impression
                .Select(impression => impression.HasValue)
                .Subscribe(impressionGroup.Rx().IsVisible())
                .DisposedBy(disposeBag);

            ratingViewModel.Impression
                .Select(impression => impression.HasValue)
                .Select(Invert)
                .Subscribe(questionGroup.Rx().IsVisible())
                .DisposedBy(disposeBag);

            ratingViewModel.Impression
               .Select(impression => impression ?? false)
               .Select(drawableFromImpression)
               .Subscribe(impressionThumbsImage.Rx().Image(itemView.Context))
               .DisposedBy(disposeBag);

            thumbsUpButton.Rx().Tap()
                .Subscribe(() => ratingViewModel.RegisterImpression(true))
                .DisposedBy(disposeBag);

            yesText.Rx().Tap()
                .Subscribe(() => ratingViewModel.RegisterImpression(true))
                .DisposedBy(disposeBag);

            thumbsDownButton.Rx().Tap()
                .Subscribe(() => ratingViewModel.RegisterImpression(false))
                .DisposedBy(disposeBag);

            noText.Rx().Tap()
                .Subscribe(() => ratingViewModel.RegisterImpression(false))
                .DisposedBy(disposeBag);

            rateButton.Rx().Tap()
                .Subscribe(ratingViewModel.PerformMainAction.Inputs)
                .DisposedBy(disposeBag);

            laterButton.Rx().Tap()
                .Subscribe(ratingViewModel.Dismiss)
                .DisposedBy(disposeBag);
        }

        private void initializeViews(View view)
        {
            userFeedbackTitle = view.FindViewById<TextView>(Resource.Id.UserFeedbackTitle);
            thumbsUpButton = view.FindViewById<ImageView>(Resource.Id.UserFeedbackThumbsUp);
            thumbsDownButton = view.FindViewById<ImageView>(Resource.Id.UserFeedbackThumbsDown);
            yesText = view.FindViewById<TextView>(Resource.Id.UserFeedbackThumbsUpText);
            noText = view.FindViewById<TextView>(Resource.Id.UserFeedbackThumbsDownText);
            impressionTitle = view.FindViewById<TextView>(Resource.Id.UserFeedbackImpressionTitle);
            impressionThumbsImage = view.FindViewById<ImageView>(Resource.Id.UserFeedbackImpressionThumbsImage);
            impressionDescription = view.FindViewById<TextView>(Resource.Id.UserFeedbackDescription);
            rateButton = view.FindViewById<Button>(Resource.Id.UserFeedbackRateButton);
            laterButton = view.FindViewById<TextView>(Resource.Id.UserFeedbackLaterButton);

            questionGroup = view.FindViewById<Group>(Resource.Id.QuestionView);
            impressionGroup = view.FindViewById<Group>(Resource.Id.ImpressionView);

            userFeedbackTitle.Text = Shared.Resources.RatingTitle;
            yesText.Text = Shared.Resources.RatingYes;
            noText.Text = Shared.Resources.RatingNotReally;
            laterButton.Text = Shared.Resources.Later;
        }

        private int drawableFromImpression(bool impression)
            => impression ? Resource.Drawable.ic_thumbs_up : Resource.Drawable.ic_thumbs_down;

        private string callToActionTitle(bool? impressionIsPositive)
        {
            if (impressionIsPositive == null)
                return string.Empty;

            return impressionIsPositive.Value
                   ? Shared.Resources.RatingViewPositiveCallToActionTitle
                   : Shared.Resources.RatingViewNegativeCallToActionTitle;
        }

        private string callToActionDescription(bool? impressionIsPositive)
        {
            if (impressionIsPositive == null)
                return string.Empty;

            return impressionIsPositive.Value
                   ? Shared.Resources.RatingViewPositiveCallToActionDescriptionDroid
                   : Shared.Resources.RatingViewNegativeCallToActionDescription;
        }

        private string callToActionButtonTitle(bool? impressionIsPositive)
        {
            if (impressionIsPositive == null)
                return string.Empty;

            return impressionIsPositive.Value
                   ? Shared.Resources.RatingViewPositiveCallToActionButtonTitle
                   : Shared.Resources.RatingViewNegativeCallToActionButtonTitle;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            disposeBag.Dispose();
        }
    }
}
