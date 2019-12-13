// Generated code. Do not modify!
//
// Copyright (c) 2001-2019 by PDFTron Systems Inc. All Rights Reserved.
//

using System;
using pdftron;
using pdftron.Common;
using pdftron.Filters;
using pdftron.SDF;
using pdftron.PDF;
using pdftron.FDF;

using NUnit.Framework;

namespace MiscellaneousSamples
{
	/// <summary>
	/// PDFNet includes full support for FDF (Forms Data Format) and for merging/extracting
	/// forms data (FDF) with/from PDF. This sample illustrates basic FDF merge/extract functionality 
	/// available in PDFNet.
	/// </summary>
	[TestFixture]
	public class FDFTest
	{
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[Test]
		public static void Sample()
		{

			// Relative path to the folder containing test files.
			const string input_path =  "TestFiles/";

			// Example 1)
			// Iterate over all form fields in the document. Display all field names.
			try  
			{
				using (PDFDoc doc = new PDFDoc(Utils.GetAssetTempFile(input_path + "form1.pdf")))
				{
					doc.InitSecurityHandler();
					
					FieldIterator itr;
					for(itr=doc.GetFieldIterator(); itr.HasNext(); itr.Next())
					{
						Console.WriteLine("Field name: {0:s}", itr.Current().GetName());
						Console.WriteLine("Field partial name: {0:s}", itr.Current().GetPartialName());

						Console.Write("Field type: ");
						Field.Type type = itr.Current().GetType();
						switch(type)
						{
							case Field.Type.e_button: 
								Console.WriteLine("Button"); break;
							case Field.Type.e_text: 
								Console.WriteLine("Text"); break;
							case Field.Type.e_choice: 
								Console.WriteLine("Choice"); break;
							case Field.Type.e_signature: 
								Console.WriteLine("Signature"); break;
						}

						Console.WriteLine("------------------------------");
					}

					Console.WriteLine("Done.");
				}
			}
			catch (PDFNetException e)
			{
				Console.WriteLine(e.Message);
				Assert.True(false);
			}

			// Example 2) Import XFDF into FDF, then merge data from FDF into PDF
			try  
			{
				// XFDF to FDF
				// form fields
				Console.WriteLine("Import form field data from XFDF to FDF.");
				
				FDFDoc fdf_doc1 = FDFDoc.CreateFromXFDF(Utils.GetAssetTempFile(input_path + "form1_data.xfdf"));
				fdf_doc1.Save(Utils.CreateExternalFile("form1_data.fdf"));
				
				// annotations
				Console.WriteLine("Import annotations from XFDF to FDF.");
				
				FDFDoc fdf_doc2 = FDFDoc.CreateFromXFDF(Utils.GetAssetTempFile(input_path + "form1_annots.xfdf"));
				fdf_doc2.Save(Utils.CreateExternalFile("form1_annots.fdf"));
				
				// FDF to PDF
				// form fields
				Console.WriteLine("Merge form field data from FDF.");
				
				using (PDFDoc doc = new PDFDoc(Utils.GetAssetTempFile(input_path + "form1.pdf")))
				{
					doc.InitSecurityHandler();
					doc.FDFMerge(fdf_doc1);
					
					// To use PDFNet form field appearance generation instead of relying on 
					// Acrobat, uncomment the following two lines:
					// doc.RefreshFieldAppearances();
					// doc.GetAcroForm().Put("NeedAppearances", Obj.CreateBool(false));
					
					doc.Save(Utils.CreateExternalFile("form1_filled.pdf"), SDFDoc.SaveOptions.e_linearized);
					
					// annotations
					Console.WriteLine("Merge annotations from FDF.");
					
					doc.FDFMerge(fdf_doc2);
					doc.Save(Utils.CreateExternalFile("form1_filled_with_annots.pdf"), SDFDoc.SaveOptions.e_linearized);

					Console.WriteLine("Done.");
				}
			}
			catch (PDFNetException e)
			{
				Console.WriteLine(e.Message);
				Assert.True(false);
			}

			// Example 3) Extract data from PDF to FDF, then export FDF as XFDF
			try  
			{
				// PDF to FDF
				using (PDFDoc in_doc = new PDFDoc(Utils.CreateExternalFile("form1_filled_with_annots.pdf")))
				{
					in_doc.InitSecurityHandler();
					
					// form fields only
					Console.WriteLine("Extract form fields data to FDF.");
					
					FDFDoc doc_fields = in_doc.FDFExtract(PDFDoc.ExtractFlag.e_forms_only);
					doc_fields.SetPdfFileName("../form1_filled_with_annots.pdf");
					doc_fields.Save(Utils.CreateExternalFile("form1_filled_data.fdf"));
					
					// annotations only
					Console.WriteLine("Extract annotations to FDF.");
					
					FDFDoc doc_annots = in_doc.FDFExtract(PDFDoc.ExtractFlag.e_annots_only);
					doc_annots.SetPdfFileName("../form1_filled_with_annots.pdf");
					doc_annots.Save(Utils.CreateExternalFile("form1_filled_annot.fdf"));
					
					// both form fields and annotations
					Console.WriteLine("Extract both form fields and annotations to FDF.");
					
					FDFDoc doc_both = in_doc.FDFExtract(PDFDoc.ExtractFlag.e_both);
					doc_both.SetPdfFileName("../form1_filled_with_annots.pdf");
					doc_both.Save(Utils.CreateExternalFile("form1_filled_both.fdf"));
					
					// FDF to XFDF
					// form fields
					Console.WriteLine("Export form field data from FDF to XFDF.");
					
					doc_fields.SaveAsXFDF(Utils.CreateExternalFile("form1_filled_data.xfdf"));
					
					// annotations
					Console.WriteLine("Export annotations from FDF to XFDF.");
					
					doc_annots.SaveAsXFDF(Utils.CreateExternalFile("form1_filled_annot.xfdf"));
					
					// both form fields and annotations
					Console.WriteLine("Export both form fields and annotations from FDF to XFDF.");
					
					doc_both.SaveAsXFDF(Utils.CreateExternalFile("form1_filled_both.xfdf"));
					
					Console.WriteLine("Done.");
				}
			}
			catch (PDFNetException e)
			{
				Console.WriteLine(e.Message);
				Assert.True(false);
			}

            // Example 4) Merge/Extract XFDF into/from PDF
            try
            {
                // Merge XFDF from string
                PDFDoc in_doc = new PDFDoc(Utils.GetAssetTempFile(input_path + "numbered.pdf"));
				{
					in_doc.InitSecurityHandler();

					Console.WriteLine("Merge XFDF string into PDF.");

					string str = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?><xfdf xmlns=\"http://ns.adobe.com/xfdf\" xml:space=\"preserve\"><square subject=\"Rectangle\" page=\"0\" name=\"cf4d2e58-e9c5-2a58-5b4d-9b4b1a330e45\" title=\"user\" creationdate=\"D:20120827112326-07'00'\" date=\"D:20120827112326-07'00'\" rect=\"227.7814207650273,597.6174863387978,437.07103825136608,705.0491803278688\" color=\"#000000\" interior-color=\"#FFFF00\" flags=\"print\" width=\"1\"><popup flags=\"print,nozoom,norotate\" open=\"no\" page=\"0\" rect=\"0,792,0,792\" /></square></xfdf>";

					using (FDFDoc fdoc = FDFDoc.CreateFromXFDF(str))
					{
						in_doc.FDFMerge(fdoc);
						in_doc.Save(Utils.CreateExternalFile("numbered_modified.pdf"), SDFDoc.SaveOptions.e_linearized);
						Console.WriteLine("Merge complete.");
					}

					// Extract XFDF as string
					Console.WriteLine("Extract XFDF as a string.");
					FDFDoc fdoc_new = in_doc.FDFExtract(PDFDoc.ExtractFlag.e_both);
					string XFDF_str = fdoc_new.SaveAsXFDF();
					Console.WriteLine("Extracted XFDF: ");
					Console.WriteLine(XFDF_str);
					Console.WriteLine("Extract complete.");
				}
            }
            catch (PDFNetException e)
            {
                Console.WriteLine(e.Message);
                Assert.True(false);
            }

			// Example 5) Read FDF files directly
			try  
			{
				FDFDoc doc = new FDFDoc(Utils.CreateExternalFile("form1_filled_data.fdf"));
				FDFFieldIterator itr = doc.GetFieldIterator();
				for(; itr.HasNext(); itr.Next()) 
				{
					Console.WriteLine("Field name: {0:s}", itr.Current().GetName());
					Console.WriteLine("Field partial name: {0:s}", itr.Current().GetPartialName());
					Console.WriteLine("------------------------------");
				}

				Console.WriteLine("Done.");
			}
			catch (PDFNetException e)
			{
				Console.WriteLine(e.Message);
				Assert.True(false);
			}

			// Example 6) Direct generation of FDF.
			try  
			{
				FDFDoc doc = new FDFDoc();

				// Create new fields (i.e. key/value pairs).
				doc.FieldCreate("Company", (int)Field.Type.e_text, "PDFTron Systems");
				doc.FieldCreate("First Name", (int)Field.Type.e_text, "John");
				doc.FieldCreate("Last Name", (int)Field.Type.e_text, "Doe");
				// ...		

				// doc.SetPdfFileName("mydoc.pdf");
				doc.Save(Utils.CreateExternalFile("sample_output.fdf"));
				Console.WriteLine("Done. Results saved in sample_output.fdf");
			}
			catch (PDFNetException e)
			{
				Console.WriteLine(e.Message);
				Assert.True(false);
			}

		}
	}
}
