using CoreGraphics;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Views;
using Toggl.iOS.ViewSources;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public sealed partial class SelectColorViewController : ReactiveViewController<SelectColorViewModel>
    {
        private const int customColorEnabledHeightPad = 490;
        private const int customColorEnabledHeight = 365;
        private const int customColorDisabledHeight = 233;

        private ColorSelectionCollectionViewSource source;

        public SelectColorViewController(SelectColorViewModel viewModel)
            : base(viewModel, nameof(SelectColorViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CloseButton.SetTemplateColor(ColorAssets.Text2);

            TitleLabel.Text = Resources.ProjectColor;
            SaveButton.SetTitle(Resources.Save, UIControlState.Normal);

            prepareViews();

            //Collection View
            ColorCollectionView.RegisterNibForCell(ColorSelectionViewCell.Nib, ColorSelectionViewCell.Identifier);
            source = new ColorSelectionCollectionViewSource(ViewModel.SelectableColors);
            ColorCollectionView.Source = source;
            ViewModel.SelectableColors
                .Subscribe(replaceColors)
                .DisposedBy(DisposeBag);

            source.ColorSelected
                .Subscribe(ViewModel.SelectColor.Inputs)
                .DisposedBy(DisposeBag);

            // Commands
            SaveButton.Rx()
                .BindAction(ViewModel.Save)
                .DisposedBy(DisposeBag);

            CloseButton.Rx().Tap()
                .Subscribe(() => ViewModel.CloseWithDefaultResult())
                .DisposedBy(DisposeBag);

            // Picker view
            PickerView.Rx().Hue()
                .Subscribe(ViewModel.SetHue.Inputs)
                .DisposedBy(DisposeBag);


            PickerView.Rx().Saturation()
                .Subscribe(ViewModel.SetSaturation.Inputs)
                .DisposedBy(DisposeBag);

            SliderView.Rx().Value()
                .Select(v => 1 - v)
                .Subscribe(ViewModel.SetValue.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.Hue
                .Subscribe(PickerView.Rx().HueObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Saturation
                .Subscribe(PickerView.Rx().SaturationObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Value
                .Subscribe(PickerView.Rx().ValueObserver())
                .DisposedBy(DisposeBag);
        }

        private void prepareViews()
        {
            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            PreferredContentSize = new CGSize
            {
                // ScreenWidth - 32 for 16pt margins on both sides
                Width = TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular
                    ? 0
                    : screenWidth - 32,
                Height = TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Regular
                    ? (ViewModel.AllowCustomColors ? customColorEnabledHeightPad : customColorDisabledHeight)
                    : (ViewModel.AllowCustomColors ? customColorEnabledHeight : customColorDisabledHeight)
            };

            if (!ViewModel.AllowCustomColors)
            {
                SliderView.RemoveFromSuperview();
                PickerView.RemoveFromSuperview();
                SliderBackgroundView.RemoveFromSuperview();
                return;
            }

            // Remove track
            SliderView.SetMinTrackImage(new UIImage(), UIControlState.Normal);
            SliderView.SetMaxTrackImage(new UIImage(), UIControlState.Normal);
        }

        private void replaceColors(IEnumerable<SelectableColorViewModel> colors)
        {
            source.SetNewColors(colors);
            ColorCollectionView.ReloadData();
        }
    }
}

