// Generated code. Do not modify!
//---------------------------------------------------------------------------------------
// Copyright (c) 2001-2019 by PDFTron Systems Inc. All Rights Reserved.
// Consult legal.txt regarding legal and license information.
//---------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Collections;

using pdftron;
using pdftron.Common;
using pdftron.Filters;
using pdftron.SDF;
using pdftron.PDF;

using NUnit.Framework;

namespace MiscellaneousSamples
{
	// PDF Redactor is a separately licensable Add-on that offers options to remove 
	// (not just covering or obscuring) content within a region of PDF. 
	// With printed pages, redaction involves blacking-out or cutting-out areas of 
	// the printed page. With electronic documents that use formats such as PDF, 
	// redaction typically involves removing sensitive content within documents for 
	// safe distribution to courts, patent and government institutions, the media, 
	// customers, vendors or any other audience with restricted access to the content. 
	//
	// The redaction process in PDFNet consists of two steps:
	// 
	//  a) Content identification: A user applies redact annotations that specify the 
	// pieces or regions of content that should be removed. The content for redaction 
	// can be identified either interactively (e.g. using 'pdftron.PDF.PDFViewCtrl' 
	// as shown in PDFView sample) or programmatically (e.g. using 'pdftron.PDF.TextSearch'
	// or 'pdftron.PDF.TextExtractor'). Up until the next step is performed, the user 
	// can see, move and redefine these annotations.
	//  b) Content removal: Using 'pdftron.PDF.Redactor.Redact()' the user instructs 
	// PDFNet to apply the redact regions, after which the content in the area specified 
	// by the redact annotations is removed. The redaction function includes number of 
	// options to control the style of the redaction overlay (including color, text, 
	// font, border, transparency, etc.).
	// 
	// PDFTron Redactor makes sure that if a portion of an image, text, or vector graphics 
	// is contained in a redaction region, that portion of the image or path data is 
	// destroyed and is not simply hidden with clipping or image masks. PDFNet API can also 
	// be used to review and remove metadata and other content that can exist in a PDF 
	// document, including XML Forms Architecture (XFA) content and Extensible Metadata 
	// Platform (XMP) content.
	[TestFixture]
	public class PDFRedactTest
	{
		
		static void Redact(string input, string output, ArrayList rarr, Redactor.Appearance app)
		{
			using (PDFDoc doc = new PDFDoc(input))
			{
				doc.InitSecurityHandler();
                Redactor.Redact(doc, rarr, app, false, true);
				doc.Save(output, SDFDoc.SaveOptions.e_linearized);
			}
		}

		/// <summary>
		/// The following sample illustrates how to redact a PDF document using 'pdftron.PDF.Redactor'.
		/// </summary>
		[Test]
		public static void Sample()
		{

			const string input_path =  "TestFiles/";
			try
			{
				ArrayList rarr = new ArrayList();
				rarr.Add(new Redactor.Redaction(1, new Rect(100, 100, 550, 600), false, "Top Secret"));
                rarr.Add(new Redactor.Redaction(2, new Rect(30, 30, 450, 450), true, "Negative Redaction"));
                rarr.Add(new Redactor.Redaction(2, new Rect(0, 0, 100, 100), false, "Positive"));
                rarr.Add(new Redactor.Redaction(2, new Rect(100, 100, 200, 200), false, "Positive"));
                rarr.Add(new Redactor.Redaction(2, new Rect(300, 300, 400, 400), false, ""));
                rarr.Add(new Redactor.Redaction(2, new Rect(500, 500, 600, 600), false, ""));
                rarr.Add(new Redactor.Redaction(3, new Rect(0, 0, 700, 20), false, ""));

                Redactor.Appearance app = new Redactor.Appearance();
                app.RedactionOverlay = true;
                app.Border = false;
                app.ShowRedactedContentRegions = true;

                Redact(Utils.GetAssetTempFile(input_path + "newsletter.pdf"), Utils.CreateExternalFile("redacted.pdf"), rarr, app);

                Console.WriteLine("Done...");
			}
			catch (PDFNetException e)
			{
				Console.WriteLine(e.Message);
				Assert.True(false);
			}
		}
	}
}
