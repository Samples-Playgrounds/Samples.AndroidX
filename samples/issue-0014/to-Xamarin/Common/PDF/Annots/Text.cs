using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> A text annotation represents a “sticky note” attached to a point in 
    /// the PDF document. When closed, the annotation shall appear as an icon; 
    /// when open, it shall display a pop-up window containing the text of 
    /// the note in a font and size chosen by the conforming reader. 
    /// Text annotations shall not scale and rotate with the page; 
    /// they shall behave as if the NoZoom and NoRotate annotation flags.
    /// </summary>
    public class Text : Markup
    {
        internal Text(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Text annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public Text(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		/// <summary> Creates a new Text annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank Text annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Text Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Text(result, doc);
        }

		/// <summary> Creates a new Text annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <param name="contents">The text contents of the Text annotation.
		/// </param>
		/// <returns> A newly created Text annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Text Create(SDF.SDFDoc doc, Rect pos, String contents)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            UString str = new UString(contents);
            PDFNetException.REX(PDFNetPINVOKE.TRN_AnnotSetContents(result, str.mp_impl));
            return new Text(result, doc);
        }	

		/// <summary> Gets the initial openning condition of the Text annotation.
		/// 
		/// </summary>
		/// <returns> A bool indicating whether the Text annotation is initially open.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this Open option is a flag specifying whether the annotation shall
		///  initially be displayed open.
        /// Default value: false.
        /// </remarks>
        public bool IsOpen()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotIsOpen(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the initial openning condition of the Text annotation.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="isopen">A bool indicating whether the Text annotation is initially open.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this Open option is a flag specifying whether the annotation shall
        /// initially be displayed open.
        /// Default value: false.</remarks>
        public void SetOpen(bool isopen)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotSetOpen(mp_annot, isopen));
        }

        /// <summary> Gets the Icon type as an entry of the enum "Icon".
		/// 
		/// </summary>
		/// <returns> An entry of "Icon" that represents the type of icon
		/// that is corresponding with this Text annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The icon type represnets the name of an icon that shall be
		/// used in displaying the annotation. Conforming readers shall
		/// provide predefined icon appearances for at least the following standard icon types:
		/// e_Comment, e_Key, e_Note, e_Help, e_NewParagraph, e_Paragraph, e_Insert
		/// Additional names may be supported as well. Default value: e_Note.
		/// The annotation dictionary’s Appearance(AP) entry,
        /// if present, shall take precedence over this Name entry.
        /// </remarks>
        public Icon GetIcon()
        {
            Icon result = Icon.e_Unknown;
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotGetIcon(mp_annot, ref result));
            return result;
        }

		/// <summary> Gets the string indicating the type of icon corresponding to
		/// the Text annotation.
		/// 
		/// </summary>
		/// <returns> A string that represents the type of icon
		/// that is corresponding with this Text annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the icon type represnets the name of an icon that shall be
		/// used in displaying the annotation. Conforming readers shall
		/// provide predefined icon appearances for at least the following standard icon types:
		/// e_Comment, e_Key, e_Note, e_Help, e_NewParagraph, e_Paragraph, e_Insert
		/// Additional names may be supported as well. Default value: Note.
        /// The annotation dictionary’s Appearance(AP) entry,
        /// if present, shall take precedence over this Name entry.</remarks>
        public string GetIconName()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotGetIconName(mp_annot, ref result));
            return Marshal.PtrToStringUTF8(result);
        }

		/// <summary> Sets the Icon type as an entry of the enum "Icon".
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="icon">An entry of "Icon" that represents the type of icon
		/// that is corresponding with this Text annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the icon type represnets the name of an icon that shall be
		/// used in displaying the annotation. Conforming readers shall
		/// provide predefined icon appearances for at least
		/// the following standard icon names:
		/// Comment, Key, Note, Help, NewParagraph, Paragraph, Insert
		/// Additional names may be supported as well. Default value: e_Note.
        /// The annotation dictionary’s Appearance(AP) entry,
        /// if present, shall take precedence over this Name entry.</remarks>
        public void SetIcon(Icon icon)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotSetIcon(mp_annot, icon));
        }

		/// <summary> Sets the string indicating the type of icon corresponding to
		/// (Optional).
		/// 
		/// </summary>
		/// <param name="icon">A string that represents the type of icon
		/// that is corresponding with this Text annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the icon type represnets the name of an icon that shall be
		/// used in displaying the annotation. Conforming readers shall
		/// provide predefined icon appearances for at least the following standard icon types:
		/// e_Comment, e_Key, e_Note, e_Help, e_NewParagraph, e_Paragraph, e_Insert
		/// Additional names may be supported as well. Default value: Note.
        /// The annotation dictionary’s Appearance(AP) entry,
        /// if present, shall take precedence over this Name entry.</remarks>
        public void SetIcon(String icon)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotSetIconName(mp_annot, icon));
        }

		/// <summary> Gets the string indicating the state of the Text annotation.
		/// 
		/// </summary>
		/// <returns> A string that represents the state of the Text annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The state is a state to which the original annotation shall
		/// be set.
		/// Default: “Unmarked” if StateModel is “Marked”; “None” if StateModel
        /// is “Review”.
        /// </remarks>
        public string GetState()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotGetState(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets the string indicating the state of the Text annotation.
		/// (Optional; PDF 1.5 )
		/// 
		/// </summary>
		/// <param name="state">A string that represents the state of the Text annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The state is a state to which the original annotation shall
		/// be set.
        /// Default: “Unmarked” if StateModel is “Marked”; “None” if StateModel
        /// is “Review”.</remarks>
        public void SetState(String state)
        {
            UString str = new UString(state);
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotSetState(mp_annot, str.mp_impl));
        }

		/// <summary> Gets the string indicating the state model of the Text annotation.
		/// 
		/// </summary>
		/// <returns> A string that represents the state model of the Text annotation.
		/// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The State model is the state model corresponding to the State entry;</remarks>
        public string GetStateModel()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotGetStateModel(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets the string indicating the state model of the Text annotation.
		/// (Required if State is present, otherwise optional; PDF 1.5 )
		/// 
		/// </summary>
		/// <param name="statemodule">- A string that represents the state model of the Text annotation.
		/// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The State model is the state model corresponding to the State entry; </remarks>
        public void SetStateModel(String statemodule)
        {
            UString str = new UString(statemodule);
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextAnnotSetStateModel(mp_annot, str.mp_impl));
        }

		/// <summary> Creates a Text annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="mku">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public Text(Annot mku) : base(mku.GetSDFObj()) { }
		/// <summary> Releases all resources used by the Text </summary>
		~Text()
        {
            Dispose(false);
        }

        // Nested Types
        ///<summary>type of icon 
		/// corresponding with the Text annotation.</summary>
		public enum Icon //Corresponding to the "name" attribute.
		{
			/// <summary>comment icon</summary>
			e_Comment,
			/// <summary>key icon</summary>
			e_Key,					
			/// <summary>Help icon</summary>
			e_Help,
			/// <summary>New Paragraph icon</summary>
			e_NewParagraph,
			/// <summary>Paragraph icon</summary>
			e_Paragraph,
			/// <summary>Insert icon</summary>
			e_Insert,
			/// <summary>Note icon</summary>
			e_Note,
			/// <summary>Unknown, no icon associated or non-standard icon.</summary>
			e_Unknown   //This corresponds to user defined names.							 
		}
    }
}