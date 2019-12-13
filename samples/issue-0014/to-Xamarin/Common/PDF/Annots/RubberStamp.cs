using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> A rubber stamp annotation (PDF 1.3) displays text or graphics intended 
    /// to look as if they were stamped on the page with a rubber stamp. 
    /// When opened, it shall display a pop-up window containing the text of 
    /// the associated note. Table 181 shows the annotation dictionary entries 
    /// specific to this type of annotation.
    /// </summary>
    public class RubberStamp : Markup
    {
        internal RubberStamp(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a RubberStamp annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public RubberStamp(SDF.Obj d) : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RubberStampAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

        /// <summary> Creates a new RubberStamp annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <param name="icon">icon for the rubber stamp
		/// </param>
		/// <returns> A newly created blank RubberStamp annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static RubberStamp Create(SDF.SDFDoc doc, Rect pos, Icon icon)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RubberStampAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            PDFNetException.REX(PDFNetPINVOKE.TRN_RubberStampAnnotSetIcon(result, icon));
            return new RubberStamp(result, doc);
        }
		/// <summary> Creates a new RubberStamp annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank RubberStamp annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static RubberStamp Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RubberStampAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new RubberStamp(result, doc);
        }
		/// <summary> Gets the Icon type as an entry of the enum "Icon".
		/// 
		/// </summary>
		/// <returns> An entry of "Icon" that represents the type of icon
		/// that is corresponding with this RubberStamp annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Icon GetIcon()
        {
            Icon result = Icon.e_Draft;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RubberStampAnnotGetIcon(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the Icon type as an entry of the enum "Icon". 	 *
		/// 
		/// </summary>
		/// <param name="type">An entry of "Icon" that represents the type of icon
		/// that is corresponding with this RubberStamp annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>
		/// The annotation dictionary’s Appearance(AP) entry,
        /// if present, shall take precedence over this Name entry.
        /// </remarks>
        public void SetIcon(Icon type)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RubberStampAnnotSetIcon(mp_annot, type));
        }
        /// <summary>Sets the type of the icon associated with the RubberStamp annotation to e_Draft.</summary>
        public void SetIcon()
        {
            SetIcon(Icon.e_Draft);
        }

		/// <summary> Gets the Icon type as a string.
		/// 
		/// </summary>
		/// <returns> A string representing icon type of the RubberStamp annotation
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  the icon type represnets an icon that shall be used in displaying the annotation.
		/// There are predefined icon appearances for at the following standard names:
		/// Approved, Experimental, NotApproved, AsIs, Expired , NotForPublicRelease, Confidential,
		/// Final, Sold, Departmental, ForComment, TopSecret, Draft, ForPublicRelease. Additional
		/// names may be supported as well.
		/// Default value: Draft.
        /// The annotation dictionary’s Appearance(AP) entry,
        /// if present, shall take precedence over this Name entry.</remarks>
        public string GetIconName() 
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RubberStampAnnotGetIconName(mp_annot, ref result));
            return Marshal.PtrToStringUTF8(result);
        }
        /// <summary> Creates a RubberStamp annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical 
		/// equivalent of a type cast.</remarks>
        public RubberStamp(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Sets the Icon type as a string.
		/// 
		/// </summary>
		/// <param name="icon">A string representing icon type of the RubberStamp annotation
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>
		/// The annotation dictionary’s Appearance(AP) entry,
        /// if present, shall take precedence over this Name entry.
        /// </remarks>
        public void SetIcon(string icon)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RubberStampAnnotSetIconName(mp_annot, icon));
        }
		/// <summary> Releases all resources used by the RubberStamp </summary>
		~RubberStamp() 
        {
            Dispose(false);
        }

        // Nested Types
        ///<summary>Icon type of the RubberStamp annotation</summary>
        public enum Icon
        {
            ///<summary>a stamp with the text "Approved" </summary>
            e_Approved,
            ///<summary>a stamp with the text "Experimental"</summary>
            e_Experimental,
            ///<summary>a stamp with the text "NotApproved"</summary>
            e_NotApproved,
            ///<summary>a stamp with the text "AsIs"</summary>
            e_AsIs,
            ///<summary>a stamp with the text "Expired"</summary>
            e_Expired,
            ///<summary>a stamp with the text "NotForPublicRelease"</summary>
            e_NotForPublicRelease,
            ///<summary>a stamp with the text "Confidential"</summary>
            e_Confidential,
            ///<summary>a stamp with the text "Final" </summary>
            e_Final,
            ///<summary>a stamp with the text "Sold"</summary>
            e_Sold,
            ///<summary>a stamp with the text "Departmental" </summary>
            e_Departmental,
            ///<summary>a stamp with the text "ForComment" </summary>
            e_ForComment,
            ///<summary>a stamp with the text "TopScret"</summary>
            e_TopSecret,
            ///<summary>a stamp with the text "ForPublicRelease"</summary>
            e_ForPublicRelease,
            ///<summary>a stamp with the text "Draft"</summary>
            e_Draft,
            ///<summary>User defined or invalid.</summary>
            e_Unknown
        }

    }
}