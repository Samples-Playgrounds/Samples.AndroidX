using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Iterator = System.IntPtr;
using TRN_ItrData = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> DigitalSignatureFieldIterator is an iterator type that can be used to traverse a list 
    /// of digital signature form fields in a PDF document. 
    /// </summary>
    public class DigitalSignatureFieldIterator : Iterator<DigitalSignatureField>
    {
        internal DigitalSignatureFieldIterator(TRN_Iterator imp) : base(imp) { }
        /// <summary>Gets the current <c>DigitalSignatureField</c> object</summary>
        /// <returns>current <c>DigitalSignatureField</c> object</returns>
        public override DigitalSignatureField Current()
        {
            if (mp_impl == IntPtr.Zero)
                throw new PDFNetException();
            TRN_ItrData result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_IteratorCurrent(mp_impl, ref result));
            BasicTypes.TRN_DigitalSignatureField res = (BasicTypes.TRN_DigitalSignatureField)Marshal.PtrToStructure(result, typeof(BasicTypes.TRN_DigitalSignatureField));
            return new DigitalSignatureField(res, this);
        }

        /// <summary>Determines if equals to the given object</summary>
        /// <param name="o">given object</param>
        /// <returns>true if equals to the given object</returns>
        public new bool Equals(Object o)
        {
            if (o == null) return false;
            DigitalSignatureFieldIterator i = o as DigitalSignatureFieldIterator;
            if (i == null) return false;
            bool b = mp_impl == i.mp_impl;
            return b;
        }
        ///<summary>Equality operator check whether two DigitalSignatureFieldIterator objects are the same</summary>
        ///<param name="l">a <c>DigitalSignatureFieldIterator</c> object on the left of the operator</param>
        ///<param name="r">a <c>DigitalSignatureFieldIterator</c> object on the right of the operator</param>
        ///<returns>true, if both objects are equal</returns>
        public static Boolean operator ==(DigitalSignatureFieldIterator l, DigitalSignatureFieldIterator r)
        {
            if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return true;
            if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null))
                return false;
            if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return false;

            return l.Equals(r);
        }
        ///<summary>Inequality operator checks whether two DigitalSignatureFieldIterator objects are different</summary>
        ///<param name="l">a <c>DigitalSignatureFieldIterator</c> object on the left of the operator</param>
        ///<param name="r">a <c>DigitalSignatureFieldIterator</c> object on the right of the operator</param>
        ///<returns>true, if both objects are not equal</returns>
        public static Boolean operator !=(DigitalSignatureFieldIterator l, DigitalSignatureFieldIterator r)
        {
            return !(l == r);
        }
    }
}
