using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Font = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Action = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> Interactive forms use widget annotations (PDF 1.2) to represent the appearance 
    /// of fields and to manage user interactions. As a convenience, when a field has 
    /// only a single associated widget annotation, the contents of the field dictionary 
    /// and the annotation dictionary may be merged into a single dictionary containing 
    /// entries that pertain to both a field and an annotation.
    /// NOTE This presents no ambiguity, since the contents of the two kinds of 
    /// dictionaries do not conflict.
    /// </summary>
    public class Widget : Annot
    {
        internal Widget(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Widget annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public Widget(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		/// <summary> Creates a new Widget annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <param name="field">the field
		/// </param>
		/// <returns> A newly created blank Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Widget Create(SDF.SDFDoc doc, Rect pos, Field field)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref field.mp_field, ref result));
            return new Widget(result, doc);
        }

		/// <summary> Gets the field corresponding to the Widget annotation.
		/// 
		/// </summary>
		/// <returns> A Field object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Field GetField()
        {
            BasicTypes.TRN_Field result = new BasicTypes.TRN_Field();
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetField(mp_annot, ref result));
            return new Field(result, GetRefHandleInternal());
        }

        /// <summary> Gets the HighlightingMode of the Widget Annotation.
		/// 
		/// </summary>
		/// <returns> an entry of the enum "HighlightingMode", specifying the highlighting
		/// mode of the widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>		
        public HighlightingMode GetHighlightingMode()
        {
            HighlightingMode result = HighlightingMode.e_none;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetHighlightingMode(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the HighlightingMode of the Widget Annotation.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="mode">- an entry of the enum "HighlightingMode", specifying the highlighting
		/// mode of the widget annotation.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>	
        public void SetHighlightingMode(HighlightingMode mode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetHighlightingMode(mp_annot, mode));
        }

		/// <summary> Gets the action of the Widget annotation.
		/// 
		/// </summary>
		/// <returns> An action object representing the action of the Widget annotation.
		/// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The action is an action that shall be performed when the annotation is activated </remarks>
        public Action GetAction()
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetAction(mp_annot, ref result));
            return new Action(result, GetRefHandleInternal());
        }

		/// <summary> Sets the action of the Widget annotation
		/// (Optional; PDF 1.2 )
		/// 
		/// </summary>
		/// <param name="action">An action object representing the action of the Widget annotation.
		/// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The action is an action that shall be performed when the annotation is activated </remarks>
        public void SetAction(Action action)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetAction(mp_annot, action.mp_action));
        }

		/// <summary> Gets the number indicating border color space of the Widget annotation.
		/// 
		/// </summary>
		/// <returns> An integer indicating a color space value from the ColorSpace.Type enum. That is, 1 corresponding to "e_device_gray",
		/// 3 corresponding to "e_device_rgb", and 4 corresponding to "e_device_cmyk" if color space is applicable,
		/// orelse 0 corresponding to "e_null" if the color is transparent.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  This property is part of the Widget appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public int GetBorderColorCompNum()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetBorderColorCompNum(mp_annot, ref result));
            return result;
        }

		/// <summary> Gets the border color of the Widget annotation.
		/// 
		/// </summary>
		/// <returns> A ColorPt object that denotes the color of the Widget border.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  that the color can be in different color spaces: Gray, RGB, or CMYK. Call "GetBorderColorCompNum"
		/// to access the color space information corresponding to the border color.
		/// 
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public ColorPt GetBorderColor()
        {
            ColorPt result = new ColorPt();
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetBorderColor(mp_annot, result.mp_colorpt));
            return result;
        }

		/// <summary> Sets the border color of the Widget.
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
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public void SetBorderColor(ColorPt c, int CompNum)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetBorderColor(mp_annot, c.mp_colorpt, CompNum));
        }				

		/// <summary> Gets the number indicating background color space of the Widget annotation.
		/// 
		/// </summary>
		/// <returns> An integer indicating a color space value from the ColorSpace.Type enum. That is,
		/// 1 corresponding to "e_device_gray",
		/// 3 corresponding to "e_device_rgb", and 4 corresponding to "e_device_cmyk" if color space is applicable,
		/// orelse 0 corresponding to "e_null" if the color is transparent.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  This property is part of the Widget appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public int GetBackgroundColorCompNum()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetBackgroundColorCompNum(mp_annot, ref result));
            return result;
        }

		/// <summary> Gets the background color of the Widget annotation.
		/// 
		/// </summary>
		/// <returns> A ColorPt object that denotes the color of the Widget background.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  that the color can be in different color spaces: Gray, RGB, or CMYK. Call "GetBackgroundColorCompNum"
		/// to access the color space information corresponding to the border color.
		/// 
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public ColorPt GetBackgroundColor()
        {
            ColorPt result = new ColorPt();
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetBackgroundColor(mp_annot, result.mp_colorpt));
            return result;
        }

		/// <summary> Sets the background color of the Widget.
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
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public void SetBackgroundColor(ColorPt c, int CompNum)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetBackgroundColor(mp_annot, c.mp_colorpt, CompNum));
        }

		/// <summary> Gets static caption text of the Widget annotation.
		/// 
		/// </summary>
		/// <returns> A string containing the static caption text of the Widget annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The static caption is the widget annotation’s normal caption, which
		/// shall be displayed when it is not interacting with the user.
		/// Unlike the remaining entries with the captions, which apply only to widget
		/// annotations associated with pushbutton fields, the Static Caption(CA) entry may be used
		/// with any type of button field, including check boxes.
		/// 
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public string GetStaticCaptionText()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetStaticCaptionText(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets static caption text of the Widget annotation.
		/// (Optional; button fields only)
		/// 
		/// </summary>
		/// <param name="ct">A string containing the static caption text of the Widget annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The static caption is the widget annotation’s normal caption, which
		/// shall be displayed when it is not interacting with the user.
		/// Unlike entries "RC, AC, I, RI, IX, IF, TP", which apply only to widget
		/// annotations associated with pushbutton fields, the Static Caption(CA) entry may be used
		/// with any type of button field, including check boxes.
		/// 
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public void SetStaticCaptionText(string ct)
        {
            UString str = new UString(ct);
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetStaticCaptionText(mp_annot, str.mp_impl));
        }

		/// <summary> Gets the roll over caption text of the Widget annotation.
		/// 
		/// </summary>
		/// <returns> A string containing the static caption text of the Widget annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>The rollover caption shall be displayed when the user rolls the cursor
		/// into its active area without pressing the mouse button.
		/// 
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public string GetRolloverCaptionText()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetRolloverCaptionText(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets the roll over caption text of the Widget annotation.
		/// (Optional; button fields only)
		/// 
		/// </summary>
		/// <param name="ct">A string containing the roll over caption text of the Widget annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The rollover caption shall be displayed when the user rolls the cursor
        /// into its active area without pressing the mouse button.</remarks>
        public void SetRolloverCaptionText(string ct)
        {
            UString str = new UString(ct);
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetRolloverCaptionText(mp_annot, str.mp_impl));
        }

		/// <summary> Gets the button down caption text of the Widget annotation.
		/// 
		/// </summary>
		/// <returns> A string containing the button down text of the Widget annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The button down caption shall be displayed when the mouse button is
		/// pressed within its active area.
		/// 
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public string GetMouseDownCaptionText()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetMouseDownCaptionText(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets the button down caption text of the Widget annotation.
		/// (Optional; button fields only)
		/// 
		/// </summary>
		/// <param name="ct">A string containing the button down text of the Widget annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The button down caption shall be displayed when the mouse button is
		/// pressed within its active area.
		/// 
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public void SetMouseDownCaptionText(string ct)
        {
            UString str = new UString(ct);
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetMouseDownCaptionText(mp_annot, str.mp_impl));
        }

		/// <summary> Gets the static icon associated with the Widget annotation.
		/// 
		/// </summary>
		/// <returns> An SDF object that represents the static icon
		/// associated with the Widget annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>The static icon object is a form XObject defining the
		///  widget annotation’s normal icon, which shall be
		/// displayed when it is not interacting with the user.
		/// 
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary 
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public SDF.Obj GetStaticIcon()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetStaticIcon(mp_annot, ref result));
            return new SDF.Obj(result, GetRefHandleInternal());
        }

		/// <summary> Sets the static icon associated with the Widget annotation.
		/// (Optional; button fields only)
		/// 
		/// </summary>
		/// <param name="ic">An SDF object that represents the static icon
		/// associated with the Widget annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The static icon object is a form XObject defining the
		/// widget annotation’s normal icon, which shall be
		/// displayed when it is not interacting with the user.
		/// 
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public void SetStaticIcon(SDF.Obj ic)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetStaticIcon(mp_annot, ic.mp_obj));
        }

		/// <summary> Gets the rollover icon associated with the Widget annotation.
		/// 
		/// </summary>
		/// <returns> An SDF object that represents the rollover icon
		/// associated with the Widget annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>The rollover icon object is a form XObject defining the 
		/// widget annotation’s rollover icon, which shall be displayed
		/// when the user rolls the cursor into its active area without
		/// pressing the mouse button.
		/// 
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public SDF.Obj GetRolloverIcon()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetRolloverIcon(mp_annot, ref result));
            return new SDF.Obj(result, GetRefHandleInternal());
        }

		/// <summary> Sets the rollover icon associated with the Widget annotation.
		/// (Optional; button fields only)
		/// 
		/// </summary>
		/// <param name="ic">An SDF object that represents the rollover icon
		/// associated with the Widget annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The rollover icon object is a form XObject defining the
		/// widget annotation’s rollover icon, which shall be displayed
		/// when the user rolls the cursor into its active area without
		/// pressing the mouse button.
		/// 
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
        /// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.</remarks>
        public void SetRolloverIcon(SDF.Obj ic)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetRolloverIcon(mp_annot, ic.mp_obj));
        }

		/// <summary> Gets the Mouse Down icon associated with the Widget annotation.
		/// 
		/// </summary>
		/// <returns> An SDF object that represents the Mouse Down icon
		/// associated with the Widget annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Mouse Down icon object is a form XObject defining the
		/// widget annotation’s alternate (down) icon, which shall be displayed
		/// when the mouse button is pressed within its active area.		
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
        /// presentation on the page.
        /// </remarks>
        public SDF.Obj GetMouseDownIcon()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetMouseDownIcon(mp_annot, ref result));
            return new SDF.Obj(result, GetRefHandleInternal());
        }

		/// <summary> Sets the Mouse Down icon associated with the Widget annotation.
		/// (Optional; button fields only)
		/// Sets the Mouse Down icon associated with the Widget annotation.
		/// 
		/// </summary>
		/// <param name="ic">An SDF object that represents the Mouse Down icon
		/// associated with the Widget annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>The Mouse Down icon object is a form XObject defining the
		/// widget annotation’s alternate (down) icon, which shall be displayed
		/// when the mouse button is pressed within its active area.
		/// 
		/// This property is part of the Widget appearance characteristics dictionary, this dictionary
		/// that shall be used in constructing a dynamic appearance stream specifying the annotation’s visual
		/// presentation on the page.</remarks>
        public void SetMouseDownIcon(SDF.Obj ic)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetMouseDownIcon(mp_annot, ic.mp_obj));
        }
        /// <summary> Gets the Scale Type of the Widget annotation.
		/// 
		/// </summary>
		/// <returns> An entry of the "ScaleType" enum which represents the
		/// Scale Type of the Widget annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>		
		/// <remarks>This property is part of the Icon Fit dictionary, where the Icon Fit
		/// dictionary specifys how to display the button’s icon within the annotation
		/// rectangle of its widget annotation(Optional; pushbutton fields only)
		/// If present, the icon fit dictionary shall apply to all of the annotation’s icons
        /// (normal, rollover, and alternate).
        /// </remarks>
        public ScaleType GetScaleType()
        {
            ScaleType result = ScaleType.e_Proportional;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetScaleType(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the Scale Type of the Widget annotation.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="st">- an entry of the "ScaleType" enum which represents the
		/// Scale Type of the Widget annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>This property is part of the Icon Fit dictionary, where the Icon Fit.
		/// dictionary specifys how to display the button’s icon within the annotation
		/// rectangle of its widget annotation(Optional; pushbutton fields only)
		/// If present, the icon fit dictionary shall apply to all of the annotation’s icons
		/// (normal, rollover, and alternate).</remarks>
        public void SetScaleType(ScaleType st)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetScaleType(mp_annot, st));
        }
        /// <summary> Gets the Icon and caption relationship of the Widget annotation.
		/// 
		/// </summary>
		/// <returns> An entry of the "IconCaptionRelation" enum which
		/// represents the relationship between the icon and the caption of the
		/// Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public IconCaptionRelation GetIconCaptionRelation()
        {
            IconCaptionRelation result = IconCaptionRelation.e_NoIcon;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetIconCaptionRelation(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the Icon and caption relationship of the Widget annotation.
		/// (Optional; pushbutton fields only)
		/// 
		/// </summary>
		/// <param name="icr">An entry of the "IconCaptionRelation" enum which
		/// represents the relationship between the icon and the caption of the
		/// Widget annotation.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>		
        public void SetIconCaptionRelation(IconCaptionRelation icr)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetIconCaptionRelation(mp_annot, icr));
        }

        /// <summary> Gets the condition under which the icon should be scaled.
		/// 
		/// </summary>
		/// <returns> an entry of the "ScaleCondition" enum which the icon should
		/// be scaled.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>		
		/// <remarks>
		/// This property is part of the Icon Fit dictionary, where the Icon Fit
		/// dictionary specifys how to display the button’s icon within the annotation
		/// rectangle of its widget annotation(Optional; pushbutton fields only)
        /// If present, the icon fit dictionary shall apply to all of the annotation’s icons
        /// (normal, rollover, and alternate).</remarks>
        public ScaleCondition GetScaleCondition()
        {
            ScaleCondition result = ScaleCondition.e_Never;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetScaleCondition(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the condition under which the icon should be scaled.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="sd">An entry of the "ScaleCondition" enum which the icon should
		/// be scaled.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>		
		/// <remarks>This property is part of the Icon Fit dictionary, where the Icon Fit 
		/// dictionary specifys how to display the button’s icon within the annotation
		/// rectangle of its widget annotation(Optional; pushbutton fields only)
        /// If present, the icon fit dictionary shall apply to all of the annotation’s icons
        /// (normal, rollover, and alternate).</remarks>
        public void SetScaleCondition(ScaleCondition sd)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetScaleCondition(mp_annot, sd));
        }

		/// <summary> Gets the horizontal leftover of the icon within the annotatin.
		/// 
		/// </summary>
		/// <returns> A number indicating the horizontal
		/// leftover of the icon within the annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the horizontal leftover is a number that shall be between
		/// 0.0 and 1.0 indicating the fraction of leftover space to allocate at the left.
		/// A value of 0.0 shall position the icon at the left of the annotation rectangle.
		/// A value of 0.5 shall center it in the horizontal direction within the rectangle.
        /// This entry shall be used only if the icon is scaled proportionally.
        /// Default value: 0.5.</remarks>
        public double GetHIconLeftOver()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetHIconLeftOver(mp_annot, ref result));
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
        /// Default value: 0.5.</remarks>
        public void SetHIconLeftOver(double hl)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetHIconLeftOver(mp_annot, hl));
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
        /// Default value: 0.5.</remarks>
        public double GetVIconLeftOver()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetVIconLeftOver(mp_annot, ref result));
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
		/// <remarks>the vertical leftover is a number that
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetVIconLeftOver(mp_annot, vl));
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
        /// width of the border. Default value: false.</remarks>
        public bool GetFitFull()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetFitFull(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the fit full option being used.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="ff">A boolean value indicating the fit full option being used.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>the FitFull value, if true, indicates that the button
		/// appearance shall be scaled to fit fully within the bounds of
        /// the annotation without taking into consideration the line
        /// width of the border. Default value: false.</remarks>
        public void SetFitFull(bool ff)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetFitFull(mp_annot, ff));
        }
        /// <summary> Sets the text color of the Widget Annotation.
        /// 
        /// </summary>
        /// <param name="color">ColorPt object representing the color.
        /// </param>
        /// <param name="col_comp">An integer indicating a color space value from the ColorSpace::Type enum. That is,
        /// 1 corresponding to "e_device_gray",
        /// 3 corresponding to "e_device_rgb", and 4 corresponding to "e_device_cmyk".
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetTextColor(ColorPt color, int col_comp)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetTextColor(mp_annot, color.mp_colorpt, col_comp));
        }
        /// <summary> Returns the text color of the Widget Annotation.
        /// 
        /// </summary>
        /// <returns> The text color. 
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  that the color can be in different color spaces: Gray, RGB, or CMYK. Call "GetTextColorCompNum"
        /// to access the color space information corresponding to the border color.
        /// </remarks>
        public ColorPt GetTextColor()
        {
            ColorPt color = new ColorPt();
            int col_comp = 0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetTextColor(mp_annot, color.mp_colorpt, ref col_comp));
            return color;
        }
        /// <summary> Returns the number of color components in the text color of the Widget Annotation.
        /// 
        /// </summary>
        /// <returns>An integer indicating a color space value from the ColorSpace::Type enum. That is,
        /// 1 corresponding to "e_device_gray",
        /// 3 corresponding to "e_device_rgb", and 4 corresponding to "e_device_cmyk" if color space is applicable.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetTextColorCompNum()
        {
            ColorPt color = new ColorPt();
            int col_comp = 0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetTextColor(mp_annot, color.mp_colorpt, ref col_comp));
            return col_comp;
        }
        /// <summary> Sets the font size of the Widget Annotation.
        /// 
        /// </summary>
        /// <param name="font_size">the new font size
        /// </param>
        /// <exception cref="PDFNetException"> PDFNetException the PDFNet exception </exception>
        /// <remarks> A font size of 0 specifies that the text should be autosized to fit in the Widget.
        /// </remarks>
        public void SetFontSize(double font_size)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetFontSize(mp_annot, font_size));
        }
        /// <summary> Returns the font size used in this Widget Annotation.
        /// 
        /// </summary>
        /// <returns> the font size
        /// </returns>
        /// <exception cref="PDFNetException"> PDFNetException the PDFNet exception </exception>
        /// <remarks> A font size of 0 specifies that the text should be autosized to fit in the Widget.
        /// </remarks>
        public double GetFontSize()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetFontSize(mp_annot, ref result));
            return result;
        }
        /// <summary> Specifies a font to be used for text in this Widget
        /// </summary>
        /// <param name="font">the font to use</param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetFont(Font font)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotSetFont(mp_annot, font.mp_font));
        }
        /// <summary> Retrieves the font used for displaying text in this Widget.
        /// 
        /// </summary>
        /// <returns> The font used by this Widget
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Font GetFont()
        {
            TRN_Font result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WidgetAnnotGetFont(mp_annot, ref result));
            return new Font(result, this.m_ref);
        }

        /// <summary> Creates a Widget annotation and initialize it using given annotation object.
        /// 
        /// </summary>
		/// <param name="ann"> The input annotation.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public Widget(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Releases all resources used by the Widget </summary>
		~Widget() 
        {
            Dispose(false);
        }

        // Nested Types
        ///<summary> This enum specifies the Highlighting mode of the widget annotation. </summary>
		public enum HighlightingMode
        {
            ///<summary>No highlighting</summary>
			e_none,
			///<summary>Invert the contents of the annotation rectangle.</summary>
			e_invert,
			///<summary>Invert the annotation’s border</summary>
			e_outline,
			///<summary>Display the annotation’s down appearance, if any
			/// If no down appearance is defined, the contents
			/// of the annotation rectangle shall be offset to appear as if it
			/// were being pushed below the surface of the page.</summary>
			e_push,
			///<summary>Same as e_push (which is preferred).</summary>
			e_toggle
        }

        ///<summary>Icon and Caption Relation of the Widget annotation</summary>
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

        ///<summary>spedifies the circumstances under which the
		///icon shall be scaled inside the annotation rectangle</summary>
		public enum ScaleCondition
        {
            ///<summary>Always scale</summary>
			e_Always,
			///<summary>Scale only when the icon is bigger than the annotation rectangle.</summary>
			e_WhenBigger,
			///<summary>Scale only when the icon is smaller than the annotation rectangle.</summary>
			e_WhenSmaller,
			///<summary>Never scale.</summary>
			e_Never
        }

        ///<summary>scale types</summary>
        public enum ScaleType
        {
            ///<summary>Scale the icon to fill the annotation rectangle 
            /// exactly, without regard to its original aspect 
            /// ratio (ratio of width to height).</summary>
            e_Anamorphic,
            ///<summary>Scale the icon to fit the width or height 
            /// of the annotation rectangle while maintaining 
            /// the icon’s original aspect ratio. If the 
            /// required horizontal and vertical scaling 
            /// factors are different, use the smaller of the two, 
            /// centering the icon within the annotation rectangle 
            /// in the other dimension.</summary>
            e_Proportional
        }


    }
}