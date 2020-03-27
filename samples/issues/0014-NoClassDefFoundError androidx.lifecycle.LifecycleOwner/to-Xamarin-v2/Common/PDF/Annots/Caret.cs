using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Exception = System.IntPtr;
using TRN_Annot = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> 
	/// A Caret annotation (PDF 1.5) is a visual symbol that indicates 
	/// the presence of text edits.
	/// </summary>
	public class Caret : Markup
    {
        //Caret(SDF::Obj^ d);
        /// <summary> Creates an Caret annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical 
        /// equivalent of a type cast.</remarks>
        public Caret(SDF.Obj d) : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_CaretAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		//Caret(Annot& ann) : Markup(ann.GetSDFObj()) {} 

		//static Caret Create(SDF.SDFDoc& doc, Rect& pos);
		/// <summary> Creates an Caret annotation and initialize it using given annotation object.
		/// </summary>
		/// <param name="doc">SDFDoc to create Caret in
		/// </param>
		/// <param name="pos">position of the caret
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <returns>newly created Caret
        /// </returns>
        public static Caret Create(SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_CaretAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Caret(result, doc);
        }

        public static Caret Create(SDFDoc doc, Rect pos, Rect padding)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_CaretAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            PDFNetException.REX(PDFNetPINVOKE.TRN_MarkupAnnotSetPadding(result, ref padding.mp_imp));
            return new Caret(result, doc);
        }


		//String GetSymbol() ;
		/// <summary> Gets the paragraph symbol displayed along with the Caret.
		/// 
		/// </summary>
		/// <returns> A pointer to an array of charactors that specifies the Caret content.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the content of the Caret Annotation is a name specifying a symbol 
		///  that shall be associated with the Caret: P - A new paragraph symbol (¶)
		/// should be associated with the Caret.
        /// None - No symbol should be associated with the Caret.
        /// Default value: None.</remarks>
        public string GetSymbol()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_CaretAnnotGetSymbol(mp_annot, ref result));
            return Marshal.PtrToStringUTF8(result);
        }

		//void SetSymbol(String contt);
		/// <summary> Sets the paragraph symbol displayed along with the Caret.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="contt">A pointer to an array of charactors that specifies the Caret content.
		/// specifies the Caret content.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the content of the Caret Annotation is a name specifying a symbol
		/// that shall be associated with the Caret: P - A new paragraph symbol (¶)
		/// should be associated with the Caret.
		/// None - No symbol should be associated with the Caret.
		/// Default value: None.</remarks>
        public void SetSymbol(string contt)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_CaretAnnotSetSymbol(mp_annot, contt));
        }
		/// <summary> Creates an Caret annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical 
		/// equivalent of a type cast.</remarks>
        public Caret(Annot ann) : base(ann.GetSDFObj()) { }

        internal Caret(TRN_Annot markup, Object reference) : base(markup, reference) { }

		/// <summary> Releases all resources used by the Caret </summary>
		~Caret()
        {
            Dispose(false);
        }
    }
}