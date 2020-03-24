using CoreGraphics;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels.Settings.Siri;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings.Siri
{

    public partial class PasteFromClipboardViewController : ReactiveViewController<PasteFromClipboardViewModel>
    {
        private readonly int cardHeight = 374;
        private readonly int cardWidth = 288;

        public PasteFromClipboardViewController(PasteFromClipboardViewModel viewModel)
            : base(viewModel, nameof(PasteFromClipboardViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            localizeViews();

            PreferredContentSize = new CGSize(
                cardWidth,
                cardHeight
            );

            OkayButton.Rx().Tap()
                .Subscribe(() => ViewModel.CloseWithDefaultResult())
                .DisposedBy(DisposeBag);

            DoNotShowAgainButton.Rx()
                .BindAction(ViewModel.DoNotShowAgain)
                .DisposedBy(DisposeBag);
        }

        private void localizeViews()
        {
            TitleLabel.Text = Resources.SiriClipboardInstructionTitle;
            DescriptionLabel.Text = Resources.SiriClipboardInstructionDescription;
            OkayButton.SetTitle(Resources.SiriClipboardInstructionConfirm, UIControlState.Normal);
            DoNotShowAgainButton.SetTitle(Resources.SiriClipboardInstructionDoNotShowAgain, UIControlState.Normal);
        }
    }
}

