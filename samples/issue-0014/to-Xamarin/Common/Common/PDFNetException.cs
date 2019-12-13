using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using TRN_Exception = System.IntPtr;

namespace pdftron.Common
{
    /// <summary>  PDFNetException is derived from the standard exception class and it is a superclass 
    /// for all PDFNet library exceptions.
    /// 
    /// </summary>
    public class PDFNetException : System.Exception
    {
        // Fields
        internal TRN_Exception m_e = IntPtr.Zero;

        // Methods
        public PDFNetException() 
            : base("Unknown exception.")
        {
            m_e = IntPtr.Zero;
        }
        public PDFNetException(TRN_Exception e)
			: base(GetFullMessage(e))
        {
            this.m_e = e;
        }
        public PDFNetException(string cond_expr, string filename, int linenumber, string function, string message, ErrorCodes errorcode)
        {
            this.m_e = PDFNetPINVOKE.TRN_CreateExceptionEx(cond_expr, filename, linenumber, function, message, (uint)errorcode);
        }
        public string GetCondExpr() 
        {
            IntPtr result = PDFNetPINVOKE.TRN_GetCondExpr(m_e);
            return Marshal.PtrToStringUTF8(result);
        }
        public string GetFileName()
        {
            IntPtr result = PDFNetPINVOKE.TRN_GetFileName(m_e);
            return Marshal.PtrToStringUTF8(result);
        }
        public string GetFunction() 
        {
            IntPtr result = PDFNetPINVOKE.TRN_GetFunction(m_e);
            return Marshal.PtrToStringUTF8(result);
        }
        public string GetMessage()
        {
			return GetMessage(m_e);
		}
		private static string GetMessage(TRN_Exception e)
		{
			IntPtr result = PDFNetPINVOKE.TRN_GetMessage(e);
			return Marshal.PtrToStringUTF8(result);
		}

        public string GetFullMessage()
        {
            return GetFullMessage(m_e);
        }

        private static string GetFullMessage(TRN_Exception e)
        {
            IntPtr result = PDFNetPINVOKE.TRN_GetFullMessage(e);
            return Marshal.PtrToStringUTF8(result);
        }

        public int GetLineNumber()
		{
			return PDFNetPINVOKE.TRN_GetLineNum(m_e);
		}
        public ErrorCodes GetErrorCode()
        {
            uint code = PDFNetPINVOKE.TRN_GetErrorCode(m_e);
            return (ErrorCodes)code;
        }

        public enum ErrorCodes
        {
            e_error_general,
            e_error_network,
            e_error_credentials,
            e_error_num
        }

        internal static void REX(IntPtr result)
        {
            if (result != IntPtr.Zero)
            {
                throw new PDFNetException(result);
            }
        }
    }
}
