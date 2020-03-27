//
// Copyright (c) 2001-2019 by PDFTron Systems Inc. All Rights Reserved.
//
using System;

using Android.App;
using Android.Widget;
using Android.OS;

using Android.Support.V7.Widget;

using pdftron.PDF.Config;

namespace CompleteReaderAndroid
{
    [Activity(Label = "CompleteReaderAndroid", MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : Android.Support.V7.App.AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            try
            {
                pdftron.Demo.Utils.AppUtils.InitializePDFNetApplication(this, pdftron.Demo.Key.LicenseKey);
                Console.WriteLine(pdftron.PDFNet.GetVersion());
            }
            catch (pdftron.Common.PDFNetException e)
            {
                Console.WriteLine(e.GetMessage());
                return;
            }

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var showActivity = FindViewById<AppCompatButton>(Resource.Id.activityButton);
            showActivity.Click += ShowActivity_Click;

            var showFragment = FindViewById<AppCompatButton>(Resource.Id.fragmentButton);
            showFragment.Click += ShowFragment_Click;

            var demoApp = FindViewById<AppCompatButton>(Resource.Id.demoAppButton);
            demoApp.Click += DemoApp_Click;
        }

        private void DemoApp_Click(object sender, EventArgs e)
        {
            pdftron.Demo.App.AdvancedReaderActivity.Open(this);
        }

        private void ShowFragment_Click(object sender, System.EventArgs e)
        {
            var config = getConfig();
            var intent = new Android.Content.Intent(this, typeof(MyReaderActivity));
            intent.PutExtra("extra_config", config);
            StartActivity(intent);
        }

        private void ShowActivity_Click(object sender, System.EventArgs e)
        {
            var config = getConfig();
            pdftron.Demo.App.SimpleReaderActivity.Open(this, config);
        }

        private ViewerConfig getConfig()
        {
            var builder = new ViewerConfig.Builder();

            var pdfViewCtrlConfig = PDFViewCtrlConfig.GetDefaultConfig(this)
                .SetHighlightFields(getCheckedState(Resource.Id.HighlightFields))
                .SetImageSmoothing(getCheckedState(Resource.Id.ImageSmoothing))
                .SetMaintainZoomEnabled(getCheckedState(Resource.Id.MaintainZoom));

            var toolManagerBuilder = ToolManagerBuilder.From(this, Resource.Style.MyToolManager);
            toolManagerBuilder.SetDoubleTapToZoom(getCheckedState(Resource.Id.double_tap_to_zoom));

            var config = builder
                .FullscreenModeEnabled(getCheckedState(Resource.Id.FullscreenModeEnabled))
                .MultiTabEnabled(getCheckedState(Resource.Id.MultiTabEnabled))
                .DocumentEditingEnabled(getCheckedState(Resource.Id.DocumentEditingEnabled))
                .LongPressQuickMenuEnabled(getCheckedState(Resource.Id.LongPressQuickMenuEnabled))
                .ShowPageNumberIndicator(getCheckedState(Resource.Id.ShowPageNumberIndicator))
                .ShowBottomNavBar(getCheckedState(Resource.Id.ShowBottomNavBar))
                .ShowThumbnailView(getCheckedState(Resource.Id.ShowThumbnailView))
                .ShowBookmarksView(getCheckedState(Resource.Id.ShowBookmarksView))
                .ToolbarTitle("PDFTron")
                .ShowSearchView(getCheckedState(Resource.Id.ShowSearchView))
                .ShowShareOption(getCheckedState(Resource.Id.ShowShareOption))
                .ShowDocumentSettingsOption(getCheckedState(Resource.Id.ShowDocumentSettingsOption))
                .ShowAnnotationToolbarOption(getCheckedState(Resource.Id.ShowAnnotationToolbarOption))
                .ShowOpenFileOption(getCheckedState(Resource.Id.ShowOpenFileOption))
                .ShowOpenUrlOption(getCheckedState(Resource.Id.ShowOpenUrlOption))
                .ShowEditPagesOption(getCheckedState(Resource.Id.ShowEditPagesOption))
                .ShowPrintOption(getCheckedState(Resource.Id.ShowPrintOption))
                .ShowCloseTabOption(getCheckedState(Resource.Id.ShowCloseTabOption))
                .ShowAnnotationsList(getCheckedState(Resource.Id.ShowAnnotationsList))
                .ShowOutlineList(getCheckedState(Resource.Id.ShowOutlineList))
                .ShowUserBookmarksList(getCheckedState(Resource.Id.ShowUserBookmarksList))
                .PdfViewCtrlConfig(pdfViewCtrlConfig)
                .ToolManagerBuilder(toolManagerBuilder)
                .Build();

            return config;
        }

        private bool getCheckedState(int id)
        {
            var checkbox = FindViewById<CheckBox>(id);
            return checkbox.Checked;
        }
    }
}

