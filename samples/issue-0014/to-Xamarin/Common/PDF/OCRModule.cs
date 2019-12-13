using System;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;


using TRN_PDFDoc = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF
{
	/// <summary>
	/// static interface to PDFTron SDKs OCR functionality
	/// </summary>
	public static class OCRModule
	{


		/// <summary>
        /// Find out whether the OCR module is available (and licensed).
        /// </summary>
        /// <returns>returns true if OCR operations can be performed</returns>
		public static bool IsModuleAvailable()
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_OCRModuleIsModuleAvailable(ref result));
			return result;
		}

		/// <summary>
        /// Convert an image to a PDF with searchable text.
        /// </summary>
        /// <param name="dst">The destination document</param>
        /// <param name="src">The path to the input image</param>
        /// <param name="options">OCR options (optional)</param>
		public static void ImageToPDF(PDFDoc dst, string src, OCROptions options)
		{
			TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCRModuleImageToPDF(dst.mp_doc, UString.ConvertToUString(src).mp_impl, opt_ptr));
		}

		/// <summary>
        /// Add searchable and selectable text to raster images within a PDF.
        /// </summary>
        /// <param name="dst">The source and destination document</param>
        /// <param name="options">OCR options (optional)</param>
		public static void ProcessPDF(PDFDoc dst, OCROptions options)
		{
			TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_OCRModuleProcessPDF(dst.mp_doc, opt_ptr));
		}

		/// <summary>
        /// Perform OCR on an image and return resulting JSON string. Side effect: source image is converted to PDF and stored in the destination document.
        /// </summary>
        /// <param name="dst">The destination document</param>
        /// <param name="src">The path to the input image</param>
        /// <param name="options">OCR options (optional)</param>
        /// <returns>JSON string represeting OCR results</returns>
		public static string GetOCRJsonFromImage(PDFDoc dst, string src, OCROptions options)
		{
			UString result = new UString();
			TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_OCRModuleGetOCRJsonFromImage(dst.mp_doc, UString.ConvertToUString(src).mp_impl, opt_ptr, ref result.mp_impl));
			return result.ConvToManagedStr();
		}

		/// <summary>
        /// Perform OCR on raster images within a PDF and return resulting JSON string.
        /// </summary>
        /// <param name="src">The source document</param>
        /// <param name="options">OCR options (optional)</param>
        /// <returns>JSON string represeting OCR results</returns>
		public static string GetOCRJsonFromPDF(PDFDoc src, OCROptions options)
		{
			UString result = new UString();
			TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_OCRModuleGetOCRJsonFromPDF(src.mp_doc, opt_ptr, ref result.mp_impl));
			return result.ConvToManagedStr();
		}

		/// <summary>
        /// Add hidden text layer to a PDF consisting of raster image(s).
        /// </summary>
        /// <param name="dst">The source and destination document</param>
        /// <param name="json">JSON representing OCR results</param>
		public static void ApplyOCRJsonToPDF(PDFDoc dst, string json)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_OCRModuleApplyOCRJsonToPDF(dst.mp_doc, UString.ConvertToUString(json).mp_impl));
		}

		/// <summary>
        /// Perform OCR on an image and return resulting XML string. Side effect: source image is converted to PDF and stored in the destination document.
        /// </summary>
        /// <param name="dst">The destination document</param>
        /// <param name="src">The path to the input image</param>
        /// <param name="options">OCR options (optional)</param>
        /// <returns>XML string represeting OCR results</returns>
		public static string GetOCRXmlFromImage(PDFDoc dst, string src, OCROptions options)
		{
			UString result = new UString();
			TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_OCRModuleGetOCRXmlFromImage(dst.mp_doc, UString.ConvertToUString(src).mp_impl, opt_ptr, ref result.mp_impl));
			return result.ConvToManagedStr();
		}

		/// <summary>
        /// Perform OCR on raster images within a PDF and return resulting XML string.
        /// </summary>
        /// <param name="src">The source document</param>
        /// <param name="options">OCR options (optional)</param>
        /// <returns>XML string represeting OCR results</returns>
		public static string GetOCRXmlFromPDF(PDFDoc src, OCROptions options)
		{
			UString result = new UString();
			TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_OCRModuleGetOCRXmlFromPDF(src.mp_doc, opt_ptr, ref result.mp_impl));
			return result.ConvToManagedStr();
		}

		/// <summary>
        /// Add hidden text layer to a PDF consisting of raster image(s).
        /// </summary>
        /// <param name="dst">The source and destination document</param>
        /// <param name="xml">XML representing OCR results</param>
		public static void ApplyOCRXmlToPDF(PDFDoc dst, string xml)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_OCRModuleApplyOCRXmlToPDF(dst.mp_doc, UString.ConvertToUString(xml).mp_impl));
		}
	}
}
