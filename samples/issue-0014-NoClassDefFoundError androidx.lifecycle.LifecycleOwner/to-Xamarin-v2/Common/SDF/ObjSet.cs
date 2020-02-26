using System;

using pdftronprivate.trn;
using pdftron;
using pdftron.Common;

using TRN_Obj = System.IntPtr;
using TRN_ObjSet = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.SDF
{
    /// <summary> ObjSet is a lightweight container that can hold a collection of SDF objects.</summary>
    public class ObjSet : IDisposable
    {
        internal TRN_ObjSet mp_set = IntPtr.Zero;

        /// <summary> Releases all resources used by the ObjSet </summary>
        ~ObjSet()
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
            if (mp_set != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetDestroy(mp_set));
                mp_set = IntPtr.Zero;
            }
        }

        /// <summary> Instantiates a new obj set.</summary>
        public ObjSet()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetCreate(ref mp_set));
        }

        /// <summary> Creates the name.
		/// 
		/// </summary>
		/// <param name="name">the name
		/// </param>
		/// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateName(string name)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetCreateName(mp_set, name, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
		/// <summary> Creates the array.
		/// 
		/// </summary>
		/// <returns> the obj
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateArray()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetCreateArray(mp_set, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
		/// <summary> Creates the bool.
		/// 
		/// </summary>
		/// <param name="value">the value
		/// </param>
		/// <returns> the obj
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateBool(bool value)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetCreateBool(mp_set, value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
		/// <summary> Creates the dict.
		/// 
		/// </summary>
		/// <returns> the obj
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateDict()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetCreateDict(mp_set, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
		/// <summary> Creates the null.
		/// 
		/// </summary>
		/// <returns> the obj
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateNull()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetCreateNull(mp_set, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
		/// <summary> Creates the number.
		/// 
		/// </summary>
		/// <param name="value">the value
		/// </param>
		/// <returns> the obj
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateNumber(double value)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetCreateNumber(mp_set, value, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
		/// <summary> Creates the string.
		/// 
		/// </summary>
		/// <param name="value">the value
		/// </param>
		/// <returns> the obj
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj CreateString(string value)
        {
            UString str = new UString(value);
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ObjSetCreateString(mp_set, str.mp_impl, ref result));
            return result != IntPtr.Zero ? new Obj(result, this) : null;
        }
    }
}
