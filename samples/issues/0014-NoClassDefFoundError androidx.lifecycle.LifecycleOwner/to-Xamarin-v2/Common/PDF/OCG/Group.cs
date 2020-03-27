using System;

using pdftron.Common;
using pdftron.SDF;
using pdftronprivate.trn;

using TRN_Obj = System.IntPtr;
using TRN_OCG = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.OCG
{
    /// <summary> The OCG.Group object represents an optional-content group. This corresponds
    /// to a PDF OCG dictionary representing a collection of graphic objects that can 
    /// be made visible or invisible (Section 4.10.1 'Optional Content Groups' in PDF 
    /// Reference). Any graphic content of the PDF can be made optional, including page 
    /// contents, XObjects, and annotations. The specific content objects in the group 
    /// have an OC entry in the PDF as part of the surrounding marked content or in the 
    /// XObject dictionary. The group itself is a named object that can be typically 
    /// manipulated through a Layers panel in a PDF viewer.
    /// <p>
    /// In the simplest case, the group's ON-OFF state makes the associated content 
    /// visible or hidden. The ON-OFF state of a group can be toggled for a particular 
    /// context (OCG.Context), and a set of states is kept in a configuration (OCG.Config). 
    /// The visibility can depend on more than one group in an optional-content membership 
    /// dictionary (OCG.OCMD), and can also be affected by the context's draw mode
    /// (OCGContext.OCDrawMode).
    /// </p><p>
    /// A group has an Intent entry, broadly describing the intended use. A group's 
    /// content is considered to be optional (that is, the group's state is considered in 
    /// its visibility) if any intent in its list matches an intent of the context. The 
    /// intent list of the context is usually set from the intent list of the document 
    /// configuration.
    /// </p><p>
    /// A Usage dictionary entry provides more specific intended usage information than 
    /// an intent entry. Possible key values are: CreatorInfo, Language, Export, Zoom,
    /// Print, View, User, PageElement. The usage value can act as a kind of metadata, 
    /// describing the sort of things that belong to the group, such as text in French, 
    /// fine detail on a map, or a watermark. The usage values can also be used by the 
    /// AutoState mechanism to make decisions about what groups should be on and what 
    /// groups should be off. The AutoState mechanism considers the usage information 
    /// in the OCGs, the AS array of the configuration, and external factors; for example, 
    /// the language the application is running in, the current zoom level on the page, 
    /// or whether the page is being printed.</p>
    /// </summary>
    public class Group
    {
        internal TRN_OCG mp_obj = IntPtr.Zero;
        internal Object m_ref;

        internal Group(TRN_OCG impl, Object reference) 
		{
            this.mp_obj = impl;
            this.m_ref = reference;
		}

        /// <summary> Creates a new optional-content group (OCG) object in the document.
		/// 
		/// </summary>
		/// <param name="doc">The document in which the new OCG will be created.
		/// </param>
		/// <param name="name">The name of the optional-content group.
		/// </param>
		/// <returns> The newly created OCG.Group object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Group Create(PDFDoc doc, String name)
        {
            TRN_OCG g = IntPtr.Zero;
            UString str = new UString(name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGCreate(doc.mp_doc, str.mp_impl, ref g));
            return new Group(g, doc);
        }

		/// <summary> Creates a new optional-content group (OCG) object from an existing
		/// SDF/Cos object.
		/// 
		/// </summary>
        /// <param name="ocg">the ocg
        /// </param>
        public Group(Obj ocg)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGCreateFromObj(ocg.mp_obj, ref mp_obj));
            this.m_ref = ocg.GetRefHandleInternal();
        }

		/// <summary> Checks if is valid.
		/// 
		/// </summary>
		/// <returns> True if this is a valid (non-null) OCG, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGIsValid(mp_obj, ref result));
            return result;
        }
		/// <summary> Gets the name.
		/// 
		/// </summary>
		/// <returns> the name of this optional-content group (OCG).
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetName()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGGetName(mp_obj, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
		/// <summary> Sets the name of this optional-content group (OCG).
		/// 
		/// </summary>
		/// <param name="name">The new name.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetName(String name)
        {
            UString str = new UString(name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGSetName(mp_obj, str.mp_impl));
        }
		/// <summary> Gets the current state.
		/// 
		/// </summary>
		/// <param name="context">The context for which to get the group's state.
		/// </param>
		/// <returns> true if this OCG object is in the ON state in a given context,
		/// false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool GetCurrentState(Context context)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGGetCurrentState(mp_obj, context.mp_impl, ref result));
            return result;
        }
		/// <summary> Sets the current ON-OFF state of the optional-content group (OCG) object in a given context.
		/// 
		/// </summary>
		/// <param name="context">The context for which to set the group's state.
		/// </param>
		/// <param name="state">The new state.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetCurrentState(Context context, bool state)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGSetCurrentState(mp_obj, context.mp_impl, state));
        }
		/// <summary> Gets the initial state.
		/// 
		/// </summary>
		/// <param name="config">The configuration for which to get the group's initial state.
		/// </param>
		/// <returns> The initial state (ON or OFF) of the optional-content group
		/// (OCG) object in a given configuration.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If the configuration has a BaseState of Unchanged, and the OCG is 
		/// not listed explicitly in its ON list or OFF list, then the initial state
		/// is taken from the OCG's current state in the document's default context.</remarks>
        public bool GetInitialState(Config config)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGGetInitialState(mp_obj, config.mp_obj, ref result));
            return result;
        }
		/// <summary> Sets the initial state (ON or OFF) of the optional-content group (OCG)
		/// object in a given configuration.
		/// 
		/// </summary>
		/// <param name="config">The configuration for which to set the group's initial state.
		/// </param>
		/// <param name="state">The new initial state, true if the state is ON, false if it is OFF.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetInitialState(Config config, bool state)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGSetInitialState(mp_obj, config.mp_obj, state));
        }
		/// <summary> Gets the intent.
		/// 
		/// </summary>
		/// <returns> OCG intent.
		/// An intent is a name object (or an array of name objects) broadly describing the
		/// intended use, which can be either "View" or "Design". A group's content is
		/// considered to be optional (that is, the group's state is considered in its
		/// visibility) if any intent in its list matches an intent of the context. The
		/// intent list of the context is usually set from the intent list of the document
		/// configuration.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetIntent()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGGetIntent(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Sets the Intent entry in an optional-content group's SDF/Cos dictionary.
		/// For more information, see GetIntent().
		/// 
		/// </summary>
		/// <param name="intent">The new Intent entry value (a name object or an array of name objects).
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetIntent(Obj intent)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGSetIntent(mp_obj, intent.mp_obj));
        }
		/// <summary> Checks if is locked.
		/// 
		/// </summary>
		/// <param name="config">The OC configuration.
		/// </param>
		/// <returns> true if this OCG is locked in a given configuration, false otherwise.
		/// The on/off state of a locked OCG cannot be toggled by the user through the user interface.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsLocked(Config config)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGIsLocked(mp_obj, config.mp_obj, ref result));
            return result;
        }
		/// <summary> Sets the locked state of an OCG in a given configuration. The on/off state of a
		/// locked OCG cannot be toggled by the user through the user interface.
		/// 
		/// </summary>
		/// <param name="config">IN/OUT The optional-content configuration.
		/// </param>
		/// <param name="locked">true if the OCG should be locked, false otherwise.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetLocked(Config config, bool locked)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGSetLocked(mp_obj, config.mp_obj, locked));
        }
		/// <summary> Checks for usage.
		/// 
		/// </summary>
		/// <returns> true if this group is associated with a Usage dictionary, false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool HasUsage()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGHasUsage(mp_obj, ref result));
            return result;
        }
		/// <summary> Gets the usage.
		/// 
		/// </summary>
		/// <param name="key">The usage key in the usage dictionary entry. The possible key values are:
		/// CreatorInfo, Language, Export, Zoom, Print, View, User, PageElement.
		/// </param>
		/// <returns> The usage information associated with the given key in the Usage dictionary
		/// for the group, or a NULL if the entry is not present. A Usage dictionary entry
		/// provides more specific intended usage information than an intent entry.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetUsage(string key)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGGetUsage(mp_obj, key, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }

		/// <summary> Gets the SDFObj.
		/// 
		/// </summary>
		/// <returns> Pointer to the underlying SDF/Cos object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGGetSDFObj(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
    }
}