using System;
using System.Collections.Generic;
using System.Text;

using pdftron.Common;
using pdftron.SDF;
using pdftronprivate.trn;

using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_GState = System.IntPtr;
using TRN_ViewChangeCollection = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> 
    /// <para>
    /// An interactive form (sometimes referred to as an AcroForm) is a 
    /// collection of fields for gathering information interactively from 
    /// the user. A PDF document may contain any number of Fields appearing 
    /// on any combination of pages, all of which make up a single, global 
    /// interactive form spanning the entire document.
    /// </para>
    /// <para>
    /// PDFNet fully supports reading, writing, and editing PDF forms and 
    /// provides many utility methods so that work with forms is simple and 
    /// efficient. Using PDFNet forms API arbitrary subsets of form fields 
    /// can be imported or exported from the document, new forms can be 
    /// created from scratch, and the appearance of existing forms can be 
    /// modified. 
    /// </para>
    /// 
    /// <example>
    /// In PDFNet Fields are accessed through FieldIterator-s. For a full 
    /// sample, please refer to 'InteractiveForms' sample project. The 
    /// list of all Fields present in the document can be traversed as 
    /// follows:	
    /// <code>  
    /// for(FieldIterator itr = doc.getFieldIterator(); itr.hasNext();) {
    /// Field current=(Field)(itr.next());
    /// System.out.println("Field name: " + current.getName());
    /// System.out.println("Field partial name: " + current.getPartialName());
    /// }
    /// </code>	
    /// To search field by name use FieldFind method. For example:	
    /// <code>
    /// // Search for a specific field
    /// FieldIterator itr = doc.fieldFind("employee.name.first");
    /// if (itr.hasNext()) System.out.println("Field search for " + ((Field)(itr.next())).getName() + " was successful");
    /// else System.out.println("Field search failed");
    /// </code>
    /// 
    /// If a given field name was not found or if the end of the field list 
    /// was reached the iterator <c>HasNext()</c> will return false.
    /// 
    /// If you have a valid iterator you can access the Field using <c>Current()</c> method. For example: 
    /// <code>Field field = itr.Current();</code>
    /// 
    /// Using <c>Flatten(...)</c> method it is possible to merge field 
    /// appearances with the page content. Form 'flattening' refers to the 
    /// operation that changes active form fields into a static area that is  
    /// part of the PDF document, just like the other text and images in 
    /// the document. A completely flattened PDF form does not have any 
    /// widget annotations or interactive fields. 
    /// </example>
    /// </summary>
    public class Field : IDisposable
    {
        internal BasicTypes.TRN_Field mp_field;
        internal Object m_ref;

        /// <summary> Releases all resources used by the Field </summary>
        ~Field()
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
            if (mp_field.leaf_node != IntPtr.Zero && mp_field.builder != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_FieldDestroy(ref mp_field));
                mp_field.leaf_node = IntPtr.Zero;
                mp_field.builder = IntPtr.Zero;
            }
        }

        internal Field(BasicTypes.TRN_Field imp, Object reference)
        {
            this.mp_field = imp;
            this.m_ref = reference;
        }

        /// <summary> Construct a Field from a SDF dictionary representing a terminal field node.
        /// 
        /// </summary>
        /// <param name="field_dict">the field_dict
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Field(Obj field_dict)
        {
            mp_field = new BasicTypes.TRN_Field();
            mp_field.builder = IntPtr.Zero;
            mp_field.leaf_node = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldCreate(field_dict.mp_obj, ref mp_field));
            this.m_ref = field_dict.GetRefHandleInternal();
        }
        /// <summary> Removes any appearances associated with the field.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void EraseAppearance()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldEraseAppearance(ref mp_field));
        }
        /// <summary> Some of the Field attributes are designated as inheritable.
        /// If such an attribute is omitted from a Field object, its value is inherited
        /// from an ancestor node in the Field tree. If the attribute is a required one,
        /// a value must be supplied in an ancestor node; if it is optional and no
        /// inherited value is specified, the default value should be used.
        /// 
        /// The function walks up the Field inhritance tree in search for specified
        /// attribute.
        /// 
        /// </summary>
        /// <param name="attrib">the attrib
        /// </param>
        /// <returns> The attribute value if the given attribute name was found
        /// or a NULL object if the given attribute name was not found.
        /// 
        /// Resources dictionary (Required; inheritable)
        /// MediaBox rectangle (Required; inheritable)
        /// CropBox rectangle (Optional; inheritable)
        /// Rotate integer (Optional; inheritable)
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj FindInheritedAttribute(string attrib)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldFindInheritedAttribute(ref mp_field, attrib, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary> Flatten/Merge existing form field appearances with the page content and
        /// remove widget annotation.
        /// 
        /// Form 'flattening' refers to the operation that changes active form fields
        /// into a static area that is part of the PDF document, just like the other
        /// text and images in the document. A completely flattened PDF form does not
        /// have any widget annotations or interactive fields.
        /// 
        /// </summary>
        /// <param name="page">the page
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>an alternative approach to set the field as read only is using 
        /// <c>Field.SetFlag(Field::e_read_only, true)</c> method. Unlike <c>Field.SetFlag(...)</c>,
        /// the result of <c>Flatten()</c> operation can not be programatically reversed.
        /// </remarks>
        public void Flatten(Page page)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldFlatten(ref mp_field, page.mp_page));
        }
        /// <summary> Gets the default graphics state.
        /// 
        /// </summary>
        /// <returns> The default graphics state that should be used in formatting the text. 
        /// The state corresponds to /DA entry in the field dictionary. </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public GState GetDefaultAppearance()
        {
            TRN_GState result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetDefaultAppearance(ref mp_field, ref result));
            return new GState(result, this.m_ref, null);
        }
        /// <summary> Gets the default value.
        /// 
        /// </summary>
        /// <returns> The default value to which the field reverts when a reset-form action
        /// is executed or NULL if the default value is not specified.
        /// 
        /// The format of field’s value varies depending on the field type.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetDefaultValue()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetDefaultValue(ref mp_field, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary> Gets the default value as string.
        /// 
        /// </summary>
        /// <returns> the default value as string
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetDefaultValueAsString()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetDefaultValueAsString(ref mp_field, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
        /// <summary> Gets the flag.
        /// 
        /// </summary>
        /// <param name="flag">the flag
        /// </param>
        /// <returns> the value of given field flag
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool GetFlag(Flag flag)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetFlag(ref mp_field, flag, ref result));
            return result;
        }
        /// <summary> Gets the justification.
        /// 
        /// </summary>
        /// <returns> the form of quadding (justification) to be used in displaying
        /// the text fields.
        /// </returns>				
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public TextJustification GetJustification()
        {
            TextJustification result = TextJustification.e_left_justified;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetJustification(ref mp_field, ref result)); 
            return result;
        }
        /// <summary> Gets the maximum length.
        /// 
        /// </summary>
        /// <returns> The maximum length of the field's text, in characters, or a
        /// negative number if the length is not limited.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  This method is specific to a text field. </remarks>
        public int GetMaxLen()
        {
            int result = Int32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetMaxLen(ref mp_field, ref result));
            return result;
        }
        /// <summary> Gets the name.
        /// 
        /// </summary>
        /// <returns> a string representing the fully qualified name of the field
        /// (e.g. "employee.name.first").
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetName()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetName(ref mp_field, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
        public string GetOpt(int index)
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetOpt(ref mp_field, index, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
        public int GetOptCount()
        {
            int result = Int32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetOptCount(ref mp_field, ref result)); 
            return result;
        }
        /// <summary> Gets the partial name.
        /// 
        /// </summary>
        /// <returns> a string representing the partial name of the field (e.g.
        /// "first" when "employee.name.first" is fully qualified name).
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetPartialName()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetPartialName(ref mp_field, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
        /// <summary> Gets the SDFObj.
        /// 
        /// </summary>
        /// <returns> the underlying SDF/Cos object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetSDFObj(ref mp_field, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary> Gets the type.
        /// 
        /// </summary>
        /// <returns> The field’s value, whose type/format varies depending on the field type.
        /// See the descriptions of individual field types for further information.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public new Type GetType()
        {
            Field.Type result = Type.e_null;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetType(ref mp_field, ref result));
            return result;
        }
        /// <summary> Return the rectangle that should be refreshed after changing a field.
        /// </summary>
        public Rect GetUpdateRect()
        {
            BasicTypes.TRN_Rect result = new BasicTypes.TRN_Rect();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetUpdateRect(ref mp_field, ref result));
            return new Rect(result);
        }
        /// <summary> Gets the value.
        /// 
        /// </summary>
        /// <returns> the value of the Field (the value of its /V key) or NULL if the
        /// value is not specified.
        /// 
        /// The format of field’s value varies depending on the field type.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetValue()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetValue(ref mp_field, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary> Gets the value as bool.
        /// 
        /// </summary>
        /// <returns> the value as bool
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool GetValueAsBool()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetValueAsBool(ref mp_field, ref result));
            return result;
        }
        /// <summary> Gets the value as string.
        /// 
        /// </summary>
        /// <returns> the value as string
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public string GetValueAsString()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldGetValueAsString(ref mp_field, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
        /// <summary> Checks if is annot.
        /// 
        /// </summary>
        /// <returns> true if this Field is a Widget Annotation
        /// 
        /// Determines whether or not this Field is an Annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsAnnot()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldIsAnnot(ref mp_field, ref result));
            return result;
        }
        /// <summary> Checks if current field is valid.
        /// 
        /// </summary>
        /// <returns> whether this is a valid (non-null) Field. If the function returns false 
        /// the underlying SDF/Cos object is null and the Field object should be treated as null 
        /// as well. </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldIsValid(ref mp_field, ref result));
            return result;
        }
        /// <summary>Assignment operator</summary>
        /// <param name="r">a <c>Field</c> object</param>			
        /// <returns>a <c>Field</c> object equals to given object</returns>
        public Field op_Assign(Field r)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldAssign(ref this.mp_field, ref r.mp_field));
            return this;
        }
        /// <summary> Regenerates the appearance stream for the Widget Annotation containing
        /// variable text. Call this method if you modified field's value and would
        /// like to update field's appearance.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks> If this field contains text, and has been added to a rotated page, the text in 
        /// the field may be rotated. If RefreshAppearance is called *after* the field is added 
        /// to a rotated page, then any text will be rotated in the opposite direction of the page 
        /// rotation. If this method is called *before* the field is added to any rotated page, then 
        /// no counter rotation will be applied. If you wish to call RefreshAppearance on a field 
        /// already added to a rotated page, but you don't want the text to be rotated, you can do one 
        /// of the following; temporarily un-rotate the page, or, temporarily remove the "P" object 
        /// from the field. </remarks>
        public void RefreshAppearance()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldRefreshAppearance(ref mp_field));
        }
        /// <summary> Modifies the field name.
        /// 
        /// </summary>
        /// <param name="field_name">a string representing the fully qualified name of
        /// the field (e.g. "employee.name.first").
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Rename(string field_name)
        {
            UString str = new UString(field_name);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldRename(ref mp_field, str.mp_impl));
        }
        /// <summary>Sets to given <c>Field</c> object</summary>
        /// <param name="p">a <c>Field</c> object</param>			
        public void Set(Field p)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldAssign(ref this.mp_field, ref p.mp_field));
        }
        /// <summary> Set the value of given FieldFlag.
        /// 
        /// </summary>
        /// <param name="flag">the flag
        /// </param>
        /// <param name="value">the value
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>You can use this method to set the field as read-only. An alternative approach to set the 
        /// field as read only is using <c>Page.Flatten(...)</c> method. Unlike <c>Flatten(...)</c>, the result of <c>SetFlag(...)</c> 
        /// can be programatically reversed.
        /// </remarks>
        public void SetFlag(Flag flag, bool value)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldSetFlag(ref mp_field, flag, value));
        }
        /// <summary> Sets the justification to be used in displaying the text field.
        /// 
        /// </summary>
        /// <param name="j">the new justification
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetJustification(TextJustification j)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldSetJustification(ref mp_field, j));
        }
        /// <summary> Sets the maximum length of the field's text, in characters.
        /// 
        /// </summary>
        /// <param name="max_len">the new maximum length
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  This method is specific to a text field. </remarks>
        public void SetMaxLen(int max_len)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldSetMaxLen(ref mp_field, max_len));
        }
        /// <summary> Sets the value of the Field (i.e. the value of the field's /V key).
        /// The format of field’s value varies depending on the field type.
        /// 
        /// </summary>
        /// <param name="value">the new value
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  in order to remove/erase the existing value use pass a <c>SDF::Null</c> object to <c>SetValue()</c>.		
        /// In PDF, Field's value is separate from its annotation (i.e. how the field appears on the page). 
        /// After you modify Field's value you need to
        /// refresh Field's appearance using RefreshAppearance() method.
        /// 
        /// Alternatively, you can delete "AP" entry from the Widget annotation and set
        /// "NeedAppearances" flag in AcroForm dictionary (i.e.
        /// doc.GetAcroForm().Put("NeedAppearances", Obj.CreateBool(true)); )
        /// This will force viewer application to auto-generate new field appearances
        /// every time the document is opened.
        /// 
        /// Yet another option is to generate a custom annotation appearance using
        /// ElementBuilder and ElementWriter and then set the "AP" entry in the widget
        /// dictionary to the new appearance stream. This functionality is useful in
        /// applications that need advanced control over how the form fields are rendered.
        /// </remarks>
        public ViewChangeCollection SetValue(Obj value)
        {
			TRN_ViewChangeCollection result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_FieldSetValue(ref this.mp_field, value.mp_obj, ref result));
			return new ViewChangeCollection(result);
        }
        /// <summary> Sets the value.
        /// 
        /// </summary>
        /// <param name="is_checked">the new value
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ViewChangeCollection SetValue(bool is_checked)
        {
			TRN_ViewChangeCollection result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_FieldSetValueAsBool(ref this.mp_field, is_checked, ref result));
			return new ViewChangeCollection(result);
        }
        /// <summary> Sets the value.
        /// 
        /// </summary>
        /// <param name="value">the new value
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ViewChangeCollection SetValue(string value)
        {
			TRN_ViewChangeCollection result = IntPtr.Zero;
            UString str = new UString(value);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldSetValueAsString(ref this.mp_field, str.mp_impl, ref result));
			return new ViewChangeCollection(result);
        }
        /// <summary> Sets the signature handler to use for adding a signature to this field. If the signature handler is not found
        /// in PDFDoc's signature handlers list, this field will not be signed. To add signature handlers, use PDFDoc.AddSignatureHandler
        /// method.
        /// If a signature handler is already assigned to this field and this method is called once again, the associate signature
        /// handler for this field will be updated with the new handler.
        /// </summary>
        /// <param name="signature_handler_id">The unique id of the SignatureHandler to use for adding signature in this field.</param>
        /// <returns>The signature dictionary created using the SignatureHandler, or null if the signature handler is not found.</returns>
        public Obj UseSignatureHandler(SignatureHandlerId signature_handler_id)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldUseSignatureHandler(ref mp_field, signature_handler_id, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary>
        /// Returns whether modifying this field would invalidate a digital signature in the document
        /// </summary>
        /// <returns>whether modifying this field would invalidate a digital signature in the document</returns>
        public Boolean IsLockedByDigitalSignature()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FieldIsLockedByDigitalSignature(ref mp_field, ref result));
            return result;
        }

        // Nested Types
        /// <summary> Flags specifying various characteristics of the fields.</summary>
        public enum Flag
        {
            ///<summary>If e_read_only flag is set the user may not change the value 
            /// of the field. Any associated widget annotations will not interact with 
            /// the user; that is, they will not respond to mouse clicks or change their 
            /// appearance in response to mouse motions. This flag is useful for fields 
            /// whose values are computed or imported from a database.</summary>
            e_read_only,
            ///<summary>If e_required flag is set, the field must have a value at the time 
            /// it is exported by a submit-form action.</summary>
            e_required,
            ///<summary>If e_no_export flag is set, the field must not be exported by a 
            /// submit-form action.</summary>
            e_no_export,
            ///<summary>Push Buttons flags</summary>
            e_pushbutton_flag,
            /// <summary>Radio buttons flags
            /// </summary>
            e_radio_flag,
            ///<summary>If e_toggle_to_off is clear, exactly one radio button must be selected 
            /// at all times; clicking the currently selected button has no effect. 
            /// If set, clicking the selected button deselects it, leaving no button selected.</summary>
            e_toggle_to_off,
            ///<summary>If e_radios_in_unison is set, a group of radio buttons within a radio button 
            /// field that use the same value for the on state will turn on and off in unison; 
            /// that is if one is checked, they are all checked. If clear, the buttons are 
            /// mutually exclusive (the same behavior as HTML radio buttons).</summary>
            e_radios_in_unison,
            ///<summary>If e_multiline is set, the field can contain multiple lines of text; 
            /// if clear, the field’s text is restricted to a single line.</summary>
            e_multiline,
            ///<summary>If e_password If set, the field is intended for entering a secure password 
            /// that should not be echoed visibly to the screen. Characters typed from the 
            /// keyboard should instead be echoed in some unreadable form, such as asterisks 
            /// or bullet characters. The value is not stored if this flag is set.</summary>
            e_password,
            ///<summary>If e_file_select is set, the text entered in the field represents the pathname 
            /// of a file whose contents are to be submitted as the value of the field.</summary>
            e_file_select,
            ///<summary>If e_no_spellcheck is set, text entered in the field is not spell-checked.</summary>
            e_no_spellcheck,
            ///<summary>If e_no_scroll is set, the field does not scroll (horizontally for single-line 
            /// fields, vertically for multiple-line fields) to accommodate more text than fits
            /// within its annotation rectangle. Once the field is full, no further text is 
            /// accepted.</summary>
            e_no_scroll,
            ///<summary>If e_comb is set, the field is automatically divided into as many equally
            /// spaced positions, or combs, as the value of MaxLen, and the text is laid out
            /// into those combs. Meaningful only if the MaxLen entry is present in the text 
            /// field and if the Multiline, Password, and FileSelect flags are clear. </summary>
            e_comb,
            ///<summary>If e_rich_text is set, the value of this field should be represented as a rich 
            /// text string. If the field has a value, the RV entry of the field dictionary 
            /// specifies the rich text string.</summary>
            e_rich_text,
            ///<summary>If e_combo is set, the field is a combo box; 
            /// if clear, the field is a list box.</summary>
            e_combo,                    // Choice fields flags
            ///<summary>If e_edit is set, the combo box includes an editable text box as well as a 
            /// dropdown list; if clear, it includes only a drop-down list. This flag is 
            /// meaningful only if the e_combo flag is set.</summary>
            e_edit,
            ///<summary>If e_sort is set, the field’s option items should be sorted alphabetically. 
            /// This flag is intended for use by form authoring tools, not by PDF viewer 
            /// applications. Viewers should simply display the options in the order in
            /// which they occur in the Opt array.</summary>
            e_sort,
            ///<summary>If e_multiselect is set, more than one of the field’s option items may be 
            /// selected simultaneously; if clear, no more than one item at a time may be
            /// selected.</summary>
            e_multiselect,
            ///<summary>If e_commit_on_sel_change is set, the new value is committed as soon as a 
            /// selection is made with the pointing device. This option enables applications 
            /// to perform an action once a selection is made, without requiring the user
            /// to exit the field. If clear, the new value is not committed until the user
            /// exits the field.</summary>
            e_commit_on_sel_change
        }

        ///<summary>form of quadding (justification) to be used in displaying the text fields.</summary>
	
        public enum TextJustification
        {
            ///<summary>left justified text style</summary>
            e_left_justified,
            ///<summary>centered text style</summary>
            e_centered,
            ///<summary>right justified style</summary>
            e_right_justified
        }

        ///<summary>interactive form field type</summary>
        public enum Type
        {
            ///<summary>Pushbutton field</summary>
            e_button,
            ///<summary>Check box field</summary>
            e_check,
            ///<summary>Radio button field</summary>
            e_radio,
            ///<summary>Text field</summary>
            e_text,
            ///<summary>Choice field</summary>
            e_choice,
            ///<summary>Digital signature field</summary>
            e_signature,
            ///<summary>Unknown field type</summary>
            e_null
        }


    }
}
