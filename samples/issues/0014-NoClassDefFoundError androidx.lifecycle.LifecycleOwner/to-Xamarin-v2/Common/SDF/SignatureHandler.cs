using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Exception = System.IntPtr;
using TRN_SignatureHandler = System.IntPtr;
using TRN_UString = System.IntPtr;

namespace pdftron.SDF
{
    /// <summary>A base class for SignatureHandler. SignatureHandler instances are responsible for defining the digest and cipher
    /// algorithms to sign and/or verify a PDF document. SignatureHandlers are added to PDFDoc instances by calling the
    /// PDFDoc.AddSignatureHandler method.
    /// </summary>
    public abstract class SignatureHandler
    {
        //internal TRN_SignatureHandler mp_imp = IntPtr.Zero;
        private IntPtr pnt = IntPtr.Zero;
        //internal SignatureHandler(TRN_SignatureHandler imp)
        //{
        //    this.mp_imp = imp;
        //}
        internal TRN_Exception TRN_SignatureHandlerGetNameFunction(ref TRN_UString name, IntPtr unused)
        {
            try
            {
                if (unused == IntPtr.Zero)
                {
                    PDFNetException exception = new PDFNetException("Exception", "SignatureHandler.cs", 0, "SignatureHandler.GetName", "Failed to obtain derived instance of pdftron.SDF.SignatureHandler.", PDFNetException.ErrorCodes.e_error_general);
                    return exception.m_e;
                }
                string result = GetName();
                pdftronprivate.trn.UString result2 = new pdftronprivate.trn.UString(result);
                name = result2.mp_impl;
                GC.KeepAlive(unused);
                return IntPtr.Zero;
            }
            catch (System.Exception ex)
            {
                PDFNetException exception = new PDFNetException("Exception", "SignatureHandler.cs", 0, "SignatureHandler.GetName", ex.Message, PDFNetException.ErrorCodes.e_error_general);
                return exception.m_e;
            }
        }
        internal TRN_Exception TRN_SignatureHandlerAppendDataFunction(BasicTypes.TRN_SignatureData data, IntPtr unused)
        {            
            try
            {
                if (unused == IntPtr.Zero)
                {
                    PDFNetException exception = new PDFNetException("Exception", "SignatureHandler.cs", 0, "SignatureHandler.AppendData", "Failed to obtain derived instance of pdftron.SDF.SignatureHandler.", PDFNetException.ErrorCodes.e_error_general);
                    return exception.m_e;
                }
                
                int size = System.Convert.ToInt32(data.length.ToUInt32());
                byte[] dataToAppend = new byte[size];
                System.Runtime.InteropServices.Marshal.Copy(data.data, dataToAppend, 0, size);
                
                AppendData(dataToAppend);
                GC.KeepAlive(unused);
                return IntPtr.Zero;
            }
            catch (System.Exception ex)
            {
                PDFNetException exception = new PDFNetException("Exception", "SignatureHandler.cs", 0, "SignatureHandler.AppendData", ex.Message, PDFNetException.ErrorCodes.e_error_general);
                return exception.m_e;
            }
        }

        internal TRN_Exception TRN_SignatureHandlerResetFunction([MarshalAs(UnmanagedType.U1)] ref bool result, IntPtr unused)
        {
            try
            {
                if (unused == IntPtr.Zero)
                {
                    PDFNetException exception = new PDFNetException("Exception", "SignatureHandler.cs", 0, "SignatureHandler.Reset", "Failed to obtain derived instance of pdftron.SDF.SignatureHandler.", PDFNetException.ErrorCodes.e_error_general);
                    return exception.m_e;
                }
                result = Reset();
                GC.KeepAlive(unused);
                return IntPtr.Zero;
            }
            catch (System.Exception ex)
            {
                PDFNetException exception = new PDFNetException("Exception", "SignatureHandler.cs", 0, "SignatureHandler.Reset", ex.Message, PDFNetException.ErrorCodes.e_error_general);
                return exception.m_e;
            }
        }

        internal TRN_Exception TRN_SignatureHandlerCreateSignatureFunction(ref BasicTypes.TRN_SignatureData signature, IntPtr unused)
        {
            try
            {
                if (unused == IntPtr.Zero)
                {
                    PDFNetException exception = new PDFNetException("Exception", "SignatureHandler.cs", 0, "SignatureHandler.CreateSignature", "Failed to obtain derived instance of pdftron.SDF.SignatureHandler.", PDFNetException.ErrorCodes.e_error_general);
                    return exception.m_e;
                }

                if (true)
                {
                    byte[] source = CreateSignature();
                    if (source.Length > 0)
                    {
                        int size = Marshal.SizeOf(source[0]) * source.Length;
                        pnt = Marshal.AllocHGlobal(size);
                        Marshal.Copy(source, 0, pnt, source.Length);
                        signature.data = pnt;
                        signature.length = new UIntPtr(System.Convert.ToUInt32(source.Length));
                    }
                }
                GC.KeepAlive(unused);
                return IntPtr.Zero;

                /*BasicTypes.TRN_SignatureData signature = new BasicTypes.TRN_SignatureData();
                mp_imp = Marshal.AllocHGlobal(Marshal.SizeOf(signature));

                // convert managed array to unmanaged memory to hold the array
                byte[] source = CreateSignature();
                int size = Marshal.SizeOf(source[0]) * source.Length;
                pnt = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.Copy(source, 0, pnt, source.Length);
                    signature.data = pnt;
                    signature.length = System.Convert.ToUInt32(source.Length);

                    Marshal.StructureToPtr(signature, mp_imp, false);
                    sigData = mp_imp;

                    return IntPtr.Zero;
                }
                finally
                {
                    // Free pnt in DestructorFunction
                }*/
            }
            catch (System.Exception ex)
            {
                PDFNetException exception = new PDFNetException("Exception", "SignatureHandler.cs", 0, "SignatureHandler.CreateSignature", ex.Message, PDFNetException.ErrorCodes.e_error_general);
                return exception.m_e;
            }
        }

        internal TRN_Exception TRN_SignatureHandlerDestructorFunction(IntPtr unused)
        {
            try
            {
                if (unused == IntPtr.Zero)
                {
                    PDFNetException exception = new PDFNetException("Exception", "SignatureHandler.cs", 0, "SignatureHandler.Destructor", "Failed to obtain derived instance of pdftron.SDF.SignatureHandler.", PDFNetException.ErrorCodes.e_error_general);
                    return exception.m_e;
                }
                Marshal.FreeHGlobal(pnt);
                pnt = IntPtr.Zero;
                //Marshal.FreeHGlobal(mp_imp);
                GC.KeepAlive(unused);
                return IntPtr.Zero;
            }
            catch (System.Exception ex)
            {
                PDFNetException exception = new PDFNetException("Exception", "SignatureHandler.cs", 0, "SignatureHandler.Destructor", ex.Message, PDFNetException.ErrorCodes.e_error_general);
                return exception.m_e;
            }
            
        }

        public SignatureHandler() { }

        // Methods
        /// <summary> Adds data to be signed. This data will be the raw serialized byte buffer as the PDF is being saved to any stream.
        /// </summary>
        /// <param name="data">A chunk of data to be signed.</param>
        public abstract void AppendData(byte[] data);
        /// <summary> Calculates the actual signature using client implemented signing methods. The returned value (byte array) will
        /// be written as the /Contents entry in the signature dictionary.
        /// </summary>
        /// <returns>The calculated signature data.</returns>
        public abstract byte[] CreateSignature();
        /// <summary> Gets the name of this SignatureHandler. The name of the SignatureHandler is what identifies this SignatureHandler
        /// from all others. This name is also added to the PDF as the value of /Filter entry in the signature dictionary.
        /// </summary>
        /// <returns>The name of this SignatureHandler.</returns>
        public abstract string GetName();
        /// <summary> Resets any data appending and signature calculations done so far. This method should allow PDFNet to restart the
        /// whole signature calculation process. It is important that when this method is invoked, any data processed with
        /// the AppendData method should be discarded.
        /// </summary>
        /// <returns>True if there are no errors, otherwise false.</returns>
        public abstract bool Reset();
    }

    /// <summary> Used for identifying a SignatureHandler instances as they are added to the PDFDoc's SignatureManager.
    /// </summary>
    public struct SignatureHandlerId
    {
        internal UIntPtr mp_imp;
        private SignatureHandlerId(UIntPtr imp)
        {
            this.mp_imp = imp;
        }
        public static implicit operator SignatureHandlerId(UIntPtr value)
        {
            return new SignatureHandlerId(value);
        }
        public static implicit operator UIntPtr(SignatureHandlerId handler)
        {
            return handler.mp_imp;
        }
    }
}
