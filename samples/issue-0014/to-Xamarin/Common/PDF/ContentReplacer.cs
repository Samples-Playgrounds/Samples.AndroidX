using System;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_ContentReplacer = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary>
    /// ContentReplacer is a utility class for replacing content (text and images)
    /// in existing PDF (template) documents.
    ///
    /// Users can replace content in a PDF page using the following operations:
    /// - Replace an image that exists in a target rectangle with a replacement image. 
    /// - Replace text that exists in a target rectangle with replacement text. 
    /// - Replace all instances of a specially marked string with replacement string.
    ///
    /// <example>
    /// The following code replaces an image in a target region. It also replaces
    /// the text "[NAME]" and "[JOB_TITLE]" with "John Smith"
    /// and "Software Developer" respectively. Notice the square braces ('[' and ']') on
    /// the target strings in the original PDFDoc. These square braces are not included in
    /// the actual function calls below, as they're implicitly added.
    ///
    /// <code>
    /// PDFDoc doc("../../TestFiles/BusinessCardTemplate.pdf");
    /// doc.InitSecurityHandler();
    /// ContentReplacer replacer;
    /// Page pg = doc.GetPage(1);
    /// Image img = Image.Create(doc, "../../TestFiles/peppers.jpg");
    /// replacer.AddImage(page.GetMediaBox(), img.GetSDFObj());
    /// replacer.AddString("NAME", "John Smith");
    /// replacer.AddString("JOB_TITLE", "Software Developer");
    /// replacer.Process(page);
    /// </code>
    /// </example>
    /// </summary>
    public class ContentReplacer : IDisposable
    {
        internal TRN_ContentReplacer mp_impl = IntPtr.Zero;

        internal ContentReplacer(TRN_ContentReplacer impl) 
		{
            this.mp_impl = impl;
		}
        /// <summary>
	    /// Create a new ContentReplacer object, to which replacement rules will be added.
        /// The same object can be used to 'Process' multiple pages.
        /// </summary>
        public ContentReplacer()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentReplacerCreate(ref mp_impl));
        }
	
	    /// <summary>
	    /// Replace the image that best fits 'target_region' with 'replacement_image'.
	    /// </summary>
	    /// <param name="target_region"> The rectangle defining the area in which an image
	    /// that best fits the rectangle will be replaced by 'replacement_image'.
	    /// </param>
	    /// <param name="replacement_image"> The 'SDF.Obj' of a 'PDF.Image' object.
	    /// </param>
	    /// <remarks> The best fit is the image that closest matches 'target_region'. For example if
	    /// there are two images on the page, one taking up the entire page, and the other 
	    /// smaller, and the smaller one has similar dimensions and position of 
	    /// 'target_region', then the smaller image would be replaced, not the larger.
	    /// Furthermore, if 'target_region' encloses multiple images, then only the image
        /// with the largest area will be replaced.
        /// </remarks>
        public void AddImage(Rect target_region, Obj replacement_image)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentReplacer_AddImage(mp_impl, ref target_region.mp_imp, replacement_image.mp_obj));
        }
	
	    /// <summary>
	    /// All text inside 'target_region' will be deleted and replaced with 'replacement_text'. 
	    /// </summary>
	    /// <param name="target_region"> The rectangle defining the area in which all text will
	    /// be replaced by 'replacement_text'.
	    /// </param>
	    /// <param name="replacement_text"> The new text that will replace the existing text
	    /// in 'target_region'.
	    /// </param>
	    /// <remarks> The 'replacement_text' will be styled in the same font/color/style that is used 
	    /// by the original text. If there are multiple font styles, the most prevalent style will
	    /// be used. Also, the 'replacement_text' will wrap within the 'target_region', 
	    /// but if it is too long, the overflow text will not be visible, and no surrounding
	    /// content will be affected.
	    /// </remarks>
        public void AddText(Rect target_region, String replacement_text)
        {
            UString str = new UString(replacement_text);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentReplacer_AddText(mp_impl, ref target_region.mp_imp, str.mp_impl));
        }
	
	    /// <summary>
	    /// Any text of the form "[template_text]" will be replaced by "replacement_text".
	    /// </summary>
	    /// <param name="template_text"> The text to remove.
	    /// </param>
	    /// <param name="replacement_text"> The new text that will appear in place of 'template_text'.
	    /// </param>
	    /// <remarks> Only text wrapped in '[' and ']' will be checked, and if it matches 'template_text',
	    /// then 'template_text' and the surrounding square braces will be replaced
	    /// by 'replacement_text'. For example AddString("TITLE", "Doctor") will replace any
	    /// text consisting of "[TITLE]" with "Doctor".
	    /// </remarks>
        public void AddString(String template_text, String replacement_text)
        {
            UString str_template = new UString(template_text);
            UString str_replacement = new UString(replacement_text);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentReplacer_AddString(mp_impl, str_template.mp_impl, str_replacement.mp_impl));
        }
	
	    /// <summary>
	    /// Apply the replacement instructions to the target page. Subsequent calls
	    /// to 'Process' can be made on other pages, and it will apply the same rules.
	    /// </summary>
	    /// <param name="page"> The page to apply the content replacement instructions to.
	    /// </param>
        public void Process(Page page)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentReplacer_Process(mp_impl, page.mp_page));
        }
	    /// <summary> Releases all resources used by the ContentReplacer </summary>
        ~ContentReplacer()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (disposing)
            {
                // Dispose managed resources.
            }

            // Clean up native resources
            Destroy();
        }
        public void Destroy()
        {
            if (mp_impl != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_ContentReplacerDestroy(mp_impl));
                mp_impl = IntPtr.Zero;
            }
        }
    }
}