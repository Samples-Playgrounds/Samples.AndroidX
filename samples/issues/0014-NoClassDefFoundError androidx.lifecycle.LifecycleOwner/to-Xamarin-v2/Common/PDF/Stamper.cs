using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Stamper = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary>
    /// Stamper is a utility class that can be used to stamp PDF pages with text, images, 
    /// or vector art (including another PDF page) in only a few lines of code.
    /// 
    /// Although Stamper is very simple to use compared to ElementBuilder/ElementWriter 
    /// it is not as powerful or flexible. In case you need full control over PDF creation
    /// use ElementBuilder/ElementWriter to add new content to existing PDF pages as 
    /// shown in the ElementBuilder sample project.
    /// </summary>
    public class Stamper : IDisposable
    {
        internal TRN_Stamper mp_impl = IntPtr.Zero;

        internal Stamper(TRN_Stamper impl) 
		{
            this.mp_impl = impl;
		}

        ~Stamper()
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
                PDFNetException.REX(PDFNetPINVOKE.TRN_StamperDestroy(mp_impl));
                mp_impl = IntPtr.Zero;
            }
        }

        /// <summary> Stamper constructor.
        /// 
        /// </summary>
        /// <param name="size_type">Specifies how the stamp will be sized	
        /// </param>
        /// <param name="a">the a
        /// </param>
        /// <param name="b">the b
        /// </param>
        public Stamper(SizeType size_type, double a, double b)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperCreate(size_type, a, b, ref mp_impl));
        }

        /// <summary> Stamps an image to the given destination document at the set of page numbers.
        /// 
        /// </summary>
        /// <param name="dest_doc">The document being stamped
        /// </param>
        /// <param name="src_img">The image that is being stamped to the document
        /// </param>
        /// <param name="dest_pages">The set of pages in the document being stamped
        /// </param>
        public void StampImage(PDFDoc dest_doc, Image src_img, PageSet dest_pages)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperStampImage(mp_impl, dest_doc.mp_doc, src_img.mp_image, dest_pages.mp_imp));
        }

        /// <summary> Stamps a PDF page to the given destination document at the set of page numbers.
        /// 
        /// </summary>
        /// <param name="dest_doc">The document being stamped
        /// </param>
        /// <param name="src_page">The page that is being stamped to the document
        /// </param>
        /// <param name="dest_pages">The set of pages in the document being stamped
        /// </param>
        public void StampPage(PDFDoc dest_doc, Page src_page, PageSet dest_pages)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperStampPage(mp_impl, dest_doc.mp_doc, src_page.mp_page, dest_pages.mp_imp));
        }

        /// <summary> Stamps text to the given destination document at the set of page numbers.
        /// 
        /// </summary>
        /// <param name="dest_doc">The document being stamped
        /// </param>
        /// <param name="src_txt">The image that is being stamped to the document
        /// </param>
        /// <param name="dest_pages">The set of pages in the document being stamped
        /// </param>
        public void StampText(PDFDoc dest_doc, string src_txt, PageSet dest_pages)
        {
            UString u_src_text = new UString(src_txt);
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperStampText(mp_impl, dest_doc.mp_doc, u_src_text.mp_impl, dest_pages.mp_imp));
        }

        /// <summary> 
        /// Defines the font of the stamp. (This only applies to text-based stamps)
        /// 
        /// </summary>
        /// <param name="font">The font of the text stamp
        /// </param>
        public void SetFont(Font font)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperSetFont(mp_impl, font.mp_font));
        }

        /// <summary> Sets the font color (This only effects text-based stamps).
        /// 
        /// </summary>
        /// <param name="color">the new font color
        /// </param>
        public void SetFontColor(ColorPt color)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperSetFontColor(mp_impl, color.mp_colorpt));
        }

        /// <summary> Sets the opacity value for the stamp.
        /// 
        /// </summary>
        /// <param name="opacity">The opacity value of the stamp
        /// </param>
        public void SetOpacity(double opacity)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperSetOpacity(mp_impl, opacity));
        }

        /// <summary> Rotates the stamp by the given number of degrees.
        /// 
        /// </summary>
        /// <param name="rotation">Rotation in degrees
        /// </param>
        public void SetRotation(double rotation)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperSetRotation(mp_impl, rotation));
        }

        /// <summary> Specifies if the stamp is to be stamped in the background or the foreground.
        /// 
        /// </summary>
        /// <param name="background">A flag specifying if the stamp should be added 
        /// as a background layer to the destination page
        /// </param>
        public void SetAsBackground(bool background)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperSetAsBackground(mp_impl, background));
        }

        /// <summary> Specifies if the stamp is to be stamped as annotation or not.
        /// 
        /// </summary>
        /// <param name="annotation">A flag specifying if the stamp should be added 
        /// as an annotation to the destination page
        /// </param>
        /// <remarks> SetAsBackground, HasStamps and DeleteStamps methods will not
        /// work on stamps created with this as true. </remarks>
        public void SetAsAnnotation(bool annotation)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperSetAsAnnotation(mp_impl, annotation));
        }

        /// <summary> Shows on screen.
        /// 
        /// </summary>
        /// <param name="on_screen">Specifies if the watermark will be displayed on screen
        /// </param>
        public void ShowsOnScreen(bool on_screen)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperShowsOnScreen(mp_impl, on_screen));
        }

        /// <summary> Shows on print.
        /// 
        /// </summary>
        /// <param name="on_print">Specifies if the watermark will be displayed when printed
        /// </param>
        public void ShowsOnPrint(bool on_print)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperShowsOnPrint(mp_impl, on_print));
        }

        /// <summary> Sets the horizontal and vertical position of the stamp.
        /// 
        /// </summary>
        /// <param name="horizontal_distance">Horizontal distance from left, right or center of crop box
        /// </param>
        /// <param name="vertical_distance">the vertical_distance
        /// </param>
        /// <param name="percentage">If true, horizontal_distance is a percentage of the crop
        /// box width (e.g.: 0.5 is 50% of the width of the crop box) and vertical_distance
        /// is a percentage of the crop box height. If false, horizontal_distance and
        /// vertical_distance is measured in points.
        /// </param>
        /// <seealso cref="SetAlignment">
        /// </seealso>
        public void SetPosition(double horizontal_distance, double vertical_distance, bool percentage=false)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperSetPosition(mp_impl, horizontal_distance, vertical_distance, percentage));
        }

        /// <summary> Sets the alignment for the x and y variables.
        /// 
        /// </summary>
        /// <param name="horizontal_alignment">the horizontal alignment
        /// </param>
        /// <param name="vertical_alignment">the vertical alignment
        /// </param>
        /// <seealso cref="SetPosition(double,double,bool)">
        /// </seealso>
        public void SetAlignment(HorizontalAlignment horizontal_alignment, VerticalAlignment vertical_alignment)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperSetAlignment(mp_impl, horizontal_alignment, vertical_alignment));
        }

        /// <summary> Sets the text alignment (note: this only applies to text watermarks).
        /// 
        /// </summary>
        /// <param name="text_alignment">Enumerator for text alignment (e_left, e_center, e_right)
        /// </param>
        public void SetTextAlignment(TextAlignment text_alignment)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperSetTextAlignment(mp_impl, text_alignment));
        }

        /// <summary> Sets the size of the stamp.
        /// 
        /// </summary>
        /// <param name="size_type">Specifies how the stamp will be sized		
        /// </param>
        /// <param name="a">the a
        /// </param>
        /// <param name="b">the b
        /// </param>
        public void SetSize(SizeType size_type, double a, double b)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperSetSize(mp_impl, size_type, a, b));
        }

        /// <summary> Deletes PDFTron stamps from document at given page numbers.
        /// 
        /// </summary>
        /// <param name="doc">The document to delete stamps from
        /// </param>
        /// <param name="page_set">The set of pages to delete stamps from
        /// </param>
        public static void DeleteStamps(PDFDoc doc, PageSet page_set)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperDeleteStamps(doc.mp_doc, page_set.mp_imp));
        }

        /// <summary> Returns true if the given set of pages has at least one stamp.
        /// 
        /// </summary>
        /// <param name="doc">The document that's being checked
        /// </param>
        /// <param name="page_set">The set of page that's being checked
        /// </param>
        /// <returns> true, if successful
        /// </returns>
        public static bool HasStamps(PDFDoc doc, PageSet page_set)
        {
            bool res = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_StamperHasStamps(doc.mp_doc, page_set.mp_imp, ref res));
            return res;
        }

        ///<summary>Size Types</summary>
        public enum SizeType
        {
            ///<summary>Stamp size is relative to the size of the crop box of
            /// the destination page.</summary>
            e_relative_scale = 1,
            ///<summary>Stamp size is explicitly set.The width and height are constant, regardless of the size of the
            /// destination page's bounding box.</summary>
            e_absolute_size = 2,
            ///<summary>This type only applies to text stamps.</summary>
            e_font_size = 3
        }

        public enum HorizontalAlignment
        {
            e_horizontal_left = -1,
            e_horizontal_center = 0,
            e_horizontal_right = 1
        }

        public enum VerticalAlignment
        {
            e_vertical_bottom = -1,
            e_vertical_center = 0,
            e_vertical_top = 1
        }

        public enum TextAlignment
        {
            e_align_left = -1,
            e_align_center = 0,
            e_align_right = 1
        }
    }
}