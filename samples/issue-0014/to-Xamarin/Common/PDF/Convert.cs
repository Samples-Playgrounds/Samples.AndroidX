using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Obj = System.IntPtr;
using TRN_DocumentConversion = System.IntPtr;
using TRN_Filter = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary>
    /// Encapsulates the conversion of a single document from one format to another.
    /// </summary>
    /// <remark>
    /// DocumentConversion instances are created through methods belonging to
    /// the Convert class. See Convert.WordToPDFConversion for an example.
    /// </remark>
    public class Convert : IDisposable
    {

        /// <summary> Convert the specified XPS document to PDF and append converted pages
        /// to the specified PDF document.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to append to
        /// </param>
        /// <param name="in_filename">the path to the XPS document to convert
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void FromXps(PDFDoc in_pdfdoc, String in_filename)
        {
            UString ustr = new UString(in_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertFromXps(in_pdfdoc.mp_doc, ustr.mp_impl));
        }

        /// <summary> Convert the specified XPS document contained in memory to PDF and append converted 
        /// pages to the specified PDF document.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to append to
        /// </param>
        /// <param name="buf">the buffer containing the xps document
        /// </param>
        /// <param name="buf_size">the size of the buffer
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void FromXps(PDFDoc in_pdfdoc, byte[] buf, Int32 buf_size)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertFromXpsMem(in_pdfdoc.mp_doc, buf, System.Convert.ToUInt32(buf_size)));
        }

        /// <summary> Convert the specified EMF to PDF and append converted pages
        /// to the specified PDF document.  EMF will be fitted to the page.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to append to
        /// </param>
        /// <param name="in_filename">the path to the EMF document to convert
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  This method is available only on Windows platforms. </remarks>
        public static void FromEmf(PDFDoc in_pdfdoc, string in_filename)
        {
            UString ustr = new UString(in_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertFromEmf(in_pdfdoc.mp_doc, ustr.mp_impl));
        }

        /// <summary>
        /// Convert the a Word document (in .docx format) to pdf and append to the specified PDF document. This conversion is performed entirely within PDFNet, and does not rely on Word interop or any other external functionality.
        /// <br/>Font requirements: on some systems you may need to specify extra font resources to aid in conversion. Please see http://www.pdftron.com/kb_fonts_and_builtin_office_conversion
        /// </summary>
        /// <param name="doc"> the conversion result will be appended to this pdf</param>
        /// <param name="filePath">the path to the source document. The source must be in .docx format.</param> 
        /// <param name="options">the conversion options</param> 
        /// <remarks>
        /// see WordToPdfConversion() if you would like more control over the conversion process
        /// </remarks>
        public static void WordToPDF(PDFDoc inDoc, string inFilename, WordToPDFOptions options)
        {
            TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertWordToPdf(inDoc.mp_doc, UString.ConvertToUString(inFilename).mp_impl, opt_ptr));
        }

        /// <summary>
        /// Convert the a Word document (in .docx format) to pdf and append to the specified PDF document. This conversion is performed entirely within PDFNet, and does not rely on Word interop or any other external functionality.
        /// <br/>Font requirements: on some systems you may need to specify extra font resources to aid in conversion. Please see http://www.pdftron.com/kb_fonts_and_builtin_office_conversion
        /// </summary>
        /// <param name="doc"> the conversion result will be appended to this pdf</param>
        /// <param name="in_stream">the source data stream. The source must be in .docx format.</param> 
        /// <param name="options">the conversion options</param> 
        /// <remarks>
        /// see WordToPdfConversion() if you would like more control over the conversion process
        /// </remarks>
        public static void WordToPDF(PDFDoc inDoc, pdftron.Filters.Filter inData, WordToPDFOptions options)
        {
            TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertWordToPdfWithFilter(inDoc.mp_doc, inData.mp_imp, opt_ptr));
        }

        /// <summary>
        /// Create a DocumentConversion object suitable for converting a Word document (in .docx format) to pdf and appending to the specified PDF document. This conversion will be performed entirely within PDFNet, and does not rely on Word interop or any other external functionality.
        /// <br/>This method allows for more control over the conversion process than the single call WordToPDF() interface. This method does not perform any  conversion logic and can be expected to return quickly. To do the actual conversion, use the returned DocumentConversion object (see PDF.DocumentConversion)
        /// <br/> Font requirements: on some systems you may need to specify extra font resources to aid in conversion. Please see http://www.pdftron.com/kb_fonts_and_builtin_office_conversion
        /// </summary>
        /// <param name="doc"> the conversion result will be appended to this pdf</param>
        /// <param name="filePath">the path to the source document. The source must be in .docx format.</param> 
        /// <param name="options">the conversion options</param>
        /// <returns>A DocumentConversion object which encapsulates this particular conversion</returns>
        /// <remarks>
        /// see WordToPdfConversion() if you would like more control over the conversion process
        /// </remarks>
        public static DocumentConversion WordToPDFConversion(PDFDoc doc, string filePath, WordToPDFOptions options)
        {
            TRN_DocumentConversion ret = IntPtr.Zero;
            TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertWordToPdfConversion(ref doc.mp_doc, UString.ConvertToUString(filePath).mp_impl, opt_ptr, ref ret));
            return new DocumentConversion(ret);
        }

        /// <summary>
        /// Create a DocumentConversion object suitable for converting a Word document (in .docx format) to pdf and appending to the specified PDF document. This conversion will be performed entirely within PDFNet, and does not rely on Word interop or any other external functionality.
        /// <br/>This method allows for more control over the conversion process than the single call WordToPDF() interface. This method does not perform any  conversion logic and can be expected to return quickly. To do the actual conversion, use the returned DocumentConversion object (see PDF.DocumentConversion)
        /// <br/> Font requirements: on some systems you may need to specify extra font resources to aid in conversion. Please see http://www.pdftron.com/kb_fonts_and_builtin_office_conversion
        /// </summary>
        /// <param name="doc"> the conversion result will be appended to this pdf</param>
        /// <param name="in_stream">the source data stream. The source must be in .docx format.</param> 
        /// <param name="options">the conversion options</param>
        /// <returns>A DocumentConversion object which encapsulates this particular conversion</returns>
        /// <remarks>
        /// see WordToPdfConversion() if you would like more control over the conversion process
        /// </remarks>
        public static DocumentConversion WordToPDFConversion(PDFDoc doc, pdftron.Filters.Filter inData, WordToPDFOptions options)
        {
            TRN_DocumentConversion ret = IntPtr.Zero;
            TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertWordToPdfConversion(ref doc.mp_doc, inData.mp_imp, opt_ptr, ref ret));
            return new DocumentConversion(ret);
        }

        /// <summary>
        /// Convert the an office document (in .docx, .xlsx, pptx, or .doc format) to pdf and append to the specified PDF document. This conversion is performed entirely within PDFNet, and does not rely on Word interop or any other external functionality.
        /// <br/>Font requirements: on some systems you may need to specify extra font resources to aid in conversion. Please see http://www.pdftron.com/kb_fonts_and_builtin_office_conversion
        /// </summary>
        /// <param name="doc"> the conversion result will be appended to this pdf</param>
        /// <param name="filePath">the path to the source document</param> 
        /// <param name="options">the conversion options</param> 
        /// <remarks>
        /// see StreamingPDFConversion() if you would like more control over the conversion process
        /// </remarks>
        public static void OfficeToPDF(PDFDoc inDoc, string inFilename, ConversionOptions options)
        {
            TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertOfficeToPdfWithPath(inDoc.mp_doc, UString.ConvertToUString(inFilename).mp_impl, opt_ptr));
        }

        /// <summary>
        /// Convert the an office document (in .docx, .xlsx, pptx, or .doc format) to pdf and append to the specified PDF document. This conversion is performed entirely within PDFNet, and does not rely on Word interop or any other external functionality.
        /// <br/>Font requirements: on some systems you may need to specify extra font resources to aid in conversion. Please see http://www.pdftron.com/kb_fonts_and_builtin_office_conversion
        /// </summary>
        /// <param name="doc"> the conversion result will be appended to this pdf</param>
        /// <param name="in_stream">the source data stream.</param> 
        /// <param name="options">the conversion options</param> 
        /// <remarks>
        /// see StreamingPDFConversion() if you would like more control over the conversion process
        /// </remarks>
        public static void OfficeToPDF(PDFDoc inDoc, pdftron.Filters.Filter inData, ConversionOptions options)
        {
            TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertOfficeToPdfWithFilter(inDoc.mp_doc, inData.mp_imp, opt_ptr));
        }

        /// <summary>
        /// Create a DocumentConversion object suitable for converting a file to pdf.
        /// <br/>This conversion will be performed entirely within PDFNet, and handles incoming files in .docx, .xlsx, pptx, .doc, .png, .jpg, .bmp, .gif, .jp2, .tif, .txt, .xml and .md format
        /// <br/>This method does not perform any  conversion logic and can be expected to return quickly. To do the actual conversion, use the returned DocumentConversion object (see PDF.DocumentConversion)
        /// <br/> Font requirements: on some systems you may need to specify extra font resources to aid in conversion. Please see http://www.pdftron.com/kb_fonts_and_builtin_office_conversion
        /// </summary>
        /// <param name="filePath">the path to the source document</param> 
        /// <param name="options">the conversion options</param>
        /// <returns>A DocumentConversion object which encapsulates this particular conversion</returns>
        public static DocumentConversion StreamingPDFConversion(string filePath, ConversionOptions options)
        {
            TRN_DocumentConversion ret = IntPtr.Zero;
            TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertStreamingPdfConversionWithPath(UString.ConvertToUString(filePath).mp_impl, opt_ptr, ref ret));
            return new DocumentConversion(ret);
        }

        /// <summary>
        /// Create a DocumentConversion object suitable for converting a file to pdf.
        /// <br/>This conversion will be performed entirely within PDFNet, and handles incoming files in .docx, .xlsx, pptx, .doc, .png, .jpg, .bmp, .gif, .jp2, .tif, .txt, .xml and .md format
        /// <br/>This method does not perform any  conversion logic and can be expected to return quickly. To do the actual conversion, use the returned DocumentConversion object (see PDF.DocumentConversion)
        /// <br/> Font requirements: on some systems you may need to specify extra font resources to aid in conversion. Please see http://www.pdftron.com/kb_fonts_and_builtin_office_conversion
        /// </summary>
        /// <param name="in_stream">the source data stream</param> 
        /// <param name="options">the conversion options</param>
        /// <returns>A DocumentConversion object which encapsulates this particular conversion</returns>
        /// <remarks>
        /// see StreamingPDFConversion() if you would like more control over the conversion process
        /// </remarks>
        public static DocumentConversion StreamingPDFConversion(pdftron.Filters.Filter inData, ConversionOptions options)
        {
            TRN_DocumentConversion ret = IntPtr.Zero;
            TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertStreamingPdfConversionWithFilter(inData.mp_imp, opt_ptr, ref ret));
            return new DocumentConversion(ret);
        }

        /// <summary>
        /// Create a DocumentConversion object suitable for converting a file to pdf and appending to an existing  PDF document. 
        /// <br/>This conversion will be performed entirely within PDFNet, and handles incoming files in .docx, .xlsx, pptx, .doc, .png, .jpg, .bmp, .gif, .jp2, .tif, .txt, .xml and .md format
        /// <br/>This method does not perform any  conversion logic and can be expected to return quickly. To do the actual conversion, use the returned DocumentConversion object (see PDF.DocumentConversion)
        /// <br/> Font requirements: on some systems you may need to specify extra font resources to aid in conversion. Please see http://www.pdftron.com/kb_fonts_and_builtin_office_conversion
        /// </summary>
        /// <param name="doc"> the conversion result will be appended to this pdf</param>
        /// <param name="filePath">the path to the source document</param> 
        /// <param name="options">the conversion options</param>
        /// <returns>A DocumentConversion object which encapsulates this particular conversion</returns>
        public static DocumentConversion StreamingPDFConversion(PDFDoc doc, string filePath, ConversionOptions options)
        {
            TRN_DocumentConversion ret = IntPtr.Zero;
            TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertStreamingPdfConversionWithPdfAndPath(ref doc.mp_doc, UString.ConvertToUString(filePath).mp_impl, opt_ptr, ref ret));
            return new DocumentConversion(ret);
        }

        /// <summary>
        /// Create a DocumentConversion object suitable for converting a file to pdf and appending to an existing  PDF document. 
        /// <br/>This conversion will be performed entirely within PDFNet, and handles incoming files in .docx, .xlsx, pptx, .doc, .png, .jpg, .bmp, .gif, .jp2, .tif, .txt, .xml and .md format
        /// <br/>This method does not perform any  conversion logic and can be expected to return quickly. To do the actual conversion, use the returned DocumentConversion object (see PDF.DocumentConversion)
        /// <br/> Font requirements: on some systems you may need to specify extra font resources to aid in conversion. Please see http://www.pdftron.com/kb_fonts_and_builtin_office_conversion
        /// </summary>
        /// <param name="doc"> the conversion result will be appended to this pdf</param>
        /// <param name="in_stream">the source data stream</param> 
        /// <param name="options">the conversion options</param>
        /// <returns>A DocumentConversion object which encapsulates this particular conversion</returns>
        public static DocumentConversion StreamingPDFConversion(PDFDoc doc, pdftron.Filters.Filter inData, ConversionOptions options)
        {
            TRN_DocumentConversion ret = IntPtr.Zero;
            TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertStreamingPdfConversionWithPdfAndFilter(ref doc.mp_doc, inData.mp_imp, opt_ptr, ref ret));
            return new DocumentConversion(ret);
        }
        
        /// <summary>
        /// Convert the specified plain text file to PDF and append converted pages to the specified PDF document.  
        /// </summary>
        /// <param name="in_pdfdoc"> the PDFDoc to append to </param>
        /// <param name="in_filename"> the path to the plain text document to convert </param>
        /// <remarks>
        /// in_options the conversion options. The availble options are:
        ///| Option Name             | Type    | Note                                                    |
        ///|-------------------------|---------|---------------------------------------------------------|
        ///| BytesPerBite            | Integer | In bytes. Use for streaming conversion only.            |
        ///| FontFace                | String  | Set the font face used for the conversion.              |
        ///| FontSize                | Integer | Set the font size used for the conversion.              |
        ///| LineHeightMultiplier    | Double  | Set the line height multiplier used for the conversion. |
        ///| MarginBottom            | Double  | In inches. Set the bottom margin of the page.           |
        ///| MarginLeft              | Double  | In inches. Set the left margin of the page.             |
        ///| MarginRight             | Double  | In inches. Set the right margin of the page.            |
        ///| MarginTop               | Double  | In inches. Set the top margin of the page.              |
        ///| PageHeight              | Double  | In inches. Set the page height.                         |
        ///| PageWidth               | Double  | In inches. Set the page width.                          |
        ///| UseSourceCodeFormatting | Boolean | Set whether to use mono font for the conversion.        |
        /// </remarks>
        public static void FromText(PDFDoc in_pdfdoc, string in_filename, SDF.Obj options)
        {
            UString ustr = new UString(in_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertFromText(in_pdfdoc.mp_doc, ustr.mp_impl, options.mp_obj));
        }

#if (__DESKTOP__)

        /// <summary>
        /// Convert the specified CAD file to PDF and append converted pages to the specified PDF document.
        /// This conversion requires that the optional PDFTron CAD add-on module is avaialable.
        /// See also: the 'CADModule' class
        /// </summary>
        /// <param name="in_pdfdoc"> the PDFDoc to append to </param>
        /// <param name="in_filename"> the path to the CAD document to convert </param>
        /// <param name="options">The options to use when converting.</param>
        /// <remarks>
        /// See the `CADConvertOptions` class for the available options.
        /// </remarks>
        public static void FromCAD(PDFDoc in_pdfdoc, string in_filename, CADConvertOptions options)
        {
            UString ustr = new UString(in_filename);
            TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertFromCAD(in_pdfdoc.mp_doc, ustr.mp_impl, opt_ptr));
        }
#endif

        /// <summary> Convert the PDFDoc to EMF and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to EMF
        /// </param>
        /// <param name="in_filename">the path to the EMF files to create, one file per page
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  This method is available only on Windows platforms. </remarks>
        public static void ToEmf(PDFDoc in_pdfdoc, string in_filename)
        {
            UString ustr = new UString(in_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertDocToEmf(in_pdfdoc.mp_doc, ustr.mp_impl));
        }
        /// <summary> Convert the Page to EMF and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_page">the Page to convert to EMF
        /// </param>
        /// <param name="in_filename">the path to the EMF file to create
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  This method is available only on Windows platforms. </remarks>
        public static void ToEmf(Page in_page, string in_filename)
        {
            UString ustr = new UString(in_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertPageToEmf(in_page.mp_page, ustr.mp_impl));
        }

        /// <summary> Convert the PDFDoc to SVG and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to SVG
        /// </param>
        /// <param name="in_filename">the path to the SVG files to create, one file per page
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToSvg(PDFDoc in_pdfdoc, string in_filename)
        {
            ToSvg(in_pdfdoc, in_filename, new SVGOutputOptions());
        }

        /// <summary> Convert the PDFDoc to SVG and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to SVG
        /// </param>
        /// <param name="in_filename">the path to the SVG files to create, one file per page
        /// </param>
        /// <param name="in_options">the conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToSvg(PDFDoc in_pdfdoc, string in_filename, SVGOutputOptions in_options)
        {
            UString ustr = new UString(in_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertDocToSvgWithOptions(in_pdfdoc.mp_doc, ustr.mp_impl, in_options.m_obj));
        }

        /// <summary> Convert the Page to SVG and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_page">the Page to convert to SVG
        /// </param>
        /// <param name="in_filename">the path to the SVG file to create
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToSvg(Page in_page, string in_filename)
        {
            ToSvg(in_page, in_filename, new SVGOutputOptions());
        }

        /// <summary> Convert the Page to SVG and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_page">the Page to convert to SVG
        /// </param>
        /// <param name="in_filename">the path to the SVG file to create
        /// </param>
        /// <param name="in_options">the conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToSvg(Page in_page, string in_filename, SVGOutputOptions in_options)
        {
            UString ustr = new UString(in_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertPageToSvgWithOptions(in_page.mp_page, ustr.mp_impl, in_options.m_obj));
        }
        
        /// <summary> Convert the PDFDoc to XPS and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to XPS
        /// </param>
        /// <param name="in_filename">the path to the document to create
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToXps(PDFDoc in_pdfdoc, string in_filename)
        {
            ToXps(in_pdfdoc, in_filename, new XPSOutputOptions());
        }
        /// <summary> Convert the PDFDoc to XPS and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to XPS
        /// </param>
        /// <param name="in_filename">the path to the document to create
        /// </param>
        /// <param name="options">the conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToXps(PDFDoc in_pdfdoc, string in_filename, XPSOutputOptions options)
        {
            UString ustr = new UString(in_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertToXps(in_pdfdoc.mp_doc, ustr.mp_impl, options.m_obj));
        }
        /// <summary> Convert the file or document to XPS and write to the specified file.
        /// 
        /// </summary>
        /// <param name="in_inputFilename">the path to the document to be converted to XPS
        /// </param>
        /// <param name="in_outputFilename">the path to the output XPS file
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS.
        /// Formats that require external applications for conversion use the 
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static void ToXps(string in_inputFilename, string in_outputFilename)
        {
            ToXps(in_inputFilename, in_outputFilename, new XPSOutputOptions());
        }
        /// <summary> Convert the file or document to XPS and write to the specified file.
        /// 
        /// </summary>
        /// <param name="in_inputFilename">the path to the document to be converted to XPS
        /// </param>
        /// <param name="in_outputFilename">the path to the output XPS file
        /// </param>
        /// <param name="options">the conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS. 
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static void ToXps(string in_inputFilename, string in_outputFilename, XPSOutputOptions options)
        {
            UString in_ustr = new UString(in_inputFilename);
            UString out_ustr = new UString(in_outputFilename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertFileToXps(in_ustr.mp_impl, out_ustr.mp_impl, options.m_obj));
        }
        /// <summary> Convert the input file to XOD format and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_filename">the in_filename
        /// </param>
        /// <param name="out_filename">the out_filename
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static void ToXod(string in_filename, string out_filename)
        {
            ToXod(in_filename, out_filename, new XODOutputOptions());
        }
        /// <summary> Convert the input file to XOD format and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_filename">the in_filename
        /// </param>
        /// <param name="out_filename">the out_filename
        /// </param>
        /// <param name="options">the conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static void ToXod(string in_filename, string out_filename, XODOutputOptions options)
        {
            UString in_ustr = new UString(in_filename);
            UString out_ustr = new UString(out_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertFileToXod(in_ustr.mp_impl, out_ustr.mp_impl, options.m_obj));
        }

        /// <summary> Convert the PDFDoc to XOD format and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to XPS
        /// </param>
        /// <param name="out_filename">the out_filename
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static void ToXod(PDFDoc in_pdfdoc, string out_filename)
        {
            ToXod(in_pdfdoc, out_filename, new XODOutputOptions());
        }

        /// <summary> Convert the PDFDoc to XOD format and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to XPS
        /// </param>
        /// <param name="out_filename">the out_filename
        /// </param>
        /// <param name="options">the conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static void ToXod(PDFDoc in_pdfdoc, string out_filename, XODOutputOptions options)
        {
            UString ustr = new UString(out_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertToXod(in_pdfdoc.mp_doc, ustr.mp_impl, options.m_obj));
        }

        /// <summary> Generate a stream that incrementally converts the input file to XOD format.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to XPS
        /// </param>
        /// <returns>converted file in Stream
        /// </returns>			
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static Filters.Filter ToXod(PDFDoc in_pdfdoc)
        {
            return ToXod(in_pdfdoc, new XODOutputOptions());
        }

        /// <summary> Generate a stream that incrementally converts the input file to XOD format.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to XPS
        /// </param>
        /// <returns>converted file in Stream
        /// </returns>
        /// <param name="options">the conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static Filters.Filter ToXod(PDFDoc in_pdfdoc, XODOutputOptions options)
        {
            TRN_Filter result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertToXodStream(in_pdfdoc.mp_doc, options.m_obj, ref result)); // try
            return new Filters.Filter(result, null);
        }

        /// <summary> Generate a stream that incrementally converts the input file to XOD format.
        /// 
        /// </summary>
        /// <param name="in_filename">the in_filename
        /// </param>
        /// <returns>converted XPS file as Stream
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static Filters.Filter ToXod(string in_filename)
        {
            return ToXod(in_filename, new XODOutputOptions());
        }
        /// <summary> Generate a stream that incrementally converts the input file to XOD format.
        /// 
        /// </summary>
        /// <param name="in_filename">the in_filename
        /// </param>
        /// <returns>converted file as Stream
        /// </returns>
        /// <param name="options">the conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static Filters.Filter ToXod(string in_filename, XODOutputOptions options)
        {
            TRN_Filter result = IntPtr.Zero;
            UString ustr = new UString(in_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertFileToXodStream(ustr.mp_impl, options.m_obj, ref result));
            return new Filters.Filter(result, null);
        }

        /// <summary>Convert a file to HTML and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_filename">the file to convert to HTML
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static void ToHtml(string in_filename, string output_path)
        {
            ToHtml(in_filename, output_path, new HTMLOutputOptions());
        }

        /// <summary>Convert a file to HTML and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_filename">the file to convert to HTML
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <param name="options">the conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static void ToHtml(string in_filename, string output_path, HTMLOutputOptions options)
        {
            UString in_ustr = new UString(in_filename);
            UString out_ustr = new UString(output_path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertFileToHtml(in_ustr.mp_impl, out_ustr.mp_impl, options.m_obj));
        }

        /// <summary>Convert the PDF to HTML and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDF doc to convert to HTML
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToHtml(PDFDoc in_pdfdoc, string output_path)
        {
            ToHtml(in_pdfdoc, output_path, new HTMLOutputOptions());
        }

        /// <summary>Convert the PDF to HTML and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDF doc to convert to HTML
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <param name="options">the conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToHtml(PDFDoc in_pdfdoc, string output_path, HTMLOutputOptions options)
        {
            UString ustr = new UString(output_path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertToHtml(in_pdfdoc.mp_doc, ustr.mp_impl, options.m_obj));
        }

        /// <summary>Convert a file to EPUB format and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_filename">the file to convert to EPUB
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static void ToEpub(string in_filename, string output_path)
        {

        }

        /// <summary>Convert a file to EPUB format and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_filename">the file to convert to EPUB
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <param name="html_options">the HTML conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToEpub(string in_filename, string output_path, HTMLOutputOptions html_options)
        {
            ToEpub(in_filename, output_path, html_options, new EPUBOutputOptions());
        }

        /// <summary>Convert a file to EPUB format and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_filename">the file to convert to EPUB
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <param name="epub_options">the EPUB conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static void ToEpub(string in_filename, string output_path, EPUBOutputOptions epub_options)
        {
            ToEpub(in_filename, output_path, new HTMLOutputOptions(), epub_options);
        }

        /// <summary>Convert a file to EPUB format and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_filename">the file to convert to EPUB
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <param name="html_options">the HTML conversion options
        /// </param>
        /// <param name="epub_options">the EPUB conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS, PDF.
        /// Formats that require external applications for conversion use the
        /// Convert.Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static void ToEpub(string in_filename, string output_path, HTMLOutputOptions html_options, EPUBOutputOptions epub_options)
        {
            UString in_ustr = new UString(in_filename);
            UString out_ustr = new UString(output_path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertFileToEpub(in_ustr.mp_impl, out_ustr.mp_impl, html_options.m_obj, epub_options.m_obj));
        }

        /// <summary>Convert the PDFDoc to EPUB format and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to EPUB
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToEpub(PDFDoc in_pdfdoc, string output_path)
        {

        }

        /// <summary>Convert the PDFDoc to EPUB format and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to EPUB
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <param name="html_options">the HTML conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToEpub(PDFDoc in_pdfdoc, string output_path, HTMLOutputOptions html_options)
        {
            ToEpub(in_pdfdoc, output_path, html_options, new EPUBOutputOptions());
        }

        /// <summary>Convert the PDFDoc to EPUB format and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to EPUB
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <param name="epub_options">the EPUB conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToEpub(PDFDoc in_pdfdoc, string output_path, EPUBOutputOptions epub_options)
        {
            ToEpub(in_pdfdoc, output_path, new HTMLOutputOptions(), epub_options);
        }

        /// <summary>Convert the PDFDoc to EPUB format and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to convert to EPUB
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <param name="html_options">the HTML conversion options
        /// </param>
        /// <param name="epub_options">the EPUB conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToEpub(PDFDoc in_pdfdoc, string output_path, HTMLOutputOptions html_options, EPUBOutputOptions epub_options)
        {
            UString ustr = new UString(output_path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertToEpub(in_pdfdoc.mp_doc, ustr.mp_impl, html_options.m_obj, epub_options.m_obj));
        }

        /// <summary>Convert a file to multipage Tiff and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_filename">the file to convert to multipage Tiff
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToTiff(string in_filename, string output_path)
        {
            ToTiff(in_filename, output_path, new TiffOutputOptions());
        }

        /// <summary>Convert a file to multipage Tiff and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_filename">the file to convert to multipage Tiff
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <param name="options">the conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToTiff(string in_filename, string output_path, TiffOutputOptions options)
        {
            UString in_ustr = new UString(in_filename);
            UString out_ustr = new UString(output_path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertFileToTiff(in_ustr.mp_impl, out_ustr.mp_impl, options.m_obj));
        }

        /// <summary>Convert the PDF to multipage Tiff and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDF doc to convert to multipage Tiff
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToTiff(PDFDoc in_pdfdoc, string output_path)
        {
            ToTiff(in_pdfdoc, output_path, new TiffOutputOptions());
        }

        /// <summary>Convert the PDF to multipage Tiff and save to the specified path.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDF doc to convert to multipage Tiff
        /// </param>
        /// <param name="output_path">the path to where generated content will be stored
        /// </param>
        /// <param name="options">the conversion options
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void ToTiff(PDFDoc in_pdfdoc, string output_path, TiffOutputOptions options)
        {
            UString ustr = new UString(output_path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertToTiff(in_pdfdoc.mp_doc, ustr.mp_impl, options.m_obj));
        }

        /// <summary> Convert the file or document to PDF and append to the specified PDF document.
        /// 
        /// </summary>
        /// <param name="in_pdfdoc">the PDFDoc to append the converted document to. The
        /// PDFDoc can then be converted to XPS, EMF or SVG using the other functions
        /// in this class.
        /// </param>
        /// <param name="in_filename">the path to the document to be converted to XPS
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Internally formats include BMP, EMF, JPEG, PNG, TIF, XPS.
        /// Formats that require external applications for conversion use the
        /// Convert::Printer class and the PDFNet printer to be installed. This is
        /// only supported on Windows platforms.  Document formats in this category
        /// include RTF(MS Word or Wordpad), TXT (Notepad or Wordpad), DOC and DOCX
        /// (MS Word), PPT and PPTX (MS PowerPoint), XLS and XLSX (MS Excel),
        /// OpenOffice documents, HTML and MHT (Internet Explorer), PUB (MS Publisher),
        /// MSG (MS Outlook).</remarks>
        public static void ToPdf(PDFDoc in_pdfdoc, String in_filename)
        {
            UString ustr = new UString(in_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertToPdf(in_pdfdoc.mp_doc, ustr.mp_impl));
        }

        /// <summary> Utility function to determine if ToPdf will require the PDFNet
        /// printer to convert a specific file to PDF.
        /// 
        /// </summary>
        /// <param name="in_filename">the path to the document to be checked
        /// </param>
        /// <returns> true if ToPdf requires the printer to convert the file, false
        /// otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Current implementation looks only at the file extension not
        /// file contents. If the file extension is missing, false will be returned</remarks>
        public static bool RequiresPrinter(String in_filename)
        {
            bool result = false;
            UString ustr = new UString(in_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertRequiresPrinter(ustr.mp_impl, ref result));
            return result;
        }

        #region enums
        public enum FlattenThresholdFlag
        {
            /// <summary>
            /// Render (flatten) any text that is clipped or occluded.
            /// </summary>
            e_very_strict,
		    /// <summary>
		    /// Render text that are marginally clipped or occluded.
		    /// </summary>
		    e_strict,
		    /// <summary>
		    /// Render text that are somewhat clipped or occluded.
		    /// </summary>
		    e_default,
		    /// <summary>
		    /// Only render text that are seriously clipped or occluded.
		    /// </summary>
		    e_keep_most,
		    /// <summary>
		    /// Only render text that are completely occluded, or used as a clipping path.
		    /// </summary>
		    e_keep_all
        };

        public enum FlattenFlag
        {
            /// <summary>
            /// Disable flattening and convert all content as is.
            /// </summary>
            e_off,
            /// <summary>
            /// Feature reduce PDF to a simple two layer representation consisting 
            /// of a single background RGB image and a simple top text layer.
            /// </summary>
            e_simple,
            /// <summary>
            /// Feature reduce PDF while trying to preserve some 
            /// complex PDF features (such as vector figures, transparency, shadings, 
            /// blend modes, Type3 fonts etc.) for pages that are already fast to render. 
            /// This option can also result in smaller &amp; faster files compared to e_simple,
            /// but the pages may have more complex structure.
            /// </summary>
            e_fast,
            /// <summary>
            /// Preserve vector content where possible. In particular only feature reduce
            /// PDF files containing overprint or very complex vector content. Currently this
            /// option can only be used with XODOutputOptions.
            /// </summary>
            e_high_quality
        };
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // Clean up native resources

                disposedValue = true;
            }
        }

        ~Convert()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion



        internal static void SetFlattenContentImpl(TRN_Obj obj, FlattenFlag flatten, ref TRN_Obj result)
        {
            switch (flatten)
            {
                case FlattenFlag.e_off:
                    PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(obj, "FLATTEN_CONTENT", "OFF", ref result));
                    break;
                case FlattenFlag.e_simple:
                    PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(obj, "FLATTEN_CONTENT", "SIMPLE", ref result));
                    break;
                case FlattenFlag.e_fast:
                    PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(obj, "FLATTEN_CONTENT", "FAST", ref result));
                    break;
                case FlattenFlag.e_high_quality:
                    PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(obj, "FLATTEN_CONTENT", "HIGH_QUALITY", ref result));
                    break;
            }
        }

        internal static void SetFlattenThresholdImpl(TRN_Obj obj, FlattenThresholdFlag threshold, ref TRN_Obj result)
        {
            switch (threshold)
            {
                case FlattenThresholdFlag.e_very_strict:
                    PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(obj, "FLATTEN_THRESHOLD", "VERY_STRICT", ref result));
                    break;
                case FlattenThresholdFlag.e_strict:
                    PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(obj, "FLATTEN_THRESHOLD", "STRICT", ref result));
                    break;
                case FlattenThresholdFlag.e_default:
                    PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(obj, "FLATTEN_THRESHOLD", "DEFAULT", ref result));
                    break;
                case FlattenThresholdFlag.e_keep_most:
                    PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(obj, "FLATTEN_THRESHOLD", "KEEP_MOST", ref result));
                    break;
                case FlattenThresholdFlag.e_keep_all:
                    PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(obj, "FLATTEN_THRESHOLD", "KEEP_ALL", ref result));
                    break;
            }
        }

        internal static void SetOverprintImpl(TRN_Obj obj, PDFRasterizer.OverprintPreviewMode mode, ref TRN_Obj result)
        {
            switch (mode)
            {
                case PDFRasterizer.OverprintPreviewMode.e_op_off:
                    PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(obj, "OVERPRINT_MODE", "OFF", ref result));
                    break;
                case PDFRasterizer.OverprintPreviewMode.e_op_on:
                    PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(obj, "OVERPRINT_MODE", "ON", ref result));
                    break;
                case PDFRasterizer.OverprintPreviewMode.e_op_pdfx_on:
                    PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(obj, "OVERPRINT_MODE", "PDFX", ref result));
                    break;
            }
        }

        public class SVGOutputOptions
        {
            internal TRN_Obj m_obj;
            internal SDF.ObjSet m_objset;
            /// <summary>
            /// Creates an SVGOutputOptions object with default settings
            /// </summary>
            public SVGOutputOptions()
            {
                m_objset = new SDF.ObjSet();
                m_obj = m_objset.CreateDict().mp_obj;
            }

            /// <summary>
            /// Sets whether to embed all images
            /// </summary>
            /// <param name="embed_images">if true, images will be embeded. Default is false.</param>
            public void SetEmbedImages(bool embed_images)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "EMBEDIMAGES", embed_images, ref result));
            }

            /// <summary>
            /// Sets whether to disable conversion of font data to SVG
            /// </summary>
            /// <param name="no_fonts">if true, font data conversion is disabled. Default is false.</param>
            public void SetNoFonts(bool no_fonts)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "NOFONTS", no_fonts, ref result));
            }

            /// <summary>
            /// Sets whether to convert all fonts to SVG or not.
            /// </summary>
            /// <param name="svg_fonts">if true, fonts are converted to SVG. Otherwise they are converted to OpenType. Default is false.</param>
            public void SetSvgFonts(bool svg_fonts)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "SVGFONTS", svg_fonts, ref result));
            }

            /// <summary>
            /// Sets whether to embed fonts into each SVG page file, or to have them shared.
            /// </summary>
            /// <param name="embed_fonts">if true, fonts are injected into each SVG page. 
            /// Otherwise they are created as separate files that are shared between SVG pages. Default is false.</param>
            public void SetEmbedFonts(bool embed_fonts)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "EMBEDFONTS", embed_fonts, ref result));
            }

            /// <summary>
            /// Sets whether to use the font/font-family naming scheme as obtained from the source file.
            /// </summary>
            /// <param name="preserve">if true, font family names are preserved. Default is false.</param>
            public void SetPreserveFontFamilyNames(bool preserve)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "PRESERVEFONTNAME", preserve, ref result));
            }

            /// <summary>
            /// Sets whether to disable mapping of text to public Unicode region. Instead text will be converted using a custom encoding
            /// </summary>
            /// <param name="no_unicode">if true, mapping of text to public Unicode region is disabled. Default is false.</param>
            public void SetNoUnicode(bool no_unicode)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "NOUNICODE", no_unicode, ref result));
            }

            /// <summary>
            /// Some viewers do not support the default text positioning correctly. This option works around this issue to place text correctly, but produces verbose output. This option will override SetRemoveCharPlacement
            /// </summary>
            /// <param name="individual_char_placement">if true, text will be positioned correctly</param>
            public void SetIndividualCharPlacement(bool individual_char_placement)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "INDIVIDUALCHARPLACEMENT", individual_char_placement, ref result));
            }

            /// <summary>
            /// Sets whether to disable the output of character positions.  This will produce slightly smaller output files than the default setting, but many viewers do not support the output correctly
            /// </summary>
            /// <param name="remove_char_placement">if true, the output of character positions is disabled</param>
            public void SetRemoveCharPlacement(bool remove_char_placement)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "REMOVECHARPLACEMENT", remove_char_placement, ref result));
            }

            /// <summary>
            /// Flatten images and paths into a single background image overlaid with 
            /// vector text. This option can be used to improve speed on devices with 
            /// little processing power such as iPads. Default is e_fast.
            /// </summary>
            /// <param name="flatten"> select which flattening mode to use.</param>
            public void SetFlattenContent(FlattenFlag flatten)
            {
                TRN_Obj result = IntPtr.Zero;
                SetFlattenContentImpl(m_obj, flatten, ref result);
            }

            /// <summary>
            /// Used to control how precise or relaxed text flattening is. When some text is 
            /// preserved (not flattened to image) the visual appearance of the document may be altered.
            /// </summary>
            /// <param name="threshold"> the threshold setting to use.</param>
            public void SetFlattenThreshold(FlattenThresholdFlag threshold)
            {
                TRN_Obj result = IntPtr.Zero;
                SetFlattenThresholdImpl(m_obj, threshold, ref result);
            }

            /// <summary>
            /// The output resolution, from 1 to 1000, in Dots Per Inch (DPI) at which to render elements which cannot be directly converted. 
            /// Default is 140.
            /// </summary>
            /// <param name="dpi">  the resolution in Dots Per Inch
            /// </param>
            public void SetFlattenDPI(UInt32 dpi)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "DPI", dpi, ref result));
            }

            /// <summary>
            /// Specifies the maximum image size in pixels. Default is 2000000.
            /// </summary>
            /// <param name="max_pixels"> the maximum number of pixels an image can have.
            /// </param>
            public void SetFlattenMaximumImagePixels(UInt32 max_pixels)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "MAX_IMAGE_PIXELS", max_pixels, ref result));
            }

            /// <summary>
            /// Compress output SVG files using SVGZ.
            /// </summary>
            /// <param name="svgz"> if true, SVG files are written in compressed format. Default is false.</param>
            public void SetCompress(bool svgz)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "SVGZ", svgz, ref result));
            }

            /// <summary>
            /// Sets whether per page thumbnails should be included in the file. Default is true.
            /// </summary>
            /// <param name="include_thumbs"> if true thumbnails will be included</param>
            public void SetOutputThumbnails(bool include_thumbs)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "NOTHUMBS", !include_thumbs, ref result));
            }

            /// <summary>
            /// The maximum dimension for thumbnails.
            /// </summary>
            /// <param name="size"> the maximum dimension (width or height) that thumbnails will have. Default is 400.</param>
            public void SetThumbnailSize(UInt32 size)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "THUMB_SIZE", size, ref result));
            }

            /// <summary>
            ///Create a XML document that contains metadata of the SVG document created.
            /// </summary>
            /// <param name="xml"> if true, XML wrapper is created. Default is true.</param>
            public void SetCreateXmlWrapper(bool xml)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "NOXMLDOC", !xml, ref result));
            }

            /// <summary>
            /// Set whether the DTD declaration is included in the SVG files.
            /// </summary>
            /// <param name="dtd"> if false, no DTD is added to SVG files. Default is true.</param>
            public void SetDtd(bool dtd)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "OMITDTD", !dtd, ref result));
            }

            /// <summary>
            /// Control generation of form fields and annotations in SVG.
            /// </summary>
            /// <param name="annots"> if false, no form fields or annotations are converted. Default is true.</param>
            public void SetAnnots(bool annots)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "NOANNOTS", !annots, ref result));
            }

            /// <summary>
            /// Enable or disable support for overprint and overprint simulation. 
            /// Overprint is a device dependent feature and the results will vary depending on 
            ///	the output color space and supported colorants (i.e. CMYK, CMYK+spot, RGB, etc).
            /// Default is e_op_pdfx_on.
            /// </summary>
            /// <param name="mode"> op e_op_on: always enabled; e_op_off: always disabled; 
            /// e_op_pdfx_on: enabled for PDF/X files only.
            /// </param>
            public void SetOverprint(PDFRasterizer.OverprintPreviewMode mode)
            {
                TRN_Obj result = IntPtr.Zero;
                SetOverprintImpl(m_obj, mode, ref result);
            }
        }

        public class XPSOutputCommonOptions
        {
            internal TRN_Obj m_obj;
            internal SDF.ObjSet m_objset;
            /// <summary>
            /// Creates an XPSConvertOptions object with default settings
            /// </summary>
            /// 
            public XPSOutputCommonOptions()
            {
                m_objset = new SDF.ObjSet();
                m_obj = m_objset.CreateDict().mp_obj;
            }
            /// <summary>
            /// Sets whether ToXps should be run in print mode. Default is false.
            /// </summary>
            /// <param name="print_mode">  if true print mode is enabled
            /// </param>
            public void SetPrintMode(bool print_mode)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "PRINTMODE", print_mode, ref result));
            }

            /// <summary>
            /// The output resolution, from 1 to 1000, in Dots Per Inch (DPI) at which to render elements which cannot be directly converted. 
            /// Default is 140.
            /// </summary>
            /// <param name="dpi">  the resolution in Dots Per Inch
            /// </param>
            public void SetDPI(UInt32 dpi)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "DPI", dpi, ref result));
            }

            /// <summary>
            /// Sets whether rendering of pages should be permitted when necessary to guarantee output
            /// the default setting is to allow rendering in this case. Default is true.
            /// </summary>
            /// <param name="render">  if false rendering is not permitted under any circumstance
            /// </param>
            public void SetRenderPages(bool render)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "RENDER", render, ref result));
            }

            /// <summary>
            /// Sets whether thin lines should be thickened. Default is true for XPS and false for XOD.
            /// </summary>
            /// <param name="thicken">  if true then thin lines will be thickened
            /// </param>
            public void SetThickenLines(bool thicken)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "THICKENLINES", thicken, ref result));
            }

            /// <summary>
            /// Sets whether links should be generated from urls
            /// found in the document. Default is false.
            /// </summary>
            /// <param name="generate"> if true links will be generated from urls 
            ///	</param>
            public void GenerateURLLinks(bool generate)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "URL_LINKS", generate, ref result));
            }

            /// <summary>
            /// Enable or disable support for overprint and overprint simulation. 
            /// Overprint is a device dependent feature and the results will vary depending on 
            ///	the output color space and supported colorants (i.e. CMYK, CMYK+spot, RGB, etc).
            /// Default is e_op_pdfx_on.
            /// </summary>
            /// <param name="mode"> op e_op_on: always enabled; e_op_off: always disabled; 
            /// e_op_pdfx_on: enabled for PDF/X files only.
            /// </param>
            public void SetOverprint(PDFRasterizer.OverprintPreviewMode mode)
            {
                TRN_Obj result = IntPtr.Zero;
                SetOverprintImpl(m_obj, mode, ref result);
            }
        }

        public class XPSOutputOptions : XPSOutputCommonOptions
        {
            public void SetOpenXps(bool openxps)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "OPENXPS", openxps, ref result));
            }
        }

        public class XODOutputOptions : XPSOutputCommonOptions
        {
            public enum AnnotationOutputFlag
            {
                ///<summary>include the annotation file in the XOD output. This is the default option.</summary>
                e_internal_xfdf = 0,
                ///<summary>output the annotation file externally to the same output path with extension .xfdf. This is not available when using streaming conversion</summary>
                e_external_xfdf = 1,
                ///<summary>flatten all annotations that are not link annotations</summary>
                e_flatten = 2
            }

            /// <summary>
            /// Sets whether per page thumbnails should be included in the file. Default is true.
            /// </summary>
            /// <param name="include_thumbs"> if true thumbnails will be included
            /// </param>
            public void SetOutputThumbnails(bool include_thumbs)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "NOTHUMBS", !include_thumbs, ref result));
            }
            /// <summary>
            /// The width and height of squares in which thumbnails will be contained. Default is 400 for normal pages and 1500 for large pages.
            /// </summary>
            /// <remarks>A large page is a page larger than twice the area of the standard page size (8.5 X 11).</remarks>
            /// <param name="size"> size the maximum dimension (width or height) that thumbnails will have.
            /// </param>
            public void SetThumbnailSize(UInt32 size)
            {
                SetThumbnailSize(size, size);
            }
            /// <summary>
            /// The width and height of squares in which thumbnails will be contained. Default is 400 for normal pages and 1500 for large pages.
            /// </summary>
            /// <param name="regular_size"> the maximum dimension that thumbnails for regular size pages will have.
            /// </param>
            /// <remarks>A large page is a page larger than twice the area of the standard page size (8.5 X 11).</remarks>
            /// <param name="large_size"> the maximum dimension that thumbnails for large pages will have.
            /// </param>
            public void SetThumbnailSize(UInt32 regular_size, UInt32 large_size)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "THUMB_SIZE", regular_size, ref result));
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "LARGE_THUMB_SIZE", large_size, ref result));
            }
            /// <summary>
            /// If rendering is permitted, sets the maximum number of page elements before that page will be rendered.
            /// Default is 2000000000 which will never cause pages to be completely rendered in this manner.
            /// </summary>
            /// <param name="element_limit"> the maximum number of elements before a given page will be rendered
            /// </param>
            public void SetElementLimit(UInt32 element_limit)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "ELEMENTLIMIT", element_limit, ref result));
            }
            /// <summary>
            /// If rendering is permitted, sets whether pages containing opacity masks should be rendered.
            /// This option is used as a workaround to a bug in Silverlight where opacity masks are transformed incorrectly.
            /// the default setting is not to render pages with opacity masks. Default is false. 
            /// </summary>
            /// <param name="opacity_render"> if true pages with opacity masks will be rendered
            /// </param>
            public void SetOpacityMaskWorkaround(bool opacity_render)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "MASKRENDER", opacity_render, ref result));
            }
            /// <summary>
            /// Specifies the maximum image size in pixels. Default is 2000000.
            /// </summary>
            /// <remarks>This setting now will no longer reduce the total number of image pixels.
            /// Instead a lower value will just produce more slices and vice versa. </remarks>
            /// <remarks>Since image compression works better with more pixels a larger
            /// max pixels should generally create smaller files. </remarks>
            /// <param name="max_pixels"> the maximum number of pixels an image can have.
            /// </param>
            public void SetMaximumImagePixels(UInt32 max_pixels)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "MAX_IMAGE_PIXELS", max_pixels, ref result));
            }
            /// <summary>
            /// Flatten images and paths into a single background image overlaid with 
            /// vector text. This option can be used to improve speed on devices with 
            /// little processing power such as iPads. Default is e_high_quality.
            /// </summary>
            /// <param name="flatten"> select which flattening mode to use.
            /// </param>
            public void SetFlattenContent(FlattenFlag flatten)
            {
                TRN_Obj result = IntPtr.Zero;
                SetFlattenContentImpl(m_obj, flatten, ref result);
            }
            /// <summary>
            /// Used to control how precise or relaxed text flattening is. When some text is 
            /// preserved (not flattened to image) the visual appearance of the document may be altered.
            /// </summary>
            /// <param name="threshold"> the threshold setting to use.
            /// </param>
            public void SetFlattenThreshold(FlattenThresholdFlag threshold)
            {
                TRN_Obj result = IntPtr.Zero;
                SetFlattenThresholdImpl(m_obj, threshold, ref result);
            }
            /// <summary>
            /// Where possible output JPG files rather than PNG. This will apply to both thumbnails and document images. Default is true.
            /// </summary>
            /// <param name="prefer_jpg"> if true JPG images will be used whenever possible.
            /// </param>
            public void SetPreferJPG(bool prefer_jpg)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "PREFER_JPEG", prefer_jpg, ref result));
            }
            /// <summary>
            /// Specifies the compression quality to use when generating JPEG images.
            /// </summary>
            /// <param name="quality"> the JPEG compression quality, from 0(highest compression) to 100(best quality).
            /// </param>
            public void SetJPGQuality(UInt32 quality)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "JPEG_QUALITY", quality, ref result));
            }
            /// <summary>
            /// Outputs rotated text as paths. This option is used as a workaround to a bug in Silverlight 
            /// where pages with rotated text could cause the plugin to crash. Default is false.
            /// </summary>
            /// <param name="workaround"> if true rotated text will be changed to paths.
            /// </param>
            public void SetSilverlightTextWorkaround(bool workaround)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "REMOVE_ROTATED_TEXT", workaround, ref result));
            }
            /// <summary>
            /// Choose how to output annotations. Default is e_internal_xfdf.
            /// </summary>
            /// <param name="annot_output"> the chosen annotation output option
            /// </param>
            public void SetAnnotationOutput(AnnotationOutputFlag annot_output)
            {
                TRN_Obj result = IntPtr.Zero;
                switch (annot_output)
                {
                    case AnnotationOutputFlag.e_internal_xfdf:
                        PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "ANNOTATION_OUTPUT", "INTERNAL", ref result));
                        break;
                    case AnnotationOutputFlag.e_external_xfdf:
                        PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "ANNOTATION_OUTPUT", "EXTERNAL", ref result));
                        break;
                    case AnnotationOutputFlag.e_flatten:
                        PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "ANNOTATION_OUTPUT", "FLATTEN", ref result));
                        break;
                }
            }
            /// <summary>
            /// Output XOD as a collection of loose files rather than a zip archive. 
            /// This option should be used when using the external part retriever in Webviewer. Default is false.
            /// </summary>
            /// <param name="generate"> if true XOD is output as a collection of loose files.
            /// </param>
            public void SetExternalParts(bool generate)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "EXTERNAL_PARTS", generate, ref result));
            }
            /// <summary>
            /// Encrypt XOD parts with AES 128 encryption using the supplied password. 
            /// </summary>
            /// <param name="pass"> the encryption password.
            /// </param>
            public void SetEncryptPassword(string pass)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "ENCRYPT_PASSWORD", pass, ref result));
            }
            /// <summary>
            /// The latest XOD format is only partially supported in Silverlight and Flash
            ///	due to various optimizations in the text format and the addition of blend mode support. 
            ///	this option forces the converter to use an older version of XOD that is Silverlight/Flash compatible
            ///	but does not have these improvements. By default the latest XOD format is generated.
            /// </summary>
            /// <param name="compatible"> if true will use the older XOD format which is not compatible with Silverlight/Flash.
            /// </param>
            public void UseSilverlightFlashCompatible(bool compatible)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "COMPATIBLE_XOD", compatible, ref result));
            }
        }

        /// <summary>
        /// A class containing options for PDF to HTML conversion
        /// </summary>
        public class HTMLOutputOptions
        {
            internal TRN_Obj m_obj;
            internal SDF.ObjSet m_objset;
            /// <summary>
            /// Creates an HTMLOutputOptions object with default settings
            /// </summary>
            /// 
            public HTMLOutputOptions()
            {
                m_objset = new SDF.ObjSet();
                m_obj = m_objset.CreateDict().mp_obj;
            }
            /// <summary>
            ///  Use JPG files rather than PNG. This will apply to all generated images. Default is true.
            /// </summary>
            /// <param name="prefer_jpg"> if true JPG images will be used whenever possible
            /// </param>
            public void SetPreferJPG(bool prefer_jpg)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "PREFER_JPEG", prefer_jpg, ref result));
            }
            /// <summary>
            /// Specifies the compression quality to use when generating JPEG images.
            /// </summary>
            /// <param name="quality"> the JPEG compression quality, from 0(highest compression) to 100(best quality).
            /// </param>
            public void SetJPGQuality(UInt32 quality)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "JPEG_QUALITY", quality, ref result));
            }
            /// <summary>
            /// The output resolution, from 1 to 1000, in Dots Per Inch (DPI) at which to render elements which cannot be directly converted. 
            /// Default is 140.
            /// </summary>
            /// <param name="dpi"> the resolution in Dots Per Inch
            /// </param>
            public void SetDPI(UInt32 dpi)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "DPI", dpi, ref result));
            }

            /// <summary>
            /// Specifies the maximum image size in pixels. Default is 2000000.
            /// </summary>
            /// <param name="max_pixels"> the maximum number of pixels an image can have
            /// </param>
            public void SetMaximumImagePixels(UInt32 max_pixels)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "MAX_IMAGE_PIXELS", max_pixels, ref result));
            }

            /// <summary>
            /// Switch between fixed (pre-paginated) and reflowable HTML generation. Default is false.
            /// </summary>
            /// <param name="reflow"> if true, generated HTML will be reflowable, otherwise, fixed positioning will be used
            ///	</param>
            public void SetReflow(bool reflow)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "REFLOW", reflow, ref result));
            }

            /// <summary>
            /// Set an overall scaling of the generated HTML pages. Default is 1.0.
            /// </summary>
            ///  <param name="scale">A number greater than 0 which is used as a scale factor. For example, calling SetScale(0.5) will reduce the HTML body of the page to half its original size, whereas SetScale(2) will double the HTML body dimensions of the page and will rescale all page content appropriately.
            /// </param>
            public void SetScale(double scale)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "SCALE", scale, ref result));
            }

            /// <summary>
            /// Enable the conversion of external URL navigation. Default is false.
            /// </summary>
            /// <param name="enable"> if true, links that specify external URL's are converted into HTML.
            ///	</param>
            public void SetExternalLinks(bool enable)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "EXTERNAL_LINKS", enable, ref result));
            }

            /// <summary>
            /// Enable the conversion of internal document navigation. Default is false.
            /// </summary>
            /// <param name="enable"> if true, links that specify page jumps are converted into HTML.
            ///	</param>
            public void SetInternalLinks(bool enable)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "INTERNAL_LINKS", enable, ref result));
            }

            /// <summary>
            /// Controls whether converter optimizes DOM or preserves text placement accuracy. Default is false.
            /// </summary>
            /// <param name="enable">If true, converter will try to reduce DOM complexity at the expense of text placement accuracy.</param>
            public void SetSimplifyText(bool enable)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "SIMPLIFY_TEXT", enable, ref result));
            }

            /// <summary>
            /// Generate a XML file that contains additional information about the conversion process. By default no report is generated.
            /// </summary>
            /// <param name="path">The file path to which the XML report is written to.</param>
            public void SetReportFile(string path)
            {
                TRN_Obj result = IntPtr.Zero;
                UString ustr = new UString(path);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutText(m_obj, "REPORT_FILE", ustr.mp_impl, ref result));
            }
        }

        /// <summary>
        /// A class containing options for PDF to Tiff conversion
        /// </summary>
        public class TiffOutputOptions
        {
            internal TRN_Obj m_obj;
            internal SDF.ObjSet m_objset;
            /// <summary>
            /// Creates an TiffOutputOptions object with default settings
            /// </summary>
            /// 
            public TiffOutputOptions()
            {
                m_objset = new SDF.ObjSet();
                m_obj = m_objset.CreateDict().mp_obj;
            }

            /// <summary>
            /// Specifies the page box/region to rasterize.
            /// Possible values are media, crop, trim,
            /// bleed, and art.  By default, page crop
            /// region will be rasterized.
            /// </summary>
            /// <param name="type"> The type cf box to use</param>
            public void SetBox(Page.Box type)
            {
                TRN_Obj result = IntPtr.Zero;
                switch (type)
                {
                    case Page.Box.e_media:
                        PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "BOX", "media", ref result));
                        break;
                    case Page.Box.e_crop:
                        PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "BOX", "crop", ref result));
                        break;
                    case Page.Box.e_bleed:
                        PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "BOX", "bleed", ref result));
                        break;
                    case Page.Box.e_trim:
                        PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "BOX", "trim", ref result));
                        break;
                    case Page.Box.e_art:
                        PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "BOX", "art", ref result));
                        break;
                }
            }

            /// <summary>
            /// Rotates all pages by a given number of
            /// degrees counterclockwise. The allowed
            /// values are 0, 90, 180, and 270. The default
            /// value is 0.
            /// </summary>
            /// <param name="rotate"> The type cf box to use</param>
            public void SetRotate(Page.Rotate rotate)
            {
                TRN_Obj result = IntPtr.Zero;
                switch (rotate)
                {
                    case Page.Rotate.e_0:
                        PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "ROTATE", "0", ref result));
                        break;
                    case Page.Rotate.e_90:
                        PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "ROTATE", "90", ref result));
                        break;
                    case Page.Rotate.e_180:
                        PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "ROTATE", "180", ref result));
                        break;
                    case Page.Rotate.e_270:
                        PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "ROTATE", "270", ref result));
                        break;
                }
            }

            /// <summary>
            /// User definable clip box. By default, the
            /// clip region is identical to current page
            /// 'box'.
            /// </summary>
            /// <param name="x1"> The value of the x1 coordinate</param>
            /// <param name="y1"> The value of the y1 coordinate</param>
            /// <param name="x2"> The value of the x2 coordinate</param>
            /// <param name="y2"> The value of the y2 coordinate</param>
            public void SetClip(double x1, double y1, double x2, double y2)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "CLIP_X1", x1, ref result));
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "CLIP_Y1", y1, ref result));
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "CLIP_X2", x2, ref result));
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "CLIP_Y2", y2, ref result));
            }

            /// <summary>
            /// Specifies the list of pages to convert. By
            /// default, all pages are converted.
            /// </summary>
            /// <param name="page_desc"> A description of the pages to be converted.</param>
            public void SetPages(string page_desc)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutName(m_obj, "PAGES", page_desc, ref result));
            }


            /// <summary> 
            /// Enable or disable support for overprint and overprint simulation. 
            /// Overprint is a device dependent feature and the results will vary depending on 
            /// the output color space and supported colorants (i.e. CMYK, CMYK+spot, RGB, etc). 
            /// Default is e_op_pdfx_on.
            /// </summary>
            /// <param name="mode"> e_op_on: always enabled; e_op_off: always disabled; e_op_pdfx_on: enabled for PDF/X files only.</param>
            public void SetOverprint(PDFRasterizer.OverprintPreviewMode mode)
            {
                TRN_Obj result = IntPtr.Zero;
                SetOverprintImpl(m_obj, mode, ref result);
            }

            /// <summary>
            /// Render and export the image in CMYK mode.
            /// By default, the image is rendered and
            /// exported in RGB color space.
            /// </summary>
            /// <param name="enable"> if true then cmyk will be enabled</param>
            public void SetCMYK(bool enable)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "CMYK", enable, ref result));
            }

            /// <summary>
            /// Enables dithering when the image is
            /// exported in palletized or monochrome mode.
            /// This option is disabled by default.
            /// </summary>
            /// <param name="enable"> if true then dithering will be enabled</param>
            public void SetDither(bool enable)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "DITHER", enable, ref result));
            }

            /// <summary>
            /// Render and export the image in grayscale
            /// mode. Sets pixel format to 8 bits per pixel
            /// grayscale. By default, the image is
            /// rendered and exported in RGB color space.
            /// </summary>
            /// <param name="enable"> if true then grayscale will be enabled</param>
            public void SetGray(bool enable)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "GRAY", enable, ref result));
            }

            /// <summary>
            /// Export the rendered image as 1 bit per
            /// pixel (monochrome) image. The image will be
            /// compressed using G4 CCITT compression
            /// algorithm. By default, the image is not
            /// dithered. To enable dithering use
            /// 'SetDither' option. This option is disabled by
            /// default.
            /// </summary>
            /// <param name="enable"> if true then monochrome will be enabled</param>
            public void SetMono(bool enable)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "MONO", enable, ref result));
            }

            /// <summary>
            /// Enables or disables drawing of
            /// annotations.
            /// This option is enabled by default.
            /// </summary>
            /// <param name="enable"> if false then annotations will not be drawn</param>
            public void SetAnnots(bool enable)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "ANNOTS", enable, ref result));
            }

            /// <summary>
            /// Enables or disables image
            /// smoothing (default: enabled).
            /// </summary>
            /// <param name="enable"> if false then images will not be smoothed</param>
            public void SetSmooth(bool enable)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "SMOOTH", enable, ref result));
            }

            /// <summary>
            /// Renders annotations in the print mode. This
            /// option can be used to render 'Print Only'
            /// annotations and to hide 'Screen Only'
            /// annotations.
            /// This option is disabled by default.
            /// </summary>
            /// <param name="enable"> if true then print mode will be enabled</param>
            public void SetPrintmode(bool enable)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "PRINTMODE", enable, ref result));
            }

            /// <summary>
            /// Sets the page color to transparent. By
            /// default, Convert assumes that the page is
            /// drawn directly on an opaque white surface.
            /// Some applications may need to draw the page
            /// on a different backdrop. In this case any
            /// pixels that are not covered during
            /// rendering will be transparent.
            /// This option is disabled by default.
            /// </summary>
            /// <param name="enable"> if true then transparent page will be enabled</param>
            public void SetTransparentPage(bool enable)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "TRANSPARENT_PAGE", enable, ref result));
            }

            /// <summary>
            /// Enabled the output of palettized TIFFs.
            /// This option is disabled by default.
            /// </summary>
            /// <param name="enable"> if true then the TIFF will be palettized</param>
            public void SetPalettized(bool enable)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "PALETTIZED", enable, ref result));
            }

            /// <summary>
            /// The output resolution, from 1 to 1000, in Dots Per Inch (DPI). The
            /// higher the DPI, the larger the image. Resolutions larger than 1000 DPI can
            /// be achieved by rendering image in tiles or stripes. The default resolution
            /// is 92 DPI.
            /// </summary>
            /// <param name="dpi"> specifies the output DPI</param>
            public void SetDPI(double dpi)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "DPI", dpi, ref result));
            }

            /// <summary>
            /// Sets the gamma factor used for anti-aliased
            /// rendering. Typical values are in the range
            /// from 0.1 to 3. Gamma correction can be used
            /// to improve the quality of anti-aliased
            /// image output and can (to some extent)
            /// decrease the appearance common
            /// anti-aliasing artifacts (such as pixel
            /// width lines between polygons).  The default
            /// gamma is 0.
            /// </summary>
            /// <param name="gamma"> specifies the gamma factor</param>
            public void SetGamma(double gamma)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "GAMMA", gamma, ref result));
            }

            /// <summary>
            /// Sets the width of the output image, in pixels. 
            /// </summary>
            /// <param name="hres"> specifies the width</param>
            public void SetHRes(Int32 hres)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "HRES", hres, ref result));
            }

            /// <summary>
            /// Sets the height of the output image, in pixels. 
            /// </summary>
            /// <param name="vres"> specifies the width</param>
            public void SetVRes(Int32 vres)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutNumber(m_obj, "VRES", vres, ref result));
            }
        }

        /// <summary>
        /// A class containing options for PDF to EPUB conversion
        /// </summary>
        public class EPUBOutputOptions
        {
            internal TRN_Obj m_obj;
            internal SDF.ObjSet m_objset;
            /// <summary>
            /// Creates an EPUBOutputOptions object with default settings
            /// </summary>
            /// 
            public EPUBOutputOptions()
            {
                m_objset = new SDF.ObjSet();
                m_obj = m_objset.CreateDict().mp_obj;
            }
            /// <summary>
            /// Create the EPUB in expanded format. Default is false.
            /// </summary>
            /// <param name="expanded">if false a single EPUB file will be generated, otherwise, 
            /// the generated EPUB will be in unzipped (expanded) format
            /// </param>
            public void SetExpanded(bool expanded)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "EPUB_EXPANDED", expanded, ref result));
            }

            /// <summary>
            /// Set whether the first content page in the EPUB uses the cover image or not. If this
            /// is set to true, then the first content page will simply wrap the cover image in HTML.
            /// Otherwise, the page will be converted the same as all other pages in the EPUB. Default is false.
            /// </summary>
            /// <param name="reuse"> if true the first page will simply be EPUB cover image, 
            /// otherwise, the first page will be converted the same as the other pages.
            /// </param>
            public void SetReuseCover(bool reuse)
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjPutBool(m_obj, "EPUB_REUSE_COVER", reuse, ref result));
            }
        }

        /// <summary>Convert.Printer is a utility class to install the a printer for print-based conversion of documents for Convert.ToPdf.
        /// </summary>
        public class Printer
        {
            /// <summary>Install the PDFNet printer.
            /// Installation can take a few seconds, so it is recommended that you install the printer once as part of 
            /// your deployment process. Duplicated installations will be quick since the presence of the printer is checked 
            /// before installation is attempted. There is no need to uninstall the printer after conversions, it can be left 
            /// installed for later access.
            /// </summary>
            /// <param name="in_printerName">the name of the printer to install and use for conversions. If in_printerName is not provided then the name "PDFTron PDFNet" is used.
            /// </param>
            /// <remarks>Installing and uninstalling printer drivers requires the process to be running as administrator.
            /// </remarks>
            public static void Install(string in_printerName)
            {
                UString ustr = new UString(in_printerName);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertPrinterInstall(ustr.mp_impl));
            }
            /// <summary>Install the PDFNet printer.
            /// Installation can take a few seconds, so it is recommended that you install the printer once as part of 
            /// your deployment process. Duplicated installations will be quick since the presence of the printer is checked 
            /// before installation is attempted. There is no need to uninstall the printer after conversions, it can be left 
            /// installed for later access.
            /// </summary>
            /// <remarks>install PDFNet printer using "PDFTron PDFNet" as the printer name
            /// </remarks>
            public static void Install()
            {
                Install("PDFTron PDFNet");
            }
            /// <summary>Uninstall all printers using the PDFNet printer driver.
            /// </summary>
            /// <remarks>Installing and uninstalling printer drivers requires the process to be running as administrator.
            /// </remarks>
            public static void Uninstall()
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertPrinterUninstall());
            }
            /// <summary>Get the name of the PDFNet printer installed in this process session.
            /// </summary>
            /// <returns>the Unicode name of the PDFNet printer
            /// </returns>
            /// <remarks>if no printer was installed in this process then the predefined string "PDFTron PDFNet" will be returned.
            /// </remarks>
            public static string GetPrinterName()
            {
                UString result = new UString();
                PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertPrinterGetPrinterName(ref result.mp_impl));
                return result.ConvToManagedStr();
            }
            /// <summary>Set the name of the PDFNet printer installed in this process session.
            /// </summary>
            /// <param name="in_printerName">the Unicode name of the PDFNet printer
            /// </param>
            /// <remarks>if no printer was installed in this process then the predefined string "PDFTron PDFNet" will be used.
            /// </remarks>
            public static void SetPrinterName(string in_printerName)
            {
                UString ustr = new UString(in_printerName);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertPrinterSetPrinterName(ustr.mp_impl));
            }
            /// <summary>Set the name of the PDFNet printer installed in this process session to "PDFTron PDFNet"
            /// </summary>
            public static void SetPrinterName()
            {
                SetPrinterName("PDFTron PDFNet");
            }
            /// <summary>Determine if the PDFNet printer is installed.
            /// </summary>
            /// <param name="in_printerName">the name of the printer to install and use for conversions. 
            /// </param>
            /// <returns>true if the named printer is installed, false otherwise
            /// </returns>
            /// <remarks>may or may not check if the printer with the given name is actually a PDFNet printer.
            /// </remarks>
            public static bool IsInstalled(string in_printerName)
            {
                bool result = false;
                UString ustr = new UString(in_printerName);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertPrinterIsInstalled(ustr.mp_impl, ref result));
                return result;
            }
            /// <summary>Determine if the "PDFTron PDFNet" printer is installed.
            /// </summary>
            /// <returns>true if the named printer is installed, false otherwise
            /// </returns>
            public static bool IsInstalled()
            {
                return IsInstalled("PDFTron PDFNet");
            }

            public enum Mode
            {
                /// <summary>
                /// By default one or more print paths will be used to create a PDF file.
                /// </summary>
                e_auto,
			    /// <summary>
			    /// For Office file conversions, force COM Interop to be used, regardless if this virtual printer is installed or not.
			    /// </summary>
			    e_interop_only,
			    /// <summary>
			    /// For Office file conversions, do not check for COM Interop availability, and use the printer path instead.
			    /// </summary>
			    e_printer_only,
			    /// <summary>
     		    /// For Office file conversions, use the built in converter if it is available for the converted file type.
    		    /// </summary>
    		    e_prefer_builtin_converter
            };
            /// <summary>Control how the PDFNet converts files, in particular Office documents.
            /// </summary>
            /// <param name="mode">set the print mode. Default is e_auto.
            /// </param>
            public static void SetMode(Mode mode)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertPrinterSetMode(mode));
            }
            /// <summary>Get the current mode for print jobs.
            /// </summary>
            /// <returns>the current print mode
            /// </returns>
            public static Mode GetMode()
            {
                Mode result = Mode.e_auto;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ConvertPrinterGetMode(ref result));
                return result;
            }
        }
    }
}
