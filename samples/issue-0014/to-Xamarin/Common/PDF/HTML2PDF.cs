#if (__DESKTOP__)

using System;
using System.Runtime.InteropServices;

using pdftron.Common;
using pdftronprivate.trn;

using TRN_Exception = System.IntPtr;
using TRN_HTML2PDF = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary>
    /// <para>
    /// 'pdftron.PDF.HTML2PDF' is an optional PDFNet Add-On utility class that can be 
    /// used to convert HTML web pages into PDF documents by using an external
    /// module (html2pdf). </para>
    /// <para>The html2pdf modules can be downloaded from http: www.pdftron.com/pdfnet/downloads.html.</para>
    /// <para>
    /// Users can convert HTML pages to PDF using the following operations:
    /// - Simple one line static method to convert a single web page to PDF. 
    /// - Convert HTML pages from URL or string, plus optional table of contents, in user defined order. 
    /// - Optionally configure settings for proxy, images, java script, and more for each HTML page. 
    /// - Optionally configure the PDF output, including page size, margins, orientation, and more. 
    /// - Optionally add table of contents, including setting the depth and appearance.
    /// </para>
    /// <example>
    /// The following code converts a single webpage to pdf
    /// 
    /// <code>
    /// using System;
    /// using System.IO;
    /// using pdftron;
    /// using pdftron.Common;
    /// using pdftron.SDF;
    /// using pdftron.PDF;
    ///
    /// using (PDFDoc doc = new PDFDoc())
    /// {
    ///		if ( HTML2PDF.Convert(doc, "http://www.gutenberg.org/wiki/Main_Page") )
    ///			doc.Save(outputFile, SDFDoc.SaveOptions.e_linearized);
    /// }
    /// </code>
    /// 
    /// The following code demonstrates how to convert multiple web pages into one pdf,
    /// including any images and the background, but with lowered image quality to save space.
    /// 
    /// <code>
    /// using System;
    /// using System.IO;
    /// using pdftron;
    /// using pdftron.Common;
    /// using pdftron.SDF;
    /// using pdftron.PDF;
    ///
    /// using (PDFDoc doc = new PDFDoc())
    /// {
    ///		HTML2PDF converter = new HTML2PDF();
    ///		converter.SetImageQuality(25);
    ///
    ///		HTML2PDF.WebPageSettings settings = new HTML2PDF.WebPageSettings();
    ///		settings.SetPrintBackground(false);
    ///
    ///		converter.InsertFromURL("http://www.gutenberg.org/wiki/Main_Page", settings);
    ///
    ///		if ( HTML2PDF.Convert(doc, "http://en.wikipedia.org/wiki/Canada") )
    ///			doc.Save(outputFile, SDFDoc.SaveOptions.e_linearized);
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class HTML2PDF : IDisposable
    {
        internal TRN_HTML2PDF mp_html2pdf = IntPtr.Zero;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                }

                Destroy();

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion



        /// <summary> Convert the HTML document at url and append the results
        /// to doc.
        /// 
        /// </summary>
        /// <remarks> html2pdf module must be located in the working directory, or
        /// with the PDFNet library.
        /// </remarks>
        /// 
        /// <returns> true if successful, otherwise false. Use 
        /// GetHttpErrorCode for possible HTTP errors.
        /// </returns>
        /// <param name="doc">Target PDF to which converted HTML pages will
        /// be appended to.
        /// </param>
        /// <param name="url">HTML page, or relative path to local HTML page,
        /// that will be converted to PDF format.
        /// </param>
        /// <remarks> If you wish to convert more than one web page you need to use
        /// an instance of HTML2PDF.
        /// </remarks>
        public static bool Convert(PDFDoc doc, String url)
        {
            bool result = false;
            UString uString = new UString(url);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFStaticConvert(doc.mp_doc, uString.mp_impl, ref result));
            return result;
        }

        /// <summary> Convert the HTML document at url and append the results
        /// to doc.</summary>
        ///
        /// <remarks> html2pdf module must be located in the working directory, or
        /// with the PDFNet library.
        /// </remarks>
        /// <returns> true if successful, otherwise false. Use 
        /// GetHttpErrorCode for possible HTTP errors.
        /// </returns>
        /// <param name="doc">Target PDF to which converted HTML pages will
        /// be appended to.
        /// </param>
        /// <param name="url">HTML page, or relative path to local HTML page,
        /// that will be converted to PDF format.
        /// </param>
        /// <param name="settings">Modify how the web page is loaded and
        /// converted.
        /// </param>
        /// <remarks> If you wish to convert more than one web page you need to use
        /// an instance of HTML2PDF.
        /// </remarks>
        public static bool Convert(PDFDoc doc, String url, WebPageSettings settings)
        {
            bool result = false;
            UString uString = new UString(url);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFStaticConvert2(doc.mp_doc, uString.mp_impl, settings.mp_impl, ref result));
            return result;
        }

        /// <summary> Add a web page to be converted. A single URL typically 
        /// results in many PDF pages.
        /// <param name="url">HTML page, or relative path to local HTML page
        /// </param>
        /// </summary>
        public void InsertFromURL(String url)
        {
            UString uString = new UString(url);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFInsertFromUrl(mp_html2pdf, uString.mp_impl));
        }

        /// <summary> Add a web page to be converted. A single URL typically 
        /// results in many PDF pages.
        /// 
        /// </summary>
        /// <param name="url">HTML page, or relative path to local HTML page.
        /// </param>
        /// <param name="settings">How the web page should be loaded and converted
        /// </param>
        public void InsertFromURL(String url, WebPageSettings settings)
        {
            UString uString = new UString(url);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFInsertFromUrl2(mp_html2pdf, uString.mp_impl, settings.mp_impl));
        }

        /// <summary> Convert HTML encoded in string.
        /// 
        /// </summary>
        /// <param name="html">String containing HTML code.
        /// </param>
        public void InsertFromHtmlString(String html)
        {
            UString uString = new UString(html);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFInsertFromHtmlString(mp_html2pdf, uString.mp_impl));

        }

        /// <summary> Convert HTML encoded in string.
        /// 
        /// </summary>
        /// <param name="html">String containing HTML code.
        /// </param>
        /// <param name="settings">How the HTML content described in html is loaded.
        /// </param>
        public void InsertFromHtmlString(String html, WebPageSettings settings)
        {
            UString uString = new UString(html);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFInsertFromHtmlString2(mp_html2pdf, uString.mp_impl, settings.mp_impl));
        }

        /// <summary> Add a table of contents to the produced PDF.
        ///
        /// </summary>
        public void InsertTOC()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFInsertTOC(mp_html2pdf));
        }

        /// <summary> Add a table of contents to the produced PDF.
        /// 
        /// </summary>
        /// <param name="settings">Settings for the table of contents.
        /// </param>
        public void InsertTOC(TOCSettings settings)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFInsertTOC2(mp_html2pdf, settings.mp_impl));
        }

        /// <summary> Convert HTML documents and append the results
        /// to doc.
        /// 
        /// </summary>
        /// <remarks> html2pdf module must be located in the working directory, or
        /// with the PDFNet library.
        /// </remarks>
        /// <returns> true if successful, otherwise false. Use 
        /// GetHttpErrorCode for possible HTTP errors.
        /// </returns>
        /// <param name="doc">Target PDF to which converted HTML pages will
        /// be appended to.
        /// </param>
        /// <remarks> Use InsertFromURL and InsertFromHtmlString to
        /// add HTML documents to be converted.
        /// </remarks>
        public bool Convert(PDFDoc doc)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFConvert(mp_html2pdf, doc.mp_doc, ref result));
            return result;
        }

        /// <summary> Return the largest HTTP error code encountered during conversion
        /// 
        /// </summary>
        /// <returns> the largest HTTP code greater then or equal to 300 encountered during loading
        /// of any of the supplied objects, if no such error code is found 0 is returned.
        /// </returns>
        /// <remarks> This function will only return a useful result after Convert has been called.
        /// </remarks>
        public int GetHTTPErrorCode()
        {
            int result = Int32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFGetHttpErrorCode(mp_html2pdf, ref result));
            return result;
        }

        /// <summary> Display HTML to PDF conversion progress, warnings, and errors, to stdout.
        ///
        /// </summary>
        /// <param name="quiet">If false, progress information is sent to stdout during conversion.
        /// </param>
        /// <remarks> You can get the final results using GetLog.
        /// </remarks>
        public void SetQuiet(bool quiet)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFSetQuiet(mp_html2pdf, quiet));
        }

        /// <summary> Get results of conversion, including errors and warnings, in human readable form.
        ///
        /// </summary>
        /// <returns> String containing results of conversion.
        /// </returns>
        public String GetLog()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFGetLog(mp_html2pdf, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

        /// <summary> Set the first location that PDFNet will look for the html2pdf module.
        ///
        /// </summary>
        /// <param name="path">A folder or file path. If non-empty, PDFNet will only
        /// look in path for the html2pdf module, otherwise it will search in
        /// the default locations for the module.
        /// </param>
        public static void SetModulePath(String path)
        {
            UString uString = new UString(path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFSetModulePath(uString.mp_impl));
        }

        /// <summary> Default constructor.
        ///
        /// </summary>
        public HTML2PDF()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFCreate(ref mp_html2pdf));
        }

        public void Destroy()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFDestroy(mp_html2pdf));
        }

        /// <summary> Releases all resources used by the HTML2PDF </summary>
        ~HTML2PDF()
        {
            Dispose(false);
        }

        /// <summary> Set paper size of output PDF
        /// 
        /// </summary>
        /// <param name="size">Paper size to use for produced PDF.
        /// </param>
        public void SetPaperSize(PrinterMode.PaperSize size)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFSetPaperSize(mp_html2pdf, size));
        }

        /// <summary> Manually set the paper dimensions of the produced PDF.
        /// 
        /// </summary>
        /// <param name="width">Width of the page, e.g. "4cm".
        /// </param>
        /// <param name="height">Height of the page, eg. "12in".
        /// </param>
        /// <remarks>  Supported units are mm, cm, m, in, pica(pc), pixel(px) and point(pt).
        /// </remarks>
        public void SetPaperSize(String width, String height)
        {
            UString uStringWidth = new UString(width);
            UString uStringHeight = new UString(height);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFSetPaperSize2(mp_html2pdf, uStringWidth.mp_impl, uStringHeight.mp_impl));
        }

        /// <summary> Set page orientation for output PDF.
        /// 
        /// </summary>
        /// <param name="enable">If true generated PDF pages will be orientated to
        /// landscape, otherwise orientation will be portrait.
        /// </param>
        public void SetLandscape(bool enable)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFSetLandscape(mp_html2pdf, enable));
        }

        /// <summary> Change the DPI explicitly for the output PDF.
        /// 
        /// </summary>
        /// <param name="dpi">Dots per inch, e.g. 80.
        /// <remarks> This has no effect on X11 based systems.
        /// Results also depend on SetSmartShrinking.
        /// </remarks>
        /// </param>
        public void SetDPI(int dpi)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFSetDPI(mp_html2pdf, dpi));
        }

        /// <summary> Add bookmarks to the PDF.
        /// 
        /// </summary>
        /// <param name="enable">If true bookmarks will be generated for the
        /// produced PDF.
        /// </param>
        /// <param name="depth">Maximum depth of the outline (e.g. 4).
        /// </param>
        public void SetOutline(bool enable, int depth)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFSetOutline(mp_html2pdf, enable, depth));
        }

        /// <summary> Save outline to a xml file.
        /// 
        /// </summary>
        /// <param name="xml_file">Path of where xml data representing outline 
        /// of produced PDF should be saved to.
        /// </param>
        public void DumpOutline(String xml_file)
        {
            UString uString = new UString(xml_file);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFDumpOutline(mp_html2pdf, uString.mp_impl));
        }

        /// <summary> Use lossless compression to create PDF.
        /// 
        /// </summary>
        /// <param name="enable">If true loss less compression will be used to
        /// create PDF.
        /// </param>
        public void SetPDFCompression(bool enable)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFSetPDFCompression(mp_html2pdf, enable));
        }

        /// <summary> Set margins of generated PDF.
        /// 
        /// </summary>
        /// <param name="top">Size of the top margin, e.g. "2cm". 
        /// </param>
        /// <param name="bottom">Size of the bottom margin, e.g. "2cm".
        /// </param>
        /// <param name="left">Size of the left margin, e.g. "2cm".
        /// </param>
        /// <param name="right">Size of the right margin, e.g. "2cm".
        /// </param>
        /// <remarks>  Supported units are mm, cm, m, in, pica(pc), pixel(px) and point(pt).
        /// </remarks>
        public void SetMargins(String top, String bottom, String left, String right)
        {
            UString uStringTop = new UString(top);
            UString uStringBottom = new UString(bottom);
            UString uStringLeft = new UString(left);
            UString uStringRight = new UString(right);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFSetMargins(mp_html2pdf, uStringTop.mp_impl, uStringBottom.mp_impl, uStringLeft.mp_impl, uStringRight.mp_impl));
        }

        /// <summary> Maximum DPI to use for images in the generated PDF.
        /// 
        /// </summary>
        /// <param name="dpi">Maximum dpi of images in produced PDF, e.g. 80.
        /// </param>
        public void SetImageDPI(int dpi)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFSetImageDPI(mp_html2pdf, dpi));
        }

        /// <summary> JPEG compression factor to use when generating PDF.
        /// 
        /// </summary>
        /// <param name="quality">Compression factor, e.g. 92.
        /// </param>
        public void SetImageQuality(int quality)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFSetImageQuality(mp_html2pdf, quality));
        }

        /// <summary> Path of file used for loading and storing cookies.
        /// 
        /// </summary>
        /// <param name="path">Path to file used for loading and storing cookies.
        /// </param>
        public void SetCookieJar(String path)
        {
            UString uString = new UString(path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDFSetCookieJar(mp_html2pdf, uString.mp_impl));
        }

        // Methods
        internal HTML2PDF(TRN_HTML2PDF imp)
        {
            this.mp_html2pdf = imp;
        }

        /// <summary> Proxy settings to be used when loading content from web pages.
        /// 
        /// </summary>
        /// <remarks> These Proxy settings will only be used if type is not e_default.
        /// </remarks>
        public class Proxy : IDisposable
        {
            internal IntPtr mp_impl = IntPtr.Zero;

            public enum Type
            {
                /// <summary> use whatever type of proxy the html2pdf library decides on </summary>
                e_default,
                /// <summary> explicitly sets that no proxy is to be used </summary>
                e_none,
                /// <summary> use http proxy </summary>
                e_http,
                /// <summary> use socks5 proxy </summary>
                e_socks5
            };
            /// <summary> Default constructor
            ///
            /// </summary>
            public Proxy()
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_ProxyCreate(ref mp_impl));
            }

            /// <summary> Releases all resources used by the Proxy </summary>
            ~Proxy()
            {
                Dispose(false);
            }

            public void Destroy()
            {
                if (mp_impl != IntPtr.Zero)
                {
                    PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_ProxyDestroy(mp_impl));
                    mp_impl = IntPtr.Zero;
                }
            }

            /// <summary> Set the type of proxy to use.
            /// 
            /// </summary>
            /// <param name="type">If e_default, use whatever the html2pdf library decides
            /// on. If e_none, explicitly sets that no proxy is to be used. If e_http
            /// or e_socks5 then the corresponding proxy protocol is used.
            /// </param>
            public void SetType(Type type)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_ProxySetType(mp_impl, type));
            }

            /// <summary> Set the proxy host to use.
            /// 
            /// </summary>
            /// <param name="host">String defining the host name, e.g. "myserver" or "www.xxx.yyy.zzz"
            /// </param>
            public void SetHost(String host)
            {
                UString uString = new UString(host);
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_ProxySetHost(mp_impl, uString.mp_impl));
            }

            /// <summary> Set the port number to use
            /// 
            /// </summary>
            /// <param name="port">A valid port number, e.g. 3128.
            /// </param>
            public void SetPort(int port)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_ProxySetPort(mp_impl, port));
            }

            /// <summary> Set the username to use when logging into the proxy
            /// 
            /// </summary>
            /// <param name="username">The login name, e.g. "elbarto".
            /// </param>
            public void SetUsername(String username)
            {
                UString uString = new UString(username);
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_ProxySetUsername(mp_impl, uString.mp_impl));
            }

            /// <summary> Set the password to use when logging into the proxy with username
            /// 
            /// </summary>
            /// <param name="password">The password to use, e.g. "bart".
            /// </param>
            public void SetPassword(String password)
            {
                UString uString = new UString(password);
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_ProxySetPassword(mp_impl, uString.mp_impl));
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // dispose managed state (managed objects).
                    }

                    Destroy();

                    disposedValue = true;
                }
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion

        }

        /// <summary> Settings that control how a web page is opened and converted to PDF.
        ///
        /// </summary>
        public class WebPageSettings : IDisposable
        {
            internal IntPtr mp_impl = IntPtr.Zero;
            /// <summary> How to handle objects that failed to load. </summary>
            public enum ErrorHandling
            {
                /// <summary>Abort the conversion process.</summary>
                e_abort,
                /// <summary>Do not add the object to the final output.</summary>
                e_skip,
                /// <summary>Try to add the object to the final output.</summary>
                e_ignore
            };
            /// <summary> Default constructor
            ///
            /// </summary>
            public WebPageSettings()
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsCreate(ref mp_impl));
            }

            public void Destroy()
            {
                if (mp_impl != IntPtr.Zero)
                {
                    PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsDestroy(mp_impl));
                    mp_impl = IntPtr.Zero;
                }
            }

            /// <summary> Releases all resources used by the WebPageSettings </summary>
            ~WebPageSettings()
            {
                Dispose(false);
            }

            /// <summary> Print the background of this web page.
            /// 
            /// </summary>
            /// <param name="background">If true background is printed.
            /// </param>
            public void SetPrintBackground(bool background)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetPrintBackground(mp_impl, background));
            }

            /// <summary> Print the images of this web page.
            /// 
            /// </summary>
            /// <param name="load">If true images are printed.
            /// </param>
            public void SetLoadImages(bool load)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetLoadImages(mp_impl, load));
            }

            /// <summary> Allow javascript from this web page to be run.
            /// 
            /// </summary>
            /// <param name="enable">If true javascript's are allowed.
            /// </param>
            public void SetAllowJavaScript(bool enable)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetAllowJavaScript(mp_impl, enable));
            }

            /// <summary> Allow intelligent shrinking to fit more content per page.
            /// 
            /// </summary>
            /// <param name="enable">If true intelligent shrinking is enabled and
            /// the pixel/dpi ratio is non constant.
            /// </param>
            public void SetSmartShrinking(bool enable)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetSmartShrinking(mp_impl, enable));
            }

            /// <summary> Set the smallest font size allowed, e.g 9.
            /// 
            /// </summary>
            /// <param name="size">No fonts will appear smaller than this.
            /// </param>
            public void SetMinimumFontSize(int size)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetMinimumFontSize(mp_impl, size));
            }

            /// <summary> Default encoding to be used when not specified by the web page.
            /// 
            /// </summary>
            /// <param name="encoding">Default encoding, e.g. utf-8 or iso-8859-1.
            ///  </param>
            /// <remarks> Supported encodings are
            /// Apple Roman, Big5, Big5-HKSCS, CP949, EUC-JP, EUC-KR, GB18030-0, IBM 850, 
            /// IBM 866, IBM 874, ISO 2022-JP, ISO 8859-1 to 10, ISO 8859-13 to 16, 
            /// Iscii-Bng, Dev, Gjr, Knd, Mlm, Ori, Pnj, Tlg, Tml, JIS X 0201, JIS X 0208, 
            /// KOI8-R, KOI8-U, MuleLao-1, ROMAN8, Shift-JIS, TIS-620, TSCII, 
            /// UTF-8, UTF-16, UTF-16BE, UTF-16LE, UTF-32, UTF-32BE, UTF-32LE, 
            /// Windows-1250 to 1258, WINSAMI2.
            /// </remarks>
            public void SetDefaultEncoding(String encoding)
            {
                UString uString = new UString(encoding);
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetDefaultEncoding(mp_impl, uString.mp_impl));
            }

            /// <summary> Url or path to user specified style sheet.
            /// </summary>
            /// <param name="url">URL or file path to user style sheet to be used
            /// with this web page.
            /// </param>
            public void SetUserStyleSheet(String url)
            {
                UString uString = new UString(url);
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetUserStyleSheet(mp_impl, uString.mp_impl));
            }

            /// <summary> Allow Netscape and flash plugins from this web page to
            /// be run. Enabling will have limited success.
            /// </summary>
            /// <param name="enable">If true Netscape and flash plugins will be run.
            /// </param>
            public void SetAllowPlugins(bool enable)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetAllowPlugins(mp_impl, enable));
            }

            /// <summary> Controls how content will be printed from this web page.
            /// </summary>
            /// <param name="print">If true the print media type will be used, otherwise
            /// the screen media type will be used to print content.
            /// </param>
            public void SetPrintMediaType(bool print)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetPrintMediaType(mp_impl, print));
            }

            /// <summary> Add sections from this web page to the outline and
            /// table of contents.
            /// 
            /// </summary>
            /// <param name="include">If true PDF pages created from this web
            /// page will show up in the outline, and table of contents,
            /// otherwise, produced PDF pages will be excluded.
            /// </param>
            public void SetIncludeInOutline(bool include)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetIncludeInOutline(mp_impl, include));
            }

            /// <summary> Username to use when logging into the website.
            /// 
            /// </summary>
            /// <param name="username">The login name to use with the server, e.g. "bart".
            /// </param>
            public void SetUsername(String username)
            {
                UString uString = new UString(username);
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetUsername(mp_impl, uString.mp_impl));
            }

            /// <summary> Password to use when logging into the website.
            /// 
            /// </summary>
            /// <param name="password">The login password to use with the server, e.g. "elbarto".
            /// </param>
            public void SetPassword(String password)
            {
                UString uString = new UString(password);
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetPassword(mp_impl, uString.mp_impl));
            }

            /// <summary> Amount of time to wait for a web page to start printing after
            /// it's completed loading. Converter will wait a maximum of msec milliseconds
            /// for javascript to call window.print().
            /// 
            /// </summary>
            /// <param name="msec">Maximum wait time in milliseconds, e.g. 1200.
            /// </param>
            public void SetJavaScriptDelay(int msec)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetJavaScriptDelay(mp_impl, msec));
            }

            /// <summary> Zoom factor to use when loading object.
            /// 
            /// </summary>
            /// <param name="zoom">How much to magnify the web content by, e.g. 2.2.
            /// </param>
            public void SetZoom(double zoom)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetZoom(mp_impl, zoom));
            }

            /// <summary> Allow local and piped files access to other local files.
            /// 
            /// </summary>
            /// <param name="block">If true local files will be inaccessible.
            /// </param>
            public void SetBlockLocalFileAccess(bool block)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetBlockLocalFileAccess(mp_impl, block));
            }

            /// <summary> Stop slow running javascript's.
            /// 
            /// </summary>
            /// <param name="stop">If true, slow running javascript's will be stopped.
            /// </param>
            public void SetStopSlowScripts(bool stop)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetStopSlowScripts(mp_impl, stop));
            }

            /// <summary> Forward javascript warnings and errors to output.
            /// 
            /// </summary>
            /// <param name="forward">If true javascript errors and warnings will be forwarded
            /// to stdout and the log.
            /// </param>
            public void SetDebugJavaScriptOutput(bool forward)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetDebugJavaScriptOutput(mp_impl, forward));
            }

            /// <summary> How to handle objects that failed to load.
            /// 
            /// </summary>
            /// <param name="type">If e_abort then conversion process is aborted, if
            /// e_skip then the converter will not add this web page to the PDF
            /// output, and if e_skip then the converter will try to add this
            /// web page to the PDF output.
            /// </param>
            public void SetLoadErrorHandling(ErrorHandling type)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetLoadErrorHandling(mp_impl, type));
            }

            /// <summary> Convert external links in HTML document to external
            /// PDF links.
            /// 
            /// </summary>
            /// <param name="convert">If true PDF pages produced from this web page
            /// can have external links.
            /// </param>
            public void SetExternalLinks(bool convert)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetExternalLinks(mp_impl, convert));
            }

            /// <summary> Convert internal links in HTML document into PDF references.
            /// 
            /// </summary>
            /// <param name="convert">If true PDF pages produced from this web page
            /// will have links to other PDF pages.
            /// </param>
            public void SetInternalLinks(bool convert)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetInternalLinks(mp_impl, convert));
            }

            /// <summary> Turn HTML forms into PDF forms.
            /// 
            /// </summary>
            /// <param name="forms">If true PDF pages produced from this web page
            /// will have PDF forms for any HTML forms the web page has.
            /// </param>
            public void SetProduceForms(bool forms)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetProduceForms(mp_impl, forms));
            }

            /// <summary> Use this proxy to load content from this web page.
            /// 
            /// </summary>
            /// <param name="proxy">Contains settings for proxy
            /// </param>
            public void SetProxy(Proxy proxy)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_WebPageSettingsSetProxy(mp_impl, proxy.mp_impl));
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // dispose managed state (managed objects).
                    }

                    Destroy();

                    disposedValue = true;
                }
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            #endregion

        }

        public class TOCSettings : IDisposable
        {
            internal IntPtr mp_impl = IntPtr.Zero;
            /// <summary> Default table of contents settings.
            ///
            /// </summary>
            public TOCSettings()
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_TOCSettingsCreate(ref mp_impl));
            }

            public void Destroy()
            {
                if (mp_impl != IntPtr.Zero)
                {
                    PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_TOCSettingsDestroy(mp_impl));
                    mp_impl = IntPtr.Zero;
                }
            }

            /// <summary> Releases all resources used by the TOCSettings </summary>
            ~TOCSettings()
            {
                Dispose(false);
            }

            /// <summary> Use a dotted line when creating TOC.
            /// 
            /// </summary>
            /// <param name="enable">Table of contents will use dotted lines.
            /// </param>
            public void SetDottedLines(bool enable)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_TOCSettingsSetDottedLines(mp_impl, enable));
            }

            /// <summary> Create links from TOC to actual content.
            /// 
            /// </summary>
            /// <param name="enable">Entries in table of contents will
            /// link to section in the PDF.
            /// </param>
            public void SetLinks(bool enable)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_TOCSettingsSetLinks(mp_impl, enable));
            }

            /// <summary> Caption text to be used with TOC.
            /// 
            /// </summary>
            /// <param name="caption">Text that will appear with the table of contents.
            /// </param>
            public void SetCaptionText(String caption)
            {
                UString uString = new UString(caption);
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_TOCSettingsSetCaptionText(mp_impl, uString.mp_impl));
            }

            /// <summary> Indentation used for every TOC level...
            /// 
            /// </summary>
            /// <param name="indentation">How much to indent each level, e.g. "2"
            /// </param>
            public void SetLevelIndentation(int indentation)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_TOCSettingsSetLevelIndentation(mp_impl, indentation));
            }

            /// <summary> How much to shrink font for every level, e.g. 0.8
            /// 
            /// </summary>
            /// <param name="shrink">Rate at which lower level entries will appear smaller
            /// </param>
            public void SetTextSizeShrink(double shrink)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_TOCSettingsSetTextSizeShrink(mp_impl, shrink));
            }

            /// <summary> xsl style sheet used to convert outline XML into a
            /// table of content.
            /// 
            /// </summary>
            /// <param name="style_sheet">Path to xsl style sheet to be used to generate
            /// this table of contents.
            /// </param>
            public void SetXsl(String style_sheet)
            {
                UString uString = new UString(style_sheet);
                PDFNetException.REX(PDFNetPINVOKE.TRN_HTML2PDF_TOCSettingsSetXsl(mp_impl, uString.mp_impl));
            }

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // dispose managed state (managed objects).
                    }

                    Destroy();

                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~TOCSettings() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion

        }
    }
}

#endif