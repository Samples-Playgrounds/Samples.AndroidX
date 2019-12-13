using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Obj = System.IntPtr;
using TRN_OCGConfig = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.OCG
{
    /// <summary> The OCG.Config object represents an optional-content (OC) configuration 
    /// structure (see section 4.10.3 in PDF Reference), used to persist a set 
    /// of visibility states and other optional-content information in a PDF file 
    /// A document has a default configuration, saved in the D entry in the 
    /// 'OCProperties' dictionary (that can be obtained using pdfdoc.GetOCConfig()), 
    /// and can have a list of other configurations, saved as an array in the 
    /// 'Configs' entry in the OCProperties dictionary.
    /// <p>
    /// Configurations are typically used to initialize the OCG ON-OFF states for 
    /// an optional-content context (OCG.Context). The OCG order in the configuration 
    /// is the order in which the groups appear in the Layers panel of a PDF viewer. 
    /// The configuration can also define a set of mutually exclusive OCGs, called a 
    /// radio button group.
    /// </p>
    /// </summary>
    public class Config
    {
        internal TRN_OCGConfig mp_obj = IntPtr.Zero;
        internal Object m_ref;
        internal Config(TRN_OCGConfig impl, Object reference) 
		{
            this.mp_obj = impl;
            this.m_ref = reference;
		}

        /// <summary>Creates a new optional-content configuration from an existing SDF&#47;Cos object.
		/// </summary>
		/// <param name="doc">document to create OCG configuration in
		/// </param>
		/// <param name="default_config">whether to use default configuration
		/// </param>
        /// <returns>newly created configuration
        /// </returns>
        public static Config Create(PDFDoc doc, bool default_config)
        {
            TRN_OCGConfig g = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigCreate(doc.mp_doc, default_config, ref g));
            return new Config(g, doc);
        }
		/// <summary>Creates a new optional-content configuration from an existing SDF Cos object.
		/// </summary>
        /// <param name="ocg_config">existing OCG configuration
        /// </param>
        public Config(Obj ocg_config)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigCreateFromObj(ocg_config.mp_obj, ref mp_obj));
            this.m_ref = ocg_config.GetRefHandleInternal();
        }

		//bool IsValid();
		/// <summary> Gets the order.
		/// 
		/// </summary>
		/// <returns> the Obj array that specifies the order of optional content (OC) groups
		/// in this configuration or NULL if the configuration does not contain any OCGs.
		/// The order of OCGs in the array is used to represent the order in which the
		/// group names are displayed in the Layers panel of a PDF viewer application.
		/// For more information, please refer to Section 4.10.3 in the PDF Reference.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetOrder()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigGetOrder(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Sets the user interface display order of optional-content groups (OCGs) in
		/// this configuration. This is the order in which the group names are displayed
		/// in the Layers panel of a PDF viewer.
		/// 
		/// </summary>
		/// <param name="ocgs_array">The SDF/Cos object containing the OCG order array.
		/// For more information, please refer to section 4.10.3 in the PDF Reference.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetOrder(Obj ocgs_array)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigSetOrder(mp_obj, ocgs_array.mp_obj));
        }
		/// <summary> Gets the name.
		/// 
		/// </summary>
		/// <returns> the name of an optional-content configuration (suitable for
		/// presentation in a user interface).
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetName()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigGetName(mp_obj, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
		/// <summary> Sets the name of an optional-content configuration (suitable for presentation
		/// in a user interface). The method stores the specified string as the Name entry
		/// in the configuration's SDF/Cos dictionary.
		/// 
		/// </summary>
		/// <param name="name">The new name string.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetName(string name)
        {
            UString str = new UString(name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigSetName(mp_obj, str.mp_impl));
        }
		/// <summary> Gets the creator.
		/// 
		/// </summary>
		/// <returns> the creator property of an optional-content configuration. The string
		/// is used to represent the name of the application or feature that created this
		/// configuration.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetCreator()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigGetCreator(mp_obj, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
		/// <summary> Sets the creator property of an optional-content configuration. Stores the
		/// specified string as the Creator entry in the configuration's SDF/Cos dictionary.
		/// 
		/// </summary>
		/// <param name="name">The new creator string.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetCreator(string name)
        {
            UString str = new UString(name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigSetCreator(mp_obj, str.mp_impl));
        }
        /// <summary> Gets the inits the base state.
        /// 
        /// </summary>
        /// <returns> the base initialization state. This state is used to initialize the states
        /// of all the OCGs in a document when this configuration is applied. The value of this entry
        /// must be one of the following names:
        /// <list type="bullet">
        /// <item><term>ON</term><description> The states of all groups are turned ON.</description></item>
        /// <item><term>OFF</term><description> The states of all groups are turned OFF.</description></item>
        /// <item><term>Unchanged</term><description> The states of all groups are left unchanged.</description></item>
        /// </list>
        /// <p> After base initialization, the contents of the ON and OFF arrays are processed, overriding
        /// the state of the groups included in the arrays.</p>
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  If BaseState is present in the document's default configuration dictionary, its value must 
        /// be "ON".</remarks>
        /// <default>  ON. </default>
        public string GetInitBaseState()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigGetInitBaseState(mp_obj, ref result));
            return Marshal.PtrToStringUTF8(result);
        }
		/// <summary> Gets the inits the on states.
		/// 
		/// </summary>
		/// <returns> the "ON" initialization array from the configuration dictionary or
		/// NULL if the array is not present. The returned object is an array of optional
		/// content groups whose state should be set to ON when this configuration is applied.
		/// Note: If the BaseState entry is ON, this entry is redundant.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetInitOnStates()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigGetInitOnStates(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Gets the inits the off states.
		/// 
		/// </summary>
		/// <returns> the "OFF" initialization array from the configuration dictionary or
		/// NULL if the array is not present. The returned object is an array of optional
		/// content groups whose state should be set to OFF when this configuration is applied.
		/// Note: If the BaseState entry is OFF, this entry is redundant.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetInitOffStates()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigGetInitOffStates(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Sets the base initialization state. For more info, please see GetInitBaseState().
		/// 
		/// </summary>
		/// <param name="state">new base state ("ON", "OFF", or "Unchanged").
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetInitBaseState(string state)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigSetInitBaseState(mp_obj, state));
        }
		/// <summary> Sets the 'ON' initialization array in the configuration dictionary.
		/// For more info, please see SetInitOnStates() and  section 4.10.3 in PDF Reference.
		/// 
		/// </summary>
		/// <param name="on_array">the new inits the on states
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetInitOnStates(Obj on_array)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigSetInitOnStates(mp_obj, on_array.mp_obj));
        }
		/// <summary> Sets the 'OFF' initialization array in the configuration dictionary.
		/// For more info, please see SetInitOffStates() and section 4.10.3 in PDF Reference.
		/// 
		/// </summary>
		/// <param name="off_array">the new inits the off states
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetInitOffStates(Obj off_array)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigSetInitOffStates(mp_obj, off_array.mp_obj));
        }
		/// <summary> Gets the intent.
		/// 
		/// </summary>
		/// <returns> OCG configuration intent. An intent is a name object (or an array of name
		/// objects) broadly describing the intended use, which can be either "View" or "Design".
		/// A group's content is considered to be optional (that is, the group's state is considered
		/// in its visibility) if any intent in its list matches an intent of the context. The
		/// intent list of the context is usually set from the intent list of the document
		/// configuration. If the configuration has no Intent entry, the default value of
		/// "View" is used.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetIntent()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigGetIntent(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Sets the Intent entry in an optional-content configuration's SDF/Cos dictionary.
		/// For more information, see GetIntent().
		/// 
		/// </summary>
		/// <param name="intent">The new Intent entry value (a name object or an array of name objects).
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetIntent(Obj intent)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigSetIntent(mp_obj, intent.mp_obj));
        }
		/// <summary> Gets the locked oc gs.
		/// 
		/// </summary>
		/// <returns> the list of locked OCGs or NULL if there are no locked OCGs.
		/// The on/off state of a locked OCG cannot be toggled by the user through the
		/// user interface.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetLockedOCGs()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigGetLockedOCGs(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Sets the array of locked OCGs. The on/off state of a locked OCG cannot be
		/// toggled by the user through the user interface.
		/// 
		/// </summary>
		/// <param name="locked_ocg_array">An SDF/Cos array of OCG objects to be locked in this
		/// configuration, or an empty array if the configuration should not contain
		/// locked OCGs. The default is the empty array.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetLockedOCGs(Obj locked_ocg_array)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigSetLockedOCGs(mp_obj, locked_ocg_array.mp_obj));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCGConfigGetSDFObj(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
    }
}
