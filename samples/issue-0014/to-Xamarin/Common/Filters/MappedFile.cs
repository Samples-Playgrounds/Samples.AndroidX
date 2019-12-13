using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

namespace pdftron.Filters
{
    /// <summary> MappedFile is a utility class to read files on a file system. Because MappedFile file is 
    /// derived from pdftron.Filters.Filter you can directly chain MappedFile objects to other
    /// 'pdftron.Filters'.
    ///
    /// MappedFile objects support random access to files using the Seek method. Seek 
    /// allows the read/write position to be moved to any position within the file. This
    /// is done through a shared memory mapped chunk manager. The byte offset is relative 
    /// to the seek reference point, which can be the beginning, the current position, 
    /// or the end of the underlying file, as represented by the three properties of the 
    /// Filter.ReferencePos class.
    /// MappedFile objects are thread-safe, meaning separate copies of a MappedFile can Seek
    /// to different locations in the file, without conflicting with one another.
    /// Disk files always support random access. At the time of construction, the CanSeek()
    /// property value is set to true or false depending on the underlying file type.
    /// 
    /// </summary>
    /// <remarks>  .NET or Java applications should explicitly Close() files when they are not needed. </remarks>
    /// <summary> If the files are not closed or disposed this may lead to the resource exhaustion.
    /// </summary>
    public class MappedFile : Filter, IDisposable
    {
        private bool disposed = false;
        /// <summary> Create a new instance of MappedFile class with the specified path.
		/// 
		/// </summary>
		/// <param name="filename">filename
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public MappedFile(String filename)
        {
            UString str = new UString(filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCreateMappedFileFromUString(str.mp_impl, ref mp_imp));
        }
        ///<summary>close filter and release relevant resources</summary>
        public void Close()
        {
            if (!this.disposed)
                Dispose(false);
            this.disposed = true;
        }
		/// <summary> Gets File size.
		/// 
		/// </summary>
		/// <returns> the size of the current file.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 FileSize()
        {
            UIntPtr result = UIntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterMappedFileFileSize(mp_imp, ref result));
            return (unchecked((IntPtr)(long)(ulong)result)).ToInt32();
        }
		/// <summary> Releases all resources used by the MappedFile </summary>
        ~MappedFile()
        {
            if(!this.disposed)
                Dispose(false);
            this.disposed = true;
        }
        // Nested Types
        ///<summary>open file mode</summary>
        public enum OpenMode
        {
            ///<summary>Opens file for reading. An exception is thrown if the file doesn't exist.</summary>
            e_read_mode,
            ///<summary>Opens an empty file for writing. If the given file exists, its contents are destroyed.</summary>
            e_write_mode,
            ///<summary>Opens for reading and appending. Creates the file first if it doesn't exist.</summary>
            e_append_mode
        }
    }
}