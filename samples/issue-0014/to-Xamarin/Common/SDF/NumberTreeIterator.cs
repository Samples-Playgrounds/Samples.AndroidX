using System;

using pdftronprivate.trn;
using pdftron;
using pdftron.Common;

using TRN_Obj = System.IntPtr;
using TRN_DictIterator = System.IntPtr;

namespace pdftron.SDF
{
    /// <summary>  NumberTreeIterator is used to traverse key/value pairs in a NumberTree.</summary>
    public class NumberTreeIterator : DictIterator
    {
        internal NumberTreeIterator(TRN_DictIterator imp, Object reference) : base(imp, reference) { }
    }
}