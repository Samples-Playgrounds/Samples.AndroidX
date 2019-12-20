using CoreGraphics;
using Foundation;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class EditProjectViewController : ReactiveViewController<EditProjectViewModel>
    {
        private const double desiredIpadHeight = 360;
        private static readonly nfloat errorVisibleHeight = 16;

        public EditProjectViewController(EditProjectViewModel viewModel)
            : base(viewModel, nameof(EditProjectViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CloseButton.SetTemplateColor(ColorAssets.Text2);

            TitleLabel.Text = Resources.NewProject;
            NameTextField.Placeholder = Resources.ProjectName;
            ErrorLabel.Text = Resources.ProjectNameTakenError;
            DoneButton.SetTitle(Resources.Create, UIControlState.Normal);
            ProjectNameUsedErrorTextHeight.Constant = 0;
            PrivateProjectLabel.Text = Resources.PrivateProject;

            // Name
            NameTextField.Rx().Text()
                .Subscribe(ViewModel.Name.Accept)
                .DisposedBy(DisposeBag);

            ViewModel.Name
                .Subscribe(NameTextField.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            // Color
            ColorPickerOpeningView.Rx()
                .BindAction(ViewModel.PickColor)
                .DisposedBy(DisposeBag);

            ViewModel.Color
                .Select(color => color.ToNativeColor())
                .Subscribe(ColorCircleView.Rx().BackgroundColor())
                .DisposedBy(DisposeBag);

            // Error
            ViewModel.Error
                .Subscribe(ErrorLabel.Rx().Text())
                .DisposedBy(DisposeBag);

            ViewModel.Error
                .Select(e => string.IsNullOrEmpty(e) ? new nfloat(0) : errorVisibleHeight)
                .Subscribe(ProjectNameUsedErrorTextHeight.Rx().Constant())
                .DisposedBy(DisposeBag);

            if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular)
            {
                ViewModel.Error
                    .Select(e => string.IsNullOrEmpty(e) ? desiredIpadHeight : errorVisibleHeight + desiredIpadHeight)
                    .Select(h => new CGSize(0, h))
                    .Subscribe(this.Rx().PreferredContentSize())
                    .DisposedBy(DisposeBag);
            }

            // Workspace
            WorkspaceLabel.Rx()
                .BindAction(ViewModel.PickWorkspace)
                .DisposedBy(DisposeBag);

            ViewModel.WorkspaceName
                .Subscribe(WorkspaceLabel.Rx().Text())
                .DisposedBy(DisposeBag);

            // Client
            ClientLabel.Rx()
                .BindAction(ViewModel.PickClient)
                .DisposedBy(DisposeBag);

            var emptyText = Resources.AddClient.PrependWithAddIcon(ClientLabel.Font.CapHeight);
            ViewModel.ClientName
                .Select(attributedClientName)
                .Subscribe(ClientLabel.Rx().AttributedText())
                .DisposedBy(DisposeBag);

            // Is Private
            PrivateProjectSwitch.Rx().Changed()
                .Select(_ => PrivateProjectSwitch.On)
                .Subscribe(ViewModel.IsPrivate.Accept)
                .DisposedBy(DisposeBag);

            ViewModel.CanCreatePublicProjects
                .Subscribe(PrivateProjectSwitchContainer.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            // Save
            DoneButton.Rx()
                .BindAction(ViewModel.Save)
                .DisposedBy(DisposeBag);

            CloseButton.Rx().Tap()
                .Subscribe(() => ViewModel.CloseWithDefaultResult())
                .DisposedBy(DisposeBag);

            NSAttributedString attributedClientName(string clientName)
            {
                if (string.IsNullOrEmpty(clientName))
                    return emptyText;

                return new NSAttributedString(clientName);
            }

            NameView.InsertSeparator();
            WorkspaceView.InsertSeparator();
            ClientView.InsertSeparator();
            PrivateProjectSwitchContainer.InsertSeparator();
        }
    }
}

