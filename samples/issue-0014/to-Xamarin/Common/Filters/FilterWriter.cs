using System;
using System.Collections.Generic;
using System.Text;

using pdftron.Common;

using TRN_FilterWriter = System.IntPtr;
using TRN_Filter = System.IntPtr;
using System.Runtime.InteropServices;

namespace pdftron.Filters
{
    /// <summary> FilterWriter is a utility class providing a convenient way to write data
    /// to an output filter (using Filter directly is not very intuitive).
    /// </summary>
    /// <example>
    /// <code>  
    /// StdFile outfile=new StdFile("file.dat"), StdFile.e_write_mode);
    /// FilterWriter fwriter=new FilterWriter(outfile);
    /// byte[] buf=...
    /// fwriter.writeBuffer(buf);
    /// fwriter.flush();
    /// </code>
    /// </example>
    public class FilterWriter : IDisposable
    {
        internal TRN_FilterWriter mp_imp = IntPtr.Zero;
        internal Filter m_attached = null;

        internal FilterWriter(TRN_FilterWriter imp)
        {
            this.mp_imp = imp;
        }

        /// <summary> Instantiates a new filter writer.
		/// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FilterWriter()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterCreate(IntPtr.Zero, ref mp_imp));
        }
		/// <summary> Instantiates a new filter writer.
		/// 
		/// </summary>
		/// <param name="output_filter">the filter
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FilterWriter(Filter output_filter)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterCreate(output_filter.mp_imp, ref mp_imp));
            this.m_attached = output_filter;
        }
		/// <summary> Write a single character to the output stream.
		/// 
		/// </summary>
		/// <param name="ch">An unsigned character to write to the output stream.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void WriteUChar(Byte ch)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterWriteUChar(mp_imp, ch));
        }
		/// <summary> Write a 32 bit integer to the output stream.
		/// 
		/// </summary>
		/// <param name="num">An integer to write to the output stream.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void WriteInt(Int32 num)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterWriteInt32(mp_imp, num));
        }
		/// <summary> Write a 64 bit integer to the output stream.
		/// 
		/// </summary>
		/// <param name="num">An integer to write to the output stream.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void WriteInt(Int64 num)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterWriteInt64(mp_imp, num));
        }
		/// <summary> Write a string to the output stream.
		/// 
		/// </summary>
		/// <param name="str">A string to write to the output stream.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void WriteString(string str)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterWriteString(mp_imp, str));
        }
		/// <summary> Write out a null terminated 'line' followed by a end of line character
		/// default end of line character  is carriage return.
		/// 
		/// </summary>
		/// <param name="str">the line
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void WriteLine(string str)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterWriteLine(mp_imp, str, 0x0D)); //char eol = 0x0D
        }
		/// <summary> Write the entire input stream to the output stream (i.e. to this FilterWriter).
		/// 
		/// </summary>
		/// <param name="reader">A FilterReader attached to an input stream.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void WriteFilter(FilterReader reader)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterWriteFilter(mp_imp, reader.mp_imp));
        }
		/// <summary> Write buffer.
		/// 
		/// </summary>
		/// <param name="buf">the buf
		/// </param>
		/// <returns> - returns the number of bytes actually written to a stream. This number may
		/// less than buf_size if the stream is corrupted.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 WriteBuffer(byte[] buf)
        {
            UIntPtr result = UIntPtr.Zero;

            GCHandle pinnedRawData = GCHandle.Alloc(buf, GCHandleType.Pinned);
            IntPtr pnt = pinnedRawData.AddrOfPinnedObject();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterWriteBuffer(mp_imp, pnt, new UIntPtr(System.Convert.ToUInt32(buf.Length)), ref result));
            int bytesWrite = System.Convert.ToInt32(result.ToUInt32());
            return bytesWrite;
        }
		/// <summary> Count.
		/// 
		/// </summary>
		/// <returns> - the number of bytes consumed since opening the filter or
		/// since the last Seek operation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 Count()
        {
            UIntPtr result = UIntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterCount(mp_imp, ref result));
            return System.Convert.ToInt32(result.ToUInt32());
        }
		/// <summary> Attaches a filter to the this FilterWriter.
		/// 
		/// </summary>
		/// <param name="filter">the filter
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void AttachFilter(Filter filter)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterAttachFilter(mp_imp, filter.mp_imp));
            this.m_attached = filter;
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
        public void Seek(Int32 offset, Filter.ReferencePos origin)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterSeek(mp_imp, new IntPtr(offset), origin));
        }
		/// <summary> Reports the current read position in the stream relative to the stream origin.
		/// 
		/// </summary>
		/// <returns> - The current position in the stream
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 Tell()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterTell(mp_imp, ref result));
            return result.ToInt32();
        }
		/// <summary> Forces any data remaining in the buffer to be written to input or output filter.
		/// 
		/// </summary>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Flush()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterFlush(mp_imp));
        }
		/// <summary> Forces any data remaining in the filter chain to the source or destination.
		/// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void FlushAll()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterFlushAll(mp_imp));
        }

        /// <summary> Releases all resources used by the FilterWriter </summary>
        ~FilterWriter()
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
                PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriterDestroy(mp_imp));
                mp_imp = IntPtr.Zero;
            }
        }
    }
}
