using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron;
using pdftron.Common;

using TRN_NameTree = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_DictIterator = System.IntPtr;

namespace pdftron.SDF
{
    /// <summary> A NameTree is a common data structure in PDF. See section 3.8.5 'Name Trees' 
    /// in PDF Reference Manual for more details.
    /// 
    /// A name tree serves a similar purpose to a dictionary - associating keys and
    /// values - but by different means. NameTrees allow efficient storage of very 
    /// large association collections (string/Obj* maps). A NameTree can have many 
    /// more entries than a SDF/Cos dictionary can.
    /// 
    /// NameTree-s use SDF/Cos-style strings (not null-terminated C strings), which 
    /// may use Unicode encoding etc.
    /// 
    /// </summary>
    /// <code>  
    /// PDFDoc doc = new PDFDoc("../Data/PDFReference.pdf");
    /// NameTree dests = NameTree.find(doc.getSDFDoc(), "Dests");
    /// if (dests.isValid()) {
    /// // Traversing the NameTree
    /// for (DictIterator i = dests.getIterator(); i.hasNext(); i.next()) 
    /// string key = i.key().getAsPDFText(); // ...
    /// }
    /// </code>
    public class NameTree : IDisposable
    {
        internal TRN_NameTree mp_obj = IntPtr.Zero;
        internal Object m_ref;
        internal NameTree(TRN_NameTree imp, Object reference)
        {
            this.mp_obj = imp;
            this.m_ref = reference;
        }
        /// <summary> Create a high level NameTree wrapper around an existing SDF/Cos NameTree. 
        /// This does not copy the object.
        /// 
        /// </summary>
        /// <param name="name_tree">SDF/Cos root of the NameTree object.
        /// </param>
        public NameTree(Obj name_tree)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreeCreateFromObj(name_tree.mp_obj, ref mp_obj));
            this.m_ref = name_tree.GetRefHandleInternal();
        }
        /// <summary> Retrieves the NameTree inside the '/Root/Names' dictionary with the
        /// specified key name, or creates it if it does not exist.
        /// 
        /// </summary>
        /// <param name="doc">- The document in which the name tree is created.
        /// </param>
        /// <param name="name">- The name of the NameTree to create.
        /// </param>
        /// <returns> The newly created NameTree for the doc or an exising tree with
        /// the same key name.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks> although it is possible to create a name tree anywhere in the document 
        /// the convention is that all trees are located under '/Root/Names' dictionary.
        /// </remarks>
        public static NameTree Create(SDFDoc doc, String name)
        {
            TRN_NameTree result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreeCreate(doc.mp_doc, name, ref result));
            return new NameTree(result, doc);
        }
        /// <summary> Retrieves a name tree, with the given key name, from the '/Root/Names'
        /// dictionary of the doc.
        /// 
        /// </summary>
        /// <param name="doc">- The document in which to search for the name.
        /// </param>
        /// <param name="name">- The name of the name tree to find.
        /// </param>
        /// <returns> The requested NameTree. If the requested NameTree exists
        /// NameTree.IsValid() will return true, and false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static NameTree Find(SDFDoc doc, String name)
        {
            TRN_NameTree result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreeFind(doc.mp_doc, name, ref result));
            return new NameTree(result, doc);
        }

        /// <summary>Sets value to the specified <c>NameTree</c>
        /// </summary>
        /// <param name="p"><c>NameTree</c> object to set the value to
        /// </param>
        public void Set(NameTree p)/* { NameTree::Assign(this, p); }*/
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreeCopy(p.mp_obj, ref mp_obj));
        }
        //static NameTree Assign(NameTree lhs, NameTree rhs);
        /// <summary>Assignment operator</summary>
        /// <param name="rhs"><c>NameTree</c> object at the right of the operator
        /// </param>
        /// <returns><c>NameTree</c> object equal to the specified object
        /// </returns>
        public NameTree op_Assign(/*NameTree lhs, */NameTree rhs)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreeCopy(rhs.mp_obj, ref mp_obj));
            return this;
        }
        /// <summary> Checks if is valid.
        /// 
        /// </summary>
        /// <returns> whether this is a valid (non-null) NameTree. If the
        /// function returns false the underlying SDF/Cos object is null and
        /// the NameTree object should be treated as null as well.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreeIsValid(mp_obj, ref result));
            return result;
        }

        /// <summary> Gets the value.
        /// 
        /// </summary>
        /// <param name="key">the key
        /// </param>
        /// <returns> the value
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetValue(byte[] key)
        {
            TRN_Obj result = IntPtr.Zero;
            BasicTypes.TRN_String keyString = new BasicTypes.TRN_String();
            int psize = Marshal.SizeOf(key[0]) * key.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(key, 0, pnt, key.Length);

                keyString.str = pnt;
                keyString.len = System.Convert.ToUInt32(key.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreeGetValue(mp_obj, keyString, ref result));
                return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }
        /// <summary> Search for the specified key in the NameTree.
        /// 
        /// </summary>
        /// <param name="key">data buffer representing the key to be found.
        /// </param>
        /// <returns> If the key is present the function returns a NameTreeIterator the points
        /// to the given Key/Value pair. If the key is not found the function returns End()
        /// (a non-valid) iterator.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <code>  
        /// NameTreeIterator i = dests.find("MyKey", 5);
        /// if (i.hasNext()) {
        /// string key = i.getKey().getAsPDFText();
        /// Obj value = i.getValue();
        /// }
        /// </code>
        public NameTreeIterator GetIterator(byte[] key)
        {
            TRN_DictIterator result = IntPtr.Zero;
            BasicTypes.TRN_String keyString = new BasicTypes.TRN_String();
            int psize = Marshal.SizeOf(key[0]) * key.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(key, 0, pnt, key.Length);
                
                keyString.str = pnt;
                keyString.len = System.Convert.ToUInt32(key.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreeGetIterator(mp_obj, keyString, ref result));
                return new NameTreeIterator(result, this.m_ref);
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }
        /// <summary> Gets the iterator.
        /// 
        /// </summary>
        /// <returns> an iterator to the first key/value pair (i.e. NNTreeData) in
        /// the document. You can use the increment operator on the returned iterator to
        /// traverese all entries stored under the NameTree.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <code>  
        /// for (NameTreeIterator i = dests.getIterator(); i.hasNext(); i.next()) {
        /// string key = i.getKey().getAsPDFText();
        /// }
        /// </code>
        public NameTreeIterator GetIterator()
        {
            TRN_DictIterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreeGetIteratorBegin(mp_obj, ref result));
            return new NameTreeIterator(result, this.m_ref);
        }

        /// <summary> Puts a new entry in the name tree. If an entry with this key
        /// is already in the tree, it is replaced.
        /// 
        /// </summary>
        /// <param name="key">data buffer representing the key of the new entry.
        /// </param>
        /// <param name="value">the value
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Put(byte[] key, Obj value)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreePut(mp_obj, key, key.Length, value.mp_obj));
        }
        /// <summary> Removes the specified object from the tree. Does nothing if no object
        /// with that name exists.
        /// 
        /// </summary>
        /// <param name="key">data buffer representing the key of the entry to be removed.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Erase(byte[] key)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreeEraseKey(mp_obj, key, key.Length));
        }
        /// <summary> Removes the NameTree entry pointed by the iterator.
        /// 
        /// </summary>
        /// <param name="pos">the pos
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Erase(NameTreeIterator pos)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreeErase(mp_obj, pos.mp_impl));
        }
        /// <summary> Gets the SDFObj.
        /// 
        /// </summary>
        /// <returns> the object to the underlying SDF/Cos object. If the NameTree.IsValid()
        /// returns false the SDF/Cos object is NULL.
        /// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_NameTreeGetSDFObj(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
        /// <summary> Releases all resources used by the NameTree </summary>
        ~NameTree()
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
            if (mp_obj != IntPtr.Zero)
            {
                mp_obj = IntPtr.Zero;
            }
        }
    }
}