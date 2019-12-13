using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace pdftron.PDF
{
    /// <summary>
    /// Contains the information required to draw the path. Contains an array of 
    /// PathSegmentType Operators and corresponding path data Points. A point may 
    /// be on or off (off points are control points). The meaning of a point 
    /// depends on associated id (or segment type) in the path segment type array.
    /// </summary>
    public class PathData
    {
        // Fields
        internal bool _def;
        internal byte[] _ops;
        internal double[] _pts;

        /// <summary> Create empty path data object.
	    /// 
	    /// </summary>
        public PathData()
        {
            _ops = null;
            _pts = null;
            _def = true;
        }
        /// <summary> Create a path data object </summary>
        /// <param name="defined">false if this is an undefined character code</param>
        /// <param name="operators">an array of path segment types</param>
        /// <param name="points">an array of path data points.</param>
        public PathData(bool defined, byte[] operators, double[] points)
        {
            _ops = operators;
            _pts = points;
            _def = defined;
        }

        // Properties
        public bool defined 
        {
            get
            {
                return _def;
            }
        }
        public byte[] operators
        {
            get
            {
                return _ops;
            }
            set
            {
                _ops = value;
            }
        }
        public double[] points
        {
            get
            {
                return _pts;
            }
            set
            {
                _pts = value;
            } 
        }

        // Nested Types
        /// <summary> Enumaration used to indicate operator type. </summary>
        public enum PathSegmentType
        {
            /// <summary> Start a new sub-path at the given (x,y) coordinate.
            /// Number of arguments: 2
            /// </summary>
            e_moveto = 1,
            /// <summary> A line from the current point to the given (x,y) coordinate which becomes 
            /// the new current point.
            /// Number of arguments: 2
            /// </summary>
            e_lineto,
            /// <summary> A cubic Bézier curve from the current point to (x,y) using (x1,y1) as
            /// the control point at the beginning of the curve and (x2,y2) as the control
            /// point at the end of the curve.
            /// Number of arguments: 6
            /// </summary>
            e_cubicto,
            /// <summary>A quadratic Bézier curve from the current point to (x,y) using (x1,y1) as
            /// the control point. Note that e_conicto does not appear in PDF content streams.
            /// This operator is only used to represent glyph outlines (e.g. PDF::Font::GetGlyphPath()
            /// may return a path containing e_conicto operator).
            /// Number of arguments: 4
            /// </summary>
            e_conicto,
            /// <summary> A rectangle at the given (x,y) coordinate and the given width and height (w, h).
            /// Number of arguments: 4
            /// </summary>
            e_rect,
            /// <summary> Close the current subpath by drawing a straight line from the current point 
            /// to current subpath's initial point.
            /// Number of arguments: 0
            /// </summary>
            e_closepath
        }
    }
}
