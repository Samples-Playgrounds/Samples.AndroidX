using System;
using System.Collections.Generic;
using System.Text;

using pdftronprivate.trn;
using pdftron.Common;
using pdftron.SDF;

using TRN_Font = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_Iterator = System.IntPtr;
using System.Runtime.InteropServices;

namespace pdftron.PDF
{
    /// <summary> A font that is used to draw text on a page. It corresponds to a Font Resource 
    /// in a PDF file. More than one page may reference the same Font object.
    /// A Font has a number of attributes, including an array of widths, the character 
    /// encoding, and the font’s resource name.
    /// 
    /// PDF document can contain several different types of fonts and Font class 
    /// represents a single, flat interface around all PDF font types.
    /// 
    /// There are two main classes of fonts in PDF: simple and composite fonts.
    /// 
    /// Simple fonts are Type1, TrueType, and Type3 fonts. All simple fonts have the 
    /// following properties:
    /// <list type="bullet">
    /// <item><description>
    /// Glyphs in the font are selected by single-byte character codes 
    /// obtained from a string that is shown by the text-showing operators. 
    /// Logically, these codes index into a table of 256 glyphs; the mapping 
    /// from codes to glyphs is called the font’s encoding. Each font program 
    /// has a built-in encoding. Under some circumstances, the encoding can 
    /// be altered by means described in Section 5.5.5 "Character Encoding" in 
    /// PDF Reference Manual.
    /// </description></item>
    /// <item><description>
    /// Each glyph has a single set of metrics. Therefore simple fonts support 
    /// only horizontal writing mode.
    /// </description></item>
    /// </list>
    /// A composite font is one whose glyphs are obtained from a font like object 
    /// called a CIDFont (e.g. CIDType0Font and CIDType0Font). A composite font is 
    /// represented by a font dictionary whose Subtype value is Type0. The Type 0 font 
    /// is known as the root font, while its associated CIDFont is called its descendant.
    /// CID-keyed fonts provide a convenient and efficient method for defining
    /// multiple-byte character encodings and fonts with a large number of glyphs. 
    /// These capabilities provide great flexibility for representing text in writing 
    /// systems for languages with large character sets, such as Chinese, Japanese, 
    /// and Korean (CJK).
    /// </summary>
    public class Font : IDisposable
    {
        internal TRN_Font mp_font = IntPtr.Zero;
        internal Object m_ref;
        internal Font(TRN_Font imp, Object reference)
        {
            this.mp_font = imp;
            this.m_ref = reference;
        }
        /// <summary> Releases all resources used by the Font </summary>
        ~Font()
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
            if (mp_font != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_FontDestroy(mp_font));
                mp_font = IntPtr.Zero;
            }
        }

        // Common Font methods ----------------------------------------------------------
	    /// <summary> Create a PDF.Font object 
	    /// 
	    /// </summary>
	    /// <param name="font_dict">font dictionary object
	    /// </param>			
	    /// <returns> newly created font
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public static Font Create(Obj font_dict)
        {
            TRN_Font result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontCreateFromObj(font_dict.mp_obj, ref result));
            return new Font(result, font_dict.GetRefHandleInternal());
        }

        /// <summary> Create a PDF.Font object for the given standard (also known as base 14 font).
	    /// 
	    /// </summary>
	    /// <param name="doc">PDF document
	    /// </param>
	    /// <param name="type">font type
	    /// </param>
	    /// <returns> newly created font
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    public static Font Create(PDFDoc doc, StandardType1Font type)
        {
            return Create(doc.GetSDFDoc(), type, false);
        }
	    /// <summary> Create a PDF.Font object for the given standard (also known as base 14 font).
	    /// 
	    /// </summary>
	    /// <param name="doc">PDF document
	    /// </param>
	    /// <param name="type">font type
	    /// </param>
	    /// <returns> newly created font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font Create(SDFDoc doc, StandardType1Font type)
        {
            return Create(doc, type, false);
        }
	    /// <summary> Create a PDF.Font object for the given standard (also known as base 14 font).
	    /// 
	    /// </summary>
	    /// <param name="doc">PDF document
	    /// </param>
	    /// <param name="type">font type
	    /// </param>
	    /// <param name="embed">if it is embeded font
	    /// </param>
	    /// <returns> the created font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font Create(PDFDoc doc, StandardType1Font type, Boolean embed)
        {
            return Create(doc.GetSDFDoc(), type);
        }
	    /// <summary> Create a PDF.Font object for the given standard (also known as base 14 font).
	    /// 
	    /// </summary>
	    /// <param name="doc">PDF document
	    /// </param>
	    /// <param name="type">font type
	    /// </param>
	    /// <param name="embed">if it is embeded font
	    /// </param>
	    /// <returns> the created font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font Create(SDFDoc doc, StandardType1Font type, Boolean embed)
        {
            TRN_Font result = IntPtr.Zero;
            if (embed)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_FontCreateAndEmbed(doc.mp_doc, type, ref result));
            }
            else
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_FontCreate(doc.mp_doc, type, ref result));
            }

            return result != IntPtr.Zero ? new Font(result, doc) : null;
        }

	    /// <summary> Create a new Unicode font based on the description of an existing PDF font.
	    /// 
	    /// </summary>
	    /// <param name="doc">PDF document
	    /// </param>
	    /// <param name="from">existing PDF font
	    /// </param>
	    /// <param name="char_set">the character set
	    /// </param>
	    /// <returns> the created font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font Create(PDFDoc doc, Font from, String char_set)
        {
            return Create(doc.GetSDFDoc(), from, char_set);
        }
	    /// <summary> Create a new Unicode font based on the description of an existing PDF font.
	    /// 
	    /// </summary>
	    /// <param name="doc">PDF document
	    /// </param>
	    /// <param name="from">existing PDF font
	    /// </param>
	    /// <param name="char_set">the character set
	    /// </param>
	    /// <returns> the created font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font Create(SDFDoc doc, Font from, String char_set)
        {
            TRN_Font result = IntPtr.Zero;
            UString str = new UString(char_set);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontCreateFromFontDescriptor(doc.mp_doc, from.mp_font, str.mp_impl, ref result));
            return new Font(result, doc);
        }

	    /// <summary> Create a new Unicode font based on the description of an existing PDF font.
	    /// 
	    /// </summary>
	    /// <param name="doc">PDF document
	    /// </param>
	    /// <param name="name">name of existing PDF font
	    /// </param>
	    /// <param name="char_set">the character set
	    /// </param>
	    /// <returns> the created font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font Create(PDFDoc doc, String name, String char_set)
        {
            return Create(doc.GetSDFDoc(), name, char_set);
        }
	    /// <summary> Create a new Unicode font based on the description of an existing PDF font.
	    /// 
	    /// </summary>
	    /// <param name="doc">PDF document
	    /// </param>
	    /// <param name="name">name of existing PDF font
	    /// </param>
	    /// <param name="char_set">the character set
	    /// </param>
	    /// <returns> the created font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font Create(SDFDoc doc, String name, String char_set)
        {
            TRN_Font result = IntPtr.Zero;
            UString str = new UString(char_set);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontCreateFromName(doc.mp_doc, name, str.mp_impl, ref result));
            return new Font(result, doc);
        }

	    /// <summary> Embed an external TrueType font in the document as a Simple font.
	    /// 
	    /// </summary>
	    /// <param name="doc">Document in which the external font should be embedded.
	    /// </param>
	    /// <param name="font_path">Path to the external font file.
	    /// </param>
	    /// <returns> the font
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  glyphs in the Simple font are selected by single-byte character codes.
	    /// If you want to work with multi-byte character codes (e.g. UTF16) you need to
        /// create a CID font.
        /// </remarks>
        public static Font CreateTrueTypeFont(PDFDoc doc, String font_path)
        {
            return CreateTrueTypeFont(doc.GetSDFDoc(), font_path);
        }
	    /// <summary> Embed an external TrueType font in the document as a Simple font.
	    /// 
	    /// </summary>
	    /// <param name="doc">Document in which the external font should be embedded.
	    /// </param>
	    /// <param name="font_path">Path to the external font file.
	    /// </param>
	    /// <returns> the font
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  glyphs in the Simple font are selected by single-byte character codes.
	    /// If you want to work with multi-byte character codes (e.g. UTF16) you need to
        /// create a CID font.
        /// </remarks>
        public static Font CreateTrueTypeFont(SDFDoc doc, String font_path)
        {
            return CreateTrueTypeFont(doc, font_path, true, true);
        }

	    /// <summary> Creates the true type font.
	    /// 
	    /// </summary>
	    /// <param name="doc">PDF Document
	    /// </param>
	    /// <param name="font_path">Path to the external font file.
	    /// </param>
	    /// <param name="embed">if font is embeded
	    /// </param>
	    /// <param name="subset">if font is subset
	    /// </param>
	    /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateTrueTypeFont(PDFDoc doc, String font_path, Boolean embed, Boolean subset)
        {
            return CreateTrueTypeFont(doc.GetSDFDoc(), font_path, embed, subset);
        }
	    /// <summary> Creates the true type font.
	    /// 
	    /// </summary>
	    /// <param name="doc">PDF Document
	    /// </param>
	    /// <param name="font_path">Path to the external font file.
	    /// </param>
	    /// <param name="embed">if font is embeded
	    /// </param>
	    /// <param name="subset">if font is subset
	    /// </param>
	    /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateTrueTypeFont(SDFDoc doc, String font_path, Boolean embed, Boolean subset)
        {
            TRN_Font result = IntPtr.Zero;
            UString str = new UString(font_path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontCreateTrueTypeFont(doc.mp_doc, str.mp_impl, embed, subset, ref result));
            return new Font(result, doc);
        }

	    /// <summary> Embed an external TrueType font in the document as a CID font.
	    /// By default the function selects "Identity-H" encoding that maps 2-byte
	    /// character codes ranging from 0 to 65,535 to the same Unicode value.
	    /// Other predefined encodings are listed in Table 5.15 'Predefined CMap names'
	    /// in PDF Reference Manual.
	    /// 
	    /// </summary>
	    /// <param name="doc">- document in which the external font should be embedded.
	    /// </param>
	    /// <param name="font_path">- path to the external font file.
	    /// </param>
	    /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateType1Font(PDFDoc doc, String font_path)
        {
            return CreateType1Font(doc.GetSDFDoc(), font_path);
        }
	    /// <summary> Embed an external TrueType font in the document as a CID font.
	    /// By default the function selects "Identity-H" encoding that maps 2-byte
	    /// character codes ranging from 0 to 65,535 to the same Unicode value.
	    /// Other predefined encodings are listed in Table 5.15 'Predefined CMap names'
	    /// in PDF Reference Manual.
	    /// 
	    /// </summary>
	    /// <param name="doc">- document in which the external font should be embedded.
	    /// </param>
	    /// <param name="font_path">- path to the external font file.
	    /// </param>
	    /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateType1Font(SDFDoc doc, String font_path)
        {
            return CreateType1Font(doc, font_path, true);
        }
	    /// <summary> Creates the type1 font.
	    /// 
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="font_path">the font_path
	    /// </param>
	    /// <param name="embed">the embed
	    /// </param>
	    /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateType1Font(PDFDoc doc, String font_path, Boolean embed)
        {
            return CreateType1Font(doc.GetSDFDoc(), font_path, embed);
        }
	    /// <summary> Creates the type1 font.
	    /// 
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="font_path">the font_path
	    /// </param>
	    /// <param name="embed">the embed
	    /// </param>
	    /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateType1Font(SDFDoc doc, String font_path, Boolean embed)
        {
            TRN_Font result = IntPtr.Zero;
            UString str = new UString(font_path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontCreateType1Font(doc.mp_doc, str.mp_impl, embed, ref result));
            return new Font(result, doc);
        }

	    /// <summary> Embed an external TrueType font in the document as a CID font.
	    /// By default the function selects "Identity-H" encoding that maps 2-byte
	    /// character codes ranging from 0 to 65,535 to the same Unicode value.
	    /// Other predefined encodings are listed in Table 5.15 'Predefined CMap names'
	    /// in PDF Reference Manual.
	    /// 
	    /// </summary>
	    /// <param name="doc">- document in which the external font should be embedded.
	    /// </param>
	    /// <param name="font_path">- path to the external font file.
	    /// </param>
	    /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateCIDTrueTypeFont(PDFDoc doc, String font_path)
        {
            return CreateCIDTrueTypeFont(doc.GetSDFDoc(), font_path);
        }
	    /// <summary> Embed an external TrueType font in the document as a CID font.
	    /// By default the function selects "Identity-H" encoding that maps 2-byte
	    /// character codes ranging from 0 to 65,535 to the same Unicode value.
	    /// Other predefined encodings are listed in Table 5.15 'Predefined CMap names'
	    /// in PDF Reference Manual.
	    /// 
	    /// </summary>
	    /// <param name="doc">- document in which the external font should be embedded.
	    /// </param>
	    /// <param name="font_path">- path to the external font file.
	    /// </param>
	    /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateCIDTrueTypeFont(SDFDoc doc, String font_path)
        {
            return CreateCIDTrueTypeFont(doc, font_path, true, true);
        }
	    /// <summary> Creates the cid true type font.
	    /// 
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="font_path">the font_path
	    /// </param>
	    /// <param name="embed">the embed
	    /// </param>
	    /// <param name="subset">the subset
	    /// </param>
	    /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateCIDTrueTypeFont(PDFDoc doc, String font_path, Boolean embed, Boolean subset)
        {
            return CreateCIDTrueTypeFont(doc.GetSDFDoc(), font_path, embed, subset);
        }
	    /// <summary> Creates the cid true type font.
	    /// 
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="font_path">the font_path
	    /// </param>
	    /// <param name="embed">the embed
	    /// </param>
	    /// <param name="subset">the subset
	    /// </param>
	    /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateCIDTrueTypeFont(SDFDoc doc, String font_path, Boolean embed, Boolean subset)
        {
            return CreateCIDTrueTypeFont(doc, font_path, embed, subset, Encoding.e_IdentityH);
        }
	    /// <summary> Creates the cid true type font.
	    /// 
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="font_path">the font_path
	    /// </param>
	    /// <param name="embed">the embed
	    /// </param>
	    /// <param name="subset">the subset
	    /// </param>
	    /// <param name="encoding">the encoding
	    /// </param>
	    /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateCIDTrueTypeFont(PDFDoc doc, String font_path, Boolean embed, Boolean subset, Encoding encoding)
        {
            return CreateCIDTrueTypeFont(doc.GetSDFDoc(), font_path, embed, subset, encoding);
        }
	    /// <summary> Creates the cid true type font.
	    /// 
	    /// </summary>
	    /// <param name="doc">the doc
	    /// </param>
	    /// <param name="font_path">the font_path
	    /// </param>
	    /// <param name="embed">the embed
	    /// </param>
	    /// <param name="subset">the subset
	    /// </param>
	    /// <param name="encoding">the encoding
	    /// </param>
	    /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateCIDTrueTypeFont(SDFDoc doc, String font_path, Boolean embed, Boolean subset, Encoding encoding)
        {
            TRN_Font result = IntPtr.Zero;
            UString str = new UString(font_path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontCreateCIDTrueTypeFont(doc.mp_doc, str.mp_impl, embed, subset, encoding, 0, ref result));
            return new Font(result, doc);
        }
        /// <summary> Creates the cid true type font.
        /// 
        /// </summary>
        /// <param name="doc">the doc
        /// </param>
        /// <param name="font_path">the font_path
        /// </param>
        /// <param name="embed">the embed
        /// </param>
        /// <param name="subset">the subset
        /// </param>
        /// <param name="encoding">the encoding
        /// </param>
        /// <param name="ttc_font_index">for TTC fonts the index of the actual font to use
        /// </param>
        /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateCIDTrueTypeFont(PDFDoc doc, String font_path, Boolean embed, Boolean subset, Encoding encoding, uint ttc_font_index)
        {
            return CreateCIDTrueTypeFont(doc.GetSDFDoc(), font_path, embed, subset, encoding, ttc_font_index);
        }
        /// <summary> Creates the cid true type font.
        /// 
        /// </summary>
        /// <param name="doc">the doc
        /// </param>
        /// <param name="font_path">the font_path
        /// </param>
        /// <param name="embed">the embed
        /// </param>
        /// <param name="subset">the subset
        /// </param>
        /// <param name="encoding">the encoding
        /// </param>
        /// <param name="ttc_font_index">for TTC fonts the index of the actual font to use
        /// </param>
        /// <returns> the font
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Font CreateCIDTrueTypeFont(SDFDoc doc, String font_path, Boolean embed, Boolean subset, Encoding encoding, uint ttc_font_index)
        {
            TRN_Font result = IntPtr.Zero;
            UString str = new UString(font_path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontCreateCIDTrueTypeFont(doc.mp_doc, str.mp_impl, embed, subset, encoding, ttc_font_index, ref result));
            return new Font(result, doc);
        }

        /// <summary> Instantiates a new font.
	    /// 
	    /// </summary>
        /// <param name="fnt_dict">the font_dict
        /// </param>
        public Font(Obj fnt_dict)
        {
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontCreateFromObj(fnt_dict.mp_obj, ref mp_font));
        }

	    /// <summary> Gets the type.
	    /// 
	    /// </summary>
	    /// <returns> Font Type
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Type GetType()
        {
            Type result = Type.e_Type0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetType(mp_font, ref result));
            return result;
        }
	    /// <summary> Checks if is simple.
	    /// </summary>
	    /// <returns> true for non-CID based fonts such as Type1, TrueType, and Type3</returns>
	    /// <remarks>
	    /// All simple fonts have the following properties:
	    /// <list type="bullet">
	    /// <item><description>
	    /// Glyphs in the font are selected by single-byte character codes
	    /// obtained from a string that is shown by the text-showing operators.
	    /// Logically, these codes index into a table of 256 glyphs; the mapping
	    /// from codes to glyphs is called the font’s encoding. Each font program
	    /// has a built-in encoding. Under some circumstances, the encoding can
	    /// be altered by means described in Section 5.5.5 "Character Encoding" in
	    /// PDF Reference Manual.
	    /// </description></item>
	    /// <item><description>
	    /// Each glyph has a single set of metrics. Therefore simple fonts support
	    /// only horizontal writing mode.
	    /// </description></item>
	    /// </list>
        /// </remarks>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsSimple()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontIsSimple(mp_font, ref result));
            return result;
        }
	    /// <summary> Gets the type.
	    /// 
	    /// </summary>
	    /// <param name="font_dict">the font_dict
	    /// </param>
	    /// <returns> The type of a given SDF/Cos font dictionary
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public static Type GetType(Obj font_dict)
        {
            Type result = Type.e_Type0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetTypeFromObj(font_dict.mp_obj, ref result));
            return result;
        }
	    /// <summary> Gets the SDFObj.
	    /// 
	    /// </summary>
        /// <returns> a SDF/Cos object of this Font.
        /// </returns>
        public Obj GetSDFObj()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetSDFObj(mp_font, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
	    /// <summary> Gets the descriptor.
	    /// 
	    /// </summary>
	    /// <returns> a SDF/Cos object representing FontDescriptor or NULL is FontDescriptor
	    /// is not present.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Obj GetDescriptor()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetDescriptor(mp_font, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
	    /// <summary> Gets the name.
	    /// 
	    /// </summary>
	    /// <returns> the name of a font. The behavior depends on the font type;
	    /// for a Type 3 font it gets the value of the Name key in a PDF Font resource.
	    /// For other types it gets the value of the BaseFont key in a PDF font resource.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public String GetName()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetName(mp_font, ref result));
            string res = Marshal.PtrToStringUTF8(result);
            return res;
        }
	    /// <summary> Gets the family name.
	    /// 
	    /// </summary>
	    /// <returns> the face's family name. This is an ASCII string, usually in English,
	    /// which describes the typeface's family (like 'Times New Roman', 'Bodoni', 'Garamond',
	    /// etc). This is a least common denominator used to list fonts.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public String GetFamilyName()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetFamilyName(mp_font, ref result));
            return Marshal.PtrToStringUTF8(result);
        }
	    /// <summary> Checks if is fixed width.
	    /// 
	    /// </summary>
	    /// <returns> true if all glyphs have the same width
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsFixedWidth()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontIsFixedWidth(mp_font, ref result));
            return result;
        }
	    /// <summary> Checks if is serif.
	    /// 
	    /// </summary>
	    /// <returns> true if glyphs have serifs
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsSerif()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontIsSerif(mp_font, ref result));
            return result;
        }
	    /// <summary> Checks if is symbolic.
	    /// 
	    /// </summary>
	    /// <returns> true if font contains characters outside the Adobe standard Latin character set.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsSymbolic()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontIsSymbolic(mp_font, ref result));
            return result;
        }
	    /// <summary> Checks if is italic.
	    /// 
	    /// </summary>
	    /// <returns> true if glyphs have dominant vertical strokes that are slanted.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsItalic()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontIsItalic(mp_font, ref result));
            return result;
        }
	    /// <summary> Checks if is all cap.
	    /// 
	    /// </summary>
	    /// <returns> true if font contains no lowercase letters
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsAllCap()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontIsAllCap(mp_font, ref result));
            return result;
        }
	    /// <summary> Checks if is force bold.
	    /// 
	    /// </summary>
	    /// <returns> true if bold glyphs should be painted with extra pixels at very small text sizes.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsForceBold()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontIsForceBold(mp_font, ref result));
            return result;
        }
	    /// <summary> Checks if is horizontal mode.
	    /// 
	    /// </summary>
	    /// <returns> true if the font uses horizontal writing mode, false for vertical writing mode.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsHorizontalMode()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontIsHorizontalMode(mp_font, ref result));
            return result;
        }
	    /// <summary> Gets the width.
	    /// 
	    /// </summary>
	    /// <param name="char_code">the char_code
	    /// </param>
	    /// <returns> advance width, measured in character space units for the glyph
	    /// matching given character code.
	    /// 
	    /// The function gets the advance width of the font glyph. The advance width
	    /// is the amount by which the current point advances when the glyph is drawn.
	    /// The advance width may not correspond to the visible width of the glyph
	    /// and for this reason, the advance width cannot be used to determine the glyphs’
	    /// bounding boxes.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Double GetWidth(Int32 char_code)
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetWidth(mp_font,System.Convert.ToUInt32(char_code), ref result));
            return result;
        }
	    /// <summary> Gets the max width.
	    /// 
	    /// </summary>
	    /// <returns> the maximal advance width, in font units, for all glyphs in this face.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Double GetMaxWidth()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetMaxWidth(mp_font, ref result));
            return result;
        }
	    /// <summary> Gets the missing width.
	    /// 
	    /// </summary>
	    /// <returns> the default width to use for character codes whose widths are
	    /// not specified in a font dictionary’s Widths array.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Double GetMissingWidth()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetMissingWidth(mp_font, ref result));
            return result;
        }
	    /// <summary> Gets the vertical advance.
	    /// 
	    /// </summary>
	    /// <param name="char_code">character to query for vertical advance
	    /// </param>
	    /// <param name="pos_x">x coordinate
	    /// </param>
	    /// <param name="pos_y">y coordinate
	    /// </param>
	    /// <returns> an double array containing in the following order
	    /// 
	    /// vertical advance. vertical advance is a displacement vector for vertical
	    /// writing mode (i.e. writing mode 1); its horizontal component is always 0.
	    /// 
	    /// horizontal component of the position vector defining the position
	    /// of the vertical writing mode origin relative to horizontal writing mode origin.
	    /// 
	    /// vertical component of the position vector defining the position
	    /// of the vertical writing mode origin relative to horizontal writing mode origin.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>Use this method only for composite fonts with vertical writing mode
	    /// (i.e. if Font.IsHorizontalMode() returns false). The method will return 0 as vertical
        /// advance for simple fonts or for composite fonts with only horizontal writing mode.		
        /// Relevant only for a Type0 font. </remarks>
        public Double GetVerticalAdvance(Int32 char_code, Double pos_x, Double pos_y)
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetVerticalAdvance(mp_font,System.Convert.ToUInt32(char_code), ref pos_x, ref pos_y, ref result));
            return result;
        }

	    /// <summary> CharCodeGetIterator represents an iterator interface used to traverse
	    /// a list of char codes for which there is a glyph outline in the embedded font.
	    /// 
	    /// </summary>
	    /// <returns> the char code iterator
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>this function is in the process of being tested and shouldn't be used 
        /// in production applications.</remarks>  
        public FontCharCodeIterator GetCharCodeIterator()
        {
            TRN_Iterator result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetCharCodeIterator(mp_font, ref result));
            return new FontCharCodeIterator(result);
        }
	    /// <summary> The function retrieves the glyph outline for a given character code.
	    /// 
	    /// </summary>
	    /// <param name="char_code">character to query
	    /// </param>
	    /// <param name="conics2cubics">if set to true converts all quadratic Bezier curves to cubic
	    /// Beziers, otherwise no coversion is performed.
	    /// </param>
	    /// <returns> A PathData object containing the path information.
	    /// </returns>
	    /// <remarks>  the function can return only the following operators (Element.e_moveto,
	    /// Element.e_lineto, Element.e_cubicto and optionaly Element.e_conicto if
	    /// conics2cubics parameter is set to true.
	    /// 
	    /// This function is not applicable to Type3 font and will throw an exception.
	    /// Use <c>GetType3GlyphStream</c> instead.
	    ///
        /// Check PathData.IsDefined to see if the char_code was mapped to 'undefined character code'.
        /// </remarks>
        public PathData GetGlyphPath(Int32 char_code, Boolean conics2cubics)
        {
            byte[] oprs = null;
            double[] data = null;
            bool result = GetGlyphPath(char_code, ref oprs, ref data, conics2cubics);
            return new PathData(result, oprs, data);
        }

	    /// <summary> The function retrieves the glyph outline for a given character code.
	    /// 
	    /// </summary>
	    /// <param name="char_code">character to query
	    /// </param>
	    /// <param name="out_oprs">a vector of operators. The array is filled in by the method.
	    /// </param>
	    /// <param name="out_data">a vector of data points that represent arguments for operators. The array is filled in by the method.
	    /// </param>
	    /// <param name="conics2cubics">if set to true converts all quadratic Bezier curves to cubic
	    /// Beziers, otherwise no coversion is performed.
	    /// </param>
	    /// <returns> false if the char_code was mapped to 'undefined character code'.
	    /// In some fonts 'undefined character code' corresponds to a space, in some fonts it
	    /// is a box, and in others it may be a more complicated glyph.
	    /// </returns>
	    /// <remarks>  the function can return only the following operators (Element.e_moveto,
	    /// Element.e_lineto, Element.e_cubicto and optionaly Element.e_conicto if
	    /// conics2cubics parameter is set to true.
	    /// 
	    /// This function is not applicable to Type3 font and will throw an exception.
        /// Use <c>GetType3GlyphStream</c> instead.
        /// </remarks>
        public Boolean GetGlyphPath(Int32 char_code, ref byte[] out_oprs, ref double[] out_data, Boolean conics2cubics)
        {
			bool result0 = false;
			int oprs_sz = 0;
			int data_sz = 0;
			int glyph_idx = 0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetGlyphPath(mp_font,System.Convert.ToUInt32(char_code), IntPtr.Zero, ref oprs_sz, IntPtr.Zero, ref data_sz, ref glyph_idx, conics2cubics, IntPtr.Zero, ref result0));
			if (result0 && oprs_sz > 0 && data_sz > 0)
			{
				out_oprs = new byte[oprs_sz];
				out_data = new double[data_sz];
                int marshal_oprs_size = Marshal.SizeOf(out_oprs[0]) * out_oprs.Length;
                int marshal_data_size = Marshal.SizeOf(out_data[0]) * out_data.Length;
				IntPtr oprs_pnt = Marshal.AllocHGlobal(marshal_oprs_size);
				IntPtr data_pnt = Marshal.AllocHGlobal(marshal_data_size);
				try
				{
					bool result = false;
                    PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetGlyphPath(mp_font,System.Convert.ToUInt32(char_code), oprs_pnt, ref oprs_sz, data_pnt, ref data_sz, ref glyph_idx, conics2cubics, IntPtr.Zero, ref result));
                                        
					System.Runtime.InteropServices.Marshal.Copy(oprs_pnt, out_oprs, 0, oprs_sz);
					System.Runtime.InteropServices.Marshal.Copy(data_pnt, out_data, 0, data_sz);

					return result;
				}
				finally
				{
					Marshal.FreeHGlobal(oprs_pnt);
					Marshal.FreeHGlobal(data_pnt);
				}
			}
			out_oprs = new byte[0];
			out_data = new double[0];
			return result0;
        }

	    /// <summary> Maps the encoding specific 'charcode' to Unicode. Conversion of 'charcode'
	    /// to Unicode can result in upto four Unicode characters.
	    /// 
	    /// </summary>
	    /// <param name="char_code">encoding specific 'charcode' that needs to be converted
	    /// to Unicode.
	    /// </param>
	    /// <param name="out_unicode">A pointer to an array of Unicode characters where the conversion result will be stored.
	    /// </param>
	    /// <param name="in_uni_sz">The number of characters that can be written to out_uni_arr. You can assume that the function will never map to more than 10 characters.
	    /// </param>
	    /// <param name="out_chars">The function will modify this value to return the number of Unicode characters written in 'out_uni_arr' array.
	    /// </param>
	    /// <returns> true if char_code was mapped to Unicode public area or false if
	    /// the char_code was mapped to Unicode private area.
	    /// 
	    /// A char_code is mapped to Unicode private area when the information required
	    /// for proper mapping is missing in PDF document (e.g. a predefined encoding
	    /// or ToUnicode CMap).
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  This function is not applicable to CIDFonts (e_CIDType0 and e_CIDType2)
        /// and will throw an exception if called.</remarks>
        public Boolean MapToUnicode(int char_code, ref int[] out_unicode, int in_uni_sz, ref int out_chars)
        {
            bool result = false;
            int maxuni_sz = 25;
            UInt16[] unicode = new UInt16[maxuni_sz];

            int out_chars_num = 0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontMapToUnicode(mp_font,System.Convert.ToUInt32(char_code), unicode, ((in_uni_sz < maxuni_sz) ? in_uni_sz : maxuni_sz), ref out_chars_num, ref result));
            out_chars = out_chars_num;

            if(out_unicode == null || out_unicode.Length < out_chars)
                out_unicode = new int[out_chars];
            for (int i = 0; i < out_chars; ++i)
            {
                out_unicode[i] = unicode[i];
            }
            return result;
        }

	    /// <summary> Gets the encoding.
	    /// 
	    /// </summary>
	    /// <returns> the font’s encoding array (the mapping of character codes to glyphs).
	    /// The array contains 256 Strings. If a String is not NULL, it containing the name
	    /// of the glyph for the code point corresponding to the index.
	    /// If it is NULL, then the name of the glyph is unchanged from
	    /// that specified by the font’s builtin encoding.
	    /// 
	    /// For a Type 3 font, all glyph names will be present in the encoding array,
	    /// and NULL entries correspond to un-encoded code points.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>The Font object is the owner of the array.
	    /// This function is not applicable to composite fonts (e_type0, e_CIDType0,
	    /// and e_CIDType2) and will throw an exception.</remarks>
        public IntPtr GetEncoding()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetEncoding(mp_font, ref result));
            return result;
        }
	    /// <summary> Tests whether or not the specified font is stored as a font file in a stream
	    /// embedded in the PDF file.
	    /// 
	    /// </summary>
	    /// <returns> true if the font is embedded in the file, false otherwise.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Boolean IsEmbedded()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontIsEmbedded(mp_font, ref result));
            return result;
        }
	    /// <summary> Gets the embedded font name.
	    /// 
	    /// </summary>
	    /// <returns> the PostScript font name for the embedded font. If the embedded font
	    /// name is not available the function returns the empty string .
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public String GetEmbeddedFontName()
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetEmbeddedFontName(mp_font, ref result));
            return Marshal.PtrToStringUTF8(result);
        }
	    /// <summary> Gets the embedded font.
	    /// 
	    /// </summary>
	    /// <returns> the stream object of the embedded font or NULL if there if the
	    /// font is not embedded.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  This function is not applicable to Type3 font and will throw an exception. </remarks>
        public Obj GetEmbeddedFont()
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetEmbeddedFont(mp_font, ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }
	    /// <summary> Gets the embedded font buf size.
	    /// 
	    /// </summary>
	    /// <returns> the size of decoded buffer containing embedded font data or 0
	    /// if this information is not known in advance.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  The size of decoded buffer may not be known in advance for all
	    /// fonts and may not be correct.
        /// This function is not applicable to Type3 font and will throw an exception. 
        /// </remarks>
        public Int32 GetEmbeddedFontBufSize()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetEmbeddedFontBufSize(mp_font, ref result));
            return result;
        }
	    /// <summary> Gets the units per em.
	    /// 
	    /// </summary>
	    /// <returns> the number of font units per EM square for this face. This is
	    /// typically 2048 for TrueType fonts, 1000 for Type1 fonts
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>  Only relevant for scalable formats (such as TrueType and Type1).
        /// This function is not applicable to Type3 font and will throw an exception.
        /// Use GetType3FontMatrix instead.</remarks>
        public Int16 GetUnitsPerEm()
        {
            UInt16 result = UInt16.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetUnitsPerEm(mp_font, ref result));
            return System.Convert.ToInt16(result);
        }
	    /// <summary> The face's ascender is the vertical distance from the baseline to the topmost
	    /// point of any glyph in the face. This field's value is a positive number, expressed
	    /// in the glyph coordinate system. For all font types except Type 3, the units of
	    /// glyph space are one-thousandth of a unit of text space. Some font designs use
	    /// a value different from 'bbox.yMax'.
	    /// 
	    /// </summary>
	    /// <returns> the ascent
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Only relevant for scalable formats. </remarks>
        public Double GetAscent()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetAscent(mp_font, ref result));
            return result;
        }
	    /// <summary> The face's descender is the vertical distance from the baseline to the bottommost
	    /// point of any glyph in the face. This field's value is a negative number expressed
	    /// in the glyph coordinate system. For all font types except Type 3, the units of
	    /// glyph space are one-thousandth of a unit of text space. Some font designs use
	    /// a value different from 'bbox.yMin'.
	    /// 
	    /// </summary>
	    /// <returns> the descent
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Only relevant for scalable formats. </remarks>
        public Double GetDescent()
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetDescent(mp_font, ref result));
            return result;
        }

	    /// <summary> Gets the b box.
	    /// 
	    /// </summary>
	    /// <returns> A rectangle expressed in the glyph coordinate system, specifying the
	    /// font bounding box. This is the smallest rectangle enclosing the shape that would
	    /// result if all of the glyphs of the font were placed with their origins coincident
	    /// and then filled.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Rect GetBBox()
        {
            BasicTypes.TRN_Rect result = new BasicTypes.TRN_Rect();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetBBox(mp_font, ref result));
            return new Rect(result);
        }
	    /// <summary> Gets the standard type1 font type.
	    /// 
	    /// </summary>
	    /// <returns> Font.e_null if the font is not a standard Type1 font or some
	    /// other StandardType1Font value for a standard Type1 font.
        /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public Int32 GetStandardType1FontType()
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetStandardType1FontType(mp_font, ref result));
            return result;
        }

	    /// <summary> Checks if is cFF.
	    /// 
	    /// </summary>
	    /// <returns> true if the embedded font is represented as CFF (Compact Font Format).
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Only Type1 and Type1C fonts can be represented in CFF format</remarks>
        public Boolean IsCFF()
        {
            bool result = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontIsCFF(mp_font, ref result));
            return result;
        }

	    // Type3 specific methods -------------------------------------------------------
	    /// <summary> Gets the type3 font matrix.
	    /// 
	    /// </summary>
	    /// <returns> Type3 font matrix, mapping glyph space to text space
	    /// A common practice is to define glyphs in terms of a 1000-unit
	    /// glyph coordinate system, in which case the font matrix is [0.001 0 0 0.001 0 0].
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Relevant only for a Type3 font. </remarks>
        public Matrix2D GetType3FontMatrix()
        {
            BasicTypes.TRN_Matrix2D result = new BasicTypes.TRN_Matrix2D();
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetType3FontMatrix(mp_font, ref result));
            return new Matrix2D(result);
        }
	    /// <summary> Gets the type3 glyph stream.
	    /// 
	    /// </summary>
	    /// <param name="char_code">the char_code
	    /// </param>
	    /// <returns> a SDF/Cos glyph stream for the given char_code.
	    /// If specified char_code is not found in CharPorcs dictionary the
	    /// function returns NULL.
        /// </returns>
        /// <remarks>  Relevant only for a Type3 font. </remarks>
        public Obj GetType3GlyphStream(Int32 char_code)
        {
            TRN_Obj result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetType3GlyphStream(mp_font,System.Convert.ToUInt32(char_code), ref result));
            return ((result == IntPtr.Zero) ? null : new Obj(result, this.m_ref));
        }

	    // Type0 specific methods -------------------------------------------------------
	    /// <summary> Gets the descendant.
	    /// 
	    /// </summary>
	    /// <returns> descendant CIDFont.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Relevant only for a Type0 font. </remarks>
        public Font GetDescendant()
        {
            TRN_Font result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetDescendant(mp_font,ref result));
            return new Font(result, this.m_ref);
        }
	    /// <summary> Map to CID.
	    /// 
	    /// </summary>
	    /// <param name="char_code">the char_code
	    /// </param>
	    /// <returns> a CID matching specified charcode.
	    /// </returns>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        /// <remarks>  Relevant only for a Type0 font. </remarks>
        public Int32 MapToCID(Int32 char_code)
        {
            uint result = uint.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontMapToCID(mp_font,System.Convert.ToUInt32(char_code), ref result));
            return System.Convert.ToInt32(result);
        }
	    /// <summary> Gets the vertical advance.
	    /// 
	    /// </summary>
	    /// <param name="char_code">character to query for vertical advance
	    /// </param>
	    /// <param name="out_pos_vect_x"> initialized by the method. horizontal component of the position vector defining the position of the vertical writing mode origin relative to horizontal writing mode origin.
	    /// </param>
	    /// <param name="out_pos_vect_y"> initialized by the method. vertical component of the position vector defining the position of the vertical writing mode origin relative to horizontal writing mode origin.
	    /// </param>
	    /// <returns> an double array containing in the following order
	    /// 
	    /// vertical advance. vertical advance is a displacement vector for vertical
	    /// writing mode (i.e. writing mode 1); its horizontal component is always 0.
	    /// 
	    /// horizontal component of the position vector defining the position
	    /// of the vertical writing mode origin relative to horizontal writing mode origin.
	    /// 
	    /// vertical component of the position vector defining the position
	    /// of the vertical writing mode origin relative to horizontal writing mode origin.
	    /// </returns>
	    /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
	    /// <remarks>Use this method only for composite fonts with vertical writing mode
	    /// (i.e. if Font.IsHorizontalMode() returns false). The method will return 0 as vertical
        /// advance for simple fonts or for composite fonts with only horizontal writing mode.		
        /// Relevant only for a Type0 font. </remarks>
        public double GetVerticalAdvance(int char_code, ref double out_pos_vect_x, ref double out_pos_vect_y)
        {
            double result = double.NaN;
            PDFNetException.REX(PDFNetPINVOKE.TRN_FontGetVerticalAdvance(mp_font,System.Convert.ToUInt32(char_code), ref out_pos_vect_x, ref out_pos_vect_y, ref result));
            return result;
        }

        // Nested Types
        //TODO: enum documentation missing
        public enum Encoding
        {
            e_IdentityH,
            e_Indices
        }

        //TODO: enum documentation missing
        public enum StandardType1Font
        {
            e_times_roman,
            e_times_bold,
            e_times_italic,
            e_times_bold_italic,
            e_helvetica,
            e_helvetica_bold,
            e_helvetica_oblique,
            e_helvetica_bold_oblique,
            e_courier,
            e_courier_bold,
            e_courier_oblique,
            e_courier_bold_oblique,
            e_symbol,
            e_zapf_dingbats,
            e_null
        }

        /// <summary>Font types</summary>
        public enum Type
        {
            /// <summary>Type 1 PostScript font	</summary>
		    e_Type1,
		    /// <summary>TrueType font</summary>
		    e_TrueType,
		    /// <summary>Type 1 multiple master PostScript font</summary>
		    e_MMType1,
		    /// <summary>Type 3 PostScript font</summary>
		    e_Type3,
		    /// <summary>Type 0 PostScript composite (CID) font</summary>
		    e_Type0,
		    /// <summary>Type 0 CID font</summary>
		    e_CIDType0,
		    /// <summary>Type 2 CID font</summary>
		    e_CIDType2,
        }

    }
}
