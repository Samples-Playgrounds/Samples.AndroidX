using System;
using System.Runtime.InteropServices;

using pdftron.Common;
using pdftron.Filters;
using pdftron.SDF;

using TRN_Image = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> Image2RGB is a filter that can decompress and normalize any PDF image stream 
    /// (e.g. monochrome, CMYK, etc) into a raw RGB pixel stream.
    /// </summary>
    public class Image2RGB : Filter, IDisposable
    {
        /// <summary> Instantiates a new image2 rgb.
	    /// 
	    /// </summary>
	    /// <param name="image_element">the image_element
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Image2RGB(Element image_element)
            : base(IntPtr.Zero, null)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCreateImage2RGBFromElement(image_element.mp_elem, ref mp_imp));
        }
	    /// <summary> Instantiates a new image2 rgb.
	    /// 
	    /// </summary>
	    /// <param name="image_xobject">the image_xobject
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Image2RGB(Obj image_xobject)
            : base(IntPtr.Zero, null)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCreateImage2RGBFromObj(image_xobject.mp_obj, ref mp_imp));
        }
	    /// <summary> Instantiates a new image2 rgb.
	    /// 
	    /// </summary>
	    /// <param name="image">the image
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Image2RGB(Image image)
            : base(IntPtr.Zero, null)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCreateImage2RGB(image.mp_image, ref mp_imp));
        }
    }
}