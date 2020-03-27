using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    ///<summary>Polygon annotation</summary>
    public class Polygon : PolyLine
    {
        internal Polygon(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Polygon annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical 
        /// equivalent of a type cast.</remarks>
        public Polygon(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PolygonAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		/// <summary> Creates a new Polygon annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the Polygon annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the PolyLine annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank PolyLine annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Polygon Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PolygonAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Polygon(result, doc);
        }

		/// <summary> Creates a Polygon annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public Polygon(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Releases all resources used by the Polygon </summary>
		~Polygon()
        {
            Dispose(false);
        }
    }
}