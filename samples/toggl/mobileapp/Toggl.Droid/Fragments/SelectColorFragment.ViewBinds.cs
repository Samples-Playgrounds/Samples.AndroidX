using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Adapters;
using Toggl.Droid.ViewHolders;
using Toggl.Droid.Views;

namespace Toggl.Droid.Fragments
{
    public partial class SelectColorFragment
    {
        private TextView titleLabel;
        private HueSaturationPickerView hueSaturationPicker;
        private ValueSlider valueSlider;
        private Button saveButton;
        private Button closeButton;
        private RecyclerView recyclerView;
        private SimpleAdapter<SelectableColorViewModel> selectableColorsAdapter;

        protected override void InitializeViews(View view)
        {
            titleLabel = view.FindViewById<TextView>(Resource.Id.SelectColorTitle);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.SelectColorRecyclerView);
            saveButton = view.FindViewById<Button>(Resource.Id.SelectColorSave);
            closeButton = view.FindViewById<Button>(Resource.Id.SelectColorClose);
            hueSaturationPicker = view.FindViewById<HueSaturationPickerView>(Resource.Id.SelectColorHueSaturationPicker);
            valueSlider = view.FindViewById<ValueSlider>(Resource.Id.SelectColorValueSlider);
            
            titleLabel.Text = Shared.Resources.ProjectColor;
            closeButton.Text = Shared.Resources.Cancel;
            saveButton.Text = Shared.Resources.Done;
            selectableColorsAdapter = new SimpleAdapter<SelectableColorViewModel>(
                Resource.Layout.SelectColorFragmentCell, ColorSelectionViewHolder.Create);
            recyclerView.SetLayoutManager(new GridLayoutManager(Context, 5));
            recyclerView.SetAdapter(selectableColorsAdapter);
        }
    }
}
