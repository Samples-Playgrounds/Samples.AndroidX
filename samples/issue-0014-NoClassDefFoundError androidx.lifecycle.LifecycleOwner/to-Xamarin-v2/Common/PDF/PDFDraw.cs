using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron;
using pdftron.Common;
using pdftron.SDF;

using TRN_PDFDraw = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF
{
    public class BitmapInfo
    {
        // Fields
        private int width, height, stride;
        private double dpi;
        private IntPtr buf;

        // Methods
        public BitmapInfo(int w, int h, int s, double d, IntPtr b)
        {
            width = w;
            height = h;
            stride = s;
            dpi = d;
            buf = b;
        }
        public BitmapInfo(BitmapInfo b)
        {
            width = b.width;
            height = b.height;
            stride = b.stride;
            dpi = b.dpi;
            buf = b.buf;
        }

        // Properties
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        public int Stride
        {
            get { return this.stride; }
            set { this.stride = value; }
        }
        public double Dpi
        {
            get { return this.dpi; }
            set { this.dpi = value; }
        }
        public byte[] Buffer
        {
            get
            {
                byte[] res = new byte[height * stride];
                System.Runtime.InteropServices.Marshal.Copy(this.buf, res, 0, height * stride);
                return res;
            }
        }
        internal IntPtr BufPtr
        {
            get
            {
                return this.buf;
            }
        }
    }

    /// <summary> PDFDraw contains methods for converting PDF pages to images and to Bitmap objects.
    /// Utility methods are provided to export PDF pages to various raster formats as well 
    /// as to convert pages to GDI+ bitmaps for further manipulation or drawing.
    /// 
    /// </summary>
    /// <remarks>  This class is available on all platforms supported by PDFNet.  </remarks>
    public class PDFDraw : IDisposable
    {
        internal TRN_PDFDraw mp_draw = IntPtr.Zero;
        internal bool m_page_transparent = false;

        /// <summary> Releases all resources used by the PDFDraw </summary>
        ~PDFDraw()
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
            if (mp_draw != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawDestroy(mp_draw));
                mp_draw = IntPtr.Zero;
            }
        }

        // Methods
        internal PDFDraw(TRN_PDFDraw imp)
        {
            this.mp_draw = imp;
            this.m_page_transparent = false;
        }
        /// <summary> PDFDraw constructor and destructor.
	    /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PDFDraw()
        {
            double dpi = 92;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawCreate(dpi, ref mp_draw));
        }
        /// <summary> Instantiates a new pDF draw.
        /// 
        /// </summary>
        /// <param name="dpi">the dpi
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PDFDraw(double dpi)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawCreate(dpi, ref mp_draw));
        }

        /// <summary> Sets the core graphics library used for rasterization and
        /// rendering. Using this method it is possible to quickly switch
        /// between different implementations. By default, PDFDraw uses
        /// the built-in, platform independent rasterizer.
        /// 
        /// </summary>
        /// <param name="type">Rasterizer type.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        [Obsolete("Deprecated, will be removed in the next update.")]
        public void SetRasterizerType(PDFRasterizer.Type type)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetRasterizerType(mp_draw, type));
        }

        /// <summary> Sets the output image resolution.
        /// 
        /// DPI stands for Dots Per Inch. This parameter is used to specify the output
        /// image size and quality. A typical screen resolution for monitors these days is
        /// 92 DPI, but printers could use 200 DPI or more.
        /// 
        /// </summary>
        /// <param name="dpi">the new dPI
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks> <para>The size of resulting image is a function of DPI and the dimensions of 
        /// the source PDF page. For example, is DPI is 92 and page is 8 inches wide, the
        /// output bitmap will have 92*8 = 736 pixels per line. If you know the dimensions
        /// of the destination bitmap, but don't care about DPI of the image you can use
        /// pdfdraw.SetImageSize() instead.
        /// </para>
        /// <para>
        /// if you would like to rasterize extremely large bitmaps (e.g. with
        /// resolutions of 2000 DPI or more) it is not practical to use PDFDraw directly
        /// because of the memory required to store the entire image. In this case, you
        /// can use PDFRasterizer directly to generate the rasterized image in stripes or
        /// tiles.
        /// </para>
        /// </remarks>
        public void SetDPI(double dpi)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetDPI(mp_draw, dpi));
        }
        /// <summary> Sets the image size.
        /// 
        /// </summary>
        /// <param name="width">the width
        /// </param>
        /// <param name="height">the height
        /// </param>
        /// <param name="preserve_aspect_ratio">the preserve_aspect_ratio
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetImageSize(int width, int height, bool preserve_aspect_ratio)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetImageSize(mp_draw, width, height, preserve_aspect_ratio));
        }
        /// <summary> SetImageSize can be used instead of SetDPI() to adjust page  scaling so that
        /// image fits into a buffer of given dimensions.
        /// 
        /// If this function is used, DPI will be calculated dynamically for each
        /// page so that every page fits into the buffer of given dimensions.
        /// 
        /// </summary>
        /// <param name="width">- The width of the image, in pixels/samples.
        /// </param>
        /// <param name="height">- The height of the image, in pixels/samples.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetImageSize(int width, int height)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetImageSize(mp_draw, width, height, true));
        }
        /// <summary> Selects the page box/region to rasterize.
        /// 
        /// </summary>
        /// <param name="region">Page box to rasterize. By default, PDFDraw will rasterize
        /// page crop box.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPageBox(Page.Box region)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetPageBox(mp_draw, region));
        }
        /// <summary> Clip the render region to the provided rect (in page space)
        /// 
        /// </summary>
        /// <param name="clip_rect">Clipping rect. By default, PDFDraw will rasterize 
        /// the entire page box.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetClipRect(Rect clip_rect)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetClipRect(mp_draw, ref clip_rect.mp_imp));
        }
        /// <summary> Flips the vertical (i.e. Y) axis of the image.
        /// 
        /// </summary>
        /// <param name="flip_y">true to flip the Y axis, false otherwise. For compatibility with
        /// most raster formats 'flip_y' is true by default.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetFlipYAxis(bool flip_y)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetFlipYAxis(mp_draw, flip_y));
        }
        /// <summary> Sets the rotation value for this page.
        /// 
        /// </summary>
        /// <param name="r">the new rotate
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  This method is used only for drawing purposes and it does not modify
        /// the document (unlike Page.SetRotate()).</remarks>
        public void SetRotate(Page.Rotate r)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetRotate(mp_draw, r));
        }
        /// <summary>Enable or disable annotation and forms rendering. By default, all annotations and form fields are rendered.
        /// </summary>
        /// <param name="render_annots">True to draw annotations, false otherwise.
        /// </param>
        public void SetDrawAnnotations(bool render_annots)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetDrawAnnotations(mp_draw, render_annots));
        }

        /// <summary>Enable or disable highlighting form fields. Default is disabled.
        /// </summary>
        /// <param name="highlight_fields">true to highlight, false otherwise. 
        /// </param>
        public void SetHighlightFields(bool highlight_fields)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetHighlightFields(mp_draw, highlight_fields));
        }

        /// <summary>Sets the gamma factor used for anti-aliased rendering.
        /// </summary>
        /// <param name="exp">exponent value of gamma function. Typical values are in the range from 0.1 to 3.
        /// </param>
        /// <remarks>Gamma correction can be used to improve the quality of anti-aliased image output and can (to some extent) decrease the appearance common anti-aliasing artifacts (such as pixel width lines between polygons).
        /// Gamma correction is used only in the built-in rasterizer.
        /// </remarks>
        public void SetGamma(double exp)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetGamma(mp_draw, exp));
        }
        /// <summary>Sets the Optional Content Group (OCG) context that should be used when rendering the page.
        /// This function can be used to selectively render optional content (such as PDF layers) based on the states of optional content groups in the given context.
        /// </summary>
        /// <param name="ctx">Optional Content Group (OCG) context, or NULL if the rasterizer should render all content on the page.
        /// </param>
        public void SetOCGContext(OCG.Context ctx)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetOCGContext(mp_draw, ctx.mp_impl));
        }
        /// <summary>Tells the rasterizer to render the page 'print' mode.
        /// Certain page elements (such as annotations or OCG-s) are meant to be visible either on the screen or on the printed paper but not both. A common example, is the "Submit" button on electronic forms.
        /// </summary>
        /// <param name="is_printing">set to true if the page should be rendered in print mode. By default, print mode flag is set to false.
        /// </param>
        public void SetPrintMode(bool is_printing)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetPrintMode(mp_draw, is_printing));
        }
        /// <summary>Sets the page color to transparent. By default, PDFDraw assumes that the page is imposed directly on an opaque white surface. Some applications may need to impose the page on a different backdrop. In this case any pixels that are not covered during rendering will be transparent.
        /// </summary>
        /// <param name="is_transp">If true, page's backdrop color will be transparent. If false, the page's backdrop will be a opaque white.
        /// </param>
        /// <remarks>If true, page's backdrop color will be transparent. If false, the page's backdrop will be a opaque white.
        /// </remarks>
        public void SetPageTransparent(bool is_transp)
        {
            this.m_page_transparent = is_transp;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetPageTransparent(mp_draw, is_transp));
        }
        /// <summary>Sets the default color of the page backdrop.
        /// </summary>
        /// <param name="c">The color (RGB) to set.
        /// </param>
        public void SetDefaultPageColor(byte r, byte g, byte b)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetDefaultPageColor(mp_draw, r, g, b));
        }
        /// <summary>Enable or disable support for overprint. Overprint is a device dependent feature and the results will vary depending on the output color space and supported colorants (i.e. CMYK, CMYK+spot, RGB, etc). By default overprint is enabled for only PDF/X files.
        /// </summary>
        /// <param name="op">e_op_on: always enabled; e_op_off: always disabled; e_op_pdfx_on: enabled for PDF/X files only.
        /// </param>
        public void SetOverprint(PDFRasterizer.OverprintPreviewMode op)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetOverprint(mp_draw, op));
        }
        /// <summary>Enable or disable anti-aliasing. Anti-Aliasing is a technique used to improve the visual quality of images 
        /// when displaying them on low resolution devices (for example, low DPI computer monitors). 
        /// Anti-aliasing is enabled by default.
        /// </summary>
        /// <param name="enable_aa">whether to enable anti-aliasing
        /// </param>
        public void SetAntiAliasing(bool enable_aa)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetAntiAliasing(mp_draw, enable_aa));
        }

        /// <summary>Enable or disable path hinting. Path hinting is used to slightly adjust paths in order to avoid or alleviate artifacts of hair line cracks between
        ///	certain graphical elements. This option is turned on by default.
        /// </summary>
        /// <param name="enable_ph">whether to enable path hinting.
        /// </param>
        public void SetPathHinting(bool enable_ph)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetPathHinting(mp_draw, enable_ph));
        }

        /// <summary>Set thin line adjustment parameters.
        /// </summary>
        /// <param name="pixel_grid_fit">if true (horizontal/vertical) thin lines will be snapped to 
        /// integer pixel positions. This helps make thin lines look sharper and clearer. This
        /// option is turned off by default and it only works if path hinting is enabled.
        /// </param>
        /// <param name="stroke_adjust">if true auto stroke adjustment is enabled. Currently, this would 
        /// make lines with sub-pixel width to be one-pixel wide. This option is turned on by default.
        /// </param>
        public void SetThinLineAdjustment(bool pixel_grid_fit, bool stroke_adjust)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetThinLineAdjustment(mp_draw, pixel_grid_fit, stroke_adjust));
        }
        /// <summary> Enable or disable image smoothing.			
        /// The rasterizer allows a tradeoff between rendering quality and rendering speed.
        /// This function can be used to indicate the preference between rendering speed and quality. image smoothing is enabled by default.
        /// </summary>
        /// <param name="smoothing_enabled">whether to enable image smoothing
        /// </param>
        /// <param name="hq_image_resampling">whether to use a higher quality (but slower) smoothing algorithm
        /// </param>
        /// <exception cref="PDFNetException">PDFNetException the PDFNet exception </exception>
        /// <remarks>image smoothing option has effect only if the source image has higher resolution 
        /// that the output resolution of the image on the rasterized page. PDFNet automatically
        /// controls at what resolution/zoom factor, 'image smoothing' needs to take effect.</remarks>
        public void SetImageSmoothing(bool smoothing_enabled, bool hq_image_resampling)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetImageSmoothing(mp_draw, smoothing_enabled, hq_image_resampling));
        }
        /// <summary> Sets the caching.
        /// 
        /// </summary>
        /// <param name="enabled">the new caching
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetCaching(bool enabled)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetCaching(mp_draw, enabled));
        }
        /// <summary>
        /// Draws the page into a given memory buffer.
        /// Set the color post processing transformation.
        /// This transform is applied to the rasterized bitmap as the final step
        /// in the rasterization process, and is applied directly to the resulting
        /// bitmap (disregarding any color space information). Color post
        /// processing only supported for RGBA output.
        /// </summary>
        /// <param name="mode">mode is the specific transform to be applied.</param>
        void SetColorPostProcessMode(pdftron.PDF.PDFRasterizer.ColorPostProcessMode mode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetColorPostProcessMode(mp_draw, mode));
        }
        /// <summary> Export.
        /// 
        /// </summary>
        /// <param name="page">the page
        /// </param>
        /// <param name="filename">the filename
        /// </param>
        /// <param name="format">the format
        /// </param>
        /// <param name="encoder_hints">the encoder_params
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Export(Page page, string filename, string format, Obj encoder_hints)
        {
            UString str = new UString(filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawExport(mp_draw, page.mp_page, str.mp_impl, format, encoder_hints.mp_obj));
        }
        /// <summary> Export.
        /// 
        /// </summary>
        /// <param name="page">the page
        /// </param>
        /// <param name="filename">the filename
        /// </param>
        /// <param name="format">the format
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Export(Page page, string filename, string format)
        {
            UString str = new UString(filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawExport(mp_draw, page.mp_page, str.mp_impl, format, IntPtr.Zero));
        }
        /// <summary> A utility method to export the given PDF page to an image file.
        /// 
        /// </summary>
        /// <param name="page">The source PDF page.
        /// </param>
        /// <param name="filename">- The name of the output image file. The filename should include
        /// the extension suffix (e.g. 'c:/output/myimage.png').
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>By default, the function exports to PNG.. The file format of the output image. Currently supported formats are:
        /// <list type="bullet">
        /// <item><term>"RAW"</term><description> RAW format. There are four possibilities: 
        ///							 e_rgba - if transparent and color page;
        ///							 e_gray_alpha - if transparent and gray page;
        ///				    		 e_rgb - if opaque and color page;
        ///							 e_gray - if opaque and gray page.</description></item>
        /// <item><term>"BMP"</term><description> Bitmap image format (BMP)</description></item>
        /// <item><term>"JPEG"</term><description> Joint Photographic Experts Group (JPEG) image format</description></item>
        /// <item><term>"PNG"</term><description> 24-bit W3C Portable Network Graphics (PNG) image format</description></item>
        /// <item><term>"PNG8"</term><description> 8-bit, palettized PNG format. The exported file size should be
        /// smaller than the one generated using "PNG", possibly at the
        /// expense of some image quality.</description></item>
        /// <item><term>"TIFF"</term><description> Tag Image File Format (TIFF) image format.</description></item>
        /// <item><term>"TIFF8"</term><description> Tag Image File Format (TIFF) image format (with 8-bit pallete).</description></item>
        /// </list>
        /// </remarks>
        public void Export(Page page, string filename)
        {
            UString str = new UString(filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawExport(mp_draw, page.mp_page, str.mp_impl, "PNG", IntPtr.Zero));
        }

#if (__DESKTOP__)
        /// <summary>Sets the default color of the page backdrop.
        /// </summary>
        /// <param name="c">The color (RGB) to set.
        /// </param>
        public void SetDefaultPageColor(System.Drawing.Color c)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawSetDefaultPageColor(mp_draw, c.R, c.G, c.B));
		}
        /// <summary> Gets the bitmap. (WPF BitmapSource)</summary>
        /// <param name = "page">the page
        /// </param>
        /// <returns> the bitmap (WPF BitmapSource)
        /// </returns>
        /*public System.Windows.Media.Imaging.BitmapSource GetBitmapSource(Page page)
        {
            //byte[] buf = new byte[0];
            IntPtr src = IntPtr.Zero;
            int width = 0, height = 0, stride = 0;
	        double dpi = 96;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawGetBitmap(mp_draw, page.mp_page, ref width, ref height, ref stride, ref dpi, this.m_page_transparent ? PixelFormat.e_bgra : PixelFormat.e_bgr, false, ref src));
            System.Windows.Media.PixelFormat pixfmt = m_page_transparent ? System.Windows.Media.PixelFormats.Pbgra32 : System.Windows.Media.PixelFormats.Bgr24;
            System.Windows.Media.Imaging.WriteableBitmap bmp = new System.Windows.Media.Imaging.WriteableBitmap(width, height, dpi, dpi, pixfmt, null);
            if (bmp == null) return null;

            System.Windows.Int32Rect rect = new System.Windows.Int32Rect(0, 0, width, height);
            bmp.Lock();

            //IntPtr src_buf = IntPtr.Zero;
            //System.Runtime.InteropServices.Marshal.Copy(buf, 0, src_buf, buf.Length);

            bmp.WritePixels(rect, src, stride * height, stride, 0, 0);
            bmp.Unlock();
            bmp.Freeze();

            return bmp;
        }*/
        /// <summary> Gets the bitmap.</summary>
        /// <param name="buf">An unmanaged buffer of image data
        /// </param>
        /// <param name="width">Width of buf
        /// </param>
        /// <param name="height">Height of buf
        /// </param>
        /// <param name="stride">Stride of buf
        /// </param>
        /// <param name="dpi">Resolution of buf
        /// </param>
        /// <param name="pixfmt">Pixel format of buf
        /// </param>
        /// <returns> the bitmap
        /// </returns>
        public static System.Drawing.Bitmap GetBitmap(IntPtr buf, int width, int height, int stride, double dpi, System.Drawing.Imaging.PixelFormat pixfmt)
		{
			System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(width, height, pixfmt);
			if (bmp == null) return null;

			if (dpi > 0)
				bmp.SetResolution((float)dpi, (float)dpi);

			System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
			System.Drawing.Imaging.BitmapData bd = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, pixfmt);
			if (bd == null) return null;

            IntPtr dest_buf = bd.Scan0;
            int copy_length = Math.Min(stride, bd.Stride);

            byte[] managedArray = new byte[stride*height];
 			Marshal.Copy(buf, managedArray, 0, stride*height);

 			byte[] destination = new byte[bd.Stride*bd.Height];

            int s = 0;
            int d = 0;
            for (int i = 0; i<height; ++i, s += stride, d += bd.Stride) {
              	Buffer.BlockCopy(managedArray, s, destination, d, copy_length);
            }

            Marshal.Copy(destination, 0, dest_buf, bd.Stride*bd.Height);
 
            bmp.UnlockBits(bd);

            return bmp;
		}
        /// <summary> Gets the bitmap.
        /// 
        /// </summary>
        /// <param name="page">the page
        /// </param>
        /// <returns> the bitmap
        /// </returns>
        public System.Drawing.Bitmap GetBitmap(Page page)
        {
            //byte[] buf = new byte[0];
            //int width = 0, height = 0, stride = 0;
            //double dpi = 96;
            //PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawGetBitmapDotNet(mp_draw, page.mp_page, ref width, ref height, ref stride, ref dpi, this.m_page_transparent ? PixelFormat.e_bgra : PixelFormat.e_bgr, false, ref buf));
            //System.Drawing.Bitmap bmp = GetBitmap(buf, width, height, stride, dpi, this.m_page_transparent ? System.Drawing.Imaging.PixelFormat.Format32bppPArgb : System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            //return bmp;

            //TODO
            BitmapInfo info = GetBitmapInfo(page);
            if (info.Width * info.Height == 0)
                return null;

            //int size = Marshal.SizeOf(info.Buffer[0] * info.Buffer.Length);
            //IntPtr buf = Marshal.AllocHGlobal(size);
            //Marshal.Copy(info.Buffer, 0, buf, info.Buffer.Length);

            System.Drawing.Bitmap result = GetBitmap(info.BufPtr, info.Width, info.Height, info.Stride, info.Dpi, this.m_page_transparent ? System.Drawing.Imaging.PixelFormat.Format32bppPArgb : System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            return result;
        }
		/// <summary> Draws the contents of the page to a given Graphics object.
		/// 
		/// </summary>		
		/// <param name="page">The source PDF page.
		/// </param>
		/// <param name="dc">Device context (i.e. HDC structure).
		/// </param>
		/// <param name="rect">The rectangle in the device context inside of which the page will be drawn.
		/// </param>
		/// <remarks>this method is only supported on Windows platforms. If your application is running on a Windows 
		/// platform, you can select GDI+ rasterizer with SetRasterizerType() and e_GDIPlus type.
		/// </remarks>
        [Obsolete("Deprecated, will be removed in the next update.")]
        public void DrawInRect(Page page, IntPtr dc, Rect rect)
		{
			
		}
        /// <summary> Draws the contents of the page to a given Graphics object.
        /// 
        /// </summary>
        /// <param name="page">The source PDF page.
        /// </param>
        /// <param name="gr">Device context
        /// </param>		
        /// <param name="rect">The rectangle in the device context inside of which the page will be drawn.
        /// </param>
        /// <remarks>this method is only supported on Windows platforms. If your application is running on a Windows platform, you can select GDI+ rasterizer with SetRasterizerType&#40;&#41; and e_GDIPlus type.
        /// </remarks>
        [Obsolete("Deprecated, will be removed in the next update.")]
        public void DrawInRect(Page page, System.Drawing.Graphics gr, Rect rect)
		{
			
		}
#endif

        public BitmapInfo GetBitmap(Page page, PixelFormat pix_fmt, bool demult)
        {
            //byte[] result = new byte[0];
            IntPtr src = IntPtr.Zero;
            int width = 0;
            int height = 0;
            int stride = 0;
            double dpi = 0.0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDrawGetBitmap(mp_draw, page.mp_page, ref width, ref height, ref stride, ref dpi, pix_fmt, demult, ref src));

            //byte[] result = new byte[width * stride];
            //System.Runtime.InteropServices.Marshal.Copy(src, result, 0, width * stride);

            return new BitmapInfo(width, height, stride, dpi, src);
        }

        public BitmapInfo GetBitmapInfo(Page page)
        {
            return GetBitmap(page, PixelFormat.e_bgra, false);
        }

#if (__ANDROID__)
		public Android.Graphics.Bitmap GetBitmap(Page page)
		{
			BitmapInfo info = GetBitmapInfo (page);
			int width = info.Width;
			int height = info.Height;
			if (width * height == 0) 
			{
				return null;
			}

			int size = Marshal.SizeOf(info.Buffer[0]) * info.Buffer.Length;
			IntPtr buf = Marshal.AllocHGlobal(size);
			try
			{
				Marshal.Copy(info.Buffer, 0, buf, info.Buffer.Length);

				int sz = width*height;
				int[] buf_copy = new int[sz];
				System.Runtime.InteropServices.Marshal.Copy (buf, buf_copy, 0, sz);

				Android.Graphics.Bitmap bmp = Android.Graphics.Bitmap.CreateBitmap (buf_copy, width, height, Android.Graphics.Bitmap.Config.Argb4444);
				return bmp;
			}
			finally 
			{
				Marshal.FreeHGlobal (buf);
			}
		}
#endif


        public enum PixelFormat
        {
            e_rgba,
            e_bgra,
            e_rgb,
            e_bgr,
            e_gray,
            e_gray_alpha,
            e_cmyk,
            e_cmyka
        }
    }
}
