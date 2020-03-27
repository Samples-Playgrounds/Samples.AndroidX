using System;
using System.Collections.Generic;
using System.Text;

using pdftron.Common;
using pdftronprivate.trn;

using TRN_Exception = System.IntPtr;

namespace pdftron.Common
{
    /// <summary> 2D Matrix
    /// 
    /// A Matrix2D object represents a 3x3 matrix that, in turn, represents an affine transformation. 
    /// A Matrix2D object stores only six of the nine numbers in a 3x3 matrix because all 3x3 
    /// matrices that represent affine transformations have the same third column (0, 0, 1).
    /// 
    /// Affine transformations include rotating, scaling, reflecting, shearing, and translating. 
    /// In PDFNet, the Matrix2D class provides the foundation for performing affine transformations 
    /// on vector drawings, images, and text.
    /// 
    /// A transformation matrix specifies the relationship between two coordinate spaces.
    /// By modifying a transformation matrix, objects can be scaled, rotated, translated,
    /// or transformed in other ways.
    /// 
    /// A transformation matrix in PDF is specified by six numbers, usually in the form
    /// of an array containing six elements. In its most general form, this array is denoted
    /// [a b c d h v]; The following table lists the arrays that specify the most common
    /// transformations:
    /// 
    /// <list type="bullet">
    /// <item>   
    /// <description>   
    /// Translations are specified as [1 0 0 1 tx ty], where tx and ty are the distances
    /// to translate the origin of the coordinate system in the horizontal and vertical
    /// dimensions, respectively.
    /// </description>
    /// </item>
    /// <item>   
    /// <description>
    /// Scaling is obtained by [sx 0 0 sy 0 0]. This scales the coordinates so that 1
    /// unit in the horizontal and vertical dimensions of the new coordinate system is
    /// the same size as sx and sy units, respectively, in the previous coordinate system.
    /// </description>
    /// </item>
    /// <item>   
    /// <description>
    /// Rotations are produced by [cos(A) sin(A) -sin(A) cos(A) 0 0], which has the effect
    /// of rotating the coordinate system axes by an angle 'A' counterclockwise.
    /// </description>
    /// </item>
    /// <item>   
    /// <description>
    /// Skew is specified by [1 tan(A) tan(B) 1 0 0], which skews the x axis by an angle
    /// A and the y axis by an angle B.
    ///</description>
    /// </item>
    /// 
    /// </list>
    /// Matrix2D elements are positioned as follows :
    /// 
    /// | m_a m_b 0 |
    /// | m_c m_d 0 |
    /// | m_h m_v 1 |
    ///
    /// A single Matrix2D object can store a single transformation or a sequence of transformations. 
    /// The latter is called a composite transformation. The matrix of a composite transformation is 
    /// obtained by multiplying (concatenating) the matrices of the individual transformations. 
    /// Because matrix multiplication is not commutative-the order in which matrices are multiplied
    /// is significant. For example, if you first rotate, then scale, then translate, you get a 
    /// different result than if you first translate, then rotate, then scale.
    /// 
    /// For more information on properties of PDF matrices please refer to PDF Reference Manual 
    /// (Sections 4.2 'Coordinate Systems' and 4.2.3 'Transformation Matrices')
    /// 	
    /// 
    /// <example>
    /// The following sample illustrates how to use Matrix2D in order to position
    /// an image on the page. Note that PDFNet uses the same convention of matrix 
    /// multiplication used in PostScript and OpenGL.
    /// <code> 
    /// Element element = eb.CreateImage(Image(...));
    /// double deg2rad = 3.1415926535 / 180.0;
    /// 
    /// Matrix2D mtx = Matrix2D(1, 0, 0, 1, 0, 200); // Translate
    /// mtx.multiply(Matrix2D(300, 0, 0, 200, 0, 0));    // Scale
    /// mtx.multiply(Matrix2D.RotationMatrix( 90 * deg2rad )); // Rotate
    /// element.GetGState().SetTransform(mtx);
    /// writer.WritePlacedElement(element);	
    /// </code>
    ///
    /// The following sample sample illustrates how to use Matrix2D in order to calculate 
    /// absolute positioning for the text on the page.
    /// <code>
    /// ...
    /// Matrix2D text_mtx = text_element.GetTextMatrix();
    /// double x, y;
    /// for (CharIterator itr = text_element.getCharIterator(); itr.HasNext(); itr.Next()) {
    /// x = itr.current().x; // character positioning information
    /// y = itr.current().y;
    /// </code>
    /// 
    /// 
    /// Get current transformation matrix (CTM)
    /// <code>
    /// Matrix2D ctm = text_element.getCTM();
    /// </code>
    /// 
    /// To get the absolute character positioning information concatenate current 
    /// text matrix with CTM and then multiply relative postitioning coordinates with 
    /// the resulting matrix.
    /// <code>
    /// Matrix2D mtx = ctm.multiply(text_mtx);
    /// mtx.multPoint(x, y);	
    /// </code>	
    /// </example>
    /// </summary>
    public class Matrix2D : IDisposable
    {
        internal BasicTypes.TRN_Matrix2D mp_mtx;

        /// <summary> Releases all resources used by the Matrix2D </summary>
        ~Matrix2D()
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
            if (disposing)
            {
                // Dispose managed resources.
            }
        }

        internal Matrix2D(BasicTypes.TRN_Matrix2D mtx)
        {
            this.mp_mtx = mtx;
        }

        /// <summary> Creates an identity matrix 
		/// 
		/// </summary>
		/// <throws>  PDFNetException  </throws>
        public Matrix2D()
        {
            double a = 1;
            double b = 0;
            double c = 0;
            double d = 1;
            double h = 0;
            double v = 0;
            //mp_mtx = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DSet(ref mp_mtx, a, b, c, d, h, v));
        }
        ///<summary>Create a matrix and initialize it with values from another matrix
		///</summary>
		///<param name="m">matrix to initialize
		///</param>
		public Matrix2D(Matrix2D m)
        {
            //mp_mtx = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DCopy(ref m.mp_mtx, ref mp_mtx));
        }
        /// <summary> Creates a Matrix object based on six numbers that define an
		/// affine transformation.
		/// 
		/// </summary>
		/// <param name="a">the matrix element in the first row, first column.
		/// </param>
		/// <param name="b">the matrix element in the first row, second column.
		/// </param>
		/// <param name="c">the matrix element in the second row, first column.
		/// </param>
		/// <param name="d">the matrix element in the second row, second column.
		/// </param>
		/// <param name="h">the matrix element in the third row, first column.
		/// </param>
		/// <param name="v">the matrix element in the third row, second column.
		/// </param>
		/// <throws>  PDFNetException  </throws>
        public Matrix2D(double a, double b, double c, double d, double h, double v)
        {
            //mp_mtx = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DSet(ref mp_mtx, a, b, c, d, h, v));
        }

        ///<summary> the matrix element in the first row, first column.
		///</summary>
		public Double m_a 
		{
			get
			{
				Double t = mp_mtx.m_a;
				return t;
			}

			set
			{
				mp_mtx.m_a = value;
			}

		}
		///<summary> the matrix element in the first row, second column
		///</summary>
		public Double m_b 
		{
			get
			{
				Double t = mp_mtx.m_b;
				return t;
			}

			set
			{
				mp_mtx.m_b = value;
			}

		}
		///<summary>the matrix element in the second row, first column
		///</summary>
		public Double m_c 
		{
			get
			{
				Double t = mp_mtx.m_c;
				return t;
			}

			set
			{
				mp_mtx.m_c = value;
			}

		}
		///<summary>the matrix element in the second row, second column.
		///</summary>
		public Double m_d 
		{
			get
			{
				Double t = mp_mtx.m_d;
				return t;
			}

			set
			{
				mp_mtx.m_d = value;
			}

		}
		///<summary>the matrix element in the third row, first column.
		///</summary>
		public Double m_h 
		{
			get
			{
				Double t = mp_mtx.m_h;
				return t;
			}

			set
			{
				mp_mtx.m_h = value;
			}

		}
		///<summary>the matrix element in the third row, second column.
		///</summary>
		public Double m_v 
		{
			get
			{
				Double t = mp_mtx.m_v;
				return t;
			}

			set
			{
				mp_mtx.m_v = value;
			}

		}
		/// <summary> The Set method sets the elements of this matrix.
		/// 
		/// </summary>
		/// <param name="a">the matrix element in the first row, first column. 
		/// </param>
		/// <param name="b">the matrix element in the first row, second column. 
		/// </param>
		/// <param name="c">the matrix element in the second row, first column. 
		/// </param>
		/// <param name="d">the matrix element in the second row, second column. 
		/// </param>
		/// <param name="h">the matrix element in the third row, first column. 
		/// </param>
		/// <param name="v">the matrix element in the third row, second column. 
		/// </param>
		public void Set( Double a, Double b, Double c, Double d, Double h, Double v ) 
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DSet(ref mp_mtx, a, b, c, d, h, v));
        }
		///<summary>set value to given matrix
		///</summary>
		///<param name="p">matrix value for the created matrix
		///</param>
		public void Set(Matrix2D p)
        {
            this.mp_mtx = p.mp_mtx;
        }
		/// <summary> The Concat method updates this matrix with the product of itself and another matrix
		/// specified through an argument list.
		/// 
		/// </summary>
		/// <param name="a">the matrix element in the first row, first column.
		/// </param>
		/// <param name="b">the matrix element in the first row, second column.
		/// </param>
		/// <param name="c">the matrix element in the second row, first column.
		/// </param>
		/// <param name="d">the matrix element in the second row, second column.
		/// </param>
		/// <param name="h">the matrix element in the third row, first column.
		/// </param>
		/// <param name="v">the matrix element in the third row, second column.
		/// </param>
		/// <throws>  PDFNetException  </throws>
		public void Concat( Double a, Double b, Double c, Double d, Double h, Double v )
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DConcat(ref mp_mtx, a, b, c, d, h, v));
        }


		///<summary>assign value from another matrix
		///</summary>
		///<param name="rm"> another matrix
		///</param>
		///<returns>matrix with value from <c>rm</c>
		///</returns>
		public Matrix2D op_Assign(Matrix2D rm )
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DCopy(ref rm.mp_mtx, ref mp_mtx));
            return this;
        }
		//static Matrix2D Assign( Matrix2D lm, Matrix2D rm );

		///<summary>multiply two matrices
		///</summary>
		///<param name="lm"> left matrix
		///</param>
		///<param name="rm"> right matrix
		///</param>
		///<returns> multiplication result matrix
		///</returns>
		public static Matrix2D operator*( Matrix2D lm, Matrix2D rm )
        {
            BasicTypes.TRN_Matrix2D result = lm.mp_mtx;
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DConcat(ref result, rm.m_a, rm.m_b, rm.m_c, rm.m_d, rm.m_h, rm.m_v));
            return new Matrix2D(result);
        }
		///<summary>check if two matrices are equal
		///</summary>
		///<param name="lm"> left matrix
		///</param>
		///<param name="rm"> right matrix
		///</param>
        ///<returns> true, if both matrices are equal. false, otherwise.
        ///</returns>
        public static Boolean operator ==(Matrix2D lm, Matrix2D rm)
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DEquals(ref lm.mp_mtx, ref rm.mp_mtx, ref result));
            return result;
        }
		///<summary>check if two matrices are inequal
		///</summary>
		///<param name="lm"> left matrix
		///</param>
		///<param name="rm"> right matrix
		///</param>
        ///<returns> true, if both matrices are not equal, false, otherwise.
        ///</returns>
        public static Boolean operator !=(Matrix2D lm, Matrix2D rm)
        {
            return !(lm == rm);
        }
		///<summary> Transform/multiply the point (in_out_x, in_out_y) using this matrix
		///</summary>
		///<param name="in_out_x">x coordinate of the result point
		///</param>
        ///<param name="in_out_y">y coordinate of the result point
        ///</param>	  
        public void Mult(ref double in_out_x, ref double in_out_y)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DMult(ref mp_mtx, ref in_out_x, ref in_out_y));
        }
		/// <summary> If this matrix is invertible, the Inverse method returns its inverse matrix.
		/// 
		/// </summary>
		/// <returns> inverse of the matrix
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public Matrix2D Inverse()
        {
            BasicTypes.TRN_Matrix2D result = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DInverse(ref mp_mtx, ref result));
            return new Matrix2D(result);
        }
		/// <summary> The Translate method updates this matrix with the product of itself and a
		/// translation matrix (i.e. it is equivalent to this.m_h += h; this.m_v += v).
		/// 
		/// </summary>
		/// <param name="x">the horizontal component of the translation.
		/// </param>
		/// <param name="y">the vertical component of the translation.
		/// </param>
		/// <returns> translated matrix
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public Matrix2D Translate(Double x, Double y)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DTranslate(ref mp_mtx, x, y));
            return this;
        }
		/// <summary> The Scale method updates this matrix with the product of itself and a scaling matrix.
		/// 
		/// </summary>
		/// <param name="x">the horizontal scale factor.
		/// </param>
		/// <param name="y">the vertical scale factor
		/// </param>
		/// <returns> translated matrix
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public Matrix2D Scale(Double x, Double y)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DScale(ref mp_mtx, x, y));
            return this;
        }
		/// <summary> Create zero matrix (0 0 0 0 0 0).
		/// 
		/// </summary>
		/// <returns> zero matrix
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public static Matrix2D ZeroMatrix()
        {
            BasicTypes.TRN_Matrix2D result = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DCreateZeroMatrix(ref result));
            return new Matrix2D(result);
        }
		/// <summary> Create identity matrix 
		/// {1 0 
		///  0 1 
		///  0 0}.
		/// 
		/// </summary>
		/// <returns> the identity matrix
        /// </returns>
        /// <throws>  PDFNetException  </throws>
        public static Matrix2D IdentityMatrix()
        {
            BasicTypes.TRN_Matrix2D result = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DCreateIdentityMatrix(ref result));
            return new Matrix2D(result);
        }
		/// <summary> Rotation matrix.
		/// 
		/// </summary>
		/// <param name="angle">the angle of rotation in radians.
		/// Positive values specify clockwise rotation.
		/// </param>
		/// <returns> A rotation matrix for a given angle.
		/// </returns>
		/// <throws>  PDFNetException  </throws>
        public static Matrix2D RotationMatrix(Double angle)
        {
            BasicTypes.TRN_Matrix2D result = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_Matrix2DCreateRotationMatrix(angle, ref result));
            return new Matrix2D(result);
        }
    }
}
