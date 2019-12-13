using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> A watermark annotation (PDF 1.6) shall be used to represent graphics 
    /// that shall be printed at a fixed size and position on a page, 
    /// regardless of the dimensions of the printed page. The FixedPrint entry 
    /// of a watermark annotation dictionary shall be a dictionary that 
    /// contains values for specifying the size and position of the annotation.
    /// </summary>
    public class Watermark : Annot
    {
        internal Watermark(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Watermark annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public Watermark(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_WatermarkAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		/// <summary> Creates a new Watermark annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the Popup annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the Popup annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank Watermark annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Watermark Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_WatermarkAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Watermark(result, doc);
        }
		/// <summary> Creates a Watermark annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public Watermark(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Releases all resources used by the Watermark </summary>
		~Watermark()
        {
            Dispose(false);
        }
    }
}