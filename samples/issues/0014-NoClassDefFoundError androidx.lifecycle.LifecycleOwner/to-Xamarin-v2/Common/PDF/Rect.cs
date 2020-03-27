using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using pdftron.Common;
using pdftron.SDF;
using pdftronprivate.trn;

using TRN_Exception = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> Rect is a utility class used to manipulate PDF rectangle objects (refer to 
    /// section 3.8.3 of the PDF Reference Manual). 
    /// 
    /// Rect can be associated with a SDF/Cos rectangle array using Rect(Obj*) constructor 
    /// or later using Rect::Attach(Obj*) or Rect::Update(Obj*) methods. 
    /// 
    /// Rect keeps a local cache for rectangle points so it is necessary to call Rect::Update() 
    /// method if the changes to the Rect should be saved in the attached Cos/SDF array.
    /// 
    /// </summary>
    /// <remarks>Although rectangles are conventionally specified by their lower-left and
    /// upperright corners, it is acceptable to specify any two diagonally opposite corners.
    /// </remarks>
    public class Rect : IDisposable
    {
        internal BasicTypes.TRN_Rect mp_imp;

        /// <summary> Releases all resources used by the Rect </summary>
        ~Rect()
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
            // If disposing equals true, dispose all managed 
            // and unmanaged resources. 
            if (disposing)
            {
                // Dispose managed resources.

            }

            // Call the appropriate methods to clean up 
            // unmanaged resources here. 
            // If disposing is false, 
            // only the following code is executed.
            if (mp_imp.mp_rect != IntPtr.Zero)
            {
                mp_imp.mp_rect = IntPtr.Zero;
            }
        }

        // Methods
        internal Rect(BasicTypes.TRN_Rect imp)
        {
            this.mp_imp = imp;
        }
        /// <summary> Rect default constructor.
	    ///
        /// </summary>
        /// <exception cref="PDFNetException">PDFNetException the PDFNet exception </exception>
        public Rect()
        {
            mp_imp = new BasicTypes.TRN_Rect();

            PDFNetException.REX(PDFNetPINVOKE.TRN_RectInit(0, 0, 0, 0, ref mp_imp));
        }

	    /// <summary> Create a Rect and initialize it using given Cos/SDF rectangle Array object.
	    /// The rect is attached to this object.
	    /// 
	    /// </summary>
	    /// <param name="rect">the rect
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Rect(Obj rect)
        {
            mp_imp = new BasicTypes.TRN_Rect();

            PDFNetException.REX(PDFNetPINVOKE.TRN_RectAttach(ref mp_imp, rect.mp_obj));
        }
	    /// <summary> Create a Rect and initialize it using specified parameters.
	    /// The rect is not attached to any Cos/SDF object.
	    /// 
	    /// </summary>
	    /// <param name="x1">x coordinate of the bottom left corner
	    /// </param>
	    /// <param name="y1">y coordinate of the bottom left corner
	    /// </param>
	    /// <param name="x2">x coordinate of the top right corner
	    /// </param>
	    /// <param name="y2">y coordinate of the top right corner
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public Rect(double x1, double y1, double x2, double y2)
        {
            mp_imp = new BasicTypes.TRN_Rect();

            PDFNetException.REX(PDFNetPINVOKE.TRN_RectInit(x1, y1, x2, y2, ref mp_imp));
        }

        /// <summary> Set the coordinates of the rectangle.
	    /// 
	    /// </summary>
	    /// <param name="p">the r
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public void Set(Rect p) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectAssign(ref mp_imp, ref p.mp_imp));
        }
	    /// <summary>Assignment operator</summary>
	    /// <param name="rr">a <c>Rect</c> object at the right of the operator
	    /// </param>
	    /// <returns>a <c>Rect</c> object equals to the given <c>Rect</c> object
	    /// </returns>
        public Rect op_Assign(Rect rr)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectAssign(ref mp_imp, ref rr.mp_imp));
            return this;
        }
	    /// <summary> Attach the Cos/SDF object to the Rect.
	    /// 
	    /// </summary>
	    /// <param name="obj">- underlying Cos/SDF object. Must be an SDF::Array with four
	    /// SDF::Number elements.
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Attach(Obj obj)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectAttach(ref mp_imp, obj.mp_obj));
        }
	    /// <summary> Saves changes made to the Rect object in the attached (or specified) SDF/Cos rectangle.
	    /// 
	    /// </summary>
	    /// <returns> true if the attached Cos/SDF rectangle array was successfully updated,
	    /// false otherwise.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool Update()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectUpdate(ref mp_imp, IntPtr.Zero, ref result));
            return result;
        }
	    /// <summary> Saves changes made to the Rect object in the attached (or specified) SDF/Cos rectangle.
	    /// 
	    /// </summary>
	    /// <param name="obj">- an SDF array that should be
	    /// updated and attached to this Rect. If parameter rect is NULL or is omitted, update
	    /// is performed on previously attached Cos/SDF rectangle.
	    /// </param>
	    /// <returns> true if the attached Cos/SDF rectangle array was successfully updated,
	    /// false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool Update(Obj obj)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectUpdate(ref mp_imp, obj.mp_obj, ref result));
            return result;
        }
	    /// <summary> Gets the Rectangle coordinates	
	    /// </summary>
	    /// <param name="out_x1">x coordinate of the bottom left corner</param>
	    /// <param name="out_y1">y coordinate of the bottom left corner</param>
	    /// <param name="out_x2">x coordinate of the top right corner</param>
        /// <param name="out_y2">y coordinate of the top right corner</param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Get(double out_x1, double out_y1, double out_x2, double out_y2)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectGet(ref mp_imp, ref out_x1, ref out_y1, ref out_x2, ref out_y2));
        }

	    /// <summary> Set the coordinates of the rectangle.
	    /// 
	    /// </summary>
	    /// <param name="x1">x coordinate of the bottom left corner
	    /// </param>
	    /// <param name="y1">y coordinate of the bottom left corner
	    /// </param>
	    /// <param name="x2">x coordinate of the top right corner
	    /// </param>
	    /// <param name="y2">y coordinate of the top right corner
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void Set(double x1, double y1, double x2, double y2)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectSet(ref mp_imp, x1, y1, x2, y2));
        }

	    /// <summary> Gets the width.
	    /// 
	    /// </summary>
	    /// <returns> rectangle's width
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double Width()
        {
            double result = Double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectWidth(ref mp_imp, ref result));
            return result;
        }
	    /// <summary> Gets the height.
	    /// 
	    /// </summary>
	    /// <returns> rectangle's height
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public double Height()
        {
            double result = Double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectHeight(ref mp_imp, ref result));
            return result;
        }
	    /// <summary> Determines if the specified point is contained within the rectangular region
	    /// defined by this Rectangle.
	    /// 
	    /// </summary>
	    /// <param name="x">x coordinate of the specified point
	    /// </param>
	    /// <param name="y">y coordinate of the specified point
	    /// </param>
	    /// <returns> true is the point is in the rectangle, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public bool Contains(double x, double y)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectContains(ref mp_imp, x, y, ref result));
            return result;
        }
	    /// <summary> Makes a Rect equal to the intersection of two existing rectangles.
	    /// 
	    /// </summary>
	    /// <param name="rect1">- A Rect object that contains a source rectangle.
	    /// </param>
	    /// <param name="rect2">- A Rect object that contains a source rectangle.
	    /// </param>
	    /// <returns> true if the intersection is not empty; 0 if the intersection is empty.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  The intersection is the largest rectangle contained in both existing rectangles. </remarks>
        public bool IntersectRect(Rect rect1, Rect rect2)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectIntersectRect(ref mp_imp, ref rect1.mp_imp, ref rect2.mp_imp, ref result));
            return result;
        }
	    /// <summary> Normalizes the rectagle to the one with lower-left and upper-right corners.
	    /// 
	    /// </summary>        
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  Although rectangles are conventionally specified by their lower-left 
	    /// and upper-right corners, it is acceptable to specify any two diagonally
        /// opposite corners.
        /// </remarks>
        public void Normalize()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectNormalize(ref mp_imp));
        }
	    /// <summary> Inflate.
	    /// 
	    /// </summary>
        /// <param name="amount"> amount of inflate
        /// </param>
        public void Inflate(double amount)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectInflate1(ref mp_imp, amount));
        }
	    /// <summary> Inflate.
	    /// 
	    /// </summary>
	    /// <param name="x">the x
	    /// </param>
        /// <param name="y">the y
        /// </param>
        public void Inflate(double x, double y)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_RectInflate2(ref mp_imp, x, y));
        }

	    ///<summary> x coordinate of the bottom left corner
	    ///</summary>
	    public double x1 
	    {
		    get
		    {
                return PDFNetPINVOKE.TRN_RectGetX1(ref mp_imp);
		    }

		    set
		    {
                PDFNetPINVOKE.TRN_RectSetX1(ref mp_imp, value);
		    }
	    }

	    ///<summary> y coordinate of the bottom left corner
	    ///</summary>
	    public double y1 
	    {
		    get
		    {
                return PDFNetPINVOKE.TRN_RectGetY1(ref mp_imp);
		    }

		    set
		    {
                PDFNetPINVOKE.TRN_RectSetY1(ref mp_imp, value);
		    }
	    }

	    ///<summary> x coordinate of the top right corner
	    ///</summary>
	    public double x2 
	    {
		    get
		    {
                return PDFNetPINVOKE.TRN_RectGetX2(ref mp_imp);
		    }

		    set
		    {
                PDFNetPINVOKE.TRN_RectSetX2(ref mp_imp, value);
		    }
	    }

	    ///<summary> y coordinate of the top right corner
	    ///</summary>
	    public double y2 
	    {
		    get
		    {
                return PDFNetPINVOKE.TRN_RectGetY2(ref mp_imp);
		    }

		    set
		    {
                PDFNetPINVOKE.TRN_RectSetY2(ref mp_imp, value);
		    }
	    }
    }
}
