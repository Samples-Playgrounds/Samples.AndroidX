using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Iterator = System.IntPtr;
using TRN_ItrData = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> FontCharCodeIterator is an iterator type that can be used to traverse a list 
    /// of visible char codes in a font embedded in PDF. For more information, please 
    /// take a look at Font.getCodeIterator().
    /// </summary>
    public class FontCharCodeIterator : Iterator<uint>
    {
        internal FontCharCodeIterator(TRN_Iterator imp) : base(imp) { }
        ///<summary>get the current FDFFontCharCodeIterator
        ///</summary>
        ///<returns>current <c>FDFField</c> object</returns>
        public override uint Current()
        {
            if (mp_impl == IntPtr.Zero)
                throw new PDFNetException();
            TRN_ItrData result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_IteratorCurrent(mp_impl, ref result));
            int res = Marshal.ReadInt32(result);
            return System.Convert.ToUInt32(res);
        }
    }
}