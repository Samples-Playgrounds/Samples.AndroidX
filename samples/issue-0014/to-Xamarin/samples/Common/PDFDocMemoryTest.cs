// Generated code. Do not modify!
//
// Copyright (c) 2001-2019 by PDFTron Systems Inc. All Rights Reserved.
//

using System;
using System.IO;
using pdftron;
using pdftron.Common;
using pdftron.Filters;
using pdftron.SDF;
using pdftron.PDF;

// The following sample illustrates how to read/write a PDF document from/to 
// a memory buffer.  This is useful for applications that work with dynamic PDF
// documents that don't need to be saved/read from a disk.
using NUnit.Framework;

namespace MiscellaneousSamples
{
	[TestFixture]
	public class PDFDocMemoryTest
	{
		
		[Test]
		public static void Sample()
		{

			// Relative path to the folder containing test files.
			const string input_path =  "TestFiles/";

			try  
			{
				// Read a PDF document from a stream or pass-in a memory buffer...
				FileStream istm = new FileStream(Utils.GetAssetTempFile(input_path + "tiger.pdf"), FileMode.Open, FileAccess.Read);
				using (PDFDoc doc = new PDFDoc(istm))
				using (ElementWriter writer = new ElementWriter())
				using (ElementReader reader = new ElementReader())
				{
					doc.InitSecurityHandler();

					int num_pages = doc.GetPageCount();

					Element element;

					// Perform some document editing ...
					// Here we simply copy all elements from one page to another.
					for(int i = 1; i <= num_pages; ++i)
					{
						Page pg = doc.GetPage(2 * i - 1);

						reader.Begin(pg);
						Page new_page = doc.PageCreate(pg.GetMediaBox());
						doc.PageInsert(doc.GetPageIterator(2*i), new_page);

						writer.Begin(new_page);
						while ((element = reader.Next()) != null) 	// Read page contents
						{
							writer.WriteElement(element);
						}

						writer.End();
						reader.End();
					}

					doc.Save(Utils.CreateExternalFile("doc_memory_edit.pdf"), SDFDoc.SaveOptions.e_remove_unused);

					// Save the document to a stream or a memory buffer...
					using (FileStream ostm = new FileStream(Utils.CreateExternalFile("doc_memory_edit.txt"), FileMode.Create, FileAccess.Write)) {
						doc.Save(ostm, SDFDoc.SaveOptions.e_remove_unused);
					}

					// Read some data from the file stored in memory
					reader.Begin(doc.GetPage(1));
					while ((element = reader.Next()) != null) {
						if (element.GetType() == Element.Type.e_path)
							Console.Write("Path, ");
					}
					reader.End();

                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("Done. Result saved in doc_memory_edit.pdf and doc_memory_edit.txt ...");
				}
			}
			catch (PDFNetException e)
			{
				Console.WriteLine(e.Message);
				Assert.True(false);
			}
		}
	}
}
