using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Obj = System.IntPtr;
using TRN_Vector = System.IntPtr;
using TRN_UString = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary>
    /// A class representing a digital signature form field.
    /// </summary>
    public class DigitalSignatureField : IDisposable
    {
        internal BasicTypes.TRN_DigitalSignatureField m_impl;
        internal Object m_ref;

        internal DigitalSignatureField(BasicTypes.TRN_DigitalSignatureField impl_ptr, Object reference)
        {
            this.m_impl = impl_ptr;
            this.m_ref = reference;
        }

        ~DigitalSignatureField()
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
            if (m_impl.mp_field_dict_obj != IntPtr.Zero)
            {
                m_impl.mp_field_dict_obj = IntPtr.Zero;
            }
        }

        /// <summary> Constructs a DigitalSignatureField from a Field.
		/// </summary>
		/// <param name="field">
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public DigitalSignatureField(Field field)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldCreateFromField(ref field.mp_field, ref m_impl));
            this.m_ref = field.m_ref;
        }

        /// <summary>
        /// Returns whether the digital signature field has been cryptographically signed. Checks whether there is a digital signature dictionary in the field and whether it has a Contents entry. Must be called before using various digital signature dictionary-related functions. Does not check validity - will return true even if a valid hash has not yet been generated (which will be the case after [Certify/Sign]OnNextSave[WithCustomHandler] has been called on the signature but even before Save is called on the document).
        /// </summary>
        /// <returns>A boolean value representing whether the digital signature field has a digital signature dictionary with a Contents entry</returns>
        public bool HasCryptographicSignature()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldHasCryptographicSignature(ref m_impl, ref result));
            return result;
        }

        /// <summary>
        /// Returns the SubFilter type of the digital signature. Specification says that one must check the SubFilter before using various getters. Must call HasCryptographicSignature first and use it to check whether the signature is signed.
        /// </summary>
        /// <returns>An enumeration describing what the SubFilter of the digital signature is from within the digital signature dictionary</returns>
        public SubFilterType GetSubFilter()
        {
            SubFilterType result = SubFilterType.e_absent;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldGetSubFilter(ref m_impl, ref result));
            return result;
        }

        /// <summary>
        /// Should not be called when SubFilter is ETSI.RFC3161 (i.e. on a DocTimeStamp). Returns the name of the signer of the signature from the digital signature dictionary. Must call HasCryptographicSignature first and use it to check whether the signature is signed.
        /// </summary>
        /// <returns>A unicode string containing the name of the signer from within the digital signature dictionary. Empty if Name entry not present</returns>
        public string GetSignatureName()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldGetSignatureName(ref m_impl, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

        /// <summary>Should not be called when SubFilter is ETSI.RFC3161 (i.e. on a DocTimeStamp). 
		/// Returns the "M" entry from the digital signature dictionary, which represents the 
		/// signing date/time. Must call HasCryptographicSignature first and use it to check whether the 
		/// signature is signed. </summary>
		/// 
		/// <returns>A PDF::Date object holding the signing date/time from within the digital signature dictionary. Returns a default-constructed PDF::Date if no date is present.</returns>
		public Date GetSigningTime()
        {
            BasicTypes.TRN_Date result = new BasicTypes.TRN_Date();
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldGetSigningTime(ref m_impl, ref result));
            return new Date(result);
        }

        /// <summary>
        /// Should not be called when SubFilter is ETSI.RFC3161 (i.e. on a DocTimeStamp). Returns the Location of the signature from the digital signature dictionary. Must call HasCryptographicSignature first and use it to check whether the signature is signed.
        /// </summary>
        /// <returns>A unicode string containing the signing location from within the digital signature dictionary. Empty if Location entry not present</returns>
        public string GetLocation()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldGetLocation(ref m_impl, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

        /// <summary>
        /// Should not be called when SubFilter is ETSI.RFC3161 (i.e. on a DocTimeStamp). Returns the Reason for the signature from the digital signature dictionary. Must call HasCryptographicSignature first and use it to check whether the signature is signed.
        /// </summary>
        /// <returns>A unicode string containing the reason for the signature from within the digital signature dictionary. Empty if Reason entry not present</returns>
        public string GetReason()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldGetReason(ref m_impl, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

        /// <summary>
        /// Should not be called when SubFilter is ETSI.RFC3161 (i.e. on a DocTimeStamp). Returns the contact information of the signer from the digital signature dictionary. Must call HasCryptographicSignature first and use it to check whether the signature is signed.
        /// </summary>
        /// <returns>A unicode string containing the contact information of the signer from within the digital signature dictionary. Empty if ContactInfo entry not present</returns>
        public string GetContactInfo()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldGetContactInfo(ref m_impl, ref result.mp_impl));
            return result.ConvToManagedStr();
        }

        /// <summary>
        /// Gets a certificate in the certificate chain (Cert entry) of the digital signature dictionary by index. Throws if Cert is not Array or String, throws if index is out of range and Cert is Array, throws if index is less than 1 and Cert is string, otherwise retrieves the certificate.
        /// </summary>
        /// <param name="in_index">  -- An integral index which must be greater than 0 and less than the cert count as retrieved using GetCertCount.</param>
        /// <returns>A vector of bytes containing the certificate at the index. Returns empty vector if Cert is missing.</returns>
        public Byte[] GetCert(Int32 in_index)
        {
            TRN_Vector cvector = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldGetCert(ref m_impl, new IntPtr(in_index), ref cvector));

            IntPtr arr = IntPtr.Zero;
            IntPtr size_ptr = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_VectorGetData(cvector, ref arr));
            PDFNetException.REX(PDFNetPINVOKE.TRN_VectorGetSize(cvector, ref size_ptr));
            int size = size_ptr.ToInt32();

            byte[] result = new byte[size];
            Marshal.Copy(arr, result, 0, size);
            return result;
        }

        /// <summary>
        /// Gets number of certificates in certificate chain (Cert entry of digital signature dictionary). Must call HasCryptographicSignature first and use it to check whether the signature is signed.
        /// </summary>
        /// <returns>An integer value - the number of certificates in the Cert entry of the digital signature dictionary</returns>
        public Int32 GetCertCount()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldGetCertCount(ref m_impl, ref result));
            return result.ToInt32();
        }

        /// <summary>
        /// Returns whether the field has a visible appearance. Can be called without checking HasCryptographicSignature first, since it operates on the surrounding Field dictionary, not the "V" entry (i.e. digital signature dictionary). Performs the zero-width+height check, the Hidden bit check, and the NoView bit check as described by the PDF 2.0 specification, section 12.7.5.5 "Signature fields".
        /// </summary>
        /// <returns>A boolean representing whether or not the signature field has a visible signature</returns>
        public bool HasVisibleAppearance()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldHasVisibleAppearance(ref m_impl, ref result));
            return result;
        }

        /// <summary>
        /// Should not be called when SubFilter is ETSI.RFC3161 (i.e. on a DocTimeStamp). Sets the ContactInfo entry in the digital signature dictionary. Must create a digital signature dictionary first using [Certify/Sign]OnNextSave[WithCustomHandler]. If this function is called on a digital signature field that has already been cryptographically signed with a valid hash, the hash will no longer be valid, so do not call Save (to sign/create the hash) until after you call this function, if you need to call this function in the first place. Essentially, call this function after [Certify/Sign]OnNextSave[WithCustomHandler] and before Save.
        /// </summary>
        /// <param name="in_contact_info">A string containing the ContactInfo to be set.</param>
        public void SetContactInfo(string in_contact_info)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldSetContactInfo(ref m_impl, UString.ConvertToUString(in_contact_info).mp_impl));
        }

        /// <summary>
        /// Should not be called when SubFilter is ETSI.RFC3161 (i.e. on a DocTimeStamp). Sets the Location entry in the digital signature dictionary. Must create a digital signature dictionary first using [Certify/Sign]OnNextSave[WithCustomHandler]. If this function is called on a digital signature field that has already been cryptographically signed with a valid hash, the hash will no longer be valid, so do not call Save (to sign/create the hash) until after you call this function, if you need to call this function in the first place. Essentially, call this function after [Certify/Sign]OnNextSave[WithCustomHandler] and before Save.
        /// </summary>
        /// <param name="in_location">A string containing the Location to be set.</param>
        public void SetLocation(string in_location)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldSetLocation(ref m_impl, UString.ConvertToUString(in_location).mp_impl));
        }

        /// <summary>
        /// Should not be called when SubFilter is ETSI.RFC3161 (i.e. on a DocTimeStamp). Sets the Reason entry in the digital signature dictionary. Must create a digital signature dictionary first using [Certify/Sign]OnNextSave[WithCustomHandler]. If this function is called on a digital signature field that has already been cryptographically signed with a valid hash, the hash will no longer be valid, so do not call Save (to sign/create the hash) until after you call this function, if you need to call this function in the first place. Essentially, call this function after [Certify/Sign]OnNextSave[WithCustomHandler] and before Save.
        /// </summary>
        /// <param name="in_reason">A string containing the Reason to be set.</param>
        public void SetReason(string in_reason)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldSetReason(ref m_impl, UString.ConvertToUString(in_reason).mp_impl));
        }

        /// <summary>
        /// Sets the document locking permission level for this digital signature field. Call only on unsigned signatures, otherwise a valid hash will be invalidated.
        /// </summary>
        /// <param name="in_perms">An enumerated value representing the document locking permission level to set.</param>
        public void SetDocumentPermissions(DocumentPermissions in_perms)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldSetDocumentPermissions(ref m_impl, in_perms));
        }
        /// <summary>Tentatively sets which fields are to be locked by this digital signature upon signing. It is not necessary to call HasCryptographicSignature before using this function. Throws if non-empty array of field names is passed along with FieldPermissions Action == e_lock_all. </summary>
		/// <param name="in_action">An enumerated value representing which sort of field locking should be done. Options are All (lock all fields), Include (lock listed fields), and Exclude (lock all fields except listed fields).</param>
		/// <param name="in_field_names">A list of field names; can be empty (and must be empty, if Action is set to All). Empty by default.</param>
        public void SetFieldPermissions(FieldPermissions in_action, String[] in_field_names)
        {
            // conv to ustring
            IntPtr[] ustringNames = new IntPtr[in_field_names.Length];
            for (int i = 0; i < in_field_names.Length; i++)
            {
                ustringNames[i] = UString.ConvertToUString(in_field_names[i]).mp_impl;
            }
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldSetFieldPermissions(ref m_impl, in_action, ustringNames, new IntPtr(ustringNames.Length)));
        }

        /// <summary>Tentatively sets which fields are to be locked by this digital signature upon signing. It is not necessary to call HasCryptographicSignature before using this function. Throws if non-empty array of field names is passed along with FieldPermissions Action == e_lock_all. </summary>
		/// <param name="in_action">An enumerated value representing which sort of field locking should be done. Options are All (lock all fields), Include (lock listed fields), and Exclude (lock all fields except listed fields).</param>
        public void SetFieldPermissions(FieldPermissions in_action)
        {
            SetFieldPermissions(in_action, new string[0]);
        }

        /// <summary>
        /// Must be called to prepare a signature for signing, which is done afterwards by calling Save. Cannot sign two signatures during one save (throws). Default document permission level is e_annotating_formfilling_signing_allowed. Throws if signature field already has a digital signature dictionary.
        /// </summary>
        /// <param name="in_pkcs12_keyfile_path">The path to the PKCS 12 keyfile to use to sign this digital signature.</param>
        /// <param name="in_password">The password to use to parse the PKCS 12 keyfile.</param>
        public void SignOnNextSave(string in_pkcs12_keyfile_path, string in_password)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldSignOnNextSave(ref m_impl, UString.ConvertToUString(in_pkcs12_keyfile_path).mp_impl, UString.ConvertToUString(in_password).mp_impl));
        }

        /// <summary>Must be called to prepare a signature for signing, which is done afterwards by calling Save. Cannot sign two signatures during one save (throws). Default document permission level is e_annotating_formfilling_signing_allowed. Throws if signature field already has a digital signature dictionary.</summary>
        /// <param name="in_pkcs12_buffer">A buffer of bytes containing the PKCS #12 private key certificate store to use to sign this digital signature.</param>
        /// <param name="in_password">The password to use to parse the PKCS #12 buffer.</param>
        public void SignOnNextSave(Byte[] in_pkcs12_buffer, String in_password)
        {
            GCHandle pinnedRawData = GCHandle.Alloc(in_pkcs12_buffer, GCHandleType.Pinned);
            IntPtr pnt = pinnedRawData.AddrOfPinnedObject();
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldSignOnNextSaveFromBuffer(ref m_impl, pnt, new IntPtr(in_pkcs12_buffer.Length), UString.ConvertToUString(in_password).mp_impl));
        }

        /// <summary>Must be called to prepare a signature for signing, which is done afterwards by calling Save. Cannot sign two signatures during one save (throws). Default document permission level is e_annotating_formfilling_signing_allowed. Throws if signature field already has a digital signature dictionary.</summary>
        /// <param name="in_signature_handler_id">The unique id of the signature handler to use to sign this digital signature.</param>
        public void SignOnNextSaveWithCustomHandler(SDF.SignatureHandlerId in_signature_handler_id)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldSignOnNextSaveWithCustomHandler(ref m_impl, in_signature_handler_id.mp_imp));
        }

        /// <summary>
        /// Must be called to prepare a signature for certification, which is done afterwards by calling Save. Throws if document already certified. Default document permission level is e_annotating_formfilling_signing_allowed. Throws if signature field already has a digital signature dictionary.
        /// </summary>
        /// <param name="in_pkcs12_keyfile_path">The path to the PKCS 12 keyfile to use to certify this digital signature.</param>
        /// <param name="in_password">The password to use to parse the PKCS 12 keyfile.</param>
        public void CertifyOnNextSave(string in_pkcs12_keyfile_path, string in_password)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldCertifyOnNextSave(ref m_impl, UString.ConvertToUString(in_pkcs12_keyfile_path).mp_impl, UString.ConvertToUString(in_password).mp_impl));
        }

        /// <summary>Must be called to prepare a signature for certification, which is done afterwards by calling Save. Throws if document already certified. Default document permission level is e_annotating_formfilling_signing_allowed. Throws if signature field already has a digital signature dictionary.</summary>
		/// <param name="in_pkcs12_buffer">A buffer of bytes containing the PKCS #12 private key certificate store to use to sign this digital signature.</param>
		/// <param name="in_password">The password to use to parse the PKCS #12 buffer.</param>
		public void CertifyOnNextSave(Byte[] in_pkcs12_buffer, String in_password)
        {
            GCHandle pinnedRawData = GCHandle.Alloc(in_pkcs12_buffer, GCHandleType.Pinned);
            IntPtr pnt = pinnedRawData.AddrOfPinnedObject();
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldCertifyOnNextSaveFromBuffer(ref m_impl, pnt, new IntPtr(in_pkcs12_buffer.Length), UString.ConvertToUString(in_password).mp_impl));
        }

        /// <summary>Must be called to prepare a signature for certification, which is done afterwards by calling Save. Throws if document already certified. Default document permission level is e_annotating_formfilling_signing_allowed. Throws if signature field already has a digital signature dictionary.</summary>
		/// <param name="in_signature_handler_id">The unique id of the signature handler to use to certify this digital signature.</param>
		public void CertifyOnNextSaveWithCustomHandler(SDF.SignatureHandlerId in_signature_handler_id)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldCertifyOnNextSaveWithCustomHandler(ref m_impl, in_signature_handler_id.mp_imp));
        }

        /// <summary>Gets the SDFObj.</summary>
        /// <returns>The underlying SDF/Cos object.</returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public SDF.Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldGetSDFObj(ref m_impl, ref result));
            return new SDF.Obj(result, m_ref);
        }

        /// <summary>
        /// Returns whether this digital signature field is locked against modifications by any digital signatures. Can be called when this field is unsigned.
        /// </summary>
        /// <returns>A boolean representing whether this digital signature field is locked against modifications by any digital signatures in the document</returns>
        public bool IsLockedByDigitalSignature()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldIsLockedByDigitalSignature(ref m_impl, ref result));
            return result;
        }

        /// <summary>
		/// Returns the fully-qualified names of all fields locked by this signature using the field permissions feature. Retrieves from the digital signature dictionary if the form field HasCryptographicSignature. Otherwise, retrieves from the Lock entry of the digital signature form field. Result is invalidated by any field additions or removals. Does not take document permissions restrictions into account.
		/// </summary>
		/// <returns> An array of Strings representing the fully-qualified names of all fields locked by this signature.</returns>
        public String[] GetLockedFields()
        {
            TRN_Vector fields_vec = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldGetLockedFields(ref m_impl, ref fields_vec));

            IntPtr fields_vec_size_ptr = IntPtr.Zero;
            PDFNetPINVOKE.TRN_VectorGetSize(fields_vec, ref fields_vec_size_ptr);
            int fields_vec_size = fields_vec_size_ptr.ToInt32();

            string[] result = new string[fields_vec_size];
            for (int i = 0; i < fields_vec_size; i++)
            {
                TRN_UString current_ustr = IntPtr.Zero;
                PDFNetPINVOKE.TRN_VectorGetAt(fields_vec, new IntPtr(i), ref current_ustr);
                result[i] = (new UString(current_ustr)).ConvToManagedStr();
            }
            PDFNetPINVOKE.TRN_VectorDestroyKeepContents(fields_vec);
            return result;
        }

        /// <summary>
        /// If HasCryptographicSignature, returns most restrictive permissions found in any reference entries in this digital signature. Returns Lock-resident (i.e. tentative) permissions otherwise. Throws if invalid permission value is found.
        /// </summary>
        /// <returns>An enumeration value representing the level of restrictions (potentially) placed on the document by this signature</returns>
        public DocumentPermissions GetDocumentPermissions()
        {
            DocumentPermissions result = DocumentPermissions.e_unrestricted;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldGetDocumentPermissions(ref m_impl, ref result));
            return result;
        }

        /// <summary>
        /// Clears cryptographic signature, if present. Otherwise, does nothing. Do not need to call HasCryptographicSignature before calling this. After clearing, other  signatures should still pass validation. Clears the appearance as well, like Acrobat does.
        /// </summary>
        public void ClearSignature()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DigitalSignatureFieldClearSignature(ref m_impl));
        }

        public enum SubFilterType
        {
            e_adbe_x509_rsa_sha1 = 0,
            e_adbe_pkcs7_detached = 1,
            e_adbe_pkcs7_sha1 = 2,
            e_ETSI_CAdES_detached = 3,
            e_ETSI_RFC3161 = 4,
            e_unknown = 5,
            e_absent = 6,
        }
        public enum DocumentPermissions
        {
            // No changes to the document shall be permitted; any change to the document shall invalidate the signature.
            e_no_changes_allowed = 1,
            // Permitted changes shall be filling in forms, instantiating page templates, and signing; other changes shall invalidate the signature.
            e_formfilling_signing_allowed = 2,
            // Permitted changes shall be the same as for 2, as well as annotation creation, deletion, and modification; other changes shall invalidate the signature.
            e_annotating_formfilling_signing_allowed = 3,
            // Represents the absence of any document permissions during retrieval; not to be used during setting
            e_unrestricted = 4,
        }
        public enum FieldPermissions
        {
            // Locks all form fields.
            e_lock_all = 0,
            // Locks only those form fields specified.
            e_include = 1,
            // Locks only those form fields not specified.
            e_exclude = 2,
        }

    }
}
