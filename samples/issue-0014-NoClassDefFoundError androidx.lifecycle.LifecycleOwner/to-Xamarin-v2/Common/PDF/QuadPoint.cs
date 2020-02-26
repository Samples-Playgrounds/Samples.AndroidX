using System;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Exception = System.IntPtr;

namespace pdftron.PDF
{
    public class QuadPoint
    {
        internal BasicTypes.TRN_QuadPoint mp_imp;

        internal QuadPoint(BasicTypes.TRN_QuadPoint imp)
        {
            this.mp_imp = imp;
        }

        /// <summary> Instantiates a new quad point.</summary>
        public QuadPoint()
        {
            this.mp_imp = new BasicTypes.TRN_QuadPoint();
        }

        /// <summary> Instantiates a new quad point.
	    /// 
	    /// </summary>
	    /// <param name="p11">the p1
	    /// </param>
	    /// <param name="p22">the p2
	    /// </param>
	    /// <param name="p33">the p3
	    /// </param>
	    /// <param name="p44">the p4
	    /// </param>
        public QuadPoint(Point p11, Point p22, Point p33, Point p44)
        {
            this.mp_imp = new BasicTypes.TRN_QuadPoint(p11.mp_imp, p22.mp_imp, p33.mp_imp, p44.mp_imp);
        }

        public Point p1
        {
            get { return new Point(mp_imp.p1); }
            set { mp_imp.p1 = new BasicTypes.TRN_Point(value.mp_imp.x, value.mp_imp.y); }
        }

        public Point p2
        {
            get { return new Point(mp_imp.p2); }
            set { mp_imp.p2 = new BasicTypes.TRN_Point(value.mp_imp.x, value.mp_imp.y); }
        }

        public Point p3
        {
            get { return new Point(mp_imp.p3); }
            set { mp_imp.p3 = new BasicTypes.TRN_Point(value.mp_imp.x, value.mp_imp.y); }
        }

        public Point p4
        {
            get { return new Point(mp_imp.p4); }
            set { mp_imp.p4 = new BasicTypes.TRN_Point(value.mp_imp.x, value.mp_imp.y); }
        }
    }
}