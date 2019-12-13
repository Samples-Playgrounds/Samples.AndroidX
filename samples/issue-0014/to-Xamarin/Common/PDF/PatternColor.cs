using System;
using System.Collections.Generic;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_PatternColor = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_Shading = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> Patterns are quite general, and have many uses; for example, they can be used 
    /// to create various graphical textures, such as weaves, brick walls, sunbursts, 
    /// and similar geometrical and chromatic effects.
    /// Patterns are specified in a special family of color spaces named Pattern, whose
    /// 'color values' are PatternColor objects instead of the numeric component values 
    /// used with other spaces. Therefore PatternColor is to pattern color space what is 
    /// ColorPt to all other color spaces.
    /// 
    /// A tiling pattern consists of a small graphical figure called a pattern cell. 
    /// Painting with the pattern replicates the cell at fixed horizontal and vertical 
    /// intervals to fill an area. The effect is as if the figure were painted on the 
    /// surface of a clear glass tile, identical copies of which were then laid down 
    /// in an array covering the area and trimmed to its boundaries. This is called 
    /// tiling the area.
    /// 
    /// The pattern cell can include graphical elements such as filled areas, text, 
    /// and sampled images. Its shape need not be rectangular, and the spacing of 
    /// tiles can differ from the dimensions of the cell itself.
    /// 
    /// The order in which individual tiles (instances of the cell) are painted is 
    /// unspecified and unpredictable; it is inadvisable for the figures on adjacent 
    /// tiles to overlap.
    /// </summary>
    public class PatternColor : IDisposable
    {
        internal TRN_PatternColor mp_pc = IntPtr.Zero;
        internal Object m_ref;
        internal PatternColor(TRN_PatternColor imp, Object reference)
        {
            this.mp_pc = imp;
            this.m_ref = reference;
        }
        /// <summary> Releases all resources used by the PatternColor </summary>
        ~PatternColor()
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
            if (mp_pc != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_PatternColorDestroy(mp_pc));
                mp_pc = IntPtr.Zero;
            }
        }

        /// <summary> Gets the type.
	    /// 
	    /// </summary>		
	    /// <returns> The Type of a given SDF/Cos color space, or e_null for if
	    /// SDF object is not a valid pattern object
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual Type GetType()
        {
            Type result = Type.e_null;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PatternColorGetType(mp_pc, ref result));
            return result;
        }
	    /// <summary> Gets the SDFObj.
	    /// 
	    /// </summary>
	    /// <returns> the underlying SDF/Cos object
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PatternColorGetSDFObj(mp_pc, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
	    /// <summary> Gets the matrix.
	    /// 
	    /// </summary>
	    /// <returns> pattern matrix, a transformation matrix that maps the pattern’s
	    /// internal coordinate system to the default coordinate system of the pattern’s
	    /// parent content stream (the content stream in which the pattern is defined as
	    /// a resource). The concatenation of the pattern matrix with that of the parent content
	    /// stream establishes the pattern coordinate space, within which all graphics objects
	    /// in the pattern are interpreted.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Matrix2D GetMatrix()
        {
            BasicTypes.TRN_Matrix2D result = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PatternColorGetMatrix(mp_pc, ref result));
            return new Matrix2D(result);
        }

	    // Static/Global methods ----------------------------------------------------------
	    /// <summary> Gets the type.
	    /// 
	    /// </summary>
	    /// <param name="pattern">the pattern
	    /// </param>
	    /// <returns> The Type of a given SDF/Cos color space, or e_null for if
	    /// SDF object is not a valid pattern object
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Type GetType(Obj pattern)
        {
            Type result = Type.e_null;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PatternColorGetTypeFromObj(pattern.mp_obj, ref result));
            return result;
        }

	    // Specific ShadingPattern methods ------------------------------------------------
	    /// <summary> Gets the shading.
	    /// 
	    /// </summary>
	    /// <returns> The shading object defining the shading pattern’s gradient fill.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  for patterns other than Shading this method throws an exception. </remarks>
        public virtual Shading GetShading()
        {
            TRN_Shading result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PatternColorGetShading(mp_pc, ref result));
            return new Shading(result, this.m_ref);
        }

        // Common PatternColor methods ----------------------------------------------------
	    /// <summary> Create a PatternColor from the given SDF/Cos object listed under Pattern entry
	    /// in page Resource dictionary.
	    /// 
	    /// </summary>
        /// <param name="pattern">the pattern
        /// </param>
        public PatternColor(Obj pattern)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PatternColorCreate(pattern.mp_obj, ref mp_pc));
            this.m_ref = pattern.GetRefHandleInternal();
        }

        /// <summary> Gets the tiling type.
	    /// 
	    /// </summary>
	    /// <returns> the tiling type identifier that controls adjustments to the spacing
	    /// of tiles relative to the device pixel grid:
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  for patterns other than Tiling this method throws an exception. </remarks>
        public virtual TilingType GetTilingType()
        {
            TilingType result = TilingType.e_constant_spacing;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PatternColorGetTilingType(mp_pc, ref result));
            return result;
        }
	    /// <summary> Gets the bounding box.
	    /// 
	    /// </summary>
	    /// <returns> A rectangle in the pattern coordinate system giving the
	    /// coordinates of the left, bottom, right, and top edges, respectively,
	    /// of the pattern cell’s bounding box. These boundaries are used to clip
	    /// the pattern cell.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  for patterns other than Tiling this method throws an exception. </remarks>
        public virtual Rect GetBBox()
        {
            BasicTypes.TRN_Rect result = new BasicTypes.TRN_Rect();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PatternColorGetBBox(mp_pc, ref result));
            return new Rect(result);
        }
	    /// <summary> Gets the x step.
	    /// 
	    /// </summary>
	    /// <returns> the desired horizontal spacing between pattern cells, measured
	    /// in the pattern coordinate system.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  that XStep and YStep may differ from the dimensions of the pattern
	    /// cell implied by the BBox entry. This allows tiling with irregularly shaped
	    /// figures. XStep and YStep may be either positive or negative, but not zero.		
	    ///  for patterns other than Tiling this method throws an exception. </remarks>
        public virtual Double GetXStep()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PatternColorGetXStep(mp_pc, ref result));
            return result;
        }
	    /// <summary> Gets the y step.
	    /// 
	    /// </summary>
	    /// <returns> the desired vertical spacing between pattern cells, measured in
	    /// the pattern coordinate system.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  for patterns other than Tiling this method throws an exception. </remarks>
        public virtual Double GetYStep()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PatternColorGetYStep(mp_pc, ref result));
            return result;
        }

        // Nested Types
        //TODO: enum documentation missing
        public enum TilingType
        {
            e_constant_spacing,
            e_no_distortion,
            e_constant_spacing_fast_fill
        }

        //TODO: enum documentation missing
        public enum Type
        {
            e_uncolored_tiling_pattern,
            e_colored_tiling_pattern,
            e_shading,
            e_null
        }

    }
}
