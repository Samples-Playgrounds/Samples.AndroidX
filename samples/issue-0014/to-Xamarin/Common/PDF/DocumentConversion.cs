using System;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_DocumentConversion = System.IntPtr;
using TRN_PDFDoc = System.IntPtr;

namespace pdftron.PDF
{
	public enum DocumentConversionResult
	{
		e_document_conversion_success = 0,
		e_document_conversion_incomplete = 1,
		e_document_conversion_failure = 2,
	};
	/// <summary>
	/// Encapsulates the conversion of a single document from one format to another.
	/// </summary>
	/// <remark>
	/// DocumentConversion instances are created through methods belonging to
	/// the Convert class. See Convert.WordToPDFConversion for an example.
	/// </remark>
	public class DocumentConversion : IDisposable
	{
		internal TRN_DocumentConversion m_impl = IntPtr.Zero;

		public DocumentConversion(TRN_DocumentConversion impl_ptr)
		{
			m_impl = impl_ptr;
		}

        public static DocumentConversion CreateInternal(TRN_DocumentConversion imp)
        {
            return new DocumentConversion(imp);
        }

        internal IntPtr GetHandleInternal()
        {
            return this.m_impl;
        }

        ~DocumentConversion()
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
        	if(m_impl != IntPtr.Zero)
        	{
        		TRN_DocumentConversion to_delete = m_impl;
        		m_impl = IntPtr.Zero;
            	PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionDestroy(ref to_delete));
            }
        }



		/// <summary>
        /// Perform the conversion. If the result of the conversion is failure, then GetErrorString will contain further information about the failure.
        /// </summary>
        /// <returns>Indicates that the conversion succeeded, failed, or was cancelled</returns>
		public DocumentConversionResult TryConvert()
		{
			DocumentConversionResult result = DocumentConversionResult.e_document_conversion_failure;
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionTryConvert(m_impl, ref result));
			return result;
		}

		/// <summary>
        /// Perform the conversion. Will throw an exception on failure.
        /// </summary>
		public void Convert()
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionConvert(m_impl));
		}

		/// <summary>
        /// Perform the conversion. Will throw an exception on failure. Does nothing if the conversion is already complete. Use GetConversionStatus() to check if there is remaining content to be converted.
        /// </summary>
		public void ConvertNextPage()
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionConvertNextPage(m_impl));
		}

		/// <summary>
        /// Gets the PDFDoc from the conversion. Can be accessed at any time during or after conversion.
        /// </summary>
        /// <returns>The conversion's PDFDoc</returns>
		public PDFDoc GetDoc()
		{
			TRN_PDFDoc result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionGetDoc(m_impl, ref result));
			return new PDFDoc(result);
		}

		/// <summary>
        /// Get the state of the conversion process. Pair this with ConvertNextPage().
        /// </summary>
        /// <returns></returns>
		public DocumentConversionResult GetConversionStatus()
		{
			DocumentConversionResult result = DocumentConversionResult.e_document_conversion_failure;
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionGetConversionStatus(m_impl, ref result));
			return result;
		}

		/// <summary>
        /// Cancel the current conversion, forcing TryConvert or Convert to return.
        /// </summary>
		public void CancelConversion()
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionCancelConversion(m_impl));
		}

		/// <summary>
        /// Has the conversion been cancelled?.
        /// </summary>
        /// <returns>Returns true if CancelConversion has been called previously</returns>
		public bool IsCancelled()
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionIsCancelled(m_impl, ref result));
			return result;
		}

		/// <summary>
        /// Determine whether this DocumentConversion has progress reporting capability.
        /// </summary>
        /// <returns>True if GetProgress is expected to return usable values</returns>
		public bool HasProgressTracking()
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionHasProgressTracking(m_impl, ref result));
			return result;
		}

		/// <summary>
        /// Returns a number from 0.0 to 1.0, representing the best estimate of conversion progress. This number is only an indicator, and should not be used to dictate program logic (in particular, it is possible for this method to return 1.0 while there is still work to be done. Use GetConversionStatus() to find out when the conversion is fully complete).
        /// </summary>
        /// <returns>The conversion progress. Will never return a smaller number than a previous call</returns>
		public double GetProgress()
		{
			double result = 0;
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionGetProgress(m_impl, ref result));
			return result;
		}

		/// <summary>
        /// Returns the label for the current conversion stage. May return a blank string. Warning: experimental interface; this method may be renamed or replaced with equivalent functionality in the future.
        /// </summary>
        /// <returns>The stage label</returns>
		public string GetProgressLabel()
		{
			UString result = new UString();
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionGetProgressLabel(m_impl, ref result.mp_impl));
			return result.ConvToManagedStr();
		}

		/// <summary>
        /// Returns the number of pages which have been added to the destination document. Will never decrease, and will not change after the conversion status becomes "complete".
        /// </summary>
        /// <returns>The number of pages that have been converted</returns>
		public int GetNumConvertedPages()
		{
			uint result = 0;
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionGetNumConvertedPages(m_impl, ref result));
			return (int)result;
		}

		/// <summary>
        /// If the conversion finsihed with some kind of error, this returns the value of the the error description ()otherwise returns an empty string.
        /// </summary>
        /// <returns>The error description. Will be blank unless GetConversionStatus returns Failure</returns>
		public string GetErrorString()
		{
			UString result = new UString();
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionGetErrorString(m_impl, ref result.mp_impl));
			return result.ConvToManagedStr();
		}

		/// <summary>
        /// Return the number of warning strings generated during the conversion process. Warning: experimental interface; this method may be renamed or replaced with equivalent functionality in the future.
        /// </summary>
        /// <returns>The number of stored warning strings</returns>
		public int GetNumWarnings()
		{
			uint result = 0;
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionGetNumWarnings(m_impl, ref result));
			return (int)result;
		}

		/// <summary>
        /// Retrieve warning strings that have been collected during the conversion process. Warning: experimental interface; this method may be renamed or replaced with equivalent functionality in the future.
        /// </summary>
        /// <param name="index">the index of the string to be retrieved. Must be less than GetNumWarnings()</param>
        /// <returns>The value of the particular warning string</returns>
		public string GetWarningString(int index)
		{
			UString result = new UString();
			PDFNetException.REX(PDFNetPINVOKE.TRN_DocumentConversionGetWarningString(m_impl, (uint)index, ref result.mp_impl));
			return result.ConvToManagedStr();
		}
	}
}
