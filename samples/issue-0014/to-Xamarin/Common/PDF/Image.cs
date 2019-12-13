using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.Filters;
using pdftron.SDF;

using TRN_Image = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_ColorSpace = System.IntPtr;
using TRN_Filter = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> Image class provides common methods for working with PDF images. 
    /// 
    /// </summary>
    /// <remarks>  PDF.Element contains a similar interface used to access image data.
    /// To create the Image object from image PDF.Element, pass the Element's SDF/Cos 
    /// dictionary to Image constructor (i.e. Image image(element->GetXObject()) )</remarks>
    public class Image : IDisposable
    {
        internal TRN_Image mp_image = IntPtr.Zero;
        internal Object m_ref;
        internal Image()
        {
            mp_image = IntPtr.Zero;
        }
        internal Image(TRN_Image imp, Object reference)
        {
            this.mp_image = imp;
            this.m_ref = reference;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (disposing)
            {
                // Dispose managed resources.
            }

            // Clean up native resources
            if (mp_image != IntPtr.Zero)
            {
                mp_image = IntPtr.Zero;
            }
        }

        /// <summary> Create and embed an Image from an external file.
	    /// 
	    /// </summary>
	    /// <param name="doc">A document to which the image should be added. To obtain
	    /// SDF.Doc from PDFDoc use PDFDoc.GetSDFDoc() or Obj.GetDoc().
	    /// </param>
	    /// <param name="filename">The name of the image file. Currently supported formats are
	    /// JPEG, PNG, GIF, TIFF, BMP, EMF, and WMF. Other raster formats can be embedded by
	    /// decompressing image data and using other versions of Image.Create(...) method.
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded image.		
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks> By default the function will either pass-through data preserving the original
	    /// compression or will compress data using Flate compression. It is possible to
        /// fine tune compression or to select a different compression algorithm using
        /// 'encoder_hints' object.</remarks>
        public static Image Create(SDFDoc doc, String filename)
        {
            return Create(doc, filename, null);
        }
	    /// <summary> Create and embed an Image from an external file taking into account specified 
	    /// compression hints. 
	    /// </summary>
	    /// <param name="doc">A document to which the image should be added. To obtain
	    /// SDF.Doc from PDFDoc use PDFDoc.GetSDFDoc() or Obj.GetDoc().
	    /// </param>
	    /// <param name="filename">The name of the image file. Currently supported formats are
	    /// JPEG, PNG, GIF, TIFF, BMP, EMF, and WMF. Other raster formats can be embedded by
	    /// decompressing image data and using other versions of Image.Create(...) method.
	    /// </param>
	    /// <param name="encoder_hints">An optional SDF.Obj containing a hint (or an SDF.Array of 
	    /// hints) that could be used to select a specific compression method and compression 
	    /// parameters. For a concrete example of how to create encoder hints, please take a look 
	    /// at JBIG2Test and AddImage sample projects. The image encoder accepts the following 
	    /// hints: 
	    /// <para>- /JBIG2; SDF.Name("JBIG2"), An SDF.Name Object with value equal to "JBIG2". If the 
	    ///	image is monochrome (i.e. bpc == 1), the encoder will compress the image using JBIG2Decode 
	    ///	filter. </para>
	    ///
	    /// <para>- [/JBIG2 /Threshold 0.6 /SharePages 50] - Compress a monochrome image using lossy JBIG2Decode 
	    ///  compression with the given image threshold and by sharing segments from a specified number 
	    ///  of pages. The threshold is a floating point number in the rage from 0.4 to 0.9. Increasing the threshold 
	    ///  value will increase image quality, but may increase the file size. The default value 
	    ///  for threshold is 0.85. "SharePages" parameter can be used to specify the maximum number of 
	    ///  pages sharing a common 'JBIG2Globals' segment stream. Increasing the value of this parameter 
	    ///  improves compression ratio at the expense of memory usage.</para>
	    ///
	    /// <para>- [/JPEG] - Use JPEG compression with default compression. </para>
	    /// <para>- [/JPEG /Quality 60] - Use JPEG compression with given quality setting. The "Quality" 
	    ///	value is expressed on the 0..100 scale.</para>
	    ///
	    /// <para>- [/JP2] or [/JPEG2000] - Use JPEG2000 compression to compress a RGB or a grayscale image</para>
	    ///
	    /// <para>- [/Flate] - Use Flate compression with maximum compression at the expense of 
	    ///	speed. </para>
	    ///
	    /// <para>- [/Flate /Level 9] - Use Flate compression using specified compression level. 
	    ///	Compression "Level" must be a number between 0 and 9: 1 gives best speed, 
	    ///	9 gives best compression, 0 gives no compression at all (the input data is simply 
	    ///	copied a block at a time). </para>
	    ///
	    /// <para>- /RAW or [/RAW] - The encoder will not use any compression method and the image 
	    ///	will be stored in the raw format.</para>
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded image.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks> By default the function will either pass-through data preserving the original
	    /// compression or will compress data using Flate compression. It is possible to
	    /// fine tune compression or to select a different compression algorithm using
	    /// 'encoder_hints' object.</remarks>
	    /// <remarks>For C++ developers: Current document does not take the ownership of the 
	    /// encoder_hints object. Therefore it is a good programming practice to create 
        /// encoder_hints object on the stack.
        /// </remarks>
        public static Image Create(SDFDoc doc, String filename, Obj encoder_hints)
        {
            UString str = new UString(filename);
            TRN_Image result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateFromFile(doc.mp_doc, str.mp_impl, (encoder_hints != null) ? encoder_hints.mp_obj : IntPtr.Zero, ref result));
            return new Image(result, doc);
        }

#if (__DESKTOP__)

	    /// <summary> Create and embed an Image. Embed the raw image data. 
	    /// 
	    /// </summary>
	    /// <param name="doc">A document to which the image should be added. To obtain
	    /// SDF.Doc from PDFDoc use PDFDoc.GetSDFDoc() or Obj.GetDoc().
	    /// </param>
	    /// <param name="bitmap">The bitmap containing image data. 
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded image.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <exception cref="PDFNetException">  InterruptedException the interrupted exception </exception>
        //public static Image Create(SDFDoc doc, System.Drawing.Bitmap bitmap)
        //{
        //    return Create(doc, bitmap, null);
        //}
	    /// <summary> Create and embed an Image. Embed the raw image data taking into account 
	    /// specified compression hints.
	    /// </summary>
	    /// <param name="doc">A document to which the image should be added. To obtain
	    /// SDF.Doc from PDFDoc use PDFDoc.GetSDFDoc() or Obj.GetDoc().
	    /// </param>
	    /// <param name="bitmap">The bitmap containing image data. 
	    /// </param>
	    /// <param name="encoder_hints">The encoder hints
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded image.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <exception cref="PDFNetException">  InterruptedException the interrupted exception </exception>
        /*public static Image Create(SDFDoc doc, System.Drawing.Bitmap bitmap, Obj encoder_hints)
        {
            Image ret = new Image();

            int width = bitmap.Width;
	        int height = bitmap.Height;
	        int pix_fmt = (int)bitmap.PixelFormat;
	        int pallete_sz = 0;
	        byte[] pallete = new byte[pallete_sz];

            int flags = bitmap.Flags;
            bool has_alpha = ((int)flags & (int)System.Drawing.Imaging.ImageFlags.HasAlpha) != 0;
            bool is_translucent = ((int)flags & (int)System.Drawing.Imaging.ImageFlags.HasTranslucent) != 0;

            if ((is_translucent || has_alpha) && pix_fmt!=(int)System.Drawing.Imaging.PixelFormat.Format1bppIndexed)
            {
                pix_fmt = (int)System.Drawing.Imaging.PixelFormat.Format32bppArgb;
            }

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, width, height);
            switch (pix_fmt)
            {
                case (int)System.Drawing.Imaging.PixelFormat.Format4bppIndexed:
                    return null;
                case (int)System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    System.Drawing.Imaging.ColorPalette pal = bitmap.Palette;
                    pallete_sz = pal.Entries.Length;
                    pallete = new byte[pallete_sz * 3];
                    for (int j = 0; j < pallete_sz; ++j)
                    {
                        System.Drawing.Color entry = pal.Entries[j];
                        pallete[3 * j] = entry.R;
                        pallete[3 * j+1] = entry.G;
                        pallete[3 * j+2] = entry.B;
                    }
                    return null;
                case (int)System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                    return null;
                case (int)System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    System.Drawing.Imaging.BitmapData data = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    byte[] dest = new byte[data.Height * data.Stride];
                    Marshal.Copy(data.Scan0, dest, 0, data.Height * data.Stride);
                    return Create(doc, dest, width, height, data.Stride, ColorSpace.CreateDeviceRGB(), null);
                default:
                    //System.Drawing.Imaging.BitmapData data = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    //ret = Create(doc, data.Scan0, width, height, data.Stride, null, (encoder_hints != null) ? encoder_hints.mp_obj : IntPtr.Zero);
                    return null;
            }

            //TRN_Image result = IntPtr.Zero;
            //PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateFromBitmap(doc.mp_doc, bitmap.get, (encoder_hints != null) ? encoder_hints.mp_obj : IntPtr.Zero, ref result));
            //return new Image(result);
            return null;
        }*/
#elif (__ANDROID__)
		public static Image Create(SDFDoc doc, Android.Graphics.Bitmap bmp)
		{
			int height = bmp.Height;
			int width = bmp.Width;
			if (width*height <= 0)
				return null;
			int[] arr = new int[width*height];
			bmp.GetPixels(arr, 0, width, 0, 0, width, height);

			//int stride = width * 4;

			byte[] tp = new byte[width * height * 3];
			byte[] tpa = new byte[width * height];

			int[] row = arr;

			int need_softmask = 0;
			int y;
			int i = 0;

			for (y=0; y<row.Length; ++y) 
			{
				int curr = row [y];
				byte[] buf = ConvToByteArray (curr);

				tpa [y] = buf [0];
				tp [i] = buf [1];
				tp [i + 1] = buf [2];
				tp [i + 2] = buf [3];

				if(buf[0] != 255) need_softmask=1;

				i += 3;
			}

			Image image = Image.Create (doc, tp, width, height, 8, ColorSpace.CreateDeviceRGB (), Image.InputFilter.e_none);

			if (need_softmask == 1) 
			{
				ObjSet objset = new ObjSet ();
				Obj hint = objset.CreateName("Flate");
				Image alpha = Image.CreateSoftMask (doc, tpa, width, height, 8, hint);
				image.SetSoftMask (alpha);
			}

			return image;
		}

		private static byte[] ConvToByteArray(int intValue)
		{
			byte[] inBytes = BitConverter.GetBytes (intValue);
			if (BitConverter.IsLittleEndian)
				Array.Reverse (inBytes);
			return inBytes;
		}
#endif
        /// <summary> Create and embed an Image. Embed the raw image data taking into account
	    /// specified compression hints.
	    /// 
	    /// By default the function will compress all images using Flate compression.
	    /// It is possible to fine tune compression or to select a different compression
	    /// algorithm using 'encoder_hints' object.
	    /// 
	    /// </summary>
	    /// <param name="doc">- A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">- The stream or buffer containing image data. The image data must
	    /// not be compressed and must follow PDF format for sample representation (please refer
	    /// to section 4.8.2 'Sample Representation' in PDF Reference Manual for details).
	    /// </param>
	    /// <param name="width">- The width of the image, in samples.
	    /// </param>
	    /// <param name="height">- The height of the image, in samples.
	    /// </param>
	    /// <param name="bpc">- The number of bits used to represent each color component.
	    /// </param>
	    /// <param name="color_space">- The color space in which image samples are represented.
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded image.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Image Create(SDFDoc doc, FilterReader image_data, Int32 width, Int32 height, Int32 bpc, ColorSpace color_space)
        {
            return Create(doc, image_data, width, height, bpc, color_space, null);
        }
	    /// <summary> Create and embed an Image. Embed the raw image data taking into account
	    /// specified compression hints.
	    /// 
	    /// By default the function will compress all images using Flate compression.
	    /// It is possible to fine tune compression or to select a different compression
	    /// algorithm using 'encoder_hints' object.
	    /// 
	    /// </summary>
	    /// <param name="doc">- A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">- The stream or buffer containing image data. The image data must
	    /// not be compressed and must follow PDF format for sample representation (please refer
	    /// to section 4.8.2 'Sample Representation' in PDF Reference Manual for details).
	    /// </param>
	    /// <param name="width">- The width of the image, in samples.
	    /// </param>
	    /// <param name="height">- The height of the image, in samples.
	    /// </param>
	    /// <param name="bpc">- The number of bits used to represent each color component.
	    /// </param>
	    /// <param name="color_space">- The color space in which image samples are represented.
	    /// </param>
	    /// <param name="input_format">- Image.InputFilter describing the format of pre-compressed
	    /// image data.
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded image.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  see the above method for details. </remarks>
        public static Image Create(SDFDoc doc, FilterReader image_data, Int32 width, Int32 height, Int32 bpc, ColorSpace color_space, InputFilter input_format)
        {
            TRN_Image result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateDirectFromStream(doc.mp_doc, image_data.mp_imp, width, height, bpc, color_space.mp_cs, input_format, ref result));
            return new Image(result, doc);
        }
	    /// <summary> Create and embed an Image. Embed the raw image data taking into account
	    /// specified compression hints.
	    /// 
	    /// By default the function will compress all images using Flate compression.
	    /// It is possible to fine tune compression or to select a different compression
	    /// algorithm using 'encoder_hints' object.
	    /// 
	    /// </summary>
	    /// <param name="doc">- A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">- The stream or buffer containing image data. The image data must
	    /// not be compressed and must follow PDF format for sample representation (please refer
	    /// to section 4.8.2 'Sample Representation' in PDF Reference Manual for details).
	    /// </param>
	    /// <param name="width">- The width of the image, in samples.
	    /// </param>
	    /// <param name="height">- The height of the image, in samples.
	    /// </param>
	    /// <param name="bpc">- The number of bits used to represent each color component.
	    /// </param>
	    /// <param name="color_space">- The color space in which image samples are represented.
	    /// </param>
	    /// <param name="encoder_hints">The encoder hints
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded image.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Image Create(SDFDoc doc, FilterReader image_data, Int32 width, Int32 height, Int32 bpc, ColorSpace color_space, Obj encoder_hints)
        {
            TRN_Image result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateFromStream(doc.mp_doc, image_data.mp_imp, width, height, bpc, color_space.mp_cs, (encoder_hints == null) ? IntPtr.Zero : encoder_hints.mp_obj, ref result));
            return new Image(result, doc);
        }

	    /// <summary> Directly embed the image that is already compressed using the Image.InputFilter
	    /// format. The function can be used to pass-through pre-compressed image data.
	    /// 
	    /// </summary>
	    /// <param name="doc">A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">The stream or buffer containing compressed image data.
	    /// The compression format must match the input_format parameter.
	    /// </param>
	    /// <param name="width">The width of the image, in samples.
	    /// </param>
	    /// <param name="height">The height of the image, in samples.
	    /// </param>
	    /// <param name="bpc">The number of bits used to represent each color component.
	    /// </param>
	    /// <param name="color_space">The color space in which image samples are specified.
	    /// </param>
	    /// <param name="input_format">Image.InputFilter describing the format of pre-compressed
	    /// image data.
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded image.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Image Create(SDFDoc doc, byte[] image_data, Int32 width, Int32 height, Int32 bpc, ColorSpace color_space, InputFilter input_format)
        {
            TRN_Image result = IntPtr.Zero;
			int size = Marshal.SizeOf(image_data[0]) * image_data.Length;
			IntPtr pnt = Marshal.AllocHGlobal (size);
			try
			{
				Marshal.Copy(image_data, 0, pnt, image_data.Length);
				PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateDirectFromMemory(doc.mp_doc, pnt, new UIntPtr(System.Convert.ToUInt32(image_data.Length)), width, height, bpc, color_space.mp_cs, input_format, ref result));
				return new Image(result, doc);
			}
			finally 
			{
				Marshal.FreeHGlobal (pnt);
			}            
        }
	    /// <summary> Directly embed the image that is already compressed using the Image.InputFilter
	    /// format. The function can be used to pass-through pre-compressed image data.
	    /// 
	    /// </summary>
	    /// <param name="doc">A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">The stream or buffer containing compressed image data.
	    /// The compression format must match the input_format parameter.
	    /// </param>
	    /// <param name="width">The width of the image, in samples.
	    /// </param>
	    /// <param name="height">The height of the image, in samples.
	    /// </param>
	    /// <param name="bpc">The number of bits used to represent each color component.
	    /// </param>
	    /// <param name="color_space">The color space in which image samples are specified.
	    /// </param>
	    /// <param name="encoder_hints">The encoder hints
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded image.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Image Create(SDFDoc doc, byte[] image_data, Int32 width, Int32 height, Int32 bpc, ColorSpace color_space, Obj encoder_hints)
        {
            TRN_Image result = IntPtr.Zero;

			int psize = Marshal.SizeOf(image_data[0]) * image_data.Length;
			IntPtr pnt = Marshal.AllocHGlobal(psize);
			try
			{
				Marshal.Copy(image_data, 0, pnt, image_data.Length);
				PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateFromMemory(doc.mp_doc, pnt, new UIntPtr(System.Convert.ToUInt32(image_data.Length)), width, height, bpc, color_space.mp_cs, (encoder_hints == null) ? IntPtr.Zero : encoder_hints.mp_obj, ref result));
			}
			finally
			{
				// Free the unmanaged memory.
				Marshal.FreeHGlobal(pnt);
			}

			return new Image(result, doc);
        }

	    /// <summary> Directly embed the image that is already compressed using the Image.InputFilter
	    /// format. The function can be used to pass-through pre-compressed image data.
	    /// 
	    /// </summary>
	    /// <param name="doc">A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">The stream or buffer containing compressed image data.
	    /// The compression format must match the input_format parameter.
	    /// </param>
	    /// <param name="width">The width of the image, in samples.
	    /// </param>
	    /// <param name="height">The height of the image, in samples.
	    /// </param>
	    /// <param name="bpc">The number of bits used to represent each color component.
	    /// </param>
	    /// <param name="color_space">The color space in which image samples are specified.
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded image.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Image Create(SDFDoc doc, byte[] image_data, Int32 width, Int32 height, Int32 bpc, ColorSpace color_space)
        {
            return Create(doc, image_data, width, height, bpc, color_space, null);
        }

		/// <summary> Directly embed the image that is already compressed using the Image.InputFilter
	    /// format. The function can be used to pass-through pre-compressed image data.
	    /// 
	    /// </summary>
	    /// <param name="doc">A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">The stream or buffer containing compressed image data.
	    /// The compression format must match the input_format parameter.
	    /// </param>
	    /// <param name="encoder_hints">The encoder hints
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded image.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Image Create(SDFDoc doc, byte[] image_data, Obj encoder_hints)
        {
            TRN_Image result = IntPtr.Zero;
			UIntPtr size = new UIntPtr(System.Convert.ToUInt32(image_data.Length));

			int psize = Marshal.SizeOf(image_data[0]) * image_data.Length;
			IntPtr pnt = Marshal.AllocHGlobal(psize);
			try
			{
				Marshal.Copy(image_data, 0, pnt, image_data.Length);
				PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateFromMemory2(doc.mp_doc, pnt, size, (encoder_hints == null) ? IntPtr.Zero : encoder_hints.mp_obj, ref result));
			}
			finally
			{
				// Free the unmanaged memory.
				Marshal.FreeHGlobal(pnt);
			}

			return new Image(result, doc);
        }
        public static Image Create(SDFDoc doc, byte[] image_data)
        {
            return Create(doc, image_data, null);
        }

        public static Image Create(SDFDoc doc, System.IO.Stream stream, Obj encoder_hints)
        {
        	TRN_Image result = IntPtr.Zero;
            if (stream.CanSeek)
            {
                int stream_size = (int)stream.Length;
                int bytesToRead = stream_size;
                byte[] buf = new byte[stream_size];
                int bytesRead = 0;
                while (bytesToRead > 0)
                {
                    int? n = stream.Read(buf, bytesRead, bytesToRead);
                    if (n == null)
                        break;
                    bytesRead += n.Value;
                    bytesToRead -= n.Value;
                }
                int size = Marshal.SizeOf(buf[0]) * buf.Length;
                IntPtr pnt = Marshal.AllocHGlobal(size);
                try
                {
                    Marshal.Copy(buf, 0, pnt, buf.Length);
					PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateFromMemory2(doc.mp_doc, pnt, new UIntPtr(System.Convert.ToUInt32(stream_size)), (encoder_hints==null) ? IntPtr.Zero : encoder_hints.mp_obj, ref result));
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(pnt);
                }                
            }
            else
            {
                // if we cannot seek, then we need to stream the data
                int bytesRead = 0;
                const int readSize = 0xFFFF; // 64KB
                byte[] readBuf = new byte[readSize];
                List<byte> dyArray = new List<byte>();

                bytesRead = stream.Read(readBuf, 0, readSize);
                while (bytesRead > 0)
                {
                    foreach (byte b in readBuf)
                    {
                        dyArray.Add(b);
                    }
                    bytesRead = stream.Read(readBuf, 0, readSize);
                }
                byte[] buf = dyArray.ToArray();
                uint size = System.Convert.ToUInt32(buf.Length);

                int psize = Marshal.SizeOf(buf[0]) * buf.Length;
                IntPtr pnt = Marshal.AllocHGlobal(psize);
                try
                {
                    Marshal.Copy(buf, 0, pnt, buf.Length);
					PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateFromMemory2(doc.mp_doc, pnt, new UIntPtr(size), (encoder_hints==null) ? IntPtr.Zero : encoder_hints.mp_obj, ref result));
                }
                finally
                {
                    // Free the unmanaged memory.
                    Marshal.FreeHGlobal(pnt);
                }
            }
            return new Image(result, doc);
        }
		public static Image Create(SDFDoc doc, System.IO.Stream stream)
		{
			return Image.Create(doc, stream, null);
		}

        /// <summary> Directly embed the image that is already compressed using the Image.InputFilter
	    /// format. The function can be used to pass-through pre-compressed image data.
	    /// 
	    /// </summary>
	    /// <param name="doc">A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
        /// <param name="stream">- input stream containing a serialized document. The input stream may be a
        /// random-access file, memory buffer, slow HTTP connection etc.
        /// </param>
        /// <param name="encoder_hints">The encoder hints
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded image.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  if the input stream doesn't support <c>Seek()</c> operation the document will load whole  data stream in memory before parsing. 
		/// </remarks>
        public static Image Create(SDFDoc doc, Filter stream, Obj encoder_hints)
        {
            TRN_Image result = IntPtr.Zero;
			PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateFromStream2(doc.mp_doc, stream.mp_imp, (encoder_hints == null) ? IntPtr.Zero : encoder_hints.mp_obj, ref result));
            Image that = new Image(result, doc);
            stream.setRefHandleInternal(that);
            return that;
        }
        public static Image Create(SDFDoc doc, Filter stream)
        {
        	return Image.Create(doc, stream, null);
        }

	    /// <summary> Create and embed an ImageMask. Embed the raw image data taking into account
	    /// specified compression hints. The ImageMask can be used as a stencil mask for
	    /// painting in the current color or as an explicit mask specifying which areas of
	    /// the image to paint and which to mask out. One of the most important uses of
	    /// stencil masking is for painting character glyphs represented as bitmaps.
	    /// 
	    /// </summary>
	    /// <param name="doc">- A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">- The stream or buffer containing image data stored in 1 bit per
	    /// sample format. The image data must not be compressed and must follow PDF format for
	    /// sample representation (please refer to section 4.8.2 'Sample Representation' in PDF
	    /// Reference Manual for details).
	    /// </param>
	    /// <param name="width">- The width of the image, in samples.
	    /// </param>
	    /// <param name="height">- The height of the image, in samples.
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded ImageMask.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Image CreateImageMask(SDFDoc doc, byte[] image_data, Int32 width, Int32 height)
        {
            return CreateImageMask(doc, image_data, width, height, null);
        }
	    /// <summary> Create and embed an ImageMask. Embed the raw image data taking into account
	    /// specified compression hints. The ImageMask can be used as a stencil mask for
	    /// painting in the current color or as an explicit mask specifying which areas of
	    /// the image to paint and which to mask out. One of the most important uses of
	    /// stencil masking is for painting character glyphs represented as bitmaps.
	    /// 
	    /// </summary>
	    /// <param name="doc">- A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">- The stream or buffer containing image data stored in 1 bit per
	    /// sample format. The image data must not be compressed and must follow PDF format for
	    /// sample representation (please refer to section 4.8.2 'Sample Representation' in PDF
	    /// Reference Manual for details).
	    /// </param>
	    /// <param name="width">- The width of the image, in samples.
	    /// </param>
	    /// <param name="height">- The height of the image, in samples.
	    /// </param>
	    /// <param name="encoder_hints">The encoder hints
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded ImageMask.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Image CreateImageMask(SDFDoc doc, byte[] image_data, Int32 width, Int32 height, Obj encoder_hints)
        {
            TRN_Image result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateImageMask(doc.mp_doc, image_data, new UIntPtr(System.Convert.ToUInt32(image_data.Length)), width, height, (encoder_hints == null) ? IntPtr.Zero : encoder_hints.mp_obj, ref result));
            return new Image(result, doc);
        }
	    /// <summary> Create and embed an ImageMask. Embed the raw image data taking into account
	    /// specified compression hints. The ImageMask can be used as a stencil mask for
	    /// painting in the current color or as an explicit mask specifying which areas of
	    /// the image to paint and which to mask out. One of the most important uses of
	    /// stencil masking is for painting character glyphs represented as bitmaps.
	    /// 
	    /// </summary>
	    /// <param name="doc">- A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">- The stream or buffer containing image data stored in 1 bit per
	    /// sample format. The image data must not be compressed and must follow PDF format for
	    /// sample representation (please refer to section 4.8.2 'Sample Representation' in PDF
	    /// Reference Manual for details).
	    /// </param>
	    /// <param name="width">- The width of the image, in samples.
	    /// </param>
	    /// <param name="height">- The height of the image, in samples.
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded ImageMask.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  see Image.CreateImageMask for details. </remarks>
        public static Image CreateImageMask(SDFDoc doc, FilterReader image_data, Int32 width, Int32 height)
        {
            return CreateImageMask(doc, image_data, width, height, null);
        }
	    /// <summary> Create and embed an ImageMask. Embed the raw image data taking into account
	    /// specified compression hints. The ImageMask can be used as a stencil mask for
	    /// painting in the current color or as an explicit mask specifying which areas of
	    /// the image to paint and which to mask out. One of the most important uses of
	    /// stencil masking is for painting character glyphs represented as bitmaps.
	    /// 
	    /// </summary>
	    /// <param name="doc">- A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">- The stream or buffer containing image data stored in 1 bit per
	    /// sample format. The image data must not be compressed and must follow PDF format for
	    /// sample representation (please refer to section 4.8.2 'Sample Representation' in PDF
	    /// Reference Manual for details).
	    /// </param>
	    /// <param name="width">- The width of the image, in samples.
	    /// </param>
	    /// <param name="height">- The height of the image, in samples.
	    /// </param>
	    /// <param name="encoder_hints">The encoder hints
	    /// </param>
	    /// <returns> PDF.Image object representing the embedded ImageMask.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Image CreateImageMask(SDFDoc doc, FilterReader image_data, Int32 width, Int32 height, Obj encoder_hints)
        {
            TRN_Image result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateImageMaskFromStream(doc.mp_doc, image_data.mp_imp, width, height, (encoder_hints == null) ? IntPtr.Zero : encoder_hints.mp_obj, ref result));
            return new Image(result, doc);
        }

	    /// <summary> Create and embed a Soft Mask. Embed the raw image data taking into account 
	    /// specified compression hints.
	    /// </summary>
	    /// <param name="doc">A document to which the image should be added. The 'Doc' object 
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">The stream or buffer containing image data represented in 
	    /// DeviceGray color space (i.e. one component per sample). The image data must not 
	    /// be compressed and must follow PDF format for sample representation (please refer
	    /// to section 4.8.2 'Sample Representation' in PDF Reference Manual for details).
	    /// </param>
	    /// <param name="width">The width of the image, in samples.
	    /// </param>
	    /// <param name="height">The height of the image, in samples.
	    /// </param>
	    /// <param name="bpc">The number of bits used to represent each color component.
	    /// </param>
	    /// <param name="encoder_hints">An optional parameter that can be used to fine tune 
	    /// compression or to select a different compression algorithm. See Image.Create() 
	    /// for details.
	    /// </param>
	    /// <returns> the image
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Image CreateSoftMask(SDFDoc doc, byte[] image_data, Int32 width, Int32 height, Int32 bpc, Obj encoder_hints)
        {
            TRN_Image result = IntPtr.Zero;

			int size = Marshal.SizeOf(image_data[0]) * image_data.Length;
			IntPtr pnt = Marshal.AllocHGlobal (size);
			try
			{
				Marshal.Copy(image_data, 0, pnt, image_data.Length);
				PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateSoftMask(doc.mp_doc, pnt, new UIntPtr(System.Convert.ToUInt32(image_data.Length)), width, height, bpc, (encoder_hints == null) ? IntPtr.Zero : encoder_hints.mp_obj, ref result));
				return new Image(result, doc);
			}
			finally 
			{
				Marshal.FreeHGlobal (pnt);
			}      
            
        }
	    /// <summary> Create and embed a Soft Mask. Embed the raw image data taking into account
	    /// specified compression hints.
	    /// 
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="image_data">the image_data
	    /// </param>
	    /// <param name="width">the width
	    /// </param>
	    /// <param name="height">the height
	    /// </param>
	    /// <param name="bpc">the bpc
	    /// </param>
	    /// <returns> the image
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  see Image.CreateSoftMask for details. </remarks>
        public static Image CreateSoftMask(SDFDoc doc, byte[] image_data, Int32 width, Int32 height, Int32 bpc)
        {
            return CreateSoftMask(doc, image_data, width, height, bpc, null);
        }
	    /// <summary> Create and embed a Soft Mask. Embed the raw image data taking into account
	    /// specified compression hints.
	    /// 
	    /// </summary>
	    /// <param name="doc">- A document to which the image should be added. The 'Doc' object
	    /// can be obtained using Obj.GetDoc() or PDFDoc.GetSDFDoc().
	    /// </param>
	    /// <param name="image_data">- The stream or buffer containing image data represented in
	    /// DeviceGray color space (i.e. one component per sample). The image data must not
	    /// be compressed and must follow PDF format for sample representation (please refer
	    /// to section 4.8.2 'Sample Representation' in PDF Reference Manual for details).
	    /// </param>
	    /// <param name="width">- The width of the image, in samples.
	    /// </param>
	    /// <param name="height">- The height of the image, in samples.
	    /// </param>
	    /// <param name="bpc">- The number of bits used to represent each color component.
	    /// </param>
	    /// <returns> the image
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  this feature is available only in PDF 1.4 and higher. </remarks>
        public static Image CreateSoftMask(SDFDoc doc, FilterReader image_data, Int32 width, Int32 height, Int32 bpc)
        {
            return CreateSoftMask(doc, image_data, width, height, bpc, null);
        }
	    /// <summary> Creates the soft mask.
	    /// 
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="image_data">the image_data
	    /// </param>
	    /// <param name="width">the width
	    /// </param>
	    /// <param name="height">the height
	    /// </param>
	    /// <param name="bpc">the bpc
	    /// </param>
	    /// <param name="encoder_hints">the encoder_hint
	    /// </param>
	    /// <returns> the image
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  see Image.CreateSoftMask for details. </remarks>
        public static Image CreateSoftMask(SDFDoc doc, FilterReader image_data, Int32 width, Int32 height, Int32 bpc, Obj encoder_hints)
        {
            TRN_Image result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateSoftMaskFromStream(doc.mp_doc, image_data.mp_imp, width, height, bpc, (encoder_hints == null) ? IntPtr.Zero : encoder_hints.mp_obj, ref result));
            return new Image(result, doc);
        }

	    /// <summary> Create an image from an existing image represented as a SDF/Cos object.
	    /// 
	    /// </summary>
	    /// <param name="image_xobject">the image_xobject
	    /// </param>
        /// <remarks>  To create the Image object from image PDF.Element, pass the Element's
        /// SDF/Cos dictionary to Image constructor (i.e. Image image(element->GetXObject()))</remarks>
        public Image(Obj image_xobject)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCreateFromObj(image_xobject.mp_obj, ref mp_image));
            this.m_ref = image_xobject.GetRefHandleInternal();
        }

	    /// <summary> Sets value to the given image value
	    /// </summary>
        /// <param name="p">image object
        /// </param>
        public void Set(Image p)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCopy(p.mp_image, ref mp_image));
        }
	    /// <summary>Assignment operator</summary>
	    /// <param name="r">an given <c>Image</c> object
	    /// </param>
        /// <returns>an <c>Image</c> object that equals to the given object
        /// </returns>
        public Image op_Assign(Image r)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageCopy(r.mp_image, ref mp_image));
            return this;
        }

	    /// <summary> Checks if is valid.
	    /// 
	    /// </summary>
	    /// <returns> whether this is a valid raster image. If the function returns false the
	    /// underlying SDF/Cos object is not a valid raster image and this Image object should
	    /// be treated as null.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageIsValid(mp_image, ref result));
            return result;
        }

	    /// <summary> Gets the SDFObj.
	    /// 
	    /// </summary>
	    /// <returns> the underlying SDF/Cos object
	    /// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageGetSDFObj(mp_image, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
	    /// <summary> Gets the image data.
	    /// 
	    /// </summary>
	    /// <returns> A stream (filter) containing decoded image data
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Filter GetImageData()
        {
            TRN_Filter result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageGetImageData(mp_image, ref result));
            return new Filter(result, null);
        }
	    /// <summary> Gets the image data size.
	    /// 
	    /// </summary>
	    /// <returns> the size of image data in bytes
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 GetImageDataSize()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageGetImageDataSize(mp_image, ref result));
            return result;
        }
	    /// <summary> Gets the bitmap.
	    /// 
	    /// </summary>
	    /// <returns> the bitmap
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        //public System.Drawing.Bitmap GetBitmap();
        /// <summary> Gets the image color space.
	    /// 
	    /// </summary>
	    /// <returns> The SDF object representing the color space in which image
	    /// samples are specified or NULL if:
	    /// - the image is an image mask
	    /// - or is compressed using JPXDecode with missing ColorSpace entry in image dictionary.
	    /// 
	    /// The returned color space may be any type of color space except Pattern.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorSpace GetImageColorSpace()
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageGetImageColorSpace(mp_image, ref result));
            return new ColorSpace(result, this.m_ref);
        }
	    /// <summary> Gets the image width.
	    /// 
	    /// </summary>
	    /// <returns> the width of the image, in samples.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 GetImageWidth()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageGetImageWidth(mp_image, ref result));
            return result;
        }
	    /// <summary> Gets the image height.
	    /// 
	    /// </summary>
	    /// <returns> the height of the image, in samples.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 GetImageHeight()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageGetImageHeight(mp_image, ref result));
            return result;
        }
	    /// <summary> Gets the decode array.
	    /// 
	    /// </summary>
	    /// <returns> Decode array or NULL if the paramter is not specified. A decode object is an
	    /// array of numbers describing how to map image samples into the range of values
	    /// appropriate for the images color space . If ImageMask is true, the array must be
	    /// either [0 1] or [1 0]; otherwise, its length must be twice the number of color
	    /// components required by ColorSpace. Default value depends on the color space,
	    /// See Table 4.36 in PDF Ref. Manual.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetDecodeArray()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageGetDecodeArray(mp_image, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
	    /// <summary> Gets the bits per component.
	    /// 
	    /// </summary>
	    /// <returns> the number of bits used to represent each color component. Only a
	    /// single value may be specified; the number of bits is the same for all color
	    /// components. Valid values are 1, 2, 4, 8, and 16.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 GetBitsPerComponent()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageGetBitsPerComponent(mp_image, ref result));
            return result;
        }
	    /// <summary> Gets the component number.
	    /// 
	    /// </summary>
	    /// <returns> the number of color components per sample.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 GetComponentNum()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageGetComponentNum(mp_image, ref result));
            return result;
        }
	    /// <summary> Checks if is image mask.
	    /// 
	    /// </summary>
	    /// <returns> a boolean indicating whether the inline image is to be treated as an image mask.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsImageMask()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageIsImageMask(mp_image, ref result));
            return result;
        }
	    /// <summary> Checks if is image interpolate.
	    /// 
	    /// </summary>
	    /// <returns> a boolean indicating whether image interpolation is to be performed.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsImageInterpolate()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageIsImageInterpolate(mp_image, ref result));
            return result;
        }
	    /// <summary> Gets the mask.
	    /// 
	    /// </summary>
	    /// <returns> an image XObject defining an image mask to be applied to this image (See
	    /// 'Explicit Masking', 4.8.5), or an array specifying a range of colors
	    /// to be applied to it as a color key mask (See 'Color Key Masking').
	    /// 
	    /// If IsImageMask() return true, this method will return NULL.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetMask()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageGetMask(mp_image, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
	    /// <summary> Set an Explicit Image Mask.
	    /// 
	    /// </summary>
	    /// <param name="image_mask">the new mask
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  image_mask must be a valid image mask (i.e. image_mask.IsImageMask() must 
        /// return true.</remarks>
        public void SetMask(Image image_mask)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageSetMask(mp_image, image_mask.mp_image));
        }
	    /// <summary> Set a Color Key Mask.
	    /// 
	    /// </summary>
	    /// <param name="mask">is an Cos/SDF array specifying a range of colors to be masked
	    /// out. Samples in the image that fall within this range are not painted, allowing
	    /// the existing background to show through. The effect is similar to that of the
	    /// video technique known as chroma-key. For details of the array format please
	    /// refer to section 4.8.5 'Color Key Masking' in PDF Reference Manual.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  the current document takes the ownership of the given SDF object. </remarks>
        public void SetMask(Obj mask)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageSetMaskWithObj(mp_image, mask.mp_obj));
        }
	    /// <summary> Gets the soft mask.
	    /// 
	    /// </summary>
	    /// <returns> an image XObject defining a Soft Mask to be applied to this image
	    /// (See section 7.5.4 'Soft-Mask Images' in PDF Reference Manual), or NULL
	    /// if the image does not have the soft mask.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetSoftMask()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageGetSoftMask(mp_image, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
	    /// <summary> Set a Soft Mask.
	    /// 
	    /// </summary>
	    /// <param name="soft_mask">is a subsidiary Image object defining a soft-mask image
	    /// (See section 7.5.4 'Soft-Mask Images' in PDF Reference Manual) to be used
	    /// as a source of mask shape or mask opacity values in the transparent imaging
	    /// model. The alpha source parameter in the graphics state determines whether
	    /// the mask values are interpreted as shape or opacity.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetSoftMask(Image soft_mask)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageSetSoftMask(mp_image, soft_mask.mp_image));
        }
	    /// <summary> Gets the image rendering intent.
	    /// 
	    /// </summary>
	    /// <returns> The color rendering intent to be used in rendering the image.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public GState.RenderingIntent GetImageRenderingIntent()
        {
            GState.RenderingIntent result = GState.RenderingIntent.e_absolute_colorimetric;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageGetImageRenderingIntent(mp_image, ref result));
            return result;
        }

	    /// <summary> Saves this image to a file.
	    /// 
	    /// The output image format (TIFF, JPEG, or PNG) will be
	    /// automatically selected based on the properties of the embedded
	    /// image. For example, if the embedded image is using CCITT Fax
	    /// compression, the output format will be TIFF. Similarly, if the
	    /// embedded image is using JPEG compression the output format will
	    /// be JPEG. If your application needs to explicitly control output
	    /// image format you may want to use ExportAsTiff() or ExportAsPng().
	    /// 
	    /// </summary>
	    /// <param name="filename">string that specifies the path name for
	    /// the saved image. The filename should not include the extension
	    /// which will be appended to the filename string based on the output
	    /// format.
	    /// </param>
	    /// <returns> the number indicating the selected image format:
	    /// (0 - PNG, 1 - TIF, 2 - JPEG).
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 Export(String filename)
        {
            int result = int.MinValue;
            UString str = new UString(filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageExport(mp_image, str.mp_impl, ref result));
            return result;
        }
	    /// <summary> Saves this image to the output stream.
	    /// 
	    /// </summary>
	    /// <param name="writer">A pointer to FilterWriter used to write to the
	    /// output stream. If the parameter is null, nothing will be written
	    /// to the output stream, but the function returns the format identifier.
	    /// </param>
	    /// <returns>the number indicating the selected image format:
	    /// (0 - PNG, 1 - TIF, 2 - JPEG).
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  see the overloaded Image.Export method for more information. </remarks>
        public Int32 Export(FilterWriter writer)
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageExportFromStream(mp_image, writer.mp_imp, ref result));
            return result;
        }
	    /// <summary> Saves this image to a TIFF file.
	    /// 
	    /// </summary>
	    /// <param name="filename">string that specifies the path name for
	    /// the saved image. The filename should include the file extension
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void ExportAsTiff(String filename)
        {
            UString str = new UString(filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageExportAsTiff(mp_image, str.mp_impl));
        }
	    /// <summary> Saves this image to a TIFF output stream.
	    /// 
	    /// </summary>
	    /// <param name="writer">FilterWriter used to write to the output stream.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void ExportAsTiff(FilterWriter writer)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageExportAsTiffFromStream(mp_image, writer.mp_imp));
        }
	    /// <summary> Saves this image to a PNG file.
	    /// 
	    /// </summary>
	    /// <param name="filename">string that specifies the path name for
	    /// the saved image. The filename should include the file extension
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void ExportAsPng(String filename)
        {
            UString str = new UString(filename);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageExportAsPng(mp_image, str.mp_impl));
        }
	    /// <summary> Saves this image to a PNG output stream.
	    /// 
	    /// </summary>
	    /// <param name="writer">FilterWriter used to write to the output stream.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void ExportAsPng(FilterWriter writer)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ImageExportAsPngFromStream(mp_image, writer.mp_imp));
        }
        /// <summary> Releases all resources used by the Image </summary>
        ~Image()
        {
            Dispose(false);
        }

        // Nested Types
        ///<summary>InputFilter types</summary>
        public enum InputFilter
        {
            ///<summary>Input stream is not compressed</summary>
            e_none,
            ///<summary>Input image is a JPEG image</summary>
            e_jpeg,
            ///<summary>Input image is a JP2 (JPEG2000) image</summary>
            e_jp2,
            ///<summary>Input image is a Flate compressed</summary>
            e_flate,
            ///<summary>Input image is a G3 stream</summary>
            e_g3,
            ///<summary>Input image is a G4 stream</summary>
            e_g4,
            ///<summary>Input image stream compressed using ASCIIHexDecode filter</summary>
            e_ascii_hex
        }
    }
}
