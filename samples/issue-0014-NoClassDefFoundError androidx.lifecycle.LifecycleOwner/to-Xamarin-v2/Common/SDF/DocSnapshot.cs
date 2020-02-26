using System;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_DocSnapshot = System.IntPtr;
using TRN_PDFDoc = System.IntPtr;

namespace pdftron.SDF
{
	/// <summary>
	/// Represents a state of the document.
	/// </summary>
	public class DocSnapshot : IDisposable
	{
		internal TRN_DocSnapshot m_impl = IntPtr.Zero;

		public DocSnapshot(TRN_DocSnapshot impl_ptr)
		{
			m_impl = impl_ptr;
		}

		~DocSnapshot()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            Destroy();		
        }

        public void Destroy()
        {
        	if(m_impl != IntPtr.Zero)
        	{
        		TRN_DocSnapshot to_delete = m_impl;
        		m_impl = IntPtr.Zero;
            	PDFNetException.REX(PDFNetPINVOKE.TRN_DocSnapshotDestroy(ref to_delete));
            }
        }



		/// <summary>
        /// Returns a hash that is unique to particular document states.
        /// </summary>
        /// <returns>A hash that is unique to particular document states</returns>
		public uint GetHash()
		{
			uint result = 0;
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocSnapshotGetHash(m_impl, ref result));
			return result;
		}

		/// <summary>
        /// Returns whether this snapshot is valid.
        /// </summary>
        /// <returns>Whether this snapshot is valid</returns>
		public bool IsValid()
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocSnapshotIsValid(m_impl, ref result));
			return result;
		}

		/// <summary>
        /// Returns whether this snapshot's document state is equivalent to another.
        /// </summary>
        /// <param name="snapshot">the other snapshot with which to compare</param>
        /// <returns>Whether this snapshot's document state is equivalent to another</returns>
		public bool Equals(DocSnapshot snapshot)
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocSnapshotEquals(m_impl, snapshot.m_impl, ref result));
			return result;
		}
	}
}
