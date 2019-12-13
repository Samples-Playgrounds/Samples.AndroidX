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

using NUnit.Framework;

namespace MiscellaneousSamples
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class RectTest
	{
		
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[Test]
		public static void Sample()
		{
			// Relative path to the folder containing test files.
			const string input_path =  "TestFiles/";

			Console.WriteLine("_______________________________________________");
			Console.WriteLine("Opening the input pdf...");

			try // Test  - Adjust the position of content within the page.
			{
				using (PDFDoc input_doc = new PDFDoc(Utils.GetAssetTempFile(input_path + "tiger.pdf")))
				{
					input_doc.InitSecurityHandler();

					Page pg = input_doc.GetPage(1);
					Rect media_box = pg.GetMediaBox();

					media_box.x1 -= 200;	// translate the page 200 units (1 uint = 1/72 inch)
					media_box.x2 -= 200;

					media_box.Update();	

					input_doc.Save(Utils.CreateExternalFile("tiger_shift.pdf"), 0);
				}

				Console.WriteLine("Done. Result saved in tiger_shift...");
			}
			catch (PDFNetException e)
			{
				Console.WriteLine(e.Message);
				Assert.True(false);
			}

		}
	}
}
