using System;

using pdftron.Common;

using TRN_OCGContext = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.OCG
{
    /// <summary> <p>
    /// The OCG::Context object represents an optional-content context in a document, within which 
    /// document objects such as words or annotations are visible or hidden. The context keeps track 
    /// of the ON-OFF states of all of the optional-content groups (OCGs) in a document. Content is 
    /// or is not visible with respect to the OCG states stored in a specific context. Unlike other 
    /// objects in OCG namespace, the OCG::Context does not correspond to any explicit PDF structure.
    /// </p><p>
    /// Each PDFView has a default context (PDF::GetOCGContext()) that it uses for on-screen drawing 
    /// and that determines the default state for any drawing. The context has flags that control 
    /// whether to draw content that is marked as optional, and whether to draw content that is not 
    /// marked as optional.
    /// </p><p>
    /// When enumerating page content, OCG::Context can be passed as a parameter in ElementReader.Begin() 
    /// method. When using PDFDraw, PDFRasterizer, or PDFViewCtrl class to render PDF pages use 
    /// SetOCGContext() method to select an OC context.
    /// </p><p>
    /// There can be more than one Context object, representing different combinations of OCG states. 
    /// You can change the states of OCGs within any context. You can build contexts with your own 
    /// combination of OCG states, and issue drawing or enumeration commands using that context.
    /// For example, you can pass an optional-content context to ElementReader.Begin(). You can save 
    /// the resulting state information as part of the configuration (e.g. using Config::SetInit methods), 
    /// but the context itself has no corresponding PDF representation, and is not saved. 
    /// </p>
    /// </summary>
    public class Context : IDisposable
	{
        internal TRN_OCGContext mp_impl = IntPtr.Zero;
        internal Object m_ref;
        private bool m_owner = true;

		internal Context(TRN_OCGContext impl, Object reference) 
		{
            this.mp_impl = impl;
            this.m_ref = reference;
            this.m_owner = false;
		}
		internal IntPtr GetHandleInternal() 
		{
			return mp_impl;
		}

        /// <summary> Copy constructor.
		/// 
		/// </summary>
		/// <param name="context">Another context from which to take initial OCG states.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Context(Context context)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGContextCopy(context.mp_impl, ref mp_impl));
            this.m_ref = context.m_ref;
            this.m_owner = true;
        }
		/// <summary> Create a context object that represents an optional-content state of the
		/// document from a given configuration.
		/// 
		/// </summary>
		/// <param name="config">A configuration from which to take initial OCG states.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Context(Config config)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGContextCreateFromConfig(config.mp_obj, ref mp_impl));
            m_ref = config.m_ref;
            this.m_owner = true;
        }

		// bool IsValid() { return mp_obj!=0; }
		/// <summary> Gets the state.
		/// 
		/// </summary>
		/// <param name="group">The optional-content group (OCG) that is queried.
		/// </param>
		/// <returns> the ON-OFF states (true or false) for the given optional-content
		/// group (OCG) in this OC context.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool GetState(Group group)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGContextGetState(mp_impl, group.mp_obj, ref result));
            return result;
        }
		/// <summary> Sets the ON-OFF states for the given optional-content group (OCG) in this
		/// context.
		/// 
		/// </summary>
		/// <param name="group">The optional-content group (OCG) that is queried.
		/// </param>
		/// <param name="state">true for 'ON' and false for 'OFF'.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetState(Group group, bool state)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGContextSetState(mp_impl, group.mp_obj, state));
        }
		/// <summary> Sets the sates of all OCGs in the context to ON or OFF.
		/// 
		/// </summary>
		/// <param name="all_on">A flag used to specify whether the OCG states should be set
		/// to ON (if true), or OFF (if false).
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void ResetStates(bool all_on)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGContextResetStates(mp_impl, all_on));
        }
		/// <summary> Sets the non-OC status for this context. Content that is not marked 
		/// as optional content is drawn (or element.IsOCVisible()) when 'draw_non_OC' 
		/// is true, and not drawn/visible otherwise.
		/// </summary>
		/// <param name="draw_non_OC">draw_non_OC A flag specifying whether the content that is not 
		/// marked as optional should be treated as visible.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetNonOCDrawing(bool draw_non_OC)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGContextSetNonOCDrawing(mp_impl, draw_non_OC));
        }
		/// <summary> Gets the non oc drawing.
		/// 
		/// </summary>
		/// <returns> the non-OC status for this context. The flag indicates whether the
		/// content that is not marked as optional should be treated as visible.
		/// For more information, please see SetNonOCDrawing().
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool GetNonOCDrawing()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGContextGetNonOCDrawing(mp_impl, ref result));
            return result;
        }

		/// <summary> Sets the drawing and enumeration type for this context. This type, together
		/// with the visibility determined by the OCG and OCMD states, controls whether
		/// content that is marked as optional content is drawn or enumerated.
		/// 
		/// </summary>
		/// <param name="oc_draw_mode">A flag specifying the visibility of optional content.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetOCDrawMode(OCDrawMode oc_draw_mode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGContextSetOCDrawMode(mp_impl, oc_draw_mode));
        }

		/// <summary>Gets OC drawing mode
		/// </summary>
        /// <returns>OC drawing mode
        /// </returns>
        public OCDrawMode GetOCMode()
        {
            OCDrawMode result = OCDrawMode.e_NoOC;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGContextGetOCMode(mp_impl, ref result));
            return result;
        }
        /// <summary> Releases all resources used by the Context </summary>
        ~Context()
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
            if (m_owner)
            {
                // Check to see if Dispose has already been called. 
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Clean up native resources
                Destroy();
            }            
        }
        public void Destroy()
        {
            if (mp_impl != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_OCGContextDestroy(mp_impl));
                mp_impl = IntPtr.Zero;
            }         
        }

        // Nested Types
        ///<summary>OCDrawMode controls drawing or enumerating the page with respect to optional content.
		/// Together with the value of SetNonOCDrawing this mode controls drawing or enumerating 
		/// content on a page with optional content: 
		/// <ul>
		/// <li>Content that is marked as optional content is drawn or not drawn according to the 
		/// OCDrawMode and the visibility state as determined by the Optional Content Groups (OCGs) 
		/// and OCMDs. 
		/// </li>
		/// <li>Content that is not marked as optional content is drawn when GetNonOCDrawing() is 
		/// true, and not drawn when GetNonOCDrawing() is false.
		/// </li>
		/// </ul></summary>
        public enum OCDrawMode
        {
            ///<summary>Draw or enumerate optional content that is visible, according to the current state of 
			/// Optional Content Groups (OCGs) and Optional Content Membership Dictionaries (OCMDs). 
			/// This is the default mode.</summary>
			e_VisibleOC,
			///<summary>Draw or enumerate all optional content, regardless of its visibility state. If the 
			/// context's 'SetNonOCDrawing' is enabled, all contents of document are shown. </summary>
			e_AllOC,
			///<summary>Draw or enumerate no optional content, regardless of its visibility state. If the 
			/// context's 'SetNonOCDrawing' is not enabled, nothing is drawn, resulting in a blank page.</summary>
			e_NoOC
        }

	}
}

