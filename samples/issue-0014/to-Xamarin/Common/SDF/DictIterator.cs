using System;
using System.Runtime.InteropServices;

using pdftron.Common;

using TRN_DictIterator = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.SDF
{
    /// <summary> DictIterator is used to traverse key-value pairs in a dictionary.	
    /// <example>
    /// DictIterator can be used to print out all the entries
    /// in a given Obj dictionary as follows: 
    /// <code>  
    /// DictIterator itr = dict.getDictIterator();
    /// while (itr.hasNext()) {
    /// Obj key = itr.key();		
    /// printf(key.getName());
    /// Obj value = itr.value();
    /// //...
    /// itr.next()
    /// }
    /// }
    /// </code></example>
    /// </summary>
    public class DictIterator : IDisposable
    {
        internal TRN_DictIterator mp_impl = IntPtr.Zero;
        internal Object m_ref;

        internal DictIterator(TRN_DictIterator imp, Object reference)
        {
            this.mp_impl = imp;
            this.m_ref = reference;
        }

        /// <summary> Advances the iterator to the next element of the collection.
		/// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Next()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DictIteratorNext(mp_impl));
        }

		/// <summary> Checks for next.
		/// 
		/// </summary>
		/// <returns> true if the iterator can be successfully advanced to the
		/// next element; false if the end collection is reached.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool HasNext()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DictIteratorHasNext(mp_impl, ref result));
            return result;
        }
		/// <summary>Key
		/// </summary>
		/// <returns>key of the current dictionary entry
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj Key()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DictIteratorKey(mp_impl, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
		/// <summary>Value
		/// </summary>
		/// <returns>value of the current dictionary entry
        /// </returns>
        /// <exception cref="PDFNetException">PDFNetException the PDFNet exception</exception>
        public Obj Value()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DictIteratorValue(mp_impl, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

		/// <summary>Sets value to the specified <c>DictIterator</c>
		/// </summary>
        /// <param name="p">another <c>DictIterator</c> object
        /// </param>
        public void Set(DictIterator p)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DictIteratorAssign(p.mp_impl, ref mp_impl));
        }
		/// <summary>checks if equals to the specified <c>DictIterator</c> object
		/// </summary>
		/// <param name="rhs">specified <c>DictIterator</c>
		/// </param>
        /// <returns>true if both are equal
        /// </returns>
        public override bool Equals(object rhs)
        {
            DictIterator i = rhs as DictIterator;
            if (i == null) return false;

            bool b = mp_impl.Equals(i.mp_impl);
            return b;
        }
		//static DictIterator Assign(DictIterator lhs, DictIterator rhs);
		/// <summary>Assignment operator</summary>
		/// <param name="rhs"><c>DictIterator</c> at the right side of the operator
		/// </param>
		/// <returns><c>DictIterator</c> that equals to the object at the right side of the operator
		/// </returns>
		public DictIterator op_Assign(DictIterator rhs)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DictIteratorAssign(rhs.mp_impl, ref mp_impl));
            return this;
        }
		/// <summary>Equality operator checks whether two <c>DictIterator</c> objects are the same.</summary>
		/// <param name="lhs"><c>DictIterator</c> at the left side of the operator
		/// </param>
		/// <param name="rhs"><c>DictIterator</c> at the right side of the operator
		/// </param>
		/// <returns>true if both objects are equal, false otherwise
		/// </returns>
		public static bool operator ==(DictIterator lhs, DictIterator rhs) 
        {
            if (System.Object.ReferenceEquals(lhs, null) && System.Object.ReferenceEquals(rhs, null))
                return true;
            if ( System.Object.ReferenceEquals(lhs, null) && !System.Object.ReferenceEquals(rhs, null) )
		        return false;

	        if ( !System.Object.ReferenceEquals(lhs, null) && System.Object.ReferenceEquals(rhs, null) )
		        return false;
            return (lhs.Equals(rhs));
        }
		/// <summary>Inequality operator checks whether two <c>DictIterator</c> objects are different.</summary>
		/// <param name="lhs"><c>DictIterator</c> at the left side of the operator
		/// </param>
		/// <param name="rhs"><c>DictIterator</c> at the right side of the operator
		/// </param>
		/// <returns>true if both objects are not equal, false otherwise
		/// </returns>
        public static bool operator !=(DictIterator lhs, DictIterator rhs)
        {
            if ( System.Object.ReferenceEquals(lhs, null) && System.Object.ReferenceEquals(rhs, null) )
		        return false;

	        if ( System.Object.ReferenceEquals(lhs, null) && !System.Object.ReferenceEquals(rhs, null) )
		        return true;

	        if ( !System.Object.ReferenceEquals(lhs, null) && System.Object.ReferenceEquals(rhs, null) )
		        return true;

            return !(lhs.Equals(rhs));
        }
		/// <summary> Releases all resources used by the DictIterator </summary>
		~DictIterator()
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
            Destroy();
        }
        public void Destroy()
        {
            if (mp_impl != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_DictIteratorDestroy(mp_impl));
                mp_impl = IntPtr.Zero;
            }
        }
    }
}
