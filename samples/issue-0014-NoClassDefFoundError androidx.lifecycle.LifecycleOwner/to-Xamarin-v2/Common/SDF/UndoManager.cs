using System;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_UndoManager = System.IntPtr;
using TRN_PDFDoc = System.IntPtr;
using TRN_DocSnapshot = System.IntPtr;
using TRN_ResultSnapshot = System.IntPtr;

namespace pdftron.SDF
{
	/// <summary>
	/// Undo-redo interface; one-to-one mapped to document
	/// </summary>
	public class UndoManager : IDisposable
	{
		internal TRN_UndoManager m_impl = IntPtr.Zero;

		public UndoManager(TRN_UndoManager impl_ptr)
		{
			m_impl = impl_ptr;
		}

		~UndoManager()
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
        		TRN_UndoManager to_delete = m_impl;
        		m_impl = IntPtr.Zero;
            	PDFNetException.REX(PDFNetPINVOKE.TRN_UndoManagerDestroy(ref to_delete));
            }
        }



		/// <summary>
        /// Forget all changes in this manager (without changing the document).
        /// </summary>
        /// <returns>An invalid DocSnapshot</returns>
		public DocSnapshot DiscardAllSnapshots()
		{
			TRN_DocSnapshot result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_UndoManagerDiscardAllSnapshots(m_impl, ref result));
			return new DocSnapshot(result);
		}

		/// <summary>
        /// Restores to the previous snapshot point, if there is one.
        /// </summary>
        /// <returns>The resulting snapshot id</returns>
		public ResultSnapshot Undo()
		{
			TRN_ResultSnapshot result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_UndoManagerUndo(m_impl, ref result));
			return new ResultSnapshot(result);
		}

		/// <summary>
        /// Returns whether it is possible to undo from the current snapshot.
        /// </summary>
        /// <returns>Whether it is possible to undo from the current snapshot</returns>
		public bool CanUndo()
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_UndoManagerCanUndo(m_impl, ref result));
			return result;
		}

		/// <summary>
        /// Gets the previous state of the document. This state may be invalid if it is impossible to undo.
        /// </summary>
        /// <returns>The previous state of the document. This state may be invalid if it is impossible to undo</returns>
		public DocSnapshot GetNextUndoSnapshot()
		{
			TRN_DocSnapshot result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_UndoManagerGetNextUndoSnapshot(m_impl, ref result));
			return new DocSnapshot(result);
		}

		/// <summary>
        /// Restores to the next snapshot, if there is one.
        /// </summary>
        /// <returns>A representation of the transition to the next snapshot, if there is one</returns>
		public ResultSnapshot Redo()
		{
			TRN_ResultSnapshot result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_UndoManagerRedo(m_impl, ref result));
			return new ResultSnapshot(result);
		}

		/// <summary>
        /// Returns a boolean indicating whether it is possible to redo from the current snapshot.
        /// </summary>
        /// <returns>A boolean indicating whether it is possible to redo from the current snapshot</returns>
		public bool CanRedo()
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_UndoManagerCanRedo(m_impl, ref result));
			return result;
		}

		/// <summary>
        /// Gets the next state of the document. This state may be invalid if it is impossible to redo.
        /// </summary>
        /// <returns>The next state of the document. This state may be invalid if it is impossible to redo</returns>
		public DocSnapshot GetNextRedoSnapshot()
		{
			TRN_DocSnapshot result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_UndoManagerGetNextRedoSnapshot(m_impl, ref result));
			return new DocSnapshot(result);
		}

		/// <summary>
        /// Creates a snapshot of document state, transitions to the new snapshot.
        /// </summary>
        /// <returns>A representation of the transition</returns>
		public ResultSnapshot TakeSnapshot()
		{
			TRN_ResultSnapshot result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_UndoManagerTakeSnapshot(m_impl, ref result));
			return new ResultSnapshot(result);
		}
	}
}
