//
// Copyright (c) 2001-2019 by PDFTron Systems Inc. All Rights Reserved.
//

using System;
using System.IO;
using System.Collections;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Graphics.Drawables;

using Android.Arch.Lifecycle;

using pdftron;
using pdftron.Common;
using pdftron.PDF;
using pdftron.PDF.Tools;
using pdftron.PDF.Controls;
using pdftron.PDF.Tools.Utils;
using pdftron.PDF.Config;

using com.xamarin.recipes.filepicker;
using System.Collections.Generic;

namespace PDFNetAndroidXamarinSample
{
    [Activity(Label = "@string/app_name", MainLauncher = true, HardwareAccelerated = true,
        ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden,
        WindowSoftInputMode = SoftInput.AdjustPan,
        Theme = "@style/CustomAppTheme")]
    public class MainActivity : Android.Support.V7.App.AppCompatActivity, Android.Arch.Lifecycle.IObserver
    {
        private PDFViewCtrl mPdfViewCtrl;
        private PDFDoc mPdfDoc;
        private ToolManager mToolManager;
        private ReflowControl mReflowControl;
        private AnnotationToolbar mAnnotationToolbar;

        private ViewGroup mMainView;
        private ThumbnailSlider mSeekBar;

        private CustomRelativeLayout mCustomView;

        private QuickMenu mQuickMenu;

        // for search
        private FindTextOverlay mSearchOverlay;
        private SearchToolbar mSearchToolbar;
        private SearchResultsView mSearchResultsView;


        // for image stamp
        Android.Graphics.PointF mAnnotTargetPoint;
        Android.Net.Uri mOutputFileUri;

        // bookmarks
        private int mBookmarkDialogCurrentTab = 0;

        private readonly int OPEN_FILE_REQUEST = 0;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            initialSetup();
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            if (mAnnotationToolbar != null && mAnnotationToolbar.IsShowing)
            {
                mAnnotationToolbar.OnConfigurationChanged(newConfig);
            }
        }

        public void initialSetup()
        {
            try
            {
                AppUtils.InitializePDFNetApplication(this, pdftron.Demo.Key.LicenseKey);
                Console.WriteLine(PDFNet.GetVersion());
            }
            catch (pdftron.Common.PDFNetException e)
            {
                Console.WriteLine(e.GetMessage());
                return;
            }

            SetContentView(Resource.Layout.Main);
            mMainView = (ViewGroup)FindViewById(Android.Resource.Id.Content);
            mPdfViewCtrl = FindViewById<PDFViewCtrl>(Resource.Id.pdfviewctrl);
            // config PDFViewCtrl through PDFViewCtrlConfig
            // here we use the default configuration
            AppUtils.SetupPDFViewCtrl(mPdfViewCtrl, PDFViewCtrlConfig.GetDefaultConfig(this));
            mPdfViewCtrl.SetPageBox(pdftron.PDF.Page.Box.e_user_crop);

            // Events
            mPdfViewCtrl.RenderingStarted += PdfViewCtrl_RenderingStarted;
            mPdfViewCtrl.RenderingFinished += PdfViewCtrl_RenderingFinished;
            mPdfViewCtrl.DocumentLoad += PdfViewCtrl_DocumentLoad;
            mPdfViewCtrl.DocumentDownloaded += PdfViewCtrl_DocumentDownload;
            mPdfViewCtrl.UniversalDocumentConversion += PdfViewCtrl_UniversalDocumentConversion;
            mPdfViewCtrl.PageNumberChanged += PdfViewCtrl_PageChange;
            //mPdfViewCtrl.TextSearchEnd += PdfViewCtrl_TextSearchEnd;
            mPdfViewCtrl.PagePresentationMode = PDFViewCtrl.PagePresentationModes.Single;

            bool load_from_url = false;
            if (!load_from_url)
            {
                try
                {
                    // Load from resource
                    string tempPath = copyResToLocal(Resource.Raw.sample);
                    string password = "";
                    mPdfDoc = mPdfViewCtrl.OpenPDFUri(Android.Net.Uri.FromFile(new Java.IO.File(tempPath)), password);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    mPdfDoc = null;
                }
            }
            else
            {
                try
                {
                    string cache_file = this.CacheDir.Path + "/pdfref.pdf";
                    mPdfViewCtrl.OpenUrlAsync("http://www.pdftron.com/downloads/pdfref.pdf", "", cache_file, null);
                }
                catch (pdftron.Common.PDFNetException e)
                {
                    Toast t = Toast.MakeText(mPdfViewCtrl.Context, e.StackTrace, ToastLength.Long);
                    t.SetGravity(GravityFlags.Center, 0, 0);
                    t.Show();
                }
            }

            // To customize ToolManager via XML, do:
            mToolManager = ToolManagerBuilder.From(this, Resource.Style.MyToolManager).Build(this, mPdfViewCtrl);

            // To use default ToolManager, do:
            // mToolManager = ToolManagerBuilder.From().Build(mPdfViewCtrl);

            // In addition to XML config, you can also disable tools, i.e. hide it from QuickMenu and AnnotationToolbar via DisableToolMode API
            //mToolManager.DisableToolMode(new ToolManager.ToolMode[] {
            //    ToolManager.ToolMode.TextHighlight,
            //    ToolManager.ToolMode.InkCreate,
            //    ToolManager.ToolMode.RectCreate,
            //    ToolManager.ToolMode.TextCreate
            //});

            // To customize which icons should show on phone portrait collapsed mode, do:
            //mToolManager.SetAnnotToolbarPrecedence(new ToolManager.ToolMode[] {
            //    ToolManager.ToolMode.TextCreate,
            //    ToolManager.ToolMode.InkCreate,
            //    ToolManager.ToolMode.RectCreate,
            //    ToolManager.ToolMode.LineCreate
            //});

            // To customize quick menu programmatically
            //demoCustomizeQuickMenu();

            // Events
            mToolManager.InterceptAnnotationHandling += (sender, e) =>
            {
                Console.WriteLine("ToolManager InterceptAnnotationHandling");

                var annot = TypeConvertHelper.ConvAnnotToManaged(e.Annot);

                try
                {
                    // Intercept clicking link annotation by setting e.Handled to true
                    if (annot.GetType() == Annot.Type.e_Link)
                    {
                        Console.WriteLine("InterceptAnnot handling link annotation");
                    }
                }
                catch
                {
                    // handle exception
                }

                e.Handled = false;

            };
            mToolManager.ToolChanged += (sender, e) =>
            {
                Console.WriteLine("ToolChanged oldMode: " +
                                  (e.OldTool != null ? e.OldTool.ToolMode.Value : -1) + " newMode:" +
                                  (e.NewTool != null ? e.NewTool.ToolMode.Value : -1));
            };
            mToolManager.SingleTapConfirmed += (sender, e) =>
            {
                e.Handled = false;
                Console.WriteLine("SingleTapConfirmed");
            };
            mToolManager.LongPress += (sender, e) =>
            {
                e.Handled = false;
                Console.WriteLine("LongPress");
            };
            mToolManager.ScaleBegin += (sender, e) =>
            {
                e.Handled = false;
                Console.WriteLine("ScaleBegin");
            };
            mToolManager.Scale += (sender, e) =>
            {
                e.Handled = false;
                Console.WriteLine("Scale: " + mPdfViewCtrl.Zoom);
            };
            mToolManager.ScaleEnd += (sender, e) =>
            {
                e.Handled = false;
                Console.WriteLine("ScaleEnd");
            };
            mToolManager.ScrollChanged += (sender, e) =>
            {
                //Console.WriteLine("ScrollChanged left: " + e.Left + " top:" + e.Top);
            };
            mToolManager.AnnotationsAdded += (sender, e) =>
            {
                if (mPdfDoc == null)
                {
                    return;
                }
                Console.WriteLine("AnnotationsAdded changed annotations count: " + e.Annots.Count);

                var extractor = new TextExtractor();
                foreach (var item in e.Annots)
                {
                    var nativeAnnot = item.Key;
                    var pg = (int)item.Value;
                    var page = mPdfDoc.GetPage(pg);

                    if (nativeAnnot.Type == (int)Annot.Type.e_Highlight)
                    {
                        extractor.Begin(page);
                        pdftron.PDF.Annot annot = TypeConvertHelper.ConvAnnotToManaged(nativeAnnot);
                        var content = extractor.GetTextUnderAnnot(annot);
                        Console.WriteLine("AnnotationsAdded content: " + content);
                    }
                }
            };
            mToolManager.AnnotationsModified += (sender, e) =>
            {
                Console.WriteLine("AnnotationsModified changed annotations count: " + e.Annots.Count);
                foreach (var item in e.Annots)
                {
                    var nativeAnnot = item.Key;
                    var annot = TypeConvertHelper.ConvAnnotToManaged(nativeAnnot);
                    if (annot != null && annot.IsValid())
                    {
                        Annot.Type type = annot.GetType();
                        Console.WriteLine("AnnotationsModified: type: " + type);
                    }
                }
            };
            mToolManager.AnnotationsRemoved += (sender, e) =>
            {
                Console.WriteLine("AnnotationsRemoved changed annotations count: " + e.Annots.Count);
            };
            mToolManager.ImageStamperSelected += (sender, e) =>
            {
                mAnnotTargetPoint = e.TargetPoint;
                mOutputFileUri = ViewerUtils.OpenImageIntent(this);
            };
            mToolManager.FileAttachmentSelected += (sender, e) =>
            {
                var filePath = ViewerUtils.ExportFileAttachment(mPdfViewCtrl, e.Attachment);
                OpenNewFile(filePath);
            };
            mToolManager.AttachFileSelected += (sender, e) =>
            {
                mAnnotTargetPoint = e.TargetPoint;
                ViewerUtils.OpenFileIntent(this);
            };
            mToolManager.SetCanOpenEditToolbarFromPan(true);
            mToolManager.OpenEditToolbar += (sender, e) =>
            {
                mAnnotationToolbar.Show(AnnotationToolbar.StartModeEditToolbar, null, 0, e.Mode, !mAnnotationToolbar.IsShowing);
            };

            mSeekBar = FindViewById<ThumbnailSlider>(Resource.Id.thumbseekbar);
            mSeekBar.ThumbSliderStopTrackingTouch += (sender, e) =>
            {
                Console.WriteLine("ThumbSliderStopTrackingTouch page: " + e.PageNum);
                if (mReflowControl != null)
                {
                    if (mReflowControl.Visibility == ViewStates.Visible)
                    {
                        mReflowControl.CurrentPage = e.PageNum;
                    }
                }
            };
            mSeekBar.ThumbSliderStartTrackingTouch += (sender, e) =>
            {
                Console.WriteLine("ThumbSliderStartTrackingTouch");
            };
            mSeekBar.MenuItemClicked += (sender, e) =>
            {
                if (e.MenuItemPosition == ThumbnailSlider.PositionLeft)
                {
                    openThumbnailsDialog();
                }
                else if (e.MenuItemPosition == ThumbnailSlider.PositionRight)
                {
                    openBookmarksDialog(mBookmarkDialogCurrentTab);
                }
            };

            mReflowControl = FindViewById<ReflowControl>(Resource.Id.reflow);

            mAnnotationToolbar = FindViewById<AnnotationToolbar>(Resource.Id.annotationtoolbar);
            mAnnotationToolbar.Setup(mToolManager);
            mAnnotationToolbar.SetButtonStayDown(true);
            mAnnotationToolbar.AnnotationToolbarShown += (sender, e) =>
            {
                Console.WriteLine("AnnotationToolbarShown");
            };
            mAnnotationToolbar.AnnotationToolbarClosed += (sender, e) =>
            {
                Console.WriteLine("AnnotationToolbarClosed");
            };
            mAnnotationToolbar.UndoRedo += (sender, e) =>
            {
                mSeekBar?.RefreshPageCount();
            };

            mCustomView = FindViewById<CustomRelativeLayout>(Resource.Id.customview);

            mSearchToolbar = FindViewById<SearchToolbar>(Resource.Id.searchtoolbar);
            mSearchOverlay = FindViewById<FindTextOverlay>(Resource.Id.find_text_view);
            mSearchOverlay.SetPdfViewCtrl(mPdfViewCtrl);

            mSearchToolbar.ExitSearch += (sender, e) =>
            {
                endSearch();
            };
            mSearchToolbar.ClearSearchQuery += (sender, e) =>
            {
                mSearchOverlay?.CancelFindText();
            };
            mSearchToolbar.SearchQuerySubmit += (sender, e) =>
            {
                mSearchOverlay?.QueryTextSubmit(e.Query);
            };
            mSearchToolbar.SearchQueryChange += (sender, e) =>
            {
                mSearchOverlay?.SetSearchQuery(e.Query);
            };
            mSearchToolbar.SearchOptionsItemSelected += (sender, e) =>
            {
                int id = e.Item.ItemId;
                if (id == Resource.Id.action_match_case)
                {
                    bool isChecked = e.Item.IsChecked;
                    mSearchOverlay?.SetSearchMatchCase(!isChecked);
                    mSearchOverlay?.ResetFullTextResults();
                    e.Item.SetChecked(!isChecked);
                }
                else if (id == Resource.Id.action_whole_word)
                {
                    bool isChecked = e.Item.IsChecked;
                    mSearchOverlay?.SetSearchWholeWord(!isChecked);
                    mSearchOverlay?.ResetFullTextResults();
                    e.Item.SetChecked(!isChecked);
                }
                else if (id == Resource.Id.action_list_all)
                {
                    if (mSearchResultsView == null)
                    {
                        mSearchResultsView = FindViewById<SearchResultsView>(Resource.Id.searchResultsView);
                        mSearchResultsView.SetPdfViewCtrl(mPdfViewCtrl);
                        mSearchResultsView.FullTextSearchStart += (sender2, e2) =>
                        {
                            mSearchToolbar?.SetSearchProgressBarVisible(true);
                        };
                        mSearchResultsView.SearchResultFound += (sender2, e2) =>
                        {
                            mSearchToolbar?.SetSearchProgressBarVisible(false);
                            mSearchOverlay?.HighlightFullTextSearchResult(e2.Result);
                        };
                        mSearchResultsView.SearchResultClicked += (sender2, e2) =>
                        {
                            mSearchOverlay?.HighlightFullTextSearchResult(e2.Result);
                            mPdfViewCtrl?.SetCurrentPage(e2.Result.PageNum);
                            mSearchResultsView.Visibility = ViewStates.Gone;
                        };
                    }
                    if (mSearchResultsView.Visibility == ViewStates.Gone)
                    {
                        mSearchResultsView.Visibility = ViewStates.Visible;
                        mSearchResultsView.FindText(e.Query);
                    }
                    else
                    {
                        mSearchResultsView.Visibility = ViewStates.Gone;
                    }
                }
            };
        }

        private void demoCustomizeQuickMenu()
        {
            // Programmatically change quick menu
            mToolManager.ShowQuickMenu += (sender, e) =>
            {
                if (e.Annot != null && e.Quickmenu != null)
                {
                    var annot = TypeConvertHelper.ConvAnnotToManaged(e.Annot);
                    if (annot.GetType() == Annot.Type.e_Square)
                    {
                        var item = new QuickMenuItem(this, Resource.Id.qm_custom_link, QuickMenuItem.OverflowRowMenu);
                        item.SetTitle(Resource.String.qm_custom_link);
                        var items = new List<QuickMenuItem>(1);
                        items.Add(item);
                        e.Quickmenu.AddMenuEntries(items);
                    }
                    else if (annot.GetType() == Annot.Type.e_Circle)
                    {
                        var item = new QuickMenuItem(this, Resource.Id.qm_custom_unlink, QuickMenuItem.OverflowRowMenu);
                        item.SetTitle(Resource.String.qm_custom_unlink);
                        var items = new List<QuickMenuItem>(1);
                        items.Add(item);
                        e.Quickmenu.AddMenuEntries(items);
                    }
                }
                e.Handled = false;
            };
            mToolManager.QuickMenuClicked += (sender, e) =>
            {
                var which = e.MenuItem.ItemId;
                if (which == Resource.Id.qm_custom_link)
                {
                    Toast.MakeText(this, "Link pressed!", ToastLength.Long).Show();
                }
                else if (which == Resource.Id.qm_custom_unlink)
                {
                    Toast.MakeText(this, "Unlink pressed!", ToastLength.Long).Show();
                }
                e.Handled = false;
            };
        }

        private void demoShowQuickMenu(MotionEvent ev)
        {
            if (mQuickMenu != null && mQuickMenu.IsShowing)
            {
                mQuickMenu.Dismiss();
                return;
            }
            int x = (int)(ev.GetX() + 0.5);
            int y = (int)(ev.GetY() + 0.5);
            var targetPoint = new Android.Graphics.PointF(ev.GetX(), ev.GetY());

            mQuickMenu = new QuickMenu(mPdfViewCtrl);
            mQuickMenu.InitMenuEntries(Resource.Menu.tap);
            mQuickMenu.DismissEvent += (sender, ev2) =>
            {
                var selectedItem = mQuickMenu.SelectedMenuItem;
                if (selectedItem == null)
                {
                    return;
                }
                if (selectedItem.ItemId == Resource.Id.qm_free_text)
                {
                    pdftron.PDF.Tools.FreeTextCreate freeTextTool = (pdftron.PDF.Tools.FreeTextCreate)mToolManager.CreateTool(ToolManager.ToolMode.TextCreate, null);
                    freeTextTool.InitFreeText(targetPoint);
                    mToolManager.Tool = freeTextTool;
                }
                else if (selectedItem.ItemId == Resource.Id.qm_floating_sig)
                {
                    pdftron.PDF.Tools.Signature signatureTool = (pdftron.PDF.Tools.Signature)mToolManager.CreateTool(ToolManager.ToolMode.Signature, null);
                    signatureTool.SetTargetPoint(targetPoint);
                    mToolManager.Tool = signatureTool;
                }
            };
            var anchor = new Android.Graphics.RectF(x - 5, y, x + 5, y + 1);
            mQuickMenu.SetAnchorRect(anchor);
            mQuickMenu.Show();
        }

        private void PdfViewCtrl_UniversalDocumentConversion(object sender, PDFViewCtrl.UniversalDocumentConversionEventArgs e)
        {
            //Console.WriteLine("UniversalDocumentConversion");
        }

        private string copyResToLocal(int resId)
        {
            try
            {
                Java.IO.File tempFile = new Java.IO.File(this.FilesDir, "sample.pdf");
                string result = tempFile.AbsolutePath;
                using (var source = this.Resources.OpenRawResource(resId))
                {
                    using (var fileStream = System.IO.File.Create(result))
                    {
                        try
                        {
                            source.CopyTo(fileStream);
                            return result;
                        }
                        finally
                        {
                            source.Close();
                            fileStream.Close();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        // PDFViewCtrl callbacks
        //private void PdfViewCtrl_TextSearchEnd(object sender, PDFViewCtrl.TextSearchEndEventArgs e)
        //{
        //    Console.WriteLine("mPdfViewCtrl_TextSearchEnded");

        //    if (mToolManager.Tool is pdftron.PDF.Tools.TextHighlighter)
        //    {
        //        ((pdftron.PDF.Tools.TextHighlighter)mToolManager.Tool).Update();
        //    }
        //}

        private void PdfViewCtrl_PageChange(object sender, PDFViewCtrl.PageNumberChangedEventArgs e)
        {
            if (mSeekBar != null)
            {
                mSeekBar.SetProgress(e.CurPage);
            }
        }
        private void PdfViewCtrl_DocumentDownload(object sender, PDFViewCtrl.DocumentDownloadedEventArgs e)
        {
            if (e.State == PDFViewCtrl.DownloadState.Page)
            {
                mPdfDoc = mPdfViewCtrl.GetDoc();
            }
            else if (e.State == PDFViewCtrl.DownloadState.Finished)
            {
                Toast.MakeText(this, Resource.String.demo_msg_download_finished, ToastLength.Long).Show();
                if (mSeekBar != null)
                {
                    mSeekBar.RefreshPageCount();
                }
            }
            else if (e.State == PDFViewCtrl.DownloadState.Failed)
            {
                mPdfViewCtrl.Invalidate();
                string errorMessage = e.Message;
                Toast.MakeText(this, Resource.String.demo_msg_download_failed, ToastLength.Long).Show();
            }
        }

        private void PdfViewCtrl_DocumentLoad(object sender, EventArgs e)
        {

        }

        private void PdfViewCtrl_RenderingFinished(object sender, EventArgs e)
        {

        }

        private void PdfViewCtrl_RenderingStarted(object sender, EventArgs e)
        {

        }

        protected override void OnPause()
        {
            base.OnPause();
            if (mPdfViewCtrl != null)
            {
                mPdfViewCtrl.Pause();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (mPdfViewCtrl != null)
            {
                mPdfViewCtrl.Resume();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            mSeekBar?.ClearResources();
            mSeekBar = null;

            mReflowControl?.CleanUp();
            mReflowControl = null;

            mPdfViewCtrl?.Destroy();
            mPdfViewCtrl = null;

            mPdfDoc?.Close();
            mPdfDoc = null;
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
            if (mPdfViewCtrl != null)
            {
                mPdfViewCtrl.PurgeMemory();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.menu, menu);
            return true;
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            IMenuItem itemGoToPage = menu.FindItem(Resource.Id.gotopage);
            IMenuItem itemOpenFile = menu.FindItem(Resource.Id.open_file);
            IMenuItem itemSave = menu.FindItem(Resource.Id.save_file);

            if (itemGoToPage == null || itemOpenFile == null || itemSave == null)
            {
                return false;
            }

            if (mPdfViewCtrl != null)
            {
                itemOpenFile.SetEnabled(true);
                try
                {
                    if (mPdfDoc != null)
                    {
                        bool modified = false;
                        try
                        {
                            modified = mPdfDoc.IsModified();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("OnPrepareOptionsMenu" + e.Message);
                        }
                        itemSave.SetEnabled(modified);
                        itemGoToPage.SetEnabled(true);
                    }
                    else
                    {
                        itemSave.SetEnabled(false);
                        itemGoToPage.SetEnabled(false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
            else
            {
                itemOpenFile.SetEnabled(false);
            }
            return true;
        }

        private void demoCustomView()
        {
            if (mCustomView != null)
            {
                mCustomView.Visibility = ViewStates.Visible;
            }
        }

        private void demoLinkInfo(pdftron.PDF.Tools.ToolManager.SingleTapConfirmedEventArgs e)
        {
            int x = (int)(e.Event.GetX() + 0.5);
            int y = (int)(e.Event.GetY() + 0.5);

            mPdfViewCtrl.DocLockRead();

            Annot ann = mPdfViewCtrl.GetAnnotationAt(x, y);
            PDFViewCtrl.LinkInfo linkInfo = mPdfViewCtrl.GetLinkAt(x, y);
            bool hasLink = false;
            if (linkInfo != null)
            {
                hasLink = true;
                Console.WriteLine("link: " + linkInfo.URL);
            }
            if (ann != null && ann.IsValid())
            {
                if (ann.GetType() == Annot.Type.e_Link)
                {
                    pdftron.PDF.Annots.Link link = new pdftron.PDF.Annots.Link(ann);
                    pdftron.PDF.Action action = link.GetAction();
                    if (action.GetType() == pdftron.PDF.Action.Type.e_URI)
                    {
                        pdftron.SDF.Obj o = action.GetSDFObj();
                        o = o.FindObj("URI");
                        if (o != null)
                        {
                            hasLink = true;
                            String uri = o.GetAsPDFText();
                            Console.WriteLine("link: " + uri);

                        }
                    }
                }
            }
            if (hasLink)
            {
                e.Handled = true;
            }

            mPdfViewCtrl.DocUnlockRead();
        }

        bool minking = false;

        private void demoMultiStrokeInk()
        {
            if (minking)
            {
                if (mToolManager.Tool is pdftron.PDF.Tools.FreehandCreate)
                {
                    minking = false;
                    ((pdftron.PDF.Tools.FreehandCreate)mToolManager.Tool).CommitAnnotation();
                    mToolManager.Tool = mToolManager.CreateDefaultTool();
                }
            }
            else
            {
                minking = true;
                pdftron.PDF.Tools.FreehandCreate t = (pdftron.PDF.Tools.FreehandCreate)mToolManager.CreateTool(ToolManager.ToolMode.InkCreate, null);
                t.SetMultiStrokeMode(true);
                mToolManager.Tool = t;
            }
        }

        private void demoTextSearch()
        {
            if (mPdfDoc == null)
            {
                return;
            }
            var search = new TextSearch();
            var txtSearch = new TextSearch();

            const int mode = (int)(TextSearch.SearchMode.e_page_stop | TextSearch.SearchMode.e_highlight | TextSearch.SearchMode.e_ambient_string);
            var text = "sample";
            var resultStr = "";
            var ambientString = "";
            var hlts = new Highlights();

            bool unlock = false;
            try
            {
                mPdfViewCtrl.DocLockRead();
                unlock = true;
                txtSearch.Begin(mPdfDoc, text, mode, -1, -1);

                int pageNum = mPdfViewCtrl.CurrentPage;

                while (true)
                {
                    TextSearch.ResultCode code = txtSearch.Run(ref pageNum, ref resultStr, ref ambientString, hlts);

                    if (code == TextSearch.ResultCode.e_found)
                    {
                        //found
                        mPdfViewCtrl.SelectAndJumpWithHighlights(hlts);
                        int selPageBegin = mPdfViewCtrl.SelectionBeginPage;
                        int selPageEnd = mPdfViewCtrl.SelectionEndPage;
                        // get quads
                        for (int page = selPageBegin; page <= selPageEnd; page++)
                        {
                            if (!mPdfViewCtrl.HasSelectionOnPage(page))
                            {
                                continue;
                            }
                            PDFViewCtrl.Selection sel = mPdfViewCtrl.GetSelection(page);
                            double[] quads = sel.GetQuads();
                            QuadPoint[] quadPoints = TypeConvertHelper.ConvQuadDoubleArrayToQuadPointArray(quads);
                            if (quadPoints.Length == 0)
                            {
                                continue;
                            }
                            QuadPoint q1 = quadPoints[0];
                            // do something with quads here
                        }
                    }
                    else if (code == TextSearch.ResultCode.e_page)
                    {
                    }
                    else
                    {
                        break;
                    }
                }
            }
            finally
            {
                if (unlock)
                {
                    mPdfViewCtrl.DocUnlockRead();
                }
            }
        }

        private void demoFindText()
        {
            string searchQuery = "the";
            mPdfViewCtrl.FindText(searchQuery, false, false, false, false);

            pdftron.PDF.Tools.TextHighlighter t = (pdftron.PDF.Tools.TextHighlighter)mToolManager.CreateTool(ToolManager.ToolMode.TextHighlighter, null);
            mToolManager.Tool = t;
            t.Start(searchQuery, false, false, false);

            // To exit
            //if (mToolManager.Tool is pdftron.PDF.Tools.TextHighlighter)
            //{
            //    ((pdftron.PDF.Tools.TextHighlighter)mToolManager.Tool).Clear();
            //    mPdfViewCtrl.ClearSelection();
            //    mPdfViewCtrl.Invalidate();
            //}
            //mToolManager.Tool = mToolManager.CreateDefaultTool();
        }

        private void demoHTML2PDF()
        {
            HTML2PDF hTML2PDF = new HTML2PDF(this);
            hTML2PDF.FromUrl("http://developer.android.com/about/index.html");
            hTML2PDF.ConversionFinished += (sender, e) =>
            {
                // do something with e.PdfOutput
            };
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.open_file:
                    Intent file_open_intent = new Intent().SetClass(this, typeof(FilePickerActivity));
                    StartActivityForResult(file_open_intent, OPEN_FILE_REQUEST);

                    break;
                case Resource.Id.open_url:
                    DialogURL dlg = new DialogURL(this, mPdfViewCtrl);
                    dlg.Show();

                    break;
                case Resource.Id.save_file:
                    bool docLocked = false;
                    if (mPdfDoc != null)
                    {
                        try
                        {
                            // Note: document needs to be locked first before it can be saved. Also,
                            // trying to acquire a write lock (doc.Lock()) while we hold a read lock
                            // might throw an exception, so let's use TryLock() instead.
                            docLocked = mPdfDoc.TryLock();
                            if (docLocked)
                            {
                                if (mPdfDoc.IsModified())
                                {
                                    string file_name = mPdfDoc.GetFileName();
                                    if (file_name.Length == 0)
                                    {
                                        Java.IO.File tmpFile = Java.IO.File.CreateTempFile("tmp", ".pdf", this.FilesDir);
                                        bool exist = tmpFile.Exists();
                                        bool canWrite = tmpFile.CanWrite();
                                        String filename = tmpFile.AbsolutePath;
                                        if (!exist || tmpFile.CanWrite())
                                        {
                                            // Use file name to output file
                                            mPdfDoc.Save(filename, pdftron.SDF.SDFDoc.SaveOptions.e_compatibility);
                                            Toast.MakeText(this, "Saved \"" + filename + "\"", ToastLength.Short).Show();
                                        }
                                        else
                                        {
                                            Toast.MakeText(this, "Failed to save \"" + filename + "\"", ToastLength.Short).Show();
                                        }
                                    }
                                    else
                                    {
                                        Java.IO.File file = new Java.IO.File(file_name);
                                        bool exist = file.Exists();
                                        bool canWrite = file.CanWrite();
                                        if (!exist || file.CanWrite())
                                        {
                                            // Use file name to output file
                                            mPdfDoc.Save(file_name, pdftron.SDF.SDFDoc.SaveOptions.e_compatibility);
                                            Toast.MakeText(this, "Saved \"" + file_name + "\"", ToastLength.Short).Show();
                                        }
                                        else
                                        {
                                            Toast.MakeText(this, "Failed to save \"" + file_name + "\"", ToastLength.Short).Show();
                                        }
                                    }
                                }
                                else
                                    Toast.MakeText(this, Resource.String.demo_msg_save_file_locked, ToastLength.Short).Show();
                            }
                            else
                            {
                                Toast.MakeText(this, Resource.String.demo_msg_save_file_locked, ToastLength.Short).Show();
                            }
                        }
                        catch (pdftron.Common.PDFNetException e)
                        {
                            Console.WriteLine(e.StackTrace);
                        }
                        finally
                        {
                            try
                            {
                                if (docLocked)
                                {
                                    mPdfDoc.Unlock();
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                    }
                    break;
                case Resource.Id.settings:
                    var viewModeDialog = pdftron.PDF.Dialog.ViewModePickerDialogFragment.NewInstance(mPdfViewCtrl.PagePresentationMode,
                        false, mReflowControl.Visibility == ViewStates.Visible, 0);
                    viewModeDialog.SetStyle((int)DialogFragmentStyle.Normal, Resource.Style.CustomAppTheme);
                    viewModeDialog.Show(this.SupportFragmentManager, "view_mode_picker");

                    viewModeDialog.CustomColorModeSelected += (sender, e) =>
                    {
                        PdfViewCtrlSettingsManager.SetCustomColorModeBGColor(this, e.BgColor);
                        PdfViewCtrlSettingsManager.SetCustomColorModeTextColor(this, e.TxtColor);
                        PdfViewCtrlSettingsManager.SetColorMode(this, PdfViewCtrlSettingsManager.KeyPrefColorModeCustom);
                        handleCustomColorMode(e.BgColor, e.TxtColor);
                    };

                    viewModeDialog.ViewModeColorSelected += (sender, e) =>
                    {
                        PdfViewCtrlSettingsManager.SetColorMode(this, e.ColorMode);
                        handleColorMode(e.ColorMode);
                    };

                    viewModeDialog.ViewModeSelected += (sender, e) =>
                    {
                        handleViewModeChange(e.ViewMode);
                    };
                    break;
                case Resource.Id.gotopage:
                    DialogGoToPage dlgGotoPage = new DialogGoToPage(this, mPdfViewCtrl);
                    dlgGotoPage.Show();
                    break;
                case Resource.Id.print:
                    try
                    {
                        if (mPdfDoc != null)
                        {
                            int flag = (int)(Print.PrintContent.DocumentBit | Print.PrintContent.AnnotationBit | Print.PrintContent.SummaryBit);
                            Print.StartPrintJob(this, "PDFViewCtrlDemo", mPdfDoc, flag, false);
                        }
                    }
                    catch (pdftron.Common.PDFNetException e)
                    {
                        Toast.MakeText(this, e.GetMessage(), ToastLength.Short).Show();
                    }
                    break;
                case Resource.Id.annotation_toolbar:
                    endSearch();
                    // demo to hide close/pan/overflow button from annotation toolbar
                    // mAnnotationToolbar.HideButton(AnnotationToolbarButtonId.Overflow);
                    mAnnotationToolbar.Show();
                    break;
                case Resource.Id.form_toolbar:
                    endSearch();
                    mAnnotationToolbar.Show(AnnotationToolbar.StartModeFormToolbar);
                    break;
                case Resource.Id.add_page_dialog:
                    handleAddPages();
                    break;
                case Resource.Id.create_pdf_dialog:
                    handleCreateNewDoc();
                    break;
                case Resource.Id.rotate_dialog:
                    pdftron.PDF.Dialog.RotateDialogFragment.NewInstance()
                        .SetPdfViewCtrl(mPdfViewCtrl)
                        .Show(this.SupportFragmentManager, "rotate_pages_dialog");
                    break;
                case Resource.Id.crop_dialog:
                    showCropDialog();
                    break;
                case Resource.Id.annotation_dialog:
                    {
                        openBookmarksDialog(0);
                    }
                    break;
                case Resource.Id.outline_dialog:
                    {
                        openBookmarksDialog(1);
                    }
                    break;
                case Resource.Id.user_bookmark_dialog:
                    {
                        openBookmarksDialog(2);
                    }
                    break;
                case Resource.Id.action_search:
                    {
                        if (mAnnotationToolbar != null && mAnnotationToolbar.IsShowing)
                        {
                            mAnnotationToolbar.Close();
                        }
                        startSearch();
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        void handleAddPages()
        {
            if (mPdfDoc == null || mPdfViewCtrl == null)
            {
                return;
            }

            double pageWidth = 0;
            double pageHeight = 0;

            bool shouldUnlockRead = false;
            try
            {
                mPdfViewCtrl.DocLockRead();
                shouldUnlockRead = true;
                Page lastPage = mPdfDoc.GetPage(mPdfDoc.GetPageCount());
                if (lastPage == null)
                    return;
                pageWidth = lastPage.GetPageWidth();
                pageHeight = lastPage.GetPageHeight();
            }
            catch (Exception e)
            {
                return;
            }
            finally
            {
                if (shouldUnlockRead)
                {
                    mPdfViewCtrl.DocUnlockRead();
                }
            }

            var addPageDialogFragment = AddPageDialogFragment.NewInstance(pageWidth, pageHeight);
            addPageDialogFragment.AddNewPages += (sender, e) =>
            {
                if (mPdfDoc == null || mPdfViewCtrl == null)
                {
                    return;
                }
                bool shouldUnlock = false;
                try
                {
                    mPdfViewCtrl.DocLock(true);
                    shouldUnlock = true;

                    for (int i = 1, cnt = e.Pages.Length; i <= cnt; i++)
                    {
                        int newPageNum = mPdfViewCtrl.CurrentPage + i;
                        var pageNative = e.Pages[i - 1];
                        var page = TypeConvertHelper.ConvPageToManaged(pageNative);
                        mPdfDoc.PageInsert(mPdfDoc.GetPageIterator(newPageNum), page);
                    }
                }
                catch (PDFNetException ex)
                {
                    Console.WriteLine(ex.GetMessage());
                }
                finally
                {
                    if (shouldUnlock)
                    {
                        mPdfViewCtrl.DocUnlock();
                    }
                }
                mPdfViewCtrl.UpdatePageLayout();
                mPdfViewCtrl.CurrentPage = mPdfViewCtrl.CurrentPage + 1;
                if (mSeekBar != null)
                {
                    mSeekBar.SetProgress(mPdfViewCtrl.CurrentPage);
                }
            };
            addPageDialogFragment.Show(this.SupportFragmentManager, "add_page_dialog");
        }

        void handleCreateNewDoc()
        {
            var addPageDialogFragment = AddPageDialogFragment.NewInstance();
            addPageDialogFragment.CreateNewDocument += (sender, e) =>
            {
                if (e.PdfDoc == null || e.Title == null)
                {
                    return;
                }
                var title = e.Title;
                if (!title.EndsWith(".pdf", true, null))
                {
                    title = title + ".pdf";
                }
                Java.IO.File folder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                Java.IO.File documentFile = new Java.IO.File(folder, title);
                try
                {
                    e.PdfDoc.Save(documentFile.AbsolutePath, (long)pdftron.SDF.SDFDoc.SaveOptions.e_remove_unused, null);
                    Toast.MakeText(this, "New file saved: " + documentFile.AbsolutePath, ToastLength.Short).Show();
                    Console.WriteLine("New file saved: " + documentFile.AbsolutePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    e.PdfDoc.Close();
                }

            };
            addPageDialogFragment.Show(this.SupportFragmentManager, "add_page_dialog");
        }

        void handleViewModeChange(String viewMode)
        {
            Console.WriteLine("viewmode: ViewModeSelected: " + viewMode);
            if (viewMode.Equals(PdfViewCtrlSettingsManager.KeyPrefViewmodeUsercropValue))
            {
                showCropDialog();
            }
            else if (viewMode.Equals(PdfViewCtrlSettingsManager.KeyPrefViewmodeRotationValue))
            {
                mPdfViewCtrl.RotateClockwise();
                mPdfViewCtrl.UpdatePageLayout();
            }
            else if (viewMode.Equals(PdfViewCtrlSettingsManager.KeyPrefReflowmode))
            {
                toggleReflow();
            }
            else
            {
                var mode = PDFViewCtrl.PagePresentationModes.Single;
                if (viewMode.Equals(PdfViewCtrlSettingsManager.KeyPrefViewmodeContinuousValue))
                {
                    mode = PDFViewCtrl.PagePresentationModes.SingleCont;
                }
                else if (viewMode.Equals(PdfViewCtrlSettingsManager.KeyPrefViewmodeSinglepageValue))
                {
                    mode = PDFViewCtrl.PagePresentationModes.Single;
                }
                else if (viewMode.Equals(PdfViewCtrlSettingsManager.KeyPrefViewmodeFacingValue))
                {
                    mode = PDFViewCtrl.PagePresentationModes.Facing;
                }
                else if (viewMode.Equals(PdfViewCtrlSettingsManager.KeyPrefViewmodeFacingcoverValue))
                {
                    mode = PDFViewCtrl.PagePresentationModes.FacingCover;
                }
                else if (viewMode.Equals(PdfViewCtrlSettingsManager.KeyPrefViewmodeFacingContValue))
                {
                    mode = PDFViewCtrl.PagePresentationModes.FacingCont;
                }
                else if (viewMode.Equals(PdfViewCtrlSettingsManager.KeyPrefViewmodeFacingcoverContValue))
                {
                    mode = PDFViewCtrl.PagePresentationModes.FacingCoverCont;
                }
                if (mReflowControl.Visibility == ViewStates.Visible)
                {
                    toggleReflow();
                }
                mPdfViewCtrl.PagePresentationMode = mode;
            }
        }

        void handleCustomColorMode(int bgColor, int txtColor)
        {
            int clientColor = PDFViewCtrl.DefaultBgColor;
            if (Utils.IsColorDark(bgColor))
            {
                clientColor = PDFViewCtrl.DefaultDarkBgColor;
            }
            mPdfViewCtrl.SetColorPostProcessMode(pdftron.PDF.PDFRasterizer.ColorPostProcessMode.e_postprocess_gradient_map);
            mPdfViewCtrl.SetColorPostProcessColors(bgColor, txtColor);
            mPdfViewCtrl.SetClientBackgroundColor(Android.Graphics.Color.GetRedComponent(clientColor),
                Android.Graphics.Color.GetGreenComponent(clientColor),
                Android.Graphics.Color.GetBlueComponent(clientColor),
                false);
            mPdfViewCtrl.Update(true);
        }

        void handleColorMode(int colorMode)
        {
            int clientColor = PDFViewCtrl.DefaultBgColor;
            var mode = pdftron.PDF.PDFRasterizer.ColorPostProcessMode.e_postprocess_none;
            if (colorMode == PdfViewCtrlSettingsManager.KeyPrefColorModeSepia)
            {
                mode = pdftron.PDF.PDFRasterizer.ColorPostProcessMode.e_postprocess_gradient_map;
                Stream source = this.Resources.OpenRawResource(Resource.Raw.sepia_mode_filter);
                String path = Path.Combine(Android.App.Application.Context.CacheDir.AbsolutePath, "sepia_mode_filter");
                using (var fileStream = System.IO.File.Create(path))
                {
                    source.CopyTo(fileStream);
                }
                mPdfViewCtrl.SetColorPostProcessMapFile(new pdftron.Filters.MappedFile(path));
            }
            else if (colorMode == PdfViewCtrlSettingsManager.KeyPrefColorModeNight)
            {
                mode = pdftron.PDF.PDFRasterizer.ColorPostProcessMode.e_postprocess_night_mode;
                clientColor = PDFViewCtrl.DefaultDarkBgColor;
            }
            mPdfViewCtrl.SetColorPostProcessMode(mode);
            mPdfViewCtrl.SetClientBackgroundColor(Android.Graphics.Color.GetRedComponent(clientColor),
                Android.Graphics.Color.GetGreenComponent(clientColor),
                Android.Graphics.Color.GetBlueComponent(clientColor),
                false);
            mPdfViewCtrl.Update(true);
        }

        void showCropDialog()
        {
            var userCropDialogFragment = UserCropDialogFragment.NewInstance()
                        .SetPdfViewCtrl(mPdfViewCtrl);
            userCropDialogFragment.SetStyle((int)DialogFragmentStyle.NoTitle, Resource.Style.CustomAppTheme);
            userCropDialogFragment.Show(this.SupportFragmentManager, "user_crop_pages_dialog");
        }

        void toggleReflow()
        {
            if (mReflowControl != null)
            {
                if (mReflowControl.Visibility != ViewStates.Visible)
                {
                    if (mPdfDoc != null)
                    {
                        mPdfViewCtrl.Visibility = ViewStates.Gone;
                        mReflowControl.Visibility = ViewStates.Visible;
                        mReflowControl.Setup(mPdfDoc);
                        mReflowControl.CurrentPage = mPdfViewCtrl.CurrentPage;
                    }
                }
                else
                {
                    mPdfViewCtrl.Visibility = ViewStates.Visible;
                    mPdfViewCtrl.SetCurrentPage(mReflowControl.CurrentPage);
                    mReflowControl.CleanUp();
                    mReflowControl.Visibility = ViewStates.Gone;
                }
            }
        }

        void demoCustomTool()
        {
            mToolManager.Tool = new CustomTool(mPdfViewCtrl);
        }

        private void startSearch()
        {
            if (mSearchToolbar != null)
            {
                mSearchToolbar.Visibility = ViewStates.Visible;
            }
            if (mSearchOverlay != null)
            {
                mSearchOverlay.Visibility = ViewStates.Visible;
            }
        }

        private void endSearch()
        {
            if (mSearchToolbar != null)
            {
                mSearchToolbar.Visibility = ViewStates.Gone;
            }
            if (mSearchOverlay != null)
            {
                mSearchOverlay.Visibility = ViewStates.Gone;
                mSearchOverlay.ExitSearchMode();
            }
            if (mSearchResultsView != null)
            {
                mSearchResultsView.Visibility = ViewStates.Gone;
            }
        }

        private Drawable GetDrawable(int res)
        {
            return pdftron.PDF.Tools.Utils.Utils.GetDrawable(this, res);
        }

        private void openBookmarksDialog(int which)
        {
            var bookmarksDialog = pdftron.PDF.Dialog.BookmarksDialogFragment.NewInstance();
            bookmarksDialog.SetPdfViewCtrl(mPdfViewCtrl);
            List<DialogFragmentTab> tabs = new List<DialogFragmentTab>();
            var annotationsTab = new DialogFragmentTab(
                Java.Lang.Class.FromType(typeof(AnnotationDialogFragment)), BookmarksTabLayout.TagTabAnnotation, GetDrawable(Resource.Drawable.ic_annotations_white_24dp), null, "Annotations", null, Resource.Menu.fragment_annotlist_sort);
            var outlineDialog = new DialogFragmentTab(
                Java.Lang.Class.FromType(typeof(OutlineDialogFragment)), BookmarksTabLayout.TagTabOutline, GetDrawable(Resource.Drawable.ic_outline_white_24dp), null, "Outline", null);
            var userBookmarksDialog = new DialogFragmentTab(
                Java.Lang.Class.FromType(typeof(UserBookmarkDialogFragment)), BookmarksTabLayout.TagTabBookmark, GetDrawable(Resource.Drawable.ic_bookmarks_white_24dp), null, "Bookmarks", null);
            tabs.Add(annotationsTab);
            tabs.Add(outlineDialog);
            tabs.Add(userBookmarksDialog);
            bookmarksDialog.SetDialogFragmentTabs(tabs, which);
            bookmarksDialog.SetStyle((int)DialogFragmentStyle.NoTitle, Resource.Style.CustomAppTheme);
            bookmarksDialog.Show(this.SupportFragmentManager, "bookmarks_dialog");
            bookmarksDialog.AnnotationClicked += (sender, e) =>
            {
                bookmarksDialog.Dismiss();
            };
            bookmarksDialog.ExportAnnotations += (sender, e) =>
            {
                handleExportAnnotations();
                bookmarksDialog.Dismiss();
            };
            bookmarksDialog.OutlineClicked += (sender, e) =>
            {
                bookmarksDialog.Dismiss();
            };
            bookmarksDialog.UserBookmarkClick += (sender, e) =>
            {
                mPdfViewCtrl.SetCurrentPage(e.PageNum);
                bookmarksDialog.Dismiss();
            };
            bookmarksDialog.BookmarksDialogWillDismiss += (sender, e) =>
            {
                bookmarksDialog.Dismiss();
            };
            bookmarksDialog.BookmarksDialogDismissed += (sender, e) =>
            {
                mBookmarkDialogCurrentTab = e.TabIndex;
            };
        }

        private void openThumbnailsDialog()
        {
            var thumbDialog = ThumbnailsViewFragment.NewInstance().SetPdfViewCtrl(mPdfViewCtrl);
            thumbDialog.ExportThumbnails += (sender, e) =>
            {
                Console.WriteLine("ExportThumbnails");
            };
            thumbDialog.ThumbnailsViewDialogDismiss += (sender, e) =>
            {
                Console.WriteLine("ThumbnailsViewDialogDismiss");
            };
            thumbDialog.SetStyle((int)DialogFragmentStyle.NoTitle, Resource.Style.CustomAppTheme);
            thumbDialog.Show(this.SupportFragmentManager, "thumbnails_dialog");
        }

        private void handleExportAnnotations()
        {
            if (mPdfDoc == null)
            {
                return;
            }
            bool shouldUnlockWrite = false;
            PDFDoc outputDoc = null;

            try
            {
                Java.IO.File folder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                String extension = "-annotations";
                Java.IO.File currFile = new Java.IO.File(mPdfDoc.GetFileName());
                string file_name = "test";
                if (currFile.Name.Length > 0 && currFile.Name.Contains("."))
                {
                    file_name = currFile.Name.Substring(0, currFile.Name.LastIndexOf('.'));
                }
                Java.IO.File tempFile = new Java.IO.File(folder, file_name + extension + ".pdf");
                String outputPath = tempFile.AbsolutePath;
                Java.IO.File outputFile = new Java.IO.File(outputPath);

                outputDoc = Print.ExportAnnotations(mPdfDoc, false);
                if (outputDoc != null)
                {
                    outputDoc.Lock();
                    shouldUnlockWrite = true;
                    outputDoc.Save(outputFile.AbsolutePath, pdftron.SDF.SDFDoc.SaveOptions.e_remove_unused);
                    outputDoc.Unlock();
                    shouldUnlockWrite = false;
                    outputDoc.Close();
                    Toast.MakeText(this, "Exported at: " + outputPath, ToastLength.Long).Show();
                }
            }
            catch (PDFNetException ex)
            {
                Console.WriteLine(ex.GetMessage());
            }
            finally
            {
                if (outputDoc != null && shouldUnlockWrite)
                {
                    try
                    {
                        outputDoc.Unlock();
                        outputDoc.Close();
                    }
                    catch (PDFNetException ePDFNet)
                    {
                        Console.WriteLine(ePDFNet.GetMessage());
                    }
                }
            }
        }

        private void demoAnnotStylePicker()
        {
            // 1. Instantiate an AnnotStyle with its constructor
            var annotStyle = new pdftron.PDF.Model.AnnotStyle();
            // 2. Set annotation type to annot style
            annotStyle.AnnotType = (int)Annot.Type.e_Square;
            // 3. Set blue stroke, yellow fill color, thickness 5, opacity 0.8 to the annotation style.
            annotStyle.SetStyle(Android.Graphics.Color.Blue, Android.Graphics.Color.Yellow, 5, 0.8f);
            // 4. Instantiate an AnnotStyleDialogFragment.Builder with its constructor
            var styleDialogBuilder = new AnnotStyleDialogFragment.Builder(annotStyle);
            // 5. Set anchor rectangle if you want the `AnnotStyleDialogFragment` be displayed as a popup window in tablet mode
            styleDialogBuilder.SetAnchor(new Android.Graphics.Rect(0, 0, 100, 1000));
            // 6. Build AnnotStyleDialogFragment with the arguments supplied to this builder.
            var annotStyleDialog = styleDialogBuilder.Build();
            // To show
            annotStyleDialog.Show(this.SupportFragmentManager);
            // To dismiss programmatically
            // annotStyleDialog.Dismiss();

            // Events:
            annotStyleDialog.ChangeAnnotFillColor += (sender, e) =>
            {

            };
            annotStyleDialog.ChangeAnnotFont += (sender, e) =>
            {

            };
            annotStyleDialog.ChangeAnnotIcon += (sender, e) =>
            {

            };
            annotStyleDialog.ChangeAnnotOpacity += (sender, e) =>
            {

            };
            annotStyleDialog.ChangeAnnotStrokeColor += (sender, e) =>
            {

            };
            annotStyleDialog.ChangeAnnotTextColor += (sender, e) =>
            {

            };
            annotStyleDialog.ChangeAnnotTextSize += (sender, e) =>
            {

            };
            annotStyleDialog.ChangeAnnotThickness += (sender, e) =>
            {

            };
        }

        public void DemoDiffActivity()
        {
            DiffActivity.Open(this);
        }

        public void DemoDiffOptions(Context context, Android.Support.V4.App.FragmentManager fragmentManager, List<Android.Net.Uri> files)
        {
            var fragment = pdftron.PDF.Dialog.Diffing.DiffOptionsDialogFragment.NewInstance(
                files[0], files[1]
            );
            fragment.SetStyle((int)DialogFragmentStyle.Normal, Resource.Style.CustomAppTheme);
            fragment.DiffOptionsConfirmed += (sender, e) =>
            {
                CompareFiles(context, files, e.Color1, e.Color2, e.BlendMode);
            };
            fragment.Show(fragmentManager, "diff_options_dialog");
        }

        private void CompareFiles(Context context, List<Android.Net.Uri> files, int color1, int color2, int blendMode)
        {
            Android.Net.Uri uri = pdftron.PDF.Dialog.Diffing.DiffUtils.CompareFilesImpl(context, files, color1, color2, blendMode);
            OpenNewFile(uri);
        }

        public void DemoWatermakDialog(PDFViewCtrl pdfViewCtrl, Android.Support.V4.App.FragmentManager fragmentManager)
        {
            var fragment = pdftron.PDF.Dialog.Watermark.WatermarkDialog.NewInstance(pdfViewCtrl);
            fragment.Show(fragmentManager);
        }

        public void DemoPageLabelSettingsDialog(Android.Support.V4.App.FragmentManager fragmentManager, int fromPage, int toPage, int pageCount)
        {
            var dialog = pdftron.PDF.Dialog.PageLabel.PageLabelDialog.NewInstance(fromPage, toPage, pageCount);
            dialog.SetStyle((int)DialogFragmentStyle.NoTitle, Resource.Style.CustomAppTheme);
            dialog.Show(fragmentManager, "page_label_dialog");
            PageLabelObserveOnComplete(this, mPdfViewCtrl);
        }

        public void PageLabelObserveOnComplete(Android.Support.V4.App.FragmentActivity activity, PDFViewCtrl pdfViewCtrl)
        {
            var viewModel = ViewModelProviders.Of(activity).Get(Java.Lang.Class.FromType(typeof(pdftron.PDF.Dialog.PageLabel.PageLabelSettingViewModel))) as pdftron.PDF.Dialog.PageLabel.PageLabelSettingViewModel;
            viewModel.ObserveOnComplete(activity, this);
        }
        
        public void OnChanged(Java.Lang.Object p0)
        {
            if (p0 is pdftron.PDF.Tools.Utils.Event)
            {
                var someEvent = p0 as pdftron.PDF.Tools.Utils.Event;
                if (!someEvent.HasBeenHandled)
                {
                    var actualEvent = someEvent.ContentIfNotHandled;
                    if (actualEvent is pdftron.PDF.Dialog.PageLabel.PageLabelSetting)
                    {
                        var pageLabelSettingEvent = actualEvent as pdftron.PDF.Dialog.PageLabel.PageLabelSetting;
                        pdftron.PDF.Dialog.PageLabel.PageLabelUtils.SetPageLabel(mPdfViewCtrl, pageLabelSettingEvent);
                    }
                }
            }
        }

        public void DemoOCGLayer(Context context, PDFViewCtrl pdfViewCtrl)
        {
            var pdfLayerDialog = new pdftron.PDF.Dialog.PdfLayer.PdfLayerDialog(
                context, pdfViewCtrl);
            pdfLayerDialog.Show();
        }

        public string CreateExternalFile(string fileName)
        {
            return new Java.IO.File(Android.App.Application.Context.GetExternalFilesDir(null), fileName).AbsolutePath;
        }

        private void OpenNewFile(String filepath)
        {
            try
            {
                var fileUri = Android.Net.Uri.FromFile(new Java.IO.File(filepath));
                OpenNewFile(fileUri);
            }
            catch
            {
                safeCloseDoc();
            }
        }

        private void OpenNewFile(Android.Net.Uri fileUri)
        {
            try
            {
                safeCloseDoc();

                string extension = Utils.GetUriExtension(this.ContentResolver, fileUri);
                if (extension.Contains("pdf"))
                {
                    mPdfDoc = mPdfViewCtrl.OpenPDFUri(fileUri, "");
                }
                else
                {
                    mPdfViewCtrl.OpenNonPDFUri(fileUri, null);
                }
            }
            catch
            {
                safeCloseDoc();
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
            {
                if (requestCode == OPEN_FILE_REQUEST)
                {
                    string str = data.GetStringExtra("PDFViewCtrl.FileOpenData");
                    if (str.Length > 0)
                    {
                        OpenNewFile(str);
                    }
                }
                else if (requestCode == RequestCode.PickPhotoCam)
                {
                    ViewerUtils.CreateImageStamp(this, data, mPdfViewCtrl, mOutputFileUri, mAnnotTargetPoint);
                }
                else if (requestCode == RequestCode.SelectFile)
                {
                    ViewerUtils.CreateFileAttachment(this, data, mPdfViewCtrl, mAnnotTargetPoint);
                }
            }
            else
            {
                if (mToolManager != null && mToolManager.Tool != null)
                {
                    ((Tool)mToolManager.Tool).ClearTargetPoint();
                }
            }
        }

        private void safeCloseDoc()
        {
            mPdfViewCtrl?.CloseDoc();
            mPdfDoc?.Close();
            mPdfDoc = null;
        }

        private class DialogURL : AlertDialog
        {
            private EditText mTextBox;
            private PDFViewCtrl mCtrl;

            public DialogURL(Context context, PDFViewCtrl ctrl)
                : base(context)
            {
                SetTitle(Resource.String.demo_menu_openurl);
                SetIcon(0);
                mCtrl = ctrl;
                mTextBox = new EditText(context);
                mTextBox.SetHint(Resource.String.demo_openurl_enter_url);
                ViewGroup.LayoutParams layout = new ViewGroup.LayoutParams(-1, -1);
                mTextBox.LayoutParameters = layout;
                SetView(mTextBox, 8, 8, 8, 8);

                SetButton("OK", HandlePosButtonOnClick);
                SetButton2("Cancel", HandleNegButtonOnClick);
            }

            public override void Show()
            {
                base.Show();
                mTextBox.Text = "https://pdftron.s3.amazonaws.com/downloads/pdfref.pdf";
            }

            void HandlePosButtonOnClick(object sender, DialogClickEventArgs args)
            {
                string url = mTextBox.Text;
                try
                {
                    if (url.Length > 0)
                    {
                        string fn = url.Substring(url.LastIndexOf('/'), url.Length - url.LastIndexOf('/'));
                        string cache_file = mCtrl.Context.CacheDir.Path + fn;
                        mCtrl.OpenUrlAsync(url, "", cache_file, null);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }

            void HandleNegButtonOnClick(object sender, DialogClickEventArgs args)
            {

            }
        }

        private class DialogGoToPage
        {
            private EditText mTextBox;
            private PDFViewCtrl mCtrl;
            private Context mContext;
            private String mHint;
            private int mPageCount;

            public DialogGoToPage(Context context, PDFViewCtrl ctrl)
            {
                mCtrl = ctrl;
                mContext = context;
                mPageCount = 0;
                mHint = "";
                bool shouldUnlock = false;
                try
                {
                    mCtrl.DocLockRead();
                    shouldUnlock = true;
                    PDFDoc doc = mCtrl.GetDoc();
                    if (doc != null)
                    {
                        mPageCount = doc.GetPageCount();
                        if (mPageCount > 0)
                        {
                            mHint = "Enter page number (1 - " + mPageCount + ")";
                        }
                    }
                }
                catch
                {
                    mPageCount = 0;
                }
                finally
                {
                    if (shouldUnlock)
                    {
                        mCtrl.DocUnlockRead();
                    }
                }
            }

            public void Show()
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(mContext);
                builder.SetTitle(Resource.String.demo_menu_gotopage);
                mTextBox = new EditText(mContext);
                mTextBox.InputType = Android.Text.InputTypes.ClassNumber;
                if (mPageCount > 0)
                {
                    mTextBox.Hint = mHint;
                }
                ViewGroup.LayoutParams layoutParams = new ViewGroup.LayoutParams(-1, -1);
                mTextBox.LayoutParameters = layoutParams;
                FrameLayout layout = new FrameLayout(mCtrl.Context);
                layout.AddView(mTextBox);
                layout.SetPadding(12, 0, 12, 8);
                builder.SetView(layout);

                builder.SetPositiveButton("Ok", HandlePosButtonOnClick);
                builder.SetNegativeButton("Cancel", HandleNegButtonOnClick);

                AlertDialog dialog = builder.Create();
                dialog.Show();
                dialog.GetButton((int)DialogButtonType.Positive).Click += delegate
                {
                    int pageNum = 0;
                    try
                    {
                        pageNum = int.Parse(mTextBox.Text.ToString());
                    }
                    catch
                    {
                        pageNum = 0;
                    }
                    if (pageNum > 0 && pageNum <= mPageCount)
                    {
                        mCtrl.SetCurrentPage(pageNum);
                        dialog.Dismiss();
                    }
                    else
                    {
                        mTextBox.Text = "";
                    }
                };
            }

            void HandlePosButtonOnClick(object sender, DialogClickEventArgs args)
            {

            }

            void HandleNegButtonOnClick(object sender, DialogClickEventArgs args)
            {

            }
        }
    }

}