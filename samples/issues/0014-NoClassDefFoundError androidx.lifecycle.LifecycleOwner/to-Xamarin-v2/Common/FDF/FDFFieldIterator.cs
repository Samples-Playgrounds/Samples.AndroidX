using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Iterator = System.IntPtr;
using TRN_ItrData = System.IntPtr;

namespace pdftron.FDF
{
    /// <summary> FDFFieldIterator is an iterator type used to traverse interactive form fields 
    /// in a FDF document. A FDFFieldIterator points to FDF.FDFField nodes or to the <c>null</c>
    /// FDFField node. 
    /// </summary>
    /// <example>
    /// A sample use case:
    /// <code>  
    /// for(FDFFieldIterator itr = fdf_doc.getFieldIterator(); itr.hasNext(); itr.next()) {
    /// Field name: itr.current().getName();
    /// Field partial name: itr.current().getPartialName();
    /// }
    /// </code>
    /// </example>
    public class FDFFieldIterator : Iterator<FDFField>
    {
        internal FDFFieldIterator(TRN_Iterator imp) : base(imp) { }
        ///<summary>get the current FDFFieldIterator
        ///</summary>
        ///<returns>current <c>FDFField</c> object</returns>
        public override FDFField Current()
        {
            if (mp_impl == IntPtr.Zero)
                throw new PDFNetException();
            TRN_ItrData result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_IteratorCurrent(mp_impl, ref result));
            BasicTypes.TRN_FDFField res = (BasicTypes.TRN_FDFField)Marshal.PtrToStructure(result, typeof(BasicTypes.TRN_FDFField));
            return new FDFField(res, this);
        }
        ///<summary>check if equals to another <c>FDFFieldIterator</c>
        ///</summary>
        ///<param name="o">another <c>FDFFieldIterator</c> to compare to
        ///</param>
        ///<returns>true if equals to another <c>FDFFieldIterator</c>. false, otherwise.
        ///</returns>
        public new bool Equals(Object o)
        {
            if (o == null) return false;
            FDFFieldIterator i = o as FDFFieldIterator;
            if (i == null) return false;
            bool b = mp_impl == i.mp_impl;
            return b;
        }
        ///<summary>check if two <c>FDFFieldIterator</c> are equal
        ///</summary>
        ///<param name="l">a <c>FDFFieldIterator</c> object
        ///</param>
        ///<param name="r">another <c>FDFFieldIterator</c> object
        ///</param>
        ///<returns>true, if both are equal. false, otherwise.
        ///</returns>
        public static bool operator ==(FDFFieldIterator l, FDFFieldIterator r)
        {
            if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return true;
            if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null))
                return false;
            if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null))
                return false;

            return l.Equals(r);
        }
        ///<summary>check if two <c>FDFFieldIterator</c> are not equal
        ///</summary>
        ///<param name="l">a <c>FDFFieldIterator</c> object
        ///</param>
        ///<param name="r">another <c>FDFFieldIterator</c> object
        ///</param>
        ///<returns>false, if both are equal. true, otherwise.
        ///</returns>
        public static bool operator !=(FDFFieldIterator l, FDFFieldIterator r)
        {
            return !(l == r);
        }
    }
}
