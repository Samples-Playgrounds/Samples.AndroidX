using System;

namespace pdftron.PDF
{
    public class OCROptions : OptionsBase
    {
        public OCROptions() : base()
        {
        }
            
        ///<summary>
        /// Adds a collection of ignorable regions for the given page
        /// Optional list of page areas that will be not be processed
        ///</summary>
        ///<param name="regions">optional list of page areas to be excluded from analysis</param>
        ///<param name="pageNum">the page number the added regions belong to</param>
        ////<returns> this object, for call chaining</returns>
        public OCROptions AddIgnoreZonesForPage(RectCollection regions, int pageNum)
        {
            insertRectCollection("IgnoreZones", regions, pageNum - 1);
            return this;
        }
        
        ///<summary>
        /// Adds a  to the Langs array
        /// The list of languages
        ///</summary>
        ///<param name="value">The list of languages</param>
        ////<returns> this object, for call chaining</returns>
        public OCROptions AddLang(String value)
        {
            PushBackText("Langs", value);
            return this;
        }
        
        ///<summary>
        /// Adds a collection of known text regions for the given page.
        /// This information will be used as a hint to improve OCR quality.
        ///</summary>
        ///<param name="regions">optional list of known text regions</param>
        ///<param name="pageNum">the page number the added regions belong to</param>
        ////<returns> this object, for call chaining</returns>
        public OCROptions AddTextZonesForPage(RectCollection regions, int pageNum)
        {
            insertRectCollection("TextZones", regions, pageNum - 1);
            return this;
        }
        
    }
}
