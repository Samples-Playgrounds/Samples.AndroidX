using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> 
    /// An object representing a check box used in a PDF Form.
    /// </summary>
    public class CheckBoxWidget : Widget
    {

        internal CheckBoxWidget(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Check Box Widget annotation and initialize it using given Cos/SDF object.
        /// </summary>
        /// <param name="d">the object to use to initialize the Check Box Widget
        /// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public CheckBoxWidget(SDF.Obj d) : base(d) { }

        /// <summary> Creates a Check Box Widget annotation and initialize it using given annotation object.
        /// 
        /// </summary>
        /// <param name="ann">the annot
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public CheckBoxWidget(Annot ann) : base(ann.GetSDFObj()) { }

        /// <summary> Creates a new Check Box Widget annotation, in the specified document.
        /// 
        /// </summary>
        /// <param name="doc">The document to which the annotation is to be added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.
        /// </param>
        /// <param name="field_name">The name of the field for which to create a CheckBox widget
        /// </param>
        /// <returns> A newly created blank Check Box Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static CheckBoxWidget Create(PDFDoc doc, Rect pos, string field_name)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_CheckBoxWidgetCreate(doc.mp_doc, ref pos.mp_imp, UString.ConvertToUString(field_name).mp_impl, ref result));
            return new CheckBoxWidget(result, doc);
        }

        /// <summary> Creates a new Check Box Widget annotation, in the specified document with a default field name.
        /// 
        /// </summary>
        /// <param name="doc">The document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.</param>
        /// <returns> A newly created blank Check Box Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static CheckBoxWidget Create(PDFDoc doc, Rect pos)
        {
            return Create(doc, pos, "");
        }

        /// <summary> Creates a new Widget annotation, in the specified document.
        /// 
        /// </summary>
        /// <param name="doc">A document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.
        /// </param>
        /// <param name="field">the field for which to create a CheckBox widget
        /// </param>
        /// <returns> A newly created blank Check Box Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static CheckBoxWidget Create(PDFDoc doc, Rect pos, Field field)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_CheckBoxWidgetCreateWithField(doc.mp_doc, ref pos.mp_imp, ref field.mp_field, ref result));
            return new CheckBoxWidget(result, doc);
        }

        /// <summary> Returns whether the checkbox is checked.
        /// 
        /// </summary>
        /// <returns> A boolean value indicating whether the checkbox is checked.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsChecked()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_CheckBoxWidgetIsChecked(mp_annot, ref result));
            return result;
        }

        /// <summary> Check or uncheck the Check Box Widget
        /// 
        /// </summary>
        /// <param name="value">If true, the annotation should be checked. Otherwise it should be unchecked.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetChecked(bool isChecked)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_CheckBoxWidgetSetChecked(mp_annot, isChecked));
        }

    }
}