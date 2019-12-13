// Generated code. Do not modify!
//
// Copyright (c) 2001-2019 by PDFTron Systems Inc. All Rights Reserved.
//

using System;
using System.Drawing;

using pdftron;
using pdftron.Common;
using pdftron.Filters;
using pdftron.SDF;
using pdftron.PDF;

using NUnit.Framework;

namespace MiscellaneousSamples
{
	/// <summary>
    ///---------------------------------------------------------------------------------------
    /// The following sample illustrates how to use the PDF::Convert utility class to convert 
    /// .docx files to PDF
    ///
    /// This conversion is performed entirely within the PDFNet and has *no* external or
    /// system dependencies dependencies
    ///
    /// Please contact us if you have any questions.	
    ///---------------------------------------------------------------------------------------
	/// </summary>



	[TestFixture]
	public class OfficeToPDFTest
	{

		const string input_path =  "TestFiles/";

		static void SimpleConvert(String input_filename, String output_filename)
		{
            // Start with a PDFDoc (the conversion destination)
            using (PDFDoc pdfdoc = new PDFDoc())
            {
                // perform the conversion with no optional parameters
                pdftron.PDF.Convert.OfficeToPDF(pdfdoc, Utils.GetAssetTempFile(input_path + input_filename), null);

                // save the result
                pdfdoc.Save(Utils.CreateExternalFile(output_filename), SDFDoc.SaveOptions.e_linearized);

                // And we're done!
                Console.WriteLine("Saved " + output_filename);
            }   
		}

		static void FlexibleConvert(String input_filename, String output_filename)
		{
			// Start with a PDFDoc (the conversion destination)
            using (PDFDoc pdfdoc = new PDFDoc())
            {
                OfficeToPDFOptions options = new OfficeToPDFOptions();
                // create a conversion object -- this sets things up but does not yet
                // perform any conversion logic.
                // in a multithreaded environment, this object can be used to monitor
                // the conversion progress and potentially cancel it as well
                DocumentConversion conversion = pdftron.PDF.Convert.StreamingPDFConversion(
                    pdfdoc, Utils.GetAssetTempFile(input_path + input_filename), options);

                // actually perform the conversion
                // this particular method will not throw on conversion failure, but will
                // return an error status instead
                if (conversion.TryConvert() == DocumentConversionResult.e_document_conversion_success)
                {
                    int num_warnings = conversion.GetNumWarnings();

                    // print information about the conversion 
                    for (int i = 0; i < num_warnings; ++i)
                    {
                        Console.WriteLine("Warning: " + conversion.GetWarningString(i));
                    }

                    // save the result
                    pdfdoc.Save(Utils.CreateExternalFile(output_filename), SDFDoc.SaveOptions.e_linearized);
                    // done
                    Console.WriteLine("Saved " + output_filename);
                }
                else
                {
                    Console.WriteLine("Encountered an error during conversion: " + conversion.GetErrorString());
                }
            }
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[Test]
		public static void Sample()
		{

			try
			{
				// first the one-line conversion method
				Utils.GetAssetTempFile(input_path + "pdftron_smart_substitution.plugin");
				var res = Utils.GetAssetTempFile(input_path + "pdftron_layout_resources.plugin");
				res = res.Replace("pdftron_layout_resources.plugin", "");
				PDFNet.SetResourcesPath(res);
				SimpleConvert("simple-word_2007.docx", "simple-word_2007.pdf");

                // then the more flexible line-by-line conversion API
                FlexibleConvert("the_rime_of_the_ancient_mariner.docx", "the_rime_of_the_ancient_mariner.pdf");
			}
			catch (pdftron.Common.PDFNetException e)
			{
				Console.WriteLine(e.Message);
				Assert.True(false);
			}
			catch (Exception e)
			{
				Console.WriteLine("Unrecognized Exception: " + e.Message );
				Assert.True(false);
			}

            Console.WriteLine("Done.");
		}
	}
}
