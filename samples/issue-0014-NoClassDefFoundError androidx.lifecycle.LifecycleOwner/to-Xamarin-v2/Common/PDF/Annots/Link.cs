using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Action = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> A link annotation represents either a hypertext link to a destination elsewhere in the document 
    /// or an action to be performed. 
    /// </summary>
    public class Link : Annot
    {
        internal Link(TRN_Annot imp, Object reference) : base(imp, reference) { }
        
        /// <summary> Creates a Link annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
        /// <remarks>  The constructor does not copy any data, but is instead the logical
        /// equivalent of a type cast.</remarks>
        public Link(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LinkAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		/// <summary> Creates a new Link annotation.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A new Link annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Link Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LinkAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Link(result, doc);
        }

		/// <summary> Creates a new Link annotation.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <param name="action">Action for the link annotation.
		/// </param>
		/// <returns> A new Link annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Link Create(SDF.SDFDoc doc, Rect pos, Action action)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LinkAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            PDFNetException.REX(PDFNetPINVOKE.TRN_LinkAnnotSetAction(result, action.mp_action));
            return new Link(result, doc);
        }

		/// <summary> Removes a link annotation's action.
		/// 
        /// </summary>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void RemoveAction()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LinkAnnotRemoveAction(mp_annot));
        }

		/// <summary> Gets the Action of the Linked Annotation.
		/// 
		/// </summary>
		/// <returns> An Action object that denotes the action of the linked annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The return value is an action that shall be performed when the 
        /// link annotation is activated</remarks>
        public Action GetAction()
        {
            TRN_Action result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LinkAnnotGetAction(mp_annot, ref result));
            return result != IntPtr.Zero ? new Action(result, GetRefHandleInternal()) : null;;
        }

		/// <summary> Sets the Action of the Linked Annotation.
		/// (Optional; PDF 1.1 )
		/// 
		/// </summary>
		/// <param name="action">An Action object that denotes the action of the linked annotation.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The parameter is an action that shall be performed when the 
        /// link annotation is activated</remarks>
        public void SetAction(Action action)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LinkAnnotSetAction(mp_annot, action.mp_action));
        }

        /// <summary> Gets the HighlightingMode Linked Annotation.
		/// 
		/// </summary>
		/// <returns> The HighLighting mode represented as an entry of the enum "HighlightingMode".
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public HighlightingMode GetHighlightingMode()
        {
            HighlightingMode result = HighlightingMode.e_none;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LinkAnnotGetHighlightingMode(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the HighlightingMode Linked Annotation.
		/// (Optional; PDF 1.2 )
		/// 
		/// </summary>
		/// <param name="mode">The HighLighting mode represented as an entry of the enum "HighlightingMode".
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetHighlightingMode(HighlightingMode mode)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LinkAnnotSetHighlightingMode(mp_annot, mode));
        }

		/// <summary> Gets the number of QuadPoints in the QuadPoints array of the Link annotation.
		/// 
		/// </summary>
		/// <returns> The number of QuadPoints.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  An array of n QuadPoints specifying the coordinates of n quadrilaterals
		/// in default user space that comprise the region in which the link should be activated.
		/// The coordinates specifying the four vertices of the quadrilateral in counterclockwise order.
		/// For orientation purposes, such as when applying an underline border style, the bottom of a
        /// quadrilateral is the line formed by p1 and p2 of the QuadPoint.
        /// QuadPoints shall be ignored if any coordinate in the array lies outside the region specified by Rect.</remarks>
        public int GetQuadPointCount()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LinkAnnotGetQuadPointCount(mp_annot, ref result));
            return result;
        }

		/// <summary> Gets the QuadPoint located at a certain index of the QuadPoint array of the Link
		/// annotation.
		/// 
		/// </summary>
		/// <param name="idx">the idx
		/// </param>
		/// <returns> The QuadPoint located at a certain index of the QuadPoint array of the Link annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  An array of n QuadPoints specifying the coordinates of n quadrilaterals
		/// in default user space that comprise the region in which the link should be activated.
		/// The coordinates specifying the four vertices of the quadrilateral in counterclockwise order.
		/// For orientation purposes, such as when applying an underline border style, the bottom of a
		/// quadrilateral is the line formed by p1 and p2 of the QuadPoint.
        /// QuadPoints shall be ignored if any coordinate in the array lies outside the region specified by Rect.
        /// </remarks>
        public QuadPoint GetQuadPoint(int idx)
        {
            BasicTypes.TRN_QuadPoint result = new BasicTypes.TRN_QuadPoint();
            PDFNetException.REX(PDFNetPINVOKE.TRN_LinkAnnotGetQuadPoint(mp_annot, idx, ref result));
            return new QuadPoint(result);
        }

		/// <summary> Set the QuadPoint to be located at a certain index of the QuadPoint array of the Link
		/// annotation.
		/// (Optional; PDF 1.6 )
		/// 
		/// </summary>
		/// <param name="idx">the idx
		/// </param>
		/// <param name="qp">the qp
		/// </param>
		/// <returns> The QuadPoint to be located at a certain index of the QuadPoint array of the Link annotation.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  An array of n QuadPoints specifying the coordinates of n quadrilaterals
		/// in default user space that comprise the region in which the link should be activated.
		/// The coordinates specifying the four vertices of the quadrilateral in counterclockwise order.
		/// For orientation purposes, such as when applying an underline border style, the bottom of a
		/// quadrilateral is the line formed by p1 and p2 of the QuadPoint.
		/// QuadPoints shall be ignored if any coordinate in the array lies outside the region specified by Rect.
		/// </remarks>
        public void SetQuadPoint(int idx, QuadPoint qp) //Note the default should be just a single rect that is equal to Rect entry. 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LinkAnnotSetQuadPoint(mp_annot, idx, ref qp.mp_imp));
        }
		/// <summary> Creates a Link annotation and initialize it using given annotation object.
		/// 
		/// </summary>
		/// <param name="ann">the annot
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public Link(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Releases all resources used by the Link </summary>
		~Link()
        {
            Dispose(false);
        }

         // Nested Types
        ///<summary> highlighting mode is the visual effect that shall be used when
		/// the mouse button is pressed or held down inside its active area</summary>
		public enum HighlightingMode
        {
            ///<summary>No highlighting</summary>
			e_none,     
			///<summary>Invert the contents of the annotation rectangle.</summary>
			e_invert,   
			///<summary>Invert the annotation's border</summary>
			e_outline,  
			///<summary>Display the annotation as if it were being pushed below the surface of the page.</summary>
			e_push
        }

    }
}