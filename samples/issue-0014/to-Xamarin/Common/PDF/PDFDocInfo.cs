using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_PDFDoc = System.IntPtr;
using TRN_PDFDocInfo = System.IntPtr;
using TRN_UString = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_PDFView = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> PDFDocInfo is a high-level utility class that can be used 
    /// to read and modify document's metadata.
    /// </summary>
    public class PDFDocInfo : IDisposable
    {
        // Fields
        internal TRN_PDFDocInfo mp_info;
        internal Object m_ref;

        ~PDFDocInfo()
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
            if (mp_info != IntPtr.Zero)
            {
                mp_info = IntPtr.Zero;
            }
        }

        // Methods
        internal PDFDocInfo(TRN_PDFDocInfo info, Object reference)
        {
            this.mp_info = info;
            this.m_ref = reference;
        }
        public PDFDocInfo(Obj tr)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoCreate(tr.mp_obj, ref mp_info));
            this.m_ref = tr.GetRefHandleInternal();
        }
        internal PDFDocInfo(PDFDocInfo c)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoCopy(c.mp_info, ref mp_info));
        }

        /// <summary> Gets the author.
        /// 
        /// </summary>
        /// <returns> The name of the person who created the document.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetAuthor()
        {
            UString ustr = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetAuthor(mp_info, ref ustr.mp_impl));
            return ustr.ConvToManagedStr();
        }
        /// <summary> Gets the author obj.
        /// 
        /// </summary>
        /// <returns> SDF/Cos string object representing document's author.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetAuthorObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetAuthorObj(mp_info, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary>Gets the creation date.			
        /// </summary>
        /// <returns>date and time the document was created, in human-readable form.
        /// </returns>
        /// <exception cref="PDFNetException">PDFNetException the PDFNet exception </exception>
        public Date GetCreationDate()
        {
            BasicTypes.TRN_Date result = new BasicTypes.TRN_Date();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetCreationDate(mp_info, ref result));
            return new Date(result);
        }
        /// <summary> Gets the creator.
        /// 
        /// </summary>
        /// <returns> If the document was converted to PDF from another
        /// format, the name of the application that created the original
        /// document from which it was converted.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetCreator()
        {
            UString ustr = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetCreator(mp_info, ref ustr.mp_impl));
            return ustr.ConvToManagedStr();
        }
        /// <summary> Gets the creator obj.
        /// 
        /// </summary>
        /// <returns> SDF/Cos string object representing document's creator.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetCreatorObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetCreatorObj(mp_info, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary> Gets the keywords.
        /// 
        /// </summary>
        /// <returns> Keywords associated with the document.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetKeywords()
        {
            UString ustr = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetKeywords(mp_info, ref ustr.mp_impl));
            string result = ustr.ConvToManagedStr();
            return result;
        }
        /// <summary> Gets the keywords obj.
        /// 
        /// </summary>
        /// <returns> SDF/Cos string object representing document's keywords.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetKeywordsObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetKeywordsObj(mp_info, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary>Gets the mod date.
        /// 
        /// </summary>
        /// <returns>date and time the document was most recently modified, in human-readable form.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Date GetModDate()
        {
            BasicTypes.TRN_Date result = new BasicTypes.TRN_Date();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetModDate(mp_info, ref result));
            return new Date(result);
        }
        /// <summary> Gets the producer.
        /// 
        /// </summary>
        /// <returns> If the document was converted to PDF from another format,
        /// the name of the application (for example, Distiller) that
        /// converted it to PDF.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetProducer()
        {
            UString ustr = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetProducer(mp_info, ref ustr.mp_impl));
            return ustr.ConvToManagedStr();
        }
        /// <summary> Gets the producer obj.
        /// 
        /// </summary>
        /// <returns> SDF/Cos string object representing document's producer.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetProducerObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetProducerObj(mp_info, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary> Gets the SDFObj.
        /// 
        /// </summary>
        /// <returns> document’s SDF/Cos 'Info' dictionary or NULL if
        /// the info dictionary is not available.
        /// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetSDFObj(mp_info, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary> Gets the subject.
        /// 
        /// </summary>
        /// <returns> The subject of the document.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetSubject()
        {
            UString ustr = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetSubject(mp_info, ref ustr.mp_impl));
            return ustr.ConvToManagedStr();
        }
        /// <summary> Gets the subject obj.
        /// 
        /// </summary>
        /// <returns> SDF/Cos string object representing document's subject.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetSubjectObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetSubjectObj(mp_info, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary> Gets the title.
        /// 
        /// </summary>
        /// <returns> The document’s title.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetTitle()
        {
            UString ustr = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetTitle(mp_info, ref ustr.mp_impl));
            return ustr.ConvToManagedStr();
        }
        /// <summary> Gets the title obj.
        /// 
        /// </summary>
        /// <returns> SDF/Cos string object representing document's title.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetTitleObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoGetTitleObj(mp_info, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary> Set the author of the document.
        /// 
        /// </summary>
        /// <param name="author">the new author
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetAuthor(string author)
        {
            UString str = new UString(author);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoSetAuthor(mp_info, str.mp_impl));
        }
        /// <summary> Set document’s creation date.
        /// 
        /// </summary>
        /// <param name="creation_date">The date and time the document was created.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetCreationDate(Date creation_date)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoSetCreationDate(mp_info, ref creation_date.mp_date));
        }
        /// <summary> Set document’s creator.
        /// 
        /// </summary>
        /// <param name="creator">The name of the application that created
        /// the original document.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetCreator(string creator)
        {
            UString str = new UString(creator);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoSetCreator(mp_info, str.mp_impl));
        }
        /// <summary> Set keywords associated with the document.
        /// 
        /// </summary>
        /// <param name="keywords">the new keywords
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetKeywords(string keywords)
        {
            UString str = new UString(keywords);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoSetKeywords(mp_info, str.mp_impl));
        }
        /// <summary> Set document’s modification date.
        /// 
        /// </summary>
        /// <param name="mod_date">The date and time the document was most
        /// recently modified.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetModDate(Date mod_date)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoSetModDate(mp_info, ref mod_date.mp_date));
        }
        /// <summary> Set document’s producer.
        /// 
        /// </summary>
        /// <param name="producer">The name of the application that generated PDF.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetProducer(string producer)
        {
            UString str = new UString(producer);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoSetProducer(mp_info, str.mp_impl));
        }
        /// <summary> Set the subject of the document.
        /// 
        /// </summary>
        /// <param name="subject">The subject of the document.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetSubject(string subject)
        {

            UString str = new UString(subject);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoSetSubject(mp_info, str.mp_impl));
        }
        /// <summary> Set document’s title.
        /// 
        /// </summary>
        /// <param name="title">New title of the document.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetTitle(string title)
        {
            UString str = new UString(title);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocInfoSetTitle(mp_info, str.mp_impl));
        }


        
    }
}
