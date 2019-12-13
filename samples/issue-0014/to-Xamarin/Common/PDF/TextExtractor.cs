using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.Common;

using TRN_TextExtractor = System.IntPtr;
using TRN_Obj = System.IntPtr;

namespace pdftron.PDF
{
    /// <summary> 
    /// <param>
    /// TextExtractor is used to analyze a PDF page and extract words and logical 
    /// structures that are visible within a given region. The resulting list of 
    /// lines and words can be traversed element by element or accessed as a 
    /// string buffer. The class also includes utility methods to extract PDF 
    /// text as HTML or XML.	
    /// </param>
    /// 
    /// Possible use case scenarios for TextExtractor include:
    /// <list type="bullet">
    /// <item><description>Converting PDF pages to text or XML for content repurposing.</description></item>
    /// <item><description>Searching PDF pages for specific words or keywords.</description></item>
    /// <item><description>Indexing large PDF repositories for indexing or content.</description></item>
    /// </list>
    /// retrieval purposes (i.e. implementing a PDF search engine).
    /// <list type="bullet">    
    /// <item><description>Classifying or summarizing PDF documents based on their text content.</description></item>
    /// <item><description>Finding specific words for content editing purposes (such as splitting pages.</description></item> 
    /// based on keywords etc).
    /// </list>
    /// The main task of TextExtractor is to interpret PDF pages and offer a 
    /// simple to use API to: 
    /// <list type="bullet">
    /// <item><description>Normalize all text content to Unicode.</description></item>
    /// <item><description>Extract inferred logical structure (word by word, line by line, 
    /// or paragraph by paragraph).</description></item>
    /// <item><description>Extract positioning information for every line, word, or a glyph.</description></item>
    /// <item><description>Extract style information (such as information about the font, font size,
    /// font styles, etc) for every line, word, or a glyph.</description></item>
    /// <item><description>Control the content analysis process. A number of options (such as 
    /// removal of text obscured by images) is available to let the user 
    /// direct the flow of content recognition algorithms that will meet their
    /// requirements.</description></item>
    /// <item><description>Offer utility methods to convert PDF page content to text, XML, or HTML.</description></item>
    /// </list>
    /// 
    /// <remarks>
    /// <para>TextExtractor is analyzing only textual content of the page.
    /// This means that the rasterized (e.g. in scanned pages) or vectorized
    /// text (where glyphs are converted to path outlines) will not be recognized 
    /// as text. Please note that it is still possible to extract this content 
    /// using pdftron.PDF.ElementReader interface.
    /// </para>
    /// <para>
    /// In some cases TextExtractor may extract text that does not appear to 
    /// be on the visible page (e.g. when text is obscured by an image or a 
    /// rectangle). In these situations it is possible to use processing flags
    /// such as 'e_remove_hidden_text' and 'e_no_invisible_text' to remove 
    /// hidden text. 
    /// </para>
    /// </remarks>
    /// <example>
    /// For full sample code, please take a look at TextExtract sample project.    
    /// <code>  
    /// //... Initialize PDFNet ...
    /// PDFDoc doc = new PDFDoc(filein);
    /// doc.initSecurityHandler();
    /// Page page = doc.pageBegin().current();
    /// TextExtractor txt = new TextExtractor();
    /// txt.begin(page, 0, TextExtractor.ProcessingFlags.e_remove_hidden_text);
    /// string text = txt.getAsText();
    /// // or traverse words one by one...
    /// TextExtractor.Word word;
    /// for (TextExtractor.Line line = txt.GetFirstLine(); line.IsValid(); line=line.GetNextLine()) {
    /// for (word=line.GetFirstWord(); word.IsValid(); word=word.GetNextWord()) {
    /// string w = word.GetString();
    /// }
    /// }
    /// </code>
    /// </example>
    /// </summary>
    public class TextExtractor : IDisposable
    {
        internal TRN_TextExtractor mp_extractor = IntPtr.Zero;
        internal Object m_ref;
        internal TextExtractor(TRN_TextExtractor imp, Object reference)
        {
            this.mp_extractor = imp;
            this.m_ref = reference;
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
            if (mp_extractor != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorDestroy(mp_extractor));
                mp_extractor = IntPtr.Zero;
            }
        }
        /// <summary> Constructor and destructor.</summary>
        public TextExtractor()
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorCreate(ref mp_extractor));
        }
        /// <summary> Start reading the page. </summary>
        /// <param name="page">Page to read.
        /// </param>
        public void Begin(Page page)
        {
            Begin(page, null);
        }
	    /// <summary> Start reading the page. 
	    /// 
	    /// </summary>
	    /// <param name="page">Page to read.
	    /// </param>
	    /// <param name="clip_ptr">A pointer to the optional clipping rectangle. This 
        /// parameter can be used to selectively read text from a given rectangle.
        /// </param>
        public void Begin(Page page, Rect clip_ptr)
        {
            Begin(page, clip_ptr, 0);
        }
	    /// <summary> Start reading the page. 
	    /// 
	    /// </summary>
	    /// <param name="page">Page to read.
	    /// </param>
	    /// <param name="clip_ptr">A pointer to the optional clipping rectangle. This 
	    /// parameter can be used to selectively read text from a given rectangle.
	    /// </param>
	    /// <param name="flags">A list of ProcessingFlags used to control text extraction 
        /// algorithm.
        /// </param>
        public void Begin(Page page, Rect clip, ProcessingFlags flags)
        {
            IntPtr rect = IntPtr.Zero;
			bool needToFreeIntPtr = false;
			try
			{
				if (clip != null)
				{
					rect = Marshal.AllocHGlobal(Marshal.SizeOf(clip.mp_imp));
					needToFreeIntPtr = true;
					Marshal.StructureToPtr(clip.mp_imp, rect, true);
				}
				PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorBegin(mp_extractor, page.mp_page, rect, (int)flags));
			}
			finally
			{
				if (needToFreeIntPtr)
				{
					Marshal.FreeHGlobal(rect);
				}
			}
            this.m_ref = page.m_ref;
        }

	    /// <summary> Gets the word count.
	    /// 
	    /// </summary>
        /// <returns> the number of words on the page.
        /// </returns>
        public Int32 GetWordCount()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorGetWordCount(mp_extractor, ref result));
            return result;
        }
	    /// <summary> Get all words in the current selection as a single string.
	    /// 
	    /// </summary>
	    /// <returns> The string containing all words in the current 
	    /// selection. Words will be separated with space (i.e. ' ') or 
        /// new line (i.e. '\n') characters.
        /// </returns>
        public String GetAsText()
        {
            return GetAsText(true);
        }
	    /// <summary> Get all words in the current selection as a single string.
	    /// 
	    /// </summary>
	    /// <param name="dehyphen">If true, finds and removes hyphens that split words
	    /// across two lines. Hyphens are often used a the end of lines as an
	    /// indicator that a word spans two lines. Hyphen detection enables removal
	    /// of hyphen character and merging of text runs to form a single word.
	    /// This option has no effect on Tagged PDF files.
	    /// </param>
	    /// <returns> The string containing all words in the current
	    /// selection. Words will be separated with space (i.e. ' ') or
        /// new line (i.e. '\n') characters.
        /// </returns>
        public String GetAsText(Boolean dehyphen)
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorGetAsText(mp_extractor, dehyphen, result.mp_impl));
            return result.ConvToManagedStr();
        }
	    /// <summary> Get all the characters that intersect an annotation.
	    ///
	    /// </summary>
	    /// <param name="annot">The annotation to intersect with.
	    /// </param>
	    /// <returns> The string under annot
	    /// </returns>
        public String GetTextUnderAnnot(Annot annot)
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorGetTextUnderAnnot(mp_extractor, annot.mp_annot, result.mp_impl));
            return result.ConvToManagedStr();
        }
        /// <summary> Get text content in a form of an XML string.
	    /// 
	    /// </summary>
	    /// <returns> The string containing XML output.		
	    /// </returns>
	    /// <remarks>
	    /// XML output will be encoded in UTF-8 and will have the following
	    /// structure:
	    /// <code>
	    /// &lt;Page num="1 crop_box="0, 0, 612, 792" media_box="0, 0, 612, 792" rotate="0"&gt;
	    /// &lt;Flow id="1"&gt;
	    /// &lt;Para id="1"&gt;
	    /// &lt;Line box="72, 708.075, 467.895, 10.02" style="font-family:Calibri; font-size:10.02; color: #000000;"&gt;
	    /// &lt;Word box="72, 708.075, 30.7614, 10.02"&gt;PDFNet&lt;/Word&gt;
	    /// &lt;Word box="106.188, 708.075, 15.9318, 10.02"&gt;SDK&lt;/Word&gt;
	    /// &lt;Word box="125.617, 708.075, 6.22242, 10.02"&gt;is&lt;/Word&gt;
	    /// ...
	    /// &lt;/Line&gt;
	    /// &lt;/Para&gt;  
	    /// &lt;/Flow&gt;
	    /// &lt;/Page&gt;
	    /// </code>
	    /// The above XML output was generated by passing the following union of 
	    /// flags in the call to GetAsXML(): 
	    /// <c>(TextExtractor.e_words_as_elements | TextExtractor.e_output_bbox | TextExtractor.e_output_style_info)</c>
	    /// 
	    /// In case 'xml_output_flags' was not specified, the default XML output 
	    /// would look as follows:
	    /// <code>
	    /// &lt;Page num="1 crop_box="0, 0, 612, 792" media_box="0, 0, 612, 792" rotate="0"&gt;
	    /// &lt;Flow id="1"&gt;
	    /// &lt;Para id="1"&gt;
	    /// &lt;Line&gt;PDFNet SDK is an amazingly comprehensive, high-quality PDF developer toolkit...&lt;/Line&gt;
	    /// &lt;Line&gt;levels. Using the PDFNet PDF library, ...&lt;/Line&gt;
	    /// ...
	    /// &lt;/Para&gt;
	    /// &lt;/Flow&gt;
	    /// &lt;/Page&gt;
	    /// &lt;/code&gt;
	    /// &lt;/example&gt;
        /// </code>
        /// </remarks>
        public String GetAsXML()
        {
            return GetAsXML(XMLOutputFlags.e_none);
        }
	    /// <summary> Get text content in a form of an XML string.
	    /// 
	    /// </summary>
	    /// <param name="flags">flags controlling XML output. For more
	    /// information, please see <c>TextExtract.XMLOutputFlags</c>.
	    /// </param>
	    /// <returns> The string containing XML output.
	    /// </returns>
	    /// <remarks>
	    /// XML output will be encoded in UTF-8 and will have the following
	    /// structure:
	    /// <c>
	    /// &lt;Page num="1 crop_box="0, 0, 612, 792" media_box="0, 0, 612, 792" rotate="0"&gt;
	    /// &lt;Flow id="1"&gt;
	    /// &lt;Para id="1"&gt;
	    /// &lt;Line box="72, 708.075, 467.895, 10.02" style="font-family:Calibri; font-size:10.02; color: #000000;"&gt;
	    /// &lt;Word box="72, 708.075, 30.7614, 10.02">PDFNet&lt;/Word&gt;
	    /// &lt;Word box="106.188, 708.075, 15.9318, 10.02"&lt;SDK&lt;/Word&gt;
	    /// &lt;Word box="125.617, 708.075, 6.22242, 10.02"&lt;is&lt;/Word&gt;
	    /// ...
	    /// &lt;/Line&gt;
	    /// &lt;/Para&gt;
	    /// &lt;/Flow&gt;
	    /// &lt;/Page&gt;
	    /// </c>
	    /// The above XML output was generated by passing the following union of
	    /// flags in the call to GetAsXML():
	    /// <c>(TextExtractor.e_words_as_elements | TextExtractor.e_output_bbox | TextExtractor.e_output_style_info)</c>
	    /// 
	    /// In case 'xml_output_flags' was not specified, the default XML output
	    /// would look as follows:
	    /// <code>
	    /// &lt;Page num="1 crop_box="0, 0, 612, 792" media_box="0, 0, 612, 792" rotate="0"&gt;
	    /// &lt;Flow id="1"&gt;
	    /// &lt;Para id="1"&gt;
	    /// &lt;Line&lt;PDFNet SDK is an amazingly comprehensive, high-quality PDF developer toolkit...&lt;/Line&gt;
	    /// &lt;Line&lt;levels. Using the PDFNet PDF library, ...&lt;/Line&gt;
	    /// ...
	    /// &lt;/Para&gt;
	    /// &lt;/Flow&gt;
	    /// &lt;/Page&gt;
        /// </code>
        /// </remarks>
        public String GetAsXML(XMLOutputFlags flags)
        {
            UString result = new UString();
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorGetAsXML(mp_extractor, (int)flags, result.mp_impl));
            return result.ConvToManagedStr();
        }

        /// <summary>Gets the number of line 
	    /// </summary>
        /// <returns>number of lines
        /// </returns>
        public Int32 GetNumLines()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorGetNumLines(mp_extractor, ref result));
            return result;
        }
	    /// <summary>Gets the first line of text on the selected page
	    /// </summary>
        /// <returns>The first line of text on the selected page.
        /// </returns>
        public Line GetFirstLine()
        {
            BasicTypes.TRN_TextExtractorLine result = new BasicTypes.TRN_TextExtractorLine();
	        PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorGetFirstLine(mp_extractor, ref result));
	        return new Line(result, this.m_ref);
        }
        /// <summary> Releases all resources used by the TextExtractor </summary>
        ~TextExtractor()
        {
            Dispose(false);
        }

        /// <summary> A class representing predominant text style associated with a 
        /// given Line, a Word, or a Glyph. The class includes information about 
        /// the font, font size, font styles, text color, etc.
        /// </summary>
        public class Style : IDisposable
        {
            internal BasicTypes.TRN_TextExtractorStyle mp_style;
            internal Object m_ref;
            internal Style(BasicTypes.TRN_TextExtractorStyle imp, Object reference)
            {
                this.mp_style = imp;
                this.m_ref = reference;
            }
            public Style()
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorStyleCreate(ref mp_style));
            }
		    /// <summary> Gets the font.
		    /// 
		    /// </summary>
		    /// <returns> low-level PDF font object. A high level font object can
		    /// be instantiated as follows:
            /// pdftron.PDF.Font f = new pdftron.PDF.Font(style.getFont());
            /// </returns>
            public SDF.Obj GetFont()
            {
                TRN_Obj result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorStyleGetFont(ref mp_style, ref result));
                return new SDF.Obj(result, this.m_ref);
            }
		    /// <summary> Gets the font name.
		    /// 
		    /// </summary>
            /// <returns> the font name used to draw the selected text.
            /// </returns>
            public String GetFontName()
            {
                UString result = new UString();
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorStyleGetFontName(ref mp_style, ref result.mp_impl));
                return result.ConvToManagedStr();
            }
		    /// <summary> Gets the font size.
		    /// 
		    /// </summary>
		    /// <returns> The font size used to draw the selected text as it
		    /// appears on the output page.
		    /// </returns>
		    /// <remarks>  Unlike the 'font size' in the graphics state (pdftron.PDF.GState) 
		    /// the returned font size accounts for the effects CTM, text matrix,
            /// and other graphics state attributes that can affect the appearance of
            /// text.</remarks>
            public Double GetFontSize()
            {
                double result = double.NaN;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorStyleGetFontSize(ref mp_style, ref result));
                return result;
            }
		    /// <summary> Gets the weight.
		    /// 
		    /// </summary>
		    /// <returns> The weight (thickness) component of the fully-qualified font name
		    /// or font specifier. The possible values are 100, 200, 300, 400, 500, 600, 700,
		    /// 800, or 900, where each number indicates a weight that is at least as dark as
		    /// its predecessor. A value of 400 indicates a normal weight; 700 indicates bold.
		    /// Note: The specific interpretation of these values varies from font to font.
            /// For example, 300 in one font may appear most similar to 500 in another.
            /// </returns>
            public Int32 GetWeight()
            {
                int result = int.MinValue;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorStyleGetWeight(ref mp_style, ref result));
                return result;
            }
		    /// <summary> Checks if is italic.
		    /// 
		    /// </summary>
		    /// <returns> true if glyphs have dominant vertical strokes that are slanted.
            /// </returns>
            /// <remarks>  the return value corresponds to the state of 'italic' flag in the 'Font Descriptor'. </remarks>
            public Boolean IsItalic()
            {
                bool result = false;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorStyleIsItalic(ref mp_style, ref result));
                return result;
            }
		    /// <summary> Checks if is serif.
		    /// 
		    /// </summary>
		    /// <returns> true if glyphs have serifs, which are short strokes drawn at an angle on the top
		    /// and bottom of glyph stems.
            /// </returns>
            /// <remarks>  the return value corresponds to the state of 'serif' flag in the 'Font Descriptor'. </remarks>
            public Boolean IsSerif()
            {
                bool result = false;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorStyleIsSerif(ref mp_style, ref result));
                return result;
            }
		    /// <summary> Gets the color.
		    /// 
		    /// </summary>
            /// <returns> text color in RGB color space.
            /// </returns>
            public int[] GetColor()
            {
                byte[] rgb = new byte[3];
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorStyleGetColor(ref mp_style, rgb));
                int[] result = Array.ConvertAll(rgb, item => System.Convert.ToInt32(item));
                return result;
            }
		    /// <summary>Checks whether this <c>Style</c> object is the same as the opject specified.</summary>
		    /// <param name="o">another object
		    /// </param>
		    /// <returns>true if equals specified object
		    /// </returns>
            public override bool Equals(object o)
            {
                if (o == null)
                    return false;
                if (!(o is Style))
                    return false;
                Style i = o as Style;
                bool result = false;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorStyleCompare(ref mp_style, ref i.mp_style, ref result));
                return result;
            }
		    /// <summary>Equality operator check whether two <c>Style</c> objects are the same.</summary>
		    /// <param name="l">object at the left of the operator
		    /// </param>
		    /// <param name="r">object at the right of the operator
		    /// </param>
		    /// <returns>true if both objects are equal, false otherwise
		    /// </returns>
            public static Boolean operator ==(Style l, Style r)
            {
                if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null)) return true;
                if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null)) return false;
                if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null)) return false;
                return l.Equals(r);
            }
		    /// <summary>Inequality operator check whether two <c>Style</c> objects are different.</summary>
		    /// <param name="l">object at the left of the operator
		    /// </param>
		    /// <param name="r">object at the right of the operator
		    /// </param>
            /// <returns>true if both objects are not equal, false otherwise
            /// </returns>
            public static Boolean operator !=(Style l, Style r)
            {
                return !(l == r);
            }

		    /// <summary>Sets value to the specified <c>Style</c>
		    /// </summary>
            /// <param name="r">specified <c>Style</c> object
            /// </param>
            public void Set(Style r)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorStyleCopy(ref r.mp_style, ref mp_style));
            }
		    /// <summary>Assignment operator</summary>
		    /// <param name="r">specified <c>Style</c> object
		    /// </param>
		    /// <returns>a <c>Style</c> object equals to the specified object
		    /// </returns>
            public Style op_Assign(Style r)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorStyleCopy(ref r.mp_style, ref mp_style));
                return this;
            }
		    /// <summary> Releases all resources used by the Style </summary>
            ~Style()
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
                if (mp_style.mp_imp != IntPtr.Zero)
                {
                    mp_style.mp_imp = IntPtr.Zero;
                }
            }
        }

        /// <summary> TextExtractor.Word object represents a word on a PDF page. 
        /// Each word contains a sequence of characters in one or more styles 
        /// (see TextExtractor.Style).
        /// </summary>
        public class Word : IDisposable
        {
            internal BasicTypes.TRN_TextExtractorWord mp_word;
            internal Object m_ref;
            internal Word(BasicTypes.TRN_TextExtractorWord imp, Object reference)
            {
                this.mp_word = imp;
                this.m_ref = reference;
            }
            public Word()
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordCreate(ref mp_word));
            }
		    /// <summary>Checks if valid word
		    /// </summary>
            /// <returns>true if this is a valid word, false otherwise.
            /// </returns>
            public bool IsValid()
            {
                bool result = false;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordIsValid(ref mp_word, ref result));
                return result;
            }

		    /// <summary> Gets the num glyphs.
		    /// 
		    /// </summary>
            /// <returns> The number of glyphs in this word.
            /// </returns>
            public Int32 GetNumGlyphs()
            {
                int result = int.MinValue;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordGetNumGlyphs(ref mp_word, ref result));
                return result;
            }
		    /// <summary> Gets the b box.
		    /// 
		    /// </summary>
		    /// <returns> The bounding box for this word (in unrotated page
		    /// coordinates).
		    /// </returns>
            /// <remarks>  To account for the effect of page '/Rotate' attribute,
            /// transform all points using page.GetDefaultMatrix().</remarks>
            public Rect GetBBox()
            {
                double[] bbox = new double[4];
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordGetBBox(ref mp_word, bbox));
                return new Rect(bbox[0], bbox[1], bbox[2], bbox[3]);
            }
		    //public Double GetQuad() ref[];
		    /// <summary> return The quadrilateral representing a tight bounding box
		    /// for this word (in unrotated page coordinates).
		    /// 
		    /// </summary>
            /// <returns> the quad
            /// </returns>
            public double[] GetQuad()
            {
                double[] quad = new double[8];
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordGetQuad(ref mp_word, quad));
                return quad;
            }
		    //public Double GetGlyphQuad(Int32 glyph_idx) ref[];

		    /// <summary>Gets the glyph from index
		    /// </summary>
		    /// <param name="glyph_idx">The index of a glyph in this word.
		    /// </param>
            /// <returns>The quadrilateral representing a tight bounding box for a given glyph in the word (in unrotated page coordinates).
            /// </returns>
            public double[] GetGlyphQuad(Int32 glyph_idx)
            {
                double[] quad = new double[8];
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordGetGlyphQuad(ref mp_word, glyph_idx, quad));
                return quad;
            }
		    /// <summary> Gets the char style.
		    /// 
		    /// </summary>
		    /// <param name="char_idx">The index of a character in this word.
		    /// </param>
            /// <returns> The style associated with a given character.
            /// </returns>
            public Style GetCharStyle(Int32 char_idx)
            {
                BasicTypes.TRN_TextExtractorStyle result = new BasicTypes.TRN_TextExtractorStyle();
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordGetCharStyle(ref mp_word, char_idx, ref result));
                return new Style(result, this.m_ref);
            }
		    /// <summary> Gets predominant style for this word.				
		    /// </summary>
            /// <returns> the style
            /// </returns>
            public Style GetStyle()
            {
                BasicTypes.TRN_TextExtractorStyle result = new BasicTypes.TRN_TextExtractorStyle();
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordGetStyle(ref mp_word, ref result));
                return new Style(result, this.m_ref);
            }
		
		    /// <summary> Gets the number of chars in the string.
		    /// </summary>
            /// <returns> the number of characters in this word.
            /// </returns>
            public Int32 GetStringLen()
            {
                int result = 0;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordGetStringLen(ref mp_word, ref result));
                return result;
            }

		    /// <summary>Gets Unicode string
		    /// </summary>
            /// <returns>the content of this word represented as a Unicode string.
            /// </returns>
            public String GetString()
            {
				IntPtr result = IntPtr.Zero;
				PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordGetString(ref mp_word, ref result));
				int length = GetStringLen();
				UString uString = new UString(result, length);
				String res = uString.ConvToManagedStr();
				return res;
            }


		    /// <summary>Gets the next object
		    /// </summary>
            /// <returns>the next object
            /// </returns>
            public Word GetNextWord()
            {
                BasicTypes.TRN_TextExtractorWord result = new BasicTypes.TRN_TextExtractorWord();
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordGetNextWord(ref mp_word, ref result));
                return new Word(result, this.m_ref);
            }
		    /// <summary>Gets the index of this word of the current line. A word that starts the line will return 0, whereas the last word in the line will return (line.GetNumWords()-1).
		    /// </summary>
            /// <returns>the index of this word of the current line
            /// </returns>
            public Int32 GetCurrentNum()
            {
                int result = int.MinValue;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordGetCurrentNum(ref mp_word, ref result));
                return result;
            }

		    // Implementation detail
		    /// <summary>Checks whether this <c>Word</c> object is the same as the opject specified.</summary>
		    /// <param name="o">specified object
		    /// </param>
            /// <returns>true if equals to the specified object
            /// </returns>
            public override bool Equals(Object o)
            {
                if (o == null)
                    return false;
                if (!(o is Word))
                    return false;
                Word i = o as Word;
                bool result = false;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorWordCompare(ref mp_word, ref i.mp_word, ref result));
                return result;
            }
		    /// <summary>Equality operator check whether two <c>Word</c> objects are the same.</summary>
		    /// <param name="l"><c>Word</c> object at the left of the operator
		    /// </param>
		    /// <param name="r"><c>Word</c> object at the right of the operator
		    /// </param>
            /// <returns>true if both <c>Word</c> objects are equal, false otherwise
            /// </returns>
            public static Boolean operator ==(Word l, Word r)
            {
                if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null)) return true;
                if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null)) return false;
                if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null)) return false;
                return l.Equals(r);
            }
		    /// <summary>Inequality operator check whether two <c>Word</c> objects are different.</summary>
		    /// <param name="l"><c>Word</c> object at the left of the operator
		    /// </param>
		    /// <param name="r"><c>Word</c> object at the right of the operator
		    /// </param>
            /// <returns>true if both <c>Word</c> object are not equal, false otherwise
            /// </returns>
            public static Boolean operator !=(Word l, Word r)
            {
                return !(l == r);
            }
		    /// <summary>Sets value to given <c>Word</c> object
		    /// </summary>
            /// <param name="r">a given <c>Word</c> object
            /// </param>
            public void Set(Word r)
            {
                this.mp_word = r.mp_word;
            }
		    /// <summary>Assignment operator</summary>
		    /// <param name="r">a given <c>Word</c> object
		    /// </param>
		    /// <returns><c>Word</c> object equals to the given <c>Word</c> object
		    /// </returns>
            public Word op_Assign(Word r)
            {
                this.mp_word = r.mp_word;
                return this;
            }
		    /// <summary> Releases all resources used by the Word </summary>
            ~Word()
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
                if (mp_word.mp_bld != IntPtr.Zero)
                {
                    mp_word.mp_bld = IntPtr.Zero;
                }
            }
        }

        /// <summary> TextExtractor.Line object represents a line of text on a PDF page. 
        /// Each line consists of a sequence of words, and each words in one or 
        /// more styles.
        /// </summary>
        public class Line : IDisposable
        {
            internal BasicTypes.TRN_TextExtractorLine mp_line;
            internal Object m_ref;
            internal Line(BasicTypes.TRN_TextExtractorLine imp, Object reference)
            {
                this.mp_line = imp;
                this.m_ref = reference;
            }
            public Line()
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineCreate(ref mp_line));
            }
		    /// <summary>Checks if line is valid
		    /// </summary>
            /// <returns>true if line is valid, false otherwise.
            /// </returns>
            public bool IsValid()
            {
                bool result = false;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineIsValid(ref mp_line, ref result));
                return result;
            }

		    /// <summary> Gets the num words.
		    /// 
		    /// </summary>
            /// <returns> The number of words in this line.
            /// </returns>
            public Int32 GetNumWords()
            {
                int result = int.MinValue;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineGetNumWords(ref mp_line, ref result));
                return result;
            }
		    /// <summary> Checks if is simple line.
		    /// 
		    /// </summary>
		    /// <returns> true is this line is not rotated (i.e. if the
            /// quadrilaterals returned by GetBBox() and GetQuad() coincide).
            /// </returns>
            public Boolean IsSimpleLine()
            {
                bool result = false;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineIsSimpleLine(ref mp_line, ref result));
                return result;
            }
		    /// <summary> Gets the b box.
		    /// 
		    /// </summary>
		    /// <returns> The bounding box for this line (in unrotated page
		    /// coordinates).
		    /// </returns>
            /// <remarks>  To account for the effect of page '/Rotate' attribute,
            /// transform all points using page.GetDefaultMatrix().</remarks>
            public Rect GetBBox()
            {
                IntPtr result = IntPtr.Zero;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineGetBBox(ref mp_line, ref result));
                double[] bbox = new double[4];
                System.Runtime.InteropServices.Marshal.Copy(result, bbox, 0, 4);
                return new Rect(bbox[0], bbox[1], bbox[2], bbox[3]);
            }
		    //public Double GetQuad() ref[];
		    /// <summary> Gets the quad.
		    /// 
		    /// </summary>
		    /// <returns> out_quad The quadrilateral representing a tight bounding box
            /// for this line (in unrotated page coordinates).
            /// </returns>
            public double[] GetQuad()
            {
                double[] quad = new double[8];
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineGetQuad(ref mp_line, quad));
                return quad;
            }
		    /// <summary> Gets the first word.
		    /// 
		    /// </summary>
		    /// <returns> the first word in the line.
            /// </returns>
            /// <remarks>  To traverse the list of all words on this line use word.GetNextWord(). </remarks>
            public Word GetFirstWord()
            {
                BasicTypes.TRN_TextExtractorWord result = new BasicTypes.TRN_TextExtractorWord();
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineGetFirstWord(ref mp_line, ref result));
                return new Word(result, this.m_ref);
            }
		    /// <summary> Gets the first word.
		    /// 
		    /// </summary>
		    /// <param name="word_idx">index of the word
		    /// </param>
            /// <returns> word with specified index
            /// </returns>				
            public Word GetWord(Int32 word_idx)
            {
                BasicTypes.TRN_TextExtractorWord result = new BasicTypes.TRN_TextExtractorWord();
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineGetWord(ref mp_line, word_idx, ref result));
                return new Word(result, this.m_ref);
            }
		    /// <summary> Gets the next line.
		    /// 
		    /// </summary>
            /// <returns> the next line on the page.
            /// </returns>
            public Line GetNextLine()
            {
                BasicTypes.TRN_TextExtractorLine result = new BasicTypes.TRN_TextExtractorLine();
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineGetNextLine(ref mp_line, ref result));
                return new Line(result, this.m_ref);
            }
		    /// <summary> Gets the current num.
		    /// 
		    /// </summary>
            /// <returns> the index of this line of the current page.
            /// </returns>
            public Int32 GetCurrentNum()
            {
                int result = int.MinValue;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineGetCurrentNum(ref mp_line, ref result));
                return result;
            }

		    /// <summary> Gets the style.
		    /// 
		    /// </summary>
            /// <returns> predominant style for this line.
            /// </returns>
            public Style GetStyle()
            {
                BasicTypes.TRN_TextExtractorStyle result = new BasicTypes.TRN_TextExtractorStyle();
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineGetStyle(ref mp_line, ref result));
                return new Style(result, this.m_ref);
            }
		    /// <summary> Gets the paragraph id.
		    /// 
		    /// </summary>
		    /// <returns> The unique identifier for a paragraph or column
		    /// that this line belongs to. This information can be used to
            /// identify which lines belong to which paragraphs.
            /// </returns>
            public Int32 GetParagraphID()
            {
                int result = int.MinValue;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineGetParagraphID(ref mp_line, ref result));
                return result;
            }
		    /// <summary> Gets the flow id.
		    /// 
		    /// </summary>
		    /// <returns> The unique identifier for a paragraph or column
		    /// that this line belongs to. This information can be used to
            /// identify which lines/paragraphs belong to which flows.
            /// </returns>
            public Int32 GetFlowID()
            {
                int result = int.MinValue;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineGetFlowID(ref mp_line, ref result));
                return result;
            }
		    /// <summary> Ends with hyphen.
		    /// 
		    /// </summary>
            /// <returns> true, if successful
            /// </returns>
            public Boolean EndsWithHyphen()
            {
                bool result = false;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineEndsWithHyphen(ref mp_line, ref result));
                return result;
            }

		    /// <summary>Determines if equals to the specified object
		    /// </summary>
		    /// <param name="o">specified object
		    /// </param>
            /// <returns>true if both objects are equal. false, otherwise
            /// </returns>
            public override bool Equals(object o)
            {
                if (o == null)
                    return false;
                if (!(o is Line))
                    return false;
                Line i = o as Line;
                bool result = false;
                PDFNetException.REX(PDFNetPINVOKE.TRN_TextExtractorLineCompare(ref mp_line, ref i.mp_line, ref result));
                return result;
            }
		    /// <summary>Equality operator checks whether two <c>Line</c> objects are the same.</summary>
		    /// <param name="l"><c>Line</c> object at the left of operator
		    /// </param>
		    /// <param name="r"><c>Line</c> object at the right of the operator
		    /// </param>
            /// <returns>true if both objects are equal
            /// </returns>
            public static Boolean operator ==(Line l, Line r)
            {
                if (System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null)) return true;
                if (System.Object.ReferenceEquals(l, null) && !System.Object.ReferenceEquals(r, null)) return false;
                if (!System.Object.ReferenceEquals(l, null) && System.Object.ReferenceEquals(r, null)) return false;
                return l.Equals(r);
            }
		    /// <summary>Inequality operator checks whether two <c>Line</c> objects are different.</summary>
		    /// <param name="l"><c>Line</c> object at the left of operator
		    /// </param>
		    /// <param name="r"><c>Line</c> object at the right of the operator
		    /// </param>
            /// <returns>true if both objects are equal
            /// </returns>
            public static Boolean operator !=(Line l, Line r)
            {
                return !(l == r);
            }
		    /// <summary>Sets value to the specified <c>Line</c> object
		    /// </summary>
		    /// <param name="r">another <c>Line</c> object
		    /// </param>
            public void Set(Line r)
            {
                this.mp_line = r.mp_line;
            }
		    /// <summary>Assignment operator</summary>
		    /// <param name="l">another <c>Line</c> object
		    /// </param>
		    /// <returns>a <c>Line</c> object 
		    /// </returns>
            public Line op_Assign(Line l)
            {
                this.mp_line = l.mp_line;
                return this;
            }
		    /// <summary> Releases all resources used by the Line </summary>
            ~Line()
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
                if (mp_line.mp_bld != IntPtr.Zero)
                {
                    mp_line.mp_bld = IntPtr.Zero;
                }
            }
        }

        ///<summary>Flags controlling the structure of XML output in a call to GetAsXML().</summary>
	    public enum XMLOutputFlags
        {
            //TODO: e_none missing description
		    ///<summary></summary>
		    e_none = 0,
		    ///<summary>Output words as XML elements instead of inline text.</summary>
		    e_words_as_elements = 1, 
		    ///<summary>Include bounding box information for each XML element. 
		    /// The bounding box information will be stored as 'bbox' attribute.</summary>
		    e_output_bbox = 2, 
		    ///<summary>Include font and styling information.</summary>
		    e_output_style_info = 4
        }
        /// <summary>Processing options that can be passed in Begin() method to direct the flow of content recognition algorithms. </summary>
        public enum ProcessingFlags
        {
            //TODO: e_none missing description
            ///<summary></summary>
            e_none = 0,
            ///<summary>Disables expanding of ligatures using a predefined mapping. 
            /// Default ligatures are: fi, ff, fl, ffi, ffl, ch, cl, ct, ll, 
            /// ss, fs, st, oe, OE. </summary>
            e_no_ligature_exp = 1,
            ///<summary>Disables removing duplicated text that is frequently used to 
            /// achieve visual effects of drop shadow and fake bold. </summary>
            e_no_dup_remove = 2,
            ///<summary>Treat punctuation (e.g. full stop, comma, semicolon, etc.) as 
            /// word break characters. </summary>
            e_punct_break = 4,
            ///<summary>Enables removal of text that is obscured by images or 
            /// rectangles. Since this option has small performance penalty 
            /// on performance of text extraction, by default it is not 
            /// enabled.</summary>
            e_remove_hidden_text = 8,
            ///<summary>Enables removing text that uses rendering mode 3 (i.e. invisible text).
            /// Invisible text is usually used in 'PDF Searchable Images' (i.e. scanned 
            /// pages with a corresponding OCR text). As a result, invisible text 
            /// will be extracted by default.</summary>
            e_no_invisible_text = 16
        }

    }
}