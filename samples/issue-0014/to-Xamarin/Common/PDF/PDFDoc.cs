using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.Filters;
using pdftron.SDF;
using pdftron.FDF;

using TRN_PDFDoc = System.IntPtr;
using TRN_PDFDocInfo = System.IntPtr;
using TRN_UString = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_PDFView = System.IntPtr;
using TRN_Filter = System.IntPtr;
using TRN_SDFDoc = System.IntPtr;
using TRN_SecurityHandler = System.IntPtr;
using TRN_SignatureHandler = System.IntPtr;
using TRN_ProgressMonitor = System.IntPtr;
using TRN_Page = System.IntPtr;
using TRN_Iterator = System.IntPtr;
using TRN_PageSet = System.IntPtr;
using TRN_Bookmark = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_FDFDoc = System.IntPtr;
using TRN_Action = System.IntPtr;
using TRN_FileSpec = System.IntPtr;
using TRN_FilterReader = System.IntPtr;
using TRN_PDFDocViewPrefs = System.IntPtr;
using TRN_STree = System.IntPtr;
using TRN_OCGConfig = System.IntPtr;
using TRN_UndoManager = System.IntPtr;
using TRN_DigitalSignatureField = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> PDFDoc is a high-level class describing a single PDF (Portable Document 
    /// Format) document. Most applications using PDFNet will use this class to 
    /// open existing PDF documents, or to create new PDF documents from scratch.
    /// 
    /// The class offers a number of entry points into the document. For example,
    /// <list type="bullet">	
    /// <item><description>To access pages use pdfdoc.getPageIterator() or pdfdoc.PageFind(page_num).</description></item>
    /// <item><description>To access form fields use pdfdoc.GetFieldIterator() or pdfdoc.FieldFind(name).</description></item>
    /// <item><description>To access document's meta-data use pdfdoc.GetDocInfo().</description></item>
    /// <item><description>To access the outline tree use pdfdoc.GetFirstBookmark().</description></item>
    /// <item><description>To access low-level Document Catalog use pdfdoc.GetRoot().</description></item>	
    /// </list>
    /// The class also offers utility methods to slit and merge PDF pages, 
    /// to create new pages, to flatten forms, to change security settings, etc.</summary>
    public class PDFDoc : IDisposable
    {
        // Fields
        internal TRN_PDFDoc mp_doc = IntPtr.Zero;
        internal GCHandle m_gch; // used by signature handler, keep alive

        private bool disposed = false;

        ~PDFDoc()
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
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.

                }

                // Clean up native resources
                Destroy();
                if (m_gch.IsAllocated)
                {
                    m_gch.Free();
                }
                disposed = true;
            }
        }
        public void Destroy()
        {
            if (mp_doc != IntPtr.Zero)
            {
                //TRN_SDFDoc result = IntPtr.Zero;
                //PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetSDFDoc(mp_doc, ref result));
                //PDFNetException.REX(PDFNetPINVOKE.TRN_SDFDocReleaseFileHandles(result));
                PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocDestroy(mp_doc));
                mp_doc = IntPtr.Zero;
            }
        }

        public IntPtr GetHandleInternal()
        {
            return this.mp_doc;
        }

        // Methods
        internal PDFDoc(TRN_PDFDoc doc)
        {
            this.mp_doc = doc;
        }

        public static PDFDoc CreateInternal(TRN_PDFDoc imp)
        {
            return new PDFDoc(imp);
        }

        /// <summary> Default constructor. Creates an empty new document.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PDFDoc()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreate(ref mp_doc));
        }
        /// <summary> Create a PDF document from an existing SDF/Cos document.
        /// 
        /// </summary>
        /// <param name="sdfdoc">a pointer to the SDF document. Created PDFDoc will
        /// take the ownership of the low-level document.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  the SDFDoc will become invalid.  If you would
        /// like to access the low level document use GetSDFDoc</remarks>
        public PDFDoc(SDFDoc sdfdoc)
        {
            if (sdfdoc.GetRefHandleInternal() != null)
            {
                throw new PDFNetException("PDFNetException", "PDFDoc.cs", 0, "PDFDoc(SDFDoc)", "SDFDoc is already owned by another document.", PDFNetException.ErrorCodes.e_error_general);
            }

            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateFromSDFDoc(sdfdoc.mp_doc, ref mp_doc));
            sdfdoc.SetRefHandleInternal(this);
        }

        /// <summary> Open an existing PDF document.
	    /// 
	    /// </summary>
	    /// <param name="stream">- input stream containing a serialized document. The input stream may be a
	    /// random-access file, memory buffer, slow HTTP connection etc.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public PDFDoc(System.IO.Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }
                stream.CopyTo(ms);

                int length = (int)ms.Length;
                IntPtr memIntPtr = Marshal.AllocHGlobal(length);
                UnmanagedMemoryStream writeStream = null;

                unsafe
                {
                    try
                    {
                        byte* memBytePtr = (byte*)memIntPtr.ToPointer();
                        writeStream = new UnmanagedMemoryStream(memBytePtr, length, length, FileAccess.Write);
                        writeStream.Write(ms.ToArray(), 0, length);
                        PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateFromBuffer(memIntPtr, new UIntPtr(System.Convert.ToUInt32(length)), ref mp_doc));
                    }
                    finally
                    {
                        if (writeStream != null)
                        {
                            writeStream.Close();
                        }
                        Marshal.FreeHGlobal(memIntPtr);
                    }
                }
            }
        }
        private static byte[] ReadFully(System.IO.Stream input)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                if (input.CanSeek)
                {
                    input.Position = 0;
                }
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
        /// <summary> Open an existing PDF document.
        /// 
        /// </summary>
        /// <param name="stream">- input stream containing a serialized document. The input stream may be a
        /// random-access file, memory buffer, slow HTTP connection etc.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  if the input stream doesn't support <c>Seek()</c> operation the document will load whole  data stream in memory before parsing. 
        /// In case of linearized PDF, the document may be parsed on-the-fly while it is being loaded in memory. 
        /// Note that since StdFile implements <c>Seek()</c> interface, the document does not have to be fully in memory before it is used.		
        /// Make sure to call <c>InitSecurityHandler()</c> after <c>PDFDoc(...)</c> for encrypted documents. </remarks>
        public PDFDoc(Filter stream)
        {
            stream.setRefHandleInternal(this);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateFromFilter(stream.mp_imp, ref mp_doc));
        }
        /// <summary> Open an existing PDF document.
        /// 
        /// </summary>
        /// <param name="filepath">- pathname to the file.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Make sure to call InitSecurityHandler() after PDFDoc(...) in case 
        /// a document is encrypted</remarks>
        public PDFDoc(string filepath)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateFromUFilePath(UString.ConvertToUString(filepath).mp_impl, ref mp_doc));
        }
        /// <summary> Open a SDF/Cos document from a memory buffer.
        /// 
        /// </summary>
        /// <param name="buf">a memory buffer containing the serialized document
        /// </param>
        /// <param name="buf_size">buffer size
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  the document should be fully loaded in the memory buffer. </remarks>
        public PDFDoc(byte[] buf, int buf_size)
        {
            int psize = Marshal.SizeOf(buf[0]) * buf.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(buf, 0, pnt, buf.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateFromBuffer(pnt, new UIntPtr(System.Convert.ToUInt32(buf_size)), ref mp_doc));
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }

        /// <summary> Closes PDFDoc and release resources		
        /// </summary>
        public void Close()
        {
            Dispose(false);
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocIsEncrypted(mp_doc, ref result));
            return result;
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInitSecurityHandler(mp_doc, IntPtr.Zero, ref result));
            return result;
        }
        /// <summary> Inits the security handler.
        /// 
        /// </summary>
        /// <param name="custom_data">the custom_data
        /// </param>
        /// <returns> true, if successful
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool InitSecurityHandler(int custom_data)
        {
            bool result = false;
            IntPtr ptr = new IntPtr(custom_data);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInitSecurityHandler(mp_doc, ptr, ref result));
            return result;
        }

        /// <summary> Initializes document's SecurityHandler using the supplied
        /// password. This version of InitSecurityHandler() assumes that
        /// document uses Standard security and that a password is specified
        /// directly.
        /// 
        /// This function should be called immediately after an encrypted
        /// document is opened. The function does not have any side effects on
        /// documents that are not encrypted.
        /// 
        /// If the security handler was successfully initialized, it can be later
        /// obtained using GetSecurityHandler() method.
        /// 
        /// </summary>
        /// <param name="password">Specifies the password used to open the document without
        /// any user feedback. If you would like to dynamically obtain the password,
        /// you need to derive a custom class from StdSecurityHandler() and use
        /// InitSecurityHandler() without any parameters. See EncTest sample
        /// for example code.
        /// </param>
        /// <returns> true if the given password successfully unlocked the document,
        /// false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool InitStdSecurityHandler(string password)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInitStdSecurityHandlerUString(mp_doc, UString.ConvertToUString(password).mp_impl, ref result));
            return result;
        }

        public bool InitStdSecurityHandler(byte[] passwordBuffer)
        {
            bool result = false;
            int size = Marshal.SizeOf(passwordBuffer[0]) * passwordBuffer.Length;
            IntPtr pnt = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(passwordBuffer, 0, pnt, passwordBuffer.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInitStdSecurityHandlerBuffer(mp_doc, pnt, new UIntPtr(System.Convert.ToUInt32(passwordBuffer.Length)), ref result));
                return result;
            }
            finally
            {
                Marshal.FreeHGlobal(pnt);
            }
        }

        /// <summary> Gets the security handler.
        /// 
        /// </summary>
        /// <returns> Currently selected SecurityHandler.
        /// </returns>
        /// <remarks>  InitSecurityHandler() should be called before GetSecurityHandler()
        /// in order to initialize the handler.
        /// 
        /// Returned security handler can be modified in order to change the
        /// security settings of the existing document. Changes to the current handler
        /// will not invalidate the access to the original file and will take effect
        /// during document Save().
        /// 
        /// If the security handler is modified, document will perform a full save
        /// even if e_incremental was given as a flag in Save() method.</remarks>
        public SecurityHandler GetSecurityHandler()
        {
            TRN_SecurityHandler result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetSecurityHandler(mp_doc, ref result));
            return result != IntPtr.Zero ? new SecurityHandler(result, false) : null;
        }
        /// <summary> The function sets a new SecurityHandler as the current security handler.
        /// 
        /// </summary>
        /// <param name="handler">the new security handler
        /// </param>
        /// <remarks>  Setting a new security handler will not invalidate the access to
        /// the original file and will take effect during document Save().
        /// 
        /// If the security handler is modified, document will perform a full save
        /// even if e_incremental was given as a flag in Save() method.</remarks>
        public void SetSecurityHandler(SecurityHandler handler)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocSetSecurityHandler(mp_doc, handler.mp_handler));
            handler.SetRefHandleInternal(this);
        }

        /// <summary> Removes all security for the document.</summary>
        public void RemoveSecurity()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocRemoveSecurity(mp_doc));
        }

        /// <summary> Indicates whether this documents contains any digital signatures.
        /// </summary>
        /// <returns> True if a digital signature is found in this PDFDoc.
        /// </returns>
        public bool HasSignatures()
        {
            bool hasSig = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocHasSignatures(mp_doc, ref hasSig));
            return hasSig;
        }

        /// <summary> Adds a signature handler to the signature manager.
        /// </summary>
        /// <param name="signature_handler"> The signature handler instance to add to the signature manager.
        /// </param>
        /// <returns> A unique ID representing the SignatureHandler within the SignatureManager.
        /// </returns>
        public SignatureHandlerId AddSignatureHandler(SignatureHandler signature_handler)
        {
            TRN_SignatureHandler sigHandler = IntPtr.Zero;

            m_gch = GCHandle.Alloc(signature_handler);
            IntPtr unused = GCHandle.ToIntPtr(m_gch);
            PDFNetException.REX(PDFNetPINVOKE.TRN_SignatureHandlerCreate(signature_handler.TRN_SignatureHandlerGetNameFunction,
                signature_handler.TRN_SignatureHandlerAppendDataFunction,
                signature_handler.TRN_SignatureHandlerResetFunction,
                signature_handler.TRN_SignatureHandlerCreateSignatureFunction,
                signature_handler.TRN_SignatureHandlerDestructorFunction, unused, ref sigHandler));

            if (sigHandler == IntPtr.Zero)
            {
                PDFNetException exception = new PDFNetException("Error", "PDFDoc.cs", 0, "PDFDoc.AddSignatureHandler", "Failed to add SignatureHandler.", PDFNetException.ErrorCodes.e_error_general);
                throw exception;
            }

            //signature_handler.mp_imp = sigHandler;
            GC.KeepAlive(signature_handler);
            GC.KeepAlive(m_gch);
            SignatureHandlerId result = UIntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocAddSignatureHandler(mp_doc, sigHandler, ref result));
            return result;
        }

        public SignatureHandlerId AddStdSignatureHandler(string pkcs12_keyfile, string pkcs12_keypass)
        {
            SignatureHandlerId result = UIntPtr.Zero;
            UString str1 = new UString(pkcs12_keyfile);
            UString str2 = new UString(pkcs12_keypass);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocAddStdSignatureHandlerFromFile(mp_doc, str1.mp_impl, str2.mp_impl, ref result));
            return result;
        }

        public SignatureHandlerId AddStdSignatureHandler(byte[] pkcs12_keybuffer, string pkcs12_keypass)
        {
            SignatureHandlerId result = UIntPtr.Zero;
            UString str2 = new UString(pkcs12_keypass);
            int size = Marshal.SizeOf(pkcs12_keybuffer[0]) * pkcs12_keybuffer.Length;
            IntPtr pnt = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(pkcs12_keybuffer, 0, pnt, pkcs12_keybuffer.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocAddStdSignatureHandlerFromBuffer(mp_doc, pnt, new UIntPtr(System.Convert.ToUInt32(pkcs12_keybuffer.Length)), str2.mp_impl, ref result));
                return result;
            }
            finally
            {
                Marshal.FreeHGlobal(pnt);
            }
        }

        /// <summary> Removes a signature handler from the signature manager.
        /// </summary>
        /// <param name="signature_handler_id"> The unique id of the signature handler to remove.
        /// </param>
        public void RemoveSignatureHandler(SignatureHandlerId signature_handler_id)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocRemoveSignatureHandler(mp_doc, signature_handler_id));
        }

        /// <summary> Gets the associated signature handler instance from the signature manager by looking it up with the
        /// handler name.
        /// </summary>
        /// <param name="signature_handler_id"> The unique id of the signature handler to get.
        /// </param>
        /// <returns> The signature handler instance if found, otherwise null.
        /// </returns>
        public SignatureHandler GetSignatureHandler(SignatureHandlerId signature_handler_id)
        {
            TRN_SignatureHandler sigHandler = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetSignatureHandler(mp_doc, signature_handler_id, ref sigHandler));
            if (sigHandler == IntPtr.Zero)
                return null;
            TRN_SignatureHandler result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SignatureHandlerGetUserImpl(sigHandler, ref result));
            if (result != IntPtr.Zero)
            {
                GCHandle gch = GCHandle.FromIntPtr(result);
                object tw = gch.Target;
                if (tw is SignatureHandler)
                    return tw as SignatureHandler;
                return null;
            }
            else
                return null;
        }

        /// <returns> The UndoManager object (one-to-one mapped to document)
        /// </returns>
        public UndoManager GetUndoManager()
        {
            TRN_UndoManager um = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetUndoManager(mp_doc, ref um));
            return new UndoManager(um);
        }

        /// <summary>Creates an unsigned digital signature form field inside the document.
        /// </summary>
        /// <param name="in_sig_field_name">The fully-qualified name to give the digital signature field. If one is not provided, a unique name is created automatically.
        /// </param>
        /// <returns>A DigitalSignatureField object representing the created digital signature field.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public DigitalSignatureField CreateDigitalSignatureField(String in_sig_field_name)
        {
            BasicTypes.TRN_DigitalSignatureField result = new BasicTypes.TRN_DigitalSignatureField();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateDigitalSignatureField(mp_doc, UString.ConvertToUString(in_sig_field_name).mp_impl, ref result));
            return result.mp_field_dict_obj != IntPtr.Zero ? new DigitalSignatureField(result, this) : null;
        }

        /// <summary>Creates an unsigned digital signature form field inside the document.
        /// </summary>
        /// <returns>A DigitalSignatureField object representing the created digital signature field.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public DigitalSignatureField CreateDigitalSignatureField()
        {
            return CreateDigitalSignatureField("");
        }

        /// <summary>Retrieves an iterator that iterates over digital signature fields.</summary>
        /// <returns>An iterator that iterates over digital signature fields.</returns>
        public DigitalSignatureFieldIterator GetDigitalSignatureFieldIterator()
        {
            TRN_Iterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetDigitalSignatureFieldBegin(mp_doc, ref result));
            return new DigitalSignatureFieldIterator(result);
        }

        /// <summary>Retrieves the most restrictive document permissions locking level from all of the signed digital signatures in the document.</summary>
        /// <returns>An enumerated value representing the most restrictive document permission level found in the document.</returns>
        public DigitalSignatureField.DocumentPermissions GetDigitalSignaturePermissions()
        {
            var tmp = DigitalSignatureField.DocumentPermissions.e_unrestricted;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetDigitalSignaturePermissions(mp_doc, ref tmp));
            return tmp;
        }


        /// <summary>
        /// Generates thumbnail images for all the pages in this PDF document.
        /// </summary>
        /// <param name="size">The maximum dimension (width or height) that thumbnails will have.</param>
        public void GenerateThumbnails(UInt32 size)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGenerateThumbnails(mp_doc, size)); 
		}

        /// <summary> Generates a PDF diff of the given pages by overlaying and blending them on top of each other, 
        /// then appends that diff as a new page in this document.
        /// </summary>    
        /// <param name="pageA">The first page to compare. Must be from another document.</param>
        /// <param name="pageB">The second page to compare. Must be from another document.</param>
        /// <param name="options">The options to use when comparing the page.</param>
        public void AppendVisualDiff(Page pageA, Page pageB, DiffOptions options)
        {
            TRN_Obj opt_ptr = (options != null) ? options.GetInternalObj().mp_obj : IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocAppendVisualDiffWithOptsObj(mp_doc, pageA.mp_page, pageB.mp_page, opt_ptr));
        }

        /// <summary> Call this function to determine whether the document has been modified since
        /// it was last saved.
        /// 
        /// </summary>
        /// <returns> - true if document was modified, false otherwise
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsModified()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocIsModified(mp_doc, ref result)); 
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocHasRepairedXRef(mp_doc, ref result));
            return result;
        }

        /// <summary> Call this function to determine whether the document is represented in
        /// linearized (fast web view) format.
        /// 
        /// </summary>
        /// <returns> - true if document is stored in fast web view format, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  any changes to the document can invalidate linearization. The function will
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
        /// 'Doc.SaveOptions.e_linearized' flag in the Save method.
        /// </remarks>
        public bool IsLinearized()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocIsLinearized(mp_doc, ref result)); 
            return result;
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
        /// <param name="path">- The full path name to which the file is saved.
        /// </param>
        /// <param name="flags">- A bit field composed of an OR of Doc::SaveOptions values.
        /// </param>		
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks> Saving modifies the PDFDoc's internal representation.  As such, the user should
        /// acquire a write lock before calling save. 
        /// If the original pdf has a corrupt xref table (see HasRepairedXref or
        /// http://www.pdftron.com/kb_corrupt_xref), then it can not be saved using the e_incremental flag.</remarks>
        public void Save(string path, SDFDoc.SaveOptions flags)
        {
            UString str = new UString(path);
            uint f = System.Convert.ToUInt32(flags);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocSave(mp_doc, str.mp_impl, f));
        }

#if (__ANDROID__)
        public static PDFDoc CreateFromAndroidFilter(pdftronprivate.Filters.SecondaryFileFilter fileFilter)
        {
            pdftronprivate.PDF.PDFDoc pdfdoc = new pdftronprivate.PDF.PDFDoc(fileFilter);
            return TypeConvertHelper.ConvPdfDocToManaged(pdfdoc);
        }

        /// <summary>
        /// Can be used only if PDFDoc is created from PDFDoc.CreateFromAndroidFilter API.
        /// </summary>
        public void Save()
        {
            pdftronprivate.PDF.PDFDoc pdfdoc = TypeConvertHelper.ConvPDFDocToNative(this);
            pdfdoc.Save();
        }
#endif

        /// <summary>Saves the document to a memory buffer.</summary>
        /// <returns>Byte array containing the serialized version of the document</returns>
        /// <param name="flags">A bit field composed of an OR of <c>SDF.SDFDoc.SaveOptions</c> values.</param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>this method ignores e_incremental flag</remarks>
        /// <remarks> Saving modifies the PDFDoc's internal representation.  As such, the user should
        /// acquire a write lock before calling save. 
        /// If the original pdf has a corrupt xref table (see HasRepairedXref or
        /// http://www.pdftron.com/kb_corrupt_xref), then it can not be saved using the e_incremental flag.</remarks>
        public byte[] Save(SDFDoc.SaveOptions flags)
        {
            UIntPtr size;
            IntPtr source;
            uint f = System.Convert.ToUInt32(flags);
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocSaveMemoryBuffer(mp_doc, f, out source, out size));
            
			unsafe
			{
				byte* memBytePtr = (byte*)source.ToPointer();
				using (System.IO.UnmanagedMemoryStream unmanagedStm = new System.IO.UnmanagedMemoryStream(memBytePtr, size.ToUInt32()))
				{
					return ReadFully(unmanagedStm);
				}
			}
        }

        /// <summary>Saves the document to a Stream.</summary>
        /// <param name="stm">A stream where to serialize the document.</param>
        /// <param name="flags">A bit field composed of an OR of <c>SDF.SDFDoc.SaveOptions</c> values.</param>
        /// <exception cref="PDFNetException">PDFNetException the PDFNet exception</exception>
        /// <remarks> Saving modifies the PDFDoc's internal representation.  As such, the user should
        /// acquire a write lock before calling save.
        /// If the original pdf has a corrupt xref table (see HasRepairedXref or
        /// http://www.pdftron.com/kb_corrupt_xref), then it can not be saved using the e_incremental flag.</remarks>
        public void Save(System.IO.Stream stm, SDFDoc.SaveOptions flags)
        {
            uint f = System.Convert.ToUInt32(flags);
			IntPtr source;
			UIntPtr buf_size;
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocSaveMemoryBuffer(mp_doc, f, out source, out buf_size));

			unsafe
			{
				byte* memBytePtr = (byte*)source.ToPointer();
				using (System.IO.UnmanagedMemoryStream unmanagedStm = new System.IO.UnmanagedMemoryStream(memBytePtr, buf_size.ToUInt32()))
				{
					if (stm.CanSeek)
					{
						stm.Position = 0;
					}
					if (unmanagedStm.CanSeek)
					{
						unmanagedStm.Position = 0;
					}
					unmanagedStm.CopyTo(stm);
				}
			}
		}

        /// <summary>Saves the document to a Filter.</summary>
        /// <param name="stm">A filter  where to serialize the document.</param>
        /// <param name="flags">A bit field composed of an OR of <c>SDF.SDFDoc.SaveOptions</c> values.</param>
        /// <exception cref="PDFNetException">PDFNetException the PDFNet exception</exception>
        /// <remarks> Saving modifies the PDFDoc's internal representation.  As such, the user should
        /// acquire a write lock before calling save.
        /// If the original pdf has a corrupt xref table (see HasRepairedXref or
        /// http://www.pdftron.com/kb_corrupt_xref), then it can not be saved using the e_incremental flag.</remarks>
        public void Save(Filter stm, SDFDoc.SaveOptions flags)
        {
            uint f = System.Convert.ToUInt32(flags);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocSaveStream(mp_doc, stm.mp_imp, f));
        }

        /// <summary> Gets the page iterator.
        /// 
        /// </summary>
        /// <returns> an iterator to the first page in the document.
        /// Use the <c>Next()</c> method on the returned iterator to traverse all pages in the document.
        /// For example:
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <example>
        /// For full sample code, please take a look at ElementReader, PDFPageTest and PDFDraw sample projects.
        /// <code>  
        /// PageIterator itr = pdfdoc.getPageIterator();
        /// while (itr.hasNext()) { //  Read every page
        /// Page page = itr.current();
        /// // ...
        /// itr.next()
        /// }
        /// </code>
        /// </example>
        public PageIterator GetPageIterator()
        {
            return GetPageIterator(1);
        }

        /// <summary> Gets the page iterator.
        /// 
        /// </summary>
        /// <param name="page_num">the page_number
        /// </param>
        /// <returns> the page iterator
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PageIterator GetPageIterator(int page_num)
        {
            TRN_Iterator result = IntPtr.Zero;
            uint num = System.Convert.ToUInt32(page_num);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetPageIterator(mp_doc, num, ref result));
            return new PageIterator(result, this);
        }

        /// <summary> Gets the page.
        /// 
        /// </summary>
        /// <param name="page_num">- the page number in document's page sequence. Page numbers
        /// in document's page sequence are indexed from 1.
        /// </param>
        /// <returns> a Page corresponding to a given page number, or null (invalid page)
        /// if the document does not contain the given page number.				
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <example>
        /// <code>  
        /// Page page = pdfdoc.GetPage(page_num);
        /// if (page == null) return; //  Page not found
        /// </code>
        /// </example>
        public Page GetPage(int page_num)
        {
            TRN_Page result = IntPtr.Zero;
            uint num = System.Convert.ToUInt32(page_num);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetPage(mp_doc, num, ref result));
            return result != IntPtr.Zero ? new Page(result, this) : null;
        }

        /// <summary> Insert/Import a single page at a specific location in the page sequence.
        /// 
        /// </summary>
        /// <param name="where">- The location in the page sequence indicating where to insert
        /// the page. The page is inserted before the specified location.
        /// </param>
        /// <param name="page">- A page to insert.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Invalidates all PageIterators pointing to the document. </remarks>
        public void PageInsert(PageIterator where, Page page)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocPageInsert(mp_doc, where.mp_impl, page != null ? page.mp_page : IntPtr.Zero));
        }

        /// <summary> Page remove.
        /// 
        /// </summary>
        /// <param name="page_itr">- the PageIterator to the page that should be removed
        /// A PageIterator for the given page can be obtained using PDFDoc::Find(page_num)
        /// or using direct iteration through document's page sequence.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void PageRemove(PageIterator page_itr)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocPageRemove(mp_doc, page_itr.mp_impl));
        }

        /// <summary>Inserts a range of pages from specified PDFDoc using PageSet
        /// </summary>
        ///
        /// <param name="insertBeforeThisPage">the destination of the insertion. If less than or equal to 1, 
        /// the pages are added to the beginning of the document. If larger than the number of pages 
        /// in the destination document, the pages are appended to the document.
        /// </param>
        /// <param name="sourceDoc">source PDFDoc to insert from</param>
        /// <param name="sourcePageSet">a collection of the page number to insert</param>			
        /// <param name="flag">specifies insert options</param>			
        public void InsertPages(int insertBeforeThisPage, PDFDoc sourceDoc, PageSet sourcePageSet, InsertFlag flag)
        {
            uint num = System.Convert.ToUInt32(insertBeforeThisPage);
            uint f = System.Convert.ToUInt32(flag);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInsertPageSet(mp_doc, num, sourceDoc.mp_doc, sourcePageSet.mp_imp, f, IntPtr.Zero));
        }
        /// <summary>Inserts a range of pages from specified PDFDoc
        /// </summary>
        ///
        /// <param name="insertBeforeThisPage">the destination of the insertion. If less than or equal to 1, 
        /// the pages are added to the beginning of the document. If larger than the number of pages 
        /// in the destination document, the pages are appended to the document.
        /// </param>
        /// <param name="sourceDoc">source PDFDoc to insert from</param>
        /// <param name="startPage">start of the page number to insert</param>
        /// <param name="endPage">end of the page number to insert</param>
        /// <param name="flag">specifies insert options</param>			
        public void InsertPages(int insertBeforeThisPage, PDFDoc sourceDoc, int startPage, int endPage, InsertFlag flag)
        {
            uint f = System.Convert.ToUInt32(flag);
            uint inserBefore = System.Convert.ToUInt32(insertBeforeThisPage);
            uint start = System.Convert.ToUInt32(startPage);
            uint end = System.Convert.ToUInt32(endPage);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInsertPages(mp_doc, inserBefore, sourceDoc.mp_doc, start, end, f, IntPtr.Zero));
        }

        /// <summary>Moves a range of pages from specified PDFDoc. Pages are deleted from source document after move.
        /// </summary>
        ///
        /// <param name="moveBeforeThisPage">the destination of the move. If less than or equal to 1, 
        /// the pages are moved to the beginning of the document. If larger than the number of pages 
        /// in the destination document, the pages are moved to the end of the document.
        /// </param>
        /// <param name="sourceDoc">source PDFDoc to move from</param>
        /// <param name="sourcePageSet">a collection of the page number to move</param>
        /// <param name="flag">specifies insert options</param>			
        /// <remarks>MovePages function does not save sourceDoc. It merely delete pages in memeory. For permanent changes,
        /// PDFDoc::Save should be used to save sourceDoc after function exists.</remarks>
        public void MovePages(int moveBeforeThisPage, PDFDoc sourceDoc, PageSet sourcePageSet, InsertFlag flag)
        {
            uint moveBefore = System.Convert.ToUInt32(moveBeforeThisPage);
            uint f = System.Convert.ToUInt32(flag);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocMovePageSet(mp_doc, moveBefore, sourceDoc.mp_doc, sourcePageSet.mp_imp, f, IntPtr.Zero));
        }
        /// <summary>Moves a range of pages from specified PDFDoc. Pages are deleted from source document after move.
        /// </summary>
        ///
        /// <param name="moveBeforeThisPage">the destination of the move. If less than or equal to 1, 
        /// the pages are moved to the beginning of the document. If larger than the number of pages 
        /// in the destination document, the pages are moved to the end of the document.
        /// </param>
        /// <param name="sourceDoc">source PDFDoc to move from</param>
        /// <param name="startPage">start of the page number to move</param>
        /// <param name="endPage">end of the page number to move</param>
        /// <param name="flag">specifies insert options</param>			
        /// <remarks>MovePages function does not save sourceDoc. It merely delete pages in memeory. For permanent changes,
        /// PDFDoc::Save should be used to save sourceDoc after function exists.</remarks>
        public void MovePages(int moveBeforeThisPage, PDFDoc sourceDoc, int startPage, int endPage, InsertFlag flag)
        {
            uint moveBefore = System.Convert.ToUInt32(moveBeforeThisPage);
            uint f = System.Convert.ToUInt32(flag);
            uint start = System.Convert.ToUInt32(startPage);
            uint end = System.Convert.ToUInt32(endPage);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocMovePages(mp_doc, moveBefore, sourceDoc.mp_doc, start, end, f, IntPtr.Zero));
        }

        /// <summary> Adds a page to the beginning of a documents's page sequence.
        /// 
        /// </summary>
        /// <param name="page">- a page to prepend to the document
        /// Invalidates all PageIterators pointing to the document.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void PagePushFront(Page page)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocPagePushFront(mp_doc, page.mp_page));
        }

        /// <summary> Adds a page to the end of a documents's page sequence.
        /// 
        /// </summary>
        /// <param name="page">- a page to append to the document
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Invalidates all PageIterators pointing to the document. </remarks>
        public void PagePushBack(Page page)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocPagePushBack(mp_doc, page.mp_page));
        }

        /// <summary> The function imports a list of pages to this documents. Although a list of pages
        /// can be imported using repeated calls to PageInsert(), PageImport will not import
        /// duplicate copies of resources that are shared across pages (such as fonts, images,
        /// colorspaces etc). Therefore this method is recommended when a page import list
        /// consists of several pages that share the same resources.
        /// 
        /// </summary>
        /// <param name="pages">A list of pages to import. All pages should belong to the same source document.
        /// </param>
        /// <returns> a list of imported pages. Note that imported pages are not placed in the
        /// document page sequence. This can be done using methods such as PageInsert(),
        /// PagePushBack(), etc.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ArrayList ImportPages(ArrayList pages)
        {
            return ImportPages(pages, false);
        }
        /// <summary> Import pages.
        /// 
        /// </summary>
        /// <param name="pages">the pages
        /// </param>
        /// <param name="import_bookmarks">the import_bookmarks
        /// </param>
        /// <returns> the page[]
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ArrayList ImportPages(ArrayList pages, bool import_bookmarks)
        {
            ArrayList imported_pages = new ArrayList();
            if (pages.Count > 0)
            {
                IntPtr[] in_pages = new IntPtr[pages.Count];
                for (int n = 0; n < pages.Count; n++)
                {
                    if (!(pages[n] is Page))
                        return null;
                    Page page = pages[n] as Page;
                    in_pages[n] = page.mp_page;
                }
                IntPtr[] out_pages = new IntPtr[pages.Count];
                for (int n = 0; n < pages.Count; n++)
                {
                    out_pages[n] = IntPtr.Zero;
                }

                int size = Marshal.SizeOf(in_pages[0]) * in_pages.Length;
                int size2 = Marshal.SizeOf(out_pages[0]) * out_pages.Length;
                IntPtr dest1 = Marshal.AllocHGlobal(size);
                IntPtr dest2 = Marshal.AllocHGlobal(size2);
                try
                {
                    Marshal.Copy(in_pages, 0, dest1, in_pages.Length);
                    Marshal.Copy(out_pages, 0, dest2, out_pages.Length);

                    PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocImportPages(mp_doc, dest1, pages.Count, import_bookmarks, dest2));

                    Marshal.Copy(dest2, out_pages, 0, out_pages.Length);

                    for (int n = 0; n < out_pages.Length; n++)
                    {
                        imported_pages.Add(new Page(out_pages[n], this));
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(dest1);
                    Marshal.FreeHGlobal(dest2);
                }
                
            }
            return imported_pages;
        }

        /// <summary> Create a new, empty page in the document. You can use PageWriter to fill the
	    /// page with new content. Finally the page should be inserted at specific
	    /// place within document page sequence using PageInsert/PagePushFront/PagePushBack
	    /// methods.
	    /// 
	    /// </summary>
	    /// <returns> A new, empty page.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  the new page still does not belong to document page sequence and should be 
        /// subsequently placed at a specific location within the sequence.
        /// </remarks>
        public Page PageCreate()
        {
            using (Rect media_box = new Rect(0, 0, 612, 792))
            {
                return PageCreate(media_box);
            }            
        }

        /// <summary> Page create.
        /// 
        /// </summary>
        /// <param name="media_box">the media_box
        /// </param>
        /// <returns> the page
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Page PageCreate(Rect media_box)
        {
            TRN_Page result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocPageCreate(mp_doc, ref media_box.mp_imp, ref result)); 
            return result != IntPtr.Zero ? new Page(result, this) : null;
        }

        /// <summary>Gets the first bookmark
        /// 
        /// </summary>			
        /// <returns>the first Bookmark from the document’s outline tree. If the Bookmark tree is empty 
        /// the underlying SDF&#47;Cos Object is null and returned Bookmark is not valid (i.e. Bookmark::IsValid() returns false).
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Bookmark GetFirstBookmark()
        {
            TRN_Bookmark result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetFirstBookmark(mp_doc, ref result)); 
            return result != IntPtr.Zero ? new Bookmark(result, this) : null;
        }

        /// <summary> Adds/links the specified Bookmark to the root level of document’s outline tree.
        /// 
        /// </summary>
        /// <param name="root_bookmark">the root_bookmark
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  parameter 'root_bookmark' must not be linked (must not be belong) to 
        /// a bookmark tree.</remarks>
        public void AddRootBookmark(Bookmark root_bookmark)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocAddRootBookmark(mp_doc, root_bookmark.mp_obj));
        }

        /// <summary> Gets the trailer.
        /// 
        /// </summary>
        /// <returns> - A dictionary representing the Cos root of the document (document's trailer)
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetTrailer()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetTrailer(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }

        /// <summary> Gets the root.
        /// 
        /// </summary>
        /// <returns> - A dictionary representing the Cos root of the document (/Root entry
        /// within the trailer dictionary)
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetRoot()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetRoot(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }

        /// <summary> Gets the pages.
        /// 
        /// </summary>
        /// <returns> - A dictionary representing the root of the low level page-tree
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetPages()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetPages(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }

        /// <summary> Gets the page count.
        /// 
        /// </summary>
        /// <returns> the number of pages in the document.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 GetPageCount()
        {
            Int32 count = Int32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetPageCount(mp_doc, ref count)); 
            return count;
        }

        /// <summary> Gets the field.
        /// 
        /// </summary>
        /// <param name="field_name">the field_name
        /// </param>
        /// <returns> the field
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Field GetField(string field_name)
        {
            UString str = new UString(field_name);
            BasicTypes.TRN_Field result = new BasicTypes.TRN_Field();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetField(mp_doc, str.mp_impl, ref result)); 
            return (result.leaf_node) != IntPtr.Zero ? new Field(result, this) : null;
        }
        /// <summary> An interactive form (sometimes referred to as an AcroForm) is a
        /// collection of fields for gathering information interactively from
        /// the user. A PDF document may contain any number of fields appearing
        /// on any combination of pages, all of which make up a single, global
        /// interactive form spanning the entire document.
        /// 
        /// The following methods are used to access and manipulate Interactive form
        /// fields (sometimes referred to as AcroForms).
        /// 
        /// </summary>
        /// <returns> an iterator to the first Field in the document.
        /// 
        /// The list of all Fields present in the document can be traversed as follows:
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <example>For a full sample, please refer to 'InteractiveForms' sample project.
        /// <code>  
        /// FieldIterator itr = pdfdoc.getFieldIterator();
        /// for(; itr.hasNext(); itr.next()) {
        /// Field field = itr.current();
        /// string s = field.getName();
        /// }
        /// </code>
        /// </example>
        public FieldIterator GetFieldIterator()
        {
            TRN_Iterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetFieldIteratorBegin(mp_doc, ref result)); 
            return new FieldIterator(result);
        }
        /// <summary> field_name - a string representing the fully qualified name of
        /// the field (e.g. "employee.name.first").
        /// 
        /// </summary>
        /// <param name="field_name">the field_name
        /// </param>
        /// <returns> a FieldIterator referring to an interactive Field
        /// or to invalid field if the field name was not found. If a given field name was
        /// not found itr.HasNext() will return false. For example:
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <example>
        /// <code>
        /// FieldIterator itr = pdfdoc.fieldFind("name");
        /// if (itr.hasNext()) {
        /// string name = itr.current().getName());
        /// }
        /// else { ...field was not found... }
        /// </code>
        /// </example>
        public FieldIterator GetFieldIterator(string field_name)
        {
            UString str = new UString(field_name);
            TRN_Iterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetFieldIterator(mp_doc, str.mp_impl, ref result)); 
            return new FieldIterator(result);
        }
        /// <summary> Create a new interactive form Field.
        /// 
        /// </summary>
        /// <param name="field_name">a string representing the fully qualified name of the
        /// field (e.g. "employee.name.first"). field_name must be either a unique name or
        /// equal to an existing terminal field name.
        /// </param>
        /// <param name="type">field type (e.g. Field::e_text, Field::e_button, etc.)
        /// </param>
        /// <returns> the new form Field.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Field FieldCreate(string field_name, Field.Type type)
        {
            BasicTypes.TRN_Field result = new BasicTypes.TRN_Field();
            UString str = new UString(field_name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocFieldCreate(mp_doc, str.mp_impl, type, IntPtr.Zero, IntPtr.Zero, ref result)); 
            return new Field(result, this);
        }
        /// <summary> Field create.
        /// 
        /// </summary>
        /// <param name="field_name">the field_name
        /// </param>
        /// <param name="type">the type
        /// </param>
        /// <param name="field_value">the field_value
        /// </param>
        /// <returns> the field
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Field FieldCreate(string field_name, Field.Type type, Obj field_value)
        {
            BasicTypes.TRN_Field result = new BasicTypes.TRN_Field();
            UString str = new UString(field_name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocFieldCreate(mp_doc, str.mp_impl, type, field_value.mp_obj, IntPtr.Zero, ref result));
            return new Field(result, this);
        }
        /// <summary> Field create.
        /// 
        /// </summary>
        /// <param name="field_name">the field_name
        /// </param>
        /// <param name="type">the type
        /// </param>
        /// <param name="field_value">the field_value
        /// </param>
        /// <returns> the field
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Field FieldCreate(string field_name, Field.Type type, string field_value)
        {
            BasicTypes.TRN_Field result = new BasicTypes.TRN_Field();
            UString str = new UString(field_name);
            UString str2 = new UString(field_value);
            UString str3 = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocFieldCreateFromStrings(mp_doc, str.mp_impl, type, str2.mp_impl, str3.mp_impl, ref result));
            return new Field(result, this);
        }
        /// <summary> Field create.
        /// 
        /// </summary>
        /// <param name="field_name">the field_name
        /// </param>
        /// <param name="type">the type
        /// </param>
        /// <param name="field_value">the field_value
        /// </param>
        /// <param name="def_field_value">the def_field_value
        /// </param>
        /// <returns> the field
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Field FieldCreate(string field_name, Field.Type type, Obj field_value, Obj def_field_value)
        {
            BasicTypes.TRN_Field result = new BasicTypes.TRN_Field();
            UString str = new UString(field_name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocFieldCreate(mp_doc, str.mp_impl, type, field_value.mp_obj, def_field_value.mp_obj, ref result));
            return new Field(result, this);
        }
        /// <summary> Field create.
        /// 
        /// </summary>
        /// <param name="field_name">the field_name
        /// </param>
        /// <param name="type">the type
        /// </param>
        /// <param name="field_value">the field_value
        /// </param>
        /// <param name="def_field_value">the def_field_value
        /// </param>
        /// <returns> the field
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Field FieldCreate(string field_name, Field.Type type, string field_value, string def_field_value)
        {
            BasicTypes.TRN_Field result = new BasicTypes.TRN_Field();
            UString str = new UString(field_name);
            UString str2 = new UString(field_value);
            UString str3 = new UString(def_field_value);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocFieldCreateFromStrings(mp_doc, str.mp_impl, type, str2.mp_impl, str3.mp_impl, ref result));
            return new Field(result, this);
        }
        /// <summary> Regenerates the appearance stream for every widget annotation in the document
        /// Call this method if you modified field's value and would like to update
        /// field's appearances.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void RefreshFieldAppearances()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocRefreshFieldAppearances(mp_doc));
        }
        /// <summary>Flatten all annotations in the document.</summary>
        /// <param name="forms_only"> f false flatten all annotations, otherwise flatten only form fields.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void FlattenAnnotations()
        {
            FlattenAnnotations(false);
        }

        /// <summary>Flatten all annotations in the document.</summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void FlattenAnnotations(bool forms_only)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocFlattenAnnotations(mp_doc, forms_only));
        }

        /// <summary>
        /// Flatten selected item(s) in the document.
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void FlattenAnnotationsAdvanced(PDFDoc.FlattenAnnotationFlag flags)
        {
            uint f = System.Convert.ToUInt32(flags);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocFlattenAnnotationsAdvanced(mp_doc, f));
        }

        /// <summary> Gets the acro form.
        /// 
        /// </summary>
        /// <returns> the AcroForm dictionary located in "/Root" or NULL if dictionary is not present.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetAcroForm()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetAcroForm(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }

        /// <summary>Action that is triggered when the document is opened. The returned action can be either a destination or some other kind of Action (see Section 8.5, 'Actions' in PDF Reference Manual).
        /// </summary>
        /// <returns>the open action
        /// </returns>
        /// <remarks>if the document does not nave associated action the returned Action will be null &#40;i.e. Action.IsValid&#40;&#41; returns false&#41;
        /// </remarks>
        public Action GetOpenAction()
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetOpenAction(mp_doc, ref result)); 
            return result != IntPtr.Zero ? new Action(result, this) : null;
        }

        /// <summary> Sets the Action that will be triggered when the document is opened.
        /// 
        /// </summary>
        /// <param name="action">A new Action that will be triggered when the document is opened.
        /// An example of such action is a GoTo Action that takes the user to a given
        /// location in the document.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetOpenAction(Action action)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocSetOpenAction(mp_doc, action.mp_action));
        }

        /// <summary> Associates a file attachment with the document.
        /// 
        /// The file attachment will be displayed in the user interface of a viewer application
        /// (in Acrobat this is File Attachment tab). The function differs from
        /// Annot.CreateFileAttachment() because it associates the attachment with the
        /// whole document instead of an annotation on a specific page.
        /// 
        /// </summary>
        /// <param name="file_key">A key/name under which the attachment will be stored.
        /// </param>
        /// <param name="embedded_file">the embedded_file
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <example>  Another way to associate a file attachment with the document is using SDF::NameTree: 
        /// <code>  
        /// SDF.NameTree names = SDF.NameTree.Create(doc, "EmbeddedFiles");
        /// names.put(file_key, file_keysz, embedded_file.GetSDFObj());
        /// </code>
        /// </example>
        public void AddFileAttachment(string file_key, FileSpec embedded_file)
        {
            UString str = new UString(file_key);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocAddFileAttachment(mp_doc, str.mp_impl, embedded_file.mp_impl));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateIndirectArray(mp_doc, ref result));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateIndirectBool(mp_doc, value, ref result));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateIndirectDict(mp_doc, ref result));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateIndirectName(mp_doc, name, ref result));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateIndirectNull(mp_doc, ref result));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateIndirectNumber(mp_doc, value, ref result));
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
                PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateIndirectStream(mp_doc, pnt, new UIntPtr(size), filter != null ? filter.mp_imp : IntPtr.Zero, ref result));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateIndirectStreamFromFilter(mp_doc, data.mp_imp, filter != null ? filter.mp_imp : IntPtr.Zero, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Creates the indirect string.
        /// 
        /// </summary>
        /// <param name="value">the value
        /// </param>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectString(string value)
        {
            TRN_Obj result = IntPtr.Zero;
            UString str = new UString(value);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateIndirectStringFromUString(mp_doc, str.mp_impl, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Creates the indirect string.
        /// 
        /// </summary>
        /// <param name="buf">the str
        /// </param>
        /// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateIndirectString(byte[] buf)
        {
            TRN_Obj result = IntPtr.Zero;
            uint size = System.Convert.ToUInt32(buf.Length);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateIndirectString(mp_doc, buf, size, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }

        /// <summary> Gets the SDFDoc.
        /// 
        /// </summary>
        /// <returns> document's SDF/Cos document
        /// </returns>
        public SDFDoc GetSDFDoc()
        {
            TRN_SDFDoc result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetSDFDoc(mp_doc, ref result)); 
            return new SDFDoc(result, this);
        }

        /// <summary> Locks the document to prevent competing threads from accessiong the document
        /// at the same time. Threads attempting to access the document will wait in
        /// suspended state until the thread that owns the lock calls doc.Unlock().
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Lock()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocLock(mp_doc));
        }
        /// <summary> Removes the lock from the document.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Unlock()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocUnlock(mp_doc));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocLockRead(mp_doc));
        }
        /// <summary> Removes the read lock from the document. 
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void UnlockRead()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocUnlockRead(mp_doc));
        }
        /// <summary> Try locking the document in non-blocking manner.
        /// 
        /// </summary>
        /// <returns> true if the document is locked for multi-threaded access, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool TryLock()
        {
            int milliseconds = 0;
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocTimedLock(mp_doc, milliseconds, ref result));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocTimedLockRead(mp_doc, milliseconds, ref result)); 
            return result;
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocTimedLock(mp_doc, milliseconds, ref result));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocTimedLockRead(mp_doc, milliseconds, ref result));
            return result;
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

        /// <summary> Gets the view prefs.
        /// 
        /// </summary>
        /// <returns> Viewer preferences for this document.
        /// 
        /// PDFDocViewPrefs is a high-level utility class that can be
        /// used to control the way the document is to be presented on
        /// the screen or in print.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PDFDocViewPrefs GetViewPrefs()
        {
            PDFDocViewPrefs result = new PDFDocViewPrefs();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetViewPrefs(mp_doc, ref result.mp_prefs)); 
            return result;
        }
        /// <summary> Gets the page label.
        /// 
        /// </summary>
        /// <param name="page_num">The page number. Because PDFNet indexes pages
        /// starting from 1, page_num must be larger than 0.
        /// </param>
        /// <returns> the PageLabel that is in effect for the given page.
        /// If there is no label object in effect, this method returns an
        /// invalid page label object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PageLabel GetPageLabel(int page_num)
        {
            PageLabel result = new PageLabel();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetPageLabel(mp_doc, page_num, ref result.mp_imp)); 
            return result;
        }
        /// <summary> Attaches a label to a page. This establishes the numbering scheme
        /// for that page and all following it, until another page label is
        /// encountered. This label allows PDF producers to define a page
        /// numbering system other than the default.
        /// 
        /// </summary>
        /// <param name="page_num">The number of the page to label. If page_num is
        /// less than 1 or greater than the number of pages in the document,
        /// the method does nothing.
        /// </param>
        /// <param name="label">the label
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPageLabel(int page_num, PageLabel label)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocSetPageLabel(mp_doc, page_num, ref label.mp_imp));
        }
        /// <summary> Removes the page label that is attached to the specified page,
        /// effectively merging the specified range with the previous page
        /// label sequence.
        /// 
        /// </summary>
        /// <param name="page_num">The page from which the page label is removed.
        /// Because PDFNet indexes pages starting from 1, page_num must be
        /// larger than 0.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void RemovePageLabel(int page_num)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocRemovePageLabel(mp_doc, page_num));
        }
        /// <summary>Gets the structure tree.
        /// </summary>
        /// <returns>document's logical structure tree root.
        /// </returns>
        /// <exception cref="PDFNetException">PDFNetException the PDFNet exception</exception>
        public Struct.STree GetStructTree()
        {
            TRN_STree result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetStructTree(mp_doc, ref result));
            return new Struct.STree(result, this);
        }

        /// <summary> Checks for oc.
        /// 
        /// </summary>
        /// <returns> true if the optional content (OC) feature is associated with
        /// the document. The document is considered to have optional content if
        /// there is an OCProperties dictionary in the document's catalog, and
        /// that dictionary has one or more entries in the OCGs array.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool HasOC()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocHasOC(mp_doc, ref result)); 
            return result;
        }

        /// <summary> Gets the OCGs.
        /// 
        /// </summary>
        /// <returns> the Obj array that contains optional-content groups (OCGs) for
        /// the document, or NULL if the document does not contain any OCGs. The
        /// order of the groups is not guaranteed to be the creation order, and is
        /// not the same as the display order.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetOCGs()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetOCGs(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Gets the OCG configuration.
        /// </summary>
        /// <returns>the default optional&#45;content configuration for the document
        /// from the OCProperties D entry.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public pdftron.PDF.OCG.Config GetOCGConfig()
        {
            TRN_OCGConfig result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetOCGConfig(mp_doc, ref result)); 
            return ((result == IntPtr.Zero) ? null : new pdftron.PDF.OCG.Config(result, this));
        }
        /// <summary> Fdf extract.
        /// 
        /// </summary>
        /// <returns> a pointer to the newly created FDF file with an interactive data.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FDFDoc FDFExtract()
        {
            ExtractFlag flag = ExtractFlag.e_forms_only;
            return FDFExtract(flag);
        }
        /// <summary> Fdf extract.
        /// 
        /// </summary>
        /// <param name="flag">the flag that specifies the extract options
        /// </param>
        /// <returns> a pointer to the newly created FDF file with an interactive data.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FDFDoc FDFExtract(ExtractFlag flag)
        {
            FDFDoc result = new FDFDoc();
            uint f = System.Convert.ToUInt32(flag);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocFDFExtract(mp_doc, f, ref result.mp_doc)); 
            return result;
        }
        /// <summary> Extract annotations to FDF.
        /// 
        /// </summary>
        /// <param name="annotations">specifies the array of annotations
        /// </param>
        /// <returns> the newly created FDF file with interactive data.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FDFDoc FDFExtract(ArrayList annotations)
        {
            FDFDoc result = new FDFDoc();
            if(annotations.Count > 0)
            {
                IntPtr[] in_annots = new IntPtr[annotations.Count];
                for (int n = 0; n < annotations.Count; n++)
                {
                    if (!(annotations[n] is Annot))
                    {
                        var exception = new PDFNetException("Error", "PDFDoc.cs", 0, "PDFDoc.FDFExtract", "ArrayList contains non-Annot objects.", PDFNetException.ErrorCodes.e_error_general);
                        throw exception;
                    }

                    var annot = annotations[n] as Annot;
					in_annots[n] = annot.mp_annot;
                }
                int size = Marshal.SizeOf(in_annots[0]) * in_annots.Length;
                IntPtr dest1 = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.Copy(in_annots, 0, dest1, in_annots.Length);
                    PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocFDFExtractAnnots(mp_doc, dest1, in_annots.Length, ref result.mp_doc)); 
                }
                finally
                {
                    Marshal.FreeHGlobal(dest1);
                }
            }
            return result;
        }
        /// <summary> Import form data from FDF file to PDF interactive form.
        /// 
        /// </summary>
        /// <param name="fdf_doc">- a pointer to the FDF file
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void FDFMerge(FDFDoc fdf_doc)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocFDFMerge(mp_doc, fdf_doc.mp_doc));
        }
        /// <summary> Replace existing form and annotation data with those imported from the FDF file.
        /// Since this method avoids updating annotations unnecessarily it is ideal for incremental save.
        /// 
        /// </summary>
        /// <param name="fdf_doc">- a pointer to the FDF file
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void FDFUpdate(FDFDoc fdf_doc)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocFDFUpdate(mp_doc, fdf_doc.mp_doc));
        }
        /// <summary> Gets the doc info.
        /// 
        /// </summary>
        /// <returns> The class representing document information metadata.
        /// (i.e. entries in the document information dictionary).
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PDFDocInfo GetDocInfo()
        {
            TRN_PDFDocInfo info = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetDocInfo(mp_doc, ref info)); 
            return new PDFDocInfo(info, this);
        }
        /// <summary> AddHighlights is used to highlight text in a document using 'Adobe's Highlight
        /// File Format' (Technical Note #5172 ). The method will parse the character offset data
        /// and modify the current document by adding new highlight annotations.
        /// 
        /// </summary>
        /// <param name="hilite">a string representing the filename for the highlight file or
        /// or a data buffer containing XML data.
        /// </param>
        public void AddHighlights(string hilite)
        {
            UString str = new UString(hilite);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocAddHighlights(mp_doc, str.mp_impl));
        }

        /// <summary> Checks if is tagged.
        /// 
        /// </summary>
        /// <returns> true if this document is marked as Tagged PDF, false otherwise.
        /// </returns>
        public bool IsTagged()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocIsTagged(mp_doc, ref result)); 
            return result;
        }

        // Nested Types
        /// <summary>
        /// Flatten selected items in the document.
        /// </summary>
        [Flags]
        public enum FlattenAnnotationFlag
        {
            /// <summary>Default, only flatten form fields.</summary>
            e_flatten_forms_only = 0x01,
            /// <summary>Only flatten annotations.</summary>
            e_flatten_annots_only = 0x02,
            /// <summary>Only flatten links.</summary>
            e_flatten_link_only = 0x04,
            /// <summary>Flatten all.</summary>
            e_flatten_all = 0x08
        }

        //TODO: enum documentation missing
        ///<summary></summary>
        public enum ExtractFlag
        {
            ///<summary>default, extract only form fields</summary>
            e_forms_only,
            ///<summary>extract only annotations</summary>
            e_annots_only,
            ///<summary>extract both form fields and annotations</summary>
            e_both
        }

        //TODO: enum documentation missing
        ///<summary></summary>
        public enum InsertFlag
        {
            ///<summary>default, do not insert</summary>
            e_none,
            ///<summary>insert bookmarks (use this option when inserting many pages in a single call)</summary>
            e_insert_bookmark,
            ///<summary>same as e_insert_bookmark, but ignore GoToR and URI links, while still retaining bookmark hierarchy (use this option when inserting one page at a time)</summary>
            e_insert_goto_bookmark
        }
    }
}
