using System;
using System.Collections.Generic;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Obj = System.IntPtr;
using TRN_Page = System.IntPtr;

namespace pdftron.PDF.Struct
{
    /// <summary> Content items are graphical objects that exist in the document independently 
    /// of the structure tree but are associated with structure elements.
    /// 
    /// Content items are leaf nodes of the structure tree.
    /// </summary>
    public class ContentItem : IDisposable
    {
        internal BasicTypes.TRN_ContentItem mp_imp;
        internal Object m_ref;

        internal ContentItem(BasicTypes.TRN_ContentItem impl, Object reference)
        {
            this.mp_imp = impl;
            this.m_ref = reference;
        }

        /// <summary> Releases all resources used by the ContentItem </summary>
        ~ContentItem()
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
            if (mp_imp.o != IntPtr.Zero || mp_imp.p != IntPtr.Zero)
            {
                mp_imp.o = IntPtr.Zero;
                mp_imp.p = IntPtr.Zero;
            }
        }

        /// <summary> Gets the type.
		/// 
		/// </summary>
		/// <returns> the content item type.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Type GetType()
        {
            Type result = Type.e_Unknown;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentItemGetType(ref mp_imp, ref result));
            return result;    
        }
		
		/// <summary>Find the parent structure element
		/// </summary>
		/// <returns>parent structure element
        /// </returns>
        /// <exception cref="PDFNetException">PDFNetException the PDFNet exception </exception>
        public SElement GetParent()
        {
            BasicTypes.TRN_SElement result = new BasicTypes.TRN_SElement();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentItemGetParent(ref mp_imp, ref result));
            return new SElement(result, this.m_ref);
        }

		/// <summary> The page on which the marked content is drawn, whether directly as part of
		/// page content or indirectly by being in a Form XObject or annotation drawn
		/// on that page.
		/// 
		/// </summary>
		/// <returns> the page
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Page GetPage()
        {
            TRN_Page result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentItemGetPage(ref mp_imp, ref result));
            return new Page(result, this.m_ref);
        }

		/// <summary> Gets the SDFObj.
		/// 
		/// </summary>
		/// <returns> Pointer to the underlying SDF/Cos object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentItemGetSDFObj(ref mp_imp, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }

		// Type specific methods -----------------------------
		/// <summary> Gets the MCID.
		/// 
		/// </summary>
		/// <returns> mcid (marked-content identifier).
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method only applies to content items with types e_MCR or e_MCID. </remarks>
        public Int32 GetMCID()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentItemGetMCID(ref mp_imp, ref result));
            return result;
        }
		/// <summary> Gets the containing stm.
		/// 
		/// </summary>
		/// <returns> The stream object that contains the marked-content sequence.
		/// The function will return a non-NULL object only if the marked-content
		/// sequence resides in a content stream other than the content stream for the
		/// page (e.g. in a form XObject).
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method only applies to content items with type e_MCR. </remarks>
        public Obj GetContainingStm()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentItemGetContainingStm(ref mp_imp, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Gets the stm owner.
		/// 
		/// </summary>
		/// <returns> NULL or the PDF object owning the stream returned by
		/// GetContainingStm() (e.g. the annotation to which an appearance stream
		/// belongs).
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  this method only applies to content items with type e_MCR. </remarks>
        public Obj GetStmOwner()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentItemGetStmOwner(ref mp_imp, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Gets the ref obj.
		/// 
		/// </summary>
		/// <returns> The referenced object.
		/// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  this method only applies to content items with type e_OBJR. </remarks>
        public Obj GetRefObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ContentItemGetRefObj(ref mp_imp, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }

        // Nested Types
        ///<summary>Content Item types</summary>
        public enum Type
        {
            ///<summary>marked-content reference.</summary>
            e_MCR,
            ///<summary>marked-content identifier.</summary>
            e_MCID,
            ///<summary>object reference dictionary.</summary>
            e_OBJR,
            ///<summary>unknown content type.</summary>
            e_Unknown
        }

    }
}