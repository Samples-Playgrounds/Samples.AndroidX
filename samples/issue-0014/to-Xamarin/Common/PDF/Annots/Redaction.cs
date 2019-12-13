using System;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> A redaction annotation (PDF 1.7) identifies content that is intended to 
    /// be removed from the document. The intent of redaction annotations is to 
    /// enable the following process:
    /// <list type="numbered">
    /// <item><description>
    /// a)Content identification. A user applies redact annotations that specify 
    /// the pieces or regions of content that should be removed. Up until the 
    /// next step is performed, the user can see, move and redefine these 
    /// annotations.
    /// </description></item>
    /// <item><description>
    /// b)Content removal. The user instructs the viewer application to apply 
    /// the redact annotations, after which the content in the area specified 
    /// by the redact annotations is removed. In the removed content’s place, 
    /// some marking appears to indicate the area has been redacted. Also, the 
    /// redact annotations are removed from the PDF document.
    /// </description></item>
    /// </list>
    /// <para>
    /// Redaction annotations provide a mechanism for the first step in the 
    /// redaction process (content identification). This allows content to be 
    /// marked for redaction in a non-destructive way, thus enabling a review 
    /// process for evaluating potential redactions prior to removing the 
    /// specified content.    
    /// </para>
    /// <para>
    /// Redaction annotations shall provide enough information to be used 
    /// in the second phase of the redaction process (content removal). 
    /// This phase is application-specific and requires the conforming reader 
    /// to remove all content identified by the redaction annotation, as well 
    /// as the annotation itself.
    /// </para>
    /// <para>
    /// Conforming readers that support redaction annotations shall provide 
    /// a mechanism for applying content removal, and they shall remove all 
    /// traces of the specified content. If a portion of an image is contained 
    /// in a redaction region, that portion of the image data shall be destroyed; 
    /// clipping or image masks shall not be used to hide that data. 
    /// Such conforming readers shall also be diligent in their consideration 
    /// of all content that can exist in a PDF document, including XML Forms 
    /// Architecture (XFA) content and Extensible Metadata Platform (XMP) 
    /// content.
    /// </para>
    /// </summary>
    public class Redaction : Markup
    {
        /// <summary> Creates an Redaction annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
		public Redaction(SDF.Obj d) : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		/// <summary> Creates a new Redaction annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank Circle annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public static Redaction Create(SDF.SDFDoc doc, Rect pos) 
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Redaction(result, doc);
        }

		/// <summary> Gets the number of QuadPoints in the QuadPoints array of the Redaction annotation.
		/// 
		/// </summary>
		/// <returns> The number of QuadPoints.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  An array of n QuadPoints specifying If present, these quadrilaterals denote 
		/// the content region that is intended to be removed. If this entry is not present,
		/// the Rect entry denotes the content region that is intended to be removed</remarks>
		public int GetQuadPointCount() 
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotGetQuadPointCount(mp_annot, ref result));
            return result;
        }

		/// <summary> Gets the QuadPoint located at a certain index of the QuadPoint array of the Redaction
		/// annotation.
		/// 
		/// </summary>
		/// <param name="idx">The index of where the QuadPoint of interest is located.
		/// </param>
		/// <returns> The QuadPoint located at a certain index of the QuadPoint array of the Redaction annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  An array of n QuadPoints specifying If present, these quadrilaterals denote
		/// the content region that is intended to be removed. If this entry is not present,
		/// the Rect entry denotes the content region that is intended to be removed</remarks>
		public QuadPoint GetQuadPoint(int idx) 
        {
            BasicTypes.TRN_QuadPoint result = new BasicTypes.TRN_QuadPoint();
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotGetQuadPoint(mp_annot, idx, ref result));
            return new QuadPoint(result);
        }

		/// <summary> Set the QuadPoint to be located at a certain index of the QuadPoint array of the Redaction
		/// annotation.
		/// (Optional; PDF 1.6 )
		/// 
		/// </summary>
		/// <param name="idx">The index position where the QuadPoint of interest is to be inserted.
		/// </param>
		/// <param name="pt"> The QuadPoint to be inserted at that position.
		/// </param>
		/// <returns> The QuadPoint to be located at a certain index of the QuadPoint array of the Redaction annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  An array of n QuadPoints specifying the coordinates of n quadrilaterals
		/// in default user space that comprise the region in which the link should be activated.
		/// The coordinates specifying the four vertices of the quadrilateral in counterclockwise order.
		/// For orientation purposes, such as when applying an underline border style, the bottom of a
		/// quadrilateral is the line formed by p1 and p2 of the QuadPoint.
		/// QuadPoints shall be ignored if any coordinate in the array lies outside the region specified by Rect.</remarks>
		public void SetQuadPoint(int idx, QuadPoint pt) //Note the default should be just a single rect that is equal to Rect entry. 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotSetQuadPoint(mp_annot, idx, ref pt.mp_imp));
        }

		/// <summary> Gets Overlay appearance of the Redaction annotation.
		/// 
		/// </summary>
		/// <returns> An SDF object that represents the overlay overlay appearance of the Redaction annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  This overlay overlay appearance object is a form XObject specifying the overlay appearance for this
		/// redaction annotation. After this redaction is applied and the affected content has been removed,
		/// the overlay appearance should be drawn such that its origin lines up with the lower-left corner
		/// of the annotation rectangle. This form XObject is not necessarily related to other annotation
		/// appearances, and may or may not be present in the Appearance dictionary. This entry takes precedence over the Interior Color(IC),
		/// OverlayText, OverlayTextAppearance(DA), and QuadPoint(Q) entries.</remarks>
		public SDF.Obj GetAppFormXO() 
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotGetAppFormXO(mp_annot, ref result));
            return new SDF.Obj(result, GetRefHandleInternal());
        }

		/// <summary> Sets Overlay appearance of the Redaction annotation.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="formxo">An SDF object that represents the overlay appearance of the Redaction annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  This overlay appearance object is a form XObject specifying the overlay appearance for this
		/// redaction annotation. After this redaction is applied and the affected content has been removed,
		/// the overlay appearance should be drawn such that its origin lines up with the lower-left corner
		/// of the annotation rectangle. This form XObject is not necessarily related to other annotation
		/// appearances, and may or may not be present in the Appearance dictionary. This entry takes precedence over the Interior Color(IC),
		/// OverlayText, OverlayTextAppearance(DA), and QuadPoint(Q) entries.</remarks>
		public void SetAppFormXO(SDF.Obj formxo) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotSetAppFormXO(mp_annot, formxo.mp_obj));
        }

		/// <summary> Gets Overlay text of the Redaction annotation.
		/// 
		/// </summary>
		/// <returns> A string containing the overlay text of the annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The OverlayText string is a text string specifying the
		/// overlay text that should be drawn over the redacted region
		/// after the affected content has been removed.
		/// This entry is ignored if the Overlay appearance(RO) entry is present.</remarks>
		public string GetOverlayText() 
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotGetOverlayText(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets Overlay text of the Redaction annotation.
		/// 
		/// </summary>
		/// <param name="title">A string containing the overlay text of the annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The OverlayText string is a text string specifying the
		/// overlay text that should be drawn over the redacted region
		/// after the affected content has been removed.
		/// This entry is ignored if the Overlay appearance(RO) entry is present.</remarks>
		public void SetOverlayText(string title) 
        {
            UString str = new UString(title);
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotSetOverlayText(mp_annot, str.mp_impl));
        }

		/// <summary> Gets the option of whether to use repeat for the Redaction annotation.
		/// 
		/// </summary>
		/// <returns> A bool indicating whether to repeat for the Redaction annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If UseRepeat value is true, then the text specified by OverlayText
		/// should be repeated to fill the redacted region after the affected content
		/// has been removed. This entry is ignored if the RO entry is present.
		/// Default value: false.</remarks>
		public bool GetUseRepeat() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotGetUseRepeat(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the option of whether to use repeat for the Redaction annotation.
		/// 
		/// </summary>
		/// <param name="repeat">the new use repeat
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If UseRepeat value is true, then the text specified by OverlayText
		/// should be repeated to fill the redacted region after the affected content
		/// has been removed. This entry is ignored if the RO entry is present.
		/// Default value: false.</remarks>
		public void SetUseRepeat(Boolean repeat) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotSetUseRepeat(mp_annot, repeat));
        }
		/// <summary>Sets the option of whether to use repeat for the Redaction annotation to false.</summary>
		public void SetUseRepeat() 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotSetUseRepeat(mp_annot, false));
        }

		/// <summary> Gets Overlay text appearance of the Redaction annotation.
		/// 
		/// </summary>
		/// <returns> A string containing the overlay text appearance of the annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The overlay text appearance is the appearance string to be used in formatting the overlay text
		/// when it is drawn after the affected content has been removed. This entry is ignored
		/// if the Overlay appearance strint(RO) entry is present.</remarks>
		public string GetOverlayTextAppearance() 
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotGetOverlayTextAppearance(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets Overlay text appearance of the Redaction annotation.
		/// 
		/// </summary>
		/// <param name="app">A string containing the overlay text appearance of the annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The overlay text appearance is the appearance string to be used in formatting the overlay text
		/// when it is drawn after the affected content has been removed. This entry is ignored
		/// if the Overlay appearance strint(RO) entry is present.</remarks>
		public void SetOverlayTextAppearance(string app) 
        {
            UString str = new UString(app);
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotSetOverlayTextAppearance(mp_annot, str.mp_impl));
        }

        /// <summary> Gets Overlay text quadding(justification) format of the Redaction annotation.
		/// 
		/// </summary>
		/// <returns> An entry of the "QuadForm" enum, indicating the
		/// overlay text quadding(justification) format of the Redaction annotation .
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  quadding format specifies the form of quadding (justification) to be
		/// used in laying out the overlay</remarks>
		public QuadForm GetQuadForm() 
        {
            QuadForm result = QuadForm.e_None;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotGetQuadForm(mp_annot, ref result));
            return result;
        }

        /// <summary> Sets Overlay text quadding(justification) format of the Redaction annotation.
		/// 
		/// </summary>
		/// <param name="form">An entry of the "QuadForm" enum, indicating the
		/// overlay text quadding(justification) format of the Redaction annotation .
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Quadding format specifies the form of quadding (justification) to be 
		/// used in laying out the overlay</remarks>
		public void SetQuadForm(QuadForm form) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAnnotSetQuadForm(mp_annot, form));
        }
		/// <summary>Sets Overlay text quadding(justification) format of the Redaction annotation to e_LeftJustified
		/// </summary>
		public void SetQuadForm() 
        {
            SetQuadForm(QuadForm.e_LeftJustified);
        }
		/// <summary> Releases all resources used by the Redaction </summary>

        internal Redaction(TRN_Annot imp, Object reference) : base(imp, reference){}

		/// <summary> Creates an Redaction annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical 
		/// equivalent of a type cast.</remarks>
        public Redaction(Annot ann) : base(ann.GetSDFObj()) { }
		
		~Redaction() 
        {
            Dispose(false);
        }

        // Nested Types
        ///<summary> This integer list represents the Quad Format of the Redaction annotation</summary>
		public enum QuadForm
        {
            ///<summary>Overlay text is left justified</summary>
			e_LeftJustified,
			///<summary>Overlay text is centered</summary>
			e_Centered,
			///<summary>Overlay text is right justified</summary>
			e_RightJustified,
			///<summary>No justification to the overlay text</summary>
			e_None
        }

    }
}