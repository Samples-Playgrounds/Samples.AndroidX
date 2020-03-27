using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.Filters;
using pdftron.SDF;
using pdftron.PDF;

using TRN_SDFDoc = System.IntPtr;
using TRN_UString = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.SDF
{
    /// <summary> SDFDoc is a low-level document representing a graph of SDF.Obj nodes that 
    /// can be used to build higher-level document models such as PDF (Portable Document
    /// Format) or FDF (Forms Document Format).
    /// 
    /// SDF Doc brings together document security, document utility methods, and all SDF 
    /// objects.
    /// 
    /// </summary>
    /// <example>
    /// A SDF document can be created from scratch using a default constructor:
    /// <code>
    /// SDFDoc mydoc = new SDFDoc();
    /// Obj trailer = mydoc.getTrailer();
    /// </code>
    /// SDF document can be also created from an existing file (e.g. an external PDF document): 	
    /// <code>  
    /// SDFDoc mydoc = new SDFDoc("in.pdf");
    /// Obj trailer = mydoc.getTrailer();
    /// </code>	
    /// SDF document can be also created from a memory buffer or some other Filter/Stream such as a HTTP Filter connection: 	
    /// <code>  
    /// MemoryFilter memory = ....
    /// SDFDoc mydoc = new SDFDoc(memory);
    /// Obj trailer = mydoc.getTrailer();
    /// </code>	
    /// SDF document can be accessed from a high-level PDF document as follows: 	
    /// <code>  
    /// PDFDoc doc = new PDFDoc("in.pdf");
    /// SDFDoc mydoc = doc.getSDFDoc();
    /// Obj trailer = mydoc.getTrailer();
    /// </code>
    /// </example>
    /// <remarks>
    /// Note that the examples above used doc.GetTrailer() in order to access document 
    /// trailer, the starting SDF object (root node) in every document. Following the trailer 
    /// links, it is possible to visit all low-level objects in a document (e.g. all pages, 
    /// outlines, fonts, etc). 
    /// 
    /// SDFDoc also provides utility methods used to import objects and object collections 
    /// from one document to another. These methods can be useful for copy operations between 
    /// documents such as a high-level page merge and document assembly.
    /// </remarks>
    public class SDFDoc : IDisposable
    {
        internal TRN_SDFDoc mp_doc = IntPtr.Zero;

        private Object m_ref = null;
        
        private bool disposed = false;

        ~SDFDoc()
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
            if (!this.disposed)
            {
                // Check to see if Dispose has already been called. 
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Clean up native resources
                Destroy();
                this.disposed = true;
            }
        }
        public void Destroy()
        {
            if (mp_doc != IntPtr.Zero && m_ref == null)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocDestroy(mp_doc));
                mp_doc = IntPtr.Zero;
            }
        }

        public void ReleaseFileHandles()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocReleaseFileHandles(mp_doc));
        }
        internal SDFDoc(TRN_SDFDoc doc, Object reference)
        {
            this.mp_doc = doc;
            this.m_ref = reference;
        }
        internal IntPtr GetHandleInternal()
        {
            return this.mp_doc;
        }

        internal void SetRefHandleInternal(Object reference)
        {
            this.m_ref = reference;
        }

        internal Object GetRefHandleInternal()
        {
            return this.m_ref;
        }
        /// <summary> Default constructor. Creates a new document.
        /// The new document contains only trailer and Info dictionary.
        /// To build the rest of the document get document's root dictionary using GetTrailer() and
        /// populate it with new key/value pairs.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public SDFDoc()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreate(ref mp_doc));
            this.m_ref = null;
        }
        /// <summary> Open a SDF/Cos document from a Filter (i.e. a data stream) object.
        /// </summary>
        /// <param name="stream">- input stream containing a serialized document. The input stream may be a
        /// random-access file, memory buffer, slow HTTP connection etc.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks> if the input stream doesn't support Seek() operation the document will load whole 
        /// data stream in memory before parsing. In case of linearized PDF, the document may be parsed
        /// on-the-fly while it is being loaded in memory. Note that since StdFile implements Seek()
        /// interface, the document does not have to be fully in memory before it is used.
        /// </remarks>
        /// <remarks> Make sure to call InitStdSecurityHandler() or InitSecurityHandler() after SDFDoc(...) 
        /// in case the document is encrypted.
        /// </remarks>
        public SDFDoc(Filter stream)
        {
            stream.setRefHandleInternal(this);
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateFromFilter(stream.mp_imp, ref mp_doc));
            this.m_ref = null;
        }
        /// <summary> Open a SDF/Cos document from a PDFDoc object.
        /// </summary>
        /// <param name="pdfdoc"><c>PDFDoc</c> object containing <c>SDFDoc</c> object
        /// </param>
        public SDFDoc(PDFDoc pdfdoc)
        {
            mp_doc = pdfdoc.GetSDFDoc().mp_doc;
            this.m_ref = pdfdoc;
        }
        /// <summary> Open a SDF/Cos document from a file.
        /// 
        /// </summary>
        /// <param name="filepath">- path name to the file.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks> Make sure to call InitStdSecurityHandler() or InitSecurityHandler() after SDFDoc(...) 
        /// in case the document is encrypted.
        /// </remarks>
        public SDFDoc(string filepath)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateFromFileString(filepath, ref mp_doc));
            this.m_ref = null;
        }
        /// <summary> Instantiates a new <c>SDFDoc</c>.
        /// 
        /// </summary>
        /// <param name="buf">the buffer
        /// </param>
        /// <param name="buf_size">the size
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <exception cref="PDFNetException">  IOException Signals that an I/O exception has occurred. </exception>
        public SDFDoc(byte[] buf, int buf_size)
        {
            uint size = System.Convert.ToUInt32(buf_size);

            int psize = Marshal.SizeOf(buf[0]) * buf.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(buf, 0, pnt, buf.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateFromMemoryBuffer(pnt, new UIntPtr(size), ref mp_doc));
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }
		public SDFDoc(System.IO.Stream stream) 
		{
			if (stream.CanSeek)
			{
				int stream_size = (int)stream.Length;
				int bytesToRead = stream_size;
				byte[] buf = new byte[stream_size];
				int bytesRead = 0;
				while (bytesToRead > 0)
				{
					int? n = stream.Read(buf, bytesRead, bytesToRead);
					if (n == null)
						break;
					bytesRead += n.Value;
					bytesToRead -= n.Value;
				}
                int size = Marshal.SizeOf(buf[0]) * buf.Length;
                IntPtr pnt = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.Copy(buf, 0, pnt, buf.Length);
                    PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateFromMemoryBuffer(pnt, new UIntPtr(System.Convert.ToUInt32(stream_size)), ref mp_doc));
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(pnt);
                }				
			}
			else
			{
				// if we cannot seek, then we need to stream the data
				int bytesRead = 0;
				const int readSize = 0xFFFF; // 64KB
				byte[] readBuf = new byte[readSize];
				List<byte> dyArray = new List<byte>();

				bytesRead = stream.Read(readBuf, 0, readSize);
				while (bytesRead > 0)
				{
					foreach (byte b in readBuf)
					{
						dyArray.Add(b);
					}
					bytesRead = stream.Read(readBuf, 0, readSize);
				}
				byte[] buf = dyArray.ToArray();
				uint size = System.Convert.ToUInt32(buf.Length);

                int psize = Marshal.SizeOf(buf[0]) * buf.Length;
                IntPtr pnt = Marshal.AllocHGlobal(psize);
                try
                {
                    Marshal.Copy(buf, 0, pnt, buf.Length);
                    PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateFromMemoryBuffer(pnt, new UIntPtr(size), ref mp_doc));
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(pnt);
                }
			}
		}
        /// <summary>Explicit and implicit conversion operator from <c>PDFDoc</c> to <c>SDF::Doc</c>.
        /// </summary>
        /// <param name="pdfdoc"><c>PDFDoc</c> to convert to <c>SDFDoc</c>
        /// </param>
        /// <returns> document’s SDF/Cos document </returns>
        public static implicit operator SDFDoc(PDFDoc pdfdoc)
        {
            SDFDoc d = new SDFDoc(pdfdoc);
            return d;
        }

        /// <summary> Close.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Close()
        {
            Dispose(false);
        }
        /// <summary> Removes 'marked' flag from all objects in cross reference table.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void ClearMarks()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocClearMarks(mp_doc));
        }
        /// <summary> Creates the indirect array.
        /// 
        /// </summary>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectArray()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateIndirectArray(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Creates the indirect bool.
        /// 
        /// </summary>
        /// <param name="value">the value
        /// </param>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectBool(bool value)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateIndirectBool(mp_doc, value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Creates the indirect dict.
        /// 
        /// </summary>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectDict()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateIndirectDict(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> The following list of methods is used to create SDF/Cos indirect objects.
        /// 
        /// Unlike direct objects, indirect objects can be referenced by more than one
        /// object (i.e. indirect objects they can be shared).
        /// 
        /// </summary>
        /// <param name="name">the name
        /// </param>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectName(string name)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateIndirectName(mp_doc, name, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Creates the indirect null.
        /// 
        /// </summary>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectNull()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateIndirectNull(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Creates the indirect number.
        /// 
        /// </summary>
        /// <param name="value">the value
        /// </param>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectNumber(double value)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateIndirectNumber(mp_doc, value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Creates the indirect stream.
        /// 
        /// </summary>
        /// <param name="data">the data
        /// </param>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectStream(FilterReader data)
        {
            return CreateIndirectStream(data, null);
        }
        /// <summary> Creates the indirect stream.
        /// 
        /// </summary>
        /// <param name="buf">the data
        /// </param>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectStream(byte[] buf)
        {
            return CreateIndirectStream(buf, null);
        }
        /// <summary> Creates the indirect stream.
        /// 
        /// </summary>
        /// <param name="buf">the data
        /// </param>
        /// <param name="filter">the filter_chain
        /// </param>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectStream(byte[] buf, Filter filter)
        {
            if (filter != null)
            {
                filter.setRefHandleInternal(this);
            }
            TRN_Obj result = IntPtr.Zero;
            uint size = System.Convert.ToUInt32(buf.Length);

            int psize = Marshal.SizeOf(buf[0]) * buf.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(buf, 0, pnt, buf.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateIndirectStream(mp_doc, pnt, size, filter != null ? filter.mp_imp : IntPtr.Zero, ref result));
                return result != IntPtr.Zero ? new Obj(result, this) : null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }
        /// <summary> Creates the indirect stream.
        /// 
        /// </summary>
        /// <param name="data">the data
        /// </param>
        /// <param name="filter">the filter_chain
        /// </param>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectStream(FilterReader data, Filter filter)
        {
            if (filter != null)
            {
                filter.setRefHandleInternal(this);
            }
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateIndirectStreamFromFilter(mp_doc, data.mp_imp, filter != null ? filter.mp_imp : IntPtr.Zero, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Creates the indirect string.
        /// 
        /// </summary>
        /// <param name="value">the str
        /// </param>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectString(string value)
        {
            TRN_Obj result = IntPtr.Zero;
            UString str = new UString(value);
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateIndirectStringFromUString(mp_doc, str.mp_impl, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Creates the indirect string.
        /// 
        /// </summary>
        /// <param name="buf">the value
        /// </param>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectString(byte[] buf)
        {
            TRN_Obj result = IntPtr.Zero;
            uint size = System.Convert.ToUInt32(buf.Length);
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocCreateIndirectString(mp_doc, buf, size, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> If true, this PDFDoc will use a temporary file to cache
        /// new content streams (this is the default behavior).
        /// 
        /// </summary>
        /// <param name="use_cache">the use_cache
        /// </param>
        public void EnableDiskCaching(bool use_cache)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocEnableDiskCaching(mp_doc, use_cache));
        }
        /// <summary> Gets the file name.
        /// 
        /// </summary>
        /// <returns> The filename of the document if the document is loaded from disk,
        /// or empty string if the document is not yet saved or is loaded from a memory
        /// buffer.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetFileName()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocGetFileName(mp_doc, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
        /// <summary> Gets the PDF file header.</summary>
        /// <returns> the header string identifying the document version to which the file conforms.
        /// For a file conforming to PDF version 1.4, the header should be %PDF-1.4.
        /// In general header strings have the following syntax: %AAA-N.n where AAA identifies
        /// document specification (such as PDF, FDF, PJTF etc), N is the major version and
        /// n is the minor version. The new header string can be set during a full save (see SDFDoc.save()).
        /// For a document that is not serialized the function returns an empty string.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetHeader()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocGetHeader(mp_doc, ref result));
            return Marshal.PtrToStringUTF8(result);
        }
        /// <summary> Gets document's initial linearization hint stream if it is available.
        /// 
        /// </summary>
        /// <returns> - the linearization hint stream of the original document or NULL
        /// if the hint stream is not available.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetHintStream()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocGetHintStream(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Gets document's initial linearization dictionary if it is available.
        /// 
        /// </summary>
        /// <returns> - the linearization dictionary of the original document or NULL
        /// if the dictionary is not available.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetLinearizationDict()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocGetLinearizationDict(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Gets the obj.
        /// 
        /// </summary>
        /// <param name="obj_num">the obj_num
        /// </param>
        /// <returns> - the latest version of the object matching specified object number.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetObj(int obj_num) 
        {
            TRN_Obj result = IntPtr.Zero;
            uint num = System.Convert.ToUInt32(obj_num);
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocGetObj(mp_doc, num, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Gets the security handler.
        /// 
        /// </summary>
        /// <returns> Currently selected SecurityHandler.
        /// </returns>
        /// <remarks>  InitSecurityHandler() should be called before GetSecurityHandler() in order to initialize the handler.
        /// Returned security handler can be modified in order to change the security settings of the exisitng document. Changes to the current handler
        /// will not invalidate the access to the original file and will take effect
        /// during document Save().
        /// </remarks>
        /*public SecurityHandler GetSecurityHandler()
        {
            //TODO
        }*/
        /// <summary> Gets the trailer.
        /// 
        /// </summary>
        /// <returns> - A dictionary representing the root of the document (i.e.
        /// a document trailer dictionary)
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetTrailer() 
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocGetTrailer(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Import obj.
        /// 
        /// </summary>
        /// <param name="obj">- an object to import.
        /// </param>
        /// <param name="deep_copy">- a boolean indicating whether to perform a deep or shallow copy.
        /// In case of shallow copy all indirect references will be set to null.
        /// 
        /// If the object belongs to a document the function will perform deep or shallow
        /// clone depending whether deep_copy flag was specified.
        /// 
        /// If the object does not belong to any document ImportObj does not take the
        /// object ownership. ImportObj copies the source object and it is users
        /// responibility to delete free objects.
        /// </param>
        /// <returns> - a pointer to the root indirect object in this document
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj ImportObj(Obj obj, bool deep_copy)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocImportObj(mp_doc, obj.mp_obj, deep_copy, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }

        /// <summary> Initializes document's SecurityHandler. This version of InitSecurityHandler()
        /// works with Standard and Custom PDF security and can be used in situations where
        /// the password is obtained dynamically via user feedback. See EncTest sample for
        /// example code.
        /// 
        /// This function should be called immediately after an encrypted
        /// document is opened. The function does not have any side effects on
        /// documents that are not encrypted.
        /// 
        /// If the security handler was successfully initialized it can be later obtained
        /// using GetSecurityHandler() method.
        /// 
        /// </summary>
        /// <returns> true if the SecurityHandler was successfully initialized (this
        /// may include authentication data collection, verification etc.),
        /// false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool InitSecurityHandler() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocInitSecurityHandler(mp_doc, IntPtr.Zero, ref result));
            return result;
        }
        public bool InitStdSecurityHandler(string password, int password_sz)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocInitStdSecurityHandler(mp_doc, password, password_sz, ref result)); 
            return result;
        }

        /// <summary> Checks if is encrypted.
        /// 
        /// </summary>
        /// <returns> true if the document is/was originally encrypted false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsEncrypted()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocIsEncrypted(mp_doc, ref result));
            return result;
        }

        /// <summary> Checks if is full save required.
        /// 
        /// </summary>
        /// <returns> - true if the document requires full save.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsFullSaveRequired() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocIsFullSaveRequired(mp_doc, ref result));
            return result;
        }

        /// <summary> Call this function to determine whether the document is represented in
        /// linearized (fast web view) format.
        /// 
        /// </summary>
        /// <returns> - true if document is stored in fast web view format, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks> any changes to the document can invalidate linearization. The function will 
        /// return 'true' only if the original document is linearized and if it is not
        /// modified.
        /// 
        /// In order to provide good performance over relatively slow communication links,
        /// PDFNet can generate PDF documents with linearized objects and hint tables that
        /// can allow a PDF viewer application to download and view one page of a PDF file
        /// at a time, rather than requiring the entire file (including fonts and images) to
        /// be downloaded before any of it can be viewed.
        /// 
        /// To save a document in linearized (fast web view) format you only need to pass
        /// 'SDFDoc.e_linearized' flag in the Save method.
        /// </remarks>
        public bool IsLinearized()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocIsLinearized(mp_doc, ref result));
            return result;
        }

        /// <summary> Checks if is modified.
        /// 
        /// </summary>
        /// <returns> - true if document was modified, false otherwise
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsModified()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocIsModified(mp_doc, ref result));
            return result;
        }
        /// <summary> Checks whether or not the underlying file has an XRef table that had to be repaired
        /// when the file was opened.If the document had an invalid XRef table when opened,
        /// PDFNet will have repaired the XRef table for its working representation of the document.
        ///
        /// </summary>
        /// <returns> - true if document was found to be corrupted, and was repaired, during
        /// opening and has not been saved since.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>If this function returns true, it is not possible to incrementally save the document
        /// (see http://www.pdftron.com/kb_corrupt_xref)</remarks>
        public bool HasRepairedXRef()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocHasRepairedXRef(mp_doc, ref result));
            return result;
        }
        /// <summary> Locks the document to prevent competing threads from accessiong the document
        /// at the same time. Threads attempting to access the document will wait in
        /// suspended state until the thread that owns the lock calls doc.Unlock().
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Lock()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocLock(mp_doc));
        }
        /// <summary> Locks the document to prevent competing write threads (using Lock()) from accessing the document 
        /// at the same time. Other reader threads however, will be allowed to access the document.
        /// Threads attempting to obtain write access to the document will wait in 
        /// suspended state until the thread that owns the lock calls doc.UnlockRead().
        /// Note: To avoid deadlocks obtaining a write lock while holding
        /// a read lock is not permitted and will throw an exception. If this situation is encountered
        /// please either unlock the read lock before the write lock is obtained
        /// or acquire a write lock (rather than read lock) in the first place.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void LockRead()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocLockRead(mp_doc));
        }
        /// <summary>Saves the document to a memory buffer.</summary>
        /// <returns>buffer containing the serialized version of the document</returns> 
        /// <param name="flags">A bit field composed of an OR of <c>SDF.SDFDoc.SaveOptions</c>.</param>
        /// <param name="header">File header. A new file header is set only during full save.</param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>this method ignores e_incremental flag</remarks>
        /// <remarks> Saving modifies the SDFDoc's internal representation.  As such, the user should
        /// acquire a write lock before calling save. 
        /// If the original pdf has a corrupt xref table (see HasRepairedXref or
        /// http://www.pdftron.com/kb_corrupt_xref), then it can not be saved using the e_incremental flag.</remarks>
        public byte[] Save(SaveOptions flags, string header)
        {
            UIntPtr size = UIntPtr.Zero;
            IntPtr source = IntPtr.Zero;
            uint f = System.Convert.ToUInt32(flags);
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocSaveMemory(mp_doc, ref source, ref size, f, IntPtr.Zero, header));
            byte[] buf = new byte[size.ToUInt32()];
            System.Runtime.InteropServices.Marshal.Copy(source, buf, 0, System.Convert.ToInt32(size.ToUInt32()));
            return buf;
        }
        /// <summary>Saves the document to the given stream.</summary>
        /// <param name="stm">A stream where to serialize the document.</param>
        /// <param name="flags">A bit field composed of an OR of <c>SDF.SDFDoc.SaveOptions</c>.</param>
        /// <param name="header">File header. A new file header is set only during full save.</param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>this method ignores e_incremental flag</remarks>
        /// <remarks> Saving modifies the SDFDoc's internal representation.  As such, the user should
        /// acquire a write lock before calling save.
        /// If the original pdf has a corrupt xref table (see HasRepairedXref or
        /// http://www.pdftron.com/kb_corrupt_xref), then it can not be saved using the e_incremental flag.</remarks>
        public void Save(System.IO.Stream stm, SaveOptions flags, string header)
        {
            uint f = System.Convert.ToUInt32(flags);
            IntPtr source = IntPtr.Zero;
            UIntPtr buf_size = UIntPtr.Zero;

            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocSaveMemory(mp_doc, ref source, ref buf_size, f, IntPtr.Zero, header));

            byte[] buf = new byte[buf_size.ToUInt32()];
            System.Runtime.InteropServices.Marshal.Copy(source, buf, 0, System.Convert.ToInt32(buf_size));

            for (uint i = 0; i < buf_size.ToUInt32(); i++)
            {
                stm.WriteByte(buf[i]);
            }
            stm.Flush();
        }

        /// <summary> Saves the document to file</summary>
        /// <param name="path">path to save</param>
        /// <param name="flags">- A bit field composed of an OR of <c>SDF.SDFDoc.SaveOptions</c>.
        /// Note that this method ignores e_incremental flag.
        /// </param>		
        /// <param name="header">- File header. A new file header is set only during full save.
        /// </param>
        /// <returns> the byte[]
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks> Saving modifies the SDFDoc's internal representation.  As such, the user should
        /// acquire a write lock before calling save.
        /// If the original pdf has a corrupt xref table (see HasRepairedXref or
        /// http://www.pdftron.com/kb_corrupt_xref), then it can not be saved using the e_incremental flag.</remarks>
        public void Save(string path, SaveOptions flags, string header)
        {
            UString str = new UString(path);
            uint f = System.Convert.ToUInt32(flags);
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocSave(mp_doc, str.mp_impl, f, IntPtr.Zero, header));
        }
        /// <summary> Saves the document to a file.
        /// 
        /// If a full save is requested to the original path, the file is saved to a file
        /// system-determined temporary file, the old file is deleted, and the temporary file
        /// is renamed to path.
        /// 
        /// A full save with remove unused or linearization option may re-arrange object in
        /// the cross reference table. Therefore all pointers and references to document objects
        /// and resources should be re acquired in order to continue document editing.
        /// 
        /// In order to use incremental save the specified path must match original path and
        /// e_incremental flag bit should be set.
        /// 
        /// </summary>
        /// <param name="path">- The full path name to which the file is saved.</param>
        /// <param name="flags">- A bit field composed of an OR of <c>SDF.SDFDoc.SaveOptions</c>.</param>
        /// <param name="progress">- A pointer to the progress interface. NULL if progress tracking is not required.</param>
        /// <param name="header">- File header. A new file header is set only during full save.</param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks> Saving modifies the SDFDoc's internal representation.  As such, the user should
        /// acquire a write lock before calling save.
        /// If the original pdf has a corrupt xref table (see HasRepairedXref or
        /// http://www.pdftron.com/kb_corrupt_xref), then it can not be saved using the e_incremental flag.</remarks>
        public void Save(string path, SaveOptions flags, ProgressMonitor progress, string header)
        {
            UString str = new UString(path);
            uint f = System.Convert.ToUInt32(flags);
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocSave(mp_doc, str.mp_impl, f, progress.mp_imp, header));
        }
        /// <summary>Saves the document to the given filter.</summary>
        /// <param name="stm">A filter where to serialize the document.</param>
        /// <param name="flags">A bit field composed of an OR of <c>SDF.SDFDoc.SaveOptions</c>.</param>
        /// <param name="header">File header. A new file header is set only during full save.</param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>this method ignores e_incremental flag</remarks>
        /// <remarks> Saving modifies the SDFDoc's internal representation.  As such, the user should
        /// acquire a write lock before calling save.
        /// If the original pdf has a corrupt xref table (see HasRepairedXref or
        /// http://www.pdftron.com/kb_corrupt_xref), then it can not be saved using the e_incremental flag.</remarks>
        public void Save(Filter stm, SDFDoc.SaveOptions flags, string header)
        {
            uint f = System.Convert.ToUInt32(flags);
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocSaveStream(mp_doc, stm.mp_imp, f, header));
        }
        /// <summary> The function sets a new SecurityHandler as the current security handler.
        /// 
        /// </summary>
        /// <param name="handler">the new security handler
        /// </param>
        /// <remarks> Setting a new security handler will not invalidate the access to 
        /// the original file and will take effect during document Save().
        /// </remarks>
        /// <remarks> If the security handler is modified, document will perform a full save 
        /// even if e_incremental was given as a flag in Save() method.
        /// </remarks>
        public void SetSecurityHandler(SecurityHandler handler)
        {
            handler.m_ref = this;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocSetSecurityHandler(mp_doc, handler.mp_handler));
        }
        public void RemoveSecurity()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocRemoveSecurity(mp_doc));
        }
        /// <summary> Sometimes it is desirable to modify all indirect references to a given
        /// indirect object. It would be inefficient to manually search for all
        /// indirect references to a given indirect object.
        /// 
        /// A more efficient and less error prone method is to replace the indirect
        /// object in the cross reference table with a new object. This way the object
        /// that is referred to is modified (or replaced) and indirect references do
        /// not have to be changed.
        /// 
        /// </summary>
        /// <param name="obj_num1">the obj_num1
        /// </param>
        /// <param name="obj_num2">the obj_num2
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Swap(int obj_num1, int obj_num2)
        {
            uint num1 = System.Convert.ToUInt32(obj_num1);
            uint num2 = System.Convert.ToUInt32(obj_num2);
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocSwap(mp_doc, num1, num2));
        }

        /// <summary> Try locking the document, waiting no longer than specified number of milliseconds.
        /// 
        /// </summary>
        /// <param name="milliseconds">the milliseconds
        /// </param>	
        /// <returns> true if the document is locked for multi-threaded access, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool TimedLock(int milliseconds)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocTimedLock(mp_doc, milliseconds, ref result));
            return result;
        }

        /// <summary> Tries to obtain a read lock on the document, waiting no longer than specified number of milliseconds.
        /// 
        /// </summary>
        /// <param name="milliseconds">maxiumum number of milliseconds to wait for the lock
        /// </param>
        /// <returns> true if the document is locked for multi-threaded access, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool TimedLockRead(int milliseconds)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocTimedLockRead(mp_doc, milliseconds, ref result));
            return result;
        }

        /// <summary>Try locking the document
        /// </summary>
        /// <returns> true if the document is locked for multi-threaded access, false otherwise.
        /// </returns>
        public bool TryLock()
        {
            int milliseconds = 0;
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocTimedLock(mp_doc, milliseconds, ref result));
            return result;
        }

        /// <summary> Tries to obtain a read lock in a non-blocking manner.
        /// 
        /// </summary>
        /// <returns> true if the document is locked for multi-threaded access, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool TryLockRead()
        {
            int milliseconds = 0;
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocTimedLockRead(mp_doc, milliseconds, ref result));
            return result;
        }
        /// <summary> Removes the lock from the document.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Unlock()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocUnlock(mp_doc));
        }
        /// <summary> Removes the read lock from the document. 
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void UnlockRead()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocUnlockRead(mp_doc));
        }
        //std::list<Obj*> ImportObjs(const std::list<Obj*>& obj_list);
        /// <summary> X ref size.
        /// 
        /// </summary>
        /// <returns> - The size of cross reference table
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int XRefSize()
        {
            uint result = 0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocXRefSize(mp_doc, ref result));
            return System.Convert.ToInt32(result);
        }

        ///<summary> SDFDoc save options </summary>
        [Flags]
        public enum SaveOptions
        {
            ///<summary> save document in incremental mode.</summary>
            e_incremental = 0x01,
            ///<summary> remove unused objects (requires full save)</summary>
            e_remove_unused = 0x02,
            ///<summary> save all string in hexadecimal format.</summary>
            e_hex_strings = 0x04,
            ///<summary> do not save cross-reference table</summary>
            e_omit_xref = 0x08,
            ///<summary> Save the document in linearized (fast web-view) format. Requires full save.</summary>
            e_linearized = 0x10,
            ///<summary> Save the document in a manner that maximizes compatibility with older PDF consumers (e.g. the file will not use object and compressed xref streams).</summary>
            e_compatibility = 0x20    
        }

    }
}
