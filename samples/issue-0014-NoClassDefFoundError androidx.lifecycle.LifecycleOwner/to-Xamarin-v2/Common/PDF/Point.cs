using System;

using pdftronprivate.trn;
using pdftron.Common;

namespace pdftron.PDF
{
    /// <summary> The Class Point.</summary>
    public class Point
    {
        internal BasicTypes.TRN_Point mp_imp;

        internal Point(BasicTypes.TRN_Point imp)
        {
            this.mp_imp = imp;
        }

        /// <summary> Instantiates a new point.</summary>
        public Point()
        {
            this.mp_imp = new BasicTypes.TRN_Point();
        }
        /// <summary> Instantiates a new point.
        /// 
        /// </summary>
        /// <param name="px">the x coordinate
        /// </param>
        /// <param name="py">the y coordinate
        /// </param>
        public Point(double px, double py)
        {
            this.mp_imp = new BasicTypes.TRN_Point(px, py);
        }

        // Properties
        /// <summary> x coordinate
        /// </summary>
        public double x
        {
            get { return mp_imp.x; }
            set { mp_imp.x = value; }
        }
        /// <summary> y coordinate
        /// </summary>
        public double y
        {
            get { return mp_imp.y; }
            set { mp_imp.y = value; }
        }
    }
}