using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> The Class Movie.</summary>
    public class Movie : Annot
    {
        internal Movie(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> A movie annotation (PDF 1.2) contains animated graphics and sound to be
		/// presented on the computer screen and through the speakers. When the
		/// annotation is activated, the movie shall be played.
		/// 
		/// </summary>
        /// <param name="d">the d
        /// </param>
        public Movie(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MovieAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		/// <summary> Creates a new Movie annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the Movie annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the Movie annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank Movie annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Movie Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_MovieAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Movie(result, doc);
        }

		/// <summary> Gets the title of the Movie Annotation.
		/// 
		/// </summary>
		/// <returns> A string representing the title of the Movie Annotation
		/// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Movie actions may use this title to reference the movie annotation. </remarks>
        public string GetTitle()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_MovieAnnotGetTitle(mp_annot, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

		/// <summary> Sets the title of the Movie Annotation.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="title">A string representing the title of the Movie Annotation
		/// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Movie actions may use this title to reference the movie annotation. </remarks>
        public void SetTitle(string title)
        {
            UString str = new UString(title);
            PDFNetException.REX(PDFNetPINVOKE.TRN_MovieAnnotSetTitle(mp_annot, str.mp_impl));
        }

		/// <summary> Gets the option of whether the Movie is to be played.
		/// 
		/// </summary>
		/// <returns> a boolean value indicating if the movie is to be played
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  IsToBePlayed is a flag specifying whether to play the movie when the annotation is activated.
		/// The movie shall be played using default activation parameters. If the value is false,
        /// the movie shall not be played.
        /// Default value: true.</remarks>
        public bool IsToBePlayed()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_MovieAnnotIsToBePlayed(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the option of whether the Movie is to be played.
		/// (Optional)
		/// 
		/// </summary>
		/// <param name="isplay">A boolean value telling if the movie is to be played.
		/// Default value: true.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  IsToBePlayed is a flag specifying whether to play the movie when the annotation is activated.
		/// The movie shall be played using default activation parameters. If the value is false,
        /// the movie shall not be played.
        /// Default value: true.</remarks>
        public void SetToBePlayed(bool isplay)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_MovieAnnotSetToBePlayed(mp_annot, isplay));
        }

		/// <summary> Creates a Movie annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public Movie(Annot ann) : base(ann.GetSDFObj()) { } 
		/// <summary> Releases all resources used by the Movie </summary>
		~Movie()
        {
            Dispose(false);
        }
    }
}