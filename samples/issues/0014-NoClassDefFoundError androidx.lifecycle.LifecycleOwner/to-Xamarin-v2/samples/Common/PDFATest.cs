// Generated code. Do not modify!
//
// Copyright (c) 2001-2019 by PDFTron Systems Inc. All Rights Reserved.
//

using System;
using pdftron;
using pdftron.SDF;
using pdftron.PDF;
using pdftron.PDF.PDFA;

//-----------------------------------------------------------------------------------
// The sample illustrates how to use PDF/A related API-s.
//-----------------------------------------------------------------------------------
using NUnit.Framework;

namespace MiscellaneousSamples
{
	[TestFixture]
	public class PDFATest
	{
		
		// Relative path to the folder containing test files.
		const string input_path =  "TestFiles/";

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[Test]
		public static void Sample()
		{
			PDFNet.SetColorManagement(PDFNet.CMSType.e_lcms);  // Required for PDFA validation.

			//-----------------------------------------------------------
			// Example 1: PDF/A Validation
			//-----------------------------------------------------------
			try
			{
				string filename = "newsletter.pdf";
				using (PDFACompliance pdf_a = new PDFACompliance(false, Utils.GetAssetTempFile(input_path+filename), null, PDFACompliance.Conformance.e_Level1B, null, 10, false))
				{
					PrintResults(pdf_a, filename);
				}
			}
			catch (pdftron.Common.PDFNetException e)
			{
				Console.WriteLine(e.Message);
				Assert.True(false);
			}

			//-----------------------------------------------------------
			// Example 2: PDF/A Conversion
			//-----------------------------------------------------------
			try
			{
				string filename = "fish.pdf";
				using (PDFACompliance pdf_a = new PDFACompliance(true, Utils.GetAssetTempFile(input_path+filename), null, PDFACompliance.Conformance.e_Level1B, null, 10, false))
				{
					filename = "pdfa.pdf";
					pdf_a.SaveAs(Utils.CreateExternalFile(filename), true);
				}

				// Re-validate the document after the conversion...
                filename = "pdfa.pdf";
				using (PDFACompliance pdf_a = new PDFACompliance(false, Utils.CreateExternalFile(filename), null, PDFACompliance.Conformance.e_Level1B, null, 10, false))
				{
					PrintResults(pdf_a, filename);				
				}
			}
			catch (pdftron.Common.PDFNetException e)
			{
				Console.WriteLine(e.Message);
				Assert.True(false);
			}

            Console.WriteLine("PDFACompliance test completed.");

        }

		static void PrintResults(PDFACompliance pdf_a, String filename) 
		{
			int err_cnt = pdf_a.GetErrorCount();
			if (err_cnt == 0) 
			{
				Console.WriteLine("{0}: OK.", filename);
			}
			else 
			{
				Console.WriteLine("{0} is NOT a valid PDFA.", filename);
				for (int i=0; i<err_cnt; ++i) 
				{
					PDFACompliance.ErrorCode c = pdf_a.GetError(i);
					Console.WriteLine(" - e_PDFA {0}: {1}.", 
						(int)c, PDFACompliance.GetPDFAErrorMessage(c));

					if (true) 
					{
						int num_refs = pdf_a.GetRefObjCount(c);
						if (num_refs > 0)  
						{
							Console.Write("   Objects: ");
							for (int j=0; j<num_refs; ) 
							{
								Console.Write("{0}", pdf_a.GetRefObj(c, j));
								if (++j!=num_refs) Console.Write(", ");
							}
							Console.WriteLine();
						}
					}
				}
				Console.WriteLine();
			}
		}
	}
}
