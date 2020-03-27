using System;
using System.Collections;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Exception = System.IntPtr;
using TRN_Redaction = System.IntPtr;
using TRN_RedactionAppearance = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary>
    /// PDF Redactor is a separately licensable Add-on that offers options to remove 
    /// (not just covering or obscuring) content within a region of PDF. 
    /// With printed pages, redaction involves blacking-out or cutting-out areas of 
    /// the printed page. With electronic documents that use formats such as PDF, 
    /// redaction typically involves removing sensitive content within documents for 
    /// safe distribution to courts, patent and government institutions, the media, 
    /// customers, vendors or any other audience with restricted access to the content. 
    /// 
    /// The redaction process in PDFNet consists of two steps:
    /// 
    ///  a) Content identification: A user applies redact annotations that specify the 
    /// pieces or regions of content that should be removed. The content for redaction 
    /// can be identified either interactively (e.g. using 'pdftron.PDF.PDFViewCtrl' 
    /// as shown in PDFView sample) or programmatically (e.g. using 'pdftron.PDF.TextSearch'
    /// or 'pdftron.PDF.TextExtractor'). Up until the next step is performed, the user 
    /// can see, move and redefine these annotations.
    ///  b) Content removal: Using 'pdftron.PDF.Redactor.Redact()' the user instructs 
    /// PDFNet to apply the redact regions, after which the content in the area specified 
    /// by the redact annotations is removed. The redaction function includes number of 
    /// options to control the style of the redaction overlay (including color, text, 
    /// font, border, transparency, etc.).
    /// 
    /// PDFTron Redactor makes sure that if a portion of an image, text, or vector graphics 
    /// is contained in a redaction region, that portion of the image or path data is 
    /// destroyed and is not simply hidden with clipping or image masks. PDFNet API can also 
    /// be used to review and remove metadata and other content that can exist in a PDF 
    /// document, including XML Forms Architecture (XFA) content and Extensible Metadata 
    /// Platform (XMP) content.
    /// </summary>
    public class Redactor
    {

        /// <summary>
        /// Apply the redactions specified in red_array to the PDFDoc doc.
        /// </summary>
        /// <param name="doc">the document to redact.</param>
        /// <param name="red_arr">an array of redaction regions.</param>
        /// <param name="app">optional parameter used to customize the appearance of the redaction overlay.</param>
        /// <param name="ext_neg_mode">if true, negative redactions expand beyond the page to remove
        /// content from other pages in the document. if false, the redaction will be localized
        /// to the given page.</param>
        /// <param name="page_coord_sys">if true, redaction coordinates are relative to the lower-left corner of the page,
        /// otherwise the redaction coordinates are defined in PDF user coordinate system (which may or may not coincide with 
        /// page coordinates).</param>
        public static void Redact(PDFDoc doc, ArrayList red_arr, Appearance app, bool ext_neg_mode = true, bool page_coord_sys = true)
        {
            TRN_RedactionAppearance trn_app = IntPtr.Zero;
            string fontName = "Arial";
#if (__ANDROID__)
            fontName = "Roboto";
#elif (__IOS__)
            fontName = "Helvetica";
#endif
            Font fnt = app.TextFont;
            if (app.TextFont == null)
            {
                fnt = Font.Create(doc, fontName, "");
            }
			PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAppearanceCreate(app.RedactionOverlay,
                app.PositiveOverlayColor.mp_colorpt, app.NegativeOverlayColor.mp_colorpt, app.Border, app.UseOverlayText,
                fnt.mp_font, app.MinFontSize, app.MaxFontSize, app.TextColor.mp_colorpt,
                app.HorizTextAlignment, app.VertTextAlignment, app.ShowRedactedContentRegions,
                app.RedactedContentColor.mp_colorpt, ref trn_app));

            IntPtr[] red_arr_ptr = new IntPtr[red_arr.Count];
            int index = 0;
            foreach (var red in red_arr){
                Redaction redaction = (Redaction)red;
                red_arr_ptr[index] = redaction.mp_imp;
                index++;
            }

            int size = Marshal.SizeOf(red_arr_ptr[0]) * red_arr_ptr.Length;
            IntPtr pnt = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(red_arr_ptr, 0, pnt, red_arr_ptr.Length);
				TRN_Exception ret = PDFNetPINVOKE.TRN_RedactorRedact(doc.mp_doc, pnt, red_arr_ptr.Length, trn_app, ext_neg_mode, page_coord_sys);

				PDFNetException.REX(PDFNetPINVOKE.TRN_RedactionAppearanceDestroy(trn_app));
                if (ret != IntPtr.Zero)
                {
                    throw new PDFNetException(ret);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pnt);
            } 
        }

        public class Redaction : IDisposable
        {
            internal TRN_Redaction mp_imp;

            internal Redaction(TRN_Redaction impl)
            {
                this.mp_imp = impl;
            }

            /// <summary> 
            /// </summary>
            /// <param name="page_num">a page number on which to perform the redaction.
            /// </param>		
            /// <param name="bbox">the bounding box for the redaction in PDF page coordinate system.
            /// </param>		
            /// <param name="negative">if true, remove the content outside of the redaction area, 
            /// otherwise remove the content inside the redaction area.
            /// </param>		
            /// <param name="text">optional anchor text to be placed in the redaction region.
            /// </param>	
            public Redaction(int page_num, Rect bbox, bool negative, string text)
            {
                UString uText = new UString(text);
                IntPtr box_pnt = Marshal.AllocHGlobal(Marshal.SizeOf(bbox.mp_imp));
                try
                {
                    // Copy the struct to unmanaged memory.
                    Marshal.StructureToPtr(bbox.mp_imp, box_pnt, false);
                    PDFNetException.REX(PDFNetPINVOKE.TRN_Redactor_RedactionCreate(page_num, box_pnt, negative, uText.mp_impl, ref mp_imp));
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(box_pnt);
                }
            }

            public Redaction(Redaction other)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_Redactor_RedactionCopy(other.mp_imp, ref mp_imp));
            }

		    ~Redaction()
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
                if (mp_imp != IntPtr.Zero)
                {
                    PDFNetException.REX(PDFNetPINVOKE.TRN_Redactor_RedactionDestroy(mp_imp));
                    mp_imp = IntPtr.Zero;
                }
            }
        }

        /// <summary> 
        /// Class used to customize the appearance of the optional redaction overlay.
        /// </summary>
        public class Appearance
        {
            public Appearance()
            {
                // Defaults
                RedactionOverlay = true;
                PositiveOverlayColor = new ColorPt(1, 1, 1);
                NegativeOverlayColor = new ColorPt(1, 1, 1);
                UseOverlayText = true;
                MinFontSize = 2;
                MaxFontSize = 24;
                TextColor = new ColorPt(0, 0, 0);
                HorizTextAlignment = -1; // left justified
                VertTextAlignment = 1; // top justified
                Border = true;
                ShowRedactedContentRegions = false;
                RedactedContentColor = new ColorPt(0.3, 0.3, 0.3);  // Gray
            }

            /// <summary>
            /// If RedactionOverlay is set to true, Redactor will draw an overlay
            /// covering all redacted regions. The rest of properties in the 
            /// Appearance class defines visual properties of the overlay. 
            /// if false the overlay region will not be drawn.
            /// </summary>
            public bool RedactionOverlay { get; set; }

            /// <summary>
            /// PositiveOverlayColor defines the overlay background color in RGB color space for positive redactions.
            /// </summary>
            public ColorPt PositiveOverlayColor { get; set; }

            /// <summary>
            /// NegativeOverlayColor defines the overlay background color in RGB color space for negative redactions.
            /// </summary>
            public ColorPt NegativeOverlayColor { get; set; }

            /// <summary>
            /// Border specifies if the overlay will be surrounded by a border.
            /// </summary>
            public bool Border { get; set; }

            /// <summary>
            /// Specifies if the text (e.g. "Redacted" etc.) should be placed on 
            /// top of the overlay. The remaining properties relate to the positioning, 
            /// and styling of the overlay text.
            /// </summary>
            public bool UseOverlayText { get; set; }

            /// <summary>
            /// Specifies the font used to represent the text in the overlay.
            /// </summary>
            public Font TextFont { get; set; }

            /// <summary>
            /// Specifies the minimum font size used to represent the text in the overlay.
            /// </summary>
            public double MinFontSize { get; set; }

            /// <summary>
            /// Specifies the maximum font size used to represent the text in the overlay.
            /// </summary>
            public double MaxFontSize { get; set; }

            /// <summary>
            /// Specifies the color used to paint the text in the overlay (in RGB).
            /// </summary>
            public ColorPt TextColor;

            /// <summary>
            /// Specifies the text alignment in the overlay:
            ///   align&lt;0  -> text will be left aligned.
            ///   align==0 -> text will be center aligned.
            ///   align&gt;0  -> text will be right aligned.
            /// </summary>
            public int HorizTextAlignment;

            /// <summary>
            /// Specifies the vertical text alignment in the overlay:
            ///   align&lt;0  -> text will be top aligned.
            ///   align==0 -> text will be center aligned.
            ///   align&gt;0  -> text will be bottom aligned.
            /// </summary>
            public int VertTextAlignment;

            /// <summary>
            /// Specifies whether an overlay should be drawn in place of the redacted content.
            /// This option can be used to indicate the areas where the content was removed from
            /// without revealing the content itself. 		
            /// </summary>
            /// <remarks>Default value is False.</remarks>		
            /// <remarks>The overlay region used RedactedContentColor as a fill color.</remarks>
            public bool ShowRedactedContentRegions;

            /// <summary>
            /// Specifies the color used to paint the regions where content was removed.
            /// </summary>
            /// <remarks>Default value is Gray color.</remarks>	
            public ColorPt RedactedContentColor;
        }
    }
}