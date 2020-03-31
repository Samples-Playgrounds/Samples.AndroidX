using Foundation;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views
{
    public partial class ColorSelectionViewCell : BaseCollectionViewCell<SelectableColorViewModel>
    {
        public static readonly string Identifier = "colorSelectionViewCell";

        public static readonly NSString Key = new NSString(nameof(ColorSelectionViewCell));
        public static readonly UINib Nib;

        static ColorSelectionViewCell()
        {
            Nib = UINib.FromName(nameof(ColorSelectionViewCell), NSBundle.MainBundle);
        }

        protected ColorSelectionViewCell(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            SelectedImageView.SetTemplateColor(UIColor.White);
        }

        protected override void UpdateView()
        {
            ColorCircleView.BackgroundColor = Item.Color.ToNativeColor();
            SelectedImageView.Hidden = !Item.Selected;
        }
    }
}
