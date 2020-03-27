using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.ViewSources;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class SelectTagsViewController : ReactiveViewController<SelectTagsViewModel>
    {
        public SelectTagsViewController(SelectTagsViewModel viewModel)
            : base(viewModel, nameof(SelectTagsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CloseButton.SetTemplateColor(ColorAssets.Text2);

            SearchView.InsertSeparator();

            TitleLabel.Text = Resources.Tags;
            TextField.Placeholder = Resources.AddFilterTags;
            EmptyStateLabel.Text = Resources.EmptyTagText;
            SaveButton.SetTitle(Resources.Save, UIControlState.Normal);

            var tableViewSource = new SelectTagsTableViewSource(TagsTableView);

            tableViewSource.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectTag.Inputs)
                .DisposedBy(DisposeBag);

            tableViewSource.Rx().DragStarted()
                .Subscribe(_ => TextField.ResignFirstResponder())
                .DisposedBy(DisposeBag);

            var tagsReplay = ViewModel.Tags.Replay();

            tagsReplay
                .Subscribe(TagsTableView.Rx().ReloadItems(tableViewSource))
                .DisposedBy(DisposeBag);

            tagsReplay.Connect();

            ViewModel.IsEmpty
                .Subscribe(EmptyStateImage.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsEmpty
                .Subscribe(EmptyStateLabel.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.FilterText
                .Subscribe(TextField.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            CloseButton.Rx().Tap()
                .Subscribe(() => ViewModel.CloseWithDefaultResult())
                .DisposedBy(DisposeBag);

            SaveButton.Rx()
                .BindAction(ViewModel.Save)
                .DisposedBy(DisposeBag);

            TextField.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);

            BottomConstraint.Active |= TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TextField.BecomeFirstResponder();
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            View.ClipsToBounds |= TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular;
        }
    }
}
