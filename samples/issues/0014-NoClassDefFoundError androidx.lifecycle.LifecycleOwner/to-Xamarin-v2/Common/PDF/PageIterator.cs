using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Iterator = System.IntPtr;
using TRN_ItrData = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> PageIterator is an iterator type that can be used to traverse a list 
    /// pages in a PDF document. For more information, please PDFDoc.GetPageIterator().
    /// </summary>
    public class PageIterator : Iterator<Page>
    {
        internal Object m_ref;
        internal PageIterator(TRN_Iterator imp, Object reference) : base(imp)
        {
            m_ref = reference;
        }
        /// <summary>
        /// Gets the current <c>Page</c> object
        /// </summary>
        /// <returns>current <c>Page</c> object</returns>
        public override Page Current()
        {
            if (mp_impl == IntPtr.Zero)
                throw new PDFNetException();
            TRN_ItrData result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_IteratorCurrent(mp_impl, ref result));
            IntPtr res = Marshal.ReadIntPtr(result);
            return new Page(res, this.m_ref);
        }
        /// <summary>Gets the page number
        /// </summary>
        /// <returns>page number
        /// </returns>
        public int GetPageNumber()
        {
            return Current().GetIndex();
        }
        /// <summary>Sets value to the given <c>PageIterator</c> object
        /// </summary>
        /// <param name="p">given <c>PageIterator</c> object
        /// </param>
        public void Set(PageIterator p)
        {
            this.op_Assign(p);
        }
        /// <summary>Checks whether this <c>PageIterator</c> is the same as the specified object.</summary>
        /// <param name="o">a given <c>Object</c>
        /// </param>
        /// <returns>true, if equals to the given object
        /// </returns>
        public new bool Equals(object o)
        {
            if (o == null) return false;
            PageIterator i = o as PageIterator;
            if (i == null) return false;
            bool b = mp_impl == i.mp_impl;
            return b;
        }
        /// <summary>Equality operator checks whether two <c>PageIterator</c> objects are the same.</summary>
        /// <param name="l">a <c>PageIterator</c> at the left of the operator
        /// </param>
        /// <param name="r">a <c>PageIterator</c> at the right of the operator
        /// </param>
        /// <returns>true, if both <c>PageIterator</c> are equal
        /// </returns>
        public static bool operator ==(PageIterator l, PageIterator r)
        {
            if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return true;
            if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null))
                return false;
            if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return false;

            return l.Equals(r);
        }
        /// <summary>Inequality operator checks whether two <c>PageIterator</c> objects are different.</summary>
        /// <param name="l">a <c>PageIterator</c> at the left of the operator
        /// </param>
        /// <param name="r">a <c>PageIterator</c> at the right of the operator
        /// </param>
        /// <returns>true, if both <c>PageIterator</c> are not equal
        /// </returns>
        public static bool operator !=(PageIterator l, PageIterator r)
        {
            return !(l == r);
        }
    }
}
