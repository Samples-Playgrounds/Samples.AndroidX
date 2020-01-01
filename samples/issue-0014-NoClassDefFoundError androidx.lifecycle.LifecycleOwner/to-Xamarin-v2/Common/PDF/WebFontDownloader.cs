using System;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;


using TRN_PDFDoc = System.IntPtr;

namespace pdftron.PDF
{
	/// <summary>
	/// static interface to control the behaviour of PDFNet web font downloading
	/// </summary>
	public static class WebFontDownloader
	{


		/// <summary>
        /// Find out whether the web font downloader is available in version of PDFNet.
        /// </summary>
        /// <returns>returns true if web font downloading can be done</returns>
		public static bool IsAvailable()
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_WebFontDownloaderIsAvailable(ref result));
			return result;
		}

		/// <summary>
        /// Allow PDFNet to access the network to download missing fonts when possible.
        /// </summary>
		public static void EnableDownloads()
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_WebFontDownloaderEnableDownloads());
		}

		/// <summary>
        /// Prevent PDFNet from accessing the network to download missing fonts. It may still use previously downloaded fonts (which are cached on disk).
        /// </summary>
		public static void DisableDownloads()
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_WebFontDownloaderDisableDownloads());
		}

		/// <summary>
        /// download and cache a base collection of fonts in a background thread. Will not do anything if downloading is currently disabled.
        /// </summary>
		public static void PreCacheAsync()
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_WebFontDownloaderPreCacheAsync());
		}

		/// <summary>
        /// clear any pre-cached font files residing in persistent storage.
        /// </summary>
		public static void ClearCache()
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_WebFontDownloaderClearCache());
		}

		/// <summary>
        /// Set the root path into which the web font downloader will make requests.
        /// </summary>
        /// <param name="url">The root path into which the web font downloader will make requests</param>
		public static void SetCustomWebFontURL(string url)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_WebFontDownloaderSetCustomWebFontURL(UString.ConvertToUString(url).mp_impl));
		}
	};
}
