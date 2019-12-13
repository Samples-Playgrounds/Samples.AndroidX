using System;
using System.Collections.Generic;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> 
    /// <para>PDF page labels can be used to describe a page. This is used to 
    /// allow for non-sequential page numbering or the addition of arbitrary 
    /// labels for a page (such as the inclusion of Roman numerals at the 
    /// beginning of a book). PDFNet PageLabel object can be used to specify 
    /// the numbering style to use (for example, upper- or lower-case Roman, 
    /// decimal, and so forth), the starting number for the first page,
    /// and an arbitrary prefix to be pre-appended to each number (for 
    /// example, "A-" to generate "A-1", "A-2", "A-3", and so forth.)
    /// </para>
    /// <para>
    /// <c>PageLabel</c> corresponds to the PDF Page Label object (Section 8.3.1, 
    /// 'Page Labels' in the PDF Reference Manual.
    /// </para>
    /// <para>
    /// Each page in a PDF document is identified by an integer page index 
    /// that expresses the page’s relative position within the document. 
    /// In addition, a document may optionally define page labels to identify 
    /// each page visually on the screen or in print. Page labels and page 
    /// indices need not coincide: the indices are fixed, running consecutively 
    /// through the document starting from 1 for the first page, but the 
    /// labels can be specified in any way that is appropriate for the particular
    /// document. For example, if the document begins with 12 pages of front 
    /// matter numbered in roman numerals and the remainder of the document is 
    /// numbered in Arabic, the first page would have a page index of 1 and a 
    /// page label of i, the twelfth page would have index 12 and label xii, 
    /// and the thirteenth page would have index 13 and label 1.
    /// </para>
    /// <para>
    /// For purposes of page labeling, a document can be divided into labeling 
    /// ranges, each of which is a series of consecutive pages using the same 
    /// numbering system. Pages within a range are numbered sequentially in 
    /// ascending order. A page's label consists of a numeric portion based 
    /// on its position within its labeling range, optionally preceded by a 
    /// label prefix denoting the range itself. For example, the pages in an 
    /// appendix might be labeled with decimal numeric portions prefixed with 
    /// the string "A-" and the resulting page labels would be "A-1", "A-2", 
    /// </para>	
    /// 
    /// There is no default numbering style; if no 'S' (Style) entry is present, 
    /// page labels consist solely of a label prefix with no numeric portion. 
    /// For example, if the 'P' entry (Prefix) specifies the label prefix 
    /// "Appendix", each page is simply labeled "Appendix" with no page number. 
    /// If the 'P' entry is also missing or empty, the page label is an empty 
    /// string.
    /// 
    ///	<example>
    /// Sample code (See PableLabelsTest sample project for examples):	
    ///   
    /// Create a page labeling scheme that starts with the first page in 
    /// the document (page 1) and is using uppercase roman numbering 
    /// style. 
    ///	
    /// <code> doc.SetPageLabel(1, PageLabel::Create(doc, PageLabel::e_roman_uppercase, "My Prefix ", 1)); </code>
    /// 
    /// Create a page labeling scheme that starts with the fourth page in 
    /// the document and is using decimal arabic numbering style. 
    /// Also the numeric portion of the first label should start with number 
    /// 4 (otherwise the first label would be "My Prefix 1"). 
    /// <code>
    /// PageLabel L2 = PageLabel::Create(doc, PageLabel::e_decimal, "My Prefix ", 4);
    /// doc.SetPageLabel(4, L2);
    ///	</code>
    /// 
    /// Create a page labeling scheme that starts with the seventh page in 
    /// the document and is using alphabetic numbering style. The numeric 
    /// portion of the first label should start with number 1. 
    ///	<code>
    /// PageLabel L3 = PageLabel::Create(doc, PageLabel::e_alphabetic_uppercase, "My Prefix ", 1);
    /// doc.SetPageLabel(7, L3);
    ///	</code>
    /// 
    /// Read page labels from an existing PDF document.
    /// <code>
    /// PageLabel label = new PageLabel();
    /// for (int i=1; i&lt;=doc.GetPageCount(); ++i) {
    /// label = doc.GetPageLabel(i);
    /// if (label.IsValid()) {
    /// string title = label.GetLabelTitle(i);
    /// }
    /// </code>
    /// </example>	
    /// </summary>
    public class PageLabel : IDisposable
    {
        internal BasicTypes.TRN_PageLabel mp_imp;
        internal Object m_ref;

        /// <summary> Releases all resources used by the PageLabel </summary>
        ~PageLabel()
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
            if (mp_imp.mp_obj != IntPtr.Zero)
            {
                mp_imp = new BasicTypes.TRN_PageLabel();
                mp_imp.mp_obj = IntPtr.Zero;
            }
        }

        // Methods
        internal PageLabel(BasicTypes.TRN_PageLabel imp, Object reference)
        {
            this.mp_imp = imp;
            this.m_ref = reference;
        }
        /// <summary> Creates a new PageLabel.
	    /// 
	    /// </summary>
	    /// <param name="doc">A document to which the page label is added.
	    /// </param>
	    /// <param name="style">The numbering style for the label.
	    /// </param>
	    /// <param name="prefix">page label prefix
	    /// </param>
	    /// <param name="start_at">start at position
	    /// </param>
	    /// <returns> newly created PageLabel object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static PageLabel Create(SDFDoc doc, Style style, String prefix, int start_at)
        {
            BasicTypes.TRN_PageLabel result = new BasicTypes.TRN_PageLabel();
            UString str = new UString(prefix);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelCreate(doc.mp_doc, style, str.mp_impl, start_at, ref result));
            return new PageLabel(result, doc);
        }

        private void init()
        {
            this.mp_imp.mp_obj = IntPtr.Zero;
            this.mp_imp.m_first_page = -1;
            this.mp_imp.m_last_page = -1;
        }

        public PageLabel() { init(); }

	    /// <summary> Instantiates a new page label.
	    /// 
	    /// </summary>
	    /// <param name="obj">the l
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PageLabel(Obj obj)
        {
            init();
            TRN_Obj l = IntPtr.Zero;
            int first_page = -1;
            int last_page = -1;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelCreateFromObj(l, first_page, last_page, ref this.mp_imp));
        }

        public PageLabel op_Assign(PageLabel r)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelAssign(ref this.mp_imp, ref r.mp_imp));
            return this;
        }

        public bool op_Compare(PageLabel r)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelCompare(ref this.mp_imp, ref r.mp_imp, ref result));
            return result;
        }

	    /// <summary> Checks if is valid.
	    /// 
	    /// </summary>
	    /// <returns> whether this is a valid (non-null) PageLabel. If the
	    /// function returns false the underlying SDF/Cos object is null or is
	    /// not valid and the PageLabel object should be treated as null as well.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelIsValid(ref this.mp_imp, ref result));
            return result;
        }
	    /// <summary>Gets full tittle of the page label
	    /// 
	    /// </summary>
	    /// <param name="page_num">page number
	    /// </param>
	    /// <returns> the full label title that is in effect for the given page. If there is no label object in effect, this method returns an empty string.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public String GetLabelTitle(int page_num)
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelGetLabelTitle(ref this.mp_imp, page_num, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
	    /// <summary> Sets the numbering style for the label.
	    /// 
	    /// </summary>
	    /// <param name="style">the numbering style for the label.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>
	    /// You may use label style to customize the page numbering schemes
	    /// used throughout a document. There are three numbering formats:
	    /// <list style="bullet">
	    /// <item><description>
	    /// decimal (often used for normal page ranges)
	    /// </description></item>
	    /// <item><description>
	    /// roman (often used for front matter such as a preface)
	    /// </description></item>
	    /// <item><description>
	    /// alphabetic (often used for back matter such as appendices)
	    /// </description></item>
	    ///</list>
	    /// There is no default numbering style; if no 'S' (Style) entry is present,
	    /// page labels consist solely of a label prefix with no numeric portion.
	    /// </remarks>
        public void SetStyle(Style style)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelSetStyle(ref this.mp_imp, style));
        }
	    /// <summary> Gets the style.
	    /// 
	    /// </summary>
	    /// <returns> page numbering style.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Style GetStyle()
        {
            Style result = Style.e_none;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelGetStyle(ref this.mp_imp, ref result));
            return result;
        }
	    /// <summary> Gets the prefix.
	    /// 
	    /// </summary>
	    /// <returns> the string used to prefix the numeric portion of
	    /// the page label
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public String GetPrefix()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelGetPrefix(ref this.mp_imp, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
	    /// <summary> Sets the prefix.
	    /// 
	    /// </summary>
	    /// <param name="prefix">the string used to prefix the numeric portion of
	    /// the page label.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPrefix(String prefix)
        {
            UString str = new UString(prefix);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelSetPrefix(ref this.mp_imp, str.mp_impl));
        }
	    /// <summary> Gets the start.
	    /// 
	    /// </summary>
	    /// <returns> the value to use when generating the numeric portion of
	    /// the first label in this range; must be greater than or equal to 1.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetStart()
        {
            int result = Int32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelGetStart(ref this.mp_imp, ref result));
            return result;
        }
	    /// <summary> Sets the start.
	    /// 
	    /// </summary>
	    /// <param name="start_at">the value to use when generating the numeric
	    /// portion of the first label in this range; must be greater than
	    /// or equal to 1.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetStart(int start_at)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelSetStart(ref this.mp_imp, start_at));
        }
	    /// <summary> Gets the first page num.
	    /// 
	    /// </summary>
	    /// <returns> the first page in the range associated with this label
	    /// or -1 if the label is not associated with any page.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetFirstPageNum()
        {
            int result = Int32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelGetFirstPageNum(ref this.mp_imp, ref result));
            return result;
        }
	    /// <summary> Gets the last page num.
	    /// 
	    /// </summary>
	    /// <returns> the last page in the range associated with this label
	    /// or -1 if the label is not associated with any page.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetLastPageNum()
        {
            int result = Int32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelGetLastPageNum(ref this.mp_imp, ref result));
            return result;
        }

	    /// <summary> Gets the sDF obj.
	    /// 
	    /// </summary>
	    /// <returns> The pointer to the underlying SDF/Cos object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageLabelGetSDFObj(ref this.mp_imp, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

        // Nested Types
        ///<summary>The numbering style to be used for the numeric portion of page label.</summary>
        public enum Style
        {
            ///<summary>Decimal Arabic numerals</summary>
            e_decimal,
            ///<summary>Uppercase roman numerals</summary>
            e_roman_uppercase,
            ///<summary>Lowercase roman numerals</summary>
            e_roman_lowercase,
            ///<summary>Uppercase letters (A to Z for the first 26 pages, AA to ZZ for the next 26, and so on)</summary>
            e_alphabetic_uppercase,
            ///<summary>Lowercase letters (a to z for the first 26 pages, aa to zz for the next 26, and so on)</summary>
            e_alphabetic_lowercase,
            ///<summary>No numeric portion in the label</summary>
            e_none
        }

    }
}
