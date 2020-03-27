using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Iterator = System.IntPtr;
using TRN_ItrData = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> CharIterator is an iterator type that can be used to traverse CharData 
    /// in the current e_text element. For a sample use case, please take a look 
    /// at ElementReaderAdv sample project.
    /// </summary>
    public class CharIterator : Iterator<CharData>
    {
        internal CharIterator(TRN_Iterator imp) : base(imp) { }
        /// <summary>Gets the current <c>CharData</c> value
        /// </summary>
        /// <returns>current Unicode value
        /// </returns>
        public override CharData Current()
        {
            if (mp_impl == IntPtr.Zero)
                throw new PDFNetException();
            TRN_ItrData result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_IteratorCurrent(mp_impl, ref result));
            BasicTypes.TRN_CharData res = (BasicTypes.TRN_CharData)Marshal.PtrToStructure(result, typeof(BasicTypes.TRN_CharData));
            return new CharData(res);
        }

        /// <summary>checks if this CharData object is the same as the given object</summary>
	    /// <param name="o">a given object
	    /// </param>
	    /// <returns>true if equals to the give object
	    /// </returns>
        public new bool Equals(Object o)
        {
            if (o == null) return false;
            CharIterator i = o as CharIterator;
            if (i == null) return false;
            bool b = mp_impl == i.mp_impl;
            return b;
        }
	    /// <summary>Equality operator checks whether two CharData objects are the same</summary>
	    /// <param name="l">the <c>CharData</c> on the left of the operator
	    /// </param>
	    /// <param name="r">the <c>CharData</c> on the right of the operator
	    /// </param>
        /// <returns>true if both objects are equal
        /// </returns>
        public static bool operator ==(CharIterator l, CharIterator r)
        {
            if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return true;
            if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null))
                return false;
            if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return false;

            return l.Equals(r);
        }
	    /// <summary>Inequality operator checks whether two CharData objects are different</summary>
	    /// <param name="l">the <c>CharData</c> on the left of the operator
	    /// </param>
	    /// <param name="r">the <c>CharData</c> on the right of the operator
	    /// </param>
	    /// <returns>true if both objects are not equal
	    /// </returns>
        public static bool operator !=(CharIterator l, CharIterator r)
        {
            return !(l == r);
        }
    }
}