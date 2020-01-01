using System;

namespace pdftron.PDF
{
    public class CADConvertOptions : OptionsBase
    {
        public CADConvertOptions() : base()
        {
        }
            
        ///<summary>
        /// Gets the value PageHeight from the options object
        /// The height of the output pdf, in millimeters
        ///</summary>
        ///<returns>a double, The height of the output pdf, in millimeters.</returns>
        public double GetPageHeight()
        {
            SDF.Obj found = mDict.FindObj("Page-height");
            if(found != null)
            {
                return (found.GetNumber());
            }
            return (594.0);
        }

        ///<summary>
        /// Sets the value for PageHeight in the options object
        /// The height of the output pdf, in millimeters
        ///</summary>
        ///<param name="value">The height of the output pdf, in millimeters</param>
        ////<returns> this object, for call chaining</returns>
        public CADConvertOptions SetPageHeight(double value)
        {
            PutNumber("Page-height", value);
            return this;
        }

        
        ///<summary>
        /// Gets the value PageWidth from the options object
        /// The width of the output pdf, in millimeters
        ///</summary>
        ///<returns>a double, The width of the output pdf, in millimeters.</returns>
        public double GetPageWidth()
        {
            SDF.Obj found = mDict.FindObj("Page-width");
            if(found != null)
            {
                return (found.GetNumber());
            }
            return (840.0);
        }

        ///<summary>
        /// Sets the value for PageWidth in the options object
        /// The width of the output pdf, in millimeters
        ///</summary>
        ///<param name="value">The width of the output pdf, in millimeters</param>
        ////<returns> this object, for call chaining</returns>
        public CADConvertOptions SetPageWidth(double value)
        {
            PutNumber("Page-width", value);
            return this;
        }

        
        ///<summary>
        /// Gets the value RasterDPI from the options object
        /// Rasterization dpi used when rendering 3D content. Currently only applies to .rvt conversions
        ///</summary>
        ///<returns>a double, Rasterization dpi used when rendering 3D content. Currently only applies to .rvt conversions.</returns>
        public double GetRasterDPI()
        {
            SDF.Obj found = mDict.FindObj("Raster-dpi");
            if(found != null)
            {
                return (found.GetNumber());
            }
            return (72.0);
        }

        ///<summary>
        /// Sets the value for RasterDPI in the options object
        /// Rasterization dpi used when rendering 3D content. Currently only applies to .rvt conversions
        ///</summary>
        ///<param name="value">Rasterization dpi used when rendering 3D content. Currently only applies to .rvt conversions</param>
        ////<returns> this object, for call chaining</returns>
        public CADConvertOptions SetRasterDPI(double value)
        {
            PutNumber("Raster-dpi", value);
            return this;
        }

        
        ///<summary>
        /// Adds a  to the Sheets array
        /// The list of sheets to be converted -- only applies to .rvt conversions
        ///</summary>
        ///<param name="value">The list of sheets to be converted -- only applies to .rvt conversions</param>
        ////<returns> this object, for call chaining</returns>
        public CADConvertOptions AddSheets(String value)
        {
            PushBackText("Sheets", value);
            return this;
        }
        
    }
}
