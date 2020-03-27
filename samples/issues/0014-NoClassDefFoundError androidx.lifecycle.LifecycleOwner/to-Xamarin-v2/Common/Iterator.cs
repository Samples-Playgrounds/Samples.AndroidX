using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

using pdftron;

using TRN_Iterator = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_ItrData = System.IntPtr;

namespace pdftron.Common
{
    public abstract class Iterator<T> : IDisposable
    {
        internal TRN_Iterator mp_impl = IntPtr.Zero;

        ~Iterator()
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
                Destroy();
                mp_impl = IntPtr.Zero;
            }
        }

        internal Iterator()
        {
            this.mp_impl = IntPtr.Zero;
        }
        internal Iterator(TRN_Iterator impl)
        {
            this.mp_impl = impl;
        }

        internal void Destroy()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_IteratorDestroy(mp_impl));
        }

        /// <summary>
        /// iterates to next <c>T</c> object
        /// </summary>
        public void Next()
        {
            if (mp_impl == IntPtr.Zero)
                throw new PDFNetException();
            PDFNetException.REX(PDFNetPINVOKE.TRN_IteratorNext(mp_impl));
        }
        
        public abstract T Current();

        /// <summary>
        /// Determine if true if the iteration has more elements.
        /// </summary>
        /// <returns>true if the iteration has more elements.</returns>
        public bool HasNext()
        {
            if (mp_impl == IntPtr.Zero)
                throw new PDFNetException();
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_IteratorHasNext(mp_impl, ref result));
            return result;
        }

        public Iterator<T> op_Assign(Iterator<T> other)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_IteratorAssign(other.mp_impl, ref mp_impl));
            return this;
        }
    }
}
