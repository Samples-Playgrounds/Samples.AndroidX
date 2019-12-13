using System;
using System.Collections.Generic;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.PDF;
using pdftron.SDF;
using pdftron.Filters;

using TRN_SDFDoc = System.IntPtr;
using TRN_FDFDoc = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_Iterator = System.IntPtr;
using System.Runtime.InteropServices;

namespace pdftron.FDF
{
    /// <summary> FDFDoc is a class representing Forms Data Format (FDF) documents.
    /// FDF is typically used when submitting form data to a server, receiving 
    /// the response, and incorporating it into the interactive form. It can also 
    /// be used to export form data to stand-alone files that can be stored, transmitted 
    /// electronically, and imported back into the corresponding PDF interactive form. 
    /// In addition, beginning in PDF 1.3, FDF can be used to define a container for 
    /// annotations that are separate from the PDF document to which they apply.
    /// </summary>
    public class FDFDoc : IDisposable
    {
        internal TRN_FDFDoc mp_doc = IntPtr.Zero;
        internal bool m_owner = true;

        private bool disposed = false;

        /// <summary> Releases all resources used by the FDFDoc </summary>
        ~FDFDoc()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected void Dispose(bool disposing)
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
                disposed = true;
            }
        }
        public void Destroy()
        {
            if (mp_doc != IntPtr.Zero && m_owner)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocDestroy(mp_doc));
                mp_doc = IntPtr.Zero;
            }
        }

        internal FDFDoc(TRN_FDFDoc impl)
        {
            this.mp_doc = impl;
        }
        internal IntPtr GetHandleInternal()
        {
            return this.mp_doc;
        }

        /// <summary> Default constructor. Creates an empty FDF document.
        /// 
        /// </summary>
        /// <throws>  PDFNetException  </throws>
        public FDFDoc()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocCreate(ref mp_doc));
        }
        /// <summary> Open an existing FDF document from an InputStream.
        /// 
        /// </summary>
        /// <param name="stream">- input stream containing a serialized document.
        /// </param>
        /// <throws>  PDFNetException  </throws>
        /// <throws>  IOException Signals that an I/O exception has occurred. </throws>
        /// <remarks>  Make sure to call InitSecurityHandler() after FDFDoc(...) for encrypted documents. </remarks>
        public FDFDoc(Filter stream)
        {
            stream.setRefHandleInternal(this);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocCreateFromStream(stream.mp_imp, ref mp_doc));
        }
        /// <summary> Create a FDF document from an existing SDF/Cos document.
        /// 
        /// </summary>
        /// <param name="sdfdoc">An SDF document. Created FDFDoc will
        /// take the ownership of the low-level document.
        /// </param>
        /// <throws>  PDFNetException  </throws>
        public FDFDoc(SDFDoc sdfdoc)
        {
            if (sdfdoc.GetRefHandleInternal() != null)
            {
                throw new PDFNetException("PDFNetException", "FDFDoc.cs", 0, "FDFDoc(SDFDoc)", "SDFDoc is already owned by another document.", PDFNetException.ErrorCodes.e_error_general);
            }
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocCreateFromSDFDoc(sdfdoc.mp_doc, ref mp_doc));
            sdfdoc.SetRefHandleInternal(this);
        }
        /// <summary>
        /// Create a new FDFDoc from XFDF input. Input can be either a XFDF file path, or the XFDF data itself.
        /// </summary>
        /// <param name="xfdf">string containing either the file path to a XFDF file, or the XML buffer containing the XFDF.
        /// </param>
        /// <throws>  PDFNetException  </throws>
        public FDFDoc(string xfdf)
        {
            UString str = new UString(xfdf);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocCreateFromUFilePath(str.mp_impl, ref mp_doc));
        }
        /// <summary> Open an existing FDF document from an InputStream.
        /// 
        /// </summary>
        /// <param name="buf">- input stream containing a serialized document.
        /// </param>
        /// <param name="buf_size">- the expected size of the input stream.
        /// </param>
        /// <throws>  PDFNetException  </throws>
        /// <throws>  IOException Signals that an I/O exception has occurred. </throws>
        /// <remarks>  Make sure to call InitSecurityHandler() after FDFDoc(...) for encrypted documents. </remarks>
        public FDFDoc(byte[] buf, int buf_size)
        {
            uint size = System.Convert.ToUInt32(buf_size);
            
            int psize = Marshal.SizeOf(buf[0]) * buf.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(buf, 0, pnt, buf.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocCreateFromMemoryBuffer(pnt, new UIntPtr(size), ref mp_doc));
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }

        public FDFDoc(FDFDoc other)
        {
            if (other.m_owner)
            {
                other.m_owner = false;
                m_owner = true;
            }
            else
            {
                m_owner = false;
            }
            this.mp_doc = other.mp_doc;
        }

        /// <summary> Close FDFDoc and release associated resources
        /// 
        /// </summary>
        /// <throws>  PDFNetException  </throws>
        public void Close()
        {
            if (!this.disposed)
                Dispose(false);
            this.disposed = true;
        }
        /// <summary> Create a new FDFDoc from XFDF input. Input can be either a XFDF file path, or the XFDF data itself.
        /// 
        /// </summary>
        /// <param name="xfdf">string containing either the file path to a XFDF file, or the XML buffer containing the XFDF.
        /// </param>
        /// <returns> created FDFDoc 
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public static FDFDoc CreateFromXFDF(string xfdf)
        {
            TRN_FDFDoc doc = IntPtr.Zero;
            UString str = new UString(xfdf);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocCreateFromXFDF(str.mp_impl, ref doc)); 
            return new FDFDoc(doc);
        }
        /// <summary> Create a new interactive form FDFField.
        /// 
        /// </summary>
        /// <param name="field_name">the field_name
        /// </param>
        /// <param name="type">the field_type
        /// </param>
        /// <returns> the fDF field
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public FDFField FieldCreate(string field_name, int type)
        {
            BasicTypes.TRN_FDFField result = new BasicTypes.TRN_FDFField();
            UString str = new UString(field_name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocFieldCreate(mp_doc, str.mp_impl, (Field.Type)type, IntPtr.Zero, ref result));
            return new FDFField(result, this);
        }
        /// <summary> Create a FDFField with specified name, type and string value
        /// 
        /// </summary>
        /// <param name="field_name">the field_name
        /// </param>
        /// <param name="type">the field_type
        /// </param>
        /// <param name="field_value">the field_value
        /// </param>
        /// <returns> the FDF field
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public FDFField FieldCreate(string field_name, int type, Obj field_value)
        {
            BasicTypes.TRN_FDFField result = new BasicTypes.TRN_FDFField();
            UString str = new UString(field_name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocFieldCreate(mp_doc, str.mp_impl, (Field.Type)type, field_value.mp_obj, ref result));
            return new FDFField(result, this);
        }
        /// <summary> Create a FDFField with specified name, type and value
        /// 
        /// </summary>
        /// <param name="field_name">the field_name
        /// </param>
        /// <param name="type">the field_type
        /// </param>
        /// <param name="field_value">the field_value
        /// </param>
        /// <returns> the FDF field
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public FDFField FieldCreate(string field_name, int type, string field_value)
        {
            BasicTypes.TRN_FDFField result = new BasicTypes.TRN_FDFField();
            UString str = new UString(field_name);
            UString str2 = new UString(field_value);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocFieldCreateFromString(mp_doc, str.mp_impl, (Field.Type)type, str2.mp_impl, ref result));
            return new FDFField(result, this);
        }
        /// <summary> Get the FDF dictionary.
        /// 
        /// </summary>
        /// <returns> the FDF dictionary located in "/Root" or NULL if dictionary is not present.
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public Obj GetFDF()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocGetFDF(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> field_name a string representing the fully qualified name of
        /// the field (e.g. "employee.name.first").
        /// 
        /// </summary>
        /// <param name="field_name">the field_name
        /// </param>
        /// <returns> a FDFFieldIterator referring to the given interactive FDFField
        /// or if the field name was not found HasNext() will return false.
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public FDFField GetField(string field_name)
        {
            UString str = new UString(field_name);
            BasicTypes.TRN_FDFField result = new BasicTypes.TRN_FDFField();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocGetField(mp_doc, str.mp_impl, ref result));
            return new FDFField(result, this);
        }
        /// <summary> An interactive form (sometimes referred to as an AcroForm) is a
        /// collection of fields for gathering information interactively from
        /// the user. A FDF document may contain any number of fields appearing
        /// on any combination of pages, all of which make up a single, global
        /// interactive form spanning the entire document.
        /// 
        /// The following methods are used to access and manipulate Interactive form
        /// fields (sometimes referred to as AcroForms).
        /// 
        /// </summary>
        /// <returns> an iterator to the first Filed in the document.
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        /// <remarks>  if the document has no AcroForms, <c>HasNext()</c> </remarks>
        /// <summary> will return false.
        /// </summary>
        public FDFFieldIterator GetFieldIterator()
        {
            TRN_Iterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocGetFieldIteratorBegin(mp_doc, ref result));
            return new FDFFieldIterator(result);
        }
        /// <summary> Get the field iterator.
        /// 
        /// </summary>
        /// <param name="field_name">the field_name
        /// </param>
        /// <returns> the field iterator
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public FDFFieldIterator GetFieldIterator(string field_name)
        {
            UString str = new UString(field_name);
            TRN_Iterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocGetFieldIterator(mp_doc, str.mp_impl, ref result));
            return new FDFFieldIterator(result);
        }
        /// <summary> Get the ID entry from "/Root/FDF" dictionary.
        /// 
        /// </summary>
        /// <returns> An object representing the ID entry in "/Root/FDF" dictionary.
        /// </returns>
        /// <throws>  PDFNetException  </throws>		
        public Obj GetID()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocGetID(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Get the PDF document file that this FDF file was exported from or is intended
        /// to be imported into.
        /// 
        /// </summary>
        /// <returns> a String with the PDF document file name.
        /// </returns>
        /// <throws>  PDFNetException  </throws>		
        public string GetPdfFileName()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocGetPDFFileName(mp_doc, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
        /// <summary> Get the root.
        /// 
        /// </summary>
        /// <returns> A dictionary representing the Cos root of the document (/Root entry
        /// within the trailer dictionary)
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public Obj GetRoot()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocGetRoot(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> Get the SDFDoc object in FDFDoc
        /// 
        /// </summary>
        /// <returns> document's SDF/Cos document
        /// </returns>
        public SDFDoc GetSDFDoc()
        {
            TRN_SDFDoc result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocGetSDFDoc(mp_doc, ref result));
            return new SDFDoc(result, this);
        }
        /// <summary> Get the trailer.
        /// 
        /// </summary>
        /// <returns> A dictionary representing the Cos root of the document (document's trailer)
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public Obj GetTrailer()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocGetTrailer(mp_doc, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
        /// <summary> determine whether the document is modified
        /// 
        /// </summary>
        /// <returns> true if document was modified, false otherwise
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public bool IsModified()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocIsModified(mp_doc, ref result));
            return result;
        }
        /// <summary> Merge annotations into FDF document from an XML command.
        ///
        /// </summary>
        /// <param name="command_file">the path to XML command file, or the XML command itself
        /// </param>
        /// <throws>  PDFNetException  </throws>
        public void MergeAnnots(string command_file)
        {
            MergeAnnots(command_file, "");
        }
        /// <summary> Merge annotations into FDF document from an XML command.
        /// 
        /// </summary>
        /// <param name="command_file">the path to XML command file, or the XML command itself
        /// </param>
        /// <param name="permitted_user">the user name of the permitted user
        /// </param>
        /// <throws>  PDFNetException  </throws>
        public void MergeAnnots(string command_file, string permitted_user)
        {
            UString str1 = new UString(command_file);
            UString str2 = new UString(permitted_user);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocMergeAnnots(mp_doc, str1.mp_impl, str2.mp_impl));
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
        /// <param name="path">The full path name to which the file is saved.
        /// </param>
        /// <throws>  PDFNetException  </throws>
        public void Save(string path)
        {
            UString str = new UString(path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocSave(mp_doc, str.mp_impl));
        }
        public byte[] Save()
        {
            UIntPtr size = UIntPtr.Zero;
            IntPtr source = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocSaveMemoryBuffer(mp_doc, ref source, ref size));
            byte[] buf = new byte[size.ToUInt32()];
            System.Runtime.InteropServices.Marshal.Copy(source, buf, 0, System.Convert.ToInt32(size.ToUInt32()));
            return buf;
        }
        /// <summary> Save a FDF document into a XFDF string.
        /// 
        /// </summary>
        /// <returns> String containing the XFDF representation of the FDF document
        /// </returns>
        /// <throws> PDFNetException </throws>
        public string SaveAsXFDF()
        {
            UString str = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocSaveAsXFDFAsString(mp_doc, ref str.mp_impl));
            return str.ConvToManagedStr();
        }
        /// <summary> Save a FDF document in XFDF format.
        /// 
        /// </summary>
        /// <param name="file_name">the path
        /// </param>
        /// <throws>  PDFNetException  </throws>
        public void SaveAsXFDF(string file_name)
        {
            UString str = new UString(file_name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocSaveAsXFDF(mp_doc, str.mp_impl));
        }
        /// <summary> Set the ID entry in "/Root/FDF" dictionary.
        /// 
        /// </summary>
        /// <param name="id">ID array object.
        /// </param>
        /// <throws>  PDFNetException  </throws>
        public void SetID(Obj id)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocSetID(mp_doc, id.mp_obj));
        }
        /// <summary> Set the PDF document file that this FDF file was exported from or is intended
        /// to be imported into.
        /// 
        /// </summary>
        /// <param name="filepath">pathname to the file.
        /// </param>
        /// <throws>  PDFNetException  </throws>
        public void SetPdfFileName(string filepath)
        {
            UString str = new UString(filepath);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFDocSetPDFFileName(mp_doc, str.mp_impl));
        }

    }
}
