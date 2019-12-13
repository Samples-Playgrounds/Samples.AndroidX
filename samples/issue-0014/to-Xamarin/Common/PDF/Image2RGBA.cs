using System;
using System.Runtime.InteropServices;

using pdftron.Common;
using pdftron.Filters;
using pdftron.SDF;

using TRN_Image = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary>
    /// Image2RGBA is a filter that can decompress and normalize any PDF image stream 
    /// (e.g. monochrome, CMYK, etc) into a raw RGBA pixel stream.
    /// </summary>
    public class Image2RGBA : Filter, IDisposable
    {
        /// <summary> Instantiates a new image2 rgba.
	    /// 
	    /// </summary>
	    /// <param name="image_element">the image_element</param>
        /// <param name="premultiply"></param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Image2RGBA(Element image_element, bool premultiply)
            : base(IntPtr.Zero, null)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCreateImage2RGBAFromElement(image_element.mp_elem, premultiply, ref mp_imp));
        }
	    /// <summary> Instantiates a new image2 rgba.
	    /// 
	    /// </summary>
	    /// <param name="image_xobject">the image_xobject
        /// </param>
        /// <param name="premultiply"></param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Image2RGBA(Obj image_xobject, bool premultiply)
            : base(IntPtr.Zero, null)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCreateImage2RGBAFromObj(image_xobject.mp_obj, premultiply, ref mp_imp));
        }
	    /// <summary> Instantiates a new image2 rgba.
	    /// 
	    /// </summary>
	    /// <param name="image">the image
        /// </param>
        /// <param name="premultiply"></param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Image2RGBA(Image image, bool premultiply)
            : base(IntPtr.Zero, null)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FilterCreateImage2RGBA(image.mp_image, premultiply, ref mp_imp));
        }
    }
}