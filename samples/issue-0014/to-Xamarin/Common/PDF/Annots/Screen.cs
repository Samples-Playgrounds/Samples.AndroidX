using System;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Annot = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Action = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> A screen annotation (PDF 1.5) specifies a region of a page upon which 
    /// media clips may be played. It also serves as an object from which 
    /// actions can be triggered. 12.6.4.13, “Rendition Actions” discusses 
    /// the relationship between screen annotations and rendition actions.
    /// </summary>
    public class Screen : Annot
    {
        internal Screen(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Screen annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public Screen(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }	
		/// <summary> Creates a Screen annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public Screen(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Releases all resources used by the Screen </summary>
		~Screen() 
        {
            Dispose(false);
        }

		/// <summary> Creates a new Screen annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank Screen annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Screen Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Screen(result, doc);
        }
		/// <summary> Gets the title of the Screen Annotation.
		/// 
		/// </summary>
		/// <returns> A string representing the title of the Screen Annotation
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetTitle()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetTitle(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets the title of the Screen Annotation.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="title">A string representing the title of the Screen Annotation
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetTitle(string title)
        {
            UString str = new UString(title);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetTitle(mp_annot, str.mp_impl));
        }

		/// <summary> Sets the action of the Screen annotation
		/// (Optional; PDF 1.1 )
		/// 
		/// </summary>		
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The action is an action that shall be performed when the annotation is activated </remarks>
        /// <returns>action of the screen annotation</returns>
        public Action GetAction()
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetAction(mp_annot, ref result));
            return result != IntPtr.Zero ? new Action(result, GetRefHandleInternal()) : null;
        }

		/// <summary> Sets the action of the Screen annotation
		/// (Optional; PDF 1.1 )
		/// 
		/// </summary>
		/// <param name="action">An action object representing the action of the Screen annotation.
		/// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The action is an action that shall be performed when the annotation is activated </remarks>
        public void SetAction(Action action)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetAction(mp_annot, action.mp_action));
        }

		/// <summary> Gets the number indicating border color space of the Screen annotation.
		/// 
		/// </summary>
		/// <returns> An integer indicating a color space value from the ColorSpace.Type enum. That is, 1 corresponding to "e_device_gray",
		/// 3 corresponding to "e_device_rgb", and 4 corresponding to "e_device_cmyk" if color space is applicable,
		/// orelse 0 corresponding to "e_null" if the color is transparent.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  This property is part of the Screen appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public int GetBorderColorCompNum()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetBorderColorCompNum(mp_annot, ref result));
            return result;
        }

		/// <summary> Gets the border color of the Screen annotation.
		/// 
		/// </summary>
		/// <returns> A ColorPt object that denotes the color of the Screen border.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  that the color can be in different color spaces: Gray, RGB, or CMYK. Call "GetBorderColorCompNum" 
		/// to access the color space information corresponding to the border color.		
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public ColorPt GetBorderColor()
        {
            ColorPt result = new ColorPt();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetBorderColor(mp_annot, result.mp_colorpt));
            return result;
        }

		/// <summary> Sets the border color of the Screen.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="c">A ColorPt object that denotes the color of the wdget border.
		/// </param>
		/// <param name="CompNum">An int whose value implies the color space used for the parameter c.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  it is necessary to make sure the consistancy between the ColorPt type
		/// and the ColorSpace.Type value. e_device_gray corresponds to an array of two numbers;
		/// e_device_rgb corresponds to an array of 3 numbers, e_device_cmyk corresponds to an array of
		/// 4 numnbers, while e_null correspons to an arry of 0 number. Entries out of the specified
		/// color space array length will be desgarded. However, missing entries for a specified color space
		/// will throw exception either when setting the color or when later retrieving color(colorspace)
		/// information.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public void SetBorderColor(ColorPt c, int CompNum)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetBorderColor(mp_annot, c.mp_colorpt, CompNum));
        }					

		/// <summary> Gets the number indicating background color space of the Screen annotation.
		/// 
		/// </summary>
		/// <returns> An integer indicating a color space value from the ColorSpace.Type enum. That is,
		/// 1 corresponding to "e_device_gray",
		/// 3 corresponding to "e_device_rgb", and 4 corresponding to "e_device_cmyk" if color space is applicable,
		/// orelse 0 corresponding to "e_null" if the color is transparent.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  This property is part of the Screen appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public int GetBackgroundColorCompNum()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetBackgroundColorCompNum(mp_annot, ref result));
            return result;
        }

		/// <summary> Gets the background color of the Screen annotation.
		/// 
		/// </summary>
		/// <returns> A ColorPt object that denotes the color of the Screen background.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  that the color can be in different color spaces: Gray, RGB, or CMYK. Call "GetBackgroundColorCompNum"
		/// to access the color space information corresponding to the border color.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public ColorPt GetBackgroundColor()
        {
            ColorPt result = new ColorPt();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetBackgroundColor(mp_annot, result.mp_colorpt));
            return result;
        }

		/// <summary> Sets the background color of the Screen.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="c">A ColorPt object that denotes the color of the wdget background.
		/// </param>
		/// <param name="CompNum">An int whose value implies the color space used for the parameter c.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  it is necessary to make sure the consistancy between the ColorPt type 
		/// and the ColorSpace.Type value. e_device_gray corresponds to an array of two numbers;
		/// e_device_rgb corresponds to an array of 3 numbers, e_device_cmyk corresponds to an array of
		/// 4 numnbers, while e_null correspons to an arry of 0 number. Entries out of the specified
		/// color space array length will be desgarded. However, missing entries for a specified color space
		/// will throw exception either when setting the color or when later retrieving color(colorspace)
		/// information.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public void SetBackgroundColor(ColorPt c, int CompNum)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetBackgroundColor(mp_annot, c.mp_colorpt, CompNum));
        }

		/// <summary> Gets static caption text of the Screen annotation.
		/// 
		/// </summary>
		/// <returns> A string containing the static caption text of the Screen annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The static caption is the widget annotation’s normal caption, which
		/// shall be displayed when it is not interacting with the user.
		/// Unlike the remaining entries with the captions, which apply only to widget
		/// annotations associated with pushbutton fields, the Static Caption(CA) entry may be used
		/// with any type of button field, including check boxes.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public string GetStaticCaptionText()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetStaticCaptionText(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets static caption text of the Screen annotation.
		/// (Optional; button fields only)
		/// 
		/// </summary>
		/// <param name="ct">A string containing the static caption text of the Screen annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The static caption is the widget annotation’s normal caption, which 
		/// shall be displayed when it is not interacting with the user.
		/// Unlike entries "RC, AC, I, RI, IX, IF, TP", which apply only to widget
		/// annotations associated with pushbutton fields, the Static Caption(CA) entry may be used
		/// with any type of button field, including check boxes.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public void SetStaticCaptionText(string ct)
        {
            UString str = new UString(ct);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetStaticCaptionText(mp_annot, str.mp_impl));
        }

		/// <summary> Gets the roll over caption text of the Screen annotation.
		/// 
		/// </summary>
		/// <returns> A string containing the static caption text of the Screen annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The rollover caption shall be displayed when the user rolls the cursor
		/// into its active area without pressing the mouse button.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public string GetRolloverCaptionText()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetRolloverCaptionText(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets the roll over caption text of the Screen annotation.
		/// (Optional; button fields only)
		/// 
		/// </summary>
		/// <param name="ct">A string containing the roll over caption text of the Screen annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The rollover caption shall be displayed when the user rolls the cursor
        /// into its active area without pressing the mouse button.</remarks>
        public void SetRolloverCaptionText(string ct)
        {
            UString str = new UString(ct);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetRolloverCaptionText(mp_annot, str.mp_impl));
        }

		/// <summary> Gets the button down caption text of the Screen annotation.
		/// 
		/// </summary>
		/// <returns> A string containing the button down text of the Screen annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The button down caption shall be displayed when the mouse button is
		/// pressed within its active area.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public string GetMouseDownCaptionText()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetMouseDownCaptionText(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets the button down caption text of the Screen annotation.
		/// (Optional; button fields only)
		/// 
		/// </summary>
		/// <param name="ct">A string containing the button down text of the Screen annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The button down caption shall be displayed when the mouse button is
		/// pressed within its active area.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public void SetMouseDownCaptionText(string ct)
        {
            UString str = new UString(ct);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetRolloverCaptionText(mp_annot, str.mp_impl));
        }

		/// <summary> Gets the static icon associated with the Screen annotation.
		/// 
		/// </summary>
		/// <returns> An SDF object that represents the static icon
		/// associated with the Screen annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The static icon object is a form XObject defining the
		/// widget annotation’s normal icon, which shall be
		/// displayed when it is not interacting with the user.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public SDF.Obj GetStaticIcon()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetStaticIcon(mp_annot, ref result));
            return result != IntPtr.Zero ? new Obj(result, GetRefHandleInternal()) : null;
        }

		/// <summary> Sets the static icon associated with the Screen annotation.
		/// (Optional; button fields only)
		/// 
		/// </summary>
		/// <param name="ic">An SDF object that represents the static icon
		/// associated with the Screen annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The static icon object is a form XObject defining the
		/// widget annotation’s normal icon, which shall be
		/// displayed when it is not interacting with the user.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public void SetStaticIcon(SDF.Obj ic)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetStaticIcon(mp_annot, ic.mp_obj));
        }

		/// <summary> Gets the rollover icon associated with the Screen annotation.
		/// 
		/// </summary>
		/// <returns> An SDF object that represents the rollover icon
		/// associated with the Screen annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The rollover icon object is a form XObject defining the 
		/// widget annotation’s rollover icon, which shall be displayed
		/// when the user rolls the cursor into its active area without
		/// pressing the mouse button.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public SDF.Obj GetRolloverIcon()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetRolloverIcon(mp_annot, ref result));
            return result != IntPtr.Zero ? new Obj(result, GetRefHandleInternal()) : null;
        }

		/// <summary> Sets the rollover icon associated with the Screen annotation.
		/// (Optional; button fields only)
		/// 
		/// </summary>
		/// <param name="ic">An SDF object that represents the rollover icon
		/// associated with the Screen annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The rollover icon object is a form XObject defining the 
		/// widget annotation’s rollover icon, which shall be displayed
		/// when the user rolls the cursor into its active area without
		/// pressing the mouse button.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public void SetRolloverIcon(SDF.Obj ic)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetRolloverIcon(mp_annot, ic.mp_obj));
        }

		/// <summary> Gets the Mouse Down icon associated with the Screen annotation.
		/// 
		/// </summary>
		/// <returns> An SDF object that represents the Mouse Down icon
		/// associated with the Screen annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Mouse Down icon object is a form XObject defining the 
		/// widget annotation’s alternate (down) icon, which shall be displayed
		/// when the mouse button is pressed within its active area.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public SDF.Obj GetMouseDownIcon()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetMouseDownIcon(mp_annot, ref result));
            return result != IntPtr.Zero ? new Obj(result, GetRefHandleInternal()) : null;
        }

		/// <summary> Sets the Mouse Down icon associated with the Screen annotation.
		/// (Optional; button fields only)
		/// Sets the Mouse Down icon associated with the Screen annotation.
		/// 
		/// </summary>
		/// <param name="ic">An SDF object that represents the Mouse Down icon
		/// associated with the Screen annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Mouse Down icon object is a form XObject defining the
		/// widget annotation’s alternate (down) icon, which shall be displayed
		/// when the mouse button is pressed within its active area.
		/// 
		/// This property is part of the Screen appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public void SetMouseDownIcon(SDF.Obj ic)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetMouseDownIcon(mp_annot, ic.mp_obj));
        }

        /// <summary> Gets the Scale Type of the Screen annotation.
		/// 
		/// </summary>
		/// <returns> An entry of the <c>ScaleType</c> enum which represents the
		/// Scale Type of the Screen annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>		
		/// <remarks>  This property is part of the Icon Fit dictionary, where the Icon Fit
		/// dictionary specifys how to display the button’s icon within the annotation
		/// rectangle of its widget annotation(Optional; pushbutton fields only)
		/// If present, the icon fit dictionary shall apply to all of the annotation’s icons
        /// (normal, rollover, and alternate).
        /// </remarks>
        public ScaleType GetScaleType()
        {
            ScaleType result = ScaleType.e_Anamorphic;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetScaleType(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the Scale Type of the Screen annotation.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="st">- an entry of the "ScaleType" enum which represents the
		/// Scale Type of the Screen annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>		
		/// <remarks>This property is part of the Icon Fit dictionary, where the Icon Fit
		/// dictionary specifys how to display the button’s icon within the annotation
		/// rectangle of its widget annotation(Optional; pushbutton fields only)
		/// If present, the icon fit dictionary shall apply to all of the annotation’s icons
        /// (normal, rollover, and alternate).
        /// </remarks>
        public void SetScaleType(ScaleType st)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetScaleType(mp_annot, st));
        }

        /// <summary> Gets the Icon and caption relationship of the Screen annotation.
		/// 
		/// </summary>
		/// <returns> An entry of the "IconCaptionRelation" enum which
		/// represents the relationship between the icon and the caption of the
		/// Screen annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public IconCaptionRelation GetIconCaptionRelation()
        {
            IconCaptionRelation result = IconCaptionRelation.e_NoIcon;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetIconCaptionRelation(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the Icon and caption relationship of the Screen annotation.
		/// (Optional; pushbutton fields only)
		/// 
		/// </summary>
		/// <param name="icr">An entry of the "IconCaptionRelation" enum which
		/// represents the relationship between the icon and the caption of the
		/// Screen annotation.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetIconCaptionRelation(IconCaptionRelation icr)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetIconCaptionRelation(mp_annot, icr));
        }

        /// <summary> Gets the condition under which the icon should be scaled.
		/// 
		/// </summary>
		/// <returns> an entry of the "ScaleCondition" enum which the icon should
		/// be scaled.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>		
		/// <remarks>  This property is part of the Icon Fit dictionary, where the Icon Fit
		/// dictionary specifys how to display the button’s icon within the annotation
		/// rectangle of its widget annotation(Optional; pushbutton fields only)
		/// If present, the icon fit dictionary shall apply to all of the annotation’s icons
        /// (normal, rollover, and alternate).
        /// </remarks>
        public ScaleCondition GetScaleCondition()
        {
            ScaleCondition result = ScaleCondition.e_Never;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetScaleCondition(mp_annot, ref result));
            return result;
        }

		/// <summary>Sets the condition under which the icon should be scaled.
		/// </summary>
		/// <param name="sd">an entry of the "ScaleCondition" enumerator which the icon should
        /// be scaled.
        /// </param>
        public void SetScaleCondition(ScaleCondition sd)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetScaleCondition(mp_annot, sd));
        }

		/// <summary> Gets the horizontal leftover of the icon within the annotatin.
		/// 
		/// </summary>
		/// <returns> A number indicating the horizontal
		/// leftover of the icon within the annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>the horizontal leftover is a number that shall be between
		/// 0.0 and 1.0 indicating the fraction of leftover space to allocate at the left.
		/// A value of 0.0 shall position the icon at the left of the annotation rectangle.
		/// A value of 0.5 shall center it in the horizontal direction within the rectangle.
        /// This entry shall be used only if the icon is scaled proportionally.
        /// Default value: 0.5.</remarks>
        public double GetHIconLeftOver()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetHIconLeftOver(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the horizontal leftover of the icon within the annotatin.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="hl">A number indicating the horizontal
		/// leftover of the icon within the annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the horizontal leftover is a number that shall be between
		/// 0.0 and 1.0 indicating the fraction of leftover space to allocate at the left.
		/// A value of 0.0 shall position the icon at the left of the annotation rectangle.
		/// A value of 0.5 shall center it in the horizontal direction within the rectangle.
		/// This entry shall be used only if the icon is scaled proportionally.
        /// Default value: 0.5.
        /// </remarks>
        public void SetHIconLeftOver(double hl)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetHIconLeftOver(mp_annot, hl));
        }

		/// <summary> Gets the vertical leftover of the icon within the annotatin.
		/// 
		/// </summary>
		/// <returns> a number indicating the vertical
		/// leftover of the icon within the annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the vertical leftover is a number that
		/// shall be between 0.0 and 1.0 indicating the fraction of leftover
		/// space to allocate at the bottom of the icon.
		/// A value of 0.0 shall position the icon at the bottom
		/// of the annotation rectangle.
		/// A value of 0.5 shall center it in the vertical direction within
		/// the rectangle.
		/// This entry shall be used only if the icon is scaled proportionally.
        /// Default value: 0.5.
        /// </remarks>
        public double GetVIconLeftOver()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetVIconLeftOver(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the vertical leftover of the icon within the annotatin.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="vl">A number indicating the vertical
		/// leftover of the icon within the annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the vertical leftover is a number that
		/// shall be between 0.0 and 1.0 indicating the fraction of leftover
		/// space to allocate at the bottom of the icon.
		/// A value of 0.0 shall position the icon at the bottom
		/// of the annotation rectangle.
		/// A value of 0.5 shall center it in the vertical direction within
		/// the rectangle.
        /// This entry shall be used only if the icon is scaled proportionally.
        /// Default value: 0.5.</remarks>
        public void SetVIconLeftOver(double vl)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetVIconLeftOver(mp_annot, vl));
        }

		/// <summary> Gets the fit full option being used.
		/// 
		/// </summary>
		/// <returns> A boolean value indicating the fit full option being used.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the FitFull value, if true, indicates that the button
		/// appearance shall be scaled to fit fully within the bounds of
		/// the annotation without taking into consideration the line
        /// width of the border. Default value: false.
        /// </remarks>
        public bool GetFitFull()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotGetFitFull(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the fit full option being used.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="ff">A boolean value indicating the fit full option being used.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the FitFull value, if true, indicates that the button
		/// appearance shall be scaled to fit fully within the bounds of
        /// the annotation without taking into consideration the line
        /// width of the border. Default value: false.</remarks>
        public void SetFitFull(bool ff)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ScreenAnnotSetFitFull(mp_annot, ff));
        }

        // Nested Types
        ///<summary>Icon scale types</summary>
		public enum ScaleType
		{
			///<summary>Scale the icon to fill the
			/// annotation rectangle exactly, without regard to its original
			/// aspect ratio (ratio of width to height).</summary>
			e_Anamorphic,
			///<summary>Proportional scaling: Scale the icon to fit
			/// the width or height of the annotation rectangle while maintaining
			/// the icon’s original aspect ratio. If the required horizontal and
			/// vertical scaling factors are different, use the smaller of the two,
			/// centering the icon within the annotation rectangle in the other
			/// dimension.</summary>
			e_Proportional
		}
        ///<summary>indicates where to position the text of the widget annotation’s caption relative to its icon</summary>
		public enum IconCaptionRelation
		{
			///<summary>No icon; caption only</summary>
			e_NoIcon,
			///<summary>No caption; icon only</summary>
			e_NoCaption,
			///<summary>Caption below the icon</summary>
			e_CBelowI,
			///<summary>Caption above the icon</summary>
			e_CAboveI,
			///<summary>Caption to the right of the icon</summary>
			e_CRightILeft,
			///<summary>Caption to the left of the icon</summary>
			e_CLeftIRight,
			///<summary>Caption overlaid directly on the icon</summary>
			e_COverlayI
		}

        /// <summary>spedifies the circumstances under which the icon shall be scaled inside the annotation rectangle.</summary>
		public enum ScaleCondition
		{
			///<summary>Always scale.</summary>
			e_Always,
			///<summary>Scale only when the icon is bigger than the annotation rectangle.</summary>
			e_WhenBigger,
			///<summary>Scale only when the icon is smaller than the annotation rectangle.</summary>
			e_WhenSmaller,
			///<summary>Never scale.</summary>
			e_Never
		}
    }
}