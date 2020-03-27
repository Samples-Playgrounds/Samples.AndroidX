using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Iterator = System.IntPtr;
using TRN_ItrData = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> FieldIterator is an iterator type that can be used to traverse a list 
    /// form fields in a PDF document. For more information, please PDFDoc.getFieldIterator().
    /// </summary>
    public class FieldIterator : Iterator<Field>
    {
        internal FieldIterator(TRN_Iterator imp) : base(imp) { }
        ///<summary>get the current FDFFieldIterator
        ///</summary>
        ///<returns>current <c>FDFField</c> object</returns>
        public override Field Current()
        {
            if (mp_impl == IntPtr.Zero)
                throw new PDFNetException();
            TRN_ItrData result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_IteratorCurrent(mp_impl, ref result));
            BasicTypes.TRN_Field res = (BasicTypes.TRN_Field)Marshal.PtrToStructure(result, typeof(BasicTypes.TRN_Field));
            return new Field(res, this);
        }

        /// <summary>Determines if equals to the given object</summary>
        /// <param name="o">given object</param>
        /// <returns>true if equals to the given object</returns>
        public new bool Equals(Object o)
        {
            if (o == null) return false;
            FieldIterator i = o as FieldIterator;
            if (i == null) return false;
            bool b = mp_impl == i.mp_impl;
            return b;
        }
        ///<summary>Equality operator check whether two FieldIterator objects are the same</summary>
        ///<param name="l">a <c>Field</c> iterator object on the left of the operator</param>
        ///<param name="r">a <c>Field</c> iterator object on the right of the operator</param>
        ///<returns>true, if both objects are equal</returns>
        public static Boolean operator ==(FieldIterator l, FieldIterator r)
        {
            if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return true;
            if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null))
                return false;
            if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return false;

            return l.Equals(r);
        }
        ///<summary>Inequality operator checks whether two FieldIterator objects are different</summary>
        ///<param name="l">a <c>Field</c> iterator object on the left of the operator</param>
        ///<param name="r">a <c>Field</c> iterator object on the right of the operator</param>
        ///<returns>true, if both objects are not equal</returns>
        public static Boolean operator !=(FieldIterator l, FieldIterator r)
        {
            return !(l == r);
        }
    }
}
