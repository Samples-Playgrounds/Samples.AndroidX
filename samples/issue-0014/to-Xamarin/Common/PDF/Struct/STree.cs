using System;
using System.Collections.Generic;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_STree = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF.Struct
{
    /// <summary> STree is the root of the structure tree, which is a central repository 
    /// for information related to a PDF document's logical structure. There is at most 
    /// one structure tree in each document.
    /// </summary>
    public class STree : IDisposable
    {
        internal TRN_STree mp_tree = IntPtr.Zero;
        internal Object m_ref;

        ~STree()
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
            if (mp_tree != IntPtr.Zero)
            {
                mp_tree = IntPtr.Zero;
            }
        }
        internal STree(TRN_STree imp, Object reference)
        {
            this.mp_tree = imp;
            this.m_ref = reference;
        }

        /// <summary> Initialize a STree using an existing low-level Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="struct_dict">a low-level (SDF/Cos) dictionary representing the . 
		/// 
		/// </param>
		/// <remarks>  This constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public STree(Obj struct_dict)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_STreeCreate(struct_dict.mp_obj, ref mp_tree));
            this.m_ref = struct_dict.GetRefHandleInternal();
        }
		//TODO: constructor documentation missing
		/// <summary>
		/// </summary>
		/// <param name="doc">
		/// </param>
        public STree(PDFDoc doc)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_STreeCreateFromPDFDoc(doc.mp_doc, ref mp_tree));
        }
		/// <summary>Inserts the specified kid element after the given position as a kid of 
		/// the specified structure tree root. 
		/// </summary>
		/// <param name="kid">the kid element 
		/// </param>
		/// <param name="insert_before">The position after which the kid is inserted. If 
		/// element currently has no kids, insert_before is ignored.
		/// </param>
        public void Insert(SElement kid, int insert_before)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_STreeInsert(mp_tree, ref kid.mp_elem, insert_before));
        }
		/// <summary> Checks if is valid.
		/// 
		/// </summary>
		/// <returns> true if this is a valid STree object, false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_STreeIsValid(mp_tree, ref result));
            return result;
        }
		/// <summary> Gets the num kids.
		/// 
		/// </summary>
		/// <returns> The number of kids of the structure tree root.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetNumKids()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_STreeGetNumKids(mp_tree, ref result));
            return result;
        }
		/// <summary> Gets the kid.
		/// 
		/// </summary>
		/// <param name="index">The index of the kid to obtain.
		/// </param>
		/// <returns> The kid at an array index in the structure tree root.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public SElement GetKid(int index)
        {
            BasicTypes.TRN_SElement result = new BasicTypes.TRN_SElement();
            PDFNetException.REX(PDFNetPINVOKE.TRN_STreeGetKid(mp_tree, index, ref result));
            return new SElement(result, this.m_ref);
        }
		//SElement GetElement(Byte buf[]);
		/// <summary> Gets the element.
		/// 
		/// </summary>
		/// <param name="buf">the id_buf
		/// </param>
		/// <returns> the element
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public SElement GetElement(byte[] buf)
        {
            BasicTypes.TRN_SElement result = new BasicTypes.TRN_SElement();
            PDFNetException.REX(PDFNetPINVOKE.TRN_STreeGetElement(mp_tree, buf, buf.Length, ref result));
            return new SElement(result, this.m_ref);
        }
		//RoleMap* GetRoleMap();
		//ClassMap* GetClassMap();
		/// <summary> Gets the SDFObj.
		/// 
		/// </summary>
		/// <returns> Pointer to the underlying SDF/Cos object.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_STreeGetSDFObj(mp_tree, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
    }
}
