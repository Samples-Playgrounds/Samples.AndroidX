using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> Square and circle annotations (PDF 1.3) shall display, 
    /// respectively, a rectangle or an ellipse on the page. When opened, 
    /// they shall display a pop-up window containing the text of the 
    /// associated note. The rectangle or ellipse shall be inscribed within 
    /// the annotation rectangle defined by the annotation dictionary’s Rect 
    /// entry
    /// </summary>
    public class Square : Markup
    {
        internal Square(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Square annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public Square(SDF.Obj d) : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SquareAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }	 

		/// <summary> Creates a new Square annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank Square annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Square Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SquareAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Square(result, doc);
        }

		/// <summary> Creates a Square annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public Square(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Releases all resources used by the Square </summary>
        ~Square()
        {
            Dispose(false);
        }
    }
}