using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Obj = System.IntPtr;
using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> A sound annotation (PDF 1.2) shall analogous to a text annotation 
    /// except that instead of a text note, it contains sound recorded from 
    /// the computer’s microphone or imported from a file. When the annotation 
    /// is activated, the sound shall be played. The annotation shall behave 
    /// like a text annotation in most ways, with a different icon (by default, 
    /// a speaker) to indicate that it represents a sound. 
    /// </summary>
    public class Sound : Markup
    {
        internal Sound(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a Sound annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
		public Sound(SDF.Obj d) : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SoundAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		/// <summary> Creates a new Sound annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank Sound annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Sound Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SoundAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Sound(result, doc);
        }

		/// <summary> Gets the initial sound object of the Sound annotation.
		/// 
		/// </summary>
		/// <returns> An SDF object representing a sound stream.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The sound stream is a sound object defining the sound
        /// that shall be played when the annotation is activated.</remarks>
        public SDF.Obj GetSoundStream()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SoundAnnotGetSoundStream(mp_annot, ref result));
            return new SDF.Obj(result, GetRefHandleInternal());
        }

		/// <summary> Sets the initial sound object of the Sound annotation.
		/// 
		/// </summary>
		/// <param name="sound_stream">- An SDF object representing a sound stream.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The sound stream is a sound object defining the sound
        /// that shall be played when the annotation is activated.</remarks>
        public void SetSoundStream(SDF.Obj sound_stream)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SoundAnnotSetSoundStream(mp_annot, sound_stream.mp_obj));
        }

        /// <summary> Gets the sub type of the Sound annotation.
		/// 
		/// </summary>
		/// <returns> An entry in the "GetIcon" enum
		/// indicating the subtype of the Sound annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Icon indicates name of an icon that shall be
		/// used in displaying the annotation. Conforming readers shall
		/// provide predefined icon appearances for at least the standard
		/// types e_Speaker and e_Mic.
		/// Additional types may be supported as well. However, user defined
		/// sub types has to be represented by string, in the enum"Icon",
		/// it will just show as "e_Unknow".
		/// Default value: e_Speaker.
		/// The annotation dictionary’s AP entry, if present, shall take
		/// precedence over the Name entry; see Table 168 and 12.5.5,
        /// “Appearance Streams.”
        /// </remarks>
        public Icon GetIcon()
        {
            Icon result = Icon.e_Unknown;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SoundAnnotGetIcon(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the sub type of the Sound annotation.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="type">An entry in the "GetIcon" enum
		/// indicating the subtype of the Sound annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Icon indicates name of an icon that shall be
		/// used in displaying the annotation. Conforming readers shall
		/// provide predefined icon appearances for at least the standard
		/// types e_Speaker and e_Mic.
		/// Additional types may be supported as well. However, user defined
		/// sub types has to be represented by string. In the enum"Icon",
		/// it will just show as "e_Unknow".
		/// Default value: e_Speaker.
		/// The annotation dictionary’s AP entry, if present, shall take
		/// precedence over the Name entry; see Table 168 and 12.5.5,
        /// “Appearance Streams.”
        /// </remarks>
        public void SetIcon(Icon type)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SoundAnnotSetIcon(mp_annot, type));
        }

		/// <summary> Gets the sub type of the Sound annotation.
		/// 
		/// </summary>
		/// <returns> A string indicating the subtype of the Sound annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Icon indicates a name of an icon that shall be
		/// used in displaying the annotation. Conforming readers shall
		/// provide predefined icon appearances for at least the standard
		/// types Speaker and Mic.
		/// Additional types may be supported as well.
		/// Default value: Speaker.
		/// The annotation dictionary’s AP entry, if present, shall take
		/// precedence over the Name entry; see Table 168 and 12.5.5,
		/// “Appearance Streams.”</remarks>
        public string GetIconName() //Directly copied from the old version
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SoundAnnotGetIconName(mp_annot, ref result));
            return Marshal.PtrToStringUTF8(result);
        }

		/// <summary> Sets the sub type of the Sound annotation.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="type">- A string indicating the subtype of the Sound annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Icon indicates a name of an icon that shall be
		/// used in displaying the annotation. Conforming readers shall
		/// provide predefined icon appearances for at least the standard
		/// types Speaker and Mic.
		/// Additional types may be supported as well.
		/// Default value: Speaker.
		/// The annotation dictionary’s AP entry, if present, shall take
		/// precedence over the Name entry; see Table 168 and 12.5.5,
		/// “Appearance Streams.”
		/// </remarks>
        public void SetIcon(String type)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SoundAnnotSetIconName(mp_annot, type));
        }
		/// <summary> Creates a Sound annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public Sound(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Releases all resources used by the Sound </summary>
        ~Sound()
        {
            Dispose(false);
        }

        // Nested Types
        ///<summary>The Icon of the Sound annotation. </summary>
        public enum Icon
        {
            ///<summary>Speaker Sound.</summary>
			e_Speaker,
			///<summary>Mic Sound.</summary>
			e_Mic,
			///<summary>User defined or Invalid.</summary>
			e_Unknown
        }
    }
}