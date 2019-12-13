using System;
using System.Collections.Generic;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Shading = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_ColorSpace = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> Shading is a class that represents a flat interface around all PDF shading types: 
    /// <list type="bullet">
    /// <item><description>
    /// In Function-based (type 1) shadings, the color at every point in 
    /// the domain is defined by a specified mathematical function. The function need 
    /// not be smooth or continuous. This is the most general of the available shading 
    /// types, and is useful for shadings that cannot be adequately described with any 
    /// of the other types.
    /// </description></item>
    /// <item><description>
    /// Axial shadings (type 2) define a color blend along a line between two points,
    /// optionally extended beyond the boundary points by continuing the boundary
    /// colors.
    /// </description></item>
    /// <item><description>
    /// Radial shadings (type 3) define a color blend that varies between two circles.
    /// Shadings of this type are commonly used to depict three-dimensional spheres
    /// and cones.
    /// </description></item>
    /// <item><description>
    /// Free-form Gouraud-shaded triangle mesh shadings (type 4) and lattice gouraud 
    /// shadings (type 5) are commonly used to represent complex colored and shaded 
    /// three-dimensional shapes. The area to be shaded is defined by a path composed entirely 
    /// of triangles. The color at each vertex of the triangles is specified, and a technique 
    /// known as Gouraud interpolation is used to color the interiors. The interpolation 
    /// functions defining the shading may be linear or nonlinear.
    /// </description></item></list>
    /// Coons patch mesh shadings (type 6) are constructed from one or more color
    /// patches, each bounded by four cubic Bézier curves.
    /// 
    /// A Coons patch generally has two independent aspects:
    /// <list type="bullet">
    /// <item><description>
    /// Colors are specified for each corner of the unit square, and bilinear 
    /// interpolation is used to fill in colors over the entire unit square
    /// </description></item>
    /// <item><description>
    /// Coordinates are mapped from the unit square into a four-sided patch whose
    /// sides are not necessarily linear. The mapping is continuous: the corners 
    /// of the unit square map to corners of the patch and the sides of the unit 
    /// square map to sides of the patch.
    /// </description></item>        
    /// <item><description>
    /// Tensor-product patch mesh shadings (type 7) are identical to type 6 
    /// (Coons mesh), except that they are based on a bicubic tensor-product 
    /// patch defined by 16 control points, instead of the 12 control points 
    /// that define a Coons patch. The shading Patterns dictionaries representing 
    /// the two patch types differ only in the value of the Type entry and 
    /// in the number of control points specified for each patch in the data stream. 
    /// Although the Coons patch is more concise and easier to use, the tensor-
    /// product patch affords greater control over color mapping. 
    /// </description></item>
    /// </list>
    /// </summary>
    public class Shading : IDisposable
    {
        internal TRN_Shading mp_shade = IntPtr.Zero;
        internal Object m_ref;
        internal Shading(TRN_Shading imp, Object reference)
        {
            this.mp_shade = imp;
            this.m_ref = reference;
        }
        /// <summary> Releases all resources used by the Shading </summary>
        ~Shading()
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
            if (mp_shade != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingDestroy(mp_shade));
                mp_shade = IntPtr.Zero;
            }
        }
        /// <summary> Create a Shading from the given SDF/Cos object listed under /Shading entry
	    /// in the page Resource dictionary.
	    /// 
	    /// </summary>
        /// <param name="shading_dict">shading dictionary
        /// </param>
        public Shading(Obj shading_dict)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingCreate(shading_dict.mp_obj, ref mp_shade));
            m_ref = shading_dict.GetRefHandleInternal();
        }

        /// <summary> Create a Shading from the given SDF/Cos object listed under /Shading entry
	    /// in the page Resource dictionary.
	    /// 
	    /// </summary>
	    /// <param name="shading_dict">shading dictionary
	    /// </param>
        /// <returns>newly created <c>Shading</c> object
        /// </returns>
        public static Shading Create(Obj shading_dict)
        {
            TRN_Shading result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingCreate(shading_dict.mp_obj, ref result));
            return new Shading(result, shading_dict.GetRefHandleInternal());
        }
	    /// <summary> Gets the shading type.
	    /// 
	    /// </summary>		
	    /// <returns> The Type of a given SDF/Cos shading dictionary, or e_null for if
	    /// SDF object is not a valid shading object
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual Type GetType()
        {
            Type result = Type.e_null;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetType(mp_shade, ref result));
            return result;
        }

	    /// <summary> Gets the SDFObj.
	    /// </summary>
        /// <returns> the underlying SDF/Cos object
        /// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetSDFObj(mp_shade, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
	    /// <summary> Gets the base color space.
	    /// 
	    /// </summary>
	    /// <returns> The color space in which color values are expressed.
	    /// This may be any device, CIE-based, or special color space
	    /// except a Pattern space.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorSpace GetBaseColorSpace()
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetBaseColorSpace(mp_shade, ref result));
            return new ColorSpace(result, this.m_ref);
        }
	    /// <summary> Checks for bounding box.
	    /// </summary>
	    /// <returns> true if shading has a bounding box, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean HasBBox()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingHasBBox(mp_shade, ref result));
            return result;
        }
	    /// <summary> Gets the bounding box.
	    /// </summary>
	    /// <returns> a rectangle giving the left, bottom, right, and top
	    /// coordinates, respectively, of the shading’s bounding box. The coordinates
	    /// are interpreted in the shading’s target coordinate space. If present, this
	    /// bounding box is applied as a temporary clipping boundary when the shading
	    /// is painted, in addition to the current clipping path and any other clipping
	    /// boundaries in effect at that time.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>Use HasBBox() method to determine whether the shading has a 
	    ///  background color.
	    ///  </remarks>
	    public Rect GetBBox()
        {
            BasicTypes.TRN_Rect result = new BasicTypes.TRN_Rect();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetBBox(mp_shade, ref result));
            return new Rect(result);
        }
	    /// <summary> Checks for background.
	    /// 
	    /// </summary>
	    /// <returns> true if the shading has a background color or false otherwise.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public Boolean HasBackground()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingHasBackground(mp_shade, ref result));
            return result;
        }
	    /// <summary> An color point represented in base color space specifying a single
	    /// background color value. If present, this color is used before
	    /// any painting operation involving the shading, to fill those portions of the
	    /// area to be painted that lie outside the bounds of the shading object itself.
	    /// In the opaque imaging model, the effect is as if the painting operation were
	    /// performed twice: first with the background color and then again with the
	    /// shading.
	    /// 
	    /// </summary>
	    /// <returns> the background color point
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>The background color is applied only when the shading is used as part
	    /// of a shading pattern, not when it is painted directly with the sh operator.		
	    /// Use <c>HasBackground()</c> method to determine whether the shading has a 
        /// background color.
        /// </remarks>
        public ColorPt GetBackground()
        {
            ColorPt result = new ColorPt();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetBackground(mp_shade, result.mp_colorpt));
            return result;
        }
	    /// <summary> Gets the antialias.
	    /// 
	    /// </summary>
	    /// <returns> A flag indicating whether to filter the shading function to prevent
	    /// aliasing artifacts. See Table 4.25
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean GetAntialias()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetAntialias(mp_shade, ref result));
            return result;
        }

	    // Static/Global methods --------------------------------------------------------
	    /// <summary> Gets shading type from shading dictionary.
	    /// 
	    /// </summary>
	    /// <param name="shading_dict">the shading dictionary
	    /// </param>
	    /// <returns> The Type of a given SDF/Cos shading dictionary, or e_null for if
	    /// SDF object is not a valid shading object
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Type GetType(Obj shading_dict)
        {
            using (Shading sh = new Shading(shading_dict.mp_obj, shading_dict.GetRefHandleInternal()))
            {
                Type result = Type.e_null;
                PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetType(sh.mp_shade, ref result));
                return result;
            }            
        }

	    // Specific methods ------------------------------------------------------------
	    /// <summary> Gets the param start.
	    /// 
	    /// </summary>
	    /// <returns> a number specifying the limiting value of a parametric variable t.
	    /// The variable is considered to vary linearly between GetParamStart() and
	    /// GetParamEnd() as the color gradient varies between the starting and ending points
	    /// of the axis for Axial shading or circles for Radial shading.
	    /// The variable t becomes the input argument to the color function(s).
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  the returned value corresponds to the first value in Domain array.
        /// for shadings other than Axial or Radial this method throws an exception.</remarks>
        public virtual Double GetParamStart()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetParamStart(mp_shade, ref result));
            return result;
        }
	    /// <summary> Gets the param end.
	    /// 
	    /// </summary>
	    /// <returns> a number specifying the limiting value of a parametric variable t.
	    /// The variable is considered to vary linearly between GetParamStart() and
	    /// GetParamEnd() as the color gradient varies between the starting and ending points
	    /// of the axis for Axial shading or circles for Radial shading.
	    /// The variable t becomes the input argument to the color function(s).
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  the returned value corresponds to the second value in Domain array. 
        /// for shadings other than Axial or Radial this method throws an exception. </remarks>
        public virtual Double GetParamEnd()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetParamEnd(mp_shade, ref result));
            return result;
        }
	    /// <summary> Checks if is extend start.
	    /// 
	    /// </summary>
	    /// <returns> a flag specifying whether to extend the shading beyond the starting
	    /// point of the axis for Axial shading or starting circle for Radial shading.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  for shadings other than Axial or Radial this method throws an exception. </remarks>
        public virtual Boolean IsExtendStart()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingIsExtendStart(mp_shade, ref result));
            return result;
        }
	    /// <summary> Checks if is extend end.
	    /// 
	    /// </summary>
	    /// <returns> a flag specifying whether to extend the shading beyond the ending
	    /// point of the axis for Axial shading or ending circle for Radial shading.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  for shadings other than Axial or Radial this method throws an exception. </remarks>
        public virtual Boolean IsExtendEnd()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingIsExtendEnd(mp_shade, ref result));
            return result;
        }
	    /// <summary> Gets the color.
	    /// 
	    /// </summary>
	    /// <param name="t">color point
	    /// </param>
	    /// <param name="out_color">a color point for the given value of t.
        /// </param>			
        /// <exception cref="PDFNetException">  for shadings other than Axial or Radial this method throws an exception. </exception>			
        public virtual void GetColor(Double t, ColorPt out_color)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetColor(mp_shade, t, out_color.mp_colorpt));
        }

	    // Specific AxialShading methods -----------------------------------------------
	    /// <summary> Gets the coords radial.
	    /// 
	    /// </summary>
	    /// <param name="out_x0">x0
	    /// </param>
	    /// <param name="out_y0">y0
	    /// </param>			
	    /// <param name="out_x1">x1
	    /// </param>
	    /// <param name="out_y1">y1
	    /// </param>			
	    /// <exception cref="PDFNetException"> for shadings other than Radial this method throws an exception.</exception>
	    /// <remarks>for Radial shading returns six numbers (x0 y0 r0 x1 y1 r1) specifying
	    /// the centers and radii of the starting and ending circles, expressed in the
	    /// shading’s target coordinate space. The radii r0 and r1 must both be greater
        /// than or equal to 0. If one radius is 0, the corresponding circle is treated
        /// as a point; if both are 0, nothing is painted.</remarks>
        public virtual void GetCoords(ref double out_x0, ref double out_y0, ref double out_x1, ref double out_y1)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetCoords(mp_shade, ref out_x0, ref out_y0, ref out_x1, ref out_y1));
        }

	    // Specific RadialShading methods --------------------------------------------
	    /// <summary> Gets the coords radial.
	    /// 
	    /// </summary>
	    /// <param name="out_x0">x0
	    /// </param>
	    /// <param name="out_y0">y0
	    /// </param>
	    /// <param name="out_r0">r0
	    /// </param>
	    /// <param name="out_x1">x1
	    /// </param>
	    /// <param name="out_y1">y1
	    /// </param>
	    /// <param name="out_r1">r1
	    /// </param>
	    /// <exception cref="PDFNetException"> for shadings other than Radial this method throws an exception.</exception>
	    /// <remarks>for Radial shading returns six numbers (x0 y0 r0 x1 y1 r1) specifying
	    /// the centers and radii of the starting and ending circles, expressed in the
	    /// shading’s target coordinate space. The radii r0 and r1 must both be greater
        /// than or equal to 0. If one radius is 0, the corresponding circle is treated
        /// as a point; if both are 0, nothing is painted.</remarks>
        public virtual void GetCoords(ref double out_x0, ref double out_y0, ref double out_r0, ref double out_x1, ref double out_y1, ref double out_r1)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetCoordsRadial(mp_shade, ref out_x0, ref out_y0, ref out_r0, ref out_x1, ref out_y1, ref out_r1));
        }

	    // Specific FunctionShading methods --------------------------------------------
	    /// <summary> Gets the domain.
	    /// 
	    /// </summary>
	    /// <param name="out_minx">minimum x
	    /// </param>
	    /// <param name="out_maxx">maximum x
	    /// </param>
	    /// <param name="out_miny">minimum y
	    /// </param>
	    /// <param name="out_maxy">maximum y
	    /// </param>
	    /// <returns> An array of four numbers [xmin xmax ymin ymax] specifying the rectangular
	    /// domain of coordinates over which the color function(s) are defined.
	    /// If the function does not contain /Domain entry the function returns: [0 1 0 1].
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  for shadings other than Function this method throws an exception. </remarks>
        public virtual void GetDomain(ref double out_minx, ref double out_maxx, ref double out_miny, ref double out_maxy)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetDomain(mp_shade, ref out_minx, ref out_maxx, ref out_miny, ref out_maxy));
        }
	    /// <summary> Gets the matrix.
	    /// 
	    /// </summary>
	    /// <returns> a matrix specifying a mapping from the coordinate space specified
	    /// by the Domain entry into the shading’s target coordinate space.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  for shadings other than Function this method throws an exception. </remarks>
        public virtual Matrix2D GetMatrix()
        {
            BasicTypes.TRN_Matrix2D result = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetMatrix(mp_shade, ref result));
            return new Matrix2D(result);
        }
	    /// <summary> Gets the color.
	    /// </summary>
	    /// <param name="t1">t1
	    /// </param>
	    /// <param name="t2">t2
	    /// </param>
	    /// <param name="out_color">a color point for the given value of parametric variable (t1, t2).
        /// </param>
        /// <exception cref="PDFNetException">for shadings other than Function this method throws an exception.</exception>			
        public virtual void GetColor(Double t1, Double t2, ColorPt out_color)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ShadingGetColorForFunction(mp_shade, t1, t2, out_color.mp_colorpt));
        }

        // Nested Types
        //TODO: enum documentation missing
        public enum Type
        {
            /// <summary>Function-based (type 1) shadings</summary>
		    e_function_shading,
		    /// <summary>Axial shadings (type 2)</summary>
		    e_axial_shading,
		    /// <summary>Radial shadings (type 3)</summary>
		    e_radial_shading,
		    /// <summary>Free-form Gouraud-shaded triangle mesh shadings (type 4)</summary>
		    e_free_gouraud_shading,
		    /// <summary>lattice Gouraud shadings (type 5)</summary>
		    e_lattice_gouraud_shading,
		    ///<summary>Coons patch mesh shadings (type 6)</summary>
		    e_coons_shading,
		    ///<summary>Tensor-product patch mesh shadings (type 7)</summary>
		    e_tensor_shading,
		    ///<summary></summary>
		    e_null
        }

    }
}