using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Java.IO;

using pdftron.PDF.Controls;
using pdftron.PDF.Config;
using pdftron.Demo.Utils;

namespace CompleteReaderAndroid
{
    [Activity(Label = "MyReaderActivity",
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden,
        Theme = "@style/CustomAppTheme", 
        WindowSoftInputMode = SoftInput.AdjustPan)]
    public class MyReaderActivity : Android.Support.V7.App.AppCompatActivity
    {
        ViewerConfig mViewerConfig;
        PdfViewCtrlTabHostFragment mPdfViewCtrlTabHostFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AppUtils.InitializePDFNetApplication(this);

            SetContentView(Resource.Layout.my_reader);

            if (Intent != null && Intent.Extras != null)
            {
                mViewerConfig = Intent.Extras.GetParcelable("extra_config") as ViewerConfig;
            }

            File file = pdftron.PDF.Tools.Utils.Utils.CopyResourceToLocal(this, Resource.Raw.getting_started,
                    "getting_started", ".pdf");

            var fileUri = Android.Net.Uri.FromFile(file);
            StartTabHostFragment(fileUri, "");
        }

        public override void OnBackPressed()
        {
            bool handled = false;
            if (mPdfViewCtrlTabHostFragment != null)
            {
                handled = mPdfViewCtrlTabHostFragment.HandleBackPressed();
            }
            if (!handled)
            {
                base.OnBackPressed();
            }
        }

        private void StartTabHostFragment(Android.Net.Uri fileUri, String password)
        {
            if (IsFinishing)
            {
                return;
            }

            Android.Support.V4.App.FragmentTransaction ft = this.SupportFragmentManager.BeginTransaction();
            mPdfViewCtrlTabHostFragment = (PdfViewCtrlTabHostFragment) ViewerBuilder
                .WithUri(fileUri, password)
                .UsingNavIcon(Resource.Drawable.ic_arrow_back_white_24dp)
                .UsingConfig(mViewerConfig)
                .Build(this);
            mPdfViewCtrlTabHostFragment.NavButtonPressed += (sender, e) =>
            {
                Finish();
            };
            mPdfViewCtrlTabHostFragment.LastTabClosed += (sender, e) =>
            {
                Finish();
            };

            ft.Replace(Resource.Id.container, mPdfViewCtrlTabHostFragment, null);
            ft.Commit();
        }
    }
}