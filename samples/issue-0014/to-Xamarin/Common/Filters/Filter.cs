using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Filter = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.Filters
{
    /// <summary> Provides a generic view of a sequence of bytes. 
    /// <para>
    /// A Filter is the abstract base class of all filters. A filter is an abstraction of 
    /// a sequence of bytes, such as a file, an input/output device, an inter-process communication 
    /// pipe, or a TCP/IP socket. The Filter class and its derived classes provide a generic view 
    /// of these different types of input and output, isolating the programmer from the specific 
    /// details of the operating system and the underlying devices.
    /// </para>
    /// <para>
    /// Besides providing access to input/output sources Filters can be also to transform the data
    /// (e.g. to compress the data stream, to normalize the image data, to encrypt data, etc).
    /// Filters can also be attached to each other to form pipelines. For example, a filter used to 
    /// open an image data file can be attached to a filter that decompresses the data, which is 
    /// attached to another filter that will normalize the image data.
    /// </para>
    /// <para>
    /// Depending on the underlying data source or repository, filters might support only some of 
    /// these capabilities. An application can query a stream for its capabilities by using the 
    /// IsInputFilter() and CanSeek() properties.
    /// </para>
    /// 
    /// <example>
    /// To read or write data to a filter, a user will typically use FilterReader/FilterWriter class.
    /// instead of using Filter methods	
    /// <code>  
    /// MappedFile file = new MappedFile("my_stream.txt");
    /// FilterReader reader = new FilterReader(file);
    /// while (reader.read(...)) ...
    /// </code>
    /// </example>
    /// </summary>
    public class Filter : IDisposable
    {
        // Fields
        internal TRN_Filter mp_imp;
        internal Object m_ref;
        internal Filter m_attached;

        internal Filter(TRN_Filter imp, Filter attached)
        {
            this.mp_imp = imp;
            this.m_attached = attached;
        }
        internal IntPtr GetHandleInternal()
        {
            return this.mp_imp;
        }

        internal void setRefHandleInternal(Object reference)
        {
            this.m_ref = reference;
        }

        /// <summary> Releases all resources used by the Filter </summary>
        ~Filter()
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
            if (disposing)
            {
                // Dispose managed resources.
            }

            // Clean up native resources
            Destroy();
        }
        public void Destroy()
        {
            if (m_attached == null && m_ref == null && mp_imp != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_FilterDestroy(mp_imp));
                mp_imp = IntPtr.Zero;
            }
        }

        public Filter()
        {
            mp_imp = IntPtr.Zero;
            m_ref = null;
            m_attached = null;
        }

        /// <summary> Gets the name.
        /// 
        /// </summary>
        /// <returns>descriptive name of the filter.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual string GetName()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterGetName(mp_imp, ref result));
            return Marshal.PtrToStringUTF8(result);
        }
        /// <summary> Gets the decode name.
        /// 
        /// </summary>
        /// <returns>string representing the name of corresponding decode filter as
        /// it should appear in document (e.g. both ASCIIHexDecode and ASCIIHexEncode
        /// should return ASCIIHexDecode).
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual string GetDecodeName()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterGetDecodeName(mp_imp, ref result));
            return Marshal.PtrToStringUTF8(result);
        }
        /// <summary> Get the begin of the buffer </summary>
        /// <returns>beginning of the buffer of <c>Size()</c> bytes that can be used to read or write data.</returns>
        public virtual byte Begin()
        {
            byte result = new byte();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterBegin(mp_imp, ref result));
            return result;
        }
        ///<summary>Get size of buffer</summary>
        ///<returns>the size of buffer returned by <c>Begin()</c>. If the <c>Size()</c> returns 0 end of data has been reached.</returns>
        public virtual int Size()
        {
            UIntPtr result = UIntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterSize(mp_imp, ref result));
            int res = System.Convert.ToInt32(result.ToUInt32());
            return res;
        }
        /// <summary>Moves the Begin() pointer num_bytes forward.</summary>
        /// <param name="num_bytes">number of bytes to consume. num_bytes must be less than or equal to <c>Size()</c>.</param>
        public virtual void Consume(int num_bytes)
        {
            UIntPtr num = new UIntPtr(System.Convert.ToUInt32(num_bytes));
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterConsume(mp_imp, num));
        }
        ///<summary>Get the number of bytes consumed since opening the filter or the last Seek operation</summary>
        ///<returns>the number of bytes consumed since opening the filter or the last Seek operation</returns>
        public virtual int Count()
        {
            UIntPtr result = UIntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCount(mp_imp, ref result));
            int res = System.Convert.ToInt32(result.ToUInt32());
            return res;
        }
        ///<summary>Sets a new counting point for the current filter. All subsequent <c>Consume()</c> operations will increment this counter. Make sure that the output filter is flushed before using SetCount().</summary>
        ///<param name="new_count">new counting point</param>
        ///<returns>the value of previous counter</returns>
        public virtual int SetCount(int new_count)
        {
            UIntPtr result = UIntPtr.Zero;
            UIntPtr count = new UIntPtr(System.Convert.ToUInt32(new_count));
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterSetCount(mp_imp, count, ref result));
            int res = System.Convert.ToInt32(result.ToUInt32());
            return res;
        }
        /// <summary> The functions specifies the length of the data stream. The default
        /// implementation does not do anything. For some derived filters such
        /// as file segment filter it may be useful to override this function
        /// in order to limit the stream.
        /// 
        /// </summary>
        /// <param name="bytes">the new stream length
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual void SetStreamLength(int bytes)
        {
            UIntPtr num = new UIntPtr(System.Convert.ToUInt32(bytes));
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterSetStreamLength(mp_imp, num));
        }
        /// <summary> Attaches a filter to the this filter. If this filter owns another
        /// filter it will be deleted. This filter then becomes the owner of the
        /// attached filter.
        /// 
        /// </summary>
        /// <param name="attach_filter">the attach_filter
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual void AttachFilter(Filter attach_filter)
        {
            if (attach_filter != null)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_FilterAttachFilter(mp_imp, attach_filter.mp_imp));
                attach_filter.m_attached = this;
            }
        }
        /// <summary> Release the ownership of the attached filter. After the attached filter is
        /// released this filter points to NULL filter.
        /// 
        /// </summary>
        /// <returns> - Previously attached filter.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual Filter ReleaseAttachedFilter()
        {
            TRN_Filter result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterReleaseAttachedFilter(mp_imp, ref result));
            if (result == IntPtr.Zero)
            {
                return null;
            }
            return new Filter(result, null);
        }
        /// <summary> Get the attached filter.</summary>
        /// <returns>attached Filter or a NULL filter if no filter is attached.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual Filter GetAttachedFilter()
        {
            TRN_Filter result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterGetAttachedFilter(mp_imp, ref result));
            return new Filter(result, this);
        }
        /// <summary> Get the source filter.</summary>
        /// <returns>the first filter in the chain (usually a file filter)
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual Filter GetSourceFilter()
        {
            TRN_Filter result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterGetSourceFilter(mp_imp, ref result));
            Filter f = this;
            while(f.m_attached != null)
            {
                f = f.m_attached;
            }
            return new Filter(result, f);
        }
        /// <summary> Forces any data remaining in the buffer to be written to input or
        /// output filter.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual void Flush()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterFlush(mp_imp));
        }
        /// <summary> Forces any data remaining in the filter chain to the source or destination.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual void FlushAll()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterFlushAll(mp_imp));
        }
        /// <summary> Checks if is input filter.
        /// 
        /// </summary>
        /// <returns> - bool indicating whether this is an input filter.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual bool IsInputFilter()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterIsInputFilter(mp_imp, ref result));
            return result;
        }
        /// <summary> Check whether the stream supports seeking.
        /// 
        /// </summary>
        /// <returns> - true if the stream supports seeking; otherwise, false.
        /// default is to return false.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual bool CanSeek()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCanSeek(mp_imp, ref result));
            return result;
        }
        /// <summary> When overridden in a derived class, sets the position within the current stream.
        /// 
        /// </summary>
        /// <param name="offset">A byte offset relative to origin. If offset is negative, the new position will 
        /// precede the position specified by origin by the number of bytes specified by offset. If offset is zero, 
        /// the new position will be the position specified by origin. If offset is positive, the new position will 
        /// follow the position specified by origin by the number of bytes specified by offset.
        ///	</param>
        /// <param name="origin">A value of type ReferencePos indicating the reference point used to obtain the new position</param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>After each Seek() operation the number of consumed bytes (i.e. 
        /// Count()) is set to 0.</remarks>
        public virtual void Seek(int offset, ReferencePos origin)
        {
            IntPtr ptr = new IntPtr(offset);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterSeek(mp_imp, ptr, origin));
        }
        /// <summary> Reports the current read position in the stream relative to the stream origin.
        /// 
        /// </summary>
        /// <returns> - The current position in the stream
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual int Tell()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterTell(mp_imp, ref result));
            return result.ToInt32();
        }
        /// <summary> Create Filter iterator. Filter iterator similar to a regular filter. However,
        /// there can be only one owner of the attached filter.
        /// 
        /// </summary>
        /// <returns> the filter
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  - Derived classes should make sure that there is only one owner of the attached stream. 
        /// Otherwise the attached stream may be deleted several times.
        /// </remarks>
        public virtual Filter CreateInputIterator()
        {
            TRN_Filter result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCreateInputIterator(mp_imp, ref result));
            return new Filter(result, null);
        }
        /// <summary> Gets the file path.
        /// </summary>
        /// <returns> the file path to the underlying file stream.
        /// Default implementation returns empty string.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual string GetFilePath()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterGetFilePath(mp_imp, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

        /// <summary> Writes the entire filter, starting at current position, to
        /// specified filepath.  Should only be called on an input filter.
        /// </summary>
        /// <param name="path"> The output filepath. </param>
        /// <param name="append"> 'Frue' to append to existing file contents, 'False' to overwrite. </param>
        public void WriteToFile(string path, bool append)
        {
            UString str = new UString(path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterWriteToFile(mp_imp, str.mp_impl, append));
        }

        // Nested Types
        ///<summary>Provides the fields that represent reference points in streams for seeking.</summary>
        public enum ReferencePos
        {
            ///<summary>beginning of the stream</summary>
            e_begin,
            ///<summary>end of the stream</summary>
            e_cur,
            ///<summary>current position</summary>
            e_end
        }

    }
}
