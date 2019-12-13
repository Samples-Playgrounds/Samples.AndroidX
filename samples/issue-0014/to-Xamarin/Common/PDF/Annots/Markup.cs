using System;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Exception = System.IntPtr;
using TRN_Annot = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> The meaning of an annotation’s Contents entry varies by annotation type. 
    /// Typically, it is the text that shall be displayed for the annotation or, 
    /// if the annotation does not display text, an alternate description of the 
    /// annotation’s contents in human-readable form. In either case, the Contents 
    /// entry is useful when extracting the document’s contents in support of 
    /// accessibility to users with disabilities or for other purposes.
    /// Many annotation types are defined as markup annotations because they are 
    /// used primarily to mark up PDF documents. These annotations have text that 
    /// appears as part of the annotation and may be displayed in other ways by a 
    /// conforming reader, such as in a Comments pane.
    /// Markup annotations may be divided into the following groups:
    /// <list type="bullet">
    /// <item><description>
    /// Free text annotations display text directly on the page. 
    /// The annotation’s Contents entry specifies the displayed text.
    /// </description></item>
    /// <item><description>
    /// Most other markup annotations have an associated pop-up window 
    /// that may contain text. The annotation’s Contents entry specifies 
    /// the text that shall be displayed when the pop-up window is opened. 
    /// These include text, line, square, circle, polygon, polyline, 
    /// highlight,underline, squiggly-underline, strikeout, rubber stamp, 
    /// caret, ink, and file attachment annotations.
    /// </description></item>
    /// <item><description>
    /// Sound annotations do not have a pop-up window but may also have 
    /// associated text specified by the Contents entry.
    /// When separating text into paragraphs, a CARRIAGE RETURN (0Dh) shall 
    /// be used and not, for example, a LINE FEED character (0Ah).
    /// <remarks>A subset of markup annotations is called text markup 
    /// annotations.</remarks>
    /// </description></item>
    /// </list>
    /// The remaining annotation types are not considered markup annotations:
    /// <list type="bullet">    
    /// <item><description>
    /// The pop-up annotation type shall not appear by itself; it shall be 
    /// associated with a markup annotation that uses it to display text.
    /// <remarks>If an annotation has no parent, the Contents entry shall 
    /// represent the text of the annotation, otherwise it shall be 
    /// ignored by a conforming reader.</remarks>
    /// </description></item>
    /// <item><description>
    /// For all other annotation types (Link, Movie, Widget, PrinterMark, 
    /// and TrapNet), the Contents entry shall provide an alternate 
    /// representation of the annotation’s contents in human-readable form, 
    /// which is useful when extracting the document’s contents in support of 
    /// accessibility to users with disabilities or for other purposes.
    /// </description></item>
    /// </list>
    /// </summary>
    public class Markup : Annot
    {
        /// <summary>Creates a Markup annotation and initialize it using given <c>SDF.Obj</c> object. 
		/// </summary>
		/// <param name="d">existing Markup object
		/// </param>
		/// <remarks>The constructor does not copy any data, but is instead the logical equivalent of a type cast.
		/// </remarks>
		public Markup(SDF.Obj d) : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }
		/// <summary> Gets the title of the Markup Annotation.
		/// 
		/// </summary>
		/// <returns> A string representing the title of the Markup Annotation
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The title is The text label that shall be displayed in the
		/// title bar of the annotation’s pop-up window when open and active.
        /// This entry shall identify the user who added the annotation.
        /// </remarks>
        public string GetTitle()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotGetTitle(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets the title of the Markup Annotation.
		/// (Optional; PDF 1.1)
		/// 
		/// </summary>
		/// <param name="title">A string representing the title of the Markup Annotation
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The title is The text label that shall be displayed in the
        /// title bar of the annotation’s pop-up window when open and active.
        /// This entry shall identify the user who added the annotation.</remarks>
        public void SetTitle(string title)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotSetTitle(mp_annot, title));
        }

		/// <summary> Gets the Popup object associated with this Markup annotation.
		/// 
		/// </summary>
		/// <returns> A Popup object that is associated with this Markup annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The Popup is An indirect reference to a pop-up annotation for
        ///  entering or editing the text associated with this annotation.</remarks>
        public Popup GetPopup()
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotGetPopup(mp_annot, ref result));
            return new Popup(result, GetRefHandleInternal());
        }

		/// <summary> Sets the Popup object associated with this Markup annotation.
		/// (Optional; PDF 1.3 )
		/// 
		/// </summary>
		/// <param name="bs">the new popup
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  the Popup is An indirect reference to a pop-up annotation for
        /// entering or editing the text associated with this annotation.</remarks>
        public void SetPopup(Popup bs)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotSetPopup(mp_annot, bs.mp_annot));
        }

		/// <summary> Gets the contant opacity value corresponding to the annotation.
		/// 
		/// </summary>
		/// <returns> A number indicating the opacity value corresponding to the annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constant opacity value shall be used in painting the annotation.
		/// This value shall apply to all visible elements of the annotation in its closed state
		/// (including its background and border) but not to the pop-up window that appears when the annotation is opened.
		/// The specified value shall not used if the annotation has an appearance stream
		/// in that case, the appearance stream shall specify any transparency. (However, if the compliant viewer regenerates
		/// the annotation’s appearance stream, it may incorporate the CA value into the stream’s content.)
		/// The implicit blend mode is Normal.
		/// Default value: 1.0.
		/// If no explicit appearance stream is defined for the annotation,
		/// it may bepainted by implementation-dependent means that do not
		/// necessarily conform to the PDF imaging model; in this case, the effect
        /// of this entry is implementation-dependent as well.
        /// </remarks>
        public double GetOpacity()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotGetOpacity(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the contant opacity value corresponding to the annotation.
		/// (Optional; PDF 1.4 )
		/// 
		/// </summary>
		/// <param name="op">the new opacity
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constant opacity value shall be used in painting the annotation.
		/// This value shall apply to all visible elements of the annotation in its closed state
		/// (including its background and border) but not to the pop-up window that appears when the annotation is opened.
		/// The specified value shall not used if the annotation has an appearance stream
		/// in that case, the appearance stream shall specify any transparency. (However, if the compliant viewer regenerates
		/// the annotation’s appearance stream, it may incorporate the CA value into the stream’s content.)
		/// The implicit blend mode is Normal.
		/// Default value: 1.0.
		/// If no explicit appearance stream is defined for the annotation,
		/// it may bepainted by implementation-dependent means that do not
        /// necessarily conform to the PDF imaging model; in this case, the effect
        /// of this entry is implementation-dependent as well.</remarks>
        public void SetOpacity(double op)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotSetOpacity(mp_annot, op));
        }

		/// <summary> Gets subject of the Markup Annotation.
		/// 
		/// </summary>
		/// <returns> A string representing the subject of the Markup Annotation
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The subject is The Text representing a short description of
        /// the subject being addressed by the annotation.</remarks>
        public string GetSubject()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotGetSubject(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets subject of the Markup Annotation.
		/// (Optional; PDF 1.5 )
		/// 
		/// </summary>
		/// <param name="subj">the new subject
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The subject is The Text representing a short description of
        /// the subject being addressed by the annotation.</remarks>
        public void SetSubject(string subj)
        {
            UString str = new UString(subj);
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotSetSubject(mp_annot, str.mp_impl));
        }

		/// <summary> Gets the creation date of the Markup Annotation
		/// 
		/// </summary>
		/// <returns>A Date object indicating the date the Markup Annotation is created
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>The Date object not only includes date, it actually includes both date and  time when the annotation was created.
        /// This corresponds to the 'CreationDate' field of the markup annotation's dictionary.
        /// </remarks>
        public Date GetCreationDates()
        {
            BasicTypes.TRN_Date result = new BasicTypes.TRN_Date();
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotGetCreationDates(mp_annot, ref result));
            return new Date(result);
        }

		/// <summary> Sets the creation date of the Markup Annotation.
		/// (Optional; PDF 1.5 )
		/// 
		/// </summary>
		/// <param name="dt">the new creation dates
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Date object not only includes date, it actually includes both date and
        /// time when the annotation was created.
        /// This corresponds to the 'CreationDate' field of the markup annotation's dictionary. </remarks>
        public void SetCreationDates(Date dt)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotSetCreationDates(mp_annot, ref dt.mp_date));
        }

        /// <summary> Gets the Border Effect of the Markup Annotation.
		/// 
		/// </summary>
		/// <returns> An entry from the enum "BorderEffect" that
		/// represents the border effect.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public BorderEffect GetBorderEffect()
        {
            BorderEffect result = BorderEffect.e_None;
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotGetBorderEffect(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the Border Effect of the Markup Annotation.
		/// (Optional; PDF 1.5 )
		/// 
		/// </summary>
		/// <param name="effect">An entry from the enum "BorderEffect" that
		/// represents the border effect.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetBorderEffect(BorderEffect effect)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotSetBorderEffect(mp_annot, effect));
        }

		/// <summary> Gets the Border Effect Intensity of the Markup Annotation.
		/// 
		/// </summary>
		/// <returns> A number representing the border effect.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The BorderEffectIntensity value is a number describing
        /// the intensity of the effect, in the range 0 to 2. Default value: 0.</remarks>
        public double GetBorderEffectIntensity()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotGetBorderEffectIntensity(mp_annot, ref result));
            return result;
        }


		/// <summary> Sets the Border Effect Intensity of the Markup Annotation.
		/// (Optional; valid only if Border effect is Cloudy)
		/// 
		/// </summary>
		/// <param name="intensity">A number representing the border effect.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The BorderEffectIntensity value is a number describing 
        /// the intensity of the effect, in the range 0 to 2. Default value: 0.</remarks>
        public void SetBorderEffectIntensity(double intensity)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotSetBorderEffectIntensity(mp_annot, intensity));
        }

		/// <summary> Sets the interior color of the Square.
		/// (Optional; PDF 1.4 )
		/// 
		/// </summary>		
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  it is necessary to make sure the consistancy between the ColorPt type
		/// and the ColorSpace.Type value. e_device_gray corresponds to an array of two numbers;
		/// e_device_rgb corresponds to an array of 3 numbers, e_device_cmyk corresponds to an array of
		/// 4 numnbers, while e_null correspons to an arry of 0 number. Entries out of the specified
		/// color space array length will be desgarded. However, missing entries for a specified color space
		/// will throw exception either when setting the color or when later retrieving color(colorspace)
		/// information.
		/// </remarks>
        /// <returns>interior color
        /// </returns>
        public ColorPt GetInteriorColor()
        {
            ColorPt result = new ColorPt();
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotGetInteriorColor(mp_annot, result.mp_colorpt));
            return result;
        }

		/// <summary> Gets the number indicating interior color space of the Square.
		/// 
		/// </summary>
		/// <returns> An integer indicating a color space value from the ColorSpace.Type enum. That is, 1 corresponding to "e_device_gray",
		/// 3 corresponding to "e_device_rgb", and 4 corresponding to "e_device_cmyk" if color space is applicable,
		/// orelse 0 corresponding to "e_null" if the color is transparent.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetInteriorColorCompNum()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotGetInteriorColorCompNum(mp_annot, ref result));
            return result;
        }

		/// <summary> Gets the interior color of the Square.				
		/// </summary>
		/// <param name="c">A ColorPt object that denotes the color of the Markup annotation.
		/// </param>
		/// <param name="CompNum">An integer indicating the number of channels forming the color space used. It also defines the length of the array to be allocated for storing the entries of c.
		/// </param>
		/// <returns> A ColorPt object that denotes the color of the Square.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  that the color can be in different color spaces: Gray, RGB, or CMYK. Call "GetInteriorColorCompNum"
        /// to access the color space information corresponding to the interioir color.</remarks>
        public void SetInteriorColor(ColorPt c, int CompNum)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotSetInteriorColor(mp_annot, c.mp_colorpt, CompNum));
        }

		/// <summary> Gets the inner bounding rectangle of the Square.
		/// (Optional; PDF 1.5)
		/// 
		/// </summary>
		/// <returns> A difference between the inner bounding rectangle and a positon rectangle
		/// may occur in situations where a border effect (described by BE)
		/// causes the size of the Rect to increase beyond that of the square or Square
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Rect GetContentRect()
        {
            BasicTypes.TRN_Rect result = new BasicTypes.TRN_Rect();
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotGetContentRect(mp_annot, ref result));
            return new Rect(result);
        }

		/// <summary> Sets the inner bounding rectangle of the Square.
		/// (Optional; PDF 1.5)
		/// 
		/// </summary>
		/// <param name="cr">the new content rect
		/// </param>
		/// <returns> cr A difference between the inner bounding rectangle and a positon rectangle
		/// may occur in situations where a border effect (described by BE)
		/// causes the size of the Rect to increase beyond that of the square or Square
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetContentRect(Rect cr)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotSetContentRect(mp_annot, ref cr.mp_imp));
        }

		/// <summary> Gets the rectangle difference of the Square.
		/// (Optional; PDF 1.5)
		/// 
		/// </summary>
		/// <returns> A set of four numbers(represented as a Rect object) specifying the difference on the four different directions.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  rectangle difference is A set of four numbers that shall describe the numerical differences
		/// between two rectangles: the Rect entry of the annotation and the actual boundaries of the underlying
		/// square or Square. Such a difference may occur in situations where a border effect (described by BE)
		/// causes the size of the Rect to increase beyond that of the square or Square.
		/// The four numbers shall correspond to the differences in default user space between the left, top, right,
		/// and bottom coordinates of Rect and those of the square or Square, respectively. Each value shall be greater
        /// than or equal to 0. The sum of the top and bottom differences shall be less than the height of Rect, and the
        /// sum of the left and right differences shall be less than the width of Rect.</remarks>
        public Rect GetPadding()
        {
            BasicTypes.TRN_Rect result = new BasicTypes.TRN_Rect();
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotGetPadding(mp_annot, ref result));
            return new Rect(result);
        }

		/// <summary> Sets the rectangle difference of the Square.
		/// (Optional; PDF 1.5)
		/// 
		/// </summary>
		/// <param name="rd">A set of four numbers(represented as a Rect object) specifying the difference on the four different directions.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  rectangle difference is A set of four numbers that shall describe the numerical differences 
		/// between two rectangles: the Rect entry of the annotation and the actual boundaries of the underlying
		/// square or Square. Such a difference may occur in situations where a border effect (described by BE)
		/// causes the size of the Rect to increase beyond that of the square or Square.
		/// The four numbers shall correspond to the differences in default user space between the left, top, right,
		/// and bottom coordinates of Rect and those of the square or Square, respectively. Each value shall be greater
		/// than or equal to 0. The sum of the top and bottom differences shall be less than the height of Rect, and the
        /// sum of the left and right differences shall be less than the width of Rect.
        /// </remarks>
        public void SetPadding(Rect rd)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotSetPadding(mp_annot, ref rd.mp_imp));
        }

        /// <summary> Rotates the appearance of the Markup Annotation.
        /// 
        /// </summary>
        /// <param name="angle">the new rotation
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks> Apply a rotation to an existing appearance.
        /// This rotation will be reflected in the bounding rect of the annot (which 
        /// will be updated), but not in any other part of the annotation dictionary.
        /// This will effectively create a custom appearance for the annotation, 
        /// and any subsequent calls to `RefreshAppearance` will clear this transformation.
        /// </remarks>
        public void RotateAppearance(Double angle)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotRotateAppearance(mp_annot, angle));
        }


        /// <summary> Creates a Markup annotation and initialize it using given annotation object.
        /// 
        /// </summary>
        /// <param name="ann">the annot
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public Markup(Annot ann) : base(ann.GetSDFObj()) { }

        internal Markup(TRN_Annot markup, Object reference) : base(markup, reference) { }
        
        /// <summary> Releases all resources used by the Annot </summary>
        ~Markup()
        {
            Dispose(false);
        }

        // Nested Types
        ///<summary>Markup border effects</summary>
        public enum BorderEffect
        {
            ///<summary>No effect: the border shall be as described by the annotation
			/// dictionary’s BorderStyle(BS) entry.</summary>
			e_None,
			///<summary>The border should appear “cloudy”. The width and
			/// dash array specified by BS shall be honored.</summary>
			e_Cloudy
        }

    }
}