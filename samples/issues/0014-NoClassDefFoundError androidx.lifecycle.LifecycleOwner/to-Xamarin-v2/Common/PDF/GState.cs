using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_GState = System.IntPtr;
using TRN_ColorSpace = System.IntPtr;
using TRN_PatternColor = System.IntPtr;
using TRN_Font = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> GState is a class that keeps track of a number of style attributes used to visually 
    /// define graphical Elements. Each PDF.Element has an associated GState that can be used to 
    /// query or set various graphics properties.
    /// 
    /// </summary>
    /// <remarks>  current clipping path is not tracked in the graphics state for efficiency  
    /// reasons. In most cases tracking of the current clipping path is best left to the 
    /// client.</remarks>
    public class GState
    {
        internal TRN_GState mp_state = IntPtr.Zero;
        internal Object m_ref;
        internal Object m_doc_ref;

        internal GState(TRN_GState imp, Object reference, Object doc_reference)
        {
            this.mp_state = imp;
            this.m_ref = reference;
            this.m_doc_ref = doc_reference;
        }

        // Get Methods ------------------------------------------------------------------
	    /// <summary> Gets the transform.
	    /// 
	    /// </summary>
	    /// <returns> the transformation matrix for this element.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  If you are looking for a matrix that maps coordinates to the initial
        /// user space see Element.GetCTM().</remarks>
        public Matrix2D GetTransform()
        {
            BasicTypes.TRN_Matrix2D result = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetTransform(mp_state, ref result));
            return new Matrix2D(result);
        }
	    /// <summary> Gets the stroke color.
	    /// 
	    /// </summary>
	    /// <returns> a color value/point represented in the current stroke color space
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorSpace GetStrokeColorSpace()
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetStrokeColorSpace(mp_state, ref result));
            return new ColorSpace(result, this.m_ref);
        }
	    /// <summary> Gets the fill color.
	    /// 
	    /// </summary>
	    /// <returns> a color value/point represented in the current fill color space
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorSpace GetFillColorSpace()
        {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetFillColorSpace(mp_state, ref result));
            return new ColorSpace(result, this.m_ref);
        }
	    /// <summary> Gets the stroke color space.
	    /// 
	    /// </summary>
	    /// <returns> color space used for stroking
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorPt GetStrokeColor()
        {
            ColorPt result = new ColorPt();
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetStrokeColor(mp_state, result.mp_colorpt));
            return result;
        }
	    /// <summary> Gets the stroke pattern.
	    /// 
	    /// </summary>
	    /// <returns> the SDF pattern object of currently selected PatternColorSpace used for stroking.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PatternColor GetStrokePattern()
        {
            TRN_PatternColor result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetStrokePattern(mp_state, ref result));
            return new PatternColor(result, this.m_doc_ref);
        }
	    /// <summary> Gets the fill color space.
	    /// 
	    /// </summary>
	    /// <returns> color space used for filling
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorPt GetFillColor()
        {
            //BasicTypes.TRN_ColorPt result = new BasicTypes.TRN_ColorPt();
            ColorPt result = new ColorPt();
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetFillColor(mp_state, result.mp_colorpt));

            return result;
        }
	    /// <summary> Gets the fill pattern.
	    /// 
	    /// </summary>
	    /// <returns> the SDF pattern object of currently selected PatternColorSpace used for filling.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PatternColor GetFillPattern()
        {
            TRN_PatternColor result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetFillPattern(mp_state, ref result));
            return new PatternColor(result, this.m_doc_ref);
        }
	    /// <summary> Gets the flatness.
	    /// 
	    /// </summary>
	    /// <returns> current value of flatness tolerance
	    /// 
	    /// Flatness is a number in the range 0 to 100; a value of 0 specifies the output
	    /// device’s default flatness tolerance.
	    /// 
	    /// The flatness tolerance controls the maximum permitted distance in device pixels
	    /// between the mathematically correct path and an approximation constructed from
	    /// straight line segments.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetFlatness()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetFlatness(mp_state, ref result));
            return result;
        }
        /// <summary> Gets the line cap.
	    /// 
	    /// </summary>
	    /// <returns> currently selected LineCap style
	    /// 
	    /// The line cap style specifies the shape to be used at the ends of open subpaths
	    /// (and dashes, if any) when they are stroked.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public LineCap GetLineCap()
        {
            LineCap result = LineCap.e_butt_cap;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetLineCap(mp_state, ref result));
            return result;
        }

        /// <summary> Gets the line join.
	    /// 
	    /// </summary>
	    /// <returns> currently selected LineJoin style
	    /// 
	    /// The line join style specifies the shape to be used at the corners of paths that
	    /// are stroked.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public LineJoin GetLineJoin()
        {
            LineJoin result = LineJoin.e_bevel_join;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetLineJoin(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the line width.
	    /// 
	    /// </summary>
	    /// <returns> the thickness of the line used to stroke a path.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  A line width of 0 denotes the thinnest line that can be rendered at device </remarks>
        public double GetLineWidth()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetLineWidth(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the miter limit.
	    /// 
	    /// </summary>
	    /// <returns> current value of miter limit.
	    /// 
	    /// The miter limit imposes a maximum on the ratio of the miter length to the
	    /// line width. When the limit is exceeded, the join is converted from a miter
	    /// to a bevel.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetMiterLimit()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetMiterLimit(mp_state, ref result));
            return result;
        }
	    //double GetDashes() [];
	    /// <summary> Gets the dashes.
	    /// 
	    /// </summary>
	    /// <returns> The method fills the vector with an array of numbers representing the dash pattern
	    /// 
	    /// The line dash pattern controls the pattern of dashes and gaps used to stroke
	    /// paths. It is specified by a dash array and a dash phase. The dash array’s elements
	    /// are numbers that specify the lengths of alternating dashes and gaps; the dash phase
	    /// specifies the distance into the dash pattern at which to start the dash. The elements
	    /// of both the dash array and the dash phase are expressed in user space units.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double[] GetDashes()//Don't know if it is right, but should be right, as double is like managed type. 
        {
            int dashes_sz = 0;
            double[] dashes = new double[dashes_sz];
            IntPtr src = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetDashes(mp_state, ref src, ref dashes_sz));
            if (dashes_sz < 1)
            {
                return dashes;
            }
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetDashes(mp_state, ref src, ref dashes_sz));

            System.Runtime.InteropServices.Marshal.Copy(src, dashes, 0, dashes_sz);
            return dashes;
        }
	    /// <summary> Gets the phase.
	    /// 
	    /// </summary>
	    /// <returns> the phase of the currently selected dash pattern. dash phase is expressed in
	    /// user space units.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetPhase()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetPhase(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the char spacing.
	    /// 
	    /// </summary>
	    /// <returns> currently selected character spacing.
	    /// 
	    /// The character spacing parameter is a number specified in unscaled text space
	    /// units. When the glyph for each character in the string is rendered, the character
	    /// spacing is added to the horizontal or vertical component of the glyph’s displacement,
	    /// depending on the writing mode. See Section 5.2.1 in PDF Reference Manual for details.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetCharSpacing()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetCharSpacing(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the word spacing.
	    /// 
	    /// </summary>
	    /// <returns> currently selected word spacing
	    /// 
	    /// Word spacing works the same way as character spacing, but applies only to the
	    /// space character (char code 32). See Section 5.2.2 in PDF Reference Manual for details.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetWordSpacing()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetWordSpacing(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the horizontal scale.
	    /// 
	    /// </summary>
	    /// <returns> currently selected horizontal scale
	    /// 
	    /// The horizontal scaling parameter adjusts the width of glyphs by stretching
	    /// or compressing them in the horizontal direction. Its value is specified as
	    /// a percentage of the normal width of the glyphs, with 100 being the normal width.
	    /// The scaling always applies to the horizontal coordinate in text space, independently
	    /// of the writing mode. See Section 5.2.3 in PDF Reference Manual for details.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetHorizontalScale()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetHorizontalScale(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the leading.
	    /// 
	    /// </summary>
	    /// <returns> currently selected leading parameter
	    /// 
	    /// The leading parameter is measured in unscaled text space units. It specifies
	    /// the vertical distance between the baselines of adjacent lines of text.
	    /// See Section 5.2.4 in PDF Reference Manual for details.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetLeading()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetLeading(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the font.
	    /// 
	    /// </summary>
	    /// <returns> currently selected font
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Font GetFont()
        {
            TRN_Font result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetFont(mp_state, ref result));
            return new Font(result, this.m_ref);
        }
	    /// <summary> Gets the font size.
	    /// 
	    /// </summary>
	    /// <returns> the font size
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetFontSize()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetFontSize(mp_state, ref result));
            return result;
        }
        /// <summary> Gets the text render mode.
	    /// 
	    /// </summary>
	    /// <returns> current text rendering mode.
	    /// 
	    /// The text rendering mode determines whether showing text causes glyph outlines to
	    /// be stroked, filled, used as a clipping boundary, or some combination of the three.
	    /// See Section 5.2.5 in PDF Reference Manual for details..
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public TextRenderingMode GetTextRenderMode()
        {
            TextRenderingMode result = TextRenderingMode.e_clip_text;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetTextRenderMode(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the text rise.
	    /// 
	    /// </summary>
	    /// <returns> current value of text rise
	    /// 
	    /// Text rise specifies the distance, in unscaled text space units, to move the
	    /// baseline up or down from its default location. Positive values of text rise
	    /// move the baseline up
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetTextRise()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetTextRise(mp_state, ref result));
            return result;
        }
	    /// <summary> Checks if is text knockout.
	    /// 
	    /// </summary>
	    /// <returns> a boolean flag that determines the text element is considered
	    /// elementary objects for purposes of color compositing in the transparent imaging
	    /// model.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsTextKnockout()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateIsTextKnockout(mp_state, ref result));
            return result;
        }
        /// <summary> Gets the rendering intent.
	    /// 
	    /// </summary>
	    /// <returns> The color intent to be used for rendering the Element
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public RenderingIntent GetRenderingIntent()
        {
            RenderingIntent result = RenderingIntent.e_absolute_colorimetric;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetRenderingIntent(mp_state, ref result));
            return result;
        }
	    /// <summary> A utility function that maps a string representing a rendering intent to
	    /// RenderingIntent type.
	    /// 
	    /// </summary>
	    /// <param name="name">the name
	    /// </param>
	    /// <returns> The color rendering intent type matching the specified string
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static RenderingIntent GetRenderingIntentType(String name)
        {
            RenderingIntent result = RenderingIntent.e_absolute_colorimetric;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetRenderingIntentType(name, ref result));
            return result;
        }

        /// <summary> Gets the blend mode.
	    /// 
	    /// </summary>
	    /// <returns> the current blend mode to be used in the transparent imaging model.
	    /// Corresponds to the /BM key within the ExtGState's dictionary.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public BlendMode GetBlendMode()
        {
            BlendMode result = BlendMode.e_bl_color;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetBlendMode(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the fill opacity.
	    /// 
	    /// </summary>
	    /// <returns> the opacity value for painting operations other than stroking.
	    /// Returns the value of the /ca key in the ExtGState dictionary. If the value is not
	    /// found, the default value of 1 is returned.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetFillOpacity()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetFillOpacity(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the stroke opacity.
	    /// 
	    /// </summary>
	    /// <returns> opacity value for stroke painting operations for paths and glyph outlines.
	    /// Returns the value of the /CA key in the ExtGState dictionary. If the value is not
	    /// found, the default value of 1 is returned.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetStrokeOpacity()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetStrokeOpacity(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the aIS flag.
	    /// 
	    /// </summary>
	    /// <returns> the alpha source flag ('alpha is shape'), specifying whether the
	    /// current soft mask and alpha constant are to be interpreted as shape values
	    /// (true) or opacity values (false).
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean GetAISFlag()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetAISFlag(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the soft mask.
	    /// 
	    /// </summary>
	    /// <returns> Associated soft mask. NULL if the soft mask is not selected or
	    /// SDF dictionary representing the soft mask otherwise.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetSoftMask()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetSoftMask(mp_state, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_doc_ref));
        }
	    /// <summary> Gets the soft mask transform.
	    /// 
	    /// </summary>
	    /// <returns> The soft mask transform. This is the transformation matrix at the moment the soft
	    /// mask is established in the graphics state with the gs operator. This information is only
	    /// relevant when applying the soft mask that may be specified in the graphics state to the
	    /// current element.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Matrix2D GetSoftMaskTransform()
        {
            BasicTypes.TRN_Matrix2D result = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetSoftMaskTransform(mp_state, ref result));
            return new Matrix2D(result);
        }
	    /// <summary> Gets the stroke overprint.
	    /// 
	    /// </summary>
	    /// <returns> whether overprint is enabled for stroke painting operations.
	    /// Corresponds to the /OP key within the ExtGState's dictionary.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean GetStrokeOverprint()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetStrokeOverprint(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the fill overprint.
	    /// 
	    /// </summary>
	    /// <returns> whether overprint is enabled for fill painting operations.
	    /// Corresponds to the /op key within the ExtGState's dictionary.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean GetFillOverprint()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetFillOverprint(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the overprint mode.
	    /// 
	    /// </summary>
	    /// <returns> the overprint mode used by this graphics state.
	    /// Corresponds to the /OPM key within the ExtGState's dictionary.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 GetOverprintMode()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetOverprintMode(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the auto stroke adjust.
	    /// 
	    /// </summary>
	    /// <returns> a flag specifying whether stroke adjustment is enabled in the graphics
	    /// state. Corresponds to the /SA key within the ExtGState's dictionary.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean GetAutoStrokeAdjust()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetAutoStrokeAdjust(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the smoothness tolerance.
	    /// 
	    /// </summary>
	    /// <returns> the smoothness tolerance used to control the quality of smooth
	    /// shading. Corresponds to the /SM key within the ExtGState's dictionary.
	    /// The allowable error (or tolerance) is expressed as a fraction of the range
	    /// of the color component, from 0.0 to 1.0.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double GetSmoothnessTolerance()
        {
            double result = double.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetSmoothnessTolerance(mp_state, ref result));
            return result;
        }
	    /// <summary> Gets the transfer funct.
	    /// 
	    /// </summary>
	    /// <returns> currently selected transfer function (NULL by default) used during
	    /// color conversion process. A transfer function adjusts the values of color
	    /// components to compensate for nonlinear response in an output device and in
	    /// the human eye. Corresponds to the /TR key within the ExtGState's dictionary.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetTransferFunct()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetTransferFunct(mp_state, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_doc_ref));
        }
	    /// <summary> Gets the black gen funct.
	    /// 
	    /// </summary>
	    /// <returns> currently selected black-generation function (NULL by default) used
	    /// during conversion between DeviceRGB and DeviceCMYK. Corresponds to the /BG key
	    /// within the ExtGState's dictionary.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetBlackGenFunct()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetBlackGenFunct(mp_state, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_doc_ref));
        }
	    /// <summary> Gets the uCR funct.
	    /// 
	    /// </summary>
	    /// <returns> currently selected undercolor-removal function (NULL by default) used
	    /// during conversion between DeviceRGB and DeviceCMYK. Corresponds to the /UCR key
	    /// within the ExtGState's dictionary.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetUCRFunct()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetUCRFunct(mp_state, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_doc_ref));
        }
	    /// <summary> Gets the halftone.
	    /// 
	    /// </summary>
	    /// <returns> currently selected halftone dictionary or stream (NULL by default).
	    /// Corresponds to the /HT key within the ExtGState's dictionary.
	    /// Halftoning is a process by which continuous-tone colors are approximated on an
	    /// output device that can achieve only a limited number of discrete colors.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetHalftone()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateGetHalftone(mp_state, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_doc_ref));
        }

	    /// <summary> Gets the blend mode.
	    /// 
	    /// </summary>
	    /// <returns> the current blend mode to be used in the transparent imaging model.
	    /// Corresponds to the /BM key within the ExtGState's dictionary.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <summary> Sets the current blend mode to be used in the transparent imaging model.
	    /// Corresponds to the /BM key within the ExtGState's dictionary.
	    /// </summary>				
	    /// <param name="BM">- New blending mode type.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <example>
	    /// <code>  
	    /// // C#
	    /// gs.SetBlendMode(GState.BlendMode.e_lighten);
        /// </code>
        /// </example>
        public void SetBlendMode(BlendMode BM)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetBlendMode(mp_state, BM));
        }
	    /// <summary> Sets the opacity value for painting operations other than stroking.
	    /// Corresponds to the value of the /ca key in the ExtGState dictionary.
	    /// 
	    /// </summary>
	    /// <param name="ca">the new fill opacity
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetFillOpacity(double ca)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetFillOpacity(mp_state, ca));
        }
	    /// <summary> Sets opacity value for stroke painting operations for paths and glyph outlines.
	    /// Corresponds to the value of the /CA key in the ExtGState dictionary.
	    /// 
	    /// </summary>
	    /// <param name="CA">the new stroke opacity
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetStrokeOpacity(double CA)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetStrokeOpacity(mp_state, CA));
        }
	    /// <summary> Specifies if the alpha is to be interpreted as a shape or opacity mask.
	    /// The alpha source flag ('alpha is shape'), specifies whether the
	    /// current soft mask and alpha constant are to be interpreted as shape values
	    /// (true) or opacity values (false).
	    /// 
	    /// </summary>
	    /// <param name="AIS">the new aIS flag
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetAISFlag(Boolean AIS)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetAISFlag(mp_state, AIS));
        }
	    /// <summary> Sets the soft mask of the extended graphics state.
	    /// Corresponds to the /SMask key within the ExtGState's dictionary.
	    /// 
	    /// </summary>
	    /// <param name="SM">the new soft mask
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetSoftMask(Obj SM)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetSoftMask(mp_state, SM.mp_obj));
        }
	    /// <summary> Specifies if overprint is enabled for stroke operations. Corresponds to the /OP
	    /// key within the ExtGState's dictionary.
	    /// 
	    /// </summary>
	    /// <param name="OP">the new stroke overprint
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetStrokeOverprint(Boolean OP)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetStrokeOverprint(mp_state, OP));
        }
	    /// <summary> Specifies if overprint is enabled for fill operations. Corresponds to the /op
	    /// key within the ExtGState's dictionary.
	    /// 
	    /// </summary>
	    /// <param name="op">the new fill overprint
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetFillOverprint(Boolean op)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetFillOverprint(mp_state, op));
        }
	    /// <summary> Sets the overprint mode. Corresponds to the /OPM key within the ExtGState's
	    /// dictionary.
	    /// 
	    /// </summary>
	    /// <param name="OPM">the new overprint mode
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetOverprintMode(Int32 OPM)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetOverprintMode(mp_state, OPM));
        }
	    /// <summary> Specify whether to apply automatic stroke adjustment.
	    /// Corresponds to the /SA key within the ExtGState's dictionary.
	    /// 
	    /// </summary>
	    /// <param name="SA">the new auto stroke adjust
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetAutoStrokeAdjust(Boolean SA)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetAutoStrokeAdjust(mp_state, SA));
        }
	    /// <summary> Sets the smoothness tolerance used to control the quality of smooth
	    /// shading. Corresponds to the /SM key within the ExtGState's dictionary.
	    /// 
	    /// </summary>
	    /// <param name="SM">the new smoothness tolerance
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetSmoothnessTolerance(double SM)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetSmoothnessTolerance(mp_state, SM));
        }
	    /// <summary> Sets black-generation function used during conversion between DeviceRGB
	    /// and DeviceCMYK. Corresponds to the /BG key within the ExtGState's
	    /// dictionary.
	    /// 
	    /// </summary>
	    /// <param name="BG">- SDF/Cos black-generation function or name
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetBlackGenFunct(Obj BG)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetBlackGenFunct(mp_state, BG.mp_obj));
        }
	    /// <summary> Sets undercolor-removal function used during conversion between DeviceRGB
	    /// and DeviceCMYK. Corresponds to the /UCR key within the ExtGState's
	    /// dictionary.
	    /// 
	    /// </summary>
	    /// <param name="UCR">- SDF/Cos undercolor-removal function or name
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetUCRFunct(Obj UCR)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetUCRFunct(mp_state, UCR.mp_obj));
        }
	    /// <summary> Sets transfer function used during color conversion process. A transfer
	    /// function adjusts the values of color components to compensate for nonlinear
	    /// response in an output device and in the human eye. Corresponds to the /TR key
	    /// within the ExtGState's dictionary.
	    /// 
	    /// </summary>
	    /// <param name="TR">- SDF/Cos transfer function, array, or name
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetTransferFunct(Obj TR)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetTransferFunct(mp_state, TR.mp_obj));
        }
	    /// <summary> Sets the halftone.
	    /// 
	    /// </summary>
	    /// <param name="HT">- SDF/Cos halftone dictionary, stream, or name
	    /// </param>
	    /// <returns> currently selected halftone dictionary or stream (NULL by default).
	    /// Corresponds to the /HT key within the ExtGState's dictionary.
	    /// Halftoning is a process by which continuous-tone colors are approximated on an
	    /// output device that can achieve only a limited number of discrete colors.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetHalftone(Obj HT)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetHalftone(mp_state, HT.mp_obj));
        }

	    // Set Methods ------------------------------------------------------------------
	    /// <summary> Set the transformation matrix associated with this element.
	    /// 
	    /// </summary>
	    /// <param name="mtx">The new transformation for this text element.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  in PDF associating a transformation matrix with an element
	    /// ('cm' operator) will also affect all subsequent elements.</remarks>
        public void SetTransform(Matrix2D mtx)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetTransformMatrix(mp_state, ref mtx.mp_mtx));
        }
	    /// <summary> Sets the transformation matrix for this element. This method accepts text
	    /// transformation matrix components directly.
	    /// 
	    /// A transformation matrix in PDF is specified by six numbers, usually
	    /// in the form of an array containing six elements. In its most general
	    /// form, this array is denoted [a b c d h v]; it can represent any linear
	    /// transformation from one coordinate system to another. For more
	    /// information about PDF matrices please refer to section 4.2.2 'Common
	    /// Transformations' in PDF Reference Manual, and to documentation for
	    /// pdftron.Common.Matrix2D class.
	    /// 
	    /// </summary>
	    /// <param name="a">- horizontal 'scaling' component of the new text matrix.
	    /// </param>
	    /// <param name="b">- 'rotation' component of the new text matrix.
	    /// </param>
	    /// <param name="c">- vertical 'scaling' component of the new text matrix.
	    /// </param>
	    /// <param name="d">- 'rotation' component of the new text matrix.
	    /// </param>
	    /// <param name="h">- horizontal translation component of the new text matrix.
	    /// </param>
	    /// <param name="v">- vertical translation component of the new text matrix.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetTransform(double a, double b, double c, double d, double h, double v)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetTransform(mp_state, a, b, c, d, h, v));
        }
	    /// <summary> Concatenate the given matrix to the transformation matrix of this element.
	    /// 
	    /// </summary>
	    /// <param name="mtx">the mtx
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Concat(Matrix2D mtx)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateConcatMatrix(mp_state, ref mtx.mp_mtx));
        }
	    /// <summary> Concat.
	    /// 
	    /// </summary>
	    /// <param name="a">- horizontal 'scaling' component of the new text matrix.
	    /// </param>
	    /// <param name="b">- 'rotation' component of the new text matrix.
	    /// </param>
	    /// <param name="c">- vertical 'scaling' component of the new text matrix.
	    /// </param>
	    /// <param name="d">- 'rotation' component of the new text matrix.
	    /// </param>
	    /// <param name="h">- horizontal translation component of the new text matrix.
	    /// </param>
	    /// <param name="v">- vertical translation component of the new text matrix.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Concat(double a, double b, double c, double d, double h, double v)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateConcat(mp_state, a, b, c, d, h, v));
        }
	    /// <summary> Sets the color space used for stroking operations.
	    /// 
	    /// </summary>
	    /// <param name="cs">the new stroke color space
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetStrokeColorSpace(ColorSpace cs)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetStrokeColorSpace(mp_state, cs.mp_cs));
        }
	    /// <summary> Sets the color space used for filling operations.
	    /// 
	    /// </summary>
	    /// <param name="cs">the new fill color space
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetFillColorSpace(ColorSpace cs)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetFillColorSpace(mp_state, cs.mp_cs));
        }
	    /// <summary> Sets the color value/point used for stroking operations.
	    /// The color value must be represented in the currently selected color space used
	    /// for stroking.
	    /// 
	    /// </summary>
	    /// <param name="c">the new stroke color
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetStrokeColor(ColorPt c)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetStrokeColorWithColorPt(mp_state, c.mp_colorpt));
        }
	    /// <summary> Set the stroke color to the given tiling pattern.
	    /// 
	    /// </summary>
	    /// <param name="pattern">SDF pattern object.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  The currently selected stroke color space must be Pattern color space. </remarks>
        public void SetStrokeColor(PatternColor pattern)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetStrokeColorWithPattern(mp_state, pattern.mp_pc));
        }
	    /// <summary> Set the stroke color to the given uncolored tiling pattern.
	    /// 
	    /// </summary>
	    /// <param name="pattern">the pattern
	    /// </param>
	    /// <param name="c">is a color in the pattern’s underlying color space.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  The currently selected stroke color space must be Pattern color space. </remarks>
        public void SetStrokeColor(PatternColor pattern, ColorPt c)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetStrokeColor(mp_state, pattern.mp_pc, c.mp_colorpt));
        }
	    /// <summary> Sets the color value/point used for filling operations.
	    /// The color value must be represented in the currently selected color space used
	    /// for filling.
	    /// 
	    /// </summary>
	    /// <param name="c">the new fill color
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetFillColor(ColorPt c)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetFillColorWithColorPt(mp_state, c.mp_colorpt));
        }
	    /// <summary> Set the fill color to the given tiling pattern.
	    /// 
	    /// </summary>
	    /// <param name="pattern">SDF pattern object.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  The currently selected fill color space must be Pattern color space. </remarks>
        public void SetFillColor(PatternColor pattern)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetFillColorWithPattern(mp_state, pattern.mp_pc));
        }
	    /// <summary> Set the fill color to the given uncolored tiling pattern.
	    /// 
	    /// </summary>
	    /// <param name="pattern">the pattern
	    /// </param>
	    /// <param name="c">is a color in the pattern’s underlying color space.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  The currently selected fill color space must be Pattern color space. </remarks>
        public void SetFillColor(PatternColor pattern, ColorPt c)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetFillColor(mp_state, pattern.mp_pc, c.mp_colorpt));
        }
	    /// <summary> Sets the value of flatness tolerance.
	    /// 
	    /// </summary>
	    /// <param name="flatness">is a number in the range 0 to 100; a value of 0 specifies the output
	    /// device’s default flatness tolerance.
	    /// 
	    /// The flatness tolerance controls the maximum permitted distance in device pixels
	    /// between the mathematically correct path and an approximation constructed from
	    /// straight line segments.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetFlatness(double flatness)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetFlatness(mp_state, flatness));
        }
	    /// <summary> Sets LineCap style property.
	    /// The line cap style specifies the shape to be used at the ends of open subpaths
	    /// (and dashes, if any) when they are stroked.
	    /// 
	    /// </summary>
	    /// <param name="cap">the new line cap
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetLineCap(LineCap cap)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetLineCap(mp_state, cap));
        }
	    /// <summary> Sets LineJoin style property.
	    /// 
	    /// The line join style specifies the shape to be used at the corners of paths that
	    /// are stroked.
	    /// 
	    /// </summary>
	    /// <param name="join">the new line join
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetLineJoin(LineJoin join)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetLineJoin(mp_state, join));
        }
	    /// <summary> Sets the thickness of the line used to stroke a path.
	    /// 
	    /// </summary>
	    /// <param name="width">a non-negative number expressed in user space units.
	    /// A line width of 0 denotes the thinnest line that can be rendered at device
	    /// resolution: 1 device pixel wide.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetLineWidth(double width)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetLineWidth(mp_state, width));
        }
	    /// <summary> Sets miter limit.
	    /// 
	    /// </summary>
	    /// <param name="miter_limit">A number that imposes a maximum on the ratio of the miter
	    /// length to the line width. When the limit is exceeded, the join is converted
	    /// from a miter to a bevel.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetMiterLimit(double miter_limit)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetMiterLimit(mp_state, miter_limit));
        }
	    /// <summary> Sets the dash pattern used to stroke paths.
	    /// 
	    /// The line dash pattern controls the pattern of dashes and gaps used to stroke
	    /// paths. It is specified by a dash array and a dash phase. The dash array’s elements
	    /// are numbers that specify the lengths of alternating dashes and gaps; the dash phase
	    /// specifies the distance into the dash pattern at which to start the dash. The elements
	    /// of both the dash array and the dash phase are expressed in user space units.
	    /// 
	    /// </summary>
	    /// <param name="dash_array">the dash_array
	    /// </param>
	    /// <param name="phase">the phase
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetDashPattern(double[] dash_array, double phase)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetDashPattern(mp_state, dash_array, dash_array.Length, phase));
        }
	    /// <summary> Sets character spacing.
	    /// 
	    /// </summary>
	    /// <param name="char_spacing">a number specified in unscaled text space units. When the
	    /// glyph for each character in the string is rendered, the character spacing is
	    /// added to the horizontal or vertical component of the glyph’s displacement,
	    /// depending on the writing mode. See Section 5.2.1 in PDF Reference Manual for details.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetCharSpacing(double char_spacing)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetCharSpacing(mp_state, char_spacing));
        }
	    /// <summary> Sets word spacing.
	    /// 
	    /// </summary>
	    /// <param name="word_spacing">- a number specified in unscaled text space units.
	    /// Word spacing works the same way as character spacing, but applies only to the
	    /// space character (char code 32). See Section 5.2.2 in PDF Reference Manual for details.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetWordSpacing(double word_spacing)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetWordSpacing(mp_state, word_spacing));
        }
	    /// <summary> Sets horizontal scale.
	    /// The horizontal scaling parameter adjusts the width of glyphs by stretching
	    /// or compressing them in the horizontal direction. Its value is specified as
	    /// a percentage of the normal width of the glyphs, with 100 being the normal width.
	    /// The scaling always applies to the horizontal coordinate in text space, independently
	    /// of the writing mode. See Section 5.2.3 in PDF Reference Manual for details.
	    /// 
	    /// </summary>
	    /// <param name="hscale">the new horizontal scale
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetHorizontalScale(double hscale)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetHorizontalScale(mp_state, hscale));
        }
	    /// <summary> Sets the leading parameter.
	    /// 
	    /// The leading parameter is measured in unscaled text space units. It specifies
	    /// the vertical distance between the baselines of adjacent lines of text.
	    /// See Section 5.2.4 in PDF Reference Manual for details.
	    /// 
	    /// </summary>
	    /// <param name="leading">the new leading
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetLeading(double leading)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetLeading(mp_state, leading));
        }
	    /// <summary> Sets the font and font size used to draw text.
	    /// 
	    /// </summary>
	    /// <param name="font">the font
	    /// </param>
	    /// <param name="font_sz">the font_sz
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetFont(Font font, double font_sz)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetFont(mp_state, font.mp_font, font_sz));
        }
	    /// <summary> Sets text rendering mode.
	    /// The text rendering mode determines whether showing text causes glyph outlines to
	    /// be stroked, filled, used as a clipping boundary, or some combination of the three.
	    /// See Section 5.2.5 in PDF Reference Manual for details..
	    /// 
	    /// </summary>
	    /// <param name="rmode">the new text render mode
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetTextRenderMode(TextRenderingMode rmode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetTextRenderMode(mp_state, rmode));
        }
	    /// <summary> Sets text rise.
	    /// Text rise specifies the distance, in unscaled text space units, to move the
	    /// baseline up or down from its default location. Positive values of text rise
	    /// move the baseline up
	    /// 
	    /// </summary>
	    /// <param name="rise">the new text rise
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetTextRise(double rise)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetTextRise(mp_state, rise));
        }
	    /// <summary> Mark the object as elementary for purposes of color compositing in the
	    /// transparent imaging model.
	    /// 
	    /// </summary>
	    /// <param name="knockout">the new text knockout
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetTextKnockout(Boolean knockout)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetTextKnockout(mp_state, knockout));
        }
	    /// <summary> Sets the color intent to be used for rendering the Element.
	    /// 
	    /// </summary>
	    /// <param name="intent">the new rendering intent
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetRenderingIntent(RenderingIntent intent)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_GStateSetRenderingIntent(mp_state, intent));
        }

        // Nested Types
        ///<summary>The standard separable blend modes available in PDF.</summary>
        public enum BlendMode
        {
            //TODO: enum values missing description
            e_bl_compatible,
            e_bl_normal,
            e_bl_multiply,
            e_bl_screen,
            e_bl_difference,
            e_bl_darken,
            e_bl_lighten,
            e_bl_color_dodge,
            e_bl_color_burn,
            e_bl_exclusion,
            e_bl_hard_light,
            e_bl_overlay,
            e_bl_soft_light,
            e_bl_luminosity,
            e_bl_hue,
            e_bl_saturation,
            e_bl_color
        }

        /// <summary>GState properties</summary>
        public enum GStateAttribute
        {
            //TODO: enum values missing description
            e_transform,
            e_rendering_intent,
            e_stroke_cs,
            e_stroke_color,
            e_fill_cs,
            e_fill_color,
            e_line_width,
            e_line_cap,
            e_line_join,
            e_flatness,
            e_miter_limit,
            e_dash_pattern,
            e_char_spacing,
            e_word_spacing,
            e_horizontal_scale,
            e_leading,
            e_font,
            e_font_size,
            e_text_render_mode,
            e_text_rise,
            e_text_knockout,
            e_text_pos_offset,
            e_blend_mode,
            e_opacity_fill,
            e_opacity_stroke,
            e_alpha_is_shape,
            e_soft_mask,
            e_smoothnes,
            e_auto_stoke_adjust,
            e_stroke_overprint,
            e_fill_overprint,
            e_overprint_mode,
            e_transfer_funct,
            e_BG_funct,
            e_UCR_funct,
            e_halftone,
            e_null
        }

        ///<summary>LineCap types</summary>
	    public enum LineCap
        {
            ///<summary>The stroke is squared off at the endpoint of the path.</summary>
		    e_butt_cap = 0,
		    ///<summary>A semicircular arc with a diameter equal to the line width.</summary>
		    e_round_cap,
		    ///<summary>squared off stroke continues beyond the endpoint of the path.</summary>
		    e_square_cap
        }

        ///<summary>LineJoin types</summary>
        public enum LineJoin
        {
            ///<summary>The two segments are extended until they meet</summary>
		    e_miter_join = 0,
		    ///<summary>A circle with a diameter equal to the line width</summary>
		    e_round_join,
		    ///<summary>The two segments are finished with butt caps</summary>
		    e_bevel_join
        }

        //TODO: enum documentation missing
        public enum RenderingIntent
        {
            e_absolute_colorimetric,
            e_relative_colorimetric,
            e_saturation,
            e_perceptual
        }

        ///<summary>Text Rendering modes</summary>
        public enum TextRenderingMode
        {
            ///<summary>Fill text.</summary>
		    e_fill_text = 0,
		    ///<summary>Stroke text.</summary>
		    e_stroke_text,
		    ///<summary>Fill, then stroke text.</summary>
		    e_fill_stroke_text,
		    ///<summary>Neither fill nor stroke text (invisible).</summary>
		    e_invisible_text,
		    ///<summary>Fill text and add to path for clipping (see above).</summary>
		    e_fill_clip_text,
		    ///<summary>Stroke text and add to path for clipping.</summary>
		    e_stroke_clip_text,
		    ///<summary>Fill, then stroke text and add to path for clipping.</summary>
		    e_fill_stroke_clip_text,
		    ///<summary>Add text to path for clipping.</summary>
		    e_clip_text
        }

    }
}
