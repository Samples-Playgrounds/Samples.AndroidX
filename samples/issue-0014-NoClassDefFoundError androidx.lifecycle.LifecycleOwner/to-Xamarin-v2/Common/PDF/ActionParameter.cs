using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_ActionParameter = System.IntPtr;
using TRN_Action = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary>
    /// Container for parameters used in handling various actions
    /// </summary>
    public class ActionParameter : IDisposable
	{
		internal TRN_ActionParameter mp_parameter = IntPtr.Zero;

		internal ActionParameter(TRN_ActionParameter impl)
		{
			this.mp_parameter = impl;
		}
		internal IntPtr GetHandleInternal()
		{
			return this.mp_parameter;
		}

        /// <summary>
        /// Construct an ActionParameter object
        /// </summary>
        ///
        /// <param name="action">the action object
        /// </param>
        ///
        public ActionParameter(Action action)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_ActionParameterCreate(action.mp_action, ref mp_parameter));
		}
        /// <summary>
        /// Construct an ActionParameter object
        /// </summary>
        ///
        /// <param name="action">the action object
        /// </param>
        /// <param name="annot">the annot object
        /// </param>
        ///
        public ActionParameter(Action action, Annot annot)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_ActionParameterCreateWithAnnot(action.mp_action, annot.mp_annot, ref mp_parameter));
		}
        /// <summary>
        /// Construct an ActionParameter object
        /// </summary>
        ///
        /// <param name="action">the action object
        /// </param>
        /// <param name="field">the field object
        /// </param>
        ///
        public ActionParameter(Action action, Field field)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_ActionParameterCreateWithField(action.mp_action, ref field.mp_field, ref mp_parameter));
		}
        /// <summary>
        /// Construct an ActionParameter object
        /// </summary>
        ///
        /// <param name="action">the action object
        /// </param>
        /// <param name="page">the page object
        /// </param>
        ///
        public ActionParameter(Action action, Page page)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_ActionParameterCreateWithPage(action.mp_action, page.mp_page, ref mp_parameter));
		}
        /// <summary>
        /// Get the action object 
        /// </summary>
        /// <return> The action object
        /// </return>
        public Action GetAction()
		{
			TRN_Action result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_ActionParameterGetAction(mp_parameter, ref result));
			return new Action(result, null);
		}

		/// <summary> Releases all resources used by the ActionParameter </summary>
		~ActionParameter()
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
            if (mp_parameter != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_ActionParameterDestroy(mp_parameter));
                mp_parameter = IntPtr.Zero;
            }
		}
	}
}