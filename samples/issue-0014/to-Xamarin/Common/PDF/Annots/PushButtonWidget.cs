using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> 
    /// An object representing a push button used in a PDF Form.
    /// </summary>
    public class PushButtonWidget : Widget
    {

        internal PushButtonWidget(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Push Button Widget annotation and initialize it using given Cos/SDF object.
        /// </summary>
        /// <param name="d">The object to use to initialize the PushButtonWidget
        /// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public PushButtonWidget(SDF.Obj d) : base(d) { }

        /// <summary> Creates a Push Button Widget annotation and initialize it using given annotation object.
        /// 
        /// </summary>
        /// <param name="ann"> the annotation from which to create a Push Button Widget
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public PushButtonWidget(Annot ann) : base(ann.GetSDFObj()) { }

        /// <summary> Creates a new Push Button Widget annotation, in the specified document.
        /// 
        /// </summary>
        /// <param name="doc">The document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.
        /// </param>
        /// <param name="field_name">The name of the field for which to create a PushButton widget
        /// </param>
        /// <returns> A newly created default Push Button Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static PushButtonWidget Create(PDFDoc doc, Rect pos, string field_name)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PushButtonWidgetCreate(doc.mp_doc, ref pos.mp_imp, UString.ConvertToUString(field_name).mp_impl, ref result));
            return new PushButtonWidget(result, doc);
        }

        /// <summary> Creates a new PushButton widget annotation, in the specified document.
        /// 
        /// </summary>
        /// <param name="doc">The document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.</param>
        /// <returns> A newly created default Push Button Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static PushButtonWidget Create(PDFDoc doc, Rect pos)
        {
            return Create(doc, pos, "");
        }

        /// <summary> Creates a new Push Button Widget annotation, in the specified document.
        /// 
        /// </summary>
        /// <param name="doc">A document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.
        /// </param>
        /// <param name="field">the field for which to create a Push Button Widget
        /// </param>
        /// <returns> A newly created default Push Button Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static PushButtonWidget Create(PDFDoc doc, Rect pos, Field field)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PushButtonWidgetCreateWithField(doc.mp_doc, ref pos.mp_imp, ref field.mp_field, ref result));
            return new PushButtonWidget(result, doc);
        }

    }
}