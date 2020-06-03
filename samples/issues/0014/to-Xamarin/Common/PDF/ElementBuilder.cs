using System;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

using pdftron.Common;
using pdftron.SDF;

using TRN_ElementBuilder = System.IntPtr;
using TRN_Element = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> ElementBuilder is used to build new PDF.Elements (e.g. image, text, path, etc) 
    /// from scratch. In conjunction with ElementWriter, ElementBuilder can be used to create
    /// new page content.
    /// 
    /// </summary>
    /// <remarks>  Analogous to ElementReader, every call to ElementBuilder.Create? method destroys
    /// the Element currently associated with the builder and all previous Element pointers are 
    /// invalidated. </remarks>	
    public class ElementBuilder : IDisposable
    {
        internal TRN_ElementBuilder mp_builder = IntPtr.Zero;
        internal ElementBuilder(TRN_ElementBuilder imp)
        {
            this.mp_builder = imp;
        }
        /// <summary> Instantiates a new element builder.
	    /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public ElementBuilder()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreate(ref mp_builder));
        }

	    /// <summary> The function sets the graphics state of this Element to the given value.
	    /// If 'gs' parameter is not specified or is NULL the function resets the
	    /// graphics state of this Element to the default graphics state (i.e. the
	    /// graphics state at the begining of the display list).
	    /// 
	    /// The function can be used in situations where the same ElementBuilder is used
	    /// to create content on several pages, XObjects, etc. If the graphics state is not
	    /// Reset() when moving to a new display list, the new Element will have the same
	    /// graphics state as the last Element in the previous display list (and this may
	    /// or may not be your intent).
	    /// 
	    /// Another use of Reset(gs) is to make sure that two Elements have the graphics
	    /// state.
	    /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Reset()
        {
            Reset(null);
        }
	    /// <summary> Reset.
	    /// 
	    /// </summary>
	    /// <param name="gs">the gs
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Reset(GState gs)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderReset(mp_builder, gs == null ? IntPtr.Zero : gs.mp_state));
        }

	    // Image Element ------------------------------------------------
	    /// <summary> Create a content image Element out of a given document Image.
	    /// 
	    /// </summary>
	    /// <param name="img">the img
	    /// </param>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateImage(Image img)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateImage(mp_builder, img.mp_image, ref result));
            return new Element(result, this, img.m_ref);
        }
	    /// <summary> Create a content image Element out of a given document Image.
	    /// 
	    /// </summary>
	    /// <param name="img">the img
	    /// </param>
	    /// <param name="mtx">the image transformation matrix.
	    /// </param>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateImage(Image img, Matrix2D mtx)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateImageFromMatrix(mp_builder, img.mp_image, ref mtx.mp_mtx, ref result));
            return new Element(result, this, img.m_ref);
        }
	    /// <summary> Create a content image Element out of a given document Image with
	    /// the lower left corner at (x, y), and scale factors (hscale, vscale).
	    /// 
	    /// </summary>
	    /// <param name="img">the img
	    /// </param>
	    /// <param name="x">the x
	    /// </param>
	    /// <param name="y">the y
	    /// </param>
	    /// <param name="hscale">the hscale
	    /// </param>
	    /// <param name="vscale">the vscale
	    /// </param>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateImage(Image img, Double x, Double y, Double hscale, Double vscale)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateImageScaled(mp_builder, img.mp_image, x, y, hscale, vscale, ref result));
            return new Element(result, this, img.m_ref);
        }
	    /// <summary> Create e_group_begin Element (i.e. 'q' operator in PDF content stream).
	    /// The function saves the current graphics state.
	    /// 
	    /// </summary>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateGroupBegin()
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateGroupBegin(mp_builder, ref result));
            return new Element(result, this, null);
        }
	    /// <summary> Create e_group_end Element (i.e. 'Q' operator in PDF content stream).
	    /// The function restores the previous graphics state.
	    /// 
	    /// </summary>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateGroupEnd()
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateGroupEnd(mp_builder, ref result));
            return new Element(result, this, null);
        }
	    /// <summary> Create a shading Element.
	    /// 
	    /// </summary>
	    /// <param name="sh">the sh
	    /// </param>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateShading(Shading sh)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateShading(mp_builder, sh.mp_shade, ref result));
            return new Element(result, this, sh.m_ref);
        }
	    /// <summary> Create a Form XObject Element.
	    /// 
	    /// </summary>
	    /// <param name="form">a Form XObject content stream
	    /// </param>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateForm(Obj form)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateFormFromStream(mp_builder, form.mp_obj, ref result));
            return new Element(result, this, form.GetRefHandleInternal());
        }
	    /// <summary> Create a Form XObject Element using the content of the existing page.
	    /// This method assumes that the XObject will be used in the same
	    /// document as the given page. If you need to create the Form XObject
	    /// in a different document use CreateForm(Page, Doc) method.
	    /// 
	    /// </summary>
	    /// <param name="page">A page used to create the Form XObject.
	    /// </param>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateForm(Page page)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateFormFromPage(mp_builder, page.mp_page, ref result));
            return new Element(result, this, page.m_ref);
        }
	    /// <summary> Create a Form XObject Element using the content of the existing page.
	    /// Unlike CreateForm(Page) method, you can use this method to create form
	    /// in another document.
	    /// 
	    /// </summary>
	    /// <param name="page">A page used to create the Form XObject.
	    /// </param>
	    /// <param name="doc">Destination document for the Form XObject.
	    /// </param>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateForm(Page page, PDFDoc doc)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateFormFromDoc(mp_builder, page.mp_page, doc.mp_doc, ref result));
            return new Element(result, this, doc);
        }
	    /// <summary> Start a text block ('BT' operator in PDF content stream).
	    /// The function installs the given font in the current graphics state.
	    /// 
	    /// </summary>
	    /// <param name="font">the font
	    /// </param>
	    /// <param name="font_sz">the font_sz
	    /// </param>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateTextBegin(Font font, Double font_sz)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateTextBeginWithFont(mp_builder, font.mp_font, font_sz, ref result));
            return new Element(result, this, font.m_ref);
        }
	    /// <summary> Start a text block ('BT' operator in PDF content stream).
	    /// 
	    /// </summary>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateTextBegin()
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateTextBegin(mp_builder, ref result));
            return new Element(result, this, null);
        }
	    /// <summary> Ends a text block.
	    /// 
	    /// </summary>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateTextEnd()
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateTextEnd(mp_builder, ref result));
            return new Element(result, this, null);
        }
	    /// <summary> Create a text run using the given font.
	    /// 
	    /// </summary>
	    /// <param name="text_data">the text_data
	    /// </param>
	    /// <param name="font">the font
	    /// </param>
	    /// <param name="font_sz">the font_sz
	    /// </param>
	    /// <returns> the element
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  a text run can be created only within a text block </remarks>
        public Element CreateTextRun(String text_data, Font font, Double font_sz)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateTextRun(mp_builder, text_data, font.mp_font, font_sz, ref result));
            return new Element(result, this, font.m_ref);
        }
	    /// <summary> Create a new text run.
	    /// 
	    /// </summary>
	    /// <param name="text_data">the text_data
	    /// </param>
	    /// <returns> the element
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  a text run can be created only within a text block
        /// you must set the current Font and font size before calling this function. </remarks>
        public Element CreateTextRun(String text_data)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateNewTextRun(mp_builder, text_data, ref result));
            return new Element(result, this, null);
        }

	    /// <summary> Create a new Unicode text run.
	    /// 
	    /// </summary>
	    /// <param name="text_data">the text_data
	    /// </param>
	    /// <returns> the element
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks> you must set the current Font and font size before calling this function
        /// and the font must be created using Font.CreateCIDTrueTypeFont() method.
        /// a text run can be created only within a text block </remarks>
        public Element CreateUnicodeTextRun(String text_data)
        {
            TRN_Element result = IntPtr.Zero;
            IntPtr text_data_ptr = Marshal.StringToHGlobalUni(text_data);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateUnicodeTextRun(mp_builder, text_data_ptr, System.Convert.ToUInt32(text_data.ToCharArray().Length), ref result));
            return new Element(result, this, null);
        }

	    /// <summary> Create a new Unicode text run.
	    /// 
	    /// </summary>
	    /// <param name="text_data">the text_data
	    /// </param>
	    /// <param name="font">font for the text run
	    /// </param>
	    /// <param name="font_sz">size of the font
	    /// </param>
	    /// <returns> the element
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks> you must set the current Font and font size before calling this function
        /// and the font must be created using Font.CreateCIDTrueTypeFont() method.
        /// a text run can be created only within a text block </remarks>
        public Element CreateUnicodeTextRun(String text_data, Font font, Double font_sz)
        {
            TRN_Element result = IntPtr.Zero;
            IntPtr text_data_ptr = Marshal.StringToHGlobalUni(text_data);
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateUnicodeTextRun(mp_builder, text_data_ptr, System.Convert.ToUInt32(text_data.ToCharArray().Length), ref result));
            return new Element(result, this, null);
        }

	    /// <summary> Create e_text_new_line Element (i.e. a Td operator in PDF content stream).
	    /// Move to the start of the next line, offset from the start of the current
	    /// line by (dx , dy). dx and dy are numbers expressed in unscaled text space
	    /// units.
	    /// 
	    /// </summary>
	    /// <param name="dx">the dx
	    /// </param>
	    /// <param name="dy">the dy
	    /// </param>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateTextNewLine(Double dx, Double dy)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateTextNewLineWithOffset(mp_builder, dx, dy, ref result));
            return new Element(result, this, null);
        }
	    /// <summary> Create e_text_new_line Element (i.e. a T* operator in PDF content stream).
	    /// 
	    /// </summary>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateTextNewLine()
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateTextNewLine(mp_builder, ref result));
            return new Element(result, this, null);
        }

	    // Path Element -------------------------------------------------
	    /// <summary> Create a rectangle path Element.
	    /// 
	    /// </summary>
	    /// <param name="x">the x
	    /// </param>
	    /// <param name="y">the y
	    /// </param>
	    /// <param name="width">the width
	    /// </param>
	    /// <param name="height">the height
	    /// </param>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateRect(Double x, Double y, Double width, Double height)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateRect(mp_builder, x, y, width, height, ref result));
            return new Element(result, this, null);
        }
	    /// <summary> Create an ellipse (or circle, if rx == ry) path Element.
	    /// 
	    /// </summary>
	    /// <param name="cx">the cx
	    /// </param>
	    /// <param name="cy">the cy
	    /// </param>
	    /// <param name="rx">the rx
	    /// </param>
	    /// <param name="ry">the ry
	    /// </param>
	    /// <returns> the element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreateEllipse(Double cx, Double cy, Double rx, Double ry)
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreateEllipse(mp_builder, cx, cy, rx, ry, ref result));
            return new Element(result, this, null);
        }
	    //Element CreatePath(const Double points[], Int32 point_count, const Byte seg_types[], Int32 seg_types_count);
	    /// <summary> Create a path Element using given path segment data.
	    /// 
	    /// </summary>
	    /// <param name="points">the points
	    /// </param>
	    /// <param name="point_count">number of points in points array
	    /// </param>
	    /// <param name="seg_types_count">number of segment types in seg_types array
	    /// </param>
	    /// <param name="seg_types">the seg_types
	    /// </param>
	    /// <returns> the element
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element CreatePath(double[] points, int point_count, byte[] seg_types, int seg_types_count)
        {
            TRN_Element result = IntPtr.Zero;

            int psize = Marshal.SizeOf(seg_types[0]) * seg_types.Length;
            IntPtr pnt = Marshal.AllocHGlobal(psize);
            int psize2 = Marshal.SizeOf(points[0]) * points.Length;
            IntPtr pnt2 = Marshal.AllocHGlobal(psize2);
            try
            {
                Marshal.Copy(seg_types, 0, pnt, seg_types.Length);
                Marshal.Copy(points, 0, pnt2, points.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCreatePath(mp_builder, pnt2, point_count, pnt, seg_types_count, ref result));
                return new Element(result, this, null);
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
                Marshal.FreeHGlobal(pnt2);
            }
        }
	    /// <summary> Starts building a new path Element that can contain an arbitrary sequence
	    /// of lines, curves, and rectangles.
	    /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void PathBegin()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderPathBegin(mp_builder));
        }
	    /// <summary> Finishes building of the path Element.
	    /// 
	    /// </summary>
	    /// <returns> the path Element
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Element PathEnd()
        {
            TRN_Element result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderPathEnd(mp_builder, ref result));
            return new Element(result, this, null);
        }
	    /// <summary> Add a rectangle to the current path as a complete subpath.
	    /// Setting the current point is not required before using this function.
	    /// 
	    /// </summary>
	    /// <param name="x">the x
	    /// </param>
	    /// <param name="y">the y
	    /// </param>
	    /// <param name="width">the width
	    /// </param>
	    /// <param name="height">the height
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Rect(Double x, Double y, Double width, Double height)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderRect(mp_builder, x, y, width, height));
        }
	    /// <summary> Set the current point.
	    /// 
	    /// </summary>
	    /// <param name="x">the x
	    /// </param>
	    /// <param name="y">the y
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void MoveTo(Double x, Double y)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderMoveTo(mp_builder, x, y));
        }
	    /// <summary> Draw a line from the current point to the given point.
	    /// 
	    /// </summary>
	    /// <param name="x">the x
	    /// </param>
	    /// <param name="y">the y
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void LineTo(Double x, Double y)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderLineTo(mp_builder, x, y));
        }
	    /// <summary> Draw a Bezier curve from the current point to the given point (x2, y2) using
	    /// (cx1, cy1) and (cx2, cy2) as control points.
	    /// 
	    /// </summary>
	    /// <param name="cx1">the cx1
	    /// </param>
	    /// <param name="cy1">the cy1
	    /// </param>
	    /// <param name="cx2">the cx2
	    /// </param>
	    /// <param name="cy2">the cy2
	    /// </param>
	    /// <param name="x2">the x2
	    /// </param>
	    /// <param name="y2">the y2
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void CurveTo(Double cx1, Double cy1, Double cx2, Double cy2, Double x2, Double y2)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderCurveTo(mp_builder, cx1, cy1, cx2, cy2, x2, y2));
        }
	    /// <summary> Draw an arc with the specified parameters (upper left corner, width, height and angles).
	    /// 
	    /// </summary>
	    /// <param name="x">the x
	    /// </param>
	    /// <param name="y">the y
	    /// </param>
	    /// <param name="width">the width
	    /// </param>
	    /// <param name="height">the height
	    /// </param>
	    /// <param name="start">		starting angle of the arc in degrees
	    /// </param>
	    /// <param name="extent">		angular extent of the arc in degrees
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void ArcTo(Double x, Double y, Double width, Double height, Double start, Double extent)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderArcTo(mp_builder, x, y, width, height, start, extent));
        }
	    /// <summary> Draw an arc from the current point to the end point.
	    /// 
	    /// </summary>
	    /// <param name="xr">the xr
	    /// </param>
	    /// <param name="yr">the yr
	    /// </param>
	    /// <param name="rx">				x-axis rotation in degrees
	    /// </param>
	    /// <param name="isLargeArc">		indicates if smaller or larger arc is chosen
	    /// 1 - one of the two larger arc sweeps is chosen
	    /// 0 - one of the two smaller arc sweeps is chosen
	    /// </param>
	    /// <param name="sweep">			direction in which arc is drawn (1 - clockwise, 0 - counterclockwise)
	    /// </param>
	    /// <param name="endX">the end x
	    /// </param>
	    /// <param name="endY">the end y
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>The Arc is defined the same way as it is specified by SVG or XPS standards. For
        /// further questions please refer to the XPS or SVG standards.	</remarks>  
        public void ArcTo(Double xr, Double yr, Double rx, Boolean isLargeArc, Boolean sweep, Double endX, Double endY)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderArcTo2(mp_builder, xr, yr, rx, isLargeArc, sweep, endX, endY));
        }
	    /// <summary> Add an ellipse (or circle, if rx == ry) to the current path as a complete subpath.
	    /// Setting the current point is not required before using this function.
	    /// 
	    /// </summary>
	    /// <param name="cx">the cx
	    /// </param>
	    /// <param name="cy">the cy
	    /// </param>
	    /// <param name="rx">the rx
	    /// </param>
	    /// <param name="ry">the ry
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Ellipse(double cx, double cy, double rx, double ry)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderEllipse(mp_builder, cx, cy, rx, ry));
        }
	    /// <summary> Closes the current subpath.
	    /// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void ClosePath()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderClosePath(mp_builder));
        }
	    /// <summary> Releases all resources used by the ElementBuilder </summary>
	    ~ElementBuilder()
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
            if (mp_builder != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_ElementBuilderDestroy(mp_builder));
                mp_builder = IntPtr.Zero;
            }
        }
    }
}