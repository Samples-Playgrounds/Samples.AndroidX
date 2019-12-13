using System;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> A pop-up annotation (PDF 1.3) displays text in a pop-up window for entry and 
    /// editing. It shall not appear alone but is associated with a markup annotation, 
    /// its parent annotation, and shall be used for editing the parent’s text. 
    /// It shall have no appearance stream or associated actions of its own and 
    /// shall be identified by the Popup entry in the parent’s annotation dictionary.
    /// </summary>
    public class Popup : Annot
    {
        /// <summary> Creates a Popup annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public Popup(SDF.Obj d) : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PopupAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		/// <summary> Creates a new Popup annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the Popup annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the Popup annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank Popup annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Popup Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PopupAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Popup(result, doc);
        }

		/// <summary> Gets the Parent annotation of the Popup annotation.
		/// 
		/// </summary>
		/// <returns> An annot object which is the parent annotation of the Popup annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  This annotation object represents the parent annotation with which this
		/// pop-up annotation shall be associated.
        /// If this entry is present, the parent annotation’s Contents, M, C, and Tentries
        /// shall override those of the pop-up annotation itself.</remarks>
        public Annot GetParent()
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PopupAnnotGetParent(mp_annot, ref result));
            return result != IntPtr.Zero ? new Annot(result, GetRefHandleInternal()) : null;
        }

		/// <summary> Sets the Parent annotation of the Popup annotation.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="parent">An annot object which is the parent annotation of the Popup annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  This annotation object represents the parent annotation with which this
		/// pop-up annotation shall be associated.
        /// If this entry is present, the parent annotation’s Contents, M, C, and Tentries
        /// shall override those of the pop-up annotation itself.</remarks>
        public void SetParent(Annot parent)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PopupAnnotSetParent(mp_annot, parent.mp_annot));
        }

		/// <summary> Gets the initial openning condition of Popup.
		/// 
		/// </summary>
		/// <returns> A bool indicating whether the Popup is initially open.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks> This Open option is a flag specifying whether the pop-up 
		/// annotation shall initially be displayed open.
		/// Default value: false (closed).
        /// If this entry is present, the parent annotation’s Contents, M, C, and Tentries
        /// shall override those of the pop-up annotation itself.</remarks>
        public bool IsOpen()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PopupAnnotIsOpen(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the initial openning condition of Popup.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="is_open">A bool indicating whether the Popup is initially open.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  This Open option is a flag specifying whether the pop-up 
		/// annotation shall initially be displayed open.
		/// Default value: false (closed).
		/// If this entry is present, the parent annotation’s Contents, M, C, and Tentries
        /// shall override those of the pop-up annotation itself.
        /// </remarks>
        public void SetOpen(bool is_open)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PopupAnnotSetOpen(mp_annot, is_open));
        }

        internal Popup(TRN_Annot imp, Object reference) :base(imp, reference){}

		/// <summary> Creates a Popup annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public Popup(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Releases all resources used by the Popup </summary>
		~Popup() 
        {
            Dispose(false);
        }
    }
}