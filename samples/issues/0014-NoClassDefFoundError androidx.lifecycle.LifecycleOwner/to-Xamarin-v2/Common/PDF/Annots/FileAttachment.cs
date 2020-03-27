using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_FileSpec = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> A file attachment annotation (PDF 1.3) contains a reference 
    /// to a file, which typically shall be embedded in the PDF file.
    /// </summary>
    public class FileAttachment : Markup
    {
        internal FileAttachment(TRN_Annot imp, Object reference) : base(imp, reference) { }

        //FileAttachment(SDF.Obj d);	
		/// <summary> Creates an FileAttachment annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public FileAttachment(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileAttachmentAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		//bool Export(string& save_as = "") ;
		/// <summary> The function saves the data referenced by this File Attachment to an
		/// external file.
		/// 
		/// If the file is embedded, the function saves the embedded file.
		/// If the file is not embedded, the function will copy the external file.
		/// If the file is not embedded and the external file can't be found, the function
		/// returns false.
		/// 
		/// </summary>
		/// <param name="save_as">An optional parameter indicating the filepath and filename
		/// where the data should be saved. If this parameter is not specified the function
		/// will attempt to save the file using FileSpec.GetFilePath().
		/// </param>
		/// <returns> true is the file was saved successfully, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool Export(string save_as)
        {
            bool result = false;
            UString str = new UString(save_as);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileAttachmentAnnotExport(mp_annot, str.mp_impl, ref result));
            return result;
        }
		/// <summary> The function saves the data referenced by this File Attachment to an
		/// external file.
		/// 
		/// If the file is embedded, the function saves the embedded file.
		/// If the file is not embedded, the function will copy the external file.
		/// If the file is not embedded and the external file can't be found, the function
		/// returns false.
		/// 
		/// </summary>
		/// <returns> true is the file was saved successfully, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool Export()
        {
            return Export("");
        }
		//static FileAttachment Create(SDF.SDFDoc& doc, Rect& pos, string path, string icon_name = "PushPin");
		/// <summary> Creates the.
		/// 
		/// </summary>
		/// <param name="doc">the doc
		/// </param>
		/// <param name="pos">the pos
		/// </param>
		/// <param name="path">the path
		/// </param>
		/// <param name="icon_name">the icon_name
		/// </param>
		/// <returns> the file attachment
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static FileAttachment Create(SDF.SDFDoc doc, Rect pos, string path, string icon_name)
        {
            TRN_Annot result = IntPtr.Zero;
            UString str = new UString(path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileAttachmentAnnotCreate(doc.mp_doc, ref pos.mp_imp, str.mp_impl, icon_name, ref result));
            return new FileAttachment(result, doc);
        }
		/// <summary> Creates a file attachment annotation.
		/// 
		/// A file attachment annotation contains a reference to a file, which typically
		/// is embedded in the PDF file.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <param name="path">the path
		/// </param>
		/// <returns> A new file attachment annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  PDF Viewer applications should provide predefined icon appearances for at least
        /// the following standard names: Graph PushPin Paperclip Tag. Additional names may
        /// be supported as well. Default value: PushPin.</remarks>
        public static FileAttachment Create(SDF.SDFDoc doc, Rect pos, string path)
        {
            TRN_Annot result = IntPtr.Zero;
            UString str = new UString(path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileAttachmentAnnotCreateWithIcon(doc.mp_doc, ref pos.mp_imp, str.mp_impl, Icon.e_PushPin, ref result));
            return new FileAttachment(result, doc);
        }
		//FileSpec GetFileSpec() ;
		/// <summary> Gets the file specification.
		/// 
		/// </summary>
		/// <returns> The file associated with this annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FileSpec GetFileSpec()
        {
            TRN_FileSpec result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileAttachmentAnnotGetFileSpec(mp_annot, ref result));
            return new FileSpec(result, GetRefHandleInternal());
        }

		//void SetFileSpec(FileSpec& file);
		/// <summary> Sets the file specification.
		/// 
		/// </summary>
		/// <param name="file">The file associated with this annotation.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetFileSpec(FileSpec file)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileAttachmentAnnotSetFileSpec(mp_annot, file.mp_impl));
        }

        //Icon GetIcon() ;
		/// <summary> Gets the icon type as an entry of the enum "Icon".
		/// 
		/// </summary>
		/// <returns> The enum "Icon" entry associated with this FileAttachment annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The entry specifies the name of an icon that shall be used in displaying
		/// the annotation.
		/// Conforming readers shall provide predefined icon appearances for at least
		/// the following standard names: GraphPushPin, PaperclipTag
		/// Additional names may be supported as well. Default value: PushPin.
		/// The annotation dictionary’s Appearance(AP) entry, if present,
        /// shall take precedence over the this entry
        /// </remarks>
        public Icon GetIcon()
        {
            Icon result = Icon.e_Unknown;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileAttachmentAnnotGetIcon(mp_annot, ref result));
            return result;
        }

		//void SetIcon(Icon type=e_GraphPushPin);
		/// <summary> Sets the icon type using an entry of the enum "Icon".
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="type">The enum "Icon" entry associated with this FileAttachment annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The entry specifies the name of an icon that shall be used in displaying the annotation.
		/// Conforming readers shall provide predefined icon appearances for at least
		/// the following standard names: GraphPushPin, PaperclipTag
		/// Additional names may be supported as well. Default value: PushPin.
		/// The annotation dictionary’s Appearance(AP) entry, if present,
        /// shall take precedence over the this entry
        /// </remarks>
        public void SetIcon(Icon type)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileAttachmentAnnotSetIcon(mp_annot, type));
        }
        /// <summary> Sets the icon type to e_PushPin.</summary>
        void SetIcon()
        {
            SetIcon(Icon.e_PushPin);
        }

		//string GetIconName() ; 
		/// <summary> Gets the icon type as a string.
		/// 
		/// </summary>
		/// <returns> The string specifying the icon type associated with this
		/// FileAttachment annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the string spedifies the name of an icon that shall be used in displaying the annotation.
		/// Conforming readers shall provide predefined icon appearances for at least
		/// the following standard names: GraphPushPin, PaperclipTag
		/// Additional names may be supported as well. Default value: PushPin.
        /// The annotation dictionary’s Appearance(AP) entry, if present,
        /// shall take precedence over the this entry</remarks>
        public string GetIconName()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileAttachmentAnnotGetIconName(mp_annot, ref result));
            return Marshal.PtrToStringUTF8(result);
        }

		//void SetIconName(string icon);
		/// <summary> Sets the icon type using a string.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="icon">The string specifying the icon type associated with this
		/// FileAttachment annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the string spedifies the name of an icon that shall be used in displaying 
		/// the annotation.
		/// Conforming readers shall provide predefined icon appearances for at least
		/// the following standard names:
		/// GraphPushPin
		/// PaperclipTag
		/// Additional names may be supported as well. Default value: PushPin.
		/// The annotation dictionary’s Appearance(AP) entry, if present,
        /// shall take precedence over the this entry
        /// </remarks>
        public void SetIconName(string icon)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileAttachmentAnnotSetIconName(mp_annot, icon));
        }
		/// <summary> Creates an FileAttachment annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
		public FileAttachment(Annot ann) : base(ann.GetSDFObj()) {}
		/// <summary> Releases all resources used by the FileAttachment </summary>
		~FileAttachment()
        {
            Dispose(false);
        }

        // Nested Types
        ///<summary>FileAttchment annotation icons</summary>
		public enum Icon
        {
            ///<summary>graph appearance</summary>
			e_Graph,
			///<summary>push pin appearance.</summary>
			e_PushPin,
			///<summary>paper clip appearance</summary>
			e_Paperclip,
			///<summary>tag appearance</summary>
			e_Tag,
			///<summary>unrecognized appearance type</summary>
			e_Unknown
        }
    }
}