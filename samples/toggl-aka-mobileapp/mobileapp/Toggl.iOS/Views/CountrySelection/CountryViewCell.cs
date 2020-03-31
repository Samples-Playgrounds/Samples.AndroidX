using Foundation;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views.CountrySelection
{
    public sealed partial class CountryViewCell : BaseTableViewCell<SelectableCountryViewModel>
    {
        public static readonly string Identifier = nameof(CountryViewCell);
        public static readonly UINib Nib;

        static CountryViewCell()
        {
            Nib = UINib.FromName(nameof(CountryViewCell), NSBundle.MainBundle);
        }

        public CountryViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            ContentView.InsertSeparator();
        }

        protected override void UpdateView()
        {
            NameLabel.Text = Item.Country.Name;
            CheckBoxImageView.Hidden = !Item.Selected;
        }
    }
}
