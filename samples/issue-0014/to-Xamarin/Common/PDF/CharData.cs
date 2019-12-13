using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

namespace pdftron.PDF
{
    /// <summary> CharData is a data structure returned by CharIterator that is 
    /// used to provide extra information about a character within 
    /// a text run. The extra information includes positioning information, 
    /// the character data and a number of bytes taken by the character.
    /// </summary>
    public class CharData : IDisposable
    {
        internal BasicTypes.TRN_CharData mp_imp;
        internal CharData(BasicTypes.TRN_CharData imp)
        {
            this.mp_imp = imp;
        }

        /// <summary> Releases all resources used by the CharData </summary>
        ~CharData()
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
            if (mp_imp.char_data != IntPtr.Zero)
            {
                mp_imp.char_data = IntPtr.Zero;
            }
        }

        /// <summary> Gets the char code.
	    /// 
	    /// </summary>
	    /// <returns> Char code' For SimpleFonts char_code := char_data[0]
	    /// for composite fonts char_code is the numeric value of data stored in char_data buffer.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public int char_code 
	    {
		    get
		    {
			    Int32 i = System.Convert.ToInt32(mp_imp.char_code);
			    return i;
		    }
		    set
		    {
			    mp_imp.char_code = System.Convert.ToUInt32(value);
		    }
	    }
	    /// <summary> To render text, a virtual point (x, y), located on the baseline,
	    /// called the pen position, is used to locate glyphs.
	    /// 
	    /// The pen position has already taken into account the effects of
	    /// any inter-character adjustments due to properties such as font size,
	    /// text rise, character spacing, word spacing and positioning adjustments
	    /// on 'TJ' elements.
	    /// 
	    /// </summary>
	    /// <returns> glyph horizontal position
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public double x 
	    {
		    get
            {
			    Double d = mp_imp.x;
			    return d;
		    }
		    set
            {
			    mp_imp.x = value;
		    }
	    }
	    /// <summary> Gets the glyph y.
	    /// 
	    /// </summary>
	    /// <returns> glyph vertical position
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public double y 
	    {
		    get
            {
			    Double d = mp_imp.y;
			    return d;
		    }
		    set
            {
			    mp_imp.y = value;
		    }
	    }

	    /// <summary> Gets the char data.
	    /// 
	    /// </summary>
	    /// <returns> A buffer containing character data.
	    /// For simple fonts each character is represented by a single byte.
	    /// For multibyte (CID or Type0) fonts each character may take more than
	    /// one byte.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public byte[] char_data
	    {
		    get
            {
                IntPtr c = mp_imp.char_data;
                byte[] buf = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(c, buf, 0, bytes);
			    return buf;
		    }
	    }

	    /// <summary>the number of bytes representing this character in char_data 
	    /// buffer.  For simple fonts 'bytes' will equal 1. For multibyte (CID or Type0) 
	    /// fonts 'bytes may be larger than 1.
	    /// </summary>
	    /// <returns>the number of bytes representing this character in char_data 
	    /// buffer. 1 for symple fonts.
	    /// </returns>
	    public int bytes 
	    {
		    get
            {
			    Int32 i = mp_imp.bytes;
			    return i;
		    }
		    set
            {
			    mp_imp.bytes = value;
		    }
	    }
    }
}