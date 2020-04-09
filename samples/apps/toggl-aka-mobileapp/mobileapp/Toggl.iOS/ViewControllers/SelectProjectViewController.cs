using System;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.ViewSources;
using UIKit;
using Toggl.Shared;
using static Toggl.Shared.Extensions.ReactiveExtensions;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class SelectProjectViewController : ReactiveViewController<SelectProjectViewModel>
    {
        public SelectProjectViewController(SelectProjectViewModel viewModel)
            : base(viewModel, nameof(SelectProjectViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CloseButton.SetTemplateColor(ColorAssets.Text2);

            SearchView.InsertSeparator();

            TitleLabel.Text = Resources.Projects;
            EmptyStateLabel.Text = Resources.EmptyProjectText;

            var source = new SelectProjectTableViewSource();
            source.RegisterViewCells(ProjectsTableView);

            source.UseGrouping = ViewModel.UseGrouping;

            ProjectsTableView.TableFooterView = new UIView();
            ProjectsTableView.Source = source;

            source.Rx().DragStarted()
                .Subscribe(_ => TextField.ResignFirstResponder())
                .DisposedBy(DisposeBag);

            ViewModel.Suggestions
                .Subscribe(ProjectsTableView.Rx().ReloadSections(source))
                .DisposedBy(DisposeBag);

            ViewModel.IsEmpty
                .Subscribe(EmptyStateLabel.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.IsEmpty
                .Subscribe(EmptyStateImage.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.PlaceholderText
                .Subscribe(TextField.Rx().PlaceholderText())
                .DisposedBy(DisposeBag);

            TextField.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);

            CloseButton.Rx().Tap()
                .Subscribe(() => ViewModel.CloseWithDefaultResult())
                .DisposedBy(DisposeBag);

            source.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectProject.Inputs)
                .DisposedBy(DisposeBag);

            source.ToggleTaskSuggestions
                .Subscribe(ViewModel.ToggleTaskSuggestions.Inputs)
                .DisposedBy(DisposeBag);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            TextField.BecomeFirstResponder();

            BottomConstraint.Active |= TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact;
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            View.ClipsToBounds |= TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular;
        }
    }
}
