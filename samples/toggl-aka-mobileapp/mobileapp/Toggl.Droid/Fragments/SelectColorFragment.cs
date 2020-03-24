using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public sealed partial class SelectColorFragment : ReactiveDialogFragment<SelectColorViewModel>
    {
        private const int customColorEnabledHeight = 437;
        private const int customColorDisabledHeight = 270;

        public SelectColorFragment() { }

        public SelectColorFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SelectColorFragment, null);

            InitializeViews(view);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            selectableColorsAdapter.ItemTapObservable
                .Select(x => x.Color)
                .Subscribe(ViewModel.SelectColor.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.Hue
                .Subscribe(hueSaturationPicker.Rx().HueObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Saturation
                .Subscribe(hueSaturationPicker.Rx().SaturationObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Value
                .Subscribe(hueSaturationPicker.Rx().ValueObserver())
                .DisposedBy(DisposeBag);

            hueSaturationPicker.Rx().Hue()
                .Subscribe(ViewModel.SetHue.Inputs)
                .DisposedBy(DisposeBag);

            hueSaturationPicker.Rx().Saturation()
                .Subscribe(ViewModel.SetSaturation.Inputs)
                .DisposedBy(DisposeBag);

            valueSlider.Rx().Progress()
                .Select(invertedNormalizedProgress)
                .Subscribe(ViewModel.SetValue.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.Hue
                .Subscribe(valueSlider.Rx().HueObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Saturation
                .Subscribe(valueSlider.Rx().SaturationObserver())
                .DisposedBy(DisposeBag);

            saveButton.Rx()
                .BindAction(ViewModel.Save)
                .DisposedBy(DisposeBag);

            closeButton.Rx().Tap()
                .Subscribe(() => ViewModel.CloseWithDefaultResult())
                .DisposedBy(DisposeBag);

            ViewModel.SelectableColors
                .Subscribe(selectableColorsAdapter.Rx().Items())
                .DisposedBy(DisposeBag);

            hueSaturationPicker.Visibility = ViewModel.AllowCustomColors.ToVisibility();
            valueSlider.Visibility = ViewModel.AllowCustomColors.ToVisibility();
        }

        public override void OnResume()
        {
            base.OnResume();

            var height = ViewModel.AllowCustomColors ? customColorEnabledHeight : customColorDisabledHeight;

            Dialog.Window.SetDefaultDialogLayout(Activity, Context, heightDp: height);
        }

        private float invertedNormalizedProgress(int progress)
            => 1f - (progress / 100f);
    }
}
