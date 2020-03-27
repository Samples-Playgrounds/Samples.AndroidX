// Generated code. Do not modify!
﻿//
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
	/// This example illustrates how to create Unicode text and how to embed composite fonts.
	/// </summary>
	[TestFixture]
	public class UnicodeWriteTest
	{
		
		// Note: This demo assumes that 'arialuni.ttf' is present in '/Samples/TestFiles' 
		// directory. Arial Unicode MS is about 24MB in size and it comes together with Windows and 
		// MS Office.
		// 
		// For more information about Arial Unicode MS, please consult the following Microsoft Knowledge 
		// Base Article: WD2002: General Information About the Arial Unicode MS Font
		//  http://support.microsoft.com/support/kb/articles/q287/2/47.asp
		//
		// For more information consult: 
		//    http://office.microsoft.com/search/results.aspx?Scope=DC&Query=font&CTT=6&Origin=EC010331121033
		//    http://www.microsoft.com/downloads/details.aspx?FamilyID=1F0303AE-F055-41DA-A086-A65F22CB5593
		// 
		// In case you don't have access to Arial Unicode MS you can use cyberbit.ttf 
		// (ftp://ftp.netscape.com/pub/communicator/extras/fonts/windows/) instead.
		//
		[Test]
		public static void Sample()
		{

			// Relative path to the folder containing test files.
			const string input_path =  "TestFiles/";

			try	
			{
				using (PDFDoc doc = new PDFDoc())
				{
					using (ElementBuilder eb = new ElementBuilder())
					{
						using (ElementWriter writer = new ElementWriter())
						{
							// Start a new page ------------------------------------
							Page page = doc.PageCreate(new Rect(0, 0, 612, 794));

							writer.Begin(page);	// begin writing to this page

							Font fnt;
							try
							{
								// Full font embedding
								fnt = Font.Create(doc, "Arial", "");

								// To embed the font file directly use:
								// fnt = Font.CreateCIDTrueTypeFont(doc, Utils.GetAssetTempFile(input_path + "arialuni.ttf"), true, true);

								// Example of font substitution
								// fnt = Font.CreateCIDTrueTypeFont(doc, Utils.GetAssetTempFile(input_path + "arialuni.ttf"), false);
							}
							catch (PDFNetException e)
							{
								Console.WriteLine(e.Message);
								Assert.True(false);
								Console.WriteLine();
						
								Console.WriteLine("'arialuni.ttf' font file was not found in 'Samples/TestFiles' directory.");
								return;
							}
					
							Element element = eb.CreateTextBegin(fnt, 1);
							element.SetTextMatrix(10, 0, 0, 10, 50, 600);
							element.GetGState().SetLeading(2);		 // Set the spacing between lines
							writer.WriteElement(element);

							// Hello World!!!
							string hello = "Hello World!";
							writer.WriteElement(eb.CreateUnicodeTextRun(hello));
							writer.WriteElement(eb.CreateTextNewLine());

							// Latin
							char[] latin = {   
								'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', '\x45', '\x0046', '\x00C0', 
								'\x00C1', '\x00C2', '\x0143', '\x0144', '\x0145', '\x0152', '1', '2' // etc.
							};
							writer.WriteElement(eb.CreateUnicodeTextRun(new string(latin)));
							writer.WriteElement(eb.CreateTextNewLine());

							// Greek
							char[] greek = {   
								(char)0x039E, (char)0x039F, (char)0x03A0, (char)0x03A1, (char)0x03A3,
								(char)0x03A6, (char)0x03A8, (char)0x03A9  // etc.
							};
							writer.WriteElement(eb.CreateUnicodeTextRun(new string(greek)));
							writer.WriteElement(eb.CreateTextNewLine());

							// Cyrillic
							char[] cyrillic = {   
								(char)0x0409, (char)0x040A, (char)0x040B, (char)0x040C, (char)0x040E, (char)0x040F, (char)0x0410, (char)0x0411,
								(char)0x0412, (char)0x0413, (char)0x0414, (char)0x0415, (char)0x0416, (char)0x0417, (char)0x0418, (char)0x0419 // etc.
							};
							writer.WriteElement(eb.CreateUnicodeTextRun(new string(cyrillic)));
							writer.WriteElement(eb.CreateTextNewLine());

							// Hebrew
							char[] hebrew = {
								(char)0x05D0, (char)0x05D1, (char)0x05D3, (char)0x05D3, (char)0x05D4, (char)0x05D5, (char)0x05D6, (char)0x05D7, (char)0x05D8, 
								(char)0x05D9, (char)0x05DA, (char)0x05DB, (char)0x05DC, (char)0x05DD, (char)0x05DE, (char)0x05DF, (char)0x05E0, (char)0x05E1 // etc. 
							};
							writer.WriteElement(eb.CreateUnicodeTextRun(new string(hebrew)));
							writer.WriteElement(eb.CreateTextNewLine());

							// Arabic
							char[] arabic = {
								(char)0x0624, (char)0x0625, (char)0x0626, (char)0x0627, (char)0x0628, (char)0x0629, (char)0x062A, (char)0x062B, (char)0x062C, 
								(char)0x062D, (char)0x062E, (char)0x062F, (char)0x0630, (char)0x0631, (char)0x0632, (char)0x0633, (char)0x0634, (char)0x0635 // etc. 
							};
							writer.WriteElement(eb.CreateUnicodeTextRun(new string(arabic)));
							writer.WriteElement(eb.CreateTextNewLine());

							// Thai 
							char[] thai = {
								(char)0x0E01, (char)0x0E02, (char)0x0E03, (char)0x0E04, (char)0x0E05, (char)0x0E06, (char)0x0E07, (char)0x0E08, (char)0x0E09, 
								(char)0x0E0A, (char)0x0E0B, (char)0x0E0C, (char)0x0E0D, (char)0x0E0E, (char)0x0E0F, (char)0x0E10, (char)0x0E11, (char)0x0E12 // etc. 
							};
							writer.WriteElement(eb.CreateUnicodeTextRun(new string(thai)));
							writer.WriteElement(eb.CreateTextNewLine());

							// Hiragana - Japanese 
							char[] hiragana = {
								(char)0x3041, (char)0x3042, (char)0x3043, (char)0x3044, (char)0x3045, (char)0x3046, (char)0x3047, (char)0x3048, (char)0x3049, 
								(char)0x304A, (char)0x304B, (char)0x304C, (char)0x304D, (char)0x304E, (char)0x304F, (char)0x3051, (char)0x3051, (char)0x3052 // etc. 
							};
							writer.WriteElement(eb.CreateUnicodeTextRun(new string(hiragana)));
							writer.WriteElement(eb.CreateTextNewLine());

							// CJK Unified Ideographs
							char[] cjk_uni = {
								(char)0x5841, (char)0x5842, (char)0x5843, (char)0x5844, (char)0x5845, (char)0x5846, (char)0x5847, (char)0x5848, (char)0x5849, 
								(char)0x584A, (char)0x584B, (char)0x584C, (char)0x584D, (char)0x584E, (char)0x584F, (char)0x5850, (char)0x5851, (char)0x5852 // etc. 
							};
							writer.WriteElement(eb.CreateUnicodeTextRun(new string(cjk_uni)));
							writer.WriteElement(eb.CreateTextNewLine());

							// Finish the block of text
							writer.WriteElement(eb.CreateTextEnd());		

							writer.End();  // save changes to the current page
							doc.PagePushBack(page);
							doc.Save(Utils.CreateExternalFile("unicodewrite.pdf"), SDFDoc.SaveOptions.e_remove_unused | SDFDoc.SaveOptions.e_hex_strings);
							Console.WriteLine("Done. Result saved in unicodewrite.pdf...");
						}
					}
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
