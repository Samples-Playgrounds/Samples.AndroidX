using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

using pdftron.Common;

using TRN_FilterReader = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Filter = System.IntPtr;

namespace pdftron.Filters
{
    /// <summary> FilterReader is a utility class providing a convenient way to read data
    /// from an input filter (using Filter directly is not very intuitive).		
    /// </summary>
    /// <example>
    /// <code>  
    /// MappedFile file=new MappedFile("my_stream.txt"));
    /// FilterReader reader=new FilterReader(file);
    /// while (reader.Read(...)) ...
    /// </code>
    /// </example>
    public class FilterReader : IDisposable
    {
        internal TRN_FilterReader mp_imp;
        internal Filter m_attached = null;
        internal FilterReader(TRN_FilterReader imp)
        {
            this.mp_imp = imp;
        }

        /// <summary> Releases all resources used by the FilterReader </summary>
        ~FilterReader()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (disposing)
            {
                // Dispose managed resources.
            }

            // Clean up native resources
            Destroy();
        }
        public void Destroy()
        {
            if (mp_imp != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReaderDestroy(mp_imp));
                mp_imp = IntPtr.Zero;
            }
        }

        /// <summary> Instantiates a new filter reader.
		/// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FilterReader()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReaderCreate(IntPtr.Zero, ref mp_imp));
            this.m_attached = null;
        }
		/// <summary> Instantiates a new filter reader.
		/// 
		/// </summary>
		/// <param name="input_filter">input filter
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FilterReader(Filter input_filter)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReaderCreate(input_filter.mp_imp, ref mp_imp));
            this.m_attached = input_filter;
        }
		/// <summary> Gets the next character
		/// 
		/// </summary>
		/// <returns> - the next character from the stream or EOF (-1) if the end of file is reached.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int Get()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReaderGet(mp_imp, ref result));
            return result;
        }
		/// <summary> Peek.
		/// 
		/// </summary>
		/// <returns> - the next character without extracting it from the stream or
		/// or EOF (-1) if the end of file is reached.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int Peek()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReaderPeek(mp_imp, ref result));
            return result;
        }
		/// <summary> Read.
		/// 
		/// </summary>
		/// <param name="buf">the buf_size
		/// </param>
		/// <returns> - returns the number of bytes actually read and stored in buffer (buf),
		/// which may be less than buf_size if the end of the file is encountered before
		/// reaching count.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int Read(byte[] buf)
        {
            UIntPtr result = UIntPtr.Zero;

            int psize = Marshal.SizeOf(buf[0]) * buf.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReaderRead(mp_imp, pnt, new UIntPtr(System.Convert.ToUInt32(buf.Length)), ref result));
                int bytesRead = System.Convert.ToInt32(result.ToUInt32());
                Marshal.Copy(pnt, buf, 0, bytesRead);
                return bytesRead;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }
		/// <summary> Count.
		/// 
		/// </summary>
		/// <returns> - the number of bytes consumed since opening the filter or
		/// since the last Seek operation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int Count()
        {
            UIntPtr result = UIntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReaderCount(mp_imp, ref result));
            return System.Convert.ToInt32(result.ToUInt32());
        }
		/// <summary> Attaches a filter to the this FilterReader.
		/// 
		/// </summary>
		/// <param name="filter">the filter to attach
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void AttachFilter(Filter filter)
        {
            this.m_attached = filter;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReaderAttachFilter(mp_imp, filter.mp_imp));
        }
		/// <summary> Gets the attached filter.
		/// 
		/// </summary>
		/// <returns> - The attached Filter or a NULL filter if no filter is attached.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Filter GetAttachedFilter()
        {
            return this.m_attached;
        }
		/// <summary> Sets the position within the current stream.
		/// 
		/// </summary>
		/// <param name="offset">- A byte offset relative to origin. If offset is negative,
		/// the new position will precede the position specified by origin by the number
		/// of bytes specified by offset. If offset is zero, the new position will be the
		/// position specified by origin. If offset is positive, the new position will follow
		/// the position specified by origin by the number of bytes specified by offset.
		/// </param>
		/// <param name="origin">- A value of type ReferencePos indicating the reference point used
		/// to obtain the new position
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  - After each Seek() operation the number of consumed bytes (i.e. Count()) is set to 0. </remarks>
        public void Seek(int offset, Filter.ReferencePos origin)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReaderSeek(mp_imp, new IntPtr(offset), origin));
        }
		/// <summary> Reports the current read position in the stream relative to the stream origin.
		/// 
		/// </summary>
		/// <returns> - The current position in the stream
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int Tell()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReaderTell(mp_imp, ref result));
            return result.ToInt32();
        }
		/// <summary> Forces any data remaining in the buffer to be written to input or output filter.
		/// 
		/// </summary>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Flush()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReaderFlush(mp_imp));
        }
		/// <summary> Forces any data remaining in the filter chain to the source or destination.
		/// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void FlushAll()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReaderFlush(mp_imp));
        }
    }
}
