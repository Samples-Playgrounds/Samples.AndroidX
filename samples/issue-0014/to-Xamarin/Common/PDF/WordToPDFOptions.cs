using System;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_DocumentConversion = System.IntPtr;
using TRN_PDFDoc = System.IntPtr;

namespace pdftron.PDF
{
	public class WordToPDFOptions : ConversionOptions
	{
		public WordToPDFOptions()
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

		public WordToPDFOptions SetLayoutResourcesPluginPath(string value)
		{
			mDict.PutText("LayoutResourcesPluginPath", value);
			return this;
		}

		public WordToPDFOptions SetResourceDocPath(string value)
		{
			mDict.PutText("ResourceDocPath", value);
			return this;
		}

		public WordToPDFOptions SetSmartSubstitutionPluginPath(string value)
		{
			mDict.PutText("SmartSubstitutionPluginPath", value);
			return this;
		}
	}
}
