using System;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_Annot = System.IntPtr;
using TRN_Exception = System.IntPtr;

namespace pdftron.PDF.Annots
{
    /// <summary> The purpose of a line annotation (PDF 1.3) is to display a single straight 
    /// line on the page. When opened, it shall display a pop-up window containing 
    /// the text of the associated note. 
    /// </summary>
    public class Line : Markup
    {
        internal Line(TRN_Annot imp, Object reference) : base(imp, reference) { }

        //Line.Line(SDF.Obj d=0);
		/// <summary> Creates an Line annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="d">the d
		/// </param>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public Line(SDF.Obj d)
            : base(d)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotCreateFromObj(d.mp_obj, ref mp_annot));
        }

		//static Line Create(SDF.SDFDoc& doc, Rect& pos);
		/// <summary> Creates a new Line annotation, in the specified document.
		/// 
		/// </summary>
		/// <param name="doc">A document to which the annotation is added.
		/// </param>
		/// <param name="pos">A rectangle specifying the annotation's bounds, specified in
		/// user space coordinates.
		/// </param>
		/// <returns> A newly created blank Line annotation.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>		
        public static Line Create(SDF.SDFDoc doc, Rect pos)
        {
            TRN_Annot result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotCreate(doc.mp_doc, ref pos.mp_imp, ref result));
            return new Line(result, doc);
        }

		//Point GetStartPoint() ;
		/// <summary> Gets the coordinates of the start of a line.
		/// 
		/// </summary>
		/// <returns> A Point struct, whose x entry specifies the x coordinate of the start of the line
		/// and the  y entry specifies the y coordinate of the start of the line.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Point GetStartPoint()
        {
            BasicTypes.TRN_Point result = new BasicTypes.TRN_Point();
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotGetStartPoint(mp_annot, ref result));
            return new Point(result);
        }

		//public void SetStartPoint(Point& sp);
		/// <summary> Sets the coordinates of the start of a line.
		/// 
		/// </summary>
		/// <param name="sp">A Point struct whose x entry is going to be set as the
		/// x coordinate of the start point of the line, whose y entry is going to be set as the
		/// y coordinate of the start point of the line.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetStartPoint(Point sp)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotSetStartPoint(mp_annot, ref sp.mp_imp));
        }

		//Point GetEndPoint () ;
		/// <summary> Gets the coordinates of the end of a line.
		/// 
		/// </summary>
		/// <returns> A Point struct, whose x entry specifies the x coordinate of the end of the line
		/// and the  y entry specifies the y coordinate of the end of the line.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Point GetEndPoint()
        {
            BasicTypes.TRN_Point result = new BasicTypes.TRN_Point();
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotGetEndPoint(mp_annot, ref result));
            return new Point(result);
        }

		//public void SetEndPoint(Point& ep);
		/// <summary> Sets the coordinates of the end of a line.
		/// 
		/// </summary>
		/// <param name="ep">- A Point struct whose x entry is going to be set as the
		/// x coordinate of the end point of the line, whose y entry is going to be set as the
		/// y coordinate of the end point of the line.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetEndPoint(Point ep)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotSetEndPoint(mp_annot, ref ep.mp_imp));
        }

        /// <summary> Gets the ending syle of the start of a line.
		/// 
		/// </summary>
		/// <returns> A enum value from the "EndingStyle" enum, whose value corrsponding to the ending style
		/// of the start point.
		/// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Default value: e_None. </remarks>
        public EndingStyle GetStartStyle()
        {
            EndingStyle result = EndingStyle.e_None;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotGetStartStyle(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the ending syle of the start of a line
		/// (Optional; PDF 1.4.)
		/// 
		/// </summary>
		/// <param name="sst">A enum value from the "EndingStyle" enum, whose value corrsponding to the ending style
		/// of the start point.
		/// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Default value: e_None. </remarks>
        public void SetStartStyle(EndingStyle sst)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotSetStartStyle(mp_annot, sst));
        }

		/// <summary> Gets the ending syle of the end of a line.
		/// 
		/// </summary>
		/// <returns> A enum value from the "EndingStyle" enum, whose value corrsponding to the ending style
		/// of the start point.
		/// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Default value: e_None. </remarks>
        public EndingStyle GetEndStyle()
        {
            EndingStyle result = EndingStyle.e_None;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotGetEndStyle(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the ending syle of the end of a line
		/// (Optional; PDF 1.4)
		/// 
		/// </summary>
		/// <param name="est">A enum value from the "EndingStyle" enum, whose value corrsponding to the ending style
		/// of the start point.
		/// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Default value: e_None. </remarks>
        public void SetEndStyle(EndingStyle est)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotSetEndStyle(mp_annot, est));
        }

		/// <summary> Gets the leader line length of a line. 					 *
		/// 
		/// </summary>
		/// <returns> A number denoting the length of the leader line in px.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  usually if this entry is specified, the line is intended to be a dimension line(see "IT" entry).
		/// Leader length is length of leader lines in default user space that extend from each endpoint 
		/// of the line perpendicular to the line itself. A positive value shall mean that the leader lines
		/// appear in the direction that is clockwise when traversing the line from its starting point to
		/// its ending point (as specified by L); a negative value shall indicate the opposite direction.
        /// Default value: 0 (no leader lines)
        /// </remarks>
        public double GetLeaderLineLength()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotGetLeaderLineLength(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the leader line length of a line.
		/// (PDF 1.6)
		/// 
		/// </summary>
		/// <param name="ll">the new leader line length
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Usually if this entry is specified, the line is intended to be a dimension line(see "IT" entry).
		/// Leader length is length of leader lines in default user space that extend from each endpoint 
		/// of the line perpendicular to the line itself. A positive value shall mean that the leader lines
		/// appear in the direction that is clockwise when traversing the line from its starting point to
        /// its ending point (as specified by L); a negative value shall indicate the opposite direction.
        /// Default value: 0 (no leader lines)</remarks>
        public void SetLeaderLineLength(double ll)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotSetLeaderLineLength(mp_annot, ll));
        }

		/// <summary> Gets the leader line extension length of a line.
		/// 
		/// </summary>
		/// <returns> A number denoting the length of the leader line extension in px.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Usually if this entry is specified, the line is intended to be a dimension line(see "IT" entry). 
		/// <remarks>  The leader line extension length is a non-negative number that shall represents </remarks>
		/// the length of leader line extensions that extend from the line proper 180 degrees from
        /// the leader lines.
        /// Default value: 0 (no leader line extensions)</remarks>
        public double GetLeaderLineExtensionLength()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotGetLeaderLineExtensionLength(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the leader line extension length of a line.
		/// (PDF 1.6)
		/// 
		/// </summary>
		/// <param name="ll">the new leader line extension length
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Usually if this entry is specified, the line is intended to be a dimension line(see "IT" entry).
		/// The leader line extension length is a non-negative number that shall represents
		/// the length of leader line extensions that extend from the line proper 180 degrees from
        /// the leader lines.
        /// Default value: 0 (no leader line extensions)</remarks>
        public void SetLeaderLineExtensionLength(double ll)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotSetLeaderLineExtensionLength(mp_annot, ll));
        }

		/// <summary> Gets the option of whether to show caption.
		/// 
		/// </summary>
		/// <returns> A boolean value indicating whether the caption will be shown.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If true, the text specified by the Contents or RCentries shall be replicated as a
		/// caption in the appearance of the line. The text shall be rendered in a manner
        /// appropriate to the content, taking into account factors such as writing direction.
        /// Default value: false.</remarks>
        public bool GetShowCaption()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotGetShowCaption(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the option of whether to show caption.
		/// 
		/// </summary>
		/// <param name="showCaption">A boolean value indicating whether the caption will be shown.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  If true, the text specified by the Contents or RCentries shall be replicated as a
		/// caption in the appearance of the line. The text shall be rendered in a manner
		/// appropriate to the content, taking into account factors such as writing direction.
		/// Default value: false.</remarks>
        public void SetShowCaption(bool showCaption)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotSetShowCaption(mp_annot, showCaption));
        }
 
        /// <summary> Gets the intent type of the line.
		/// 
		/// </summary>
		/// <returns> An intent type value from the "IntentType" enum.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Intent type describes the intent of the line annotation. Valid values shall be "e_LineArrow", which means
        /// that the annotation is intended to function as an arrow, and "e_LineDimension", which means that the annotation
        /// is intended to function as a dimension line.</remarks>
        public IntentType GetIntentType()
        {
            IntentType result = IntentType.e_null;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotGetIntentType(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the intent type of the line.
		/// (For PDF 1.6)
		/// 
		/// </summary>
		/// <param name="style">An intent type value from the "IntentType" enum.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Intent type describes the intent of the line annotation. Valid values shall be "e_LineArrow", which means
        /// that the annotation is intended to function as an arrow, and "e_LineDimension", which means that the annotation
        /// is intended to function as a dimension line.</remarks>
        public void SetIntentType(IntentType style)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotSetIntentType(mp_annot, style));
        }

		/// <summary> Gets the leader line offset length of a line.
		/// 
		/// </summary>
		/// <returns> A number denoting the length of the leader line offset in px.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  usually if this entry is specified, the line is intended to be a dimension line(see "IT" entry).
		/// Leader line offset number is a non-negative number that shall represent the length of the leader
        /// line offset, which is the amount of empty space between the endpoints of the
        /// annotation and the beginning of the leader lines.</remarks>
        public double GetLeaderLineOffset()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotGetLeaderLineOffset(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the leader line offset length of a line.
		/// (PDF 1.7)
		/// 
		/// </summary>
		/// <param name="ll">A number denoting the length of the leader line offset in px.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Usually if this entry is specified, the line is intended to be a dimension line(see "IT" entry). 
		/// Leader line offset number is a non-negative number that shall represent the length of the leader
        /// line offset, which is the amount of empty space between the endpoints of the
        /// annotation and the beginning of the leader lines.</remarks>
        public void SetLeaderLineOffset(double ll)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotSetLeaderLineOffset(mp_annot, ll));
        }

        /// <summary> Gets the caption position of a line.
		/// 
		/// </summary>
		/// <returns> A cap position value from the "CapPos" enum.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Caption position describs the annotation’s caption positioning.
		/// Valid values are e_Inline, meaning the caption shall be centered inside the line,
        /// and e_Top, meaning the caption shall be on top of the line.
        /// Default value: Inline</remarks>
        public CapPos GetCaptionPosition()
        {
            CapPos result = CapPos.e_Inline;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotGetCapPos(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the caption position of a line.
		/// (PDF 1.7)
		/// 
		/// </summary>
		/// <param name="style">A cap position value from the "CapPos" enum.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  Caption position describs the annotation’s caption positioning.
		/// Valid values are e_Inline, meaning the caption shall be centered inside the line,
        /// and e_Top, meaning the caption shall be on top of the line.
        /// Default value: Inline.</remarks>
        public void SetCaptionPosition(CapPos style)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotSetCapPos(mp_annot, style));
        }

		/// <summary> Gets the horizontal offset of the caption.
		/// 
		/// </summary>
		/// <returns> A number denoting the horizontal offset of caption in px.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The horizontal offset specifies the offset of the caption text from its normal position.
		/// Specifially the horizontal offset along the annotation line from its midpoint, with a positive
        /// value indicating offset to the right and a negative value indicating offset to the left.
        /// Default value: 0 (no offset from normal horizontal positioning)</remarks>
        public double GetTextHOffset()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotGetTextHOffset(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the horizontal offset of the caption.
		/// (For PDF 1.7 )
		/// 
		/// </summary>
		/// <param name="offset">A umber denoting the horizontal offset of caption in px.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The horizontal offset specifies the offset of the caption text from its normal position.
		/// Specifially the horizontal offset along the annotation line from its midpoint, with a positive
        /// value indicating offset to the right and a negative value indicating offset to the left.
        /// Default value: 0 (no offset from normal horizontal positioning)</remarks>
        public void SetTextHOffset(double offset)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotSetTextHOffset(mp_annot, offset));
        }

		/// <summary> Gets the vertical offset of the caption.
		/// 
		/// </summary>
		/// <returns> A number denoting the vertical offset of caption in px.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The vertical offset specifies the offset of the caption text from its normal position.
		/// Specifially the vertical offset perpendicular to the annotation line, with a positive value
		/// indicating a shift up and a negative value indicating a shift down.
        /// Default value: 0(no offset from normal vertical positioning).
        /// </remarks>
        public double GetTextVOffset()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotGetTextVOffset(mp_annot, ref result));
            return result;
        }

		/// <summary> Sets the vertical offset of the caption.
		/// (For PDF 1.7 )
		/// 
		/// </summary>
		/// <param name="offset">A number denoting the vertical offset of caption in px.
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  The vertical offset specifies the offset of the caption text from its normal position.
		/// Specifially the vertical offset perpendicular to the annotation line, with a positive value
        /// indicating a shift up and a negative value indicating a shift down.
        /// Default value: 0(no offset from normal vertical positioning).</remarks>
        public void SetTextVOffset(double offset)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_LineAnnotSetTextVOffset(mp_annot, offset));
        }
		/// <summary> Creates an Line annotation and initialize it using given Cos/SDF object.
		/// 
		/// </summary>
		/// <param name="ann">the d
		/// </param>
		/// <remarks>  The constructor does not copy any data, but is instead the logical
		/// equivalent of a type cast.</remarks>
        public Line(Annot ann) : base(ann.GetSDFObj()) { }
		/// <summary> Releases all resources used by the Line </summary>
		~Line()
        {
            Dispose(false);
        }

        // Nested Types
        /// <summary> For each line, one can choose a separate style for the start and the end of the line.
		/// The styles are defined by the "EndingStyle" enumeration.</summary>
		public enum EndingStyle
		{
			/// <summary>A square filled with the annotation’s interior color, if any.</summary>
			e_Square,
			/// <summary>A circle filled with the annotation’s interior color, if any.</summary>
			e_Circle,
			/// <summary>A diamond shape filled with the annotation’s interior color, if any.</summary>
			e_Diamond,
			/// <summary>Two short lines meeting in an acute angle to form an open arrowhead.</summary>
			e_OpenArrow,
			/// <summary>Two short lines meeting in an acute angle as in the OpenArrow style and connected by a third line to form a triangular closed arrowhead filled with the annotation’s interior color, if any.</summary>
			e_ClosedArrow,
			/// <summary>A short line at the endpoint perpendicular to the line itself.</summary>
			e_Butt,
			/// <summary>Two short lines in the reverse direction from OpenArrow.</summary>
			e_ROpenArrow,
			/// <summary>A triangular closed arrowhead in the reverse direction from ClosedArrow.</summary>
			e_RClosedArrow,
			/// <summary>A short line at the endpoint approximately 30 degrees clockwise from perpendicular to the line itself.</summary>
			e_Slash,
			/// <summary>No special line ending.</summary>
			e_None,
			/// <summary>Non-standard or invalid ending.</summary>
			e_Unknown
		}

        /// <summary>the caption position of the Line annotation.</summary>
		public enum IntentType
		{
			///<summary>The line is an arrow</summary>
			e_LineArrow,
			///<summary>This line is intented to function as a 
			/// dimension line. </summary>
			e_LineDimension,
			///<summary>Non-standard intent type</summary>
			e_null
		}
        /// <summary> Enumeration type describing the annotation’s caption positioning.
		/// Valid values are e_Inline, meaning the caption shall be centered inside the line, and e_Top, meaning the caption shall be on top of the line.</summary>
		public enum CapPos
		{
			///<summary>the caption shall be centered inside the line</summary>
			e_Inline,
			/// <summary>the caption shall be on top of the line</summary>
			e_Top
		}
    }
}