using System;
using System.Collections.Generic;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.Filters;
using pdftron.SDF;

using TRN_FileSpec = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Filter = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> FileSpec corresponds to the PDF file specification object. 
    /// 
    /// A PDF file can refer to the contents of another file by using a file specification, 
    /// which can take either of the following forms:
    /// <list type="bullet">
    /// <item><description>
    /// A simple file specification gives just the name of the target file in 
    /// a standard format, independent of the naming conventions of any particular file system. 
    /// </description></item>
    /// <item><description>
    /// A full file specification includes information related to one or more specific file
    /// systems.
    /// </description></item>
    /// <item><description>
    /// A URL reference.
    /// </description></item>
    /// </list>
    /// Although the file designated by a file specification is normally external to the
    /// PDF file referring to it, it is also possible to embed the file allowing its contents 
    /// to be stored or transmitted along with the PDF file. However, embedding a file does not 
    /// change the presumption that it is external to (or separate from) the PDF file.
    /// 
    /// For more details on file specifications, please refer to Section 3.10, 'File Specifications'
    /// in the PDF Reference Manual.
    /// </summary>
    public class FileSpec : IDisposable
    {
        internal TRN_FileSpec mp_impl = IntPtr.Zero;
        internal Object m_ref;

        /// <summary> Releases all resources used by the FileSpec </summary>
        ~FileSpec()
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
            if (mp_impl != IntPtr.Zero)
            {
                mp_impl = IntPtr.Zero;
            }
        }

        // Methods
        internal FileSpec(TRN_FileSpec impl, Object reference)
        {
            this.mp_impl = impl;
            this.m_ref = reference;
        }
        /// <summary> Create a FileSpec and initialize it using given Cos/SDF object. 
	    /// 
	    /// </summary>
	    /// <param name="a"> given Cos/SDF object. 
	    /// </param>			
	    /// <returns> the file spec
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FileSpec(Obj a)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileSpecCreateFromObj(a.mp_obj,ref mp_impl));
            this.m_ref = a.GetRefHandleInternal();
        }
	    /// <summary> Creates the.
	    /// 
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="path">the path
	    /// </param>
	    /// <param name="embed">the embed
	    /// </param>
	    /// <returns> the file spec
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static FileSpec Create(SDFDoc doc, string path, bool embed)
        {
            TRN_FileSpec result = IntPtr.Zero;
            UString str = new UString(path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileSpecCreate(doc.mp_doc, str.mp_impl, embed, ref result));
            return new FileSpec(result, doc);
        }
	    /// <summary> Creates a file specification for the given file. By default, the specified
	    /// file is embedded in PDF.
	    /// 
	    /// </summary>
	    /// <param name="doc">- A document to which the FileSpec should be added. To obtain
	    /// SDFDoc from PDFDoc use PDFDoc.GetSDFDoc() or Obj.GetDoc().
	    /// </param>
	    /// <param name="path">- The path to convert into a file specification.
	    /// </param>
	    /// <returns> newly created FileSpec object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static FileSpec Create(SDFDoc doc, string path)
        {
            return Create(doc, path, true);
        }
	    /// <summary> Creates a URL file specification.
	    /// 
	    /// </summary>
	    /// <param name="doc">- A document to which the FileSpec should be added. To obtain
	    /// SDF.Doc from PDFDoc use PDFDoc.GetSDFDoc() or Obj.GetDoc().
	    /// </param>
	    /// <param name="url">- A uniform resource locator (URL) of the form defined in
	    /// Internet RFC 1738, Uniform Resource Locators Specification.
	    /// </param>
	    /// <returns> newly created FileSpec object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static FileSpec CreateURL(SDFDoc doc, string url)
        {
            TRN_FileSpec result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileSpecCreateURL(doc.mp_doc, url, ref result));
            return new FileSpec(result, doc);
        }

        /// <summary>Sets value to the give <c>FileSpec</c> object</summary>
        /// <param name="p">a <c>FileSpec</c> object</param>
        public void Set(FileSpec p)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileSpecCopy(p.mp_impl, ref mp_impl));
        }
	    /// <summary>Assignment operator</summary>
        /// <param name="r">a given <c>FileSpec</c> object</param>
        /// <returns>a <c>FileSpec</c> object equals to the given object</returns>
        public FileSpec op_Assign(FileSpec r)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileSpecCopy(r.mp_impl, ref mp_impl));
            return this;
        }
	    /// <summary>Equality operator checks whether two FileSpec objects are the same</summary>
	    /// <param name="l">the<c>FileSpec</c> object on the left of the operator</param>
	    /// <param name="r">the<c>FileSpec</c> object on the right of the operator</param>
	    /// <returns>true, if both objects are equal</returns>
        public static bool operator ==(FileSpec l, FileSpec r)
        {
            if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null)) return true;
	        if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null)) return false;
	        if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null)) return false;

            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileSpecCompare(l.mp_impl, r.mp_impl, ref result));
            return result;
        }
	    /// <summary>Inequality operator checks whether two FileSpec objects are different</summary>
	    /// <param name="l">the<c>FileSpec</c> object on the left of the operator</param>
	    /// <param name="r">the<c>FileSpec</c> object on the right of the operator</param>
	    /// <returns>true, if both objects are not equal</returns>
        public static bool operator !=(FileSpec l, FileSpec r)
        {
            return !(l == r);
        }
	    /// <param name="o">a given <c>Object</c>
	    /// </param>
        /// <returns>true, if equals to the given object
        /// </returns>
        public override bool Equals(Object o)
        {
            if (o == null) return false;
            FileSpec i = o as FileSpec;
            if (i == null) return false;
            bool b = mp_impl == i.mp_impl;
            return b;
        }

	    /// <summary> Checks if is valid.
	    /// 
	    /// </summary>
	    /// <returns> whether this is a valid (non-null) FileSpec. If the
	    /// function returns false the underlying SDF/Cos object is null or is not valid
	    /// and the FileSpec object should be treated as null as well.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileSpecIsValid(mp_impl, ref result));
            return result; 
        }

	    /// <summary> Export.
	    /// 
	    /// </summary>
	    /// <param name="save_as">the save_as
	    /// </param>
	    /// <returns> true, if successful
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool Export(string save_as)
        {
            bool result = false;
            UString str = new UString(save_as);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileSpecExport(mp_impl, str.mp_impl, ref result));
            return result; 
        }
	    /// <summary> The function saves the data referenced by this FileSpec to an external file.
	    /// 
	    /// </summary>
	    /// <returns> true is the file was saved successfully, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool Export() 
        {
            return Export("");
        }

	    /// <summary> The function returns data referenced by this FileSpec.
	    /// 
	    /// </summary>
	    /// <returns> A stream (filter) containing file data.
	    /// If the file is embedded, the function returns a stream to the embedded file.
	    /// If the file is not embedded, the function will return a stream to the external file.
	    /// If the file is not embedded and the external file can't be found, the function
	    /// returns NULL.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Filter GetFileData()
        {
            TRN_Filter result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileSpecGetFileData(mp_impl, ref result));
            return new Filter(result, null);
        }
	    /// <summary> Gets the file path.
	    /// 
	    /// </summary>
	    /// <returns> The file path for this file specification.
	    /// 
	    /// If the FileSpec is a dictionary, a corresponding platform specific path
	    /// is returned (DOS, Mac, or Unix). Otherwise the function returns the path represented
	    /// in the form described in Section 3.10.1, 'File Specification Strings,' or , if the
	    /// file system is URL, as a uniform resource locator (URL). If the FileSpec is not
	    /// valid, an empty string is returned.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetFilePath()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileSpecGetFilePath(mp_impl, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
	    /// <summary> Gets the SDFObj.
	    /// 
	    /// </summary>
	    /// <returns> The underlying SDF/Cos object.
	    /// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileSpecGetSDFObj(mp_impl, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	    /// <summary>The functions sets the descriptive text associated with the file specification.
        /// This text is typically used in the EmbeddedFiles name tree.</summary>
        /// <param name="desc">descriptive text</param>
        public void SetDesc(string desc)
        {
            UString str = new UString(desc);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FileSpecSetDesc(mp_impl, str.mp_impl));
        }
    }
}
