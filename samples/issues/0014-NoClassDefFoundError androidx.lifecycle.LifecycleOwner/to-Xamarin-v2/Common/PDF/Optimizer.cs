using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Optimizer = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary>
    /// The Optimizer class provides functionality for optimizing/shrinking
    /// output PDF files.
    /// 
    /// 'pdftron.PDF.Optimizer' is an optional PDFNet Add-On utility class that can be 
    /// used to optimize PDF documents by reducing the file size, removing redundant 
    /// information, and compressing data streams using the latest in image compression 
    /// technology. PDF Optimizer can compress and shrink PDF file size with the 
    /// following operations:
    /// - Remove duplicated fonts, images, ICC profiles, and any other data stream. 
    /// - Optionally convert high-quality or print-ready PDF files to small, efficient and web-ready PDF. 
    /// - Optionally down-sample large images to a given resolution. 
    /// - Optionally compress or recompress PDF images using JBIG2 and JPEG2000 compression formats. 
    /// - Compress uncompressed streams and remove unused PDF objects. 
    /// 
    /// @note 'Optimizer' is available as a separately licensable add-on to PDFNet 
    /// core license or for use via Cloud API (http://www.pdftron.com/pdfnet/cloud).
    /// 
    /// @note See 'pdftron.PDF.Flattener' for alternate approach to optimize PDFs for fast 
    /// viewing on mobile devices and the Web.
    /// </summary>
    public class Optimizer
    {
        /// <summary>Optimize the given document using the optimizers settings
        /// </summary>
        /// <param name="doc"> the document to optimize
        /// </param>		
        public static void Optimize(PDFDoc doc)
        {
            OptimizerSettings setting = new OptimizerSettings();
            setting.m_color_image_settings = new ImageSettings();
            setting.m_grayscale_image_settings = new ImageSettings();
            setting.m_mono_image_settings = new MonoImageSettings();
            setting.m_text_settings = new TextSettings();
            Optimize(doc, setting);
        }

        /// <summary>Optimize the given document using the optimizers settings
        /// </summary>
        /// <param name="doc"> the document to optimize
        /// </param>
        /// <param name="settings">optimizer settings
        /// </param>		
        public static void Optimize(PDFDoc doc, OptimizerSettings settings)
        {
            TRN_optimizerimagesettings color_image_settings_struct = settings.m_color_image_settings == null ? (new ImageSettings()).mp_struct : settings.m_color_image_settings.mp_struct;
            TRN_optimizerimagesettings grayscale_image_settings_struct = settings.m_grayscale_image_settings == null ? (new ImageSettings()).mp_struct : settings.m_grayscale_image_settings.mp_struct;
            TRN_optimizermonoimagesettings mono_image_settings_struct = settings.m_mono_image_settings == null ? (new MonoImageSettings()).mp_struct : settings.m_mono_image_settings.mp_struct;
            TRN_optimizertextsettings text_settings_struct = settings.m_text_settings == null ? (new TextSettings()).mp_struct : settings.m_text_settings.mp_struct;

            IntPtr color_image_settings_pnt = Marshal.AllocHGlobal(Marshal.SizeOf(color_image_settings_struct));
            IntPtr grayscale_image_settings_pnt = Marshal.AllocHGlobal(Marshal.SizeOf(grayscale_image_settings_struct));
            IntPtr mono_image_settings_pnt = Marshal.AllocHGlobal(Marshal.SizeOf(mono_image_settings_struct));
            IntPtr text_settings_pnt = Marshal.AllocHGlobal(Marshal.SizeOf(text_settings_struct));
            try
            {
                // Copy the struct to unmanaged memory.
                Marshal.StructureToPtr(color_image_settings_struct, color_image_settings_pnt, false);
                Marshal.StructureToPtr(grayscale_image_settings_struct, grayscale_image_settings_pnt, false);
                Marshal.StructureToPtr(mono_image_settings_struct, mono_image_settings_pnt, false);
                Marshal.StructureToPtr(text_settings_struct, text_settings_pnt, false);

                PDFNetException.REX(PDFNetPINVOKE.TRN_OptimizerOptimize(doc.mp_doc,
                    color_image_settings_pnt,
                    grayscale_image_settings_pnt,
                    mono_image_settings_pnt,
                    text_settings_pnt,
                    settings.m_remove_custom));
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(color_image_settings_pnt);
                Marshal.FreeHGlobal(grayscale_image_settings_pnt);
                Marshal.FreeHGlobal(mono_image_settings_pnt);
                Marshal.FreeHGlobal(text_settings_pnt);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct TRN_optimizerimagesettings
        {
            public UIntPtr m_max_pixels;
            public ImageSettings.CompressionMode m_compression_mode;
            public ImageSettings.DownsampleMode m_downsample_mode;
            public UInt32 m_quality;
            public double m_max_dpi, m_resample_dpi;
            public byte m_force_recompression, m_force_changes;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct TRN_optimizermonoimagesettings
        {
            public UIntPtr m_max_pixels;
            public MonoImageSettings.CompressionMode m_compression_mode;
            public MonoImageSettings.DownsampleMode m_downsample_mode;
            public double m_max_dpi, m_resample_dpi, m_jbig2_threshold;
            public byte m_force_recompression, m_force_changes;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct TRN_optimizertextsettings
        {
            public byte m_subset_fonts, m_embed_fonts;
        }

        ///<summary>A class that stores downsampling/recompression
        /// settings for color and grayscale images.
        ///</summary>
        public class ImageSettings
        {
            internal TRN_optimizerimagesettings mp_struct;

            /// <summary> Different Compression Modes for color and grayscale images.
            /// </summary>
            public enum CompressionMode
            {
                e_retain,
                e_flate,
                e_jpeg,
                e_jpeg2000,
                e_none
            }

            /// <summary> Different Downsample Modes for color and grayscale images.
            /// </summary>
            public enum DownsampleMode
            {
                e_off,
                e_default
            }
            /// <summary>create an ImageSetting object with default options
            /// </summary>
            public ImageSettings()
            {
                mp_struct = new TRN_optimizerimagesettings();
                mp_struct.m_compression_mode = CompressionMode.e_retain;
                mp_struct.m_downsample_mode = DownsampleMode.e_default;
                mp_struct.m_quality = 5;
                mp_struct.m_max_dpi = 225.0;
                mp_struct.m_resample_dpi = 150.0;
                mp_struct.m_force_recompression = System.Convert.ToByte(false);
                mp_struct.m_force_changes = System.Convert.ToByte(false);

                IntPtr image_settings_pnt = Marshal.AllocHGlobal(Marshal.SizeOf(mp_struct));
                try
                {
                    // Copy the struct to unmanaged memory.
                    Marshal.StructureToPtr(mp_struct, image_settings_pnt, false);
                    PDFNetPINVOKE.TRN_OptimizerImageSettingsInit(image_settings_pnt);
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(image_settings_pnt);
                }
            }

            /// <summary>Sets the maximum and resampling dpi for images.
            /// By default these are set to 144 and 96 respectively.
            /// </summary>
            /// <param name="maximum">the highest dpi of an image before
            /// it will be resampled
            /// </param>
            /// <param name="resampling">resampling the image dpi to resample to if an
            /// image is encountered over the maximum dpi
            /// </param>
            public void SetImageDPI(double maximum, double resampling)
            {
                mp_struct.m_max_dpi = maximum;
                mp_struct.m_resample_dpi = resampling;
            }

            /// <summary>Sets the output compression mode for this type of image
            /// The default value is e_retain
            /// </summary>
            /// <param name="mode">the compression mode to set
            /// </param>
            public void SetCompressionMode(CompressionMode mode)
            {
                mp_struct.m_compression_mode = mode;
            }

            /// <summary>Sets the downsample mode for this type of image
            /// The default value is e_default
            /// </summary>
            /// <param name="mode">the downloadsample mode to set
            /// </param>
            public void SetDownsampleMode(DownsampleMode mode)
            {
                mp_struct.m_downsample_mode = mode;
            }

            /// <summary>Sets the quality for lossy compression modes
            /// from 1 to 10 where 10 is lossless (if possible)
            /// the default value is 5
            /// </summary>
            /// <param name="quality">compression quality to set
            /// </param>
            public void SetQuality(UInt32 quality)
            {
                mp_struct.m_quality = quality;
            }

            /// <summary>Sets whether recompression to the specified compression
            /// method should be forced when the image is not downsampled.
            /// By default the compression method for these images
            /// will not be changed.
            /// </summary>
            /// <param name="force">if true the compression method for all
            /// images will be changed to the specified compression mode
            /// </param>
            public void ForceRecompression(bool force)
            {
                mp_struct.m_force_recompression = System.Convert.ToByte(force);
            }

            /// <summary>Sets whether image changes that grow the
            /// PDF file should be kept.  This is off by default.
            /// </summary>
            /// <param name="force">if true all image changes will be kept.
            /// </param>
            public void ForceChanges(bool force)
            {
                mp_struct.m_force_changes = System.Convert.ToByte(force);
            }
        }

        /// <summary>A class that stores image downsampling/recompression
        /// settings for monochrome images.
        /// </summary>
        public class MonoImageSettings
        {
            internal TRN_optimizermonoimagesettings mp_struct;

            /// <summary>mono-image compression mode
            /// </summary>
            public enum CompressionMode
            {
                e_jbig2,
                e_flate,
                e_none
            }

            /// <summary>mono-image downsample mode
            /// </summary>
            public enum DownsampleMode
            {
                e_off,
                e_default
            }

            /// <summary>create an MonoImageSettings object with default options
            /// </summary>
            public MonoImageSettings()
            {
                mp_struct = new TRN_optimizermonoimagesettings();
                mp_struct.m_compression_mode = CompressionMode.e_jbig2;
                mp_struct.m_downsample_mode = DownsampleMode.e_default;
                mp_struct.m_max_dpi = 450.0;
                mp_struct.m_resample_dpi = 300.0;
                mp_struct.m_force_recompression = System.Convert.ToByte(false);
                mp_struct.m_force_changes = System.Convert.ToByte(false);
                mp_struct.m_jbig2_threshold = 8.5;

                IntPtr mono_image_settings_pnt = Marshal.AllocHGlobal(Marshal.SizeOf(mp_struct));
                try
                {
                    // Copy the struct to unmanaged memory.
                    Marshal.StructureToPtr(mp_struct, mono_image_settings_pnt, false);
                    PDFNetPINVOKE.TRN_OptimizerMonoImageSettingsInit(mono_image_settings_pnt);
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(mono_image_settings_pnt);
                }
            }

            /// <summary>Sets the maximum and resampling dpi for monochrome images.
            /// By default these are set to 144 and 96 respectively.
            /// </summary>
            /// <param name="maximum">the highest dpi of an image before
            /// it will be resampled
            /// </param>
            /// <param name="resampling">the image dpi to resample to if an
            /// image is encountered over the maximum dpi
            /// </param>
            public void SetImageDPI(double maximum, double resampling)
            {
                mp_struct.m_max_dpi = maximum;
                mp_struct.m_resample_dpi = resampling;
            }

            /// <summary>Sets the output compression mode for monochrome images
            /// The default value is e_jbig2
            /// </summary>
            /// <param name="mode">the compression mode to set
            /// </param>
            public void SetCompressionMode(CompressionMode mode)
            {
                mp_struct.m_compression_mode = mode;
            }


            /// <summary>Sets the downsample mode for monochrome images
            /// The default value is e_default
            /// </summary>
            /// <param name="mode">the compression mode to set
            /// </param>
            public void SetDownsampleMode(DownsampleMode mode)
            {
                mp_struct.m_downsample_mode = mode;
            }


            /// <summary>Sets whether recompression to the specified compression
            /// method should be forced when the image is not downsampled.
            /// By default the compression method for these images
            //  will not be changed.
            /// </summary>
            /// <param name="force">if true the compression method for all
            /// images will be changed to the specified compression mode
            /// </param>
            public void ForceRecompression(bool force)
            {
                mp_struct.m_force_recompression = System.Convert.ToByte(force);
            }

            /// <summary>Sets whether image changes that grow the
            /// PDF file should be kept.  This is off by default.
            /// </summary>
            /// <param name="force">if true all image changes will be kept.
            /// </param>
            public void ForceChanges(bool force)
            {
                mp_struct.m_force_changes = System.Convert.ToByte(force);
            }

            /// <summary>Sets the quality for lossy compression modes
            /// from 1 to 10 where 10 is lossless (if possible).
            /// The default value for JBIG2 is 8.5.  The setting is
            /// ignored for FLATE.
            /// </summary>
            /// <param name="jbig2_threshold">The jbig2 threshold value
            /// </param>
            public void SetJBIG2Threshold(double jbig2_threshold)
            {
                mp_struct.m_jbig2_threshold = jbig2_threshold;
            }
        }

        /// <summary> A class that stores text optimization settings.
        /// </summary>
        public class TextSettings
        {
            internal TRN_optimizertextsettings mp_struct;

            /// <summary>create an TextSettings object with default options
            /// </summary>
            public TextSettings()
            {
                mp_struct = new TRN_optimizertextsettings();
                mp_struct.m_subset_fonts = System.Convert.ToByte(false);
                mp_struct.m_embed_fonts = System.Convert.ToByte(false);

                IntPtr text_settings_pnt = Marshal.AllocHGlobal(Marshal.SizeOf(mp_struct));
                try
                {
                    // Copy the struct to unmanaged memory.
                    Marshal.StructureToPtr(mp_struct, text_settings_pnt, false);
                    PDFNetPINVOKE.TRN_OptimizerTextSettingsInit(text_settings_pnt);
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(text_settings_pnt);
                }
            }

            /// <summary>Sets whether embedded fonts will be subset.  This
            /// will generally reduce the size of fonts, but will
            /// strip font hinting.  Subsetting is off by default. </summary>
            /// <param name="subset">if true all embedded fonts will be subsetted.
            /// </param>
            public void SubsetFonts(bool subset)
            {
                mp_struct.m_subset_fonts = System.Convert.ToByte(subset);
            }

            /// <summary>Sets whether fonts should be embedded.  This
            /// will generally increase the size of the file, but will
            /// make the file appear the same on different machines.  
            /// Font embedding is off by default. </summary>
            /// <param name="embed"> if true all fonts will be embedded.
            /// </param>
            public void EmbedFonts(bool embed)
            {
                mp_struct.m_embed_fonts = System.Convert.ToByte(embed);
            }
        }

        /// <summary> A class that stores settings
        /// for the optimizer</summary>
        public class OptimizerSettings
        {
            /// <summary>create an OptimizerSettings object with default options
            /// </summary>
            public OptimizerSettings()
            {
                m_remove_custom = true;
            }

            /// <summary>updates the settings for color image processing</summary>
            public void SetColorImageSettings(ImageSettings settings)
            {
                m_color_image_settings = settings;
            }

            /// <summary>updates the settings for grayscale image processing</summary>
            public void SetGrayscaleImageSettings(ImageSettings settings)
            {
                m_grayscale_image_settings = settings;
            }

            /// <summary>updates the settings for monochrome image processing</summary>
            public void SetMonoImageSettings(MonoImageSettings settings)
            {
                m_mono_image_settings = settings;
            }

            /// <summary>updates the settings for text processing</summary>
            public void SetTextSettings(TextSettings settings)
            {
                m_text_settings = settings;
            }

            /// <summary>Enable or disable removal of custom entries in the PDF. By default custom entries are removed.</summary>
            /// <param name="should_remove">if true custom entries will be removed.</param>
            public void RemoveCustomEntries(bool should_remove)
            {
                m_remove_custom = should_remove;
            }

            public ImageSettings m_color_image_settings { get; set; }
            public ImageSettings m_grayscale_image_settings { get; set; }
            public MonoImageSettings m_mono_image_settings { get; set; }
            public TextSettings m_text_settings { get; set; }
            public bool m_remove_custom { get; set; }
        }
    }
}