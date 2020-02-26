using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> 
    /// An object representing a Text Box used in a PDF Form.
    /// </summary>
    public class TextWidget : Widget
    {

        internal TextWidget(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Text Widget annotation and initialize it using given Cos/SDF object.
        /// </summary>
        /// <param name="d"> The object to use to initialize the Text Widget
        /// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public TextWidget(SDF.Obj d) : base(d) { }

        /// <summary> Creates a Text Widget annotation and initialize it using given annotation object.
        /// 
        /// </summary>
        /// <param name="ann">the annot
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public TextWidget(Annot ann) : base(ann.GetSDFObj()) { }

        /// <summary> Creates a new Text Widget annotation, in the specified document.
        /// 
        /// </summary>
        /// <param name="doc">A document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.
        /// </param>
        /// <param name="field_name">The name of the field for which to create a Text widget
        /// </param>
        /// <returns> A newly created blank Text Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static TextWidget Create(PDFDoc doc, Rect pos, string field_name)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextWidgetCreate(doc.mp_doc, ref pos.mp_imp, UString.ConvertToUString(field_name).mp_impl, ref result));
            return new TextWidget(result, doc);
        }

        /// <summary> Creates a new Text Widget annotation, in the specified document.
        /// 
        /// </summary>
        /// <param name="doc">The document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.</param>
        /// <returns> A newly created blank Text Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static TextWidget Create(PDFDoc doc, Rect pos)
        {
            return Create(doc, pos, "");
        }

        /// <summary> Creates a new Text Widget annotation, in the specified document.
        /// 
        /// </summary>
        /// <param name="doc">The document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.
        /// </param>
        /// <param name="field">the field for which to create a Text Widget
        /// </param>
        /// <returns> A newly created blank Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static TextWidget Create(PDFDoc doc, Rect pos, Field field)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextWidgetCreateWithField(doc.mp_doc, ref pos.mp_imp, ref field.mp_field, ref result));
            return new TextWidget(result, doc);
        }

        /// <summary> Set the text content of the Text Widget.
        /// 
        /// </summary>
        /// <param name="text">The text tp be displayed in the Text Widget
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetText(string text)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextWidgetSetText(mp_annot, UString.ConvertToUString(text).mp_impl));
        }

        /// <summary> Retrieves the text content of the Text Widget.
        /// 
        /// </summary>
        /// <returns> The Text Widget contents
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetText()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextWidgetGetText(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

    }
}