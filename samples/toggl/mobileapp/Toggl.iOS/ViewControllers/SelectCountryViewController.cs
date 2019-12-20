using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Views.CountrySelection;
using Toggl.iOS.ViewSources.Generic.TableView;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class SelectCountryViewController : KeyboardAwareViewController<SelectCountryViewModel>
    {
        private const int rowHeight = 48;

        public SelectCountryViewController(SelectCountryViewModel viewModel) : base(viewModel, nameof(SelectCountryViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            SearchView.InsertSeparator();

            CloseButton.SetTemplateColor(ColorAssets.Text2);
            SearchTextField.TintColor = ColorAssets.Text2;

            TitleLabel.Text = Resources.CountryOfResidence;

            CountriesTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            CountriesTableView.RegisterNibForCellReuse(CountryViewCell.Nib, CountryViewCell.Identifier);
            CountriesTableView.RowHeight = rowHeight;

            var source = new CustomTableViewSource<SectionModel<string, SelectableCountryViewModel>, string, SelectableCountryViewModel>(
                CountryViewCell.CellConfiguration(CountryViewCell.Identifier));

            CountriesTableView.Source = source;

            source.Rx().ModelSelected()
                .Subscribe(ViewModel.SelectCountry.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.Countries
                .Subscribe(CountriesTableView.Rx().ReloadItems(source))
                .DisposedBy(DisposeBag);

            CloseButton.Rx().Tap()
                .Subscribe(() => ViewModel.CloseWithDefaultResult())
                .DisposedBy(DisposeBag);

            SearchTextField.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);

            SearchTextField.BecomeFirstResponder();
        }

        protected override void KeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            BottomConstraint.AnimateSetConstant(e.FrameEnd.Height, View);
        }

        protected override void KeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            BottomConstraint.AnimateSetConstant(0, View);
        }
    }
}

