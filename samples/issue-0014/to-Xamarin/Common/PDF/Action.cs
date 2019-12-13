using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using pdftron.Common;
using pdftron.SDF;

using TRN_Action = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Destination = System.IntPtr;
using TRN_Obj = System.IntPtr;
using pdftronprivate.trn;

namespace pdftron.PDF
{
    /// <summary> Actions are typically what happens when a user clicks on a link or bookmark.
    /// 
    /// Instead of simply jumping to a destination in the document, an annotation or
    /// outline item can specify an action for the viewer application to perform, such 
    /// as launching an application, playing a sound, or changing an annotation’s 
    /// appearance state.
    /// 
    /// </summary>
    /// <remarks>  Although the Action class provides utility functions for most commonly used
    /// action types, it is possible to read, write, and edit any action type using SDF API 
    /// and dictionary entries described in Section 8.5 in the PDF Reference Manual.</remarks>
    public class Action : IDisposable
    {
        internal TRN_Action mp_action = IntPtr.Zero;

        private Object m_ref;

        /// <summary> Releases all resources used by the Action </summary>
        ~Action()
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
            if (mp_action != IntPtr.Zero)
            {
                mp_action = IntPtr.Zero;
            }
        }

        // Methods
        internal Action(TRN_Action impl, Object reference)
        {
            this.mp_action = impl;
            this.m_ref = reference;
        }
        internal IntPtr GetHandleInternal()
        {
            return mp_action;
        }

        /// <summary> A constructor. Creates an Action and initializes it using given Cos/SDF object.
	    /// 
	    /// </summary>
	    /// <param name="a">Pointer to the Cos/SDF object.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The constructor does not copy any data, but is instead the logical 
        /// equivalent of a type cast.</remarks>
        public Action(Obj a)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreate(a.mp_obj, ref mp_action));
            this.m_ref = a.GetRefHandleInternal();
        }
	    /// <summary> Creates a new 'GoTo'action. GoTo action takes the user to the
	    /// specified Destination view located in the same document.
	    /// 
	    /// </summary>
	    /// <param name="dest">A Destination for the new Action.
	    /// </param>
	    /// <returns> the action
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  This method can only be used for destinations in the same
        /// document as the source document. For cross-document links use
        /// <c>Action.CreateGotoRemote().</c></remarks>
        public static Action CreateGoto(Destination dest)
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreateGoto(dest.mp_dest, ref result));
            return result != IntPtr.Zero ? new Action(result, dest.m_ref) : null;
        }
	    /// <summary> Creates a new 'GoTo' action using a 'Named Destination'. GoTo action
	    /// takes the user to the specified 'Named Destination' view located in the
	    /// same document.
	    /// 
	    /// </summary>
	    /// <param name="key">- a string buffer representing the destination name. The named
	    /// destination will be stored in document's '/Dest' SDF.NameTree.
	    /// </param>
	    /// <param name="dest">The explicit destination used to create the named destination.
	    /// </param>
	    /// <returns> the action
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Action CreateGoto(string key, Destination dest)
        {
            TRN_Action result = IntPtr.Zero;
            byte[] k = Encoding.ASCII.GetBytes(key); // note: using ASCII because PDFNetCLI2 uses ConvToANSIString
            BasicTypes.TRN_String keyString = new BasicTypes.TRN_String();
            int psize = Marshal.SizeOf(k[0]) * k.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(k, 0, pnt, k.Length);

                keyString.str = pnt;
                keyString.len = System.Convert.ToUInt32(k.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreateGotoWithKey(keyString, dest.mp_dest, ref result));
                return result != IntPtr.Zero ? new Action(result, dest.m_ref) : null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }
	    /// <summary> Creates a new 'GoTo' action using a 'Named Destination'. GoTo action
	    /// takes the user to the specified 'Named Destination' view located in the
	    /// same document.
	    /// 
	    /// </summary>
	    /// <param name="key">a string buffer representing the destination name. The named
	    /// destination will be stored in document's '/Dest' SDF.NameTree.
	    /// </param>
	    /// <param name="key_sz">size of the key buffer
	    /// </param>
	    /// <param name="dest">The explicit destination used to create the named destination.
	    /// </param>
	    /// <returns> the action
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Action CreateGoto(byte[] key, int key_sz, Destination dest)
        {
            TRN_Action result = IntPtr.Zero;
            BasicTypes.TRN_String keyString = new BasicTypes.TRN_String();
            int psize = Marshal.SizeOf(key[0]) * key.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(key, 0, pnt, key.Length);
                keyString.str = pnt;
                keyString.len = System.Convert.ToUInt32(key.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreateGotoWithKey(keyString, dest.mp_dest, ref result));
                return result != IntPtr.Zero ? new Action(result, dest.m_ref) : null;
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }

	    /// <summary> Creates a new 'GoToR'action. A remote go-to action is similar to an
	    /// ordinary go-to action but jumps to a destination in another PDF file
	    /// instead of the current file.
	    /// 
	    /// </summary>
	    /// <param name="file">The file referred to by the action.
	    /// </param>
	    /// <param name="page_num">A page number within the remote document. The first page is
	    /// numbered 0.
	    /// </param>
	    /// <returns> the action
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  A flag specifying whether to open the destination document
	    /// in a new window. If new_window is false, the destination document replaces
        /// the current document in the same window, otherwise the viewer application
        /// should behave in accordance with the current user preference.</remarks>
        public static Action CreateGotoRemote(FileSpec file, Int32 page_num)
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreateGotoRemote(file.mp_impl, page_num, ref result));
            return result != IntPtr.Zero ? new Action(result, file.m_ref) : null;
        }
	    /// <summary> Creates a new 'GoToR'action. See the above method for details.
	    /// 
	    /// </summary>
	    /// <param name="file">The file referred to by the action.
	    /// </param>
	    /// <param name="page_num">A page number within the remote document. The first page is
	    /// numbered 0.
	    /// </param>
	    /// <param name="new_window">the new_window
	    /// </param>
	    /// <returns> the action
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  A flag specifying whether to open the destination document
	    /// in a new window. If new_window is false, the destination document replaces
        /// the current document in the same window, otherwise the viewer application
        /// should behave in accordance with the current user preference.</remarks>
        public static Action CreateGotoRemote(FileSpec file, Int32 page_num, bool new_window)
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreateGotoRemoteSetNewWindow(file.mp_impl, page_num, new_window, ref result));
            return result != IntPtr.Zero ? new Action(result, file.m_ref) : null;
        }

	    /// <summary> Create a new URI action. The URL action is typically resolved by opening a URL in
	    /// the default web browser.
	    /// 
	    /// </summary>
	    /// <param name="doc">The document in which to create the action.
	    /// </param>
	    /// <param name="uri">The uniform resource identifier to resolve, encoded in 7-bit ASCII.
	    /// A uniform resource identifier (URI) is a string that identifies (resolves to) a resource
	    /// on the Internet—typically a file that is the destination of a hypertext link, although
	    /// it can also resolve to a query or other entity. (URIs are described in Internet RFC 2396,
	    /// Uniform Resource Identifiers (URI).
	    /// </param>
	    /// <returns> the action
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Action CreateURI(SDFDoc doc, string uri)
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreateURI(doc.mp_doc, uri, ref result));
            return result != IntPtr.Zero ? new Action(result, doc) : null;
        }
	    /// <summary> Creates a new 'SubmitForm'action. A submit-form action transmits the names
	    /// and values of selected interactive form fields to a specified uniform
	    /// resource locator (URL), presumably the address of a Web server that will
	    /// process them and send back a response.
	    /// 
	    /// </summary>
	    /// <param name="url">A URL file specification giving the uniform resource locator (URL)
	    /// of the script at the Web server that will process the submission.
	    /// </param>
	    /// <returns> the action
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Action CreateSubmitForm(FileSpec url)
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreateSubmitForm(url.mp_impl, ref result));
            return result != IntPtr.Zero ? new Action(result, url.m_ref) : null;
        }

	    /// <summary> Creates a new 'Launch' action. A launch action opens up a file using the
	    /// most appropriate program.
	    /// 
	    /// </summary>
	    /// <param name="doc">the document in which to create the action
	    /// </param>
	    /// <param name="path">full path of the file to be opened
	    /// </param>
        /// <returns> the action
        /// </returns>
        public static Action CreateLaunch(SDFDoc doc, string path)
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreateLaunch(doc.mp_doc, path, ref result));
            return result != IntPtr.Zero ? new Action(result, doc) : null;
        }
	    //TODO: method documentation did not show
	    /// <summary> Creates a new Show&#47;Hide Field action. A show&#47;hide field action 
	    /// shows or hide certain fields when it's invoked.
	    /// </summary>
	    /// <param name="doc">the document in which to create the action
	    /// </param>
	    /// <param name="field_list">list of fields to hide
	    /// </param>
        /// <returns>the action
        /// </returns>
        public static Action CreateHideField(SDFDoc doc, List<string> field_list)
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreateHideField(doc.mp_doc, field_list.Count, field_list.ToArray(), ref result));
            return result != IntPtr.Zero ? new Action(result, doc) : null;
        }
	    /// <summary> Creates a new 'Import Data' action. An import data action imports
	    /// form data from a FDF file into a PDF document.
	    /// </summary>
	    /// <param name="doc">the document in which to create the action
	    /// </param>
	    /// <param name="path">the full path of the FDF file
	    /// </param>
        /// <returns> the action
        /// </returns>
        public static Action CreateImportData(SDFDoc doc, string path)
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreateImportData(doc.mp_doc, path, ref result));
            return result != IntPtr.Zero ? new Action(result, doc) : null;
        }
	    /// <summary> Creates a new 'Reset Form' action. A reset form action reset choosen
	    /// form fields to their default value.
	    /// </summary>
	    /// <param name="doc">the document in which to create the action
	    /// </param>
        /// <returns> the action
        /// </returns>
        public static Action CreateResetForm(SDFDoc doc)
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreateResetForm(doc.mp_doc, ref result));
            return result != IntPtr.Zero ? new Action(result, doc) : null;
        }
	    /// <summary> Creates a new 'JavaScript' action. A javascript action executes a JavaScript
	    /// script when it's invoked. 
	    /// </summary>
	    /// <param name="doc">the document in which to create the action
	    /// </param>
	    /// <param name="script">the JavaScript script to be executed
	    /// </param>
        /// <returns> the action
        /// </returns>
        public static Action CreateJavaScript(SDFDoc doc, string script)
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCreateJavaScript(doc.mp_doc, script, ref result));
            return result != IntPtr.Zero ? new Action(result, doc) : null;
        }

	    /// <summary>Sets value to given <c>Action</c> object
	    /// </summary>
	    /// <param name="p">another <c>Action</c> object
	    /// </param>
        public void Set(Action p)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCopy(p.mp_action, ref mp_action));
        }
	    /// <summary>Assignment operator</summary>
	    /// <param name="r">another <c>Action</c> object
        /// </param>
        /// <returns>a <c>Action</c> object equals to the give object</returns>
        public Action op_Assign(Action r)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCopy(r.mp_action, ref this.mp_action));
            return this;
        }
	    /// <summary>Equality operator checks whether two Action objects are the same</summary>
	    /// <param name="l"><c>Action</c> object on the left of the operator</param>
        /// <param name="r"><c>Action</c> object on the right of the operator</param>
        /// <returns>true if two objects are equal, false otherwise</returns>
        public static bool operator ==(Action l, Action r)
        {
            if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null)) return true;
	        if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null)) return false;
	        if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null)) return false;

            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCompare(l.mp_action, r.mp_action, ref result));
            return result;
        }
	    /// <summary>Ineuqality operator checks whether two Action objects are different</summary>
	    /// <param name="l"><c>Action</c> object on the left of the operator</param>
        /// <param name="r"><c>Action</c> object on the right of the operator</param>
        /// <returns>true if two objects are not equal, false otherwise</returns>
        public static bool operator !=(Action l, Action r)
        {
            return !(l == r);
        }
	    /// <param name="o">a given <c>Object</c>
	    /// </param>
	    /// <returns>true, if equals to the given object
	    /// </returns>
        public override bool Equals(object o)
        {
            Action i = o as Action;
            if (i.mp_action.Equals(IntPtr.Zero)) return false;
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionCompare(mp_action, i.mp_action, ref result));
            return result;
        }
	    /// <summary> Indicates whether the Action is valid (non-null).
	    /// 
	    /// </summary>
	    /// <returns> True if this is a valid (non-null) Action; otherwise false.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  If this method returns false the underlying SDF/Cos object is null and 
        /// the Action object should be treated as null as well.</remarks>
        public bool IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionIsValid(mp_action, ref result));
            return result;
        }
        /// <summary> Gets the type.
	    /// 
	    /// </summary>
	    /// <returns> The type of this Action.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public new Type GetType()
        {
            Type result = Type.e_Unknown;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionGetType(mp_action, ref result));
            return result;
        }

	    /// <summary> Executes current action; this will only work for some action types that can be executed
	    /// only using the information contained in the action object or the associated PDF doc.
	    /// See also PDFViewCtrl.ExecuteAction()
	    /// 
	    /// </summary>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Execute()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_Action_Execute(mp_action));
        }
        /// <summary> Gets the form action flag.
	    /// 
	    /// </summary>
	    /// <param name="flag">the flag
	    /// </param>
	    /// <returns> value of the given action flag.
	    /// 
	    /// Action flags are currently only used by submit and reset form actions.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool GetFormActionFlag(FormActionFlag flag)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_Action_GetFormActionFlag(mp_action, flag, ref result));
            return result;
        }

	    /// <summary> Set the value of a given field flag.
	    /// 
	    /// Action flags are currently only used by submit and reset form actions.
	    /// 
	    /// </summary>
	    /// <param name="flag">the flag
	    /// </param>
	    /// <param name="value">the value
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetFormActionFlag(FormActionFlag flag, bool value)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_Action_SetFormActionFlag(mp_action, flag, value));
        }

	    /// <summary> Gets the dest.
	    /// 
	    /// </summary>
	    /// <returns> The Action's Destination view.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  This only works for Actions whose subtype is "GoTo".
        /// All named destinations are automatically resolved to the
        /// explicit destination so you can access destination members directly.</remarks>
        public Destination GetDest()
        {
            TRN_Destination result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionGetDest(mp_action, ref result));
            return new Destination(result, this.m_ref);
        }

	    /// <summary> Gets the next actions </summary>
	    /// <returns> The next action dictionary, an array of action dictionaries, 
	    /// or NULL if there are no additional actions.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks> Sequences of actions can be chained together.
	    /// For example, the effect of clicking a link annotation with the mouse might be to
	    /// play a sound, jump to a new page, and start up a movie. Note that the Next entry 
	    /// is not restricted to a single action but may contain an array of actions, each of
	    /// which in turn may have a Next entry of its own. The actions may thus form a tree 
	    /// instead of a simple linked list. Actions within each Next array are executed in 
        /// order, each followed in turn by any actions specified in its Next entry, and so 
        /// on recursively.</remarks>
        public Obj GetNext()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionGetNext(mp_action, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

	    /// <summary> Gets the SDFObj.
	    /// 
	    /// </summary>
        /// <returns> Pointer to the underlying SDF/Cos object.
        /// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ActionGetSDFObj(mp_action, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

        // Nested Types
        ///<summary>Flags used by submit form actions. Exclude flag is also used by reset form action.
	    /// No other action types use flags in the current version of PDF standard (ISO 2300).</summary>
	    public enum FormActionFlag
        {
            //TODO: values missing description
		    e_exclude = 0,
		    e_include_no_value_fields  = 1,
		    e_export_format = 2,			// submit in HTML format if set, FDF format if clear
		    e_get_method = 3,
		    e_submit_coordinates = 4,
		    e_xfdf = 5,
		    e_include_append_saves = 6,
		    e_include_annotations = 7,
		    e_submit_pdf = 8,
		    e_canonical_format = 9,
		    e_excl_non_user_annots = 10,
		    e_excl_F_key = 11,
		    // bit 12 is unused by PDF standard
		    e_embed_form = 13
        }

        ///<summary>Action types</summary>
	    public enum Type
        {
            ///<summary>Go to a destination in the current document.</summary>
		    e_GoTo,
		    ///<summary>('Go-to remote') Go to a destination in another document.</summary>
		    e_GoToR,
		    ///<summary>('Go-to embedded'; PDF 1.6) Go to a destination in an embedded file.</summary>
		    e_GoToE,
		    ///<summary>Launch an application, usually to open a file. </summary>
		    e_Launch,
		    ///<summary>Begin reading an article thread.</summary>
		    e_Thread,
		    ///<summary>Resolve a uniform resource identifier.</summary>
		    e_URI,
		    ///<summary>Play a sound.</summary>
		    e_Sound,
		    ///<summary>Play a movie.</summary>
		    e_Movie,
		    ///<summary>Set an annotation's Hidden flag.</summary>
		    e_Hide,
		    ///<summary>Execute an action predefined by the viewer application.</summary>
		    e_Named,
		    ///<summary>Send data to a uniform resource locator.</summary>
		    e_SubmitForm,
		    ///<summary>Set fields to their default values.</summary>
		    e_ResetForm,
		    ///<summary>Import field values from a file.</summary>
		    e_ImportData,
		    ///<summary>Execute a JavaScript script.</summary>
		    e_JavaScript,
		    ///<summary>(PDF 1.5) Set the states of optional content groups.</summary>
		    e_SetOCGState,
		    ///<summary>(PDF 1.5) Controls the playing of multimedia content.</summary>
		    e_Rendition,
		    ///<summary>Updates the display of a document, using a transition dictionary.</summary>
		    e_Trans,
		    ///<summary>(PDF 1.6) Set the current view of a 3D annotation</summary>
		    e_GoTo3DView,
		    ///<summary>Adobe supplement to ISO 32000; specifies a command to be sent to rich media annotation's handler</summary>
		    e_RichMediaExecute,
		    ///<summary>Unknown Action type</summary>
		    e_Unknown
        }    
    }
}
