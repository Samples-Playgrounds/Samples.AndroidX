using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Exception = System.IntPtr;
using TRN_PDFRasterizer = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> PDFRasterizer is a low-level PDF rasterizer.
    /// The main purpose of this class is to convert PDF pages to raster images (or bitmaps).
    /// PDFRasterizer is a relatively low-level class. If you need to convert PDF page to an image format or a Bitmap, 
    /// consider using PDF.PDFDraw. Similarly, if you are building an interactive PDF viewing application 
    /// use PDF.PDFViewCtrl instead.
    /// </summary>
    public class PDFRasterizer : IDisposable
    {
        internal TRN_PDFRasterizer mp_rast = IntPtr.Zero;
        internal volatile IntPtr mp_cancel = IntPtr.Zero;
        internal PDFRasterizer(TRN_PDFRasterizer imp) 
        {
            this.mp_rast = imp;
			this.mp_cancel = Marshal.AllocHGlobal(sizeof(byte));
            Marshal.WriteByte(this.mp_cancel, 0, 0); // set to false
        }

        /// <summary>empty <c>PDFRasterizer</c> constructor
        /// </summary>
        public PDFRasterizer()
        {
            Type type = Type.e_BuiltIn;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerCreate(type, ref mp_rast));
			this.mp_cancel = Marshal.AllocHGlobal(sizeof(byte));
            Marshal.WriteByte(this.mp_cancel, 0, 0); // set to false
        }
        /// <summary>instantiates <c>PDFRasterizer</c> with specified type
        /// </summary>
        /// <param name="type">PDFRasterizer type
        /// </param>
        [Obsolete("Deprecated, will be removed in the next update.")]
        public PDFRasterizer(Type type)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerCreate(type, ref mp_rast));
			this.mp_cancel = Marshal.AllocHGlobal(sizeof(byte));
            Marshal.WriteByte(this.mp_cancel, 0, 0); // set to false
        }

        /// <summary>Sets rasterizer to the specified type
        /// </summary>
        /// <param name="type">PDFRasterizer type
        /// </param>
        [Obsolete("Deprecated, will be removed in the next update.")]
        public void SetRasterizerType(Type type)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetRasterizerType(mp_rast, type));
        }
        /// <summary>Gets the type of current rasterizer.
        /// </summary>
        /// <returns>the type of current rasterizer.
        /// </returns>
        [Obsolete("Deprecated, will be removed in the next update.")]
        public Type GetRasterizerType()
        {
            Type result = Type.e_BuiltIn;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerGetRasterizerType(mp_rast, ref result));
            return result;
        }

	    /// <summary>Enable or disable annotation and forms rendering. By default, annotations and forms are rendered.
	    /// </summary>
        /// <param name="render_annots">True to draw annotations, false otherwise.
        /// </param>
        public void SetDrawAnnotations(bool render_annots)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetDrawAnnotations(mp_rast, render_annots));
        }

	    /// <summary>Enable or disable highlighting form fields. Default is disabled.
	    /// </summary>
        /// <param name="highlight_fields">true to highlight, false otherwise. 
        /// </param>
        public void SetHighlightFields(bool highlight_fields)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetHighlightFields(mp_rast, highlight_fields));
        }

	    /// <summary>Sets the gamma factor used for anti-aliased rendering.
	    /// </summary>
	    /// <param name="exp">exponent value of gamma function. Typical values are in the range from 0.1 to 3.
	    /// </param>
	    /// <remarks>Gamma correction can be used to improve the quality of anti-aliased image output and 
	    /// can (to some extent) decrease the appearance common anti-aliasing artifacts (such as pixel width lines between polygons).
        /// Gamma correction is used only in the built-in rasterizer.
        /// </remarks>
        public void SetGamma(double exp)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetGamma(mp_rast, exp));
        }
	    /// <summary>Sets the Optional Content Group (OCG) context that should be used when rendering the page. 
	    /// This function can be used to selectively render optional content (such as PDF layers) based on the 
	    /// states of optional content groups in the given context.
	    /// </summary>
        /// <param name="ctx">Optional Content Group (OCG) context, or NULL if the rasterizer should render all content on the page.
        /// </param>
        public void SetOCGContext(OCG.Context ctx)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetOCGContext(mp_rast, ctx.mp_impl));
        }

        /// <summary>Cancel the rendering in progress.</summary>
        public void CancelRendering()
        {
            Marshal.WriteByte(this.mp_cancel, 0, 1); // set to true
        }

	    /// <summary>Tells the rasterizer to render the page 'print' mode. 
	    /// Certain page elements (such as annotations or OCG-s) are meant to be visible either on the 
	    /// screen or on the printed paper but not both. A common example, is the "Submit" button on electronic forms.
	    /// </summary>
        /// <param name="is_printing">set to true is the page should be rendered in print mode. By default, print mode flag is set to false.
        /// </param>
        public void SetPrintMode(bool is_printing)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetPrintMode(mp_rast, is_printing));
        }
	    /// <summary>Enable or disable anti-aliasing. Anti-Aliasing is a technique used to improve the visual quality of images 
	    /// when displaying them on low resolution devices (for example, low DPI computer monitors). Anti-aliasing is 
	    /// enabled by default.
	    /// </summary>
        /// <param name="enable_aa">true to enable anti-aliasing.
        /// </param>
        public void SetAntiAliasing(bool enable_aa)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetAntiAliasing(mp_rast, enable_aa));
        }
	    /// <summary>Enable or disable path hinting. Path hinting is used to slightly adjust paths in order to avoid or alleviate artifacts of hair line cracks between
	    ///	certain graphical elements. This option is turned on by default.
	    /// </summary>
        /// <param name="enable_ph">whether to enable path hinting.
        /// </param>
        public void SetPathHinting(bool enable_ph)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetPathHinting(mp_rast, enable_ph));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetThinLineAdjustment(mp_rast, pixel_grid_fit, stroke_adjust));
        }
	    /// <summary> Enable or disable image smoothing.			
	    /// The rasterizer allows a tradeoff between rendering quality and rendering speed.
	    /// This function can be used to indicate the preference between rendering speed and quality.image smoothing is enabled by default.
	    /// </summary>
	    /// <param name="smoothing_enabled">whether to enable image smoothing
	    /// </param>
        /// <param name="hq_image_resampling">whether to use a higher quality (but slower) smoothing algorithm
        /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  image smoothing option has effect only if the source image has higher resolution 
        /// that the output resolution of the image on the rasterized page. PDFNet automatically
        /// controls at what resolution&#47;zoom factor, 'image smoothing' needs to take effect.</remarks>
        public void SetImageSmoothing(bool smoothing_enabled, bool hq_image_resampling)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetImageSmoothing(mp_rast, smoothing_enabled, hq_image_resampling));
        }
	    /// <summary>Enable or disable support for overprint. Overprint is a device dependent feature and the results will vary depending on the output color space and supported colorants (i.e. CMYK, CMYK+spot, RGB, etc). By default overprint is enabled for only PDF/X files.
	    /// </summary>
        /// <param name="op">e_op_on: always enabled; e_op_off: always disabled; e_op_pdfx_on: enabled for PDF/X files only.
        /// </param>
        public void SetOverprint(OverprintPreviewMode op)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetOverprint(mp_rast, op));
        }
	    /// <summary> Sets the caching.
	    /// 
	    /// </summary>
	    /// <param name="enabled">the new caching
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetCaching(bool enabled)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetCaching(mp_rast, enabled));
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
        void SetColorPostProcessMode(ColorPostProcessMode mode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerSetColorPostProcessMode(mp_rast, mode));
        }
        /// <returns>the current color post processing mode.</returns>
        ColorPostProcessMode GetColorPostProcessMode()
        {
            ColorPostProcessMode result = ColorPostProcessMode.e_postprocess_none;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerGetColorPostProcessMode(mp_rast, ref result));
            return result;
        }
	    /// <summary>Draws the page into a given memory buffer.
	    /// </summary>
	    /// <param name="page">The page to rasterize.
	    /// </param>
	    /// <param name="in_out_image_buffer">A pointer to a memory buffer. The buffer must contain at least (stride * height) bytes.
	    /// </param>
	    /// <param name="width">The width of the target image in pixels.
	    /// </param>
	    /// <param name="height">The height of the target image in pixels (the number of rows).
	    /// </param>
	    /// <param name="stride">Stride determines the physical width (in bytes) of one row in memory. If this value is negative the direction of the Y axis is inverted. The absolute value of stride is of importance, because it allows rendering in buffers where rows are padded in memory (e.g. in Windows bitmaps are padded on 4 byte boundaries). Besides allowing rendering on the whole buffer stride parameter can be used for rendering in a rectangular subset of a buffer.
	    /// </param>
	    /// <param name="num_comps">The number (4 or 5) representing the number of color components in the device color space. For BGR+Alpha set this parameter to 4, and for CMYK+Alpha use 5. If other values are set, exceptions will be thrown.
	    /// </param>
	    /// <param name="demult">Specifies if the alpha is de-multiplied from the resulting color components.
	    /// </param>
	    /// <param name="device_mtx">Device transformation matrix that maps PDF page from PDF user space into device coordinate space (e.g. pixel space). PDF user space is represented in page units, where one unit corresponds to 1/72 of an inch.
	    /// </param>
	    /// <param name="clip">Optional parameter defining the clip region for the page. If the parameter is null or is not specified, PDFRasterizer uses page's crop box as a default clip region.
	    /// </param>
	    /// <example>
	    /// <code>
	    /// float drawing_scale = 2: 
	    /// Common.Matrix2D mtx(drawing_scale, 0, 0, drawing_scale, 0, 0);
	    /// PDF.Rect bbox(page.GetMediaBox());
	    /// bbox.Normalize();
	    /// int width = int(bbox.Width() * drawing_scale); 
	    /// int height = int(bbox.Height() * drawing_scale);
	    ///	
	    /// // Stride is represented in bytes and is aligned on 4 byte 
	    /// // boundary so that you can render directly to GDI bitmap.
	    /// // A negative value for stride can be used to flip the image 
	    /// // upside down.
	    /// int comps = 4;  // for BGRA
	    /// int stride = ((width * comps + 3) / 4) * 4;
	    /// // buf is a memory buffer containing at least (stride*height) bytes.
	    /// 	 memset(ptr, 0xFF, height*stride);  // Clear the background to opaque white paper color.
	    ///	PDFRasterizer rast;
	    ///    rast.Rasterize(page, buf, width, height, stride, 4, false, mtx);
        /// </code>
        /// </example>
		public byte[] Rasterize(Page page, int width, int height, int stride, int num_comps, bool demult, Matrix2D device_mtx, Rect clip)
        {
			int psize = height * stride;
            var outBytes = new byte[psize];
            GCHandle pinnedRawData = GCHandle.Alloc(outBytes, GCHandleType.Pinned);
            IntPtr rect = IntPtr.Zero;
            bool needToFreeIntPtr = false;
			try {
                if (clip != null)
                {                    
                    rect = Marshal.AllocHGlobal(Marshal.SizeOf(clip.mp_imp));
                    needToFreeIntPtr = true;
                    Marshal.StructureToPtr(clip.mp_imp, rect, true);
                }
                IntPtr source = pinnedRawData.AddrOfPinnedObject();

                PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerRasterizeToMemory(mp_rast, page.mp_page, source,
                width, height, stride, num_comps, demult, ref device_mtx.mp_mtx, rect, IntPtr.Zero, mp_cancel));
                return outBytes;
			} finally {
                pinnedRawData.Free();
                if (needToFreeIntPtr)
                {
                    Marshal.FreeHGlobal(rect);
                }
			}
        }

	    /// <summary>Draws the page into a given memory buffer.
	    /// </summary>
	    /// <param name="page">The page to rasterize.
	    /// </param>
	    /// <param name="in_out_image_buffer">A pointer to a memory buffer. The buffer must contain at least (stride * height) bytes.
	    /// </param>
	    /// <param name="width">The width of the target image in pixels.
	    /// </param>
	    /// <param name="height">The height of the target image in pixels (the number of rows).
	    /// </param>
	    /// <param name="stride">Stride determines the physical width (in bytes) of one row in memory. If this value is negative the direction of the Y axis is inverted. The absolute value of stride is of importance, because it allows rendering in buffers where rows are padded in memory (e.g. in Windows bitmaps are padded on 4 byte boundaries). Besides allowing rendering on the whole buffer stride parameter can be used for rendering in a rectangular subset of a buffer.
	    /// </param>
	    /// <param name="num_comps">The number (4 or 5) representing the number of color components in the device color space. For BGR+Alpha set this parameter to 4, and for CMYK+Alpha use 5. If other values are set, exceptions will be thrown.
	    /// </param>
	    /// <param name="demult">Specifies if the alpha is de-multiplied from the resulting color components.
	    /// </param>
	    /// <param name="device_mtx">Device transformation matrix that maps PDF page from PDF user space into device coordinate space (e.g. pixel space). PDF user space is represented in page units, where one unit corresponds to 1/72 of an inch.
	    /// </param>
	    /// <seealso cref="Rasterize(Page, IntPtr, int, int, int, int, bool, Matrix2D, Rect)">
	    /// </seealso>
		public byte[] Rasterize(Page page, int width, int height, int stride, int num_comps, bool demult, Matrix2D device_mtx)
        {
            return Rasterize(page, width, height, stride, num_comps, demult, device_mtx, null);
        }

	    /// <summary>Draws the page into a given memory buffer.
	    /// </summary>
	    /// <param name="page">The page to rasterize.
	    /// </param>
	    /// <param name="hdc">
	    /// </param>			
	    /// <param name="device_mtx">Device transformation matrix that maps PDF page from PDF user space into device coordinate space (e.g. pixel space). PDF user space is represented in page units, where one unit corresponds to 1/72 of an inch.
	    /// </param>
	    /// <param name="dpi">
	    /// </param>
	    /// <param name="clip">
	    /// </param>
        /// <seealso cref="Rasterize(Page, IntPtr, int, int, int, int, bool, Matrix2D, Rect)">
        /// </seealso>
//        public void Rasterize(Page page, IntPtr hdc, Matrix2D device_mtx, Rect clip, int dpi)
//        {
//            if (device_mtx == null) device_mtx = new Matrix2D();
//            BasicTypes.TRN_Rect rect;
//            if (clip == null)
//                rect = new BasicTypes.TRN_Rect();
//            else
//                rect = clip.mp_imp;
//            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerRasterizeToDevice(mp_rast, page.mp_page, hdc, ref device_mtx.mp_mtx, ref rect, dpi, false));
//        }
	    /// <summary> Releases all resources used by the PDFRasterizer </summary>
	    ~PDFRasterizer() 
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
            Destroy();
            // Clean up native resources
            if (IntPtr.Zero != mp_cancel)
            {
                Marshal.FreeHGlobal(mp_cancel);
            }
        }
        public void Destroy()
        {
            if (mp_rast != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_PDFRasterizerDestroy(mp_rast));
                mp_rast = IntPtr.Zero;
            }
        }

        // Nested Types
        /// <summary>Determines if overprint is used.</summary>
        public enum OverprintPreviewMode
        {
            ///<summary>overprint is always off.</summary>
            e_op_off = 0,
            ///<summary>overprint is always on.</summary>
            e_op_on,
            ///<summary>overprint is on only for PDF/X files.</summary>
            e_op_pdfx_on
        }

        /// <summary>PDFNet includes two separate rasterizer implementations utilizing different graphics libraries.
        /// The default rasterizer is 'e_BuiltIn' which is a high-quality, anti-aliased and platform independent rasterizer. 
        /// This rasterizer is available on all supported platforms.
        /// On Windows platforms, PDFNet also includes GDI+ based rasterizer. 
        /// This rasterizer is included mainly to provide vector output for printing, for EMF/WMF export, etc. 
        /// For plain image rasterization we recommend using the built-in rasterizer.
        /// </summary>
        [Obsolete("Deprecated, will be removed in the next update.")]
        public enum Type
        {
            ///<summary>high-quality, platform independent rasterizer.</summary>
            e_BuiltIn,
            ///<summary>GDI+ based rasterizer.</summary>
            e_GDIPlus
        }

        /// <summary>ColorPostProcessMode is used to modify colors after rendering.</summary>
		public enum ColorPostProcessMode
		{
			e_postprocess_none = 0,
			e_postprocess_invert,
            e_postprocess_gradient_map,
            e_postprocess_night_mode
		}
    }
}
