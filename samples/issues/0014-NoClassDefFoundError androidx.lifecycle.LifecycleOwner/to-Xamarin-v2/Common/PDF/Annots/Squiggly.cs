using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> TextMarkup annotations shall appear as highlights, underlines, 
    /// strikeouts (all PDF 1.3), or jagged (“squiggly”) underlines (PDF 1.4) 
    /// in the text of a document. When opened, they shall display a pop-up 
    /// window containing the text of the associated note.
    /// </summary>
    public class Squiggly : TextMarkup
    {
        internal Squiggly(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Squiggly annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public Squiggly(SDF.Obj d) : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SquigglyAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }
		/// <summary> Creates a Squiggly annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public Squiggly(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Creates a new Squiggly annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the Popup annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the Popup annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank Squiggly annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Squiggly Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SquigglyAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Squiggly(result, doc);
        }
		/// <summary> Releases all resources used by the Squiggly </summary>
		~Squiggly()
        {
            Dispose(false);
        }
    }
}