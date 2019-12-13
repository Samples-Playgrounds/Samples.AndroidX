using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> 
    /// Square and circle annotations (PDF 1.3) shall display, 
    /// respectively, a rectangle or an ellipse on the page. When opened, 
    /// they shall display a pop-up window containing the text of the 
    /// associated note. The rectangle or ellipse shall be inscribed within 
    /// the annotation rectangle defined by the annotation dictionary’s Rect 
    /// entry
    /// </summary>
    public class Circle : Markup
    {
        internal Circle(TRN_Annot imp, Object reference) : base(imp, reference) { }

        //Circle(SDF.Obj d);
		/// <summary> Creates an Circle annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical 
        ///  equivalent of a type cast.</remarks>
        public Circle(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_CircleAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		//static Circle Create(SDF.SDFDoc& doc, Rect& pos);
		/// <summary> Creates a new Circle annotation, in the specified document.
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
        public static Circle Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_CircleAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Circle(result, doc);
        }

		/// <summary> Creates a Circle annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical 
		/// equivalent of a type cast.</remarks>
        public Circle(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Releases all resources used by the Circle </summary>
		~Circle()
        {
            Dispose(false);
        }
    }
}