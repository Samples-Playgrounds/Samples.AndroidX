using System;
using System.Collections.Generic;
using System.Text;

using pdftron.Common;

using TRN_PageSet = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> PageSet is a container of page numbers ordered following a linear sequence. 
    /// The page numbers are integers and must be greater than zero. Duplicates are allowed.
    /// </summary>
    /// <remarks>This is not a mathematical set</remarks>
    public class PageSet : IDisposable
    {
        internal TRN_PageSet mp_imp = IntPtr.Zero;

        /// <summary> Releases all resources used by the PageSet </summary>
        ~PageSet()
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
            if (mp_imp != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetDestroy(mp_imp));
                mp_imp = IntPtr.Zero;
            }
        }

        // Methods
        /// <summary> Default constructor. Constructs 'PageSet' with no pages</summary>
        public PageSet()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetCreate(ref mp_imp));
        }
	    /// <summary> Construct a set of pages with just one number.
	    /// 
	    /// </summary>
	    /// <param name="one_page">the one_page
	    /// </param>
	    /// <seealso cref="AddPage">
	    /// </seealso>
        public PageSet(int one_page)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetCreateSinglePage(one_page, ref mp_imp));
        }
	    /// <summary> Construct a range of pages.
	    /// 
	    /// </summary>
	    /// <param name="range_start">the range_start
	    /// </param>
	    /// <param name="range_end">the range_end
	    /// </param>
	    /// <seealso cref="AddRange(int,int,Filter)">
	    /// </seealso>
        public PageSet(int range_start, int range_end)
        {
            Filter filter = Filter.e_all;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetCreateFilteredRange(range_start, range_end, filter, ref mp_imp));
        }
	    /// <summary> Construct a filtered range of pages.
	    /// 
	    /// </summary>
	    /// <param name="range_start">the range_start
	    /// </param>
	    /// <param name="range_end">the range_end
	    /// </param>
	    /// <param name="filter">the filter
	    /// </param>
        /// <seealso cref="AddRange(int,int,Filter)">
        /// </seealso>
        public PageSet(int range_start, int range_end, Filter filter)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetCreateFilteredRange(range_start, range_end, filter, ref mp_imp));
        }

	    /// <summary> Add a value to the sequence.
	    /// 
	    /// </summary>
	    /// <param name="one_page">The page number being added
	    /// </param>
        public void AddPage(int one_page)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetAddPage(mp_imp, one_page));
        }
	    /// <summary> Add a range of values to the sequence. Reverse ordering is legal.
	    /// 
	    /// </summary>
	    /// <param name="range_start">The low value in the range
	    /// </param>
	    /// <param name="range_end">The high value in the range
	    /// </param>
        public void AddRange(int range_start, int range_end)
        {
            Filter filter = Filter.e_all;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetAddRange(mp_imp, range_start, range_end, filter));
        }
	    /// <summary> Add a range of values to the sequence. Reverse ordering is legal.
	    /// 
	    /// </summary>
	    /// <param name="range_start">The low value in the range
	    /// </param>
	    /// <param name="range_end">The high value in the range
	    /// </param>
        /// <param name="filter">page set filter type		
        /// </param>
        public void AddRange(int range_start, int range_end, Filter filter)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PageSetAddRange(mp_imp, range_start, range_end, filter));
        }


        // Nested Types
        ///<summary>PageSet filters</summary>
        public enum Filter
        {
            ///<summary>all pages in the range</summary>
            e_all,
            ///<summary>odd numbers in the range (discards even numbers)</summary>
            e_even,
            ///<summary>even numbers in the range (discards odd numbers)</summary>
            e_odd
        }

    }
}
