using System;

using pdftron.Common;
using pdftron.SDF;
using pdftronprivate.trn;

using TRN_OCMD = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_OCG = System.IntPtr;

namespace pdftron.PDF.OCG
{
    /// <summary><p> The OCMD object represents an Optional Content Membership Dictionary (OCMD) 
	/// that allows the visibility of optional content to depend on the states in a 
	/// set of optional-content groups (OCG::Group). The object directly corresponds 
	/// to the OCMD dictionary (Section 4.10.1 'Optional Content Groups' in PDF 
	/// Reference).</p>
	/// <p>
	/// An OCMD collects a set of OCGs. It sets a visibility policy, so that 
	/// content in the member groups is visible only when all groups are ON 
	/// or OFF, or when any of the groups is ON or OFF. This makes it possible 
	/// to set up complex dependencies among groups. For example, an object can be 
	/// visible only if some other conditions are met (such as if another layer is 
	/// visible).
	/// </p>
	/// </summary>
    public class OCMD
    {
        internal TRN_OCMD mp_obj = IntPtr.Zero;
        internal Object m_ref;
        internal OCMD(TRN_OCMD imp, Object reference)
        {
            this.mp_obj = imp;
            this.m_ref = reference;
        }
        /// <summary> Creates a new optional-content membership dictionary (OCMD) object in the
		/// given document for the given groups and visibility policy.
		/// 
		/// </summary>
		/// <param name="doc">The document in which the new OCMD will be created.
		/// </param>
		/// <param name="ocgs">An array of optional-content groups (OCGs) to be members of the dictionary.
		/// </param>
		/// <param name="vis_policy">the vis_policy
		/// </param>
		/// <returns> The newly created OCG::OCMD object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static OCMD Create(PDFDoc doc, Obj ocgs, VisibilityPolicyType vis_policy)
        {
            TRN_OCMD result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCMDCreate(doc.mp_doc, ocgs.mp_obj, vis_policy, ref result));
            return new OCMD(result, doc);
        }

		/// <summary> Creates a new optional-content group (OCG) object from an existing
		/// SDF/Cos object.
		/// 
		/// </summary>
		/// <param name="ocmd">the ocmd
		/// </param>
        public OCMD(Obj ocmd)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCMDCreateFromObj(ocmd.mp_obj, ref mp_obj));
            this.m_ref = ocmd.GetRefHandleInternal();
        }
		/// <summary> Checks if is valid.
		/// 
		/// </summary>
		/// <returns> True if this is a valid (non-null) OCG, false otherwise.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCMDIsValid(mp_obj, ref result));
            return result;
        }
		/// <summary> Gets the optional-content groups listed under 'OCGs' entry in the object
		/// dictionary.
		/// 
		/// </summary>
		/// <returns> An SDF::Obj array or OCG::Group objects or NULL is the array
		/// is not present.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetOCGs()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCMDGetOCGs(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Gets the optional-content membership dictionary's visibility policy, which
		/// determines the visibility of content with respect to the ON-OFF state of
		/// OCGs listed in the dictionary.
		/// 
		/// </summary>
		/// <returns> The visibility policy.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public VisibilityPolicyType GetVisibilityPolicy()
        {
            VisibilityPolicyType result = VisibilityPolicyType.e_AllOff;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCMDGetVisibilityPolicy(mp_obj, ref result));
            return result;
        }
		/// <summary> Sets the optional-content membership dictionary's visibility policy, which
		/// determines the visibility of content with respect to the ON-OFF state of
		/// OCGs listed in the dictionary.
		/// 
		/// </summary>
		/// <param name="vis_policy">New visibility policy.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetVisibilityPolicy(VisibilityPolicyType vis_policy)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCMDSetVisibilityPolicy(mp_obj, vis_policy));
        }
		/// <summary> Gets the visibility expression.
		/// 
		/// </summary>
		/// <returns> If the PDOCMD has a visibility expression entry, return the
		/// SDF::Obj array object representing the expression, otherwise returns NULL.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetVisibilityExpression()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCMDGetVisibilityExpression(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
		/// <summary> Checks if is currently visible.
		/// 
		/// </summary>
		/// <param name="context">The context in which the visibility of content is tested.
		/// </param>
		/// <returns> true if content tagged with this OCMD is visible in the given
		/// context, false if it is hidden.
		/// 
		/// Based on the optional-content groups listed in the dictionary, the current
		/// ON-OFF state of those groups within the specified context, and the
		/// dictionary's visibility policy, test whether the content tagged with
		/// this dictionary would be visible.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsCurrentlyVisible(Context context)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCMDIsCurrentlyVisible(mp_obj, context.mp_impl, ref result));
            return result;
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_OCMDGetSDFObj(mp_obj, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
        // Nested Types
        /// <summary> Visibility Policy Type.
        /// A Visibility Policy is used to determine whether an PDF::Element is visible 
        /// in a given OCG::Context, depending on whether all groups in OCGs array are 
        /// "ON" or "OFF", or when any of the groups is "ON" or "OFF". 
        /// </summary>
        public enum VisibilityPolicyType
        {
            /// <summary>visible only if all of the entries in OCGs are ON.</summary>
            e_AllOn,
            /// <summary>visible if any of the entries in OCGs are ON.</summary>
            e_AnyOn,
            /// <summary>visible if any of the entries in OCGs are OFF.</summary>
            e_AnyOff,
            /// <summary>visible only if all of the entries in OCGs are OFF.</summary>
            e_AllOff
        }
    }
}