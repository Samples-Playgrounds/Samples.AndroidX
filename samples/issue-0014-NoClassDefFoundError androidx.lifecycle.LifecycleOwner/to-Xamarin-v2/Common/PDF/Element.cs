using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;

using pdftron.Common;
using pdftron.SDF;
using pdftron.PDF;
using pdftron.Filters;
using pdftronprivate.trn;

using TRN_Element = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_GState = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_Filter = System.IntPtr;
using TRN_GDIPlusBitmap = System.IntPtr;
using TRN_ColorSpace = System.IntPtr;
using TRN_Shading = System.IntPtr;
using TRN_Iterator = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> Element is the abstract interface used to access graphical elements used to build the 
    /// display list. 
    /// 
    /// Just like many other classes in PDFNet (e.g. ColorSpace, Font, Annot, etc), Element
    /// class follows the composite design pattern. This means that all Elements are 
    /// accessed through the same interface, but depending on the Element type (that can be 
    /// obtained using GetType()), only methods related to that type can be called.
    /// For example, if GetType() returns e_image, it is illegal to call a method specific to 
    /// another Element type (i.e. a call to a text specific GetTextData() will throw an 
    /// Exception).
    /// </summary>
    public class Element
    {
        internal TRN_Element mp_elem = IntPtr.Zero;
        internal Object m_ref;
        internal Object m_doc_ref;

        internal Element(TRN_Element impl, Object reference, Object doc_reference)
        {
            this.mp_elem = impl;
            this.m_ref = reference;
            this.m_doc_ref = doc_reference;
        }

        // Methods
        /// <summary> Gets the type.
	    /// 
	    /// </summary>
	    /// <returns> the current element type.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public new Type GetType()
        {
            Type result = Type.e_null;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetType(mp_elem, ref result));
            return result;
        }
	    /// <summary> Gets the g state.
	    /// 
	    /// </summary>
	    /// <returns> GState of this Element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public GState GetGState()
        {
            TRN_GState result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetGState(mp_elem, ref result));
            return new GState(result, this.m_ref, this.m_doc_ref);
        }
	    /// <summary> Gets the cTM.
	    /// 
	    /// </summary>
	    /// <returns> current transformation matrix (ctm) that maps coordinates to the
	    /// initial user space.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Matrix2D GetCTM()
        {
            BasicTypes.TRN_Matrix2D result = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetCTM(mp_elem, ref result));
            return new Matrix2D(result);
        }
	    /// <summary> Obtains the bounding box for a graphical element.
	    /// 
	    /// Calculates the bounding box for a graphical element (i.e. an Element that belongs
	    /// to one of following types: e_path, e_text, e_image, e_inline_image, e_shading e_form).
	    /// The returned bounding box is guaranteed to encompass the Element, but is not guaranteed
	    /// to be the smallest box that could contain the element. For example, for Bezier curves
	    /// the bounding box will enclose all control points, not just the curve itself.
	    /// 
	    /// </summary>
	    /// <param name="bbox">(Filled by the method) A reference to a rectangle specifying the bounding box of 
	    /// Element (a rectangle that surrounds the entire element). The coordinates are represented in the default 
	    /// PDF page coordinate system and are using units called points ( 1 point = 1/72 inch = 2.54 /72 centimeter). 
	    /// The bounding box already accounts for the effects of current transformation matrix (CTM), text matrix, 
	    /// font size, and other properties in the graphics state. If this is a non-graphical element (i.e. the method 
	    /// returns false) the bounding box is undefined.
	    /// </param>
	    /// <returns> Rect if this is a graphical element and the bounding box can be calculated;
	    /// null for non-graphical elements which don't have bounding box.
	    /// The coordinates are represented in the default PDF page coordinate system and are using units called
	    /// points ( 1 point = 1/72 inch =  2.54 /72 centimeter). The bounding box already accounts for the
	    /// effects of current transformation matrix (CTM), text matrix, font size, and other properties
	    /// in the graphics state. If this is a non-graphical element (i.e. the method returns false) the
	    /// bounding box is undefined.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool GetBBox(Rect bbox)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetBBox(mp_elem, ref bbox.mp_imp, ref result));
            return result;
        }

	    // Path Element (e_path) Get Methods --------------------------------------------
	    /// <summary> Checks if is clipping path.
	    /// 
	    /// </summary>
	    /// <returns> true if the current path element is a clipping path and should be added
	    /// to clipping path stack.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsClippingPath() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementIsClippingPath(mp_elem, ref result));
            return result;
        }
	    /// <summary> Checks if is stroked.
	    /// 
	    /// </summary>
	    /// <returns> true if the current path element should be stroked
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsStroked() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementIsStroked(mp_elem, ref result));
            return result;
        }
	    /// <summary> Checks if is filled.
	    /// 
	    /// </summary>
	    /// <returns> true if the current path element should be filled
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsFilled() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementIsFilled(mp_elem, ref result));
            return result;
        }
	    /// <summary> Checks if is winding fill.
	    /// 
	    /// </summary>
	    /// <returns> true if the current path should be filled using non-zero winding rule,
	    /// or false if the path should be filled using even-odd rule.
	    /// 
	    /// According non-zero winding rule, you can determine whether a test point is inside or
	    /// outside a closed curve as follows: Draw a line from a test point to a point that
	    /// is distant from the curve. Count the number of times the curve crosses the test
	    /// line from left to right, and count the number of times the curve crosses the test
	    /// line from right to left. If those two numbers are the same, the test point is
	    /// outside the curve; otherwise, the test point is inside the curve.
	    /// 
	    /// According to even-odd rule, you can determine whether a test point is inside
	    /// or outside a closed curve as follows: Draw a line from the test point to a point
	    /// that is distant from the curve. If that line crosses the curve an odd number of
	    /// times, the test point is inside the curve; otherwise, the test point is outside
	    /// the curve.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsWindingFill() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementIsWindingFill(mp_elem, ref result));
            return result;
        }
	    /// <summary> Checks if is clip winding fill.
	    /// 
	    /// </summary>
	    /// <returns> true if the current clip path is using non-zero winding rule, or false
	    /// for even-odd rule.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsClipWindingFill() 
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementIsClipWindingFill(mp_elem, ref result));
            return result;
        }
	    /// <summary> Checks if is oC visible.
	    /// 
	    /// </summary>
	    /// <returns> true if this element is visible in the optional-content
	    /// context (OCG::Context). The method considers the context's current OCMD stack,
	    /// the group ON-OFF states, the non-OC drawing status, the drawing and enumeration mode,
	    /// and the intent.
	    /// 
	    /// When enumerating page content, OCG::Context can be passed as a parameter in
	    /// ElementReader.Begin() method. When using PDFDraw, PDFRasterizer or PDFViewCtrl class to
	    /// render PDF pages use SetOCGContext() method to select an OC context.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsOCVisible() {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementIsOCVisible(mp_elem, ref result));
            return result;
        }

	    /// <summary> Gets the parent logical structure element
	    /// 
	    /// </summary>
	    /// <returns>Parent logical structure element &#40;such as 'span' or 'paragraph'&#41;. If the Element is not associated with any structure element, the returned SElement will not be valid &#40;i.e. selem.IsValid&#40;&#41; -&#62; false&#41;.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Struct.SElement GetParentStructElement() {
            BasicTypes.TRN_SElement result = new BasicTypes.TRN_SElement();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetParentStructElement(mp_elem, ref result));
            return new Struct.SElement(result, this);
        }
	    /// <summary> Gets the struct MCID.
	    /// 
	    /// </summary>
	    /// <returns> Marked Content Identifier (MCID) for this Element or
	    /// a negative number if the element is not assigned an identifier/MCID.
	    /// 
	    /// Marked content identifier can be used to associate an Element with
	    /// logical structure element that refers to the Element.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetStructMCID() {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetStructMCID(mp_elem, ref result));
            return result;
        }
	    /// <summary> Get the PathData stored by the path element.
	    /// 
	    /// </summary>
	    /// <returns> The PathData which contains the operators and corresponding point data.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public PathData GetPathData()
        {
            PathData data = new PathData();

            int size = 0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetPathTypesCount(mp_elem, ref size));
            IntPtr source = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetPathTypes(mp_elem, ref source));

            byte[] buf = new byte[size];
            if (size > 0)
            {
                System.Runtime.InteropServices.Marshal.Copy(source, buf, 0, size);
            }
            
            data.operators = buf;

            int dataSize = 0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetPathPointCount(mp_elem, ref dataSize));
            source = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetPathPoints(mp_elem, ref source));

            double[] arr2 = new double[dataSize];
            if (dataSize > 0)
            {
                System.Runtime.InteropServices.Marshal.Copy(source, arr2, 0, dataSize);
            }
            
            data.points = arr2;

            return data;
        }

	    // Path Element (e_path) Set Methods --------------------------------------------
	    /// <summary> Indicate whether the path is a clipping path or non-clipping path.
	    /// 
	    /// </summary>
	    /// <param name="clip">the new path clip
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPathClip(bool clip) {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementSetPathClip(mp_elem, clip));
        }
	    /// <summary> Indicate whether the path should be stroked.
	    /// 
	    /// </summary>
	    /// <param name="stroke">the new path stroke
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPathStroke(bool stroke) {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementSetPathStroke(mp_elem, stroke));
        }
	    /// <summary> Indicate whether the path should be filled.
	    /// 
	    /// </summary>
	    /// <param name="fill">the new path fill
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPathFill(bool fill)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementSetPathFill(mp_elem, fill));
        }
	    /// <summary> Sets path's fill rule.
	    /// 
	    /// </summary>
	    /// <param name="winding_rule">if winding_rule is true path will be filled using non-zero
	    /// winding fill rule, otherwise even-odd fill will be used.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetWindingFill(bool winding_rule) {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementSetWindingFill(mp_elem, winding_rule));
        }
	    /// <summary> Sets clipping path's fill rule.
	    /// 
	    /// </summary>
	    /// <param name="winding_rule">if winding_rule is true clipping should use non-zero
	    /// winding rule, or false for even-odd rule.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetClipWindingFill(bool winding_rule) {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementSetClipWindingFill(mp_elem, winding_rule));
        }
	    /// <summary> Set the PathData of this element. The PathData contains the array of points
	    /// stored by the element and the array of path segment types.  
	    /// 
	    /// </summary>
	    /// <param name="data">The new path data.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPathData(PathData data)
        {
            int psize = Marshal.SizeOf(data.operators[0]) * data.operators.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            try
            {
                Marshal.Copy(data.operators, 0, pnt, data.operators.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ElementSetPathTypes(mp_elem, pnt, data.operators.Length));
                PDFNetException.REX(PDFNetPINVOKE.TRN_ElementSetPathPoints(mp_elem, data.points, data.points.Length));
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }
	    // XObject Element (e_image or e_form_begin) Methods ----------------------------
	    /// <summary> Gets the x object.
	    /// 
	    /// </summary>
	    /// <returns> the SDF object of the Image/Form object.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetXObject() {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetXObject(mp_elem, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_doc_ref) : null;
        }
	    // Image (e_image and e_inline_image) Element Methods ---------------------------
	    /// <summary> Gets the image data.
	    /// 
	    /// </summary>
	    /// <returns> A stream (filter) containing decoded image data
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Filter GetImageData() {
            TRN_Filter result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetImageData(mp_elem, ref result));
            return new Filter(result, null);
        }
	    /// <summary> Gets the image data size.
	    /// 
	    /// </summary>
	    /// <returns> the size of image data in bytes
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetImageDataSize() {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetImageDataSize(mp_elem, ref result));
            return result;
        }

        /// <summary> Gets the image color space.
	    /// 
	    /// </summary>
	    /// <returns> The SDF object representing the color space in which image
	    /// are specified or NULL if:
	    /// - the image is an image mask
	    /// - or is compressed using JPXDecode with missing ColorSpace entry in image dictionary.
	    /// 
	    /// The returned color space may be any type of color space except Pattern.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ColorSpace GetImageColorSpace() {
            TRN_ColorSpace result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetImageColorSpace(mp_elem, ref result));
            return new ColorSpace(result, this.m_doc_ref);
        }
	    /// <summary> Gets the image width.
	    /// 
	    /// </summary>
	    /// <returns> the width of the image, in samples.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetImageWidth() {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetImageWidth(mp_elem, ref result));
            return result;
        }
	    /// <summary> Gets the image height.
	    /// 
	    /// </summary>
	    /// <returns> the height of the image, in samples.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetImageHeight() {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetImageHeight(mp_elem, ref result));
            return result;
        }
	    /// <summary> Gets the decode array.
	    /// 
	    /// </summary>
	    /// <returns> Decode array or NULL if the paramter is not specified. A decode object is an
	    /// array of numbers describing how to map image samples into the range of values
	    /// appropriate for the image’s color space . If ImageMask is true, the array must be
	    /// either [0 1] or [1 0]; otherwise, its length must be twice the number of color
	    /// components required by ColorSpace. Default value depends on the color space,
	    /// See Table 4.36 in PDF Ref. Manual.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetDecodeArray() {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetDecodeArray(mp_elem, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_doc_ref) : null;
        }
	    /// <summary> Gets the bits per component.
	    /// 
	    /// </summary>
	    /// <returns> the number of bits used to represent each color component. Only a
	    /// single value may be specified; the number of bits is the same for all color
	    /// components. Valid values are 1, 2, 4, and 8.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetBitsPerComponent()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetBitsPerComponent(mp_elem, ref result));
            return result;
        }
	    /// <summary> Gets the component number.
	    /// 
	    /// </summary>
	    /// <returns> the number of color components per sample.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public int GetComponentNum()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetComponentNum(mp_elem, ref result));
            return result;
        }
	    /// <summary> Checks if is image mask.
	    /// 
	    /// </summary>
	    /// <returns> a boolean indicating whether the inline image is to be treated as an image mask.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsImageMask()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementIsImageMask(mp_elem, ref result));
            return result;
        }
	    /// <summary> Checks if is image interpolate.
	    /// 
	    /// </summary>
	    /// <returns> a boolean indicating whether image interpolation is to be performed.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool IsImageInterpolate()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementIsImageInterpolate(mp_elem, ref result));
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetMask(mp_elem, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_doc_ref) : null;
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
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetImageRenderingIntent(mp_elem, ref result));
            return result;
        }

	    // Text Element (e_text) Get Methods ------------------------------------------------
	    /// <summary> Gets the text string.
	    /// 
	    /// </summary>
	    /// <returns> a pointer to Unicode string for this text Element. The
	    /// function maps character codes to Unicode array defined by Adobe
	    /// Glyph List (http://partners.adobe.com/asn/developer/type/glyphlist.txt).
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>In PDF text can be encoded using various encoding schemes 
	    /// and in some cases it is not possible to extract Unicode encoding.
	    /// If it is not possible to map charcode to Unicode the function will
	    /// map a character to undefined code, 0xFFFD. This code is defined in
	    /// private Unicode range.
	    /// 
	    /// If you would like to map raw text to Unicode (or some other encoding)
	    /// yourself use CharIterators returned by CharBegin()/CharEnd() and
	    /// PDF::Font code mapping methods.
	    /// 
	    /// The string owner is the current element (i.e. ElementReader or ElementBuilder).
	    /// </remarks>
        public string GetTextString()
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetTextString(mp_elem, ref result.mp_impl));
            return result.ConvToManagedStr();
        }
	    //Byte GetTextData() ref[];
	    /// <summary> Gets the text data.
	    /// 
	    /// </summary>
	    /// <returns> a pointer to the internal text buffer for this text element.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>GetTextData() returns the raw text data and not a Unicode string.
	    /// In PDF text can be encoded using various encoding schemes so it is necessary
	    /// to consider Font encoding while processing the content of this buffer.		
	    /// Most of the time GetTextString() is what you are looking for instead.</remarks>  
        public byte[] GetTextData()
        {
            //byte[] result = new byte[GetTextDataSize()];
            int size = GetTextDataSize();
            IntPtr source = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetTextData(mp_elem, ref source));
            byte[] buf = new byte[size];
            System.Runtime.InteropServices.Marshal.Copy(source, buf, 0, System.Convert.ToInt32(size));
            return buf;
        }
	    /// <summary>Gets the size of the internal text buffer returned in GetTextData().
	    /// </summary>
	    /// <returns>the size of the internal text buffer returned in GetTextData().
	    /// </returns>
        public int GetTextDataSize()
        {
            uint result = uint.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetTextDataSize(mp_elem, ref result));
            return System.Convert.ToInt32(result);
        }
	    /// <summary> Gets the text matrix.
	    /// 
	    /// </summary>
	    /// <returns> a reference to the current text matrix (Tm).
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Matrix2D GetTextMatrix()
        {
            BasicTypes.TRN_Matrix2D result = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetTextMatrix(mp_elem, ref result));
            return new Matrix2D(result);
        }

	    /// <summary> Gets the char iterator.
	    /// 
	    /// </summary>
	    /// <returns> a CharIterator addressing the first CharData element in the text run.
	    /// 
	    /// CharIterator points to CharData. CharData is a data structure that contains
	    /// the char_code number (used to retrieve glyph outlines, to map to Unicode, etc.),
	    /// character positioning information (x, y), and the number of bytes taken by the
	    /// character within the text buffer.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>
	    /// CharIterator follows the standard STL forward-iterator interface.        
	    /// <example>
	    /// An example of how to use CharIterator.		
	    /// <code>  
	    /// for (CharIterator itr = element.GetCharIterator(); itr.HasNext(); itr.Next()) {
	    /// unsigned int char_code = itr.Current().char_code;
	    /// double char_pos_x = itr.Current().x;
	    /// double char_pos_y = itr.Current().y;
	    /// }
	    /// </code>				
	    /// </example>
	    /// Character positioning information (x, y) is represented in text space. 
	    /// 
	    /// In order to get the positioning in the user space, the returned value should
	    /// be scaled using the text matrix (GetTextMatrix()) and the current transformation
	    /// matrix (GetCTM()). See section 4.2 'Other Coordinate Spaces' in PDF Reference
	    /// Manual for details and PDFNet FAQ - "How do I get absolute/relative text and
	    /// character positioning?".
	    /// 
	    /// whithin a text run a character may occupy more than a single byte (e.g. 
	    /// in case of composite/Type0 fonts). The role of CharIterator/CharData is to
	    /// provide a uniform and easy to use inteface to access character information.		
	    /// </remarks>
        public CharIterator GetCharIterator()
        {
            TRN_Iterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetCharIterator(mp_elem, ref result));
            return new CharIterator(result);
        }

	    /// <summary> Gets the text length.
	    /// 
	    /// </summary>
	    /// <returns> The text advance distance in text space.
	    /// 
	    /// The total sum of all of the advance values from rendering all of the characters
	    /// within this element, including the advance value on the glyphs, the effect of
	    /// properties such as 'char-spacing', 'word-spacing' and positioning adjustments
	    /// on 'TJ' elements.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks> Computed text length is represented in text space. In order to get the
	    /// length of the text run in the user space, the returned value should be scaled
	    /// using the text matrix (GetTextMatrix()) and the current transformation
	    /// matrix (GetCTM()). See section 4.2 'Other Coordinate Spaces' in PDF Reference
	    /// Manual for details.</remarks> 
        public double GetTextLength()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetTextLength(mp_elem, ref result));
            return result;
        }
	    /// <summary> Gets the pos adjustment.
	    /// 
	    /// </summary>
	    /// <returns> The number used to adjust text matrix in horizontal direction when drawing
	    /// text. The number is expressed in thousandths of a unit of text space. The returned
	    /// number corresponds to a number value within TJ array. For 'Tj' text strings the
	    /// returned value is always 0.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  because CharIterator positioning information already accounts for TJ
	    /// adjustments this method is rarely used.
	    /// </remarks>
        public double GetPosAdjustment()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetPosAdjustment(mp_elem, ref result));
            return result;
        }
	    /// <summary> Checks for text matrix.
	    /// 
	    /// </summary>
	    /// <returns> true if this element is directly associated with a text matrix
	    /// (that is Tm operator is just before this text element) or false if the text
	    /// matrix is default or is inherited from previous text elements.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool HasTextMatrix()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementHasTextMatrix(mp_elem, ref result));
            return result;
        }

	    // Text New Line Element (e_text_new_line) Methods ------------------------------
	    /// <summary> Gets the offset (out_x, out_y) to the start of the current line relative to
	    /// the beginning of the previous line.
	    /// 
	    /// out_x and out_y are numbers expressed in unscaled text space units.
	    /// The returned numbers correspond to the arguments of 'Td' operator.
	    /// 
	    /// </summary>
	    /// <param name="out_x">x coordinate of the text line offset
	    /// </param>
	    /// <param name="out_y">y coordinate of the text line offset
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void GetNewTextLineOffset(ref double out_x, ref double out_y)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetNewTextLineOffset(mp_elem, ref out_x, ref out_y));
        }

	    // Text Element (e_text) Set Methods ------------------------------------------------

	    //void SetTextData(const Byte text_data[], int text_data_size);
	    /// <summary> GetTextString() maps the raw text directly into Unicode (as specified by Adobe
	    /// Glyph List (AGL) ). Even if you would prefer to decode text yourself it is more
	    /// convenient to use CharIterators returned by CharBegin()/CharEnd() and
	    /// PDF::Font code mapping methods.
	    /// </summary>		
	    /// <param name="text_data">a pointer to a buffer containing text.
	    /// </param>
	    /// <param name="text_data_size">size of the cli::array</param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  the buffer owner is the current element (i.e. ElementReader or ElementBuilder).
        ///  Set the text data for the current e_text Element.</remarks>
        public void SetTextData(byte[] text_data, int text_data_size)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementSetTextData(mp_elem, text_data, text_data_size));
        }

	    /// <summary> Sets the text matrix for a text element.
	    /// 
	    /// </summary>
	    /// <param name="mtx">The new text matrix for this text element
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetTextMatrix(Matrix2D mtx)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementSetTextMatrix(mp_elem, ref mtx.mp_mtx));
        }
	    /// <summary> Sets the text matrix for a text element. This method accepts text
	    /// transformation matrix components directly.
	    /// 
	    /// A transformation matrix in PDF is specified by six numbers, usually
	    /// in the form of an array containing six elements. In its most general
	    /// form, this array is denoted [a b c d h v]; it can represent any linear
	    /// transformation from one coordinate system to another. For more
	    /// information about PDF matrices please refer to section 4.2.2 'Common
	    /// Transformations' in PDF Reference Manual, and to documentation for
	    /// Matrix2D class.
	    /// 
	    /// </summary>
	    /// <param name="a">- horizontal 'scaling' component of the new text matrix.
	    /// </param>
	    /// <param name="b">- 'rotation' component of the new text matrix.
	    /// </param>
	    /// <param name="c">- 'rotation' component of the new text matrix.
	    /// </param>
	    /// <param name="d">- vertical 'scaling' component of the new text matrix.
	    /// </param>
	    /// <param name="h">- horizontal translation component of the new text matrix.
	    /// </param>
	    /// <param name="v">- vertical translation component of the new text matrix.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetTextMatrix(double a, double b, double c, double d, double h, double v)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementSetTextMatrixEntries(mp_elem, a, b, c, d, h, v));
        }
	    /// <summary> Sets the pos adjustment.
	    /// 
	    /// </summary>
	    /// <param name="adjust">the new pos adjustment
	    /// </param>
	    /// <returns> Set the horizontal adjustment factor (i.e. a number value within TJ array)
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPosAdjustment(double adjust)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementSetPosAdjustment(mp_elem, adjust));
        }
	    /// <summary> Recompute the character positioning information (i.e. CharIterator-s) and
	    /// text length.
	    /// 
	    /// Element objects caches text length and character positioning information.
	    /// If the user modifies the text data or graphics state the cached information
	    /// is not correct. UpdateTextMetrics() can be used to recalculate the correct
	    /// positioning and length information.
	    /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void UpdateTextMetrics()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementUpdateTextMetrics(mp_elem));
        }

	    // Text New Line Element (e_text_new_line) Methods ------------------------------
	    /// <summary> Sets the offset (dx, dy) to the start of the current line relative to the beginning
	    /// of the previous line.
	    /// 
	    /// </summary>
	    /// <param name="dx">the dx
	    /// </param>
	    /// <param name="dy">the dy
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetNewTextLineOffset(double dx, double dy)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementSetNewTextLineOffset(mp_elem, dx, dy));
        }

	    // Shading Element (e_shading) Methods ------------------------------------------
	    /// <summary> Gets the shading.
	    /// 
	    /// </summary>
	    /// <returns> the SDF object of the Shading object.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Shading GetShading()
        {
            TRN_Shading result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetShading(mp_elem, ref result));
            return new Shading(result, this.m_doc_ref);
        }

	    // Marked Content Methods -------------------------------------------------------
	    /// <summary> Gets the mC property dict.
	    /// 
	    /// </summary>
	    /// <returns> a dictionary containing the property list or NULL if property
	    /// dictionary is not present.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>the function automatically looks under Properties sub-dictionary of the
        /// current resource dictionary if the dictionary is not in-line. Therefore you
        /// can assume that returned Obj is dictionary if it is not NULL.</remarks>  
        public Obj GetMCPropertyDict()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetMCPropertyDict(mp_elem, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_doc_ref) : null;
        }
	    /// <summary> Gets the mC tag.
	    /// 
	    /// </summary>
	    /// <returns> a tag is a name object indicating the role or significance of
	    /// the marked content point/sequence.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetMCTag()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementGetMCTag(mp_elem, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_doc_ref) : null;
        }

        // Nested Types
        /// <summary><c>Element</c> types
        /// </summary>
        public enum Type
        {
            /// <summary>undefined element type
            /// </summary>
            e_null,
            /// <summary>path object
            /// </summary>
            e_path,
            /// <summary>marks the beginning of a text container
            /// </summary>
            e_text_begin,
            /// <summary>text object within a text container
            /// </summary>
            e_text,
            /// <summary>indicates the start of the new text line
            /// </summary>
            e_text_new_line,
            /// <summary>marks the end of text container
            /// </summary>
            e_text_end,
            /// <summary>image XObject
            /// </summary>
            e_image,
            /// <summary>inline image object
            /// </summary>
            e_inline_image,
            /// <summary>shading object
            /// </summary>
            e_shading,
            /// <summary>a form XObject 
            /// </summary>
            e_form,
            /// <summary>push graphics state operator (q)
            /// </summary>
            e_group_begin,
            /// <summary>pop graphics state operator (Q)
            /// </summary>
            e_group_end,
            /// <summary>marks the beginning of marked content sequence (BMC, BDC)
            /// </summary>
            e_marked_content_begin,
            /// <summary>marks the end of marked content sequence (EMC)
            /// </summary>
            e_marked_content_end,
            /// <summary>designate a marked-content point (MP, DP)
            /// </summary>
            e_marked_content_point
        }

    }
}
