using System;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;


using TRN_PDFDoc = System.IntPtr;

namespace pdftron.PDF
{
	public enum PDFNetInternalToolsLogBackend
	{
		e_pdf_net_internal_tools_debugger = 0,
		e_pdf_net_internal_tools_disk = 1,
		e_pdf_net_internal_tools_callback = 2,
		e_pdf_net_internal_tools_console = 3,
	}
	public enum PDFNetInternalToolsLogLevel
	{
		e_pdf_net_internal_tools_trace = 0,
		e_pdf_net_internal_tools_debug = 1,
		e_pdf_net_internal_tools_info = 2,
		e_pdf_net_internal_tools_warning = 3,
		e_pdf_net_internal_tools_error = 4,
		e_pdf_net_internal_tools_fatal = 5,
		e_pdf_net_internal_tools_disabled = 6,
	}
	/// <summary>
	/// Encapsulates the conversion of a single document from one format to another.
	/// </summary>
	/// <remark>
	/// DocumentConversion instances are created through methods belonging to
	/// the Convert class. See Convert.WordToPDFConversion for an example.
	/// </remark>
	public static class PDFNetInternalTools
	{


		/// <summary>
        /// Find out whether the logging system is built into this particular binary.
        /// </summary>
        /// <returns>returns true if TRN_LOG_ENABLE is defined in core</returns>
		public static bool IsLogSystemAvailable()
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsIsLogSystemAvailable(ref result));
			return result;
		}

		/// <summary>
        /// Attempt to configure the logging ssytem with a json configuration file.
        /// </summary>
        /// <param name="config_string">Configuration Data in json form</param>
        /// <returns>Indicates if the configuration was successful</returns>
		public static bool ConfigureLogFromJsonString(string config_string)
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsConfigureLogFromJsonString(UString.ConvertToUString(config_string).mp_impl, ref result));
			return result;
		}

		/// <summary>
        /// Get the default configuration json file. You could then modify it and pass it into ConfigureLogFromJsonString.
        /// </summary>
        /// <returns>The json string representing the default log configuration</returns>
		public static string GetDefaultConfigFile()
		{
			UString result = new UString();
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsGetDefaultConfigFile(ref result.mp_impl));
			return result.ConvToManagedStr();
		}

		/// <summary>
        /// Run universal conversion tests on all the documents found in the given path.
        /// </summary>
        /// <param name="path_with_docs">The path to search within for documents to convert</param>
        /// <returns>The json string representing the test results</returns>
		public static string RunUniversalConversionTests(string path_with_docs)
		{
			UString result = new UString();
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsRunUniversalConversionTests(UString.ConvertToUString(path_with_docs).mp_impl, ref result.mp_impl));
			return result.ConvToManagedStr();
		}

		/// <summary>
        /// Log a message to a particular stream using the core logging routines.
        /// </summary>
        /// <param name="threshold">the importance of this log message</param>
        /// <param name="message">the message to be logged</param>
        /// <param name="filename">the filename from which the log message originates</param>
        /// <param name="line_number">the line number from which the log message originates</param>
		public static void LogMessage(PDFNetInternalToolsLogLevel threshold, string message, string filename, uint line_number)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsLogMessage(threshold, UString.ConvertToUString(message).mp_impl, UString.ConvertToUString(filename).mp_impl, line_number));
		}

		/// <summary>
        /// Log a message to a particular stream using the core logging routines.
        /// </summary>
        /// <param name="threshold">the importance of this log message</param>
        /// <param name="stream">the name of the stream to which the message belongs (a category label)</param>
        /// <param name="message">the message to be logged</param>
        /// <param name="filename">the filename from which the log message originates</param>
        /// <param name="line_number">the line number from which the log message originates</param>
		public static void LogStreamMessage(PDFNetInternalToolsLogLevel threshold, string stream, string message, string filename, uint line_number)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsLogStreamMessage(threshold, UString.ConvertToUString(stream).mp_impl, UString.ConvertToUString(message).mp_impl, UString.ConvertToUString(filename).mp_impl, line_number));
		}

		/// <summary>
        /// Set the directory and filename to log to. If the directory does not exist, it will be created.
        /// </summary>
        /// <param name="log_directory">the path of the directory to log into</param>
        /// <param name="log_filename">the name of the file to log into</param>
        /// <returns>returns true we were able to open a log file successfully</returns>
		public static bool SetLogLocation(string log_directory, string log_filename)
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsSetLogLocation(UString.ConvertToUString(log_directory).mp_impl, UString.ConvertToUString(log_filename).mp_impl, ref result));
			return result;
		}

		/// <summary>
        /// Set the filename to log to.
        /// </summary>
        /// <param name="log_filename">the name of the file to log into</param>
        /// <returns>returns true we were able to open a log file successfully</returns>
		public static bool SetLogFileName(string log_filename)
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsSetLogFileName(UString.ConvertToUString(log_filename).mp_impl, ref result));
			return result;
		}

		/// <summary>
        /// set the log level for some particular stream.
        /// </summary>
        /// <param name="stream_name">the name of the stream you wish to configure</param>
        /// <param name="stream_threshold">the stream threshold. Entries with a priority greater than or equal to this level will be logged</param>
		public static void SetThresholdForLogStream(string stream_name, PDFNetInternalToolsLogLevel stream_threshold)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsSetThresholdForLogStream(UString.ConvertToUString(stream_name).mp_impl, stream_threshold));
		}

		/// <summary>
        /// set the log level for streams that do not otherwise have their level set.
        /// </summary>
        /// <param name="threshold">Entries with a priority greater than or equal to this level will be logged</param>
		public static void SetDefaultLogThreshold(PDFNetInternalToolsLogLevel threshold)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsSetDefaultLogThreshold(threshold));
		}

		/// <summary>
        /// set the global log cutoff. *No* log statements with a level less than this will pass.
        /// </summary>
        /// <param name="threshold">The threshold. Entries with a priority greater than or equal to this level will be logged</param>
		public static void SetCutoffLogThreshold(PDFNetInternalToolsLogLevel threshold)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsSetCutoffLogThreshold(threshold));
		}

		/// <summary>
        /// Enable a particular log backend.
        /// </summary>
        /// <param name="backend">The log backend to enable</param>
        /// <returns>Returns true if the backend is available and functioning</returns>
		public static bool EnableLogBackend(PDFNetInternalToolsLogBackend backend)
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsEnableLogBackend(backend, ref result));
			return result;
		}

		/// <summary>
        /// Disable a particular log backend.
        /// </summary>
        /// <param name="backend">The log backend to disable</param>
		public static void DisableLogBackend(PDFNetInternalToolsLogBackend backend)
		{
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsDisableLogBackend(backend));
		}

		/// <summary>
        /// Get a summary of the held tiles of all the know instances of PDFViewImplTiled.
        /// </summary>
        /// <returns>The json string representing the tile summary</returns>
		public static string GetPDFViewTileSummary()
		{
			UString result = new UString();
			PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsGetPDFViewTileSummary(ref result.mp_impl));
			return result.ConvToManagedStr();
		}

		/// <summary>
        /// Ensure that a document is well formed, and that its in-memory representation matches its backing filter.
        /// </summary>
        /// <param name="doc">The document to check</param>
        /// <returns>The result of the integrity check</returns>
		//public static bool CheckDocIntegrity(TRN_PDFDoc doc)
		//{
		//	bool result = false;
		//	PDFNetException.REX(PDFNetPINVOKE.TRN_PDFNetInternalToolsCheckDocIntegrity(doc, ref result));
		//	return result;
		//}
	};
}
