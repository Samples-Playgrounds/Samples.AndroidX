using Android.Content;
using System;

namespace pdftron.PDF
{
    /// <summary>
    /// Print is a utility class that can be used to print PDF documents. It is only supported on Android 4.4 and up (API Level 19+).
    /// </summary>
    public class Print : pdftronprivate.PDF.Print
    {
        public enum PrintContent
        {
            DocumentBit = PrintContentDocumentBit,
            AnnotationBit = PrintContentAnnotationBit,
            SummaryBit = PrintContentSummaryBit
        };

        /// <summary>
        /// Print the document using the current available printers or print services
        /// installed on the device.
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="jobName">job name which will be displayed in the print queue</param>
        /// <param name="doc">the document to be printed</param>
        /// <param name="printContent">the content to be printed. Valid values are
        /// 1: document only
        /// 3: document and annotations
        /// 4: summary of annotations only
        /// 7: document, annotations and summary of annotations
        /// </param>
        /// <param name="isRtl">if true, the summary of annotations will be right aligned; otherwise, left aligned</param>        
        /// Note: only available for Android 4.4 (API 19) or above.
        /// <ul>
        /// <li>It tries to acquire a read lock on the document and the print will
        /// fail in case it is not possible to acquire the lock.</li>
        /// <li>For encrypted documents, blank content is sent to the printer. Please
        /// see {@link #startPrintJob(android.content.Context, String, PDFDoc, String, Integer, Boolean)}.</li>
        /// </ul>
        public static void StartPrintJob(Context context, String jobName, PDFDoc doc, int printContent, bool isRtl)
        {
            Java.Lang.Integer printAnnot = new Java.Lang.Integer(printContent);
            Java.Lang.Boolean rtl = new Java.Lang.Boolean(isRtl);
            pdftronprivate.PDF.Print.StartPrintJob(context, jobName, TypeConvertHelper.ConvPDFDocToNative(doc), printAnnot, rtl);
        }
        /// <summary>
        /// Print the document using the current available printers or print services installed on the
        /// device and the supplied password.
        /// <p>
        /// This method will try to initialize the document's SecurityHandler using the supplied
        /// password (it assumes the document uses standard security).
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="jobName">job name which will be displayed in the print queue</param>
        /// <param name="doc">the document to be printed</param>
        /// <param name="printContent">the content to be printed. Valid values are
        /// 1: document only
        /// 3: document and annotations
        /// 4: summary of annotations only
        /// 7: document, annotations and summary of annotations
        /// </param>
        /// <param name="isRtl">if true, the summary of annotations will be right aligned; otherwise, left aligned</param> 
        public static void StartPrintJob(Context context, String jobName, PDFDoc doc, String password, int printContent, bool isRtl)
        {
            Java.Lang.Integer printAnnot = new Java.Lang.Integer(printContent);
            Java.Lang.Boolean rtl = new Java.Lang.Boolean(isRtl);
			pdftronprivate.PDF.Print.StartPrintJob(context, jobName, TypeConvertHelper.ConvPDFDocToNative(doc), password, printAnnot, rtl);
        }

        /// <summary>
        /// Export comments and summary of annotations
        /// </summary>
        /// <param name="inDoc">the document to be printed</param>
        /// <param name="isRtl">if true, the summary of annotations will be right aligned; otherwise, left aligned</param>
        /// <returns>the output document containing comments and summary of annotations</returns>
        public static PDFDoc ExportAnnotations(PDFDoc inDoc, bool isRtl)
        {
            pdftronprivate.PDF.PDFDoc outDoc = pdftronprivate.PDF.Print.ExportAnnotations(TypeConvertHelper.ConvPDFDocToNative(inDoc), isRtl);
            return TypeConvertHelper.ConvPdfDocToManaged(outDoc);
        }
    }
}
