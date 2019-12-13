using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> An ink annotation (PDF 1.3) represents a freehand “scribble” composed 
    /// of one or more disjoint paths. When opened, it shall display a pop-up 
    /// window containing the text of the associated note. 
    /// </summary>
    public class Ink : Markup
    {
        internal Ink(TRN_Annot imp, Object reference) : base(imp, reference) { }

        //Ink(SDF.Obj d);
		/// <summary> Creates an Ink annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public Ink(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_InkAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		//static Ink Create(SDF.SDFDoc& doc, Rect& pos
		/// <summary> Creates a new Ink annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the Ink annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the Ink annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank Ink annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Ink Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_InkAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Ink(result, doc);
        }

		//int GetPathCount();
		/// <summary> Gets number of stroked pathes in the InkList.
		/// 
		/// </summary>
		/// <returns> An integer representing the number of pathes in the Ink list.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Ink list is An array of n arrays,
		/// each representing a stroked path. Each array shall be a series of
		/// Point objects specifying points along the path.
		/// When drawn, the points shall be connected by straight lines or curves
        /// in an implementation-dependent way.
        /// </remarks>
        public int GetPathCount()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_InkAnnotGetPathCount(mp_annot, ref result));
            return result;
        }

		//int GetPointCount(unsigned int pathindex);
		/// <summary> Gets number of points in a certain stroked path in the InkList.
		/// 
		/// </summary>
		/// <param name="pathindex">An unsigned integer indicating the index of the stroked
		/// path, the number of points within whom is of our interest.
		/// </param>
		/// <returns> An integer representing the number of points in the stroked path
		/// of the Ink list.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Ink list is An array of n arrays, 
		/// each representing a stroked path. Each array shall be a series of
		/// Point objects specifying points along the path.
		/// When drawn, the points shall be connected by straight lines or curves
		/// in an implementation-dependent way.</remarks>
        public int GetPointCount(int pathindex)
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_InkAnnotGetPointCount(mp_annot, pathindex, ref result));
            return result;
        }

		//Point GetPoint(unsigned int pathindex, unsigned int pointindex);
		/// <summary> Gets the specific point in the Ink List.
		/// 
		/// </summary>
		/// <param name="pathindex">An unsigned integer indicating the index of the stroked
		/// path
		/// </param>
		/// <param name="pointindex">An unsigned integer indicating the index of the point
		/// within the stroked path indicated by the parameter "pathindex".
		/// </param>
		/// <returns> A Point object that is located by "pathindex" and "pointindex".
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Ink list is An array of n arrays,
		/// each representing a stroked path. Each array shall be a series of
		/// Point objects specifying points along the path.
        /// When drawn, the points shall be connected by straight lines or curves
        /// in an implementation-dependent way.</remarks>
        public Point GetPoint(int pathindex, int pointindex)
        {
            BasicTypes.TRN_Point result = new BasicTypes.TRN_Point();
            PDFNetException.REX(PDFNetPINVOKE.TRN_InkAnnotGetPoint(mp_annot,System.Convert.ToUInt32(pathindex),System.Convert.ToUInt32(pointindex), ref result));
            return new Point(result);
        }

		//void SetPoint(unsigned int pathindex, unsigned int pointindex, Point& pt);
		/// <summary> Sets the specific point in the Ink List.
		/// 
		/// </summary>
		/// <param name="pathindex">An unsigned integer indicating the index of the stroked
		/// path
		/// </param>
		/// <param name="pointindex">An unsigned integer indicating the index of the point
		/// within the stroked path indicated by the parameter "pathindex".
		/// </param>
		/// <param name="pt">A Point object that is to be located by "pathindex" and "pointindex".
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Ink list is An array of n arrays, 
		/// each representing a stroked path. Each array shall be a series of
		/// Point objects specifying points along the path.
		/// When drawn, the points shall be connected by straight lines or curves
		/// in an implementation-dependent way.</remarks>
        public void SetPoint(int pathindex, int pointindex, Point pt)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_InkAnnotSetPoint(mp_annot,System.Convert.ToUInt32(pathindex),System.Convert.ToUInt32(pointindex), ref pt.mp_imp));
        }

        //bool Erase(const Point& pt1, const Point& pt2, double width);
		/// <summary> Erase a rectangle area formed by pt1pt2 with width
		/// 
		/// </summary>
		/// <param name="pt1">A point object that is one end of the eraser segment.
		/// </param>
		/// <param name="pt2">A point object that is the other end of the eraser segment.
		/// </param>
		/// <param name="eraserHalfWidth">The half width of the eraser.
		/// </param>
		/// <returns> Whether an ink stroke was erased.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public bool Erase(Point pt1, Point pt2, double eraserHalfWidth)
		{
			bool result = false;
			PDFNetException.REX(PDFNetPINVOKE.TRN_InkAnnotErase(mp_annot, ref pt1.mp_imp, ref pt2.mp_imp, eraserHalfWidth, ref result));
            return result;
		}

		/// <summary> Creates an Ink annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical 
		/// equivalent of a type cast.</remarks>
        public Ink(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Releases all resources used by the Ink </summary>
        ~Ink()
        {
            Dispose(false);
        }
    }
}