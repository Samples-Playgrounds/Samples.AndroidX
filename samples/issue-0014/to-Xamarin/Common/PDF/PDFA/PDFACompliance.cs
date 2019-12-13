using System;
using System.Runtime.InteropServices;

using pdftron.Common;
using pdftronprivate.trn;

using TRN_PDFACompliance = System.IntPtr;

namespace pdftron.PDF.PDFA
{
    /// <summary> PDFACompliance class is used to validate PDF documents for PDF/A (ISO 19005:1/2/3)
    /// compliance or to convert existing PDF files to PDF/A compliant documents.
    /// <para>
    /// The conversion option analyses the content of existing PDF files and performs 
    /// a sequence of modifications in order to produce a PDF/A compliant document. 
    /// Features that are not suitable for long-term archiving (such as encryption, 
    /// obsolete compression schemes, missing fonts, or device-dependent color) are 
    /// replaced with their PDF/A compliant equivalents. Because the conversion process 
    /// applies only necessary changes to the source file, the information loss is 
    /// minimal. Also, because the converter provides a detailed report for each change,
    /// it is simple to inspect changes and to determine whether the conversion loss 
    /// is acceptable. </para>
    /// <para>
    /// The validation option in PDF/A Manager can be used to quickly determine whether 
    /// a PDF file fully complies with the PDF/A specification according to the 
    /// international standard ISO 19005:1/2/3. For files that are not compliant, the 
    /// validation option can be used to produce a detailed report of compliance 
    /// violations as well as a list of relevant error objects.</para>
    /// 
    /// Key Functions:
    /// <list type="bullet">
    /// <item><description>Checks if a PDF file is compliant with PDF/A (ISO 19005-1) specification.</description></item>
    /// <item><description>Converts any PDF to a PDF/A compliant document.</description></item>
    /// <item><description>Supports PDF/A-1a, PDF/A-1b, PDF/A-2b.</description></item>
    /// <item><description>Produces a detailed report of compliance violations and associated PDF objects.</description></item>
    /// <item><description>Keeps the required changes a minimum, preserving the consistency of the original.</description></item>
    /// <item><description>Tracks all changes to allow for automatic assessment of data loss.</description></item>
    /// <item><description>Allows user to customize compliance checks or omit specific changes.</description></item>
    /// <item><description>Preserves tags, logical structure, and color information in existing PDF documents.</description></item>
    /// <item><description>Offers automatic font substitution, embedding, and subsetting options.</description></item>
    /// <item><description>Supports automation and batch operation. PDF/A Converter is designed to be used 
    /// in unattended mode in high throughput server or batch environments</description></item>
    /// </list>
    /// </summary>
    public class PDFACompliance : IDisposable
    {
        // Fields
        internal TRN_PDFACompliance mp_pdfac = IntPtr.Zero;

        ~PDFACompliance()
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
            if (mp_pdfac != IntPtr.Zero)
            {
                PDFNetException.REX(PDFNetPINVOKE.TRN_PDFAComplianceDestroy(mp_pdfac));
                mp_pdfac = IntPtr.Zero;
            }
        }

        public PDFACompliance(bool convert, string file_path, string password, Conformance conf, ErrorCode[] exceptions, int max_ref_objs, bool first_stop) 
        {
            UString filePath = new UString(file_path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFAComplianceCreateFromFile(convert, filePath.mp_impl, password, conf, exceptions, exceptions != null ? exceptions.Length : 0, max_ref_objs, first_stop, ref this.mp_pdfac));
        }

        public PDFACompliance(bool convert, byte[] buf, string password, Conformance conf, ErrorCode[] exceptions, int max_ref_objs, bool first_stop)
        {
            int size = Marshal.SizeOf(buf[0]) * buf.Length;
            IntPtr pnt = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(buf, 0, pnt, buf.Length);
                PDFNetException.REX(PDFNetPINVOKE.TRN_PDFAComplianceCreateFromBuffer(convert, pnt, new UIntPtr(System.Convert.ToUInt32(buf.Length)), password, conf, exceptions, exceptions != null ? exceptions.Length : 0, max_ref_objs, first_stop, ref this.mp_pdfac));
            }
            finally
            {
                // Free the unmanaged memory.
                Marshal.FreeHGlobal(pnt);
            }
        }

        /// <summary> Gets the error count.
		/// 
		/// </summary>
		/// <returns> The number of compliance violations.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public Int32 GetErrorCount() 
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFAComplianceGetErrorCount(mp_pdfac, ref result));
            return result;
        }
		/// <summary> Gets the error.
		/// 
		/// </summary>
		/// <param name="idx">The index in the array of error code identifiers.
		/// The array is indexed starting from zero.
		/// </param>
		/// <returns> The error identifier.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public ErrorCode GetError(Int32 idx) 
        {
            ErrorCode result = ErrorCode.e_PDFA0_1_0;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFAComplianceGetError(mp_pdfac, idx, ref result));
            return result;

        }
		/// <summary> Gets the error.
		/// 
		/// </summary>
		/// <param name="id">The index in the array of error code identifiers.
		/// The array is indexed starting from zero.
		/// </param>
		/// <returns> The error identifier.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public Int32 GetRefObjCount(ErrorCode id) 
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFAComplianceGetRefObjCount(mp_pdfac, id, ref result));
            return result;
        }
		/// <summary> Gets the ref obj.
		/// 
		/// </summary>
		/// <param name="id">error code identifier (obtained using GetError() method).
		/// </param>
		/// <param name="obj_idx">the obj_idx
		/// </param>
		/// <returns> A specific object reference associated with a given error type.
		/// The return value is a PDF object identifier (i.e. object number for
		/// 'pdftron.SDF.Obj)) for the that is associated with the error.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public Int32 GetRefObj(ErrorCode id, Int32 obj_idx)
        {
            int result = int.MinValue;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFAComplianceGetRefObj(mp_pdfac, id, obj_idx, ref result));
            return result;
        }
		/// <summary> Gets the pDFA error message.
		/// 
		/// </summary>
		/// <param name="id">error code identifier (obtained using GetError() method).
		/// </param>
		/// <returns> A descriptive error message for the given error identifier.
		/// </returns>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		public static string GetPDFAErrorMessage(ErrorCode id)
        {
            IntPtr result = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFAComplianceGetPDFAErrorMessage(id, ref result));
            string res = Marshal.PtrToStringUTF8(result);
            return res;
        }

		/// <summary> Serializes the converted PDF/A document to a file on disk.
		/// 
		/// </summary>
		/// <param name="file_path">the file_path
		/// </param>
		/// <param name="linearized">- An optional flag used to specify whether the the resulting
		/// PDF/A document should be web-optimized (linearized).
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  This method assumes that the first parameter passed in PDFACompliance
		/// constructor (i.e. the convert parameter) is set to 'true'.</remarks>
		public void SaveAs(string file_path, bool linearized)
        {
            UString filePath = new UString(file_path);
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFAComplianceSaveAsFromFileName(mp_pdfac, filePath.mp_impl, linearized));
        }


		/// <summary> Serializes the converted PDF/A document to a memory buffer.</summary>
		/// <returns>Byte array containing the serialized version of the document</returns>
		/// <param name="linearized"> An optional flag used to specify whether the the resulting
		/// PDF/A document should be web-optimized (linearized).
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  This method assumes that the first parameter passed in PDFACompliance
		/// constructor (i.e. the convert parameter) is set to 'true'.</remarks>
		public byte[] SaveAs(bool linearized)
        {
            UIntPtr size = UIntPtr.Zero;
            IntPtr source = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFAComplianceSaveAsFromBuffer(mp_pdfac, ref source, ref size, linearized));
            byte[] buf = new byte[size.ToUInt32()];
            System.Runtime.InteropServices.Marshal.Copy(source, buf, 0, System.Convert.ToInt32(size.ToUInt32()));
            return buf;
        }

		/// <summary> Serializes the converted PDF/A document to a stream.</summary>
		/// <param name="stm">A stream where to serialize the document.</param>
		/// <param name="linearized"> An optional flag used to specify whether the the resulting
		/// PDF/A document should be web-optimized (linearized).
		/// </param>
		/// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
		/// <remarks>  This method assumes that the first parameter passed in PDFACompliance
		/// constructor (i.e. the convert parameter) is set to 'true'.</remarks>
        public void SaveAs(System.IO.Stream stm, bool linearized) 
        {
            UIntPtr size = UIntPtr.Zero;
            IntPtr source = IntPtr.Zero;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PDFAComplianceSaveAsFromBuffer(mp_pdfac, ref source, ref size, linearized));
            byte[] buf = new byte[size.ToUInt32()];
            System.Runtime.InteropServices.Marshal.Copy(source, buf, 0, System.Convert.ToInt32(size.ToUInt32()));
            foreach (byte b in buf)
            {
                stm.WriteByte(b);
            }
            stm.Flush();
        }


        /// <summary> PDF/A Conformance Level (19005:1/2/3).
        ///
        /// Level A conforming files must adhere to all of the requirements of ISO 19005. A file meeting this conformance level is said to be a 'conforming PDF/A -1a file.'
        ///	Level B conforming files shall adhere to all of the requirements of ISO 19005 except those of 6.3.8 and 6.8. A file meeting this conformance level is said to be 
        /// a 'conforming PDF/A-1b file'. The Level B conformance requirements are intended to be those minimally necessary to ensure that the rendered visual appearance of 
        /// a conforming file is preservable over the long term. </summary>
        public enum Conformance 
        {
			//TODO: enum values missing description
            e_NoConformance = 0,
			e_Level1A,
			e_Level1B,
			e_Level2A,
			e_Level2B,
			e_Level2U,
			e_Level3A,
			e_Level3B,
			e_Level3U,
		}

        ///<summary>PDF/A Level A and B Validation Errors</summary>
        public enum ErrorCode
		{
            /// <summary>Invalid PDF structure.</summary>
            e_PDFA0_1_0 = 10,
			///<summary>Corrupt document.</summary>
			e_PDFA0_1_1  =11,  
			///<summary>Corrupt content stream.</summary>
			e_PDFA0_1_2  =12,  
			///<summary>Using JPEG2000 compression (PDF 1.4 compatibility).</summary>
			e_PDFA0_1_3  =13,  
			///<summary>Contains compressed object streams (PDF 1.4 compatibility).</summary>
			e_PDFA0_1_4  =14,  
			///<summary>Contains cross-reference streams (PDF 1.4 compatibility).</summary>
			e_PDFA0_1_5  =15,  
			///<summary>Document does not start with % character.</summary>
			e_PDFA1_2_1  =121, 
			///<summary>File header line not followed by % and 4 characters > 127.</summary>
			e_PDFA1_2_2  =122,  
			///<summary>The trailer dictionary does not contain ID.</summary>
			e_PDFA1_3_1  =131,  
			///<summary>Trailer dictionary contains Encrypt.</summary>
			e_PDFA1_3_2  =132,  
			///<summary>Data after last EOF marker.</summary>
			e_PDFA1_3_3  =133,  
			///<summary>Linearized file: ID in 1st page and last trailer are different.</summary>
			e_PDFA1_3_4  =134,  
			///<summary>Subsection header: starting object number and range not separated by a single space.</summary>
			e_PDFA1_4_1  =141,  
			///<summary>'xref' and cross reference subsection header not separated by a single EOL marker.</summary>
			e_PDFA1_4_2  =142,  
			///<summary>Invalid hexadecimal strings used.</summary>
			e_PDFA1_6_1  =161,  
			///<summary>The 'stream' token is not followed by CR and LF or a single LF.</summary>
			e_PDFA1_7_1  =171,  
			///<summary>The 'endstream' token is not preceded by EOL.</summary>
			e_PDFA1_7_2  =172,  
			///<summary>The value of Length does not match the number of bytes.</summary>
			e_PDFA1_7_3  =173,  
			///<summary>A stream object dictionary contains the F, FFilter, or FDecodeParms keys.</summary>
			e_PDFA1_7_4  =174,  
			///<summary>Object number and generation number are not separated by a single white-space.</summary>
			e_PDFA1_8_1  =181,  
			///<summary>Generation number and 'obj' are not separated by a single white-space.</summary>
			e_PDFA1_8_2  =182,  
			///<summary>Object number not preceded by EOL marker</summary>
			e_PDFA1_8_3  =183,  
			///<summary>'endobj' not preceded by EOL marker</summary>
			e_PDFA1_8_4  =184,  
			///<summary>'obj' not followed by EOL marker</summary>
			e_PDFA1_8_5  =185,  
			///<summary>'endobj' not followed by EOL marker</summary>
			e_PDFA1_8_6  =186,  
			///<summary>Invalid UTF8 string	</summary>
			e_PDFA1_8_7  =187,  		
			///<summary>Using LZW compression.</summary>
			e_PDFA1_10_1 =1101, 
			///<summary>A file specification dictionary contains the EF key.</summary>
			e_PDFA1_11_1 =1111, 
			///<summary>Contains the EmbeddedFiles key</summary>
			e_PDFA1_11_2 =1112, 
			///<summary>Array contains more than 8191 elements</summary>
			e_PDFA1_12_1 =1121, 
			///<summary>Dictionary contains more than 4095 elements</summary>
			e_PDFA1_12_2 =1122, 
			///<summary>Name with more than 127 bytes</summary>
			e_PDFA1_12_3 =1123, 
			///<summary>Contains an integer value outside of the allowed range [-2^31, 2^31-1],</summary>
			e_PDFA1_12_4 =1124, 
			///<summary>Exceeds the maximum number (8,388,607) of indirect objects in a PDF file.</summary>
			e_PDFA1_12_5 =1125, 
			///<summary>The number of nested q/Q operators is greater than 28.</summary>
			e_PDFA1_12_6 =1126, 
			///<summary>Optional content (layers) not allowed.</summary>
			e_PDFA1_13_1 =1131, 
			///<summary>DestOutputProfile-s in OutputIntents array do not match.</summary>
			e_PDFA2_2_1  =221,  
			///<summary>Not a valid ICC color profile.</summary>
			e_PDFA2_3_2  =232,  
			///<summary> The N entry does not match the number of color components in the embedded ICC profile.</summary>
			e_PDFA2_3_3  =233, 
			///<summary> Device-specific color space used, but no GTS_PDFA1 OutputIntent.</summary>
			e_PDFA2_3_3_1=2331,
			///<summary>Device-specific color space, does not match OutputIntent.</summary>
			e_PDFA2_3_3_2=2332, 
			///<summary>Device-specific color space used in an alternate color space.</summary>
			e_PDFA2_3_4_1=2341, 
			///<summary>Image with Alternates key.</summary>
			e_PDFA2_4_1  =241,  
			///<summary>Image with OPI key.</summary>
			e_PDFA2_4_2  =242,  
			///<summary>Image with invalid rendering intent.</summary>
			e_PDFA2_4_3  =243,  
			///<summary>Image with Interpolate key set to true.</summary>
			e_PDFA2_4_4  =244,  
			///<summary>XObject with OPI key.</summary>
			e_PDFA2_5_1  =251,  
			///<summary>PostScript XObject.</summary>
			e_PDFA2_5_2  =252,  
			///<summary>Contains a reference XObject.</summary>
			e_PDFA2_6_1  =261,  
			///<summary>Contains an XObject that is not supported (e.g. PostScript XObject).</summary>
			e_PDFA2_7_1  =271,  
			///<summary>Contains an invalid Transfer Curve in the extended graphics state.</summary>
			e_PDFA2_8_1  =281,  
			///<summary>Use of an invalid rendering intent.</summary>
			e_PDFA2_9_1  =291,  
			///<summary>Illegal operator.</summary>
			e_PDFA2_10_1 =2101, 
			///<summary>Embedded font is damaged.</summary>
			e_PDFA3_2_1  =321,  
			///<summary>Incompatible CIDSystemInfo entries</summary>
			e_PDFA3_3_1  =331,  
			///<summary>Type 2 CIDFont without CIDToGIDMap</summary>
			e_PDFA3_3_2  =332,  
			///<summary>CMap not embedded</summary>
			e_PDFA3_3_3_1=3331, 
			///<summary>Inconsistent WMode in embedded CMap dictionary and stream.</summary>
			e_PDFA3_3_3_2=3332, 
			///<summary>The font is not embedded.</summary>
			e_PDFA3_4_1  =341,  
			///<summary>Embedded composite (Type0) font program does not define all font glyphs.</summary>
			e_PDFA3_5_1  =351,  
			///<summary>Embedded Type1 font program does not define all font glyphs.</summary>
			e_PDFA3_5_2  =352,  
			///<summary>Embedded TrueType font program does not define all font glyphs.</summary>
			e_PDFA3_5_3  =353,  
			///<summary>The font descriptor dictionary does not include a	CIDSet stream for CIDFont subset.</summary>
			e_PDFA3_5_4  =354,  
			///<summary>The font descriptor dictionary does not include a	CharSet string for Type1 font subset.</summary>
			e_PDFA3_5_5  =355,  
			///<summary>Widths in embedded font are inconsistent with /Widths entry in the font dictionary.</summary>
			e_PDFA3_6_1  =361,  
			///<summary> A non-symbolic TrueType font must use WinAnsiEncoding or MacRomanEncoding.</summary>
			e_PDFA3_7_1  =371,
			///<summary>A symbolic TrueType font must not specify encoding.</summary>
			e_PDFA3_7_2  =372,  
			///<summary>A symbolic TrueType font does not have exactly one entry in cmap table.</summary>
			e_PDFA3_7_3  =373,  
			///<summary>Transparency used (ExtGState with soft mask).</summary>
			e_PDFA4_1    =41,   
			///<summary>Transparency used (XObject with soft mask).</summary>
			e_PDFA4_2    =42,   
			///<summary> Transparency used (Form XObject with transparency group).</summary>
			e_PDFA4_3    =43, 
			///<summary> Transparency used (Blend mode is not 'Normal').</summary>
			e_PDFA4_4    =44, 
			///<summary> Transparency used ('CA' value is not 1.0).</summary>
			e_PDFA4_5    =45, 
			///<summary>Transparency used ('ca' value is not 1.0).</summary>
			e_PDFA4_6    =46,   
			///<summary>Unknown annotation type.</summary>
			e_PDFA5_2_1  =521,  
			///<summary> FileAttachment annotation is not permitted.</summary>
			e_PDFA5_2_2  =522,
			///<summary> Sound annotation is not permitted.</summary>
			e_PDFA5_2_3  =523,
			///<summary>Movie annotation is not permitted.</summary>
			e_PDFA5_2_4  =524,  
			///<summary> Redact annotation is not permitted.</summary>
			e_PDFA5_2_5  =525,
			///<summary> 3D annotation is not permitted.</summary>
			e_PDFA5_2_6  =526,
			///<summary> Caret annotation is not permitted.</summary>
			e_PDFA5_2_7  =527, 
			///<summary>Watermark annotation is not permitted.</summary>
			e_PDFA5_2_8  =528,  
			///<summary>Polygon annotation is not permitted.</summary>
			e_PDFA5_2_9  =529,  
			///<summary>PolyLine annotation is not permitted.</summary>
			e_PDFA5_2_10 =5210, 
			///<summary>Screen annotation is not permitted.</summary>
			e_PDFA5_2_11 =5211, 
			///<summary>An annotation dictionary contains the CA key with a value other than 1.0.</summary>
			e_PDFA5_3_1  =531,  
			///<summary>An annotation dictionary is missing F key.</summary>
			e_PDFA5_3_2_1=5321, 
			///<summary>An annotation's 'Print' flag is not set.</summary>
			e_PDFA5_3_2_2=5322, 
			///<summary>An annotation's 'Hidden' flag is set.</summary>
			e_PDFA5_3_2_3=5323, 
			///<summary>An annotation's 'Invisible' flag is set.</summary>
			e_PDFA5_3_2_4=5324, 
			///<summary>An annotation's 'NoView' flag is set.</summary>
			e_PDFA5_3_2_5=5325, 
			///<summary>An annotation's C entry present but no OutputIntent present</summary>
			e_PDFA5_3_3_1=5331, 
			///<summary> An annotation's C entry present but OutputIntent has non-RGB destination profile</summary>
			e_PDFA5_3_3_2=5332,
			///<summary>An annotation's IC entry present but no OutputIntent present</summary>
			e_PDFA5_3_3_3=5333, 
			///<summary>An annotation's IC entry present and OutputIntent has non-RGB destination profile</summary>
			e_PDFA5_3_3_4=5334, 
			///<summary>An annotation AP dictionary has entries other than the N entry.</summary>
			e_PDFA5_3_4_1=5341, 
			///<summary>An annotation AP dictionary does not contain N entry</summary>
			e_PDFA5_3_4_2=5342, 
			///<summary>AP has an N entry whose value is not a stream</summary>
			e_PDFA5_3_4_3=5343, 
			///<summary> Contains an action type that is not permitted.</summary>
			e_PDFA6_1_1  =611,
			///<summary> Contains a non-predefined Named action.</summary>
			e_PDFA6_1_2  =612,
			///<summary>The document catalog dictionary contains AA entry.</summary>
			e_PDFA6_2_1  =621,  
			///<summary> Contains the JavaScript key.</summary>
			e_PDFA6_2_2 = 622,
			///<summary>Invalid destination.</summary>
			e_PDFA6_2_3 = 623,  
			///<summary>The document catalog does not contain Metadata stream.</summary>
			e_PDFA7_2_1  =721,  
			///<summary>The Metadata object stream contains Filter key.</summary>
			e_PDFA7_2_2  =722,  
			///<summary>The XMP Metadata stream is not valid.</summary>
			e_PDFA7_2_3  =723,  
			///<summary>XMP property not predefined and no extension schema present.</summary>
			e_PDFA7_2_4  =724,  
			///<summary> XMP not included in 'xpacket'.</summary>
			e_PDFA7_2_5  =725,
			///<summary>Document information entry 'Title' not synchronized with XMP.</summary>
			e_PDFA7_3_1  =731,  
			///<summary>Document information entry 'Author' not synchronized with XMP.</summary>
			e_PDFA7_3_2  =732,  
			///<summary> Document information entry 'Subject' not synchronized with XMP.</summary>
			e_PDFA7_3_3  =733,
			///<summary> Document information entry 'Keywords' not synchronized with XMP.</summary>
			e_PDFA7_3_4  =734,
			///<summary> Document information entry 'Creator' not synchronized with XMP.</summary>
			e_PDFA7_3_5  =735,
			///<summary> Document information entry 'Producer' not synchronized with XMP.</summary>
			e_PDFA7_3_6  =736,
			///<summary>Document information entry 'CreationDate' not synchronized with XMP.</summary>
			e_PDFA7_3_7  =737,  
			///<summary>Document information entry 'ModDate' not synchronized with XMP.</summary>
			e_PDFA7_3_8  =738,  
			///<summary> Wrong value type for predefined XMP property.</summary>
			e_PDFA7_3_9  =739,
			///<summary>'bytes' and 'encoding' attributes are allowed in the header of an XMP packet.</summary>
			e_PDFA7_5_1  =751,  
			///<summary>XMP Extension schema doesn't have a description.</summary>
			e_PDFA7_8_1  =781,  
			///<summary> XMP Extension schema is not valid. Required property 'namespaceURI' might be missing in PDF/A Schema value Type.</summary>
			e_PDFA7_8_2  =782,
			///<summary>'pdfaExtension:schemas' not found. </summary>
			e_PDFA7_8_3  =783,  
			///<summary>'pdfaExtension:schemas' is using a wrong value type.</summary>
			e_PDFA7_8_4  =784,  
			///<summary> 'pdfaExtension:property' not found. </summary>
			e_PDFA7_8_5  =785,
			///<summary>'pdfaExtension:property' is using a wrong value type.</summary>
			e_PDFA7_8_6  =786,  
			///<summary>'pdfaProperty:name' not found. </summary>
			e_PDFA7_8_7  =787,  
			///<summary>'pdfaProperty:name' is using a wrong value type.</summary>
			e_PDFA7_8_8  =788,  
			///<summary>A description for a property is missing in 'pdfaSchema:property' sequence.</summary>
			e_PDFA7_8_9  =789,  
			///<summary>'pdfaProperty:valueType' not found.</summary>
			e_PDFA7_8_10 =7810, 
			///<summary>The required namespace prefix for extension schema is 'pdfaExtension'.</summary>
			e_PDFA7_8_11 =7811, 
			///<summary>The required field namespace prefix is 'pdfaSchema'.</summary>
			e_PDFA7_8_12 =7812, 
			///<summary>The required field namespace prefix is 'pdfaProperty'.</summary>
			e_PDFA7_8_13 =7813, 
			///<summary>The required field namespace prefix is 'pdfaType'.</summary>
			e_PDFA7_8_14 =7814, 
			///<summary>The required field namespace prefix is 'pdfaField'.</summary>
			e_PDFA7_8_15 =7815, 
			///<summary>'pdfaSchema:valueType' not found.</summary>
			e_PDFA7_8_16 =7816, 
			///<summary>'pdfaSchema:valueType' is using a wrong value type.</summary>
			e_PDFA7_8_17 =7817, 
			///<summary>Required property 'valueType' missing in PDF/A Schema Value Type.</summary>
			e_PDFA7_8_18= 7818, 
			///<summary>'pdfaType:type' not found.</summary>
			e_PDFA7_8_19= 7819, 
			///<summary>'pdfaType:type' is using a wrong value type.</summary>
			e_PDFA7_8_20 =7820, 
			///<summary>'pdfaType:description' not found.</summary>
			e_PDFA7_8_21 =7821, 
			///<summary>'pdfaType:namespaceURI' not found.</summary>
			e_PDFA7_8_22 =7822, 
			///<summary>'pdfaType:field' is using a wrong value type.</summary>
			e_PDFA7_8_23 =7823, 
			///<summary>'pdfaField:name' not found.</summary>
			e_PDFA7_8_24 =7824, 
			///<summary>'pdfaField:name' is using a wrong value type.</summary>
			e_PDFA7_8_25 =7825, 
			///<summary>'pdfaField:valueType' not found.</summary>
			e_PDFA7_8_26 =7826, 
			///<summary>'pdfaField:valueType' is using a wrong type.</summary>
			e_PDFA7_8_27 =7827, 
			///<summary>'pdfaField:description' not found.</summary>
			e_PDFA7_8_28 =7828, 
			///<summary>'pdfaField:description' is using a wrong type.</summary>
			e_PDFA7_8_29 =7829, 
			///<summary>Required description for 'pdfaField::valueType' is missing.</summary>
			e_PDFA7_8_30 =7830, 
			///<summary>A property doesn't match its custom schema type.</summary>
			e_PDFA7_8_31 =7831, 
			///<summary>Missing PDF/A identifier</summary>
			e_PDFA7_11_1 =7111, 
			///<summary>Invalid PDF/A identifier namespace</summary>
			e_PDFA7_11_2 =7112, 
			///<summary>Invalid PDF/A conformance level.</summary>
			e_PDFA7_11_3 =7113, 
			///<summary>Invalid PDF/A part number.</summary>
			e_PDFA7_11_4 =7114, 
			///<summary>Invalid PDF/A amendment identifier.</summary>
			e_PDFA7_11_5 =7115, 
			///<summary>An interactive form field contains an action.</summary>
			e_PDFA9_1    =91,  
			///<summary>The NeedAppearances flag in the interactive form dictionary is set to true.</summary>
			e_PDFA9_2    =92, 
			///<summary>AcroForms contains XFA.</summary>
			e_PDFA9_3    =93,   
			///<summary>Catalog contains NeedsRendering.</summary>
			e_PDFA9_4    =94,   
			// PDF/A-1 Level A Validation Errors --------------------------------
			///<summary>The font dictionary is missing 'ToUnicode' entry.</summary>
			e_PDFA3_8_1  =381,  
			///<summary>The PDF is not marked as Tagged PDF.</summary>
			e_PDFA8_2_2  =822,  
			///<summary>Bad StructTreeRoot </summary>
			e_PDFA8_3_3_1=8331, 
			///<summary>Each structure element dictionary in the structure hierarchy must have a Type entry with the name value of StructElem.</summary>
			e_PDFA8_3_3_2=8332, 
			///<summary>A non-standard structure type does not map to a standard type.</summary>
			e_PDFA8_3_4_1=8341, 


			// PDF/A-2 Level B Validation Errors --------------------------------
			///<summary>Bad file header.</summary>
			e_PDFA1_2_3   =123,     
			///<summary>Invalid use of Crypt filter.</summary>
			e_PDFA1_10_2  =1102,    
			///<summary>Bad stream Filter.</summary>
			e_PDFA1_10_3  =1103,    
			///<summary>Bad Permission Dictionary</summary>
			e_PDFA1_12_10 =11210,   
			///<summary>Page dimensions are outside of the allowed range (3-14400).</summary>
			e_PDFA1_13_5  =1135,    
			///<summary>Contains DestOutputProfileRef</summary>
			e_PDFA2_3_10  =2310,    
			///<summary>OPM is 1</summary>
			e_PDFA2_4_2_10 =24220,  
			///<summary>Incorrect colorant specification in DeviceN </summary>
			e_PDFA2_4_2_11 =24221,  
			///<summary>tintTransform is different in Separations with the same colorant name.</summary>
			e_PDFA2_4_2_12 =24222,  
			///<summary>alternateSpace is different in Separations with the same colorant name.</summary>
			e_PDFA2_4_2_13 =24223,  
			///<summary>HTP entry in ExtGState.</summary>
			e_PDFA2_5_10   =2510,   
			///<summary>Unsupported HalftoneType.</summary>
			e_PDFA2_5_11   =2511,   
			///<summary>Uses HalftoneName key.</summary>
			e_PDFA2_5_12   =2512,   
			///<summary>JPEG2000: Only the JPX baseline is supported.</summary>
			e_PDFA2_8_3_1  =2831,   
			///<summary>JPEG2000: Invalid number of colour channels.</summary>
			e_PDFA2_8_3_2  =2832,   
			///<summary>JPEG2000: Invalid color space.</summary>
			e_PDFA2_8_3_3  =2833,   
			///<summary>JPEG2000: The bit-depth JPEG2000 data must be in range 1-38. </summary>
			e_PDFA2_8_3_4  =2834,   
			///<summary>JPEG2000: All colour channels in the JPEG2000 data must have the same bit-depth.</summary>
			e_PDFA2_8_3_5  =2835,   
			///<summary>Page Group entry is missing in a document without OutputIntent.</summary>
			e_PDFA2_10_20  =21020,  
			///<summary>Invalid blend mode.</summary>
			e_PDFA2_10_21  =21021,  
			///<summary>Catalog contains Requirements key.</summary>
			e_PDFA11_0_0   =11000,  
			///<summary>PresSteps is not allowed.</summary>			
			e_PDFA6_10_0   =6100,
			///<summary>AlternatePresentations not allowed.</summary>			
			e_PDFA6_10_1   =6101, 
			///<summary>Some characters map to 0 or FFFE.</summary>			
			e_PDFA6_2_11_5 =62115,
			///<summary>Some text can't be mapped to Unicode.</summary>			
			e_PDFA6_2_11_6 =62116,
			///<summary>PUA characters are missing ActualText.</summary>			
			e_PDFA6_2_11_7 =62117,
			///<summary>Use of .notdef glyph</summary>
			e_PDFA6_2_11_8 =62118,  
			///<summary>Optional content Missing Name entry</summary>
			e_PDFA6_9_1    =69001,
			///<summary>Optional content Contains AS entry</summary>
			e_PDFA6_9_3    =69003,
			///<summary>FileSpec is missing F or UF key</summary>
			e_PDFA8_1      =81,     
			
			// PDF/A-3 Validation Errors --------------------------------------
			///<summary>Embedded file has no MIME type entry</summary>
			e_PDFA_3E1     =1,      
			///<summary>Embedded file Params has no ModDate entry</summary>
			e_PDFA_3E1_1   =101,
			///<summary>Embedded file has no AFRelationship</summary>
			e_PDFA_3E2     =2,      
			///<summary>Doc catalog is missing AF entry</summary>
			e_PDFA_3E3     =3,      

			///<summary></summary>
			e_PDFA_LAST
		}
    }
}