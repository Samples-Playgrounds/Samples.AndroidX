using System;

using pdftronprivate.trn;
using pdftron;
using pdftron.Common;

using TRN_NumberTree = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_DictIterator = System.IntPtr;

namespace pdftron.SDF
{
    /// <summary> A NumberTree is a common data structure in PDF. See section 3.8.6 'Number Trees' 
    /// in PDF Reference Manual for more details.
    /// 
    /// A number tree serves a similar purpose to a dictionary - associating keys and
    /// values - but by different means. NumberTrees allow efficient storage of very 
    /// large association collections (number/Obj* maps). A NumberTree can have many 
    /// more entries than a SDF/Cos dictionary can.
    /// </summary>
    /// <example>
    /// Sample code:
    /// <code>
    /// PDFDoc doc = new PDFDoc("../Data/test.pdf");
    /// NumberTree labels = doc.getRoot().get("PageLabels").value());
    /// if (labels.isValid()) {
    /// // Traversing the NumberTree 
    /// for (NumberTreeIterator i = labels.GetIterator(); i.HasNext(); i.Next()) 
    /// num = i.Key().GetNumber();
    /// }
    /// </code>
    /// </example>
    internal class NumberTree : IDisposable
    {
        internal TRN_NumberTree mp_obj = IntPtr.Zero;
        internal Object m_ref;
        internal NumberTree(TRN_NumberTree imp, Object reference)
        {
            this.mp_obj = imp;
            this.m_ref = reference;
        }
        /// <summary> Create a high level NumberTree wrapper around an existing SDF/Cos NumberTree. 
		/// This does not copy the object.
		/// 
		/// </summary>
        /// <param name="num_tree">SDF/Cos root of the NumberTree object.
        /// </param>
        public NumberTree(Obj num_tree)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_NumberTreeCreate(num_tree.mp_obj, ref mp_obj));
            this.m_ref = num_tree.GetRefHandleInternal();
        }

		/// <summary>Sets value to the specified <c>NumberTree</c>
		/// </summary>
		/// <param name="p">another <c>NumberTree</c> object
		/// </param>
        public void Set(NumberTree p)/* { NumberTree::Assign(this, p); }*/
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_NumberTreeCopy(p.mp_obj, ref mp_obj));
        }
		//static NumberTree Assign(NumberTree lhs, NumberTree rhs);
		/// <param name="rhs"><c>NumberTree</c> at the right side of the operator
		/// </param>
		/// <returns><c>NumberTree</c> that equals to the object at the right side of the operator
		/// </returns>
        public NumberTree op_Assign(/*NumberTree lhs, */NumberTree rhs)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_NumberTreeCopy(rhs.mp_obj, ref mp_obj));
            return this;
        }
		/// <summary> Checks if is valid.
		/// 
		/// </summary>
		/// <returns> whether this is a valid (non-null) NumberTree. If the
		/// function returns false the underlying SDF/Cos object is null and
		/// the NumberTree object should be treated as null as well.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_NumberTreeIsValid(mp_obj, ref result));
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
        public Obj GetValue(Int32 key)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_NumberTreeGetValue(mp_obj, key, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Search for the specified key in the NumberTree.
		/// 
		/// </summary>
		/// <param name="key">the number representing the key to be found.		
		/// </param>
		/// <example>
		/// Sample code:
		/// <code>
		/// NumberTreeIterator i = dests.find(5);
		/// if (i.hasNext()) {
		/// double key = i.getKey()->getNumber();
		/// Obj value=i.getValue();
		/// }
		/// </code>
		/// </example>
		/// <returns> If the key is present the function returns a NumberTreeIterator the points
		/// to the given Key/Value pair. If the key is not found the function returns End()
		/// (a non-valid) iterator.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public NumberTreeIterator GetIterator(Int32 key)
        {
            TRN_DictIterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_NumberTreeGetValue(mp_obj, key, ref result));
            return new NumberTreeIterator(result, this.m_ref);
        }
		/// <summary>Gets iterator
		/// </summary>
        /// <returns><c>NumberTreeIterator</c> object
        /// </returns>
        public NumberTreeIterator GetIterator()
        {
            TRN_DictIterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_NumberTreeGetIteratorBegin(mp_obj, ref result));
            return new NumberTreeIterator(result, this.m_ref);
        }

		/// <summary> Puts a new entry in the name tree. If an entry with this number
		/// is already in the tree, it is replaced.
		/// 
		/// </summary>
		/// <param name="key">A number representing the key of the new entry.
		/// </param>
		/// <param name="value">the value
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Put(Int32 key, Obj value)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_NumberTreePut(mp_obj, key, value.mp_obj));
        }
		/// <summary> Removes the specified object from the tree. Does nothing if no object
		/// with that number exists.
		/// 
		/// </summary>
		/// <param name="key">A number representing the key of the entry to be removed.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Erase(Int32 key)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_NumberTreeEraseKey(mp_obj, key));
        }
		/// <summary> Removes the NumberTree entry pointed by the iterator.
		/// 
		/// </summary>
		/// <param name="pos">the pos
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Erase(NumberTreeIterator pos)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_NumberTreeErase(mp_obj, pos.mp_impl));
        }
		/// <summary> Gets the SDFObj.
		/// 
		/// </summary>
		/// <returns> the object to the underlying SDF/Cos object. If the NumberTree.IsValid()
		/// returns false the SDF/Cos object is NULL.
		/// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_NumberTreeGetSDFObj(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Releases all resources used by the NumberTree </summary>
        ~NumberTree()
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