using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary>An object representing a Signature used in a PDF Form. These Widgets can be signed directly, or signed using a DigitalSignatureField.</summary>
    public class SignatureWidget : Widget
    {
        internal SignatureWidget(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary>Creates a SignatureWidget annotation and initializes it using given Cos/SDF object.</summary>
        /// <param name="d">the object to use to initialize the SignatureWidget</param>
        /// <remarks>The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public SignatureWidget(SDF.Obj d) : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SignatureWidgetCreateFromObj(d.mp_obj, ref mp_annot));
        }

        /// <summary> Creates a SignatureWidget annotation and initialize it using given annotation object.
        /// 
        /// </summary>
		/// <param name="ann">The input annotation
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public SignatureWidget(Annot ann) : base(ann.GetSDFObj())
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SignatureWidgetCreateFromAnnot(ann.mp_annot, ref mp_annot));
        }

        /// <summary>Creates a new SignatureWidget annotation in the specified document, and adds a signature form field to the document.</summary>
        /// <param name="doc">The document to which the widget is to be added.</param>
        /// <param name="pos">A rectangle specifying the widget's bounds in default user space units.</param>
        /// <param name="field_name">The name of the digital signature field to create.</param>
        /// <returns>A newly-created blank SignatureWidget annotation.</returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static SignatureWidget Create(PDFDoc doc, Rect pos, string field_name)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SignatureWidgetCreate(doc.mp_doc, ref pos.mp_imp, UString.ConvertToUString(field_name).mp_impl, ref result));
            return new SignatureWidget(result, doc);
        }

        /// <summary>Creates a new SignatureWidget annotation in the specified document, and adds an associated signature form field to the document with a default Field name.</summary>
        /// <param name="doc">The document to which the widget is to be added.</param>
        /// <param name="pos">A rectangle specifying the widget's bounds in default user space units.</param>
        /// <returns>A newly-created blank SignatureWidget annotation.</returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static SignatureWidget Create(PDFDoc doc, Rect pos)
        {
            return Create(doc, pos, "");
        }

        /// <summary>Creates a new SignatureWidget annotation associated with a particular form field in the specified document.</summary>
        /// <param name="doc">The document to which the widget is to be added.</param>
        /// <param name="pos">A rectangle specifying the widget's bounds in default user space units.</param>
        /// <param name="field">The digital signature field for which to create a signature widget.</param>
        /// <returns>A newly-created blank SignatureWidget annotation.</returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static SignatureWidget Create(PDFDoc doc, Rect pos, Field field)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SignatureWidgetCreateWithField(doc.mp_doc, ref pos.mp_imp, ref field.mp_field, ref result));
            return new SignatureWidget(result, doc);
        }

        /// <summary>Creates a new SignatureWidget annotation associated with a particular DigitalSignatureField object (representing a signature-type form field) in the specified document.</summary>
        /// <param name="doc">The document to which the widget is to be added.</param>
        /// <param name="pos">A rectangle specifying the widget's bounds in default user space units.</param>
        /// <param name="field">The digital signature field for which to create a signature widget.</param>
        /// <returns>A newly-created blank SignatureWidget annotation.</returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static SignatureWidget Create(PDFDoc doc, Rect pos, DigitalSignatureField field)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SignatureWidgetCreateWithDigitalSignatureField(doc.mp_doc, ref pos.mp_imp, ref field.m_impl, ref result));
            return new SignatureWidget(result, doc);
        }

        /// <summary>A function that will create and add an appearance to this widget by centering an image within it.</summary>
        /// <param name="img">An Image object representing the image to use.</param>
        public void CreateSignatureAppearance(Image img)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SignatureWidgetCreateSignatureAppearance(mp_annot, img.mp_image));
        }

        /// <summary>Retrieves the DigitalSignatureField associated with this SignatureWidget.</summary>
        /// <returns>A DigitalSignatureField object representing the digital signature form field associated with this signature widget annotation.</returns>
        public DigitalSignatureField GetDigitalSignatureField()
        {
            BasicTypes.TRN_DigitalSignatureField result = new BasicTypes.TRN_DigitalSignatureField();
            PDFNetException.REX(PDFNetPINVOKE.TRN_SignatureWidgetGetDigitalSignatureField(mp_annot, ref result));
            return new DigitalSignatureField(result, GetRefHandleInternal());
        }
    }
}