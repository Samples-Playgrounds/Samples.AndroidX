using System;
using System.Runtime.InteropServices;

using pdftron.Common;

namespace pdftron.Filters
{
    /// <summary> MemoryFilter is a memory buffer that can be used as a source or a sink Filter in 
    /// the Filter pipeline. When a MemoryFilter is the source, other filters can read data
    /// stored in memory; When MemoryFilter is a sink, other filters generate data which
    /// is then pushed in a memory buffer owned by MemoryFilter (MemoryFilter makes sure
    /// that there is enough space to accomodate all data).
    /// </summary>
    public class MemoryFilter : Filter
    {
        /// <summary> Instantiates a new memory filter.
        /// 
        /// </summary>
        /// <param name="buf_sz">buffer size
        /// </param>
        /// <param name="is_input">whether the Mememory filter is input filter
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public MemoryFilter(Int32 buf_sz, Boolean is_input)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCreateMemoryFilter(new UIntPtr(System.Convert.ToUInt32(buf_sz)), is_input, ref mp_imp));
        }
        /// <summary> Gets the buffer.
        /// 
        /// </summary>
        /// <returns> The entire memory buffer.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  MemoryFilter specific function. </remarks>
        public virtual byte[] GetBuffer()
        {
            IntPtr src = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterMemoryFilterGetBuffer(mp_imp, ref src));

            byte[] result = new byte[Size()];
            Marshal.Copy(src, result, 0, Size());
            return result;
        }
        //TODO: method description missing
        /// <summary>
        /// </summary>
        //public virtual void Reset()
        //{

        //}
        /// <summary> Sets this <c>MemoryFilter</c> as input filter.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks> MemoryFilter specific function used to change filter mode from output to input. </remarks>
        /// <remarks> The utility function is usefull in cases when and output data buffer should be
        /// converted to an input stream.
        /// </remarks>
        /// <remarks>  The function has no effect on an input MemoryFilter. </remarks>
        public virtual void SetAsInputFilter()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterMemoryFilterSetAsInputFilter(mp_imp));
        }
    }
}