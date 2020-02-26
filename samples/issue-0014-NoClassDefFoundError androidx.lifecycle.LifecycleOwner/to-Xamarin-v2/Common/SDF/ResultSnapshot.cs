using System;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_ResultSnapshot = System.IntPtr;
using TRN_DocSnapshot = System.IntPtr;
using TRN_PDFDoc = System.IntPtr;

namespace pdftron.SDF
{
	/// <summary>
	/// Represents a transition between two document states.
	/// </summary>
	public class ResultSnapshot : IDisposable
	{
		internal TRN_ResultSnapshot m_impl = IntPtr.Zero;

		public ResultSnapshot(TRN_ResultSnapshot impl_ptr)
		{
			m_impl = impl_ptr;
		}

		~ResultSnapshot()
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
        		TRN_ResultSnapshot to_delete = m_impl;
        		m_impl = IntPtr.Zero;
            	PDFNetException.REX(PDFNetPINVOKE.TRN_ResultSnapshotDestroy(ref to_delete));
            }
        }



		/// <summary>
        /// Retrieves the document state to which this transition has transitioned.
        /// </summary>
        /// <returns>The current document state</returns>
		public DocSnapshot CurrentState()
		{
			TRN_DocSnapshot result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_ResultSnapshotCurrentState(m_impl, ref result));
			return new DocSnapshot(result);
		}

		/// <summary>
        /// Retrieves the document state from which this transition has transitioned.
        /// </summary>
        /// <returns>The previous document state</returns>
		public DocSnapshot PreviousState()
		{
			TRN_DocSnapshot result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_ResultSnapshotPreviousState(m_impl, ref result));
			return new DocSnapshot(result);
		}

		/// <summary>
        /// Returns whether this transition is valid or a null transition.
        /// </summary>
        /// <returns>Whether this transition is valid or a null transition</returns>
		public bool IsOk()
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_ResultSnapshotIsOk(m_impl, ref result));
			return result;
		}

		/// <summary>
        /// Returns whether this transition is a null transition.
        /// </summary>
        /// <returns>Whether this transition is a null transition</returns>
		public bool IsNullTransition()
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_ResultSnapshotIsNullTransition(m_impl, ref result));
			return result;
		}
	}
}
