using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Views.Client;
using Toggl.iOS.ViewSources;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public partial class SelectClientViewController : ReactiveViewController<SelectClientViewModel>
    {
        public SelectClientViewController(SelectClientViewModel viewModel)
            : base(viewModel, nameof(SelectClientViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CloseButton.SetTemplateColor(ColorAssets.Text2);
            SearchView.InsertSeparator();

            TitleLabel.Text = Resources.Clients;
            SearchTextField.Placeholder = Resources.AddFilterClients;

            SuggestionsTableView.RegisterNibForCellReuse(ClientViewCell.Nib, ClientViewCell.Identifier);
            SuggestionsTableView.RegisterNibForCellReuse(CreateClientViewCell.Nib, CreateClientViewCell.Identifier);
            SuggestionsTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            var tableViewSource = new ClientTableViewSource(SuggestionsTableView);
            SuggestionsTableView.Source = tableViewSource;

            tableViewSource.Rx().DragStarted()
                .Subscribe(_ => SearchTextField.ResignFirstResponder())
                .DisposedBy(DisposeBag);

            var clientsReplay = ViewModel.Clients.Replay();

            clientsReplay
                .Subscribe(SuggestionsTableView.Rx().ReloadItems(tableViewSource))
                .DisposedBy(DisposeBag);

            clientsReplay.Connect();

            CloseButton.Rx().Tap()
                .Subscribe(() => ViewModel.CloseWithDefaultResult())
                .DisposedBy(DisposeBag);

            SearchTextField.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);

            tableViewSource.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectClient.Inputs)
                .DisposedBy(DisposeBag);

            BottomConstraint.Active |= TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SearchTextField.BecomeFirstResponder();
        }

        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            View.ClipsToBounds |= TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular;
        }
    }
}
