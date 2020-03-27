#if (__DESKTOP__)

using System;
using pdftron.SDF;

namespace pdftron.PDF
{
    /// <summary> PrinterMode is a utility class used to set printer options for printing PDF documents.</summary>
    public class PrinterMode : IDisposable
    {
        internal ObjSet mp_printerModeSet;
        internal Obj mp_printerMode;

        public PrinterMode()
        {
            mp_printerModeSet = new ObjSet();
            mp_printerMode = mp_printerModeSet.CreateDict();
        }

        /// <summary> Set automatic centering of document pages onto the output pages.
        /// 
        /// </summary>
        /// <param name="autoCenter">if true will center document pages onto the output
        /// pages. Default is true.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetAutoCenter(bool autoCenter)
        {
            mp_printerMode.PutBool("AutoCenter", autoCenter);
        }

        /// <summary> Set automatic rotation of document pages to best fit the output pages.
        /// 
        /// </summary>
        /// <param name="autoRotate">if true will rotate document pages onto the output
        /// pages. Default is true.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetAutoRotate(bool autoRotate)
        {
            mp_printerMode.PutBool("AutoRotate", autoRotate);
        }

        /// <summary> Set the collation of the printing, useful for multiple copies.
        /// 
        /// </summary>
        /// <param name="collation">if true, pages of copies will be printed 1, 2, 3.
        /// if false, then pages of copies will be printed 1, 1, 1, ..., 2, 2, 2, ...
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetCollation(bool collation)
        {
            mp_printerMode.PutBool("Collation", collation);
        }

        /// <summary> Set the number of copies to be printed.
        /// 
        /// </summary>
        /// <param name="copyCount">the number of copies to be printed, must be greater than zero
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetCopyCount(int copyCount)
        {
            mp_printerMode.PutNumber("CopyCount", copyCount);
        }

        /// <summary> Set the DPI (dots per inch) of the printing.
        /// 
        /// </summary>
        /// <param name="dpi">the new dPI
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetDPI(int dpi)
        {
            mp_printerMode.PutNumber("DPI", dpi);
        }
        /// <summary> Set the duplexing mode.
        /// 
        /// </summary>
        /// <param name="mode">the new duplexing
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetDuplexing(DuplexMode mode)
        {
            mp_printerMode.PutNumber("Duplexing", (double)mode);
        }
        /// <summary> Set the number of document pages to place on the output pages
        /// across and vertically.  Pages will be automatically rotated to
        /// best fit the page.
        /// 
        /// </summary>
        /// <param name="nup">given <c>NUp</c> object
        /// </param>				
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetNUp(NUp nup)
        {
            SetNUp(nup, NUpPageOrder.e_PageOrder_LeftToRightThenTopToBottom);
        }

        /// <summary> Set the number of document pages to place on the output pages
        /// across and vertically.  Pages will be automatically rotated to
        /// best fit the page.
        /// 
        /// </summary>
        /// <param name="nup">one of {e_NUp_1_1, e_NUp_2_1, e_NUp_2_2, e_NUp_3_2, e_NUp_3_3,
        /// e_NUp_4_4}.  Default is e_NUp_1_1.
        /// </param>
        /// <param name="pageOrder">order of document pages across and down output page.
        /// Default is e_PageOrder_LeftToRightThenTopToBottom.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetNUp(NUp nup, NUpPageOrder pageOrder)
        {
            mp_printerMode.PutNumber("NupType", (double)nup);
            mp_printerMode.PutNumber("NUpPageOrder", (double)pageOrder);
        }

        /// <summary> Set the number of document pages to place on the output pages
        /// across and vertically.  Pages will be automatically rotated to
        /// best fit the page.
        /// 
        /// </summary>
        /// <param name="x">number of papes in horizontal direction
        /// </param>
        /// <param name="y">number of pages in vertical direction
        /// </param>			
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetNUp(uint x, uint y)
        {
            SetNUp(x, y, NUpPageOrder.e_PageOrder_LeftToRightThenTopToBottom);
        }
        /// <summary> Set the number of document pages to place on the output pages
        /// across and vertically.  Pages will be automatically rotated to
        /// best fit the page.
        /// 
        /// Typical values: (2,1) 2-up; (2,2) 4 per page, etc
        /// 
        /// </summary>
        /// <param name="x">number of document pages across.  Default is 1.
        /// </param>
        /// <param name="y">number of document pages down.  Default is 1.
        /// </param>
        /// <param name="pageOrder">order of document pages across and down output page
        /// Default is e_PageOrder_LeftToRightThenTopToBottom.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetNUp(uint x, uint y, NUpPageOrder pageOrder)
        {
            mp_printerMode.PutNumber("NupType", (double)-1);
            mp_printerMode.PutNumber("NupX", (double)((int)x));
            mp_printerMode.PutNumber("NupY", (double)((int)y));
            mp_printerMode.PutNumber("NUpPageOrder", (double)pageOrder);
        }

        /// <summary> Set the orientation of the output document.
        /// 
        /// </summary>
        /// <param name="orientation">{e_Orientation_Portrait, e_Orientation_Landscape}
        /// Default is e_Orientation_Portrait.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetOrientation(Orientation orientation)
        {
            mp_printerMode.PutNumber("Orientation", (double)orientation);
        }

        /// <summary> Set the printing of annotations.
        /// 
        /// </summary>
        /// <param name="printContent">one of {e_PrintContent_DocumentOnly,
        /// e_PrintContent_DocumentAndAnnotations}.
        /// Default is e_PrintContent_DocumentAndAnnotations.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetOutputAnnot(PrintContentTypes printContent)
        {
            mp_printerMode.PutNumber("PrintContent", (double)printContent);
        }

        /// <summary> Set the color output of the printing.
        /// 
        /// </summary>
        /// <param name="color">the new output color
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetOutputColor(OutputColor color)
        {
            mp_printerMode.PutNumber("OutputColor", (double)color);
        }

        /// <summary> Set the quality of the printing. Overridden if SetDPI is called.
        /// 
        /// </summary>
        /// <param name="quality">the new output quality
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetOutputQuality(OutputQuality quality)
        {
            mp_printerMode.PutNumber("OutputQuality", (double)quality);
        }

        /// <summary> Set the output printer paper size (assumed to be correct).
        /// 
        /// </summary>
        /// <param name="size">the size of the output paper size in points (72 points = 1 inch).
        /// Default is US Letter or Rect(0, 0, 612, 792)
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPaperSize(Rect size)
        {
            mp_printerMode.PutNumber("PaperSizeX1", size.x1);
            mp_printerMode.PutNumber("PaperSizeX2", size.x2);
            mp_printerMode.PutNumber("PaperSizeY1", size.y1);
            mp_printerMode.PutNumber("PaperSizeY2", size.y2);
            mp_printerMode.PutNumber("PaperSize", 0);//0 is custom paper size
        }

        /// <summary> Set the output printer paper size (assumed to be correct).
        /// 
        /// </summary>
        /// <param name="size"><c>PaperSize</c> object
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetPaperSize(PaperSize size)
        {
            mp_printerMode.PutNumber("PaperSize", (int)size);
            if (size != pdftron.PDF.PrinterMode.PaperSize.e_custom)
            {
                mp_printerMode.PutNumber("PaperSizeX1", 0);
                mp_printerMode.PutNumber("PaperSizeX2", 0);
                mp_printerMode.PutNumber("PaperSizeY1", 0);
                mp_printerMode.PutNumber("PaperSizeY2", 0);
            }
        }

        /// <summary> Set the printing of page borders, helpful when printing multiple document
        /// pages per output page.
        /// 
        /// </summary>
        /// <param name="printBorder">if true will add a thin frame around each page border.
        /// Default is false.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetOutputPageBorder(bool printBorder)
        {
            mp_printerMode.PutBool("OutputPageBorder", printBorder);
        }

        /// <summary> Set the scaling of the document pages to the output pages. Causes
        /// SetScaleType(e_ScaleType_CustomScale) to be set.
        /// 
        /// </summary>
        /// <param name="scale">to apply to document pages.  1.0 is no scale, greater
        /// than 1.0 increases document page sizes, less than 1.0 reduces
        /// document pages sizes on output pages.  Default is 1.0
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetScale(double scale)
        {
            mp_printerMode.PutNumber("ScaleType", (double)-1);
            mp_printerMode.PutNumber("ScaleValue", scale);
        }
        /// <summary> Set the scaling of the document page to the output pages.
        /// 
        /// </summary>
        /// <param name="scaleType">one of {e_ScaleType_None, e_ScaleType_FitToOutputPage,
        /// e_ScaleType_ReduceToOutputPage}. Default is e_ScaleType_None.
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetScaleType(ScaleType scaleType)
        {
            mp_printerMode.PutNumber("ScaleType", (double)scaleType);
        }

        /// <summary> Set whether RLE image compression is used for printing bitmaps.
        /// 
        /// </summary>
        /// <param name="useRleImageCompression">if true, printer spool file will be
        /// reduced. However, some printers do not support this type of image
        /// compression and will produce blank pages.  Default is false
        /// </param>
        /// <exception cref="PDFNetException">  PDFNetException the PDFNet exception </exception>
        public void SetUseRleImageCompression(bool useRleImageCompression)
        {
            mp_printerMode.PutBool("UseRleImageCompression", useRleImageCompression);
        }

        /// <summary> Paper sizes. </summary>
        public enum PaperSize
        {
            //TODO: enum values missing description
            e_custom = 0,
            e_letter,
            e_letter_small,
            e_tabloid,
            e_ledger,
            e_legal,
            e_statement,
            e_executive,
            e_a3,
            e_a4,
            e_a4_small,
            e_a5,
            e_b4_jis,
            e_b5_jis,
            e_folio,
            e_quarto,
            e_10x14,
            e_11x17,
            e_note,
            e_envelope_9,
            e_envelope_10,
            e_envelope_11,
            e_envelope_12,
            e_envelope_14,
            e_c_size_sheet,
            e_d_size_sheet,
            e_e_size_sheet,
            e_envelope_dl,
            e_envelope_c5,
            e_envelope_c3,
            e_envelope_c4,
            e_envelope_c6,
            e_envelope_c65,
            e_envelope_b4,
            e_envelope_b5,
            e_envelope_b6,
            e_envelope_italy,
            e_envelope_monarch,
            e_6_3_quarters_envelope,
            e_us_std_fanfold,
            e_german_std_fanfold,
            e_german_legal_fanfold,
            e_b4_iso,
            e_japanese_postcard,
            e_9x11,
            e_10x11,
            e_15x11,
            e_envelope_invite,
            e_reserved_48,
            e_reserved_49,
            e_letter_extra,
            e_legal_extra,
            e_tabloid_extra,
            e_a4_extra,
            e_letter_transverse,
            e_a4_transverse,
            e_letter_extra_transverse,
            e_supera_supera_a4,
            e_Superb_Superb_a3,
            e_letter_plus,
            e_a4_plus,
            e_a5_transverse,
            e_b5_jis_transverse,
            e_a3_extra,
            e_a5_extra,
            e_b5_iso_extra,
            e_a2,
            e_a3_transverse,
            e_a3_extra_transverse,
            e_japanese_double_postcard,
            e_a6,
            e_japanese_envelope_kaku_2,
            e_japanese_envelope_kaku_3,
            e_japanese_envelope_chou_3,
            e_japanese_envelope_chou_4,
            e_letter_rotated,
            e_a3_rotated,
            e_a4_rotated,
            e_a5_rotated,
            e_b4_jis_rotated,
            e_b5_jis_rotated,
            e_japanese_postcard_rotated,
            e_double_japanese_postcard_rotated,
            e_a6_rotated,
            e_japanese_envelope_kaku_2_rotated,
            e_japanese_envelope_kaku_3_rotated,
            e_japanese_envelope_chou_3_rotated,
            e_japanese_envelope_chou_4_rotated,
            e_b6_jis,
            e_b6_jis_rotated,
            e_12x11,
            e_japanese_envelope_you_4,
            e_japanese_envelope_you_4_rotated,
            e_prc_16k,
            e_prc_32k,
            e_prc_32k_big,
            e_prc_envelop_1,
            e_prc_envelop_2,
            e_prc_envelop_3,
            e_prc_envelop_4,
            e_prc_envelop_5,
            e_prc_envelop_6,
            e_prc_envelop_7,
            e_prc_envelop_8,
            e_prc_envelop_9,
            e_prc_envelop_10,
            e_prc_16k_rotated,
            e_prc_32k_rotated,
            e_prc_32k_big__rotated,
            e_prc_envelop_1_rotated,
            e_prc_envelop_2_rotated,
            e_prc_envelop_3_rotated,
            e_prc_envelop_4_rotated,
            e_prc_envelop_5_rotated,
            e_prc_envelop_6_rotated,
            e_prc_envelop_7_rotated,
            e_prc_envelop_8_rotated,
            e_prc_envelop_9_rotated,
            e_prc_envelop_10_rotated
        }

        ///<summary> Enumerated values for specifying how the printed pages are flipped when duplexing</summary>
        public enum DuplexMode
        {
            ///<summary>use the current printer setting</summary>
            e_Duplex_Auto = 0,
            ///<summary>single-sided printing</summary>
            e_Duplex_None = 1,
            ///<summary>flip the paper along the long side</summary>
            e_Duplex_LongSide = 2,
            ///<summary>flip the paper along the short side</summary>
            e_Duplex_ShortSide = 3
        }

        ///<summary>Enumerated values for specifying the quality of the printing</summary>
        public enum OutputQuality
        {
            ///<summary>printer draft mode</summary>
            e_OutputQuality_Draft = -1,
            ///<summary>printer low quality mode</summary>
            e_OutputQuality_Low = -2,
            ///<summary>printer medium quality mode</summary>
            e_OutputQuality_Medium = -3,
            ///<summary>printer high quality mode</summary>
            e_OutputQuality_High = -4
        }

        ///<summary>Enumerated values for specifying the color mode for printing</summary>
        public enum OutputColor
        {
            ///<summary>24bpp</summary>
            e_OutputColor_Color = 0,
            ///<summary>8bpp</summary>
            e_OutputColor_Grayscale = 1,
            ///<summary>single color (1bpp)</summary>
            e_OutputColor_Monochrome = 2,
        }

        ///<summary>Enumerated values for specifying the orientation of output pages</summary>
        public enum Orientation
        {
            ///<summary>taller than wide</summary>
            e_Orientation_Portrait = 0,
            ///<summary>wider than tall</summary>
            e_Orientation_Landscape = 1
        }

        ///<summary>Enumerated values for specifying the scaling of document pages</summary>
        public enum ScaleType
        {
            ///<summary>no scaling</summary>
            e_ScaleType_None = 0,
            ///<summary>fit to the output page</summary>
            e_ScaleType_FitToOutputPage = 1,
            ///<summary>shrink to fit the output page</summary>
            e_ScaleType_ReduceToOutputPage = 2
        }

        ///<summary>Enumerated values for specifying the layout of multiple document pages
        /// onto output pages</summary>
        public enum NUp
        {
            ///<summary>1 document page to 1 output page</summary>
            e_NUp_1_1 = 0,
            ///<summary>2 document pages to 1 output page</summary>
            e_NUp_2_1 = 1,
            ///<summary>2 by 2 document pages to 1 output page</summary>
            e_NUp_2_2 = 2,
            ///<summary>3 by 2 document pages to 1 output page</summary>
            e_NUp_3_2 = 3,
            ///<summary>3 by 3 document pages to 1 output page</summary>
            e_NUp_3_3 = 4,
            ///<summary>4 by 4 document pages to 1 output page</summary>
            e_NUp_4_4 = 5,
        }

        ///<summary>Enumerated values for specifying the ordering of document pages onto 
        /// output pages</summary>
        public enum NUpPageOrder
        {
            ///<summary>left to right then top to bottom</summary>
            e_PageOrder_LeftToRightThenTopToBottom = 0,
            ///<summary>right to left then top to bottom</summary>
            e_PageOrder_RightToLeftThenTopToBottom = 1,
            ///<summary>top to bottom then left to right</summary>
            e_PageOrder_TopToBottomThenLeftToRight = 2,
            ///<summary>bottom to top then left to right</summary>
            e_PageOrder_BottomToTopThenLeftToRight = 3
        }

        ///<summary>Enumerated values for specifying the document content to print</summary>
        public enum PrintContentTypes
        {
            ///<summary>print document only</summary>
            e_PrintContent_DocumentOnly = 0,
            ///<summary>print document and annotations</summary>
            e_PrintContent_DocumentAndAnnotations = 1,
            ///<summary>print document, annotations, and comments</summary>
            e_PrintContent_DocumentAnnotationsAndComments = 2
        }

        ~PrinterMode()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            Destroy();
        }

        public void Destroy()
        {
            mp_printerModeSet = null;
        }
    }
}
#endif