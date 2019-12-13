using System;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;


using TRN_PDFDoc = System.IntPtr;

namespace pdftron.PDF
{
	/// <summary>
	/// static interface to PDFTron SDKs CAD functionality
	/// </summary>
	public static class CADModule
	{


		/// <summary>
        /// Find out whether the CAD module is available (and licensed).
        /// </summary>
        /// <returns>returns true if CAD operations can be performed</returns>
		public static bool IsModuleAvailable()
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_CADModuleIsModuleAvailable(ref result));
			return result;
		}
	}
}
