using System;

using pdftron.Common;

using TRN_ViewChangeCollection = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary>
    /// Class for collecting changes to a PDFDoc and/or viewer, which can be 
    /// passed to various functions to act on. Allows for chaining of 
    /// modifications, which can then be updated by PDFNet in the best possible
    /// way.
    /// </summary>
    public class ViewChangeCollection : IDisposable
	{
		internal TRN_ViewChangeCollection mp_collection = IntPtr.Zero;

		internal ViewChangeCollection(TRN_ViewChangeCollection impl)
		{
			this.mp_collection = impl;
		}
		internal IntPtr GetHandleInternal()
		{
			return this.mp_collection;
		}

        /// <summary>
        /// A default constructor for ViewChangeCollection
        /// </summary>
        public ViewChangeCollection()
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_ViewChangeCollectionCreate(ref mp_collection));
		}

		public ViewChangeCollection op_Assign(ViewChangeCollection r)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_ViewChangeCollectionAssign(mp_collection, r.mp_collection));
			return this;
		}

		/// <summary> Releases all resources used by the ActionParameter </summary>
		~ViewChangeCollection()
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
            if (mp_collection != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_ViewChangeCollectionDestroy(mp_collection));
                mp_collection = IntPtr.Zero;
            }
		}
	}
}