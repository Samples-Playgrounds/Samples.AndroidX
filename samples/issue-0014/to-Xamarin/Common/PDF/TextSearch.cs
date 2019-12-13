using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_TextSearch = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> TextSearch searches through a PDF document for a user-given search pattern.
    /// The current implementation supports both verbatim search and the search
    /// using regular expressions, whose detailed syntax can be found at:
    /// 
    /// http://www.boost.org/doc/libs/release/libs/regex/doc/html/boost_regex/syntax/perl_syntax.html
    /// 
    /// TextSearch also provides users with several useful search modes and extra 
    /// information besides the found string that matches the pattern. TextSearch
    /// can either keep running until a matched string is found or be set to return
    /// periodically in order for the caller to perform any necessary updates 
    /// (e.g., UI updates). It is also worth mentioning that the search modes can be
    /// changed on the fly while searching through a document.
    /// 
    /// Possible use case scenarios for TextSearch include:
    /// <list type="bullet">
    /// <item><description>Guide users of a PDF viewer (e.g. implemented by PDFViewCtrl) to places
    /// where they are intersted in;</description></item>
    /// <item><description>Find interested PDF documents which contain certain patterns;</description></item>
    /// <item><description>Extract interested information (e.g., credit card numbers) from a set of files;</description></item>
    /// <item><description>Extract Highlight information (refer to the Highlights class for details) from
    /// files for external use.</description></item>        
    /// <item><description>Since hyphens ('-') are frequently used in PDF documents to concatenate the two
    /// broken pieces of a word at the end of a line, for example
    /// <c>
    /// "TextSearch is powerful for finding patterns in PDF files; yes, it is really pow-
    /// erful."
    /// </c>
    /// a search for "powerful" should return both instances. However, not all end-of-line
    /// hyphens are hyphens added to connect a broken word; some of them could be "real"
    /// hyphens. In addition, an input search pattern may also contain hyphens that complicate
    /// the situation. To tackle this problem, the following conventions are adopted:
    /// <list type="number">
    /// <item><description>When in the verbatim search mode and the pattern contains no hyphen, a matching
    /// string is returned if it is exactly the same or it contains end-of-line
    /// or start-of-line hyphens. For example, as mentioned above, a search for "powerful" 
    /// would return both instances.</description></item>
    /// <item><description>When in verbatim search mode and the pattern contains one or multiple hyphens, a 
    /// matching string is returned only if the string matches the pattern exactly. For 
    /// example, a search for "pow-erful" will only return the second instance, and a search
    /// for "power-ful" will return nothing.</description></item>
    /// <item><description>When searching using regular expressions, hyphens are not taken care implicitly.
    /// Users should take care of it themselves. For example, in order to find both the
    /// "powerful" instances, the input pattern can be "pow-{0,1}erful".</description></item>
    /// </list>
    /// </description></item>
    /// </list>	
    /// <example>
    /// For a full sample, please take a look at the TextSearch sample project.
    /// <code>
    ///	//... Initialize PDFNet ...
    /// PDFDoc doc = new PDFDoc(filein);
    /// doc.initSecurityHandler();
    /// int mode = TextSearch.e_whole_word | TextSearch.e_page_stop;
    /// UString pattern( "joHn sMiTh" );
    /// TextSearch txt_search = new TextSearch();
    ///
    /// //PDFDoc doesn't allow simultaneous access from different threads. If this
    /// //document could be used from other threads (e.g., the rendering thread inside
    /// //PDFView/PDFViewCtrl, if used), it is good practice to lock it.
    /// //Notice: don't forget to call doc.Unlock() to avoid deadlock.
    /// doc.Lock(); 
    /// txt_search.Begin( doc, pattern, mode, -1, -1 );
    /// while ( true )
    /// {
    ///		TextSearchResult result = txt_search.Run();
    ///		if ( result.getCode() == TextSearchResult.e_TextSearch_found )
    ///		{
    ///			System.out.println("found one instance: " + result.getResultStr());
    ///		}
    /// }
    /// else
    /// {
    ///		break;
    /// }
    ///
    /// //unlock the document to avoid deadlock.
    /// doc.UnLock();
    /// </code>
    /// </example>
    /// </summary>
    public class TextSearch : IDisposable
    {
        internal TRN_TextSearch mp_textsearch = IntPtr.Zero;
        internal TextSearch(TRN_TextSearch imp)
        {
            this.mp_textsearch = imp;
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
            if (mp_textsearch != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextSearchDestroy(mp_textsearch));
                mp_textsearch = IntPtr.Zero;
            }
        }
        /// <summary> Constructor and destructor.</summary>
        public TextSearch()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextSearchCreate(ref mp_textsearch));
        }
        /// <summary> Releases all resources used by the TextSearch </summary>
        ~TextSearch()
        {
            Dispose(false);
        }

	    /// <summary> Initialize for the search process. This should be called before starting the actual search.
	    /// with method run().
	    /// 
	    /// </summary>
	    /// <param name="doc">the PDF document to search in.
	    /// </param>
	    /// <param name="pattern">the pattern to search for. When regular expression is used, it contains
	    /// the expression, and in verbatim mode, it is the exact string to search for.
	    /// </param>
	    /// <param name="mode">the mode of the search process.
	    /// </param>
	    /// <param name="start_page">the start page of the page range to search in. -1 indicates the
	    /// range starts from the first page.
	    /// </param>
	    /// <param name="end_page">the end page of the page range to search in. -1 indicates the range
	    /// ends at the last page.
	    /// </param>
        /// <returns> true if the initialization has succeeded.
        /// </returns>
        public Boolean Begin(PDFDoc doc, String pattern, Int32 mode, Int32 start_page, Int32 end_page)
        {
            bool result = false;
            UString str = new UString(pattern);
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextSearchBegin(mp_textsearch, doc.mp_doc, str.mp_impl,System.Convert.ToUInt32(mode), start_page, end_page, ref result));
            return result;
        }
	    /// <summary> Search the document and returns upon the following circumstances:
	    /// <list type="bullet">
	    /// <item><description>Reached the end of the document</description></item>
	    /// <item><description>Reached the end of a page (if set to return by specifying mode 'e_page_stop' )</description></item>
	    /// <item><description>Found an instance matching the search pattern</description></item>
	    /// </list>
	    /// <remarks>
	    /// Note that this method should be called in a loop in ordre to find all matching instances;
	    /// in other words, the search is conducted in an incremental fashion.
	    /// </remarks>
	    /// </summary>
	    /// <param name="page_num">the number of the page the found instance is on.
	    /// </param>
	    /// <param name="result_str">the found string that matches the search pattern.
	    /// </param>
	    /// <param name="ambient_str">the ambient string of the found string (computed if 'e_ambient_string' is set).
	    /// </param>
	    /// <param name="hlts">the Highlights info associated with the found string (computed if 'e_highlight' is set).
	    /// </param>
        /// <returns>the code indicating the reason of the return. Note that only when the returned code is 'e_TextSearch_found', the resulting information is meaningful.
	    /// </returns>
        public ResultCode Run(ref int page_num, ref string result_str, ref string ambient_str, Highlights hlts)
        {
            ResultCode code = ResultCode.e_page;
            UString result = new UString(result_str);
            UString ambient = new UString(ambient_str);
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextSearchRun(mp_textsearch, ref page_num, result.mp_impl, ambient.mp_impl, hlts.mp_highlights, ref code));
            result_str = result.ConvToManagedStr();
            ambient_str = ambient.ConvToManagedStr();
            return code;            
        }
	    /// <summary> Sets the current search pattern. Note that it is not necessary to call this method since
	    /// the search pattern is already set when calling the begin() method. This method is provided
	    /// for users to change the search pattern while searching through a document.
	    /// 
	    /// </summary>
	    /// <param name="pattern">the search pattern to set.
	    /// </param>
        /// <returns> true if the setting has succeeded.
        /// </returns>
        public Boolean SetPattern(String pattern)
        {
            bool result = false;
            UString str = new UString(pattern);
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextSearchSetPattern(mp_textsearch, str.mp_impl, ref result));
            return result;
        }
	    /// <summary> Retrieve the current search mode.</summary>
        /// <returns> the current search mode.
        /// </returns>
        public int GetMode()
        {
            uint result = uint.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextSearchGetMode(mp_textsearch, ref result));
            return System.Convert.ToInt32(result);
        }
	    /// <summary> Set the current search mode. For example, the following code turns on the regular
	    /// expression:
	    /// 
	    /// TextSearch ts = new TextSearch();
	    /// ...
	    /// int mode = ts.getMode();
	    /// mode |= TextSearch.e_reg_expression;
	    /// ts.setMode(mode);
	    /// ...
	    /// 
	    /// </summary>
        /// <param name="mode">the search mode to set.
        /// </param>
        public void SetMode(int mode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextSearchSetMode(mp_textsearch,System.Convert.ToUInt32(mode)));
        }
	    /// <summary> Retrieve the number of the current page that is searched in.
	    /// If the returned value is -1, it indicates the search process has not been initialized
	    /// (e.g., begin() is not called yet); if the returned value is 0, it indicates the search
	    /// process has finished, and if the returned value is positive, it is a valid page number.
	    /// 
	    /// </summary>
        /// <returns> the current page number.
        /// </returns>
        public Int32 GetCurrentPage()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextSearchGetCurrentPage(mp_textsearch, ref result));
            return result;
        }
        // Nested Types
        /// <summary>The code indicating the reason when a search returns.</summary>
        public enum ResultCode
        {
            /// <summary>Did not find any results</summary>
            e_done = 0,
            /// <summary>finished searching a page.</summary>
            e_page = 1,
            /// <summary>found a matching instance.</summary>
            e_found = 2
        }

        /// <summary> Search modes that control how searching is conducted.</summary>
        public enum SearchMode
        {
            ///<summary>use regular expressions</summary>
            e_reg_expression = 0x0001,
            ///<summary>match case-sensitively</summary>
            e_case_sensitive = (Int32)e_reg_expression << 1,
            ///<summary>match the entire word</summary>
            e_whole_word = (Int32)e_case_sensitive << 1,
            ///<summary>search upward (from the end of the file and from the bottom of a page)</summary>
            e_search_up = (Int32)e_whole_word << 1,
            ///<summary>tells the search process to return when each page is finished; this is
            ///useful when a user needs Run() to return periodically so that certain
            ///things (e.g., UI) can be updated from time to time.</summary>
            e_page_stop = (Int32)e_search_up << 1,
            ///<summary>tells the search process to compute Highlight information.</summary>
            e_highlight = (Int32)e_page_stop << 1,
            ///<summary>tells the search process to compute the ambient string of the found pattern.
            ///This is useful if a user wants to examine or display what surrounds the
            ///found pattern.</summary>
            e_ambient_string = (Int32)e_highlight << 1
        }

    }
}