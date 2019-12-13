// Generated code. Do not modify!
//
// Copyright (c) 2001-2019 by PDFTron Systems Inc. All Rights Reserved.
//

using System;
using System.Drawing;

using pdftron;
using pdftron.Common;
using pdftron.PDF;
using pdftron.SDF;
using pdftron.Filters;

using NUnit.Framework;

namespace MiscellaneousSamples
{
	[TestFixture]
	public class ImageExtractTest
	{
		/// <summary>
		///-----------------------------------------------------------------------------------
		/// This sample illustrates one approach to PDF image extraction 
		/// using PDFNet.
		/// 
		/// Note: Besides direct image export, you can also convert PDF images 
		/// to GDI+ Bitmap, or extract uncompressed/compressed image data directly 
		/// using element.GetImageData() (e.g. as illustrated in ElementReaderAdv 
		/// sample project).
		///-----------------------------------------------------------------------------------
		/// </summary>


		static int image_counter = 0;

		// Relative path to the folder containing test files.
		const string input_path =  "TestFiles/";

		static void ImageExtract(PDFDoc doc, ElementReader reader) 
		{
			Element element; 
			while ((element = reader.Next()) != null)
			{
				switch (element.GetType()) 
				{
					case Element.Type.e_image:
					case Element.Type.e_inline_image:
					{
						Console.WriteLine("--> Image: {0}", ++image_counter);
						Console.WriteLine("    Width: {0}", element.GetImageWidth());
						Console.WriteLine("    Height: {0}", element.GetImageHeight());
						Console.WriteLine("    BPC: {0}", element.GetBitsPerComponent());

						Matrix2D ctm = element.GetCTM();
						double x2=1, y2=1, y1=ctm.m_v;
						ctm.Mult(ref x2, ref y2);
                        // Write the coords to 3 decimal places.
						Console.WriteLine("    Coords: x1={0:N2}, y1={1:N2}, x2={2:N2}, y2={3:N2}", ctm.m_h, ctm.m_v, x2, y2);
						pdftron.PDF.Image image = null;
						if (element.GetType() == Element.Type.e_image) 
						{
							image = new pdftron.PDF.Image(element.GetXObject());
							// Convert PDF bitmap to GDI+ Bitmap...
							//Bitmap bmp = element.GetBitmap();
							//bmp.Save(fname, ImageFormat.Png);
							//bmp.Dispose();

							// Instead of converting PDF images to a Bitmap, you can also extract 
							// uncompressed/compressed image data directly using element.GetImageData() 
							// as illustrated in ElementReaderAdv sample project.
						}
						else // inline image
						{
							if(y1 > y2)
							{
								byte[] flipped_image = FlipImage(element);
								image = pdftron.PDF.Image.Create(doc, flipped_image, element.GetImageWidth(), element.GetImageHeight(), 8, ColorSpace.CreateDeviceRGB());
							}
							else
							{
								Image2RGB image2rgb = new Image2RGB(element);
								FilterReader image_reader = new FilterReader(image2rgb);
								image = pdftron.PDF.Image.Create(doc, image_reader, element.GetImageWidth(), element.GetImageHeight(), 8, ColorSpace.CreateDeviceRGB());
							}
						}
						string fname = Utils.CreateExternalFile("image_extract1_") + image_counter.ToString();
						image.Export(fname);  // or ExporAsPng() or ExporAsTiff() ...
						break;
					}
					case Element.Type.e_form: // Process form XObjects
					{
						reader.FormBegin(); 
						ImageExtract(doc, reader);
						reader.End(); 
						break; 
					}
				}
			}
		}

		static byte[] FlipImage(Element element)
		{
			Image2RGB image2rgb = new Image2RGB(element);
			int width = element.GetImageWidth();
			int height = element.GetImageHeight();
			int out_data_sz = width * height * 3;
			int stride = width * 3;
			FilterReader reader = new FilterReader(image2rgb);
			byte[] image_data = new byte[out_data_sz];
			byte[] flipped_data = new byte[out_data_sz];
			reader.Read(image_data);
			for(int row = 0; row < height; ++row)
			{
				Buffer.BlockCopy(image_data, row * stride, flipped_data, out_data_sz - (stride * (row + 1)), stride);
			}
			return flipped_data;
		}

		[Test]
		public static void Sample()
		{
			
			// Example 1: 
			// Extract images by traversing the display list for 
			// every page. With this approach it is possible to obtain 
			// image positioning information and DPI.
			try	
			{
				using (PDFDoc doc = new PDFDoc(Utils.GetAssetTempFile(input_path + "newsletter.pdf")))
				using (ElementReader reader = new ElementReader())
				{
					doc.InitSecurityHandler();

					PageIterator itr;
					for (itr=doc.GetPageIterator(); itr.HasNext(); itr.Next())	
					{				
						reader.Begin(itr.Current());
						ImageExtract(doc, reader);
						reader.End();
					}

					Console.WriteLine("Done.");
				}
			}
			catch (PDFNetException e)
			{
				Console.WriteLine(e.Message);
				Assert.True(false);
			}

			Console.WriteLine("----------------------------------------------------------------");

			// Example 2: 
			// Extract images by scanning the low-level document.
			try	
			{
				using (PDFDoc doc = new PDFDoc(Utils.GetAssetTempFile(input_path + "newsletter.pdf")))
				{
					doc.InitSecurityHandler();
					image_counter = 0;

					SDFDoc cos_doc = doc.GetSDFDoc();
					int num_objs = cos_doc.XRefSize();
					for (int i=1; i<num_objs; ++i)
					{
						Obj obj = cos_doc.GetObj(i);
						if (obj!=null && !obj.IsFree()&& obj.IsStream()) 
						{
							// Process only images
							DictIterator itr = obj.Find("Subtype");
							if (!itr.HasNext() || itr.Value().GetName() != "Image") 
								continue; 

							itr = obj.Find("Type");
							if (!itr.HasNext() || itr.Value().GetName() != "XObject") 
								continue;

							pdftron.PDF.Image image = new pdftron.PDF.Image(obj);

							Console.WriteLine("--> Image: {0}", ++image_counter);
							Console.WriteLine("    Width: {0}", image.GetImageWidth());
							Console.WriteLine("    Height: {0}", image.GetImageHeight());
							Console.WriteLine("    BPC: {0}", image.GetBitsPerComponent());

							string fname = Utils.CreateExternalFile("image_extract2_") + image_counter.ToString();
							image.Export(fname);  // or ExporAsPng() or ExporAsTiff() ...

							// Convert PDF bitmap to GDI+ Bitmap...
							//Bitmap bmp = image.GetBitmap();
							//bmp.Save(fname, ImageFormat.Png);
							//bmp.Dispose();

							// Instead of converting PDF images to a Bitmap, you can also extract 
							// uncompressed/compressed image data directly using element.GetImageData() 
							// as illustrated in ElementReaderAdv sample project.
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
