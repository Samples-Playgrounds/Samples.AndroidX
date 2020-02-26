using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Flattener = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary>
    /// Flattener is a optional PDFNet add-on that can be used to simplify and optimize 
    /// existing PDF's to render faster on devices with lower memory and speeds.
    /// 
    /// PDF documents can frequently contain very complex page description (e.g. 
    /// thousands of paths, different shadings, color spaces, blend modes, large images 
    /// etc.) that may not be suitable for interactive viewing on mobile devices.
    /// Flattener can be used to speed-up PDF rendering on mobile devices and on the Web 
    /// by simplifying page content (e.g. flattening complex graphics into images) while 
    /// maintaining vector text whenever possible.
    /// 
    /// By using the FlattenMode::e_simple option each page in the PDF will be
    /// reduced to a single background image, with the remaining text over top in vector
    /// format. Some text may still get flattened, in particular any text that is clipped, 
    /// or underneath, other content that will be flattened.
    /// 
    /// On the other hand the FlattenMode::e_fast will not flatten simple content, such
    /// as simple straight lines, nor will it flatten Type3 fonts.
    /// 
    /// @note 'Flattener' is available as a separately licensable add-on to PDFNet 
    /// core license or for use via Cloud API (http://www.pdftron.com/pdfnet/cloud).
    /// 
    /// @note See 'pdftron.PDF.Optimizer' for alternate approach to optimize PDFs with 
    /// focus on file size reduction.
    /// </summary>
    public class Flattener : IDisposable
    {
        internal TRN_Flattener mp_impl = IntPtr.Zero;

        internal Flattener(TRN_Flattener impl)
        {
            this.mp_impl = impl;
        }

        ~Flattener()
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
                PDFNetException.REX(PDFNetPINVOKE.TRN_FlattenerDestroy(mp_impl));
                mp_impl = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Flattener constructor
        /// </summary>
        public Flattener()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FlattenerCreate(ref mp_impl));
        }

        /// <summary>
        /// The output resolution, from 1 to 1000, in Dots Per Inch (DPI) at which to 
        /// render elements which cannot be directly converted. 
        /// the default value is 96 Dots Per Inch
        /// </summary>
        /// <param name="dpi"> the resolution in Dots Per Inch
        /// </param>
        public void SetDPI(UInt32 dpi)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FlattenerSetDPI(mp_impl, dpi));
        }

        /// <summary>
        /// Specifies the maximum image size in pixels.
        /// </summary>
        /// <param name="max_pixels"> the maximum number of pixels an image can have.
        /// </param>
        public void SetMaximumImagePixels(UInt32 max_pixels)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FlattenerSetMaximumImagePixels(mp_impl, max_pixels));
        }

        /// <summary>
        /// Specifies whether to leave images in existing compression, or as JPEG.
        /// </summary>
        /// <param name="jpg"> if true PDF will contain all JPEG images.
        /// </param>
        public void SetPreferJPG(bool jpg)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FlattenerSetPreferJPG(mp_impl, jpg));
        }

        /// <summary>
        /// Specifies the compression quality to use when generating JPEG images.
        /// </summary>
        /// <param name="quality"> the JPEG compression quality, from 0(highest compression) to 100(best quality).
        /// </param>
        public void SetJPGQuality(UInt32 quality)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FlattenerSetJPGQuality(mp_impl, quality));
        }

        /// <summary>
        /// Used to control how precise or relaxed text flattening is. When some text is 
        /// preserved (not flattened to image) the visual appearance of the document may be altered.
        /// </summary>
        /// <param name="threshold"> the threshold setting to use.
        /// </param>
        public void SetThreshold(Threshold threshold)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FlattenerSetThreshold(mp_impl, threshold));
        }

        /// <summary>
        /// Enable or disable path hinting.
        /// </summary>
        /// <param name="path_hinting"> if true path hinting is enabled. Path hinting is used to slightly
        /// adjust paths in order to avoid or alleviate artifacts of hair line cracks between
        /// certain graphical elements. This option is turned on by default. 
        /// </param>
        public void SetPathHinting(bool path_hinting)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FlattenerSetPathHinting(mp_impl, path_hinting));
        }

        /// <summary>
        /// Process the given page, flattening content that matches the mode criteria.
        /// </summary>
        /// <param name="page"> the page to flatten.
        /// </param>
        /// <param name="mode"> indicates the criteria for which elements are flattened.
        /// </param>
        public void Process(Page page, FlattenMode mode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FlattenerProcessPage(mp_impl, page.mp_page, mode));
        }

        /// <summary>
        /// Process each page in the PDF, flattening content that matches the mode criteria.
        /// </summary>
        /// <param name="doc"> the document to flatten.
        /// </param>
        /// <param name="mode"> indicates the criteria for which elements are flattened.
        /// </param>
        public void Process(PDFDoc doc, FlattenMode mode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FlattenerProcess(mp_impl, doc.mp_doc, mode));
        }

        public enum Threshold
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
        }

        public enum FlattenMode
        {
            /// <summary>
            /// Feature reduce PDF to a simple two layer representation consisting
            /// of a single background RGB image and a simple top text layer.
            /// </summary>
            e_simple,
            /// <summary>
            /// Feature reduce PDF while trying to preserve some 
            /// complex PDF features (such as vector figures, transparency, shadings, 
            /// blend modes, Type3 fonts etc.) for pages that are already fast to render. 
            /// This option can also result in smaller & faster files compared to e_simple,
            /// but the pages may have more complex structure.
            /// </summary>
            e_fast
        }
    }
}