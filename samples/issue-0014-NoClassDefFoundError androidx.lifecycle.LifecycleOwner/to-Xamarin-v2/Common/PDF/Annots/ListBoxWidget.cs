using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Vector = System.IntPtr;
using TRN_UString = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> 
    /// An object representing a List Box used in a PDF Form.
    /// </summary>
    public class ListBoxWidget : Widget
    {

        internal ListBoxWidget(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a List Box Widget annotation and initialize it using given Cos/SDF object.
        /// </summary>
        /// <param name="d">the object to use to initialize the List Box Widget
        /// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public ListBoxWidget(SDF.Obj d) : base(d) { }

        /// <summary> Creates a List Box Widget annotation and initialize it using given annotation object.
        /// </summary>
        /// <param name="ann"> The annotation to use.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public ListBoxWidget(Annot ann) : base(ann.GetSDFObj()) { }

        /// <summary> Creates a new List Box Widget annotation, in the specified document.
        /// </summary>
        /// <param name="doc">A document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.
        /// </param>
        /// <param name="field_name">The name of the field for which to create a List Box Widget
        /// </param>
        /// <returns> A newly created blank List Box Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ListBoxWidget Create(PDFDoc doc, Rect pos, string field_name)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ListBoxWidgetCreate(doc.mp_doc, ref pos.mp_imp, UString.ConvertToUString(field_name).mp_impl, ref result));
            return new ListBoxWidget(result, doc);
        }

        /// <summary> Creates a new List Box Widget annotation, in the specified document with a default field name.
        /// </summary>
        /// <param name="doc">A document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.</param>
        /// <returns> A newly created blank List Box Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ListBoxWidget Create(PDFDoc doc, Rect pos)
        {
            return Create(doc, pos, "");
        }

        /// <summary> Creates a new List Box Widget annotation, in the specified document.
        /// </summary>
        /// <param name="doc">The document to which the annotation is added.
        /// </param>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.
        /// </param>
        /// <param name="field">the field for which to create a List Box Widget
        /// </param>
        /// <returns> A newly created blank List Box Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ListBoxWidget Create(PDFDoc doc, Rect pos, Field field)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ListBoxWidgetCreateWithField(doc.mp_doc, ref pos.mp_imp, ref field.mp_field, ref result));
            return new ListBoxWidget(result, doc);
        }

        /// <summary> Adds option to List Box Widget.
        /// </summary>
        /// <param name="option"> The option to add
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void AddOption(string option)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ListBoxWidgetAddOption(mp_annot, UString.ConvertToUString(option).mp_impl));
        }

        /// <summary> Adds multiple options to List Box Widget.
        /// </summary>
        /// <param name="opts"> The options to add.
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
                PDFNetException.REX(PDFNetPINVOKE.TRN_ListBoxWidgetAddOptions(mp_annot, pnt, new IntPtr(ustringOpts.Length)));
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }

        /// <summary> Sets the option for the List Box Widget.
        /// </summary>
        /// <param name="opts"> The options to select.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetSelectedOptions(string[] opts)
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
                PDFNetException.REX(PDFNetPINVOKE.TRN_ListBoxWidgetSetSelectedOptions(mp_annot, pnt, new IntPtr(ustringOpts.Length)));
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }

        /// <summary> Gets all selected options in the List Box widget.
        /// </summary>
        /// <returns> The seletcted option of the List Box widget
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string[] GetSelectedOptions()
        {
            TRN_Vector options_vec = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ListBoxWidgetGetSelectedOptions(mp_annot, ref options_vec));

            IntPtr options_vec_size_ptr = IntPtr.Zero;
            PDFNetPINVOKE.TRN_VectorGetSize(options_vec, ref options_vec_size_ptr);
            int options_vec_size = options_vec_size_ptr.ToInt32();

            string[] result = new string[options_vec_size];
            for (int i=0;i< options_vec_size;i++)
            {
                TRN_UString current_ustr = IntPtr.Zero;
                PDFNetPINVOKE.TRN_VectorGetAt(options_vec, new IntPtr(i), ref current_ustr);
                result[i] = (new UString(current_ustr)).ConvToManagedStr();
            }
            PDFNetPINVOKE.TRN_VectorDestroyKeepContents(options_vec);
            return result;
        }
    }
}