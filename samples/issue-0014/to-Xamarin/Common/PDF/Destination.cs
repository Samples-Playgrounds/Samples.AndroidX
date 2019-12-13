using System;
using System.Collections.Generic;
using System.Text;

using pdftron.Common;
using pdftron.SDF;

using TRN_Destination = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Page = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> A destination defines a particular view of a document, consisting of the 
    /// following:
    /// <list type="bullet">
    /// <item><description>
    /// The page of the document to be displayed
    /// </description></item>
    /// <item><description>
    /// The location of the document window on that page
    /// </description></item>
    /// <item><description>
    /// The magnification (zoom) factor to use when displaying the page
    /// </description></item>
    /// </list>
    /// Destinations may be associated with Bookmarks, Annotations, and Remote Go-To Actions.
    /// 
    /// Destination is a utility class used to simplify work with PDF Destinations; 
    /// Please refer to section 8.2.1 'Destinations' in PDF Reference Manual for details.
    /// </summary>
    public class Destination : IDisposable
    {
        internal TRN_Destination mp_dest = IntPtr.Zero;
        internal Object m_ref;

        /// <summary> Releases all resources used by the Destination </summary>
        ~Destination()
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
            if (mp_dest != IntPtr.Zero)
            {
                mp_dest = IntPtr.Zero;
            }
        }

        // Methods
        internal Destination(TRN_Destination dest, Object reference)
        {
            this.mp_dest = dest;
            this.m_ref = reference;
        }

        /// <summary>Instantiates <c>Destination</c> from given <c>SDF::Obj</c> object
	    /// </summary>
        /// <param name="dest"><c>SDF::Obj</c> destination object
        /// </param>
        public Destination(Obj dest)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationCreate(dest.mp_obj, ref mp_dest));
            this.m_ref = dest.GetRefHandleInternal();
        }

        /// <summary> Create a new 'XYZ' Destination.
	    /// 
	    /// The new Destination displays the page designated by 'page', with the
	    /// coordinates ('left', 'top') positioned at the top-left corner of the
	    /// window and the contents of the page magnified by the factor 'zoom'.
	    /// A null value for any of the parameters 'left', 'top', or 'zoom' specifies
	    /// that the current value of that parameter is to be retained unchanged.
	    /// A 'zoom' value of 0 has the same meaning as a null value.
	    /// 
	    /// </summary>
	    /// <param name="page">the page
	    /// </param>
	    /// <param name="left">the left
	    /// </param>
	    /// <param name="top">the top
	    /// </param>
	    /// <param name="zoom">the zoom
	    /// </param>
	    /// <returns> the destination
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Destination CreateXYZ(Page page, double left, double top, double zoom)
        {
            TRN_Destination result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationCreateXYZ(page.mp_page, left, top, zoom, ref result));
            return new Destination(result, page.m_ref);
        }
	    /// <summary> Create a new 'Fit' Destination.
	    /// 
	    /// The new Destination displays the page designated by 'page', with its contents
	    /// magnified just enough to fit the entire page within the window both
	    /// horizontally and vertically. If the required horizontal and vertical
	    /// magnification factors are different, use the smaller of the two, centering
	    /// the page within the window in the other dimension.
	    /// 
	    /// </summary>
	    /// <param name="page">the page
	    /// </param>
	    /// <returns> the destination
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Destination CreateFit(Page page)
        {
            TRN_Destination result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationCreateFit(page.mp_page, ref result));
            return new Destination(result, page.m_ref);
        }
	    /// <summary> Create a new 'FitH' Destination.
	    /// 
	    /// The new Destination displays the page designated by 'page', with the
	    /// vertical coordinate 'top' positioned at the top edge of the window and
	    /// the contents of the page magnified just enough to fit the entire width
	    /// of the page within the window.
	    /// 
	    /// </summary>
	    /// <param name="page">the page
	    /// </param>
	    /// <param name="top">the top
	    /// </param>
	    /// <returns> the destination
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Destination CreateFitH(Page page, double top)
        {
            TRN_Destination result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationCreateFitH(page.mp_page, top, ref result));
            return new Destination(result, page.m_ref);
        }
	    /// <summary> Create a new 'FitV' Destination.
	    /// 
	    /// The new Destination displays the page designated by 'page', with the
	    /// horizontal coordinate 'left' positioned at the left edge of the window
	    /// and the contents of the page magnified just enough to fit the entire
	    /// height of the page within the window.
	    /// 
	    /// </summary>
	    /// <param name="page">the page
	    /// </param>
	    /// <param name="left">the left
	    /// </param>
	    /// <returns> the destination
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Destination CreateFitV(Page page, double left)
        {
            TRN_Destination result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationCreateFitV(page.mp_page, left, ref result));
            return new Destination(result, page.m_ref);
        }
	    /// <summary> Create a new 'FitR' Destination.
	    /// 
	    /// The new Destination displays the page designated by 'page', with its
	    /// contents magnified just enough to fit the rectangle specified by the
	    /// coordinates 'left', 'bottom', 'right', and 'top' entirely within the
	    /// window both horizontally and vertically. If the required horizontal
	    /// and vertical magnification factors are different, use the smaller of
	    /// the two, centering the rectangle within the window in the other
	    /// dimension.
	    /// 
	    /// </summary>
	    /// <param name="page">the page
	    /// </param>
	    /// <param name="left">the left
	    /// </param>
	    /// <param name="bottom">the bottom
	    /// </param>
	    /// <param name="right">the right
	    /// </param>
	    /// <param name="top">the top
	    /// </param>
	    /// <returns> the destination
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Destination CreateFitR(Page page,
            double left, double bottom, double right, double top)
        {
            TRN_Destination result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationCreateFitR(page.mp_page, left, bottom, right, top, ref result));
            return new Destination(result, page.m_ref);
        }
	    /// <summary> Create a new 'FitB' Destination.
	    /// 
	    /// The new Destination displays the page designated by 'page', with its
	    /// contents magnified just enough to fit its bounding box entirely within
	    /// the window both horizontally and vertically. If the required
	    /// horizontal and vertical magnification factors are different, use the
	    /// smaller of the two, centering the bounding box within the window in
	    /// the other dimension.
	    /// 
	    /// </summary>
	    /// <param name="page">the page
	    /// </param>
	    /// <returns> the destination
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Destination CreateFitB(Page page)
        {
            TRN_Destination result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationCreateFitB(page.mp_page, ref result));
            return new Destination(result, page.m_ref);
        }
	    /// <summary> Create a new 'FitBH' Destination.
	    /// 
	    /// The new Destination displays the page designated by 'page', with
	    /// the vertical coordinate 'top' positioned at the top edge of the window
	    /// and the contents of the page magnified just enough to fit the entire
	    /// width of its bounding box within the window.
	    /// 
	    /// </summary>
	    /// <param name="page">the page
	    /// </param>
	    /// <param name="top">the top
	    /// </param>
	    /// <returns> the destination
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Destination CreateFitBH(Page page, double top)
        {
            TRN_Destination result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationCreateFitBH(page.mp_page, top, ref result));
            return new Destination(result, page.m_ref);
        }
	    /// <summary> Create a new 'FitBV' Destination.
	    /// 
	    /// The new Destination displays Display the page designated by 'page',
	    /// with the horizontal coordinate 'left' positioned at the left edge of
	    /// the window and the contents of the page magnified just enough to fit
	    /// the entire height of its bounding box within the window.
	    /// 
	    /// </summary>
	    /// <param name="page">the page
	    /// </param>
	    /// <param name="left">the left
	    /// </param>
	    /// <returns> the destination
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Destination CreateFitBV(Page page, double left)
        {
            TRN_Destination result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationCreateFitBV(page.mp_page, left, ref result));
            return new Destination(result, page.m_ref);
        }

	    /// <summary>Sets value to given <c>Destination</c> object
	    /// </summary>
        /// <param name="p">given <c>Destination</c> object
        /// </param>
        public void Set(Destination p)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationCopy(p.mp_dest, ref mp_dest));
        }
	    /// <summary>Assignment operator</summary>
	    /// <param name="r"><c>Destination</c> object at the right of the operator
	    /// </param>
        /// <returns><c>Destination</c> object equals to the specified object
        /// </returns>
        public Destination op_Assign(Destination r)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationCopy(r.mp_dest, ref this.mp_dest));
            return this;
        }

	    /// <summary> Checks if is valid.
	    /// 
	    /// </summary>
	    /// <returns> True if this is a valid Destination and can be resolved, false otherwise.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  If this method returns false the underlying SDF/Cos object is null and
        /// the Action object should be treated as null as well.</remarks>
        public bool IsValid()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationIsValid(mp_dest, ref result));
            return result;
        }

	    /// <summary> Gets the fit type.
	    /// 
	    /// </summary>
	    /// <returns> destination's FitType.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public FitType GetFitType()
        {
            FitType result = new FitType();
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationGetFitType(mp_dest, ref result));
            return result;
        }
	    /// <summary> Gets the page.
	    /// 
	    /// </summary>
	    /// <returns> the Page that this destination refers to.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Page GetPage()
        {
            TRN_Page result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationGetPage(mp_dest, ref result));
            return result != IntPtr.Zero ? new Page(result, this.m_ref) : null;
        }
	    /// <summary> Modify the destination so that it refers to the new 'page' as the destination page.
	    /// 
	    /// </summary>
	    /// <param name="page">The new page associated with this Destination.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPage(Page page)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationSetPage(mp_dest, page.mp_page));
        }
	    /// <summary> Gets the SDFObj.
	    /// 
	    /// </summary>
	    /// <returns> the object to the underlying SDF/Cos object.
	    /// The returned SDF/Cos object is an explicit destination (i.e. the Obj is either
	    /// an array defining the destination, using the syntax shown in Table 8.2 in PDF
	    /// Reference Manual), or a dictionary with a 'D' entry whose value is such an
	    /// array. The latter form allows additional attributes to be associated with
	    /// the destination
	    /// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationGetSDFObj(mp_dest, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }
	    /// <summary> Gets the explicit dest obj.
	    /// 
	    /// </summary>
	    /// <returns> the explicit destination SDF/Cos object. This is always an Array
	    /// as shown in Table 8.2 in PDF Reference Manual.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetExplicitDestObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_DestinationGetExplicitDestObj(mp_dest, ref result));
            return result != IntPtr.Zero ? new Obj(result, this.m_ref) : null;
        }

        // Nested Types
        public enum FitType
        {
            /// <summary>Destination specified as upper-left corner point and a zoom factor.</summary>
            e_XYZ,
            /// <summary>Fits the page into the window</summary>
            e_Fit,
            /// <summary>Fits the widths of the page into the window</summary>
            e_FitH,
            /// <summary>Fits the height of the page into a window.</summary>
            e_FitV,
            /// <summary>Fits the rectangle specified by its upper-left and lower-right corner points into the window.</summary>
            e_FitR,
            /// <summary>Fits the rectangle containing all visible elements on the page into the window.</summary>
            e_FitB,
            /// <summary>Fits the width of the bounding box into the window.</summary>
            e_FitBH,
            /// <summary>Fits the height of the bounding box into the window.</summary>
            e_FitBV
        }
    }
}
