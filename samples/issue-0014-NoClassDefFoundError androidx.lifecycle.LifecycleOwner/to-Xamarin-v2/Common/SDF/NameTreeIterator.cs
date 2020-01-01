using System;

using pdftronprivate.trn;
using pdftron;
using pdftron.Common;

using TRN_Obj = System.IntPtr;
using TRN_DictIterator = System.IntPtr;

namespace pdftron.SDF
{
    /// <summary>  NameTreeIterator is used to traverse key/value pairs in a NameTree.</summary>
    public class NameTreeIterator : DictIterator
    {
        internal NameTreeIterator(TRN_DictIterator imp, Object reference) : base(imp, reference) { }
    }
}