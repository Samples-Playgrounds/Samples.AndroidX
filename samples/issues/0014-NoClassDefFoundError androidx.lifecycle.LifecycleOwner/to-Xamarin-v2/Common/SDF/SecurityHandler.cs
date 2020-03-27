using System;
using System.Runtime.InteropServices;

using pdftron.Common;

using TRN_SDFDoc = System.IntPtr;
using TRN_SecurityHandler = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.SDF
{
    /// <summary> Standard Security Handler is a built-in password-based security handler.</summary>
    public class SecurityHandler : IDisposable
    {
        internal TRN_SecurityHandler mp_handler = IntPtr.Zero;
        internal Object m_ref = null;

        internal SecurityHandler(TRN_SecurityHandler impl, Object reference)
        {
            this.mp_handler = impl;
            this.m_ref = reference;
        }

        internal void SetRefHandleInternal(Object reference)
        {
            this.m_ref = reference;
        }

        ~SecurityHandler()
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
            if (mp_handler != IntPtr.Zero && this.m_ref == null)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerDestroy(mp_handler));
                mp_handler = IntPtr.Zero;
            }
        }

        public SecurityHandler()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerCreateDefault(ref mp_handler));
        }

        public SecurityHandler(AlgorithmType type)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerCreate(type, ref mp_handler));
        }
        ///<summary>Create a standard security handler from a given handler</summary>
        ///<param name="s">a SecurityHandler instance</param>
        public SecurityHandler(SecurityHandler s)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerClone(s.mp_handler, ref mp_handler));
        }

        ///<summary>Creates a standard security handler</summary>
        ///<param name="key_len">The bit length of the encryption key (40 or 128 bit).</param>
        ///<param name="enc_code">The encryption algorithm identifier. The number corresponds to the V entry in encryption dictionary. Currently allowed values are (see Table 3.18 in PDF Reference Manual v1.6 for more details):
        ///<list type="number">
        ///<item><description>Encryption using 40-bit RC4 algorithm.</description></item>
        ///<item><description>Encryption using 128-bit RC4 algorithm. Available in PDF 1.4 and above.</description></item>
        ///<item><description>This algorithm was deprecated by PDF standard and is not supported.</description></item>
        ///<item><description>Encryption using Crypt filters and 128-bit AES (Advanced Encryption Standard) algorithm. Available in PDF 1.6 and above.</description></item></list></param>
        ///<param name="name">name of the security handler</param>
        public SecurityHandler(int key_len, int enc_code, string name)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerCreateFromEncCode(name, key_len, enc_code, ref mp_handler));
        }


        ///<summary>This method is invoked in case Authorize() failed.The callback must determine the user’s authorization properties for the document by obtaining authorization data (e.g. a password through a GUI dialog).The authorization data is subsequently used by the security handler’s Authorize() to determine whether or not the user is authorized to open the file.</summary>
        ///<param name="req_opr">the permission for which authorization data is requested.</param>
        ///<returns>false if the operation was canceled, true otherwise.</returns>
        public Boolean GetAuthorizationData(Permission req_opr) 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerGetAuthorizationData(mp_handler, req_opr, ref result));
            return result;
        }
		///<summary>A callback method indicating repeated failed authorization.  this callback in order to provide a UI feedback for failed authorization. Default implementation returns immediately.</summary>
		public void AuthorizeFailed()
        {
            // The default implementations is a no-op.
        }
		///<summary>Called when the security handler should activate a dialog box with the current security settings that may be modified.</summary>
		///<param name="doc">document to change security data</param>
		///<returns>true if the operation was successful false otherwise.</returns>
		public Boolean EditSecurityData(SDFDoc doc) { return false; }
		/// <summary>Clones instance of SecurityHandler.
		/// </summary>
		/// <returns>A new, cloned instance of SecurityHandler.
		/// </returns>
		/// <remarks>this method must be implemented in any derived class from SecurityHandler.
		/// </remarks>
        public SecurityHandler Clone() { return new SecurityHandler(mp_handler, true); }

		// The following methods implement some SecurityHandler's methods and 
		// should not be overridden. -----------------------------------------------------
		/// <summary>Initializes password
		/// </summary>
        /// <param name="password">initial password
        /// </param>
        public void InitPassword(String password) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerInitPassword(mp_handler, password));
        }

		
		/// <summary>Changes user password
		/// </summary>
        /// <param name="password">new password
        /// </param>
        public void ChangeUserPassword(String password) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerChangeUserPassword(mp_handler, password));
        }
		/// <summary>Gets user password
		/// </summary>
		/// <returns>user password
		/// </returns>
        public String GetUserPassword() 
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerGetUserPassword(mp_handler, ref result));
            return Marshal.PtrToStringUTF8(result);
        }

		/// <summary>Changes master password
		/// </summary>
		/// <param name="password">new master password
		/// </param>
        public void ChangeMasterPassword(String password) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerChangeMasterPassword(mp_handler, password));
        }
		/// <summary>Gets master password
		/// </summary>
		/// <returns>master password
		/// </returns>
        public String GetMasterPassword() 
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerGetMasterPassword(mp_handler, ref result));
            return Marshal.PtrToStringUTF8(result);
        }

		/// <summary>Set the permission setting of the StdSecurityHandler.
		/// </summary>
		/// <param name="perm">indicates a permission to set or clear. It can be any of the 
		/// following values:
		/// <code>
		///	e_print				// print the document.	
		///	e_doc_modify		// edit the document more than adding or modifying text notes.
		///	e_extract_content	// enable content extraction
		///	e_mod_annot			// allow modifications to annotations
		///	e_fill_forms		// allow changes to fill in forms
		///	e_access_support	// content access for the visually impaired.
		///	e_assemble_doc		// allow document assembly
		///	e_print_high		// high resolution print.
		/// </code>
		/// </param>
		/// <param name="value">true if the permission/s should be granted, false otherwise.
		/// </param>
        public void SetPermission(Permission perm, Boolean value) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerSetPermission(mp_handler, perm, value));
        }

		/// <summary>Changes revision number
		/// </summary>
		/// <param name="rev_num">new revision number
		/// </param>
        public void ChangeRevisionNumber(Int32 rev_num) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerChangeRevisionNumber(mp_handler, rev_num));
        }
		/// <summary>Gets revision number
		/// </summary>
		/// <returns>revsion number
		/// </returns>
        public Int32 GetRevisionNumber() 
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerGetRevisionNumber(mp_handler, ref result));
            return result;
        }

		/// <summary>Checks if user password required
		/// </summary>
		/// <returns>true if user password required, false otherwise
		/// </returns>
        public Boolean IsUserPasswordRequired() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerIsUserPasswordRequired(mp_handler, ref result));
            return result;
        }
		/// <summary>Checks if master password required
		/// </summary>
		/// <returns>true if master password is required, false otherwise
		/// </returns>
        public Boolean IsMasterPasswordRequired() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerIsMasterPasswordRequired(mp_handler, ref result));
            return result;
        }
		//TODO: return description missing
		///<summary>The method is called when a user tries to set security for an encrypted document and when a user tries to open a file. It must decide, based on the contents of the authorization data structure, whether or not the user is permitted to open the file, and what permissions the user has for this file.</summary>
		///<param name="req_opr">permission to authorize</param>
		///<returns></returns>
		///<remarks>This callback must not obtain the authorization data <example> by displaying a user interface into which a user can type a password). This is handled by the security handler’s GetAuthorizationData(), which must be called before this callback. Instead, Authorize() should work with authorization data it has access to.</example></remarks>
        public Boolean Authorize(Permission req_opr)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerAuthorize(mp_handler, req_opr, ref result));
            return result;
        }
		/// <summary>Called when an encrypted document is saved. Fills the document's Encryption 
		/// dictionary with whatever information the security handler wants to store in 
		/// the document.
		/// <list type="bullet">
		/// The sequence of events during creation of the encrypt_dict is as follows:
		/// <item><description>encrypt_dict is created (if it does not exist)</description></item>
		/// <item><description>Filter attribute is added to the dictionary</description></item>
		/// <item><description>call this method to allow the security handler to add its own attributes</description></item>
		/// <item><description>call the GetCryptKey to get the algorithm version, key, and key length</description></item>
		/// <item><description>checks if the V attribute has been added to the dictionary and, if not, then sets V to the algorithm version</description></item>
		/// <item><description>set the Length attribute if V is 2 or greater</description></item>
		/// <item><description>add the encrypt_dict to the document</description></item></list>
		/// </summary>
		/// <param name="doc">The document to save.
		/// </param>
		/// <returns>encrypt_dict
		/// </returns>
        public Obj FillEncryptDict(SDFDoc doc)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerFillEncryptDict(mp_handler, doc.mp_doc, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, null));
        }

		/*
		// Ivan: Dec 23. 2005. The following methods are deprecated in the .NET version. 
		// The main reason is that it is unlikely that the new sec handlers will be 
		// ever implemented under .NET from scratch.
		public Filter* EncryptStream(Filter* filter, Int32 obj_num, Int32 gen_num);
		public Filter* DecryptStream(Filter* filter, Int32 obj_num, Int32 gen_num);
		public void EncryptString(Byte in_out_str[], Int32 obj_num, Int32 gen_num);
		public void DecryptString(Byte in_out_str[], Int32 obj_num, Int32 gen_num);
		*/

		///<summary>Gets permission</summary>
		///<param name="p">A Permission to be granted.</param>
		///<returns>true if the SecurityHandler permits the specified action <c>p</c> on the document, or false if the permission was not granted</returns>
		///<remarks>in order to check for permission the method will repeatedly (up to three times) attempt to GetAuthorizationData() and Authorize() permission. If the permission is not granted AuthorizeFailed() callback will be called. This callback method allows derived class to provide UI feedback for failed authorization.</remarks>
        public Boolean GetPermission(Permission p)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerGetPermission(mp_handler, p, ref result));
            return result;
        }
		///<summary>Gets key length
		///</summary>
		///<returns>length of the encryption key
		///</returns>
        public Int32 GetKeyLength()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerGetKeyLength(mp_handler, ref result));
            return result;
        }
		///<summary>Gets encryption algorithm
		///</summary>
		///<returns>encryption algorithm
		///</returns>
        public Int32 GetEncryptionAlgorithmID()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerGetEncryptionAlgorithmID(mp_handler, ref result));
            return result;
        }

		///<summary>Checks if security handler is modified
		///</summary>
		///<returns>true if security handler is modified, false otherwise
		///</returns>
        public Boolean IsModified()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerIsModified(mp_handler, ref result));
            return result;
        }
		/// <summary> Gets the handler doc name.
		/// 
		/// </summary>
		/// <returns> The name of the security handler as it appears in the serialized file
		/// as the value of /Filter key in /Encrypt dictionary.
		/// </returns>
        public String GetHandlerDocName()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerGetHandlerDocName(mp_handler, ref result));
            return Marshal.PtrToStringUTF8(result);
        }
		///<summary>Sets if security handler is modified
		///</summary>
        public void SetModified()
        {
            SetModified(true);
        }
		///<summary>Sets if security handler is modified
		///</summary>
		///<param name="is_modified">true if the security handler is modified
		///</param>
        public void SetModified(Boolean is_modified)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerSetModified(mp_handler, is_modified));
        }

		// v 3.1
		///<summary>Sets whether to encrypt metadata
		///</summary>
        ///<param name="encrypt_metadata">whether to encrypt metadata
        ///</param>
        public void SetEncryptMetadata(Boolean encrypt_metadata)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerSetEncryptMetadata(mp_handler, encrypt_metadata));
        }
		///<summary>Checks if AES encryption algorithm is used
		///</summary>
		///<returns>true, if AES is used, false otherwise
		///</returns>
        public Boolean IsAES()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerIsAES(mp_handler, ref result));
            return result;
        }
		///<summary>The following function can be used to verify whether a given stream is 
		///encrypted using AES.
		///</summary>
		///<param name="stream">A pointer to an <c>SDF::Stream</c> object
		///</param>
        ///<returns>true if the given stream is encrypted using AES encryption.
        ///</returns>
        public Boolean IsAES(Obj stream)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerIsAESObj(mp_handler, stream.mp_obj, ref result));
            return result;
        }
		///<summary>Checks if RC4 encryption algorithm is used
		///</summary>
        ///<returns>true, if RC4 is used, false otherwise
        ///</returns>
        public Boolean IsRC4()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_SecurityHandlerIsRC4(mp_handler, ref result));
            return result;
        }

        // Nested Types
        ///<summary>permission types</summary>
        public enum Permission
        {
            ///<summary> the user has 'owner' rights (e.g. rights to change the document's security settings)</summary>
            e_owner = 1,
            ///<summary> open and decrypt the document.</summary>
            e_doc_open,
            ///<summary> edit the document more than adding or modifying text notes.</summary>
            e_doc_modify,
            ///<summary> print the document.</summary>
            e_print,
            ///<summary> high resolution print.</summary>
            e_print_high,
            ///<summary> enable content extraction</summary>
            e_extract_content,
            ///<summary> allow modifications to annotations</summary>
            e_mod_annot,
            ///<summary> allow changes to fill in forms</summary>
            e_fill_forms,
            ///<summary> content access for the visually impaired.</summary>
            e_access_support,
            ///<summary> allow document assembly</summary>
            e_assemble_doc
        }

        // Nested Types
        ///<summary>Algorithm types. New in PDFNet v3.1</summary>
        public enum AlgorithmType
        {
            ///<summary> 40-bit RC4 algorithm.</summary>
            e_RC4_40 = 1,
            ///<summary> 128-bit RC4 algorithm.</summary>
            e_RC4_128,
            ///<summary> Use Crypt filters with 128-bit AES (Advanced Encryption Standard) algorithm.</summary>
            e_AES,
            ///<summary> Use Crypt filters with 256-bit AES (Advanced Encryption Standard) algorithm.</summary>
            e_AES_256
        }

    }
}
