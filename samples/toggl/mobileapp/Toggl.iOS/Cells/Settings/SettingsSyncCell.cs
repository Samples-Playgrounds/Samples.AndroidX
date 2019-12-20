using System;
using System.Reactive.Disposables;
using Foundation;
using Toggl.Core.Sync;
using Toggl.iOS.Extensions;
using Toggl.iOS.ViewControllers.Settings.Models;
using Toggl.iOS.ViewSources;
using Toggl.Shared;
using UIKit;
using Colors = Toggl.Core.UI.Helper.Colors;
using static Toggl.Core.Sync.PresentableSyncStatus;

namespace Toggl.iOS.Cells.Settings
{
    public partial class SettingsSyncCell : BaseTableViewCell<CustomRow<PresentableSyncStatus>>
    {
        public static readonly string Identifier = nameof(SettingsSyncCell);
        public static readonly UINib Nib;

        private CompositeDisposable disposeBag = new CompositeDisposable();

        static SettingsSyncCell()
        {
            Nib = UINib.FromName("SettingsSyncCell", NSBundle.MainBundle);
        }

        protected SettingsSyncCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            ContentView.BackgroundColor = ColorAssets.TableBackground;
            StatusLabel.TextColor = ColorAssets.Text2;
            LoadingIcon.IndicatorColor = ColorAssets.Text2;

            ContentView.InsertSeparator();
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
            disposeBag.Dispose();
            disposeBag = new CompositeDisposable();
        }

        protected override void UpdateView()
        {
            switch (Item.CustomValue)
            {
                case Synced:
                    StatusLabel.Text = Resources.SyncCompleted;
                    SyncedIcon.Hidden = false;
                    LoadingIcon.Hidden = true;
                    LoadingIcon.StopSpinning();
                    break;
                case Syncing:
                    StatusLabel.Text = Resources.Syncing;
                    SyncedIcon.Hidden = true;
                    LoadingIcon.Hidden = false;
                    LoadingIcon.StartSpinning();
                    break;
                case LoggingOut:
                    StatusLabel.Text = Resources.LoggingOutSecurely;
                    SyncedIcon.Hidden = true;
                    LoadingIcon.Hidden = false;
                    LoadingIcon.StartSpinning();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Item.CustomValue), Item.CustomValue, null);
            }
        }
    }
}

