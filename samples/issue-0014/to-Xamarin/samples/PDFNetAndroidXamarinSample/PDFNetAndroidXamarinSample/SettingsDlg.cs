//
// Copyright (c) 2001-2019 by PDFTron Systems Inc. All Rights Reserved.
//

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

using pdftron.PDF;

namespace PDFNetAndroidXamarinSample
{
    class SettingsDlg : AlertDialog
    {
        private Context mContext;

        private CheckBox mProgressive;
        private RadioGroup mColorModeGrp;
        private RadioButton mDayMode;
        private RadioButton mNightMode;
        private RadioButton mSepiaMode;

        private RadioGroup mPageViewGrp;
        private RadioButton mPageViewFitPage;
        private RadioButton mPageViewFitWidth;
        private RadioButton mPageViewFitHeight;

        private RadioGroup mPagePresGrp;
        private RadioButton mPagePresSingle;
        private RadioButton mPagePresSingleCont;
        private RadioButton mPagePresFacing;
        private RadioButton mPagePresFacingCont;
        private RadioButton mPagePresFacingCover;
        private RadioButton mPagePresFacingCoverCont;

        public SettingsDlg(Context context, PDFViewCtrl ctrl)
            : base(context)
        {
            mContext = context;

            SetTitle(Resource.String.demo_menu_settings);
            SetIcon(0);

            mProgressive = new CheckBox(mContext);
            mProgressive.Text = context.GetString(Resource.String.demo_settings_progressive_rendering);
            mProgressive.Checked = ctrl.ProgressiveRendering;

            // color mode group
            mColorModeGrp = new RadioGroup(mContext);

            mDayMode = new RadioButton(mContext);
            mDayMode.Text = context.GetString(Resource.String.demo_settings_day_mode);
            mDayMode.Id = 0;

            mNightMode = new RadioButton(mContext);
            mNightMode.Text = context.GetString(Resource.String.demo_settings_night_mode);
            mNightMode.Id = 1;

            mSepiaMode = new RadioButton(mContext);
            mSepiaMode.Text = context.GetString(Resource.String.demo_settings_sepia_mode);
            mSepiaMode.Id = 2;

            mColorModeGrp.AddView(mDayMode);
            mColorModeGrp.AddView(mNightMode);
            mColorModeGrp.AddView(mSepiaMode);

            if (ctrl.GetColorPostProcessMode() == PDFRasterizer.ColorPostProcessMode.e_postprocess_night_mode)
            {
                mColorModeGrp.Check(1);
            }
            else if (ctrl.GetColorPostProcessMode() == PDFRasterizer.ColorPostProcessMode.e_postprocess_gradient_map)
            {
                mColorModeGrp.Check(2);
            }
            else
            {
                mColorModeGrp.Check(0);
            }


            // page view group
            mPageViewGrp = new RadioGroup(mContext);

            mPageViewFitPage = new RadioButton(mContext);
            mPageViewFitPage.Text = context.GetString(Resource.String.demo_settings_fit_page);
            mPageViewFitPage.Id = PDFViewCtrl.PageViewModes.FitPage.Value;

            mPageViewFitWidth = new RadioButton(mContext);
            mPageViewFitWidth.Text = context.GetString(Resource.String.demo_settings_fit_width);
            mPageViewFitWidth.Id = PDFViewCtrl.PageViewModes.FitWidth.Value;

            mPageViewFitHeight = new RadioButton(mContext);
            mPageViewFitHeight.Text = context.GetString(Resource.String.demo_settings_fit_height);
            mPageViewFitHeight.Id = PDFViewCtrl.PageViewModes.FitHeight.Value;

            mPageViewGrp.AddView(mPageViewFitPage);
            mPageViewGrp.AddView(mPageViewFitWidth);
            mPageViewGrp.AddView(mPageViewFitHeight);


            mPageViewGrp.Check(ctrl.PageViewMode.Value);

            // page presentation group
            mPagePresGrp = new RadioGroup(mContext);
            mPagePresGrp.SetPadding(5, 0, 0, 0);

            mPagePresSingle = new RadioButton(mContext);
            mPagePresSingle.Text = context.GetString(Resource.String.demo_settings_single);
            mPagePresSingle.Id = PDFViewCtrl.PagePresentationModes.Single.Value;

            mPagePresSingleCont = new RadioButton(mContext);
            mPagePresSingleCont.Text = context.GetString(Resource.String.demo_settings_single_continuous);
            mPagePresSingleCont.Id = PDFViewCtrl.PagePresentationModes.SingleCont.Value;

            mPagePresFacing = new RadioButton(mContext);
            mPagePresFacing.Text = context.GetString(Resource.String.demo_settings_facing);
            mPagePresFacing.Id = PDFViewCtrl.PagePresentationModes.Facing.Value;

            mPagePresFacingCont = new RadioButton(mContext);
            mPagePresFacingCont.Text = context.GetString(Resource.String.demo_settings_facing_continuous);
            mPagePresFacingCont.Id = PDFViewCtrl.PagePresentationModes.FacingCont.Value;

            mPagePresFacingCover = new RadioButton(mContext);
            mPagePresFacingCover.Text = context.GetString(Resource.String.demo_settings_cover);
            mPagePresFacingCover.Id = PDFViewCtrl.PagePresentationModes.FacingCover.Value;

            mPagePresFacingCoverCont = new RadioButton(mContext);
            mPagePresFacingCoverCont.Text = context.GetString(Resource.String.demo_settings_cover_continuous);
            mPagePresFacingCoverCont.Id = PDFViewCtrl.PagePresentationModes.FacingCoverCont.Value;

            mPagePresGrp.AddView(mPagePresSingle);
            mPagePresGrp.AddView(mPagePresSingleCont);
            mPagePresGrp.AddView(mPagePresFacing);
            mPagePresGrp.AddView(mPagePresFacingCont);
            mPagePresGrp.AddView(mPagePresFacingCover);
            mPagePresGrp.AddView(mPagePresFacingCoverCont);

            mPagePresGrp.Check(ctrl.PagePresentationMode.Value);

            LinearLayout view_pres_layout = new LinearLayout(mContext);
            view_pres_layout.Orientation = Orientation.Horizontal;
            view_pres_layout.AddView(mColorModeGrp);
            view_pres_layout.AddView(mPageViewGrp);
            view_pres_layout.AddView(mPagePresGrp);

            LinearLayout main_layout = new LinearLayout(mContext);
            main_layout.SetPadding(5, 0, 0, 0);
            main_layout.Orientation = Orientation.Vertical;
            main_layout.AddView(mProgressive);
            main_layout.AddView(view_pres_layout);

            SetView(main_layout);
        }

        public bool GetProgressive()
        {
            return mProgressive.Checked;
        }
        public int GetPagePresentationMode()
        {
            return mPagePresGrp.CheckedRadioButtonId;
        }

        public int GetPageViewMode()
        {
            return mPageViewGrp.CheckedRadioButtonId;
        }

        public int GetColorMode()
        {
            return mColorModeGrp.CheckedRadioButtonId;
        }
    }
}