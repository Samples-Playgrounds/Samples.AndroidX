using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_RadioButtonGroup = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> 
    /// An object representing a Radio Button used in a PDF Form.
    /// </summary>
    public class RadioButtonWidget : Widget
    {

        internal RadioButtonWidget(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Radio Button Widget annotation and initialize it using given Cos/SDF object.
        /// </summary>
        /// <param name="d">the object to use to initialize the Radio Button Widget
        /// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public RadioButtonWidget(SDF.Obj d) : base(d) { }

        /// <summary> Creates a Radio Button Widget annotation and initialize it using given annotation object.
        /// 
        /// </summary>
        /// <param name="ann"> The annotation to use.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public RadioButtonWidget(Annot ann) : base(ann.GetSDFObj()) { }

        /// <summary> Gets the group to which the current button is connected.
        /// </summary>
        /// <returns> The group containing this Radio Button.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public RadioButtonGroup GetGroup()
        {
            TRN_RadioButtonGroup result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RadioButtonWidgetGetGroup(mp_annot, ref result));
            return new RadioButtonGroup(result, GetRefHandleInternal());

        }
        /// <summary> Enable the current radio button. Note that this may disable other Radio Buttons in the same group.
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void EnableButton()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RadioButtonWidgetEnableButton(mp_annot));
        }
        /// <summary> Determines whether this button is enabled.
        /// </summary>
        /// <returns> A boolean value indicating whether the Radio Button is enabled.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsEnabled()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RadioButtonWidgetIsEnabled(mp_annot, ref result));
            return result;
        }

    }
}