using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> 
    /// An object representing a combo box used in a PDF Form.
    /// </summary>
    public class ComboBoxWidget : Widget
    {

        internal ComboBoxWidget(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Combo Box Widget annotation and initialize it using given Cos/SDF object.
        /// </summary>
        /// <param name="d">the object to use to initialize the ComboBoxWidget
        /// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public ComboBoxWidget(SDF.Obj d) : base(d) { }

        /// <summary> Creates a Widget annotation and initialize it using given annotation object.
        /// </summary>
        /// <param name="ann"> The annotation object to use.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public ComboBoxWidget(Annot ann) : base(ann.GetSDFObj()) { }

        /// <summary> Creates a new Combo Box Widget annotation, in the specified document.
        /// </summary>
        /// <param name="doc">The document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.
        /// </param>
        /// <param name="field_name">The name of the field for which to create a ComboBox widget
        /// </param>
        /// <returns> A newly created blank Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ComboBoxWidget Create(PDFDoc doc, Rect pos, string field_name)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ComboBoxWidgetCreate(doc.mp_doc, ref pos.mp_imp, UString.ConvertToUString(field_name).mp_impl, ref result));
            return new ComboBoxWidget(result, doc);
        }

        /// <summary> Creates a new Combo Box Widget annotation, in the specified document with a default Field name.
        /// </summary>
        /// <param name="doc">The document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.</param>
        /// <returns> A newly created blank Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ComboBoxWidget Create(PDFDoc doc, Rect pos)
        {
            return Create(doc, pos, "");
        }

        /// <summary> Creates a new Combo Box Widget annotation, in the specified document.
        /// </summary>
        /// <param name="doc">The document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.
        /// </param>
        /// <param name="field"> The field for which to create a Text widget
        /// </param>
        /// <returns> A newly created blank Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ComboBoxWidget Create(PDFDoc doc, Rect pos, Field field)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ComboBoxWidgetCreateWithField(doc.mp_doc, ref pos.mp_imp, ref field.mp_field, ref result));
            return new ComboBoxWidget(result, doc);
        }

        /// <summary> Add an option to Combo Box widget.
        /// </summary>
        /// <param name="option"> The option to add
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void AddOption(string option)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ComboBoxWidgetAddOption(mp_annot, UString.ConvertToUString(option).mp_impl));
        }

        /// <summary> Adds multiple options to Combo Box widget.
        /// </summary>
        /// <param name="opts"> The options to add
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void AddOptions(string[] opts)
        {
            // conv to ustring
            IntPtr[] ustringOpts = new IntPtr[opts.Length];
            for (int i = 0; i < opts.Length; i++)
            {
                ustringOpts[i] = UString.ConvertToUString(opts[i]).mp_impl;
            }
            int psize = Marshal.SizeOf(ustringOpts[0]) * ustringOpts.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(ustringOpts, 0, pnt, ustringOpts.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ComboBoxWidgetAddOptions(mp_annot, pnt, new IntPtr(ustringOpts.Length)));
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }

        /// <summary> Selects the given option in the Combo Box widget
        /// </summary>
        /// <param name="option"> The option to select
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetSelectedOption(string option)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ComboBoxWidgetSetSelectedOption(mp_annot, UString.ConvertToUString(option).mp_impl));
        }

        /// <summary> Retrieves the option selected in the ComboBox widget
        /// </summary>
        /// <returns> The option selected in the ComboBox widget
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetSelectedOption()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ComboBoxWidgetGetSelectedOption(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

    }
}