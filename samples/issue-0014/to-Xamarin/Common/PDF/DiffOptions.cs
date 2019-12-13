namespace pdftron.PDF
{
    public class DiffOptions : OptionsBase
    {
        public DiffOptions() : base()
        {
        }

        ///<summary>
        /// Gets the value BlendMode from the options object
        /// How the two modes should be blended
        ///</summary>
        ///<returns>a GState.BlendMode, How the two modes should be blended.</returns>
        public GState.BlendMode GetBlendMode()
        {
            SDF.Obj  found = mDict.FindObj("BlendMode");
            if (!found.IsNull())
            {
                return (GState.BlendMode)(int)(found.GetNumber());
            }
            return (GState.BlendMode)(int)(5);
        }

        ///<summary>
        /// Sets the value for BlendMode in the options object
        /// How the two modes should be blended
        ///</summary>
        ///<param name="value">How the two modes should be blended</param>
        ////<returns> this object, for call chaining</returns>
        public DiffOptions SetBlendMode(GState.BlendMode value)
        {
            mDict.PutNumber("BlendMode", (double)(value));
            return this;
        }


        ///<summary>
        /// Gets the value ColorA from the options object
        /// The difference color for the first page
        ///</summary>
        ///<returns>a ColorPt, The difference color for the first page.</returns>
        public ColorPt GetColorA()
        {
            SDF.Obj  found = mDict.FindObj("ColorA");
            if (!found.IsNull())
            {
                return ColorPtFromNumber(found.GetNumber());
            }
            return ColorPtFromNumber(0xFFCC0000);
        }

        ///<summary>
        /// Sets the value for ColorA in the options object
        /// The difference color for the first page
        ///</summary>
        ///<param name="value">The difference color for the first page</param>
        ////<returns> this object, for call chaining</returns>
        public DiffOptions SetColorA(ColorPt value)
        {
            mDict.PutNumber("ColorA", ColorPtToNumber(value));
            return this;
        }


        ///<summary>
        /// Gets the value ColorB from the options object
        /// The difference color for the second page
        ///</summary>
        ///<returns>a ColorPt, The difference color for the second page.</returns>
        public ColorPt GetColorB()
        {
            SDF.Obj  found = mDict.FindObj("ColorB");
            if (!found.IsNull())
            {
                return ColorPtFromNumber(found.GetNumber());
            }
            return ColorPtFromNumber(0xFF00CCCC);
        }

        ///<summary>
        /// Sets the value for ColorB in the options object
        /// The difference color for the second page
        ///</summary>
        ///<param name="value">The difference color for the second page</param>
        ////<returns> this object, for call chaining</returns>
        public DiffOptions SetColorB(ColorPt value)
        {
            mDict.PutNumber("ColorB", ColorPtToNumber(value));
            return this;
        }
    }
}
