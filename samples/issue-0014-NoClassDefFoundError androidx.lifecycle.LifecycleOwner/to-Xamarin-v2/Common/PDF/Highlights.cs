using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Highlights = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> Highlights is used to store the necessary information and perform certain 
    /// tasks in accordance with Adobe's Highlight standard, whose details can be
    /// found at:
    /// 
    /// http://partners.adobe.com/public/developer/en/pdf/HighlightFileFormat.pdf
    /// 
    /// In a nutshell, the Highlights class maintains a set of highlights.
    /// Each highlight contains three pieces of information: 
    /// 
    /// page: the number of the page this Highlight is on;
    /// position: the start position (text offset) of this Highlight;
    /// length: the length of this Highlight.
    /// 
    /// Possible use case scenarios for Highlights include:
    /// <list type="bullet">
    /// <item><description>
    /// Load a Highlight file (in XML format) and highlight the corresponding 
    /// texts in the viewer (e.g., if the viewer is implemented using PDFViewCtrl, 
    /// it can be achieved simply by calling PDFViewCtrl::SelectByHighlights() 
    /// method);
    /// </description></item>
    /// <item><description>
    /// Save the Highlight information (e.g., constructed by the TextSearch 
    /// class) to an XML file for external uses.
    /// </description></item>
    /// </list>
    /// Note: 
    /// <list type="bullet">
    /// <item><description>
    /// The Highlights class does not maintain the corresponding PDF document for
    /// its highlights. It is the user's responsibility to match them up.
    /// </description></item>
    /// <item><description>
    /// The Highlights class ensures that each highlight it maintains is 
    /// unique (no two highlights have the same page, position and length values).
    /// </description></item>
    /// <item><description>
    /// The current implementation of Highlights only supports the 'characters'
    /// encoding for 'units' as described in the format; the 'words' encoding is 
    /// not supported at this point.
    /// </description></item>
    /// </list>
    /// </summary>
    /// <remarks>For a sample code, please take a look at the TextSearchTest sample project.</remarks>
    public class Highlights : IDisposable
    {
        internal TRN_Highlights mp_highlights = IntPtr.Zero;

        internal Highlights(TRN_Highlights imp)
        {
            this.mp_highlights = imp;
        }
        internal IntPtr GetHandleInternal()
		{
			return mp_highlights;
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
            if (mp_highlights != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_HighlightsDestroy(mp_highlights));
                mp_highlights = IntPtr.Zero;
            }
        }

        /// <summary>Creates a default <c>Highlights</c> object</summary>
        public Highlights()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HighlightsCreate(ref mp_highlights));
        }
	    /// <summary>Creates a Hightlights from a given object</summary>
	    /// <param name="hlts">a given <c>Hightlights</c> object</param>
        public Highlights(Highlights hlts)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HighlightsCopyCtor(hlts.mp_highlights, ref mp_highlights));
        }
	    /// <summary> Releases all resources used by the Highlights </summary>
	    ~Highlights() 
        {
            Dispose(false);
        }

	    /// <summary> Add extra Highlight information into the class.
	    /// 
	    /// </summary>
	    /// <param name="hlts">the Highlights of which the Highlight information is to be
	    /// added.
	    /// </param>
        public void Add(Highlights hlts)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HighlightsAdd(mp_highlights, hlts.mp_highlights));
        }
	    /// <summary> Load the Highlight information from a file. Note that the
	    /// pre-existing Highlight information is discarded.
	    /// 
	    /// </summary>
        /// <param name="file_name">the name of the file to load from.
        /// </param>
        public void Load(String file_name)
        {
            UString str = new UString(file_name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HighlightsLoad(mp_highlights, str.mp_impl));
        }
	    /// <summary> Save the current Highlight information in the class to a file.
	    /// 
	    /// </summary>
        /// <param name="file_name">the name of the file to save to.
        /// </param>
        public void Save(String file_name)
        {
            UString str = new UString(file_name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_HighlightsSave(mp_highlights, str.mp_impl));
        }
        /// <summary> Clear the current Highlight information in the class.</summary>
        public void Clear()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HighlightsClear(mp_highlights));
        }
	    /// <summary> Rewind the internal pointer to the first highlight. 
	    /// 
	    /// </summary>
	    /// <param name="doc">the PDF document to which the highlights correspond.
	    /// 
	    /// </param>
        /// <remarks>  the PDF document can be a dummy document unless getCurrentQuads()
        /// is to be called.</remarks>
        public void Begin(PDFDoc doc)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HighlightsBegin(mp_highlights, doc.mp_doc));
        }
	    /// <summary> Query if there is any subsequent highlight after the current highlight.
	    /// 
	    /// </summary>
        /// <returns> true, if successful
        /// </returns>
        public Boolean HasNext()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_HighlightsHasNext(mp_highlights, ref result));
            return result;
        }
        /// <summary> Move the current highlight to the next highlight.</summary>
        public void Next()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_HighlightsNext(mp_highlights));
        }
	    /// <summary> Get the page number of the current highlight.
	    /// 
	    /// </summary>
        /// <returns> the current page number
        /// </returns>
        public Int32 GetCurrentPageNumber()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_HighlightsGetCurrentPageNumber(mp_highlights, ref result));
            return result;
        }
	    /// <summary> Get the corresponding quadrangles of the current highlight.
	    /// 
	    /// </summary>
	    /// <returns> the output quadrangles. Each quadrangle has eight doubles
	    /// (x1, y1), (x2, y2), (x3, y3), (x4, y4) denoting the four vertices
	    /// in counter-clockwise order.
	    /// </returns>
	    /// <remarks>  since a highlight may correspond to multiple quadrangles, e.g.,
	    /// when it crosses a line, the number of resulting quadrangles may be
	    /// larger than 1.</remarks>
        public double[] GetCurrentQuads()
        {
            int num = int.MinValue;
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_HighlightsGetCurrentQuads(mp_highlights, ref result, ref num));
            double[] arr = new double[num*8];
            System.Runtime.InteropServices.Marshal.Copy(result, arr, 0, num*8);
            return arr;
        }
    }
}
