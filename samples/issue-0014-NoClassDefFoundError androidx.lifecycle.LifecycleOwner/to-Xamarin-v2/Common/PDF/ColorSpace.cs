using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.Filters;
using pdftron.SDF;

using TRN_Exception = System.IntPtr;
using TRN_ColorSpace = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_Function = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> This abstract class is used to serve as a color space tag to identify the specific 
    /// color space of a Color object. It contains methods that transform colors in a specific 
    /// color space to/from several color space such as DeviceRGB and DeviceCMYK. 
    /// 
    /// For purposes of the methods in this class, colors are represented as arrays of color 
    /// components represented as doubles in a normalized range defined by each ColorSpace. 
    /// For many ColorSpaces (e.g. DeviceRGB), this range is 0.0 to 1.0. However, some ColorSpaces 
    /// have components whose values have a different range. Methods are provided to inquire per 
    /// component minimum and maximum normalized values. 
    /// 
    /// </summary>
    /// <remarks>  Note that in Pattern color space (i.e. for ColorSpace::e_pattern) 'color values'
    /// are PDF::PatternColor objects instead of the numeric component values (i.e. ColorPt) 
    /// used with other spaces.
    /// </remarks>
    public class ColorSpace : IDisposable
    {
        internal TRN_ColorSpace mp_cs = IntPtr.Zero;
        internal Object m_ref;

        /// <summary> Releases all resources used by the ColorSpace </summary>
        ~ColorSpace()
        {
            Dispose(false);
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
            Destroy();
        }
        
        public void Destroy()
        {
            if (mp_cs != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceDestroy(mp_cs));
                mp_cs = IntPtr.Zero;
            }
        }

        // Methods
        internal ColorSpace(TRN_ColorSpace impl, Object reference)
        {
            this.mp_cs = impl;
            this.m_ref = reference;
        }
        // Common ColorSpace methods ----------------------------------------------------
	    /// <summary> Create a new DeviceGray ColorSpace object.
	    /// 
	    /// </summary>
	    /// <returns> the color space
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ColorSpace CreateDeviceGray()
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceCreateDeviceGray(ref result));
            return new ColorSpace(result, null);
        }
	    /// <summary> Create a new DeviceRGB ColorSpace object.
	    /// 
	    /// </summary>
	    /// <returns> the color space
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ColorSpace CreateDeviceRGB()
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceCreateDeviceRGB(ref result));
            return new ColorSpace(result, null);
        }
	    /// <summary> Create a new DeviceCMYK ColorSpace object.
	    /// 
	    /// </summary>
	    /// <returns> the color space
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ColorSpace CreateDeviceCMYK()
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceCreateDeviceCMYK(ref result));
            return new ColorSpace(result, null);
        }
	    /// <summary> Create a new Pattern ColorSpace object.			
	    /// </summary>
	    /// <returns> the color space
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ColorSpace CreatePattern()
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceCreatePattern(ref result));
            return new ColorSpace(result, null);
        }
	    /// <summary>Create a new ColorSpace from a given object
	    /// </summary>
	    /// <param name="color_space">existing color space
	    /// </param>
        /// <returns>the color space
        /// </returns>
        public static ColorSpace Create(Obj color_space)
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceCreate(color_space.mp_obj, ref result));
            return new ColorSpace(result, null);
        }
	    /// <summary> Create a PDF 'ICCBased' color space given an ICC profile.
	    /// 
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="filepath">the filepath
	    /// </param>
	    /// <returns> the color space
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ColorSpace CreateICCFromFile(SDFDoc doc, String filepath)
        {
            TRN_ColorSpace result = IntPtr.Zero;
            UString str = new UString(filepath);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceCreateICCFromFile(doc.mp_doc, str.mp_impl, ref result));
            return new ColorSpace(result, null);
        }
	    /// <summary> Creates the icc from filter.
	    /// 
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="filter">the filter
	    /// </param>
	    /// <returns> the color space
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ColorSpace CreateICCFromFilter(SDFDoc doc, Filter filter)
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceCreateICCFromFilter(doc.mp_doc, filter.mp_imp, ref result));
            return new ColorSpace(result, null);
        }
	    /// <summary> Creates the icc from buffer.
	    /// 
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="buffer">the data
	    /// </param>
	    /// <returns> the color space
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static ColorSpace CreateICCFromBuffer(SDFDoc doc, byte[] buffer)
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceCreateICCFromBuffer(doc.mp_doc, buffer, System.Convert.ToUInt32(buffer.Length), ref result));
            return new ColorSpace(result, null);
        }

	    /// <summary> Create a ColorSpace from the given SDF/Cos object listed under ColorSpaces entry
	    /// in page Resource dictionary. If color_space dictionary is null, a non valid ColorSpace
	    /// object is created.
	    /// 
	    /// </summary>
        /// <param name="cs_dict">the color_space
        /// </param>
        public ColorSpace(Obj cs_dict)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceCreate(cs_dict.mp_obj, ref mp_cs));
            this.m_ref = cs_dict.GetRefHandleInternal();
        }

        /// <summary> Gets the type.
	    /// 
	    /// </summary>
	    /// <returns> The type of this color space
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public new virtual Type GetType()
        {
            Type result = Type.e_null;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceGetType(mp_cs, ref result));
            return result;
        }
	    /// <summary> Gets the sDF obj.
	    /// 
	    /// </summary>
        /// <returns> the underlying SDF/Cos object
        /// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceGetSDFObj(mp_cs, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	    /// <summary> Gets the component num.
	    /// 
	    /// </summary>		
	    /// <returns> The number of components (tint components) used to represent color
	    /// point for this color space
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual Int32 GetComponentNum()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceGetComponentNum(mp_cs, ref result));
            return result;
        }
	    /// <summary> Set color to the initial value used for this color space.
	    /// The initial value depends on the color space (see 4.5.7 in PDF Ref. Manual).
	    /// 
	    /// </summary>
	    /// <param name="out_colorants">the out_colorants
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual void InitColor(ColorPt out_colorants)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceInitColor(mp_cs, out_colorants.mp_colorpt));
        }
	    /// <summary> Initialize default ranges for each color component in the color space.
	    /// For example, default ranges for DeviceRGB are [0 1 0 1 0 1] but for Lab
	    /// the default values might be [0 100 -100 100 -100 100].
	    /// 
	    /// </summary>
	    /// <param name="out_decode_low">the out_decode_low
	    /// </param>
	    /// <param name="out_decode_range">the out_decode_range
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  the size of resulting vectors will match the number of color components 
        ///  in this color space.
        /// </remarks>
        public virtual void InitComponentRanges(double[] out_decode_low, double[] out_decode_range)
        {
            int num_comps = GetComponentNum();
            if (num_comps < 1) return;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceInitComponentRanges(mp_cs, out_decode_low, out_decode_range, num_comps));
        }

	    /// <summary> A convenience function used to convert color points from the current
	    /// color space to DeviceGray color space.
	    /// 
	    /// </summary>
	    /// <param name="in_color">input color point in the current color space
	    /// </param>
	    /// <param name="out_color">output color point in the DeviceGray color space
	    /// </param>
	    /// <returns> the color pt
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  the number to input colorants must match the number of colorants 
        /// expected by the current color space.
        /// </remarks>
        public virtual void Convert2Gray(ColorPt in_color, ColorPt out_color)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceConvert2Gray(mp_cs, in_color.mp_colorpt, out_color.mp_colorpt));
        }
	    /// <summary> A convenience function used to convert color points from the current
	    /// color space to DeviceRGB color space.
	    /// 
	    /// </summary>
	    /// <param name="in_color">input color point in the current color space
	    /// </param>
	    /// <param name="out_color">output color point in the DeviceRGB color space
	    /// </param>
	    /// <returns> the color pt
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  the number to input colorants must match the number of colorants
        /// expected by the current color space.
        /// </remarks>
        public virtual void Convert2RGB(ColorPt in_color, ColorPt out_color)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceConvert2RGB(mp_cs, in_color.mp_colorpt, out_color.mp_colorpt));
        }
	    /// <summary> A convenience function used to convert color points from the current
	    /// color space to DeviceCMYK color space.
	    /// 
	    /// </summary>
	    /// <param name="in_color">input color point in the current color space
	    /// </param>
	    /// <param name="out_color">output color point in the DeviceCMYK color space
	    /// </param>
	    /// <returns> the color pt
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  the number to input colorants must match the number of colorants expected by the current color space.
        /// </remarks>
        public virtual void Convert2CMYK(ColorPt in_color, ColorPt out_color)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceConvert2CMYK(mp_cs, in_color.mp_colorpt, out_color.mp_colorpt));
        }

	    /// <summary> Gets the alternate color space.
	    /// 
	    /// </summary>
	    /// <returns> the alternate color space if it is available or NULL otherwise.
	    /// Color spaces that include alternate color space are e_separation, e_device_n,
	    /// and e_icc.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual ColorSpace GetAlternateColorSpace()
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceGetAlternateColorSpace(mp_cs, ref result));
            return new ColorSpace(result, this.m_ref);
        }
	    /// <summary> Gets the base color space.
	    /// 
	    /// </summary>
	    /// <returns> the base color space if this is an e_indexed or e_pattern with
	    /// associated base color space; NULL otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual ColorSpace GetBaseColorSpace()
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceGetBaseColorSpace(mp_cs, ref result));
            return new ColorSpace(result, this.m_ref);
        }

	    /// <summary>Gets the highest index for the color lookup table for Indexed color space.
	    /// </summary>
	    /// <returns>the highest index for the color lookup table for Indexed color space. Since the color table is indexed from zero to highval, the actual number of entries is highval + 1. For color spaces other than indexed the method returns 0.
	    /// </returns>
        /// <remarks>for color spaces other than Indexed this method throws an exception.
        /// </remarks>
        public virtual int GetHighVal()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceGetHighVal(mp_cs, ref result));
            return result;
        }
	    /// <summary> Gets the lookup table.
	    /// 
	    /// </summary>
	    /// <returns> the color lookup table for Indexed color space. for color spaces other
	    /// than indexed the method returns null.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  for color spaces other than Indexed this method throws an exception. </remarks>
        public IntPtr GetLookupTable()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceGetLookupTable(mp_cs, ref result));
            return result;
        }

	    // Specific ColorSpace methods ----------------------------------------------------

	    /// <summary> Gets the tint function.
	    /// 
	    /// </summary>
	    /// <returns> the function that transforms tint values into color component
	    /// values in the alternate color space.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  for color spaces other than Separation this method throws an exception. </remarks>
        public virtual Function GetTintFunction()
        {
            TRN_Function result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceGetTintFunction(mp_cs, ref result));
            return new Function(result, this.m_ref);
        }
	    /// <summary> Checks if is all.
	    /// 
	    /// </summary>
	    /// <returns> True if Separation color space contains the colorant All.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  for color spaces other than Separation this method throws an exception. </remarks>
        public virtual bool IsAll()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceIsAll(mp_cs, ref result));
            return result;
        }
	    /// <summary> Checks if is none.
	    /// 
	    /// </summary>
	    /// <returns> True if Separation or DeviceN color space contains None colorants.
	    /// For DeviceN the function returns true only if component colorant names are all None.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  for color spaces other than Separation or DeviceN this method throws
        /// an exception. </remarks>
        public virtual bool IsNone()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceIsNone(mp_cs, ref result));
            return result;
        }

	    // public static/Global methods --------------------------------------------------------

	    /// <summary>Gets the number of colorants (tint components) used to represent color point in this color space
	    /// </summary>
	    /// <param name="cs_type"><c>ColorSpace</c> type
	    /// </param>
	    /// <param name="cs"><c>ColorSpace</c> object
	    /// </param>
        /// <returns>The number of colorants (tint components) used to represent color point in this color space
        /// </returns>
        public static Int32 GetComponentNum(Type cs_type, Obj cs)
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceGetComponentNumFromObj(cs_type, cs.mp_obj, ref result));
            return result;
        }
	    /// <summary> Gets the type.
	    /// 
	    /// </summary>
	    /// <param name="cs">Cos/SDF color space object.
	    /// </param>
	    /// <returns> The Type of a given SDF/Cos color space, or e_null for if
	    /// SDF object is not a valid color space
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Type GetType(Obj cs)
        {
            Type result = Type.e_null;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorSpaceGetTypeFromObj(cs.mp_obj, ref result));
            return result;
        }

        // Nested Types
        public enum Type
        {
            //TODO: values missing description
		    ///<summary></summary>
		    e_device_gray,
		    ///<summary></summary>
		    e_device_rgb,
		    ///<summary></summary>
		    e_device_cmyk,
		    ///<summary></summary>
		    e_cal_gray,
		    ///<summary></summary>
		    e_cal_rgb,
		    ///<summary></summary>
		    e_lab,
		    ///<summary></summary>
		    e_icc,
		    ///<summary></summary>
		    e_indexed,
		    ///<summary></summary>
		    e_pattern,
		    ///<summary></summary>
		    e_separation,
		    ///<summary></summary>
		    e_device_n,
		    ///<summary></summary>
		    e_null
        }

    }
}
