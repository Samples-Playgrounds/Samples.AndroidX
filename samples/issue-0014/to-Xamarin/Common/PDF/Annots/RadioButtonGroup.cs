using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_RadioButtonGroup = System.IntPtr;
using TRN_RadioButtonWidget = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> 
    /// An object representing a Group of Radio Buttons that can be used to create new Radio Buttons. If a group contains multiple buttons they will be connected.
    /// </summary>
    public class RadioButtonGroup : IDisposable
    {
        // Fields
        internal TRN_RadioButtonGroup mp_impl = IntPtr.Zero;
        internal Object m_ref;

        // Methods
        internal RadioButtonGroup(TRN_RadioButtonGroup imp, Object reference)
        {
            this.mp_impl = imp;
            this.m_ref = reference;
        }
        public IntPtr GetHandleInternal()
        {
            return this.mp_impl;
        }

        public Object GetRefHandleInternal()
        {
            return this.m_ref;
        }

        /// <summary> Creates a RadioButtonGroup and initialize it using given Field object.
        /// </summary>
        /// <param name="field"> The field with which to initialize the RadioButtonGroup
        /// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public RadioButtonGroup(Field field)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RadioButtonGroupCreateFromField(ref field.mp_field, ref mp_impl));
            this.m_ref = GetRefHandleInternal();
        }

        ~RadioButtonGroup()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (disposing)
            {
                // Dispose managed resources.
            }

            // Clean up native resources
            Destroy();
        }

        public void Destroy()
        {
            if (mp_impl != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_RadioButtonGroupDestroy(mp_impl));
                mp_impl = IntPtr.Zero;
            }
        }

        /// <summary> Creates a new RadioButtonGroup in the specified document.
        /// 
        /// </summary>
        /// <param name="doc"> The document in which the RadioButtonGroup is created.
        /// </param>
        /// <param name="field_name">The name of the field to create and use in this RadioButtonGroup
        /// </param>
        /// <returns> A newly created RadioButtonGroup.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static RadioButtonGroup Create(PDFDoc doc, string field_name)
        {
            TRN_RadioButtonGroup result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RadioButtonGroupCreate(doc.mp_doc, UString.ConvertToUString(field_name).mp_impl, ref result));
            return new RadioButtonGroup(result, doc);
        }

        /// <summary> Creates a new RadioButtonGroup, in the specified document with a default field name.
        /// 
        /// </summary>
        /// <param name="doc"> The document in which the RadioButtonGroup is created.
        /// </param>
        /// <returns> A newly created blank Widget annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static RadioButtonGroup Create(PDFDoc doc)
        {
            return Create(doc, "");
        }

        /// <summary> Adds a new RadioButtonWidget to the RadioButtonGroup 
        /// </summary>
        /// <param name="pos"> A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.
        /// </param>
        /// <param name="onstate"> The onstate for this button. This will rarely need to be explicitly set. However, it can be used to allow multiple radiobuttons in a group to be on at once if they have the same onstate.
        /// </param>
        /// <returns> A newly created default RadioButtonWidget.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public RadioButtonWidget Add(Rect pos, string onstate)
        {
            TRN_RadioButtonWidget annot = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RadioButtonGroupAdd(mp_impl, ref pos.mp_imp, onstate, ref annot));
            return new RadioButtonWidget(annot, GetRefHandleInternal());
        }

        /// <summary> Adds a RadioButtonWidget with a default onstate to the RadioButtonGroup
        /// </summary>
        /// <param name="pos">A rectangle specifying the annotation's bounds, specified in
        /// user space coordinates.</param>
        /// <returns> A newly created default RadioButtonWidget.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public RadioButtonWidget Add(Rect pos)
        {
            return Add(pos, "");
        }

        /// <summary> Gets the number of buttons in this RadioButtonGroup 
        /// </summary>
        /// <returns> The number of buttons in this RadioButtonGroup
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>		
        public UInt32 GetNumButtons()
        {
            UIntPtr result = UIntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RadioButtonGroupGetNumButtons(mp_impl, ref result));
            return result.ToUInt32();
        }
        /// <summary> Retrieves the button at a given index.
        /// </summary>
        /// <param name="index"> The index to use.
        /// </param>
        /// <returns> The RadioButtonWidget at the given index
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>		
        public RadioButtonWidget GetButton(UInt32 index)
        {
            TRN_RadioButtonWidget annot = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RadioButtonGroupGetButton(mp_impl, new UIntPtr(index), ref annot));
            return new RadioButtonWidget(annot, GetRefHandleInternal());
        }
        /// <summary> Gets the field associated with this RadioButtonGroup
        /// </summary>
        /// <returns> The Field associated with this RadioButtonGroup
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>	
        public Field GetField()
        {
            BasicTypes.TRN_Field result = new BasicTypes.TRN_Field();
            PDFNetException.REX(PDFNetPINVOKE.TRN_RadioButtonGroupGetField(mp_impl, ref result));
            return new Field(result, GetRefHandleInternal());

        }
        /// <summary> Add all group buttons to the page
        /// </summary>
        /// <param name="page"> The page in which to add the buttons.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void AddGroupButtonsToPage(Page page)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RadioButtonGroupAddGroupButtonsToPage(mp_impl, page.mp_page));
        }
    }
}