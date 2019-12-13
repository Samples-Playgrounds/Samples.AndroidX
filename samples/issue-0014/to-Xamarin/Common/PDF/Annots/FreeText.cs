using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Font = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> A free text annotation (PDF 1.3) displays text directly on the page. 
    /// Unlike an ordinary text annotation, a free text annotation has no 
    /// open or closed state; instead of being displayed in a pop-up window, 
    /// the text shall be always visible. 
    /// </summary>
    public class FreeText : Markup
    {
        internal FreeText(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates an FreeText annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public FreeText(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		/// <summary> Creates a new FreeText annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the FreeText annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the FreeText annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank FreeText annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static FreeText Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new FreeText(result, doc);
        }

		/// <summary> Gets the default appearance of the FreeText annotation.
		/// 
		/// </summary>
		/// <returns> A string representing the default appearance of the FreeText annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The default appearance string shall be used in formatting the text.The
        /// annotation dictionary’s Appearance (AP) entry, if present, shall take precedence over
        /// this entry.</remarks>
        public string GetDefaultAppearance()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotGetDefaultAppearance(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets the default appearance of the FreeText annotation.
		/// 
		/// </summary>
		/// <param name="app_str">A string representing the default appearance of the FreeText annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The default appearance string shall be used in formatting the text.The 
        /// annotation dictionary’s Appearance (AP) entry, if present, shall take precedence over
        /// this entry.</remarks>
        public void SetDefaultAppearance(string app_str)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotSetDefaultAppearance(mp_annot, app_str));
        }

		/// <summary> Gets the quading format of the FreeText annotation.
		/// 
		/// </summary>
		/// <returns> A int code indicating the quading format of the FreeText annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The int code specifies the form of quadding (justification) 
		/// that shall be used in displaying the annotation’s text:
		/// 0Left-justified
		/// 1Centered
        /// 2Right-justified
        /// Default value: 0 (left-justified).</remarks>
        public int GetQuaddingFormat()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotGetQuaddingFormat(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the quading format of the FreeText annotation.
		/// (Optional; PDF 1.4)
		/// 
		/// </summary>
		/// <param name="app_qform">the new quadding format
		/// </param>
		/// <returns> A int code indicating the quading format of the FreeText annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The int code specifies the form of quadding (justification) 
		/// that shall be used in displaying the annotation’s text:
		/// 0Left-justified
		/// 1Centered
        /// 2Right-justified
        /// Default value: 0 (left-justified).</remarks>
        public void SetQuaddingFormat(int app_qform)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotSetQuaddingFormat(mp_annot, app_qform));
        }

		/// <summary> Gets p1 of the callout line points of the FreeText annotation.
		/// 
		/// </summary>
		/// <returns> Three point objects if the line is bent or two point objects if
		/// the line is straight.
		/// </returns>
		/// <param name="out_p1">The starting point.</param>
		/// <param name="out_p2">The ending point.</param>
		/// <param name="out_p3">The knee point.</param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The returning values are two or three Point objects specifying a callout line attached 
		/// to the free text annotation. Three Point objects represent
		/// the starting, knee point, and ending coordinates of the line in default
		/// user space, two Point objects represent the starting and ending coordinates of the line.
		/// 
		/// If the line is straight, i.e. only has two points,
        /// two valid points will be returned in p1 and p2, the p3 will be
        /// a point with negative x and y coordinate values.</remarks>
        public void GetCalloutLinePoints(Point out_p1, Point out_p2, Point out_p3)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotGetCalloutLinePoints(mp_annot, ref out_p1.mp_imp, ref out_p2.mp_imp, ref out_p3.mp_imp));
        }

		/// <summary> Sets the callout line points of the FreeText annotation.
		/// (Optional; meaningful only if IT is FreeTextCallout; PDF 1.6)
		/// 
		/// </summary>
		/// <param name="p1">The staring point.
		/// </param>
		/// <param name="p2">The ending point.
		/// </param>
		/// <param name="p3">The kee point.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The three Point objects specify a callout line attached
		/// to the free text annotation. The three Point objects represent
        /// the starting, knee point, and ending coordinates of the line in default
        /// user space.</remarks>
        public void SetCalloutLinePoints(Point p1, Point p2, Point p3)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotSetCalloutLinePoints(mp_annot, ref p1.mp_imp, ref p2.mp_imp, ref p3.mp_imp));
        }

		/// <summary> Sets the callout line points of the FreeText annotation.
		/// (Optional; meaningful only if IT is FreeTextCallout; PDF 1.6)
		/// 
		/// </summary>
		/// <param name="p1">The staring point.
		/// </param>
		/// <param name="p2">The ending point.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The two Point objects specifies a callout line attached
		/// to the free text annotation. Tbe two Point objects represent
        /// the starting and ending coordinates of the line.
        /// </remarks>
        public void SetCalloutLinePoints(Point p1, Point p2)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotSetCalloutLinePointsTwo(mp_annot, ref p1.mp_imp, ref p2.mp_imp));
        }

        //IntentName GetIntentName() ;
		/// <summary> Gets the Intent name as an entry from the enum "IntentName"
		/// of the annnotation type.
		/// 
		/// </summary>
		/// <returns> the intent name
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <returns>s The intent name of the annotation as
		/// an entry from the enum "IntentName".
		/// </returns>
		/// <remarks>  The intent name describes the intent of the free text annotation.
		/// The following values shall be valid:
		/// e_FreeText - The annotation is intended to function as a plain free-text annotation.
		/// A plain free-text annotation is also known as a text box comment.
		/// e_FreeTextCallout - The annotation is intended to function as a callout.
		/// The callout is associated with an area on the page through the callout line specified in CL.
		/// e_FreeTextTypeWriter - The annotation is intended to function as a click-to-type or typewriter
		/// object and no callout line is drawn.
        /// Default value: e_FreeText
        /// </remarks>
        public IntentName GetIntentName()
        {
            IntentName result = IntentName.e_Unknown;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotGetIntentName(mp_annot, ref result));
            return result;
        }

		//public void SetIntentName(IntentName mode=e_FreeText);
		/// <summary> Sets the Intent name as an entry from the enum "IntentName"
		/// of the annnotation type.
		/// (Optional; PDF 1.4)
		/// 
		/// </summary>
		/// <param name="mode">The intent name of the annotation as
		/// an entry from the enum "IntentJName".
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The intent name describes the intent of the free text annotation. 
		/// The following values shall be valid:
		/// e_FreeText - The annotation is intended to function as a plain free-text annotation.
		/// A plain free - text annotation is also known as a text box comment.
		/// e_FreeTextCallout - The annotation is intended to function as a callout.
		/// The callout is associated with an area on the page through the callout line specified in CL.
		/// e_FreeTextTypeWriter - The annotation is intended to function as a click-to-type or typewriter
        /// object and no callout line is drawn.
        /// Default value: e_FreeText</remarks>
        public void SetIntentName(IntentName mode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotSetIntentName(mp_annot, mode));
        }
        /// <summary> Sets the Intent name to e_FreeText.</summary>
        public void SetIntentName()
        {
            SetIntentName(IntentName.e_FreeText);
        }

		//EndingStyle GetEndingStyle() ;
		/// <summary> Gets the ending style of the callout line of the FreeText Annotation.
		/// 
		/// </summary>
		/// <returns> The ending style represented as one of the entries of the enum "EndingStyle"
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The ending style specifies the line ending style that
		/// shall be used in drawing the callout line specified in CallOut Line Points
		/// (CL). The enum entry shall specify the line ending style for the endpoint
        /// defined by the starting point(p1) of the CallOut Line Points.
        /// Default value: e_None.</remarks>
        public Line.EndingStyle GetEndingStyle()
        {
            Line.EndingStyle result = Line.EndingStyle.e_None;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotGetEndingStyle(mp_annot, ref result));
            return result;
        }

		//public void SetEndingStyle(EndingStyle est);
		/// <summary> Sets the ending style of the callout line of the FreeText Annotation.
		/// (Optional; meaningful only if CL is present; PDF 1.6)
		/// 
		/// </summary>
		/// <param name="est">The ending style represented using one of the
		/// entries of the enum "EndingStyle"
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The ending style specifies the line ending style that
		/// shall be used in drawing the callout line specified in CallOut Line Points
		/// (CL). The enum entry shall specify the line ending style for the endpoint
        /// defined by the starting point(p1) of the CallOut Line Points.
        /// Default value: e_None.</remarks>
        public void SetEndingStyle(Line.EndingStyle est)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotSetEndingStyle(mp_annot, est));
        }

		//public void SetEndingStyle(string est);
		/// <summary> Sets the ending style of the callout line of the FreeText Annotation.
		/// (Optional; meaningful only if CL is present; PDF 1.6)
		/// 
		/// </summary>
		/// <param name="est">The ending style represented using a string.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The ending style specifies the line ending style that 
		/// shall be used in drawing the callout line specified in CallOut Line Points
		/// (CL). The enum entry shall specify the line ending style for the endpoint
        /// defined by the starting point(p1) of the CallOut Line Points.
        /// Default value: "None".</remarks>
        public void SetEndingStyle(string est)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotSetEndingStyleName(mp_annot, est));
        }

		/// <summary> Sets the text color of the FreeText Annotation.
		/// 
		/// </summary>
		/// <param name="color">ColorPt object representing the color.
		/// </param>
		/// <param name="col_comp">number of colorant components in ColorPt object.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Current implementation of this method creates a non-standard 
		/// entry in the annotation dictionary and uses it to generate the appearance
		/// stream. Make sure you call RefreshAppearance() after changing text or
        /// line color, and remember that editing the annotation in other PDF
        /// application will produce different appearance.</remarks>
        public void SetTextColor(ColorPt color, int col_comp)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotSetTextColor(mp_annot, color.mp_colorpt, col_comp));
        }
		/// <summary> Returns the text color of the FreeText Annotation.
		/// 
		/// </summary>
		/// <returns> the text color
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Current implementation of this method uses a non-standard 
        /// entry in the annotation dictionary and will not return meaningful
        /// results when called on annotations not created with PDFTron software.</remarks>
        public ColorPt GetTextColor()
        {
            ColorPt color = new ColorPt();
			int col_comp = 0;
			PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotGetTextColor(mp_annot, color.mp_colorpt, ref col_comp));
			return color;
        }
		/// <summary> Returns the number of color components in the line and border color
		/// of the FreeText Annotation.
		/// 
		/// </summary>
		/// <returns> the text color comp num
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetTextColorCompNum()
        {
            ColorPt color = new ColorPt();
			int col_comp = 0;
			PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotGetTextColor(mp_annot, color.mp_colorpt, ref col_comp));
			return col_comp;            
        }
		/// <summary> Sets the line and border color of the FreeText Annotation.
		/// 
		/// </summary>
		/// <param name="color">ColorPt object representing the color.
		/// </param>
		/// <param name="col_comp">number of colorant components in ColorPt object.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Current implementation of this method creates a non-standard
		/// entry in the annotation dictionary and uses it to generate the appearance
		/// stream. Make sure you call RefreshAppearance() after changing text or
        /// line color, and remember that editing the annotation in other PDF
        /// application will produce different appearance.</remarks>
        public void SetLineColor(ColorPt color, int col_comp)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotSetLineColor(mp_annot, color.mp_colorpt, col_comp));
        }

		/// <summary> Returns the line and border color of the FreeText Annotation.
		/// 
		/// </summary>
		/// <returns> the line color
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Current implementation of this method uses a non-standard
        /// entry in the annotation dictionary and will not return meaningful
        /// results when called on annotations not created with PDFTron software.</remarks>
        public ColorPt GetLineColor()
        {
            ColorPt color = new ColorPt();
            int col_comp = 0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotGetLineColor(mp_annot, color.mp_colorpt, ref col_comp));
            return color;
        }
		/// <summary> Returns the number of color components in the line and border color
		/// of the FreeText Annotation.
		/// 
		/// </summary>
		/// <returns> the line color comp num
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetLineColorCompNum()
        {
            ColorPt color = new ColorPt();
			int col_comp = 0;
			PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotGetLineColor(mp_annot, color.mp_colorpt, ref col_comp));
			return col_comp;            
        }

		/// <summary> Sets the font size of the FreeText Annotation.
		/// 
		/// </summary>
		/// <param name="font_size">the font size
        /// </param>
        /// <exception cref="PDFNetException"> PDFNetException the PDFNet exception </exception>
        public void SetFontSize(double font_size)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotSetFontSize(mp_annot, font_size));
        }
		/// <summary> Returns the font size of the FreeText Annotation.
		/// 
		/// </summary>
		/// <returns> the font size
        /// </returns>
        /// <exception cref="PDFNetException"> PDFNetException the PDFNet exception </exception>
        public double GetFontSize()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotGetFontSize(mp_annot, ref result));
            return result;
        }
        /// <summary> Sets the font used to FreeText.
        /// 
        /// </summary>
        /// <param name="font">the font
        /// </param>
        /// <param name="font_sz">the font_sz
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /*public void SetFont(Font font)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotSetFont(mp_annot, font.mp_font));
        }*/
        /// <summary> Gets the font.
        /// 
        /// </summary>
        /// <returns> currently selected font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /*public Font GetFont()
        {
            TRN_Font result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FreeTextAnnotGetFont(mp_annot, ref result));
            return new Font(result, this.m_ref);
        }*/
        /// <summary> Creates an FreeText annotation and initialize it using given annotation object.
        /// 
        /// </summary>
        /// <param name="ann">the annot
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public FreeText(Annot ann) : base(ann.GetSDFObj()) {}
		/// <summary> Releases all resources used by the FreeText </summary>
		~FreeText()
        {
            Dispose(false);
        }

        // Nested Types
        /// <summary>intentions of the FreeText annotation</summary>
		public enum IntentName
		{
			///<summary>plain free-text annotation is also known as a text box comment. </summary>
			e_FreeText,
			///<summary>callout. 
			/// The callout is associated with an area on the page 
			/// through the callout line specified in CL.</summary>
			e_FreeTextCallout,
			///<summary>click-to-type 
			/// or typewriter object and no callout line is drawn.</summary>
			e_FreeTextTypeWriter,
			///<summary>Undefined (Userdefined) intent type. </summary>
			e_Unknown
		}
    }
}