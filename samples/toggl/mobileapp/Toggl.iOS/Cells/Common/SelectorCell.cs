using System;
using Foundation;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Cells.Common
{
    public sealed partial class SelectorCell : BaseTableViewCell<string>
    {
        public static readonly string Identifier = nameof(SelectorCell);
        public static readonly UINib Nib;

        private bool _optionSelected;
        public bool OptionSelected
        {
            get => _optionSelected;
            set
            {
                _optionSelected = value;
                CheckMark.Hidden = !value;
            }
        }

        static SelectorCell()
        {
            Nib = UINib.FromName(nameof(SelectorCell), NSBundle.MainBundle);
        }

        public SelectorCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        protected override void UpdateView()
        {
            NameLabel.Text = Item;
            CheckMark.Hidden = !OptionSelected;
            BackgroundColor = ColorAssets.CellBackground;
        }
    }
}
