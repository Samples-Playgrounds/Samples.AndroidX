using System;
using System.Collections.Generic;
using System.Text;

using pdftron.Common;
using pdftron.SDF;
using pdftronprivate.trn;

using TRN_Exception = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.FDF
{
    //TODO: class description missing
    ///<summary>
    ///</summary>
    public class FDFField : IDisposable
    {
        internal BasicTypes.TRN_FDFField mp_imp;
        private Object m_ref;

        /// <summary> Releases all resources used by the FDFField </summary>
        ~FDFField()
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
            if (mp_imp.mp_leaf_node != IntPtr.Zero || mp_imp.mp_root_array != IntPtr.Zero)
            {
                mp_imp.mp_leaf_node = IntPtr.Zero;
                mp_imp.mp_root_array = IntPtr.Zero;
            }
        }

        internal FDFField(BasicTypes.TRN_FDFField imp, Object reference)
        {
            this.mp_imp = imp;
            this.m_ref = reference;
        }

        ///<summary> Construct a <c>FDF::FDFField</c> from a SDF dictionary representing a terminal field node.
        ///</summary>
        ///<param name="field_dict">FDFField dictionary</param>
        ///<param name="fdf_dict">FDF dictionary</param>
        public FDFField(Obj field_dict, Obj fdf_dict)
        {
            mp_imp = new BasicTypes.TRN_FDFField();
            mp_imp.mp_leaf_node = IntPtr.Zero;
            mp_imp.mp_root_array = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFFieldCreate(field_dict.mp_obj, fdf_dict.mp_obj, ref mp_imp));
            m_ref = field_dict.GetRefHandleInternal();
        }
        /// <summary> The function returns the specified attribute.			
        /// </summary>
        /// <param name="attrib">attribute name</param>
        /// <returns> return the attribute value if the given attribute name
        /// was found or a NULL object if the given attribute name was not found.
        /// </returns>
        public Obj FindAttribute(string attrib)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFFieldFindAttribute(ref mp_imp, attrib, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary> Get name of the FDFField
        /// 
        /// </summary>
        /// <returns> a string representing the fully qualified name of the field 
        /// (e.g. "employee.name.first").
        /// </returns>
        public string GetName()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFFieldGetName(ref mp_imp, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
        /// <summary> Get partial name of the FDFField
        /// 
        /// </summary>
        /// <returns> a string representing the partial name of the field (e.g. 
        /// "first" when "employee.name.first" is fully qualified name).
        /// </returns>
        public string GetPartialName()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFFieldGetPartialName(ref mp_imp, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
        /// <summary> Get underlying SDF object
        /// 
        /// </summary>
        /// <returns> the object to the underlying SDF/Cos object.
        /// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFFieldGetSDFObj(ref mp_imp, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary> Get value of the FDFField		
        /// </summary>
        /// <returns> the value of the Field (the value of its /V key) or NULL if the 
        /// value is not specified. 
        /// </returns>
        /// <remarks>  The format of field’s value varies depending on the field type.
        /// Set the value of the FDFField (the value of the field's /V key).
        /// the value of the field (the value of its /V key) or NULL if the 
        /// value is not specified. 		
        /// in order to remove/erase the existing value use <c>SetValue(SDF::Null)</c>
        /// </remarks>
        public Obj GetValue()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFFieldGetValue(ref mp_imp, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
        /// <summary> Assign value from another FDFField </summary>
        /// <param name="r">another <c>FDFField</c>object</param>
        /// <returns>a <c>FDFField</c> object equals to the give <c>FDFField</c></returns>
        public FDFField op_Assign(FDFField r)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFFieldAssign(ref this.mp_imp, ref r.mp_imp)); 
            return this;
        }
        /// <summary>Sets value to the given <c>FDFField</c> object</summary>
        /// <param name="rf">another FDFField object</param>
        public void Set(FDFField rf)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFFieldAssign(ref this.mp_imp, ref rf.mp_imp));
        }
        /// <summary> Set value of the FDFField		
        /// </summary>
        /// <param name="value">new value</param>
        /// <remarks>  The format of field’s value varies depending on the field type.
        /// Set the value of the FDFField (the value of the field's /V key).
        /// the value of the field (the value of its /V key) or NULL if the 
        /// value is not specified. 		
        /// in order to remove/erase the existing value use <c>SetValue(SDF::Null)</c>
        /// </remarks>
        public void SetValue(Obj value)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FDFFieldSetValue(ref mp_imp, value.mp_obj));
        }

    }
}
