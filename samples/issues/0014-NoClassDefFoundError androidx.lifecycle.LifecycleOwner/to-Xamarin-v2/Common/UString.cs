using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using TRN_UString = System.IntPtr;
using TRN_Exception = System.IntPtr;

using pdftron;
using pdftron.Common;

namespace pdftronprivate.trn
{
    internal class UString : IDisposable
    {
        internal TRN_UString mp_impl = IntPtr.Zero;
        internal UString(TRN_UString imp)
        {
            this.mp_impl = imp;
        }
		internal UString(TRN_UString input, int length)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_UStringCreateFromSubstring(input, length, ref mp_impl));
		}

        ~UString()
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
            if (mp_impl != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_UStringDestroy(mp_impl));
                mp_impl = IntPtr.Zero;
            }
        }

        public UString()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_UStringCreate(ref mp_impl)); 
        }
        public UString(string buf)
        {
            if (null == buf)
            {
                throw new PDFNetException("PDFNetException", "UString.cs", 0, "UString(string)", "UString cannot be initialized with null string", PDFNetException.ErrorCodes.e_error_general);
            }
            var bytes = Encoding.UTF8.GetBytes(buf);
            PDFNetException.REX(PDFNetPINVOKE.TRN_UStringCreateFromCharString(bytes, bytes.Length, TextEncoding.e_utf8, ref mp_impl)); 
        }
		public static UString ConvertToUString(string to_convert)
		{
			UString ret = new UString(to_convert);
			return ret;
		}
        public bool Empty()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_UStringIsEmpty(mp_impl, ref result));
            return result;
        }

        public int GetLength()
        {
            int result = Int32.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_UStringGetLength(mp_impl, ref result));
            return result;
        }

        public char[] GetBuffer()
        {
            IntPtr source = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_UStringGetBuffer(mp_impl, ref source));
            int size = GetLength();
            char[] result = new char[size];
            System.Runtime.InteropServices.Marshal.Copy(source, result, 0, size);
            return result;
        }

        public string ConvToManagedStr()
        {
            return new string(GetBuffer(), 0, GetLength());
        }

        public enum TextEncoding
        {
            e_ascii_enc = 0,   ///< ASCII encoded text.
            e_pdfdoc_enc,      ///< PDFDocEncoding. See Appendix 'D' in PDF Reference.
            e_winansii_enc,    ///< WinAnsiiEncoding. See Appendix 'D' in PDF Reference.
            e_pdftext_enc,     ///< Text represented as PDF Text (section 3.8.1 'Text Strings' in PDF Reference).
            e_utf16be_enc,     ///< UTF-16BE (big-endian) encoding scheme.
            e_utf8,            ///< UTF-8 encoding scheme.
            e_no_enc           ///< No specific encoding.
        };
    }
}
