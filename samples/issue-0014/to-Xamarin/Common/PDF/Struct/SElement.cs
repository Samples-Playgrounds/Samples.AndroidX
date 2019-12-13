using System;
using System.Collections.Generic;
using System.Text;

using pdftronprivate.trn;
using pdftron.SDF;
using pdftron.Common;

using TRN_STree = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF.Struct
{
    /// <summary> SElement represents PDF structural elements, which are nodes in a tree   
    /// structure, defining a PDF document's logical structure.
    /// 
    /// Unlike the StructTree, SElement can have two different kinds 
    /// of children: another SElement or a ContentItem (which can be marked 
    /// content (MC), or a PDF object reference (OBJR)). 
    /// </summary> 
    public class SElement : IDisposable
    {
        internal BasicTypes.TRN_SElement mp_elem;
        internal Object m_ref;

        internal SElement(BasicTypes.TRN_SElement impl, Object reference)
        {
            this.mp_elem = impl;
            this.m_ref = reference;
        }

        /// <summary> Releases all resources used by the SElement </summary>
        ~SElement()
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
            if (mp_elem.obj != IntPtr.Zero || mp_elem.k != IntPtr.Zero)
            {
                mp_elem.k = IntPtr.Zero;
                mp_elem.obj = IntPtr.Zero;
            }
        }

        /// <summary> Initialize a SElement using an existing low-leval Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="dict">the dict
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  This constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public SElement(Obj dict)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementCreate(dict.mp_obj, ref mp_elem));
        }
		/// <summary> Create <c>SElement</c> with specified type
		/// </summary>
		/// <param name="doc">PDF document to create the <c>SElement</c> in
		/// </param>
		/// <param name="struct_type">structure type
		/// </param>
        public SElement(pdftron.PDF.PDFDoc doc, String struct_type)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementCreateFromPDFDoc(doc.mp_doc, struct_type, ref mp_elem));
        }
		/// <summary>Insert a kid <c>SElement</c>
		/// </summary>
		/// <param name="kid">kid <c>SElement</c>
		/// </param>
		/// <param name="insert_before">The position after which the kid is inserted. If element currently has no kids, insert_before is ignored.
		/// </param>
        public void Insert(SElement kid, int insert_before)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementInsert(ref mp_elem, ref kid.mp_elem, insert_before));
        }
		/// <summary>Creates content item
		/// </summary>
		/// <param name="doc">
		/// </param>
		/// <param name="page">
		/// </param>
		/// <param name="insert_before">The position after which the kid is inserted. If element currently has no kids, insert_before is ignored.
		/// </param>
		/// <returns>
		/// </returns>
        public Int32 CreateContentItem(pdftron.PDF.PDFDoc doc, Obj page, Int32 insert_before)
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementCreateContentItem(ref mp_elem, doc.mp_doc, page.mp_obj, insert_before, ref result));
            return result;
        }
		/// <summary> Checks if is valid.
		/// 
		/// </summary>
		/// <returns> true if this is a valid structure element object, false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementIsValid(ref mp_elem, ref result));
            return result;
        }
		/// <summary> Gets the type.
		/// 
		/// </summary>
		/// <returns> The element's structural element type. The type corresponds to
		/// the 'S' (i.e. subtype) key in the structure element dictionary.
		/// 
		/// The type identifies the nature of the structure element and its role
		/// within the document (such as a chapter, paragraph, or footnote).
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public String GetType()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementGetType(ref mp_elem, ref result));
            string res = System.Runtime.InteropServices.Marshal.PtrToStringUTF8(result);
            return res;
        }
		/// <summary> Gets the num kids.
		/// 
		/// </summary>
		/// <returns> The number of direct kids.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 GetNumKids()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementGetNumKids(ref mp_elem, ref result));
            return result;
        }
		/// <summary> Checks if is content item.
		/// 
		/// </summary>
		/// <param name="index">The index of the kid type to obtain.
		/// 
		/// To retrieve a content item at a given array index use GetAsContentItem(index),
		/// otherwise use GetAsStructElem(index)
		/// </param>
		/// <returns> true if the kid at a given array index is a content item,
		/// false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsContentItem(Int32 index)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementIsContentItem(ref mp_elem, index, ref result));
            return result;
        }
		/// <summary> Gets the as content item.
		/// 
		/// </summary>
		/// <param name="index">The index of the kid to obtain.
		/// </param>
		/// <returns> The kid at a given array index assuming that the kid is a ContentItem.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  use IsContentItem(index) prior to calling this method to make sure that
		/// the kid is indeed a ContentItem.</remarks>
        public ContentItem GetAsContentItem(Int32 index)
        {
            BasicTypes.TRN_ContentItem result = new BasicTypes.TRN_ContentItem();
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementGetAsContentItem(ref mp_elem, index, ref result));
            return new ContentItem(result, this.m_ref);
        }
		/// <summary> Gets the as struct elem.
		/// 
		/// </summary>
		/// <param name="index">The index of the kid to obtain.
		/// </param>
		/// <returns> The kid at a given array index assuming that the kid is a SElement.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  use IsContentItem(index) prior to calling this method to make sure that
		/// the kid is not a ContentItem and is another SElement.</remarks>
        public SElement GetAsStructElem(Int32 index)
        {
            BasicTypes.TRN_SElement result = new BasicTypes.TRN_SElement();
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementGetAsStructElem(ref mp_elem, index, ref result));
            return new SElement(result, this.m_ref);
        }
		/// <summary> Gets the parent.
		/// 
		/// </summary>
		/// <returns> The immediate ancestor element of the specified element in
		/// the structure tree.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If the element's parent is the structure tree root, the returned
		/// SElement will be not valid (i.e. element.IsValid() -> false) and the
		/// underlying SDF/Cos object will be NULL.</remarks>
        public SElement GetParent()
        {
            BasicTypes.TRN_SElement result = new BasicTypes.TRN_SElement();
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementGetParent(ref mp_elem, ref result));
            return new SElement(result, this.m_ref);
        }
		/// <summary>Gets the struct tree root
		/// </summary>
		/// <returns>structure tree root of the document that directly or indirectly contains this element
        /// </returns>
        /// <exception cref="PDFNetException">PDFNetException the PDFNet exception</exception>
        public STree GetStructTreeRoot()
        {
            TRN_STree result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementGetStructTreeRoot(ref mp_elem, ref result));
            return new STree(result, this.m_ref);
        }

		/// <summary> Checks for title.
		/// 
		/// </summary>
		/// <returns> if this SElement has title.
		/// 
		/// The title of the structure element, a text string representing it in
		/// human-readable form.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean HasTitle()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementHasTitle(ref mp_elem, ref result));
            return result;
        }
		/// <summary> Gets the title.
		/// 
		/// </summary>
		/// <returns> The title of this structure element.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public String GetTitle()
        {
            UString str = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementGetTitle(ref mp_elem, ref str.mp_impl));
            return str.ConvToManagedStr();
        }
		/// <summary> Gets the ID.
		/// 
		/// </summary>
		/// <returns> the ID of an element, or NULL if the ID is not defined.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetID()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementGetID(ref mp_elem, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Gets the actual text.
		/// 
		/// </summary>
		/// <returns> The ActualText associated with this element.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The ActualText can be defined as an empty string. To differentiate
		/// between an ActualText string of zero length and no ActualText being defined,
		/// use HasActualText().</remarks>
        public Boolean HasActualText()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementHasActualText(ref mp_elem, ref result));
            return result;
        }
		/// <summary> Checks for actual text.
		/// 
		/// </summary>
		/// <returns> if this structure element defines ActualText.
		/// 
		/// ActualText is an exact replacement for the structure element and its children.
		/// This replacement text is useful when extracting the document's contents in
		/// support of accessibility to users with disabilities or for other purposes.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public String GetActualText()
        {
            UString str = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementGetActualText(ref mp_elem, ref str.mp_impl));
            return str.ConvToManagedStr();
        }
		/// <summary> Checks for alt.
		/// 
		/// </summary>
		/// <returns> if this structure element defines Alt text.
		/// 
		/// Alt text is an alternate description of the structure element and
		/// its children in human-readable form, which is useful when extracting
		/// the document’s contents in support of accessibility.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean HasAlt()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementHasAlt(ref mp_elem, ref result));
            return result;
        }
		/// <summary> Gets the alt.
		/// 
		/// </summary>
		/// <returns> The alternate text associated with this element.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Alt text can be defined as an empty string. To differentiate
		/// between an Alt text string of zero length and no Alt text being defined,
		/// use HasAlt().</remarks>
        public String GetAlt()
        {
            UString str = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementGetAlt(ref mp_elem, ref str.mp_impl));
            return str.ConvToManagedStr();
        }
		/// <summary> Gets the sDF obj.
		/// 
		/// </summary>
		/// <returns> Pointer to the underlying SDF/Cos object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SElementGetSDFObj(ref mp_elem, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
    }
}
