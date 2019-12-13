using System;
using System.Runtime.InteropServices;

using pdftron.Common;
using pdftronprivate.trn;

using TRN_Exception = System.IntPtr;
using TRN_ColorPt = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> ColorPt is an array of colorants (or tint values) representing a color point
    /// in an associated color space.
    /// </summary>
    public class ColorPt : IDisposable
    {
        //internal BasicTypes.TRN_ColorPt mp_cpt;
        internal TRN_ColorPt mp_colorpt;

        private void init(double x, double y, double z, double w)
        {
            // need this to allocate memory
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtCreate(ref mp_colorpt));

            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtInit(x, y, z, w, mp_colorpt));
        }

        /// <summary> Releases all resources used by the ColorPt </summary>
        ~ColorPt()
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
            if (disposing)
            {
                // Clean up managed resources
            }
            // Clean up native resources
            Destroy();
        }

        public void Destroy()
        {
            if (mp_colorpt != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtDestroy2(mp_colorpt));
                mp_colorpt = IntPtr.Zero;
            }
        }

        // Methods
        internal ColorPt(TRN_ColorPt imp)//BasicTypes.TRN_ColorPt imp)
        {
            this.mp_colorpt = imp;
        }

        /// <summary> Instantiates a new <c>ColorPt</c> object.
        /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorPt()
        {
            init(0, 0, 0, 0);
        }
        /// <summary> Instantiates a new <c>ColorPt</c> object.
        /// 
        /// </summary>
        /// <param name="x">the x
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorPt(double x)
        {
            init(x, 0, 0, 0);
        }
        /// <summary> Instantiates a new <c>ColorPt</c> object.
        /// 
        /// </summary>
        /// <param name="x">the x
        /// </param>
        /// <param name="y">the y
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorPt(double x, double y)
        {
            init(x, y, 0, 0);
        }
        /// <summary> Instantiates a new <c>ColorPt</c> object.
        /// 
        /// </summary>
        /// <param name="x">the x
        /// </param>
        /// <param name="y">the y
        /// </param>
        /// <param name="z">the z
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorPt(double x, double y, double z)
        {
            init(x, y, z, 0);
        }
        /// <summary> Instantiates a new <c>ColorPt</c> object.
        /// 
        /// </summary>
        /// <param name="x">the x
        /// </param>
        /// <param name="y">the y
        /// </param>
        /// <param name="z">the z
        /// </param>
        /// <param name="w">the w
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorPt(double x, double y, double z, double w)
        {
            init(x, y, z, w);
        }
        /// <summary> Gets the tint value at a given colorant index.
        /// 
        /// </summary>
        /// <param name="colorant_index">the colorant_index
        /// </param>
        /// <returns> the tint value at the specified colorant index.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double Get(int colorant_index)
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtGet(mp_colorpt, colorant_index, ref result));
            return result;
        }
        ///<summary>Assignment operator</summary>
        ///<param name="r">object at the right of the operator
        ///</param>
        ///<returns>object equals to the given object
        ///</returns>
        public ColorPt op_Assign(ColorPt r)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtAssign(mp_colorpt, r.mp_colorpt));
            return this;
        }
        /*public static bool operator ==(ColorPt l, ColorPt r)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtCompare(ref l.mp_cpt, ref r.mp_cpt, ref result);
            if (ex != IntPtr.Zero)
            {
                throw new PDFNetException(ex);
            }
            return result;
        }

        public static bool operator !=(ColorPt l, ColorPt r)
        {
            return !(l == r);
        }*/
        /// <summary>Sets value to the given <c>ColorPt</c> object
        /// </summary>
        /// <param name="p">given <c>ColorPt</c> object
        /// </param>
        public void Set(ColorPt p)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtAssign(mp_colorpt, p.mp_colorpt));
        }
        /// <summary> Sets the first value x of the ColorPt.
        /// 
        /// </summary>
        /// <param name="x">the x
        /// </param>		
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Set(double x)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtSet(mp_colorpt, x, 0, 0, 0));
        }
        /// <summary> Sets the first two values x and y of the ColorPt.
        /// 
        /// </summary>
        /// <param name="x">the x
        /// </param>
        /// <param name="y">the y
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Set(double x, double y)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtSet(mp_colorpt, x, y, 0, 0));
        }
        /// <summary> Sets the colorant index and value.
        /// 
        /// </summary>
        /// <param name="colorant_index">the colorant_index. For example, for a color point
        /// associated with a Gray color space the only allowed value for index 
        /// is 0. For a color point associated with a CMYK color space, the color_index
        /// can range from 0 (cyan) to 4 (black).
        /// </param>
        /// <param name="colorant_value">the colorant_value The new tint value.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>If a color point has more than 4 colorants, SetColorantNum(num_colorants)
        /// must be called before getting or setting tint values.
        /// </remarks>
        public void Set(int colorant_index, double colorant_value)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtSetByIndex(mp_colorpt, colorant_index, colorant_value));
        }
        /// <summary> Sets the first three values x, y, and z of the ColorPt.
        /// 
        /// </summary>
        /// <param name="x">the x
        /// </param>
        /// <param name="y">the y
        /// </param>
        /// <param name="z">the z
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Set(double x, double y, double z)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtSet(mp_colorpt, x, y, z, 0));
        }
        /// <summary> A utility method to set the first 4 tint values. For example, 
        /// color.Set(red, green, blue) will initialize the ColorPt to given 
        /// tint values. 
        /// </summary>
        /// <param name="x">the x
        /// </param>
        /// <param name="y">the y
        /// </param>
        /// <param name="z">the z
        /// </param>
        /// <param name="w">the w
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>All colorants should be in the range [0..1].</remarks>
        /// <remarks>color.Set(gray) is equivalent to Set(0, gray);</remarks>
        public void Set(double x, double y, double z, double w)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtSet(mp_colorpt, x, y, z, w));
        }
        /// <summary> Sets the colorant number.
        /// 
        /// </summary>
        /// <param name="num">the new colorant number
        /// </param>
        /// <remarks>If a color point has more than 4 colorants, SetColorantNum(num_colorants) must be called 
        /// before getting or setting tint values. The number of colorants depends on the associated color space. 
        /// To find how many colorant are associated with a given color space use color_space.GetComponentNum().
        /// </remarks>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetColorantNum(int num)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ColorPtSetColorantNum(mp_colorpt, num));
        }

    }
}