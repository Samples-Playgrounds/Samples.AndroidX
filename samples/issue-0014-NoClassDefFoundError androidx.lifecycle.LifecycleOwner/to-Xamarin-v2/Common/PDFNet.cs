using System;
using System.Runtime.InteropServices;

#if (__ANDROID__)
using Android.Views;
using Android.Content;
using Android.Util;
#endif

using pdftronprivate.trn;
using pdftron.Common;

using TRN_PDFDoc = System.IntPtr;
using TRN_PDFDocInfo = System.IntPtr;
using TRN_UString = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_PDFView = System.IntPtr;


namespace pdftron
{
    /// <summary>
    /// PDFNet contains global library initialization, registration, configuration,
    /// and termination methods. 
    /// 	
    /// there is only a single, static instance of PDFNet class. Initialization
    /// and termination methods need to be called only once per application session. 
    /// </summary>
    public class PDFNet
    {
#if (__DESKTOP__)
        // Methods    
        /// <summary> Initializes PDFNet library.
        /// <c>Initialize()</c> is usually called once, during process initialization.
        /// </summary>
        /// <remarks>  With the exception of SetPersistentCache and SetTempPath, it is unsafe to call any other PDFNet API without first initializing 
        /// the library
        /// </remarks>
        public static void Initialize()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInitialize(null));
        }
        /// <summary> Initializes PDFNet library.
        /// Initialize() is usually called once, during process initialization.
        /// 
        /// </summary>
        /// <param name="license_key">license key used to activate the product.
        /// If the license_key is not specified, the product will work in demo mode.
        /// If the license_key is invalid, the function will throw an exception.
        /// </param>
        /// <remarks>  With the exception of SetPersistentCache and SetTempPath, it is unsafe to call any other PDFNet API without first initializing 
        /// the library
        /// </remarks>
        public static void Initialize(string license_key)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInitialize(license_key));
        }

        /// <summary>
        /// Dummy method
        /// </summary>
        public static void Terminate()
        {

        }

#elif (__ANDROID__)
		// ==== For Android PDFNet.Initialize ==== //
		public static void Initialize(Context context, int resId, string license_key)
		{
			pdftronprivate.PDF.PDFNet.Initialize(context, resId, license_key);
		}
        // ==== For Android PDFNet.Initialize ==== //
#elif (__IOS__)
		// ==== For iOS PDFNet.Initialize ==== //
		/// <summary> Initializes PDFNet library.
		/// Initialize() is usually called once, during process initialization.
		/// 
		/// </summary>
		/// <param name="license_key">license key used to activate the product.
		/// If the license_key is not specified, the product will work in demo mode.
		/// If the license_key is invalid, the function will throw an exception.
		/// </param>
		/// <remarks>  With the exception of SetPersistentCache and SetTempPath, it is unsafe to call any other PDFNet API without first initializing 
		/// the library
		/// </remarks>
		public static void Initialize(string license_key)
		{
			pdftronprivate.PTPDFNet.Initialize (license_key);
		}
		// ==== For iOS PDFNet.Initialize ==== //
#endif

        /// <summary>
        /// A swtich that can be used to turn on/off JavaScript engine
        /// </summary>
        /// <param name="enable"> true to enable JavaScript engine, false to disable.</param>
        ///
        public static void EnableJavaScript(bool enable)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetEnableJavaScript(enable));
        }

        /// <summary>
        /// Test whether JavaScript is enabled.
        /// </summary>
        /// <returns> true, if it is enabled, false otherwise.
        /// </returns>
        public static bool IsJavaScriptEnabled()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetIsJavaScriptEnabled(ref result));
            return result;
        }

        /// <summary> Sets the location of PDFNet resource file.
        /// 
        /// </summary>
        /// <param name="path">- The default resource directory path.
        /// </param>
        /// <returns> true if path is found, false otherwise.
        /// </returns>
        /// <remarks>  Starting with v.4.5 PDFNet does't require a separate resource
        /// file (pdfnet.res) on all desktop/server platfroms. As a result, this function
        /// is not longer required for proper PDFNet initialization. The function is still
        /// available on embedded systems and for backwards compatibility. The function can
        /// be also used to specify a deault search path for ICC profiles, fonts, and other
        /// user defined resources.
        /// </remarks>
        public static bool SetResourcesPath(string path)
        {
            bool result = false;
            UString str = new UString(path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetSetResourcesPath(str.mp_impl, ref result));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>the location of PDFNet resources folder. Empty string means 
        /// that resources are located in your application folder.</returns>
        public static string GetResourcesPath()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetGetResourcesPath(ref result.mp_impl));
            return result.ConvToManagedStr();
        }

        /// <summary> Adds a search location for PDFNet resource files.
		/// 
		/// </summary>
		/// <param name="val">- The resource directory path to add to the searched list.
		/// </param>
		/// <remarks>  Starting with v.4.5 PDFNet does't require a separate resource
		/// file (pdfnet.res) on all desktop/server platfroms. As a result, this function
		/// is not longer required for proper PDFNet initialization. The function is still
		/// available on embedded systems and for backwards compatibility. This function can
		/// be also used to specify search paths for ICC profiles, fonts, and other
		/// user defined resources.
		/// </remarks>
		public static void AddResourceSearchPath(String val)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetAddResourceSearchPath(UString.ConvertToUString(val).mp_impl));
        }

        /// <summary> Gets the version.
        /// 
        /// </summary>
        /// <returns> PDFNet version number.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static double GetVersion()
        {
            double ver = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetGetVersion(ref ver));
            return ver;
        }

        /// <summary> Used to set a specific Color Management System (CMS) for
        /// use during color conversion operators, image rendering, etc.
        /// 
        /// </summary>
        /// <param name="cms">identifies the type of color management to use.
        /// </param>
        /// <exception cref="PDFNetException">PDFNetException the PDFNet exception</exception>
        public static void SetColorManagement(CMSType cms)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetSetColorManagement(cms));
        }

        /// <summary> Sets the default ICC color profile for DeviceCMYK color space.
        /// 
        /// </summary>
        /// <param name="icc_filename">the new default device cmyk profile
        /// </param>		
        /// <remarks>  You can use this method to override default PDFNet settings.
        /// For more information on default color spaces please refer to
        /// section 'Default Color Spaces' in Chapter 4.5.4 of PDF Reference Manual.
        /// </remarks>
        // <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void SetDefaultDeviceCMYKProfile(string icc_filename)
        {
            UString str = new UString(icc_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetSetDefaultDeviceCMYKProfile(str.mp_impl));
        }

        /// <summary> Sets the default ICC color profile for DeviceRGB color space.
        /// 
        /// </summary>
        /// <param name="icc_filename">the new default device rgb profile
        /// </param>		
        /// <remarks>  You can use this method to override default PDFNet settings.
        /// For more information on default color spaces please refer to
        /// section 'Default Color Spaces' in Chapter 4.5.4 of PDF Reference Manual.
        /// </remarks>
        /// <exception cref="PDFNetException">PDFNetException the pDF net exception </exception>
        public static void SetDefaultDeviceRGBProfile(string icc_filename)
        {
            UString str = new UString(icc_filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetSetDefaultDeviceRGBProfile(str.mp_impl));
        }

        ///<summary>Sets the default policy on using temporary files.</summary>
        ///<param name="use_disk">if parameter is true then new documents are allowed to create
        /// temporary files; otherwise all document contents will be stored in memory.</param>
        public static void SetDefaultDiskCachingEnabled(bool use_disk)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetSetDefaultDiskCachingEnabled(use_disk));
        }

        ///<summary>Sets the default policy on using temporary files.</summary>
        /// <param name="level"> An integer in range 0-9 representing the compression value to use as
        /// a default for any Flate streams (e.g used to compress content streams, PNG images, etc).
        /// The library normally uses the default compression level (Z_DEFAULT_COMPRESSION).
        /// For most images, compression values in the range 3-6 compress nearly as well as higher
        /// levels, and do so much faster. For on-line applications it may be desirable to have
        /// maximum speed Z_BEST_SPEED = 1). You can also specify no compression (Z_NO_COMPRESSION = 0).
        /// Default is Z_DEFAULT_COMPRESSION (-1).
        /// </param>
        public static void SetDefaultFlateCompressionLevel(int level)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetSetDefaultFlateCompressionLevel(level));
        }

        /// <summary> Sets the default parameters for the viewer cache.  Any subsequently opened documents
        /// will use these parameters.
        /// </summary>
        /// <param name="max_cache_size">The maximum size, in bytes, of the entire document's page cache. Set to zero to disable the viewer cache.
        /// </param>
        /// <param name="on_disk">If set to 'true', cache will be stored on the local filesystem. If set to 'false', cache will be stored in heap memory.
        /// </param>
        /// <remarks>Default on Desktop - max_cache_size: 512 MB on_disk: true
        ///                  on Mobile  - max_cache_size: 100 MB on_disk: false
        /// </remarks>	 
        public static void SetViewerCache(uint max_cache_size, bool on_disk)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetSetViewerCache(new UIntPtr(max_cache_size), on_disk));
        }

        /// <summary> Adds the font subst.
        /// 
        /// </summary>
        /// <param name="ordering">the ordering
        /// </param>
        /// <param name="fontpath">the fontpath
        /// </param>
        /// <returns> true, if successful
        /// </returns>
        // <exception cref="PDFNetException">PDFNetException the PDFNet exception </exception>

        public static bool AddFontSubst(CharacterOrdering ordering, string fontpath)
        {
            bool result = false;
            UString str = new UString(fontpath);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetAddFontSubst(ordering, str.mp_impl, ref result));
            return result;
        }

        /// <summary> AddFontSubst functions can be used to create font substitutes
        /// that can override default PDFNet font selection algorithm.
        /// 
        /// These functions are useful in situations where referenced fonts
        /// are not present in the document and PDFNet font substitution
        /// algorithm is not producing desired results.
        /// 
        /// AddFontSubst(fontname, fontpath) maps the given font name (i.e. 'BaseFont'
        /// entry from the font dictionary) to a font file.
        /// 
        /// AddFontSubst(ordering, fontpath) maps the given character ordering (see
        /// Ordering entry in CIDSystemInfo dictionary; Section 5.6.2 in PDF Reference)
        /// to a font file. This method is less specific that the former variant of
        /// AddFontSubst, and can be used to override a range of missing fonts (or
        /// any missing font) with a predefined substitute.
        /// 
        /// The following is an example of using these functions to provide user
        /// defined font substitutes:
        /// 
        /// </summary>
        /// <param name="fontname">the fontname
        /// </param>
        /// <param name="fontpath">the fontpath
        /// </param>
        /// <returns> true, if successful
        /// </returns>		
        /// <example>
        /// <code>
        /// PDFNet.initialize();
        /// PDFNet.setResourcesPath("c:/myapp/resources");
        /// // Specify specific font mappings...
        /// PDFNet.addFontSubst("MinionPro-Regular", "c:/myfonts/MinionPro-Regular.otf");
        /// PDFNet.addFontSubst("Times-Roman", "c:/windows/fonts/times.ttf");
        /// PDFNet.addFontSubst("Times-Italic", "c:/windows/fonts/timesi.ttf");
        /// 
        /// // Specify more general font mappings...
        /// PDFNet.addFontSubst(PDFNet.e_Identity, "c:/myfonts/arialuni.ttf");  // Arial Unicode MS
        /// PDFNet.addFontSubst(PDFNet.e_Japan1, "c:/myfonts/KozMinProVI-Regular.otf");
        /// PDFNet.addFontSubst(PDFNet.e_Japan2, "c:/myfonts/KozMinProVI-Regular.otf");
        /// PDFNet.addFontSubst(PDFNet.e_Korea1, "c:/myfonts/AdobeSongStd-Light.otf");
        /// PDFNet.addFontSubst(PDFNet.e_CNS1, "c:/myfonts/AdobeMingStd-Light.otf");
        /// PDFNet.addFontSubst(PDFNet.e_GB1, "c:/myfonts/AdobeMyungjoStd-Medium.otf");
        /// //...
        /// PDFDoc doc = new PDFDoc("c:/my.pdf");
        /// //...
        /// </code>
        /// </example>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static bool AddFontSubst(string fontname, string fontpath)
        {
            bool result = false;
            UString str = new UString(fontpath);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetAddFontSubstFromName(fontname, str.mp_impl, ref result));
            return result;
        }

        /// <summary> Set the location of temporary folder.
        /// 
        /// </summary>
        /// <param name="path">the temp_path
        /// </param>	
        /// <remarks>This method is provided for applications that require tight control of
        /// the location where temporary files are created.</remarks>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void SetTempPath(string path)
        {
            UString str = new UString(path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetSetTempPath(str.mp_impl));
        }

        /// <summary> Set the location of persistent cache files.
        /// 
        /// </summary>
        /// <param name="path">path to persistent cache folder
        /// </param>	
        /// <remarks>This method is provided for applications that require tight control of
        /// the location where temporary files are created.</remarks>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static void SetPersistentCachePath(string path)
        {
            UString str = new UString(path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetSetPersistentCachePath(str.mp_impl));
        }

        /// <summary> Terminates PDFNet library.
        /// <c>Terminate()</c> is usually called once, when the process is terminated. 
        /// 
        /// </summary>
        /// <remarks>it is unsafe to call any other PDFNet API after you terminate
        /// the library.
        /// </remarks>
        //        public static void Terminate()
        //        {
        //            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetTerminate();
        //            if (ex != IntPtr.Zero)
        //            {
        //                throw new PDFNetException(ex);
        //            }
        //        }

        // Nested Types
        ///<summary>character ordering</summary>
        public enum CharacterOrdering
        {
            ///<summary>Generic/Unicode</summary>
            e_Identity,
            ///<summary>Japanese</summary>
            e_Japan1,
            ///<summary>Japanese</summary>
            e_Japan2,
            ///<summary>Chinese; Simplified</summary>
            e_GB1,
            ///<summary>Chinese; Traditional</summary>
            e_CNS1,
            ///<summary>Korean</summary>
            e_Korea1
        }

        ///<summary>color management system types</summary>
        public enum CMSType
        {
            ///<summary>Use LittleCMS (available on all supported platforms).</summary>
            e_lcms,
            ///<summary>Use Windows ICM2 (available only on Windows platforms).</summary>
            e_icm,
            ///<summary>No ICC color management.</summary>
            e_no_cms
        }
    }
}