using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Foundation;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.ViewControllers.Settings.Models;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Cells.Settings
{
    public partial class SettingCell : BaseTableViewCell<ISettingRow>
    {
        public static readonly string Identifier = nameof(SettingCell);
        public static readonly UINib Nib;

        private CompositeDisposable disposeBag = new CompositeDisposable();

        static SettingCell()
        {
            Nib = UINib.FromName("SettingCell", NSBundle.MainBundle);
        }

        protected SettingCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            TitleLabel.TextColor = ColorAssets.Text;
            DetailLabel.TextColor = ColorAssets.Text2;
            this.InsertSeparator();
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
            disposeBag.Dispose();
            disposeBag = new CompositeDisposable();
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Item.Title;
            DetailLabel.Text = "";
            Accessory = UITableViewCellAccessory.None;
            RightConstraint.Constant = 16;
            AccessoryView = null;

            switch (Item)
            {
                case ButtonRow button:
                    break;
                case ToggleRow toggle:
                    var switchControl = new UISwitch();
                    switchControl.On = toggle.Value;
                    switchControl.Rx().Changed()
                        .Delay(TimeSpan.FromSeconds(0.5)) // This is so the switch animation has time to finish before refresh
                        .Subscribe(_ => toggle.Action.Execute())
                        .DisposedBy(disposeBag);

                    AccessoryView = switchControl;
                    break;
                case NavigationRow navigation:
                    DetailLabel.Text = navigation.Detail;
                    Accessory = UITableViewCellAccessory.DisclosureIndicator;
                    RightConstraint.Constant = 0;
                    break;
                case InfoRow info:
                    DetailLabel.Text = info.Detail;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            SetNeedsLayout();
        }
    }
}

