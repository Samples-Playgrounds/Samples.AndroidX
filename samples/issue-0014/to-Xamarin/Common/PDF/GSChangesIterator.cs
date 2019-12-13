using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Iterator = System.IntPtr;
using TRN_ItrData = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> GSChangesIterator is an iterator type that can be used to traverse a list 
    /// of changes in the graphics state between subsequnet graphical elements on the 
    /// page. For a sample use case, please take a look at ElementReaderAdv sample project.
    /// </summary>
    public class GSChangesIterator : Iterator<GState.GStateAttribute>
    {
        internal GSChangesIterator(TRN_Iterator imp) : base(imp) { }
        /// <summary>Gets the current <c>GStateAttribute</c> value
        /// </summary>
        /// <returns>current Unicode value
        /// </returns>
        public override GState.GStateAttribute Current()
        {
            if (mp_impl == IntPtr.Zero)
                throw new PDFNetException();
            TRN_ItrData result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_IteratorCurrent(mp_impl, ref result));
            int res = Marshal.ReadInt32(result);
            return (GState.GStateAttribute)res;
        }

        /// <summary>Checks whether this GSChangesIterator is the same as the given object.
        /// </summary>
        /// <param name="o">a given object
        /// </param>
        /// <returns>true if equals to the give object
        /// </returns>
        public new bool Equals(Object o)
        {
            if (o == null) return false;
            GSChangesIterator i = o as GSChangesIterator;
            if (i == null) return false;
            bool b = mp_impl == i.mp_impl;
            return b;
        }
        /// <summary>Equality operator checks whether two <c>GSChangesIterator</c> are the same</summary>
        /// <param name="l">the <c>GSChangesIterator</c> on the left of the operator
        /// </param>
        /// <param name="r">the <c>GSChangesIterator</c> on the right of the operator
        /// </param>
        /// <returns>true if both objects are equal
        /// </returns>
        public static Boolean operator ==(GSChangesIterator l, GSChangesIterator r)
        {
            if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return true;
            if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null))
                return false;
            if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return false;

            return l.Equals(r);
        }
        /// <summary>Inequality operator checks whether two <c>GSChangesIterator</c> are different</summary>
        /// <param name="l">the <c>GSChangesIterator</c> on the left of the operator
        /// </param>
        /// <param name="r">the <c>GSChangesIterator</c> on the right of the operator
        /// </param>
        /// <returns>true if both objects are not equal
        /// </returns>
        public static Boolean operator !=(GSChangesIterator l, GSChangesIterator r)
        {
            return !(l == r);
        }
    }
}