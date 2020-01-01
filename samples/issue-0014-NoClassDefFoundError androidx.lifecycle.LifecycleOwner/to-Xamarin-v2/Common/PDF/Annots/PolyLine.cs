using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> Polyline annotations (PDF 1.5) display (open or closed) shapes of multiple edges on the page. 
    /// Such polylines may have any number of vertices connected by straight lines. 
    /// For open polylines, which is the default type of polylines, the first and last vertex are not 
    /// implicitly connected.
    /// Closed polylines are polygons, whose first and last vertex are connected.
    /// </summary>
    public class PolyLine : Line
    {
        internal PolyLine(TRN_Annot imp, Object reference) : base(imp, reference) { }

        /// <summary> Creates a PolyLine annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical 
        /// equivalent of a type cast.</remarks>
        public PolyLine(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PolyLineAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		/// <summary> Creates a new PolyLine annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the PolyLine annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the PolyLine annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank PolyLine annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static PolyLine Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PolyLineAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new PolyLine(result, doc);
        }

		/// <summary> Gets the number of vertices in the Vertices array.
		/// 
		/// </summary>
		/// <returns> An int indicating the number of vertices.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Vertices array is An array of numbers specifying the width
        /// and dash pattern that shall represent each vertex in default user space
        /// in Point form,</remarks>
        public int GetVertexCount()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PolyLineAnnotGetVertexCount(mp_annot, ref result));
            return result;
        }

		/// <summary> Gets the vertex(in Point object form) corresponding to the index
		/// within the VErtices array.
		/// 
		/// </summary>
		/// <param name="idx">The index position where the vertex is located.
		/// </param>
		/// <returns> A Point object corresponding to the vertex in the specified index position.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Vertices array is An array of numbers specifying the width 
		/// and dash pattern that shall represent each vertex in default user space
		/// in Point form,</remarks>
        public Point GetVertex(int idx) //Note the default should be just a single rect that is equal to Rect entry. 
        {
            BasicTypes.TRN_Point result = new BasicTypes.TRN_Point();
            PDFNetException.REX(PDFNetPINVOKE.TRN_PolyLineAnnotGetVertex(mp_annot, idx, ref result));
            return new Point(result);
        }

		/// <summary> Sets the vertex(in Point object form) corresponding to the index
		/// within the VErtices array.
		/// 
		/// </summary>
		/// <param name="idx">The index position where the vertex is to be located.
		/// </param>
		/// <param name="pt">A Point object corresponding to the vertex to be added to the array.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The Vertices array is An array of numbers specifying the width
        /// and dash pattern that shall represent each vertex in default user space
        /// in Point form,</remarks>
        public void SetVertex(int idx, Point pt)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PolyLineAnnotSetVertex(mp_annot, idx, ref pt.mp_imp));
        }

        /// <summary> Gets the Intent name as an entry from the enum "IntentName"
		/// of the annnotation type.
		/// 
		/// </summary>
		/// <returns> the intent name
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <returns>s The intent name of the annotation as
        /// an entry from the enum "IntentName".
        /// </returns>
        public IntentType GetIntentName()
        {
            IntentType result = IntentType.e_Unknown;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PolyLineAnnotGetIntentName(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the Intent name as an entry from the enum "IntentName"
		/// of the annnotation type.
		/// (Optional; PDF 1.6 )
		/// 
		/// </summary>
		/// <param name="mode">The intent name of the annotation using
		/// an entry from the enum "IntentName".
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetIntentName(IntentType mode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_PolyLineAnnotSetIntentName(mp_annot, mode));
        }

		/// <summary> Creates a PolyLine annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public PolyLine(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Releases all resources used by the PolyLine </summary>
		~PolyLine()
        {
            Dispose(false);
        }

        ///<summary>intent of the polygon or polyline annotation.</summary>
		public new enum IntentType
		{
			///<summary>The annotation is intended to function as a cloud object.</summary>
			e_PolygonCloud,
			///<summary>The polyline annotation is intended to function as a dimension. (PDF 1.7)</summary>
			e_PolyLineDimension,
			///<summary>The polygon annotation is intended to function as a dimension. (PDF 1.7)</summary>
			e_PolygonDimension,
			///<summary>Non-standard intent type</summary>
			e_Unknown
		}
    }
}