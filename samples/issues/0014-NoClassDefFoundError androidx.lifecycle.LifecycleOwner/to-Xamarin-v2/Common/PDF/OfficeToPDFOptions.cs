using System;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_DocumentConversion = System.IntPtr;
using TRN_PDFDoc = System.IntPtr;

namespace pdftron.PDF
{
	public class OfficeToPDFOptions : ConversionOptions
	{
		public OfficeToPDFOptions()
			:base()
		{
		}

		public string GetLayoutResourcesPluginPath()
		{
			Obj found = mDict.FindObj("LayoutResourcesPluginPath");
			if (!found.IsNull())
			{
				return found.GetAsPDFText();
			}
			return "";
		}

		public string GetResourceDocPath()
		{
			Obj found = mDict.FindObj("ResourceDocPath");
			if (!found.IsNull())
			{
				return found.GetAsPDFText();
			}
			return "";
		}

		public string GetSmartSubstitutionPluginPath()
		{
			Obj found = mDict.FindObj("SmartSubstitutionPluginPath");
			if (!found.IsNull())
			{
				return found.GetAsPDFText();
			}
			return "";
		}
		
		public double GetExcelDefaultCellBorderWidth()
		{
			Obj found = mDict.FindObj("ExcelDefaultCellBorderWidth");
			if (!found.IsNull())
			{
				return found.GetNumber();
			}
			return 0;
		}
		
		public OfficeToPDFOptions SetExcelDefaultCellBorderWidth(double width)
		{
			mDict.PutNumber("ExcelDefaultCellBorderWidth", width);
			return this;
		}

		public OfficeToPDFOptions SetLayoutResourcesPluginPath(string value)
		{
			mDict.PutText("LayoutResourcesPluginPath", value);
			return this;
		}

		public OfficeToPDFOptions SetResourceDocPath(string value)
		{
			mDict.PutText("ResourceDocPath", value);
			return this;
		}

		public OfficeToPDFOptions SetSmartSubstitutionPluginPath(string value)
		{
			mDict.PutText("SmartSubstitutionPluginPath", value);
			return this;
		}
	}
}
