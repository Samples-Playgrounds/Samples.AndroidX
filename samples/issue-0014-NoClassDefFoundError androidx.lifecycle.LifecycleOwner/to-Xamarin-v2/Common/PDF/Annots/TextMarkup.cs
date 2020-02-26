using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> Text markup annotations shall appear as highlights, underlines, 
    /// strikeouts (all PDF 1.3), or jagged (“squiggly”) underlines (PDF 1.4) 
    /// in the text of a document. When opened, they shall display a pop-up 
    /// window containing the text of the associated note.
    /// </summary>
    public class TextMarkup : Markup
    {
        internal TextMarkup(TRN_Annot imp, Object reference) : base(imp, reference) { }

        public TextMarkup(Annot ann) : base(ann.GetSDFObj()) { }

        /// <summary> Creates a TextMarkup annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public TextMarkup(SDF.Obj d) : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextMarkupAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		/// <summary> Gets the number of QuadPoints in the QuadPoints array of the TextMarkup annotation.
		/// 
		/// </summary>
		/// <returns> The number of QuadPoints.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  An array of n QuadPoints specifying the coordinates of nquadrilaterals in default user space. Each quadrilateral shall
		/// encompasses a word or group of contiguous words in the text underlying the annotation. The coordinates for each quadrilateral
		/// shall be given in the order p1, p2, p3, p4 specifying the quadrilateral’s four vertices in counterclockwise order.
		/// The text shall be oriented with respect to the edge connecting points (p1) and (p2).
        /// The annotation dictionary’s Appearance(AP) entry, if present, shall take precedence over QuadPoints.
        /// </remarks>
        public int GetQuadPointCount()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextMarkupAnnotGetQuadPointCount(mp_annot, ref result));
            return result;
        }

		/// <summary> Gets the QuadPoint located at a certain index of the QuadPoint array of the TextMarkup
		/// annotation.
		/// 
		/// </summary>
		/// <param name="idx">The index where the QuadPoint is to be located (the index is counted from 0, and continue on assendingly).
		/// </param>
		/// <returns> The QuadPoint located at a certain index of the QuadPoint array of the TextMarkup annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  An array of n QuadPoints specifying the coordinates of nquadrilaterals in default user space. Each quadrilateral shall 
		/// encompasses a word or group of contiguous words in the text underlying the annotation. The coordinates for each quadrilateral
		/// shall be given in the order p1, p2, p3, p4 specifying the quadrilateral’s four vertices in counterclockwise order.
        /// The text shall be oriented with respect to the edge connecting points (p1) and (p2).
        /// The annotation dictionary’s Appearance(AP) entry, if present, shall take precedence over QuadPoints.</remarks>
        public QuadPoint GetQuadPoint(int idx)
        {
            BasicTypes.TRN_QuadPoint result = new BasicTypes.TRN_QuadPoint();
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextMarkupAnnotGetQuadPoint(mp_annot, idx, ref result));
            return new QuadPoint(result);
        }

		/// <summary> Sets the QuadPoint to be located at a certain index of the QuadPoint array of the TextMarkup
		/// annotation.
		/// (Optional; PDF 1.6 )
		/// 
		/// </summary>
		/// <param name="idx">The index where the QuadPoint is to be located (the index is counted from 0, and continue on assendingly).
		/// </param>
		/// <param name="qp">The QuadPoint to be located at a certain index of the QuadPoint array of the TextMarkup annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks> To make this QuadPoint compatible with Adobe Acrobat|Reader, you can use either clockwise or counterclockwise order,
        /// but the points p3 and p4 must be swapped. This is because those readers do not follow the PDF specification for TextMarkup QuadPoints.</remarks>
		/// <remarks>  An array of n QuadPoints specifying the coordinates of nquadrilaterals in default user space. Each quadrilateral shall
		/// encompasses a word or group of contiguous words in the text underlying the annotation. The coordinates for each quadrilateral
		/// shall be given in the order p1, p2, p3, p4 specifying the quadrilateral’s four vertices in counterclockwise order.
		/// The text shall be oriented with respect to the edge connecting points (p1) and (p2).
		/// The annotation dictionary’s Appearance(AP) entry, if present, shall take precedence over QuadPoints.</remarks>
        public void SetQuadPoint(int idx, QuadPoint qp)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextMarkupAnnotSetQuadPoint(mp_annot, idx, ref qp.mp_imp));
        }
		/// <summary> Releases all resources used by the TextMarkup </summary>
        ~TextMarkup()
        {
            Dispose(false);
        }
    }
}