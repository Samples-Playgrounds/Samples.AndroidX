using System;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_GeometryCollection = System.IntPtr;
using TRN_PDFDoc = System.IntPtr;

namespace pdftron.PDF
{
	public enum GeometryCollectionSnappingMode
	{
		e_default_snap_mode = 14,
		e_point_on_line = 1,
		e_line_midpoint = 2,
		e_line_intersection = 4,
		e_path_endpoint = 8,
	};
	/// <summary>
	/// A Preprocessed PDF geometry collection
	/// </summary>
	public class GeometryCollection : IDisposable
	{
		internal TRN_GeometryCollection m_impl = IntPtr.Zero;

		public GeometryCollection(TRN_GeometryCollection impl_ptr)
		{
			m_impl = impl_ptr;
		}

		~GeometryCollection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            Destroy();		
        }

        public void Destroy()
        {
        	if(m_impl != IntPtr.Zero)
        	{
        		TRN_GeometryCollection to_delete = m_impl;
        		m_impl = IntPtr.Zero;
            	PDFNetException.REX(PDFNetPINVOKE.TRN_GeometryCollectionDestroy(to_delete));
            }
        }



		/// <summary>
        /// return the point within the collection which is closest to the queried point. All values are in the page coordinate space.
        /// </summary>
        /// <param name="x">the x coordinate to snap, in page coordinates</param>
        /// <param name="y">the y coordinate to snap, in page coordinates</param>
        /// <param name="mode">a combination of flags from the SnappingMode enumeration</param>
        /// <returns>a point within the collection, closest to the queried point. If the collection is empty, the queried point will be returned unchanged</returns>
		public Point SnapToNearest(double x, double y, uint mode)
		{
			BasicTypes.TRN_Point result = new BasicTypes.TRN_Point();
			PDFNetException.REX(PDFNetPINVOKE.TRN_GeometryCollectionSnapToNearest(m_impl, x, y, mode, ref result));
			return new Point(result);
		}

		/// <summary>
        /// return the point within the collection which is closest to the queried point. All values are in the page coordinate space.
        /// </summary>
        /// <param name="x">the x coordinate to snap</param>
        /// <param name="y">the y coordinate to snap</param>
        /// <param name="dpi">the resolution of the rendered page, in pixels per inch</param>
        /// <param name="mode">a combination of flags from the SnappingMode enumeration</param>
        /// <returns>a point within the collection, closest to the queried point. If the collection is empty, the queried point will be returned unchanged</returns>
		public Point SnapToNearestPixel(double x, double y, double dpi, uint mode)
		{
			BasicTypes.TRN_Point result = new BasicTypes.TRN_Point();
			PDFNetException.REX(PDFNetPINVOKE.TRN_GeometryCollectionSnapToNearestPixel(m_impl, x, y, dpi, mode, ref result));
			return new Point(result);
		}
	}
}
