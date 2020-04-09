using Foundation;
using Intents;
using IntentsUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Intents;
using Toggl.iOS.Transformations;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;
using Colors = Toggl.Core.UI.Helper.Colors;

namespace Toggl.iOS.ViewControllers.Settings.Siri
{
    public partial class SiriShortcutsCustomTimeEntryViewController : KeyboardAwareViewController<SiriShortcutsCustomTimeEntryViewModel>, IINUIAddVoiceShortcutViewControllerDelegate
    {
        private ProjectTaskClientToAttributedString projectTaskClientToAttributedString;
        private TagsListToAttributedString tagsListToAttributedString;
        private UIImage pasteFromClipboardButtonImage = UIImage.FromBundle("pasteFromClipboard");
        private UIImage pasteFromClipboardButtonImageEnabled = UIImage.FromBundle("pasteFromClipboardEnabled");

        public SiriShortcutsCustomTimeEntryViewController(SiriShortcutsCustomTimeEntryViewModel viewModel)
            : base(viewModel, nameof(SiriShortcutsCustomTimeEntryViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            prepareViews();
            prepareSiriButton();
            localizeLabels();

            BillableSwitch.Rx().Changed()
                .Select(_ => BillableSwitch.On)
                .Subscribe(ViewModel.IsBillable.Accept)
                .DisposedBy(DisposeBag);

            TagsTextView.Rx()
                .BindAction(ViewModel.SelectTags)
                .DisposedBy(DisposeBag);

            ViewModel.HasTags
                .Invert()
                .Subscribe(AddTagsView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.HasTags
                .Subscribe(TagsTextView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.TagNames
                .Select(tagsListToAttributedString.Convert)
                .Subscribe(TagsTextView.Rx().AttributedTextObserver())
                .DisposedBy(DisposeBag);

            SelectTagsView.Rx()
                .BindAction(ViewModel.SelectTags)
                .DisposedBy(DisposeBag);

            SelectProjectView.Rx()
                .BindAction(ViewModel.SelectProject)
                .DisposedBy(DisposeBag);

            ViewModel.ProjectClientTask
                .Select(info => projectTaskClientToAttributedString.Convert(
                    info.Project,
                    info.Task,
                    info.Client,
                    new Color(info.ProjectColor).ToNativeColor()))
                .Subscribe(ProjectTaskClientLabel.Rx().AttributedText())
                .DisposedBy(DisposeBag);

            ViewModel.ProjectClientTask
                .Select(info => info.HasProject)
                .Subscribe(ProjectTaskClientLabel.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.ProjectClientTask
                .Select(info => !info.HasProject)
                .Subscribe(AddProjectAndTaskView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsBillable
                .Subscribe(BillableSwitch.Rx().CheckedObserver())
                .DisposedBy(DisposeBag);

            ViewModel.IsBillableAvailable
                .Subscribe(BillableView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.PasteFromClipboard
                .Invert()
                .Subscribe(DescriptionTextView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.PasteFromClipboard
                .Subscribe(DescriptionUsingClipboardWrapperView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.PasteFromClipboard
                .Subscribe(PasteFromClipboardHintView.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.PasteFromClipboard
                .Select(enabled => enabled ? pasteFromClipboardButtonImageEnabled : pasteFromClipboardButtonImage)
                .Subscribe(image =>
                {
                    PasteFromClipboardButton.SetImage(image, UIControlState.Normal);
                })
                .DisposedBy(DisposeBag);

            ViewModel.PasteFromClipboard
                .Subscribe(enabled =>
                {
                    if (enabled)
                    {
                        DescriptionTextView.ResignFirstResponder();
                    }
                    else
                    {
                        DescriptionTextView.BecomeFirstResponder();
                    }
                })
                .DisposedBy(DisposeBag);

            DescriptionTextView.TextObservable
                .Subscribe(ViewModel.Description.Accept)
                .DisposedBy(DisposeBag);

            PasteFromClipboardButton.Rx()
                .BindAction(ViewModel.SelectClipboard)
                .DisposedBy(DisposeBag);
        }

        protected override void KeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            UIEdgeInsets contentInsets = new UIEdgeInsets(0, 0, e.FrameEnd.Height, 0);
            ScrollView.ContentInset = contentInsets;
            ScrollView.ScrollIndicatorInsets = contentInsets;
        }

        protected override void KeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            ScrollView.ContentInset = UIEdgeInsets.Zero;
            ScrollView.ScrollIndicatorInsets = UIEdgeInsets.Zero;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            DescriptionTextView.BecomeFirstResponder();
        }
        private void localizeLabels()
        {
            NavigationItem.Title = Resources.SiriShortcutCustomTimeEntryTitle;
            BillabelLabel.Text = Resources.Billable;
            AddProjectTaskLabel.Text = Resources.AddProjectTask;
            AddTagsLabel.Text = Resources.AddTags;
            DescriptionFromClipboardLabel.Text = Resources.SiriShortcutDescriptionFromClipboard;
            PasteFromClipboardHintLabel.Text = Resources.SiriClipboardHintLabel;
        }

        private void prepareViews()
        {
            projectTaskClientToAttributedString = new ProjectTaskClientToAttributedString(
                ProjectTaskClientLabel.Font.CapHeight,
                Colors.EditTimeEntry.ClientText.ToNativeColor());

            tagsListToAttributedString = new TagsListToAttributedString(TagsTextView);

            centerTextVertically(TagsTextView);
            TagsTextView.TextContainer.LineFragmentPadding = 0;

            DescriptionTextView.TintColor = Colors.StartTimeEntry.Cursor.ToNativeColor();
            DescriptionTextView.PlaceholderText = Resources.AddDescription;
            DescriptionTextView.TextColor = ColorAssets.Text;

            DescriptionView.InsertSeparator();
            PasteFromClipboardHintView.InsertSeparator();
            SelectProjectView.InsertSeparator();
            SelectTagsView.InsertSeparator();
            BillableView.InsertSeparator();
        }

        private void centerTextVertically(UITextView textView)
        {
            var topOffset = (textView.Bounds.Height - textView.ContentSize.Height) / 2;
            textView.ContentInset = new UIEdgeInsets(topOffset, 0, 0, 0);
        }

        private void prepareSiriButton()
        {
            var button = new INUIAddVoiceShortcutButton(INUIAddVoiceShortcutButtonStyle.Black);
            button.TranslatesAutoresizingMaskIntoConstraints = false;

            var descriptionLabel = new UILabel
            {
                Text = Resources.SiriCustomTimeEntryInstruction,
                Font = UIFont.SystemFontOfSize(12),
                TextColor = ColorAssets.Text
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
            var intent = constructStartTimerIntent(ViewModel.PasteFromClipboard.Value);

            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(null);

            var vc = new INUIAddVoiceShortcutViewController(new INShortcut(intent));
            vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
            vc.Delegate = this;
            PresentViewController(vc, true, null);
        }

        private INIntent constructStartTimerIntent(bool fromClipboard)
        {
            if (!(ViewModel.Workspace.Value is IThreadSafeWorkspace selectedWorkspace))
            {
                return null;
            }

            var workspace = new INObject(selectedWorkspace.Id.ToString(), selectedWorkspace.Name);

            INObject project = null;
            if (ViewModel.Project.Value is IThreadSafeProject selectedProject)
            {
                project = new INObject(selectedProject.Id.ToString(), selectedProject.Name);
            }

            INObject[] tags = null;
            if (ViewModel.Tags.Value.Any())
            {
                tags = ViewModel.Tags
                    .Value
                    .Select(tag => new INObject(tag.Id.ToString(), tag.Id.ToString()))
                    .ToArray();
            }

            var billable = new INObject(ViewModel.IsBillable.Value.ToString(), ViewModel.IsBillable.Value.ToString());

            if (fromClipboard)
            {
                return new StartTimerFromClipboardIntent
                {
                    Workspace = workspace,
                    ProjectId = project,
                    Tags = tags,
                    Billable = billable
                };
            }

            var entryDescription = ViewModel.Description.Value;
            return new StartTimerIntent
            {
                Workspace = workspace,
                ProjectId = project,
                Tags = tags,
                Billable = billable,
                EntryDescription = ViewModel.Description.Value,
                SuggestedInvocationPhrase = string.Format(Resources.SiriTrackEntrySuggestedInvocationPhrase, entryDescription)
            };
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

