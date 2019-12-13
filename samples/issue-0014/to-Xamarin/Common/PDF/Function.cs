using System;
using System.Collections.Generic;
using System.Text;

using pdftron.Common;
using pdftron.SDF;

using TRN_Function = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> Although PDF is not a programming language it provides several types of function 
    /// object that represent parameterized classes of functions, including mathematical 
    /// formulas and sampled representations with arbitrary resolution. Functions are used 
    /// in various ways in PDF, including device-dependent rasterization information for 
    /// high-quality printing (halftone spot functions and transfer functions), color 
    /// transform functions for certain color spaces, and specification of colors as a 
    /// function of position for smooth shadings. Functions in PDF represent static, 
    /// self-contained numerical transformations.
    /// 
    /// PDF::Function represents a single, flat interface around all PDF function types.
    /// </summary>
    public class Function : IDisposable
    {
        internal TRN_Function mp_func = IntPtr.Zero;
        internal Object m_ref;

        internal Function(TRN_Function imp, Object reference)
        {
            this.mp_func = imp;
            this.m_ref = reference;
        }

        /// <summary> Create a PDF::Function object from an existing SDF function dictionary.
	    /// 
	    /// </summary>
	    /// <param name="function">the funct_dict
	    /// </param>
        /// <returns>newly created <c>Function</c> object
        /// </returns>
        public static Function Create(Obj function)
        {
            TRN_Function result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FunctionCreate(function.mp_obj, ref result));
            return new Function(result, function.GetRefHandleInternal());
        }

        /// <summary> Gets the type.
	    /// 
	    /// </summary>
	    /// <returns> The function type
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public virtual Type GetType()
        {
            Type result = Type.e_sampled;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FunctionGetType(mp_func, ref result));
            return result;
        }

	    /// <summary> Gets the input cardinality.
	    /// 
	    /// </summary>
	    /// <returns> the number of input components required by the function
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 GetInputCardinality()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FunctionGetInputCardinality(mp_func, ref result));
            return result;
        }
	    /// <summary> Gets the output cardinality.
	    /// 
	    /// </summary>
	    /// <returns> the number of output components returned by the function
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 GetOutputCardinality()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FunctionGetOutputCardinality(mp_func, ref result));
            return result;
        }
	    //virtual void Eval(Double (*in)[], Double (*out)[]);
	    /// <summary> Evaluate the function at a given point.
	    /// 
	    /// </summary>
	    /// <param name="in">the in
	    /// </param>
	    /// <param name="out"> the double[]
	    /// </param>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  that size of 'in' array must be greater than or equal to function input cardinality.
        /// </remarks>
        public virtual void Eval(double[] @in, double[] @out)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FunctionEval(mp_func, @in, @out));
        }
	    /// <summary> Gets the SDFObj.
	    /// 
	    /// </summary>
	    /// <returns> the underlying SDF/Cos object
	    /// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FunctionGetSDFObj(mp_func, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
	    /// <summary> Releases all resources used by the Function </summary>
        ~Function()
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
            Destroy();
        }
        public void Destroy()
        {
            if (mp_func != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_FunctionDestroy(mp_func));
                mp_func = IntPtr.Zero;
            }
        }

         // Nested Types
        ///<summary>functions types</summary>
        public enum Type
        {
            ///<summary>uses a table of sample values to define the function.</summary>
		    e_sampled     = 0,  
		    ///<summary>defines a set of coefficients for an exponential function.</summary>
		    e_exponential = 2,  
		    ///<summary>a combination of other functions, partitioned across a domain.</summary>
		    e_stitching   = 3,  
		    ///<summary>A PostScript calculator function.</summary>
		    e_postscript  = 4  
        }

    }
}
