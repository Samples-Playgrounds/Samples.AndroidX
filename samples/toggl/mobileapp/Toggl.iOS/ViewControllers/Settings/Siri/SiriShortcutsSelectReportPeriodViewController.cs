using Foundation;
using Intents;
using IntentsUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Intents;
using Toggl.iOS.Views.Settings;
using Toggl.iOS.ViewSources.Generic.TableView;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
    using ReportSection = SectionModel<Unit, SelectableReportPeriodViewModel>;
    public partial class SiriShortcutsSelectReportPeriodViewController : ReactiveViewController<SiriShortcutsSelectReportPeriodViewModel>, IINUIAddVoiceShortcutViewControllerDelegate
    {
        private const int rowHeight = 48;

        public SiriShortcutsSelectReportPeriodViewController(SiriShortcutsSelectReportPeriodViewModel viewModel)
            : base(viewModel, nameof(SiriShortcutsSelectReportPeriodViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            prepareSiriButton();

            NavigationItem.Title = Resources.ReportPeriod;
            SelectWorkspaceCellLabel.Text = Resources.SelectWorkspace;

            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            TableView.RegisterNibForCellReuse(SiriShortcutReportPeriodCell.Nib, SiriShortcutReportPeriodCell.Identifier);

            TableView.RowHeight = rowHeight;
            TableView.TableFooterView = TableFooterView;

            var source =
                new CustomTableViewSource<ReportSection, Unit,
                    SelectableReportPeriodViewModel>(
                    SiriShortcutReportPeriodCell.CellConfiguration(SiriShortcutReportPeriodCell.Identifier)
                    );

            ViewModel.ReportPeriods
                .Subscribe(TableView.Rx().ReloadItems(source))
                .DisposedBy(DisposeBag);

            source.Rx().ModelSelected()
                .Select(p => p.ReportPeriod)
                .Subscribe(ViewModel.SelectReportPeriod.Accept)
                .DisposedBy(DisposeBag);

            TableView.Source = source;

            SelectWorkspaceView.Rx()
                .BindAction(ViewModel.PickWorkspace)
                .DisposedBy(DisposeBag);

            ViewModel.WorkspaceName
                .Subscribe(SelectWorkspaceNameLabel.Rx().Text())
                .DisposedBy(DisposeBag);
        }

        private void prepareSiriButton()
        {
            var button = new INUIAddVoiceShortcutButton(INUIAddVoiceShortcutButtonStyle.Black);
            button.TranslatesAutoresizingMaskIntoConstraints = false;

            var descriptionLabel = new UILabel
            {
                Text = Resources.SiriReportPeriodInstruction,
                Font = UIFont.SystemFontOfSize(12),
                TextColor = Colors.Siri.InvocationPhrase.ToNativeColor()
            };
            descriptionLabel.TranslatesAutoresizingMaskIntoConstraints = false;

            AddToSiriWrapperView.AddSubview(button);
            AddToSiriWrapperView.AddSubview(descriptionLabel);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                descriptionLabel.CenterXAnchor.ConstraintEqualTo(AddToSiriWrapperView.CenterXAnchor),
                descriptionLabel.TopAnchor.ConstraintEqualTo(AddToSiriWrapperView.TopAnchor, 16),
                button.CenterXAnchor.ConstraintEqualTo(AddToSiriWrapperView.CenterXAnchor),
                button.TopAnchor.ConstraintEqualTo(descriptionLabel.BottomAnchor, 16),
                button.WidthAnchor.ConstraintEqualTo(150),
                button.HeightAnchor.ConstraintEqualTo(50),
            });

            button.TouchUpInside += siriButtonHandler;
        }

        private void siriButtonHandler(object sender, EventArgs args)
        {
            if (!(ViewModel.SelectedWorkspace.Value is IThreadSafeWorkspace workspace))
            {
                return;
            }

            var intent = new ShowReportPeriodIntent();
            intent.Period = ViewModel.SelectReportPeriod.Value.ToShowReportPeriodReportPeriod();
            intent.Workspace = new INObject(workspace.Id.ToString(), workspace.Name);

            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(null);

            var vc = new INUIAddVoiceShortcutViewController(new INShortcut(intent));
            vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            vc.Delegate = this;
            PresentViewController(vc, true, null);
        }

        public void DidFinish(INUIAddVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            controller.DismissViewController(true, null);
            ViewModel.CloseWithDefaultResult();
        }

        public void DidCancel(INUIAddVoiceShortcutViewController controller)
        {
            controller.DismissViewController(true, null);
        }
    }
}

