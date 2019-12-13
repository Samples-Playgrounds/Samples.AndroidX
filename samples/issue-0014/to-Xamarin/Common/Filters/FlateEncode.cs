using System;

using pdftron.Common;

namespace pdftron.Filters
{
    /// <summary> FlateEncode filter can be used to compress any data stream 
    /// using Flate (i.e. ZIP) compression method. 
    /// </summary>
    public class FlateEncode : Filter
    {
        /// <summary> Constructor for Flate encoder.
		/// 
		/// </summary>
		/// <param name="input_filter">the input data stream
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FlateEncode(Filter input_filter)
        {
            int compression_level = -1;
            UIntPtr buf_sz = new UIntPtr(256);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCreateFlateEncode((input_filter == null) ? IntPtr.Zero : input_filter.mp_imp, compression_level, buf_sz, ref mp_imp));
            if (input_filter != null)
            {
                input_filter.m_attached = this;
            }
        }
		/// <summary> Instantiates a new flate encode.
		/// 
		/// </summary>
		/// <param name="input_filter">the input_filter
		/// </param>
		/// <param name="compression_level">compression level
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FlateEncode(Filter input_filter, Int32 compression_level)
        {
            UIntPtr buf_sz = new UIntPtr(256);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCreateFlateEncode(input_filter.mp_imp, compression_level, buf_sz, ref mp_imp));
            if (input_filter != null)
            {
                input_filter.m_attached = this;
            }
        }
		/// <summary> Instantiates a new flate encode.
		/// 
		/// </summary>
		/// <param name="input_filter">input filter
		/// </param>
		/// <param name="compression_level">compression level
		/// </param>
		/// <param name="buf_sz">size of buffer
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FlateEncode(Filter input_filter, Int32 compression_level, Int32 buf_sz)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCreateFlateEncode(input_filter.mp_imp, compression_level, new UIntPtr(System.Convert.ToUInt32(buf_sz)), ref mp_imp));
            if (input_filter != null)
            {
                input_filter.m_attached = this;
            }
        }
    }
}