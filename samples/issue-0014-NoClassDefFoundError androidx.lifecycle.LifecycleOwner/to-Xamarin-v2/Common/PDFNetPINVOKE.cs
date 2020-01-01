using System;
using System.Runtime.InteropServices;

using pdftronprivate.trn;
using pdftron.PDF;
using pdftron.SDF;
using pdftron.FDF;

using TRN_PDFDoc = System.IntPtr;
using TRN_Exception = System.IntPtr;
using TRN_Vector = System.IntPtr;
using TRN_Font = System.IntPtr;
using TRN_SDFDoc = System.IntPtr;
using TRN_Filter = System.IntPtr;
using TRN_Bookmark = System.IntPtr;
using TRN_Page = System.IntPtr;
using TRN_Obj = System.IntPtr;
using TRN_FilterReader = System.IntPtr;
using TRN_FilterWriter = System.IntPtr;
using TRN_OCGConfig = System.IntPtr;
using TRN_PDFDocInfo = System.IntPtr;
using TRN_Iterator = System.IntPtr;
using TRN_ItrData = System.IntPtr;
using TRN_DictIterator = System.IntPtr;
using TRN_FDFDoc = System.IntPtr;
using TRN_SecurityHandler = System.IntPtr;
using TRN_SignatureHandler = System.IntPtr;
using TRN_STree = System.IntPtr;
using TRN_PageSet = System.IntPtr;
using TRN_ProgressMonitor = System.IntPtr;
using TRN_FileSpec = System.IntPtr;
using TRN_PDFDocViewPrefs = System.IntPtr;
using TRN_NumberTree = System.IntPtr;
using TRN_NameTree = System.IntPtr;
using TRN_FontCharCodeIterator = System.IntPtr;
using TRN_Annot = System.IntPtr;
using TRN_SignatureWidget = System.IntPtr;
using TRN_ComboBoxWidget = System.IntPtr;
using TRN_ListBoxWidget = System.IntPtr;
using TRN_TextWidget = System.IntPtr;
using TRN_CheckBoxWidget = System.IntPtr;
using TRN_RadioButtonWidget = System.IntPtr;
using TRN_RadioButtonGroup = System.IntPtr;
using TRN_PushButtonWidget = System.IntPtr;
using TRN_Action = System.IntPtr;
using TRN_ActionParameter = System.IntPtr;
using TRN_AnnotBorderStyle = System.IntPtr;
using TRN_Destination = System.IntPtr;
using TRN_PDFDC = System.IntPtr;
using TRN_PDFDCEX = System.IntPtr;
using TRN_ElementBuilder = System.IntPtr;
using TRN_Image = System.IntPtr;
using TRN_Element = System.IntPtr;
using TRN_Shading = System.IntPtr;
using TRN_ClassMap = System.IntPtr;
using TRN_RoleMap = System.IntPtr;
using TRN_AttrObj = System.IntPtr;
using TRN_OCGContext = System.IntPtr;
using TRN_OCG = System.IntPtr;
using TRN_OCMD = System.IntPtr;
using TRN_ColorSpace = System.IntPtr;
using TRN_Function = System.IntPtr;
using TRN_GState = System.IntPtr;
using TRN_ElementReader = System.IntPtr;
using TRN_ElementWriter = System.IntPtr;
using TRN_GDIPlusBitmap = System.IntPtr;
using TRN_PatternColor = System.IntPtr;
using TRN_PDFDraw = System.IntPtr;
using TRN_PDFRasterizer = System.IntPtr;
using TRN_SystemDrawingBitmap = System.IntPtr;
using TRN_PDFView = System.IntPtr;
using TRN_PDFViewSelection = System.IntPtr;
using TRN_PDFViewCtrl = System.IntPtr;
using TRN_TextExtractor = System.IntPtr;
using TRN_Highlights = System.IntPtr;
using TRN_HTML2PDF = System.IntPtr;
using TRN_HTML2PDF_WebPageSettings = System.IntPtr;
using TRN_HTML2PDF_TOCSettings = System.IntPtr;
using TRN_HTML2PDF_Proxy = System.IntPtr;
using TRN_TextSearch = System.IntPtr;
using TRN_ViewChangeCollection = System.IntPtr;
using TRN_ObjSet = System.IntPtr;
using TRN_PDFACompliance = System.IntPtr;
using TRN_ContentReplacer = System.IntPtr;
using TRN_Stamper = System.IntPtr;
using TRN_Flattener = System.IntPtr;
using TRN_UString = System.IntPtr;
using TRN_ColorPt = System.IntPtr;
using TRN_RedactionAppearance = System.IntPtr;
using TRN_Redaction = System.IntPtr;
using TRN_OptimizerImageSettings = System.IntPtr;
using TRN_OptimizerMonoImageSettings = System.IntPtr;
using TRN_OptimizerTextSettings = System.IntPtr;
using TRN_ConversionMonitor = System.IntPtr;
/* {{codegen:PDFNetInternalTools using decl}} */
using TRN_PDFNetInternalTools = System.IntPtr;
/* {{codegen:PDFNetInternalTools using decl}} */
/* {{codegen:WebFontDownloader using decl}} */
using TRN_WebFontDownloader = System.IntPtr;
/* {{codegen:WebFontDownloader using decl}} */
/* {{codegen:DocumentConversion using decl}} */
using TRN_DocumentConversion = System.IntPtr;
/* {{codegen:DocumentConversion using decl}} */
/* {{codegen:PDFDocGenerator using decl}} */
using TRN_PDFDocGenerator = System.IntPtr;
/* {{codegen:PDFDocGenerator using decl}} */
/* {{codegen:UndoManager using decl}} */
using TRN_UndoManager = System.IntPtr;
/* {{codegen:UndoManager using decl}} */
/* {{codegen:DocSnapshot using decl}} */
using TRN_DocSnapshot = System.IntPtr;
/* {{codegen:DocSnapshot using decl}} */
/* {{codegen:ResultSnapshot using decl}} */
using TRN_ResultSnapshot = System.IntPtr;
/* {{codegen:ResultSnapshot using decl}} */
/* {{codegen:GeometryCollection using decl}} */
using TRN_GeometryCollection = System.IntPtr;
/* {{codegen:GeometryCollection using decl}} */
/* {{codegen:CADModule using decl}} */
using TRN_CADModule = System.IntPtr;
/* {{codegen:CADModule using decl}} */
/* {{codegen:OCRModule using decl}} */
using TRN_OCRModule = System.IntPtr;
/* {{codegen:OCRModule using decl}} */
// {{codegen: xamarin_using}}

namespace pdftron
{

    internal class PDFNetPINVOKE
    {
		private const string dllName = PlatformInfo.dllName;

        #region PDFNet

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInitialize(string lic_key);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_PDFNetEnableJavaScript([MarshalAs(UnmanagedType.U1)] bool enable);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_PDFNetIsJavaScriptEnabled([MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetTerminate();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetSetResourcesPath(TRN_UString path, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetGetResourcesPath(ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetAddResourceSearchPath(TRN_UString path);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetSetColorManagement(PDFNet.CMSType t);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetSetDefaultDeviceCMYKProfile(TRN_UString icc_filename);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetSetDefaultDeviceRGBProfile(TRN_UString icc_filename);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetSetDefaultDiskCachingEnabled([MarshalAs(UnmanagedType.U1)] bool use_disk);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetSetDefaultFlateCompressionLevel(int level);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetSetViewerCache(UIntPtr max_cache_size, bool on_disk);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetAddFontSubstFromName(string fontname, TRN_UString fontpath, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetAddFontSubst(PDFNet.CharacterOrdering ordering, TRN_UString fontpath, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetSetTempPath(TRN_UString temp_path);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetSetPersistentCachePath(TRN_UString persistent_path);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetGetVersion(ref double version);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_PDFNetRegisterSecurityHandler(string handler_name, string gui_name, TRN_PDFNetCreateSecurityHandler factory_method);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_PDFNetGetSecHdlrInfoIterator(ref TRN_Iterator result);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_PDFNetSetNumberWriteProc(string param0);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_PDFNetSetNumberReadProc([MarshalAs(UnmanagedType.U1)] bool param0);
        #endregion

        #region Common.Matrix2D
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Matrix2DCopy(ref BasicTypes.TRN_Matrix2D m, ref BasicTypes.TRN_Matrix2D result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Matrix2DSet(ref BasicTypes.TRN_Matrix2D matrix, double a, double b, double c, double d, double h, double v);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Matrix2DConcat (ref BasicTypes.TRN_Matrix2D matrix, double a, double b, double c, double d, double h, double v);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Matrix2DEquals (ref BasicTypes.TRN_Matrix2D m1, ref BasicTypes.TRN_Matrix2D m2, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Matrix2DMult (ref BasicTypes.TRN_Matrix2D matrix, ref double in_out_x, ref double in_out_y);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Matrix2DInverse (ref BasicTypes.TRN_Matrix2D matrix, ref BasicTypes.TRN_Matrix2D result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Matrix2DTranslate (ref BasicTypes.TRN_Matrix2D matrix, double h, double v);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Matrix2DScale (ref BasicTypes.TRN_Matrix2D matrix, double h, double v);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Matrix2DCreateZeroMatrix (ref BasicTypes.TRN_Matrix2D result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Matrix2DCreateIdentityMatrix (ref BasicTypes.TRN_Matrix2D result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Matrix2DCreateRotationMatrix (double angle, ref BasicTypes.TRN_Matrix2D result);

        #endregion

        #region Common.PDFNetException
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRN_GetLineNum(TRN_Exception e);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GetCondExpr(TRN_Exception e);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GetFileName(TRN_Exception e);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GetFunction(TRN_Exception e);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GetMessage(TRN_Exception e);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GetFullMessage(TRN_Exception e);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint TRN_GetErrorCode(TRN_Exception e);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CreateExceptionEx(string cond_expr, string filename, int linenumber, string function, string message, uint errorcode);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CreateException(string cond_expr, string filename, int linenumber, string function, string message);


        #endregion

        #region Common.TRN_Vector
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_VectorGetSize(TRN_Vector vec, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_VectorGetData(TRN_Vector vec, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_VectorGetAt(TRN_Vector vec, IntPtr pos, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_VectorDestroy(TRN_Vector vec);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_VectorDestroyKeepContents(TRN_Vector vec);
        #endregion

        #region Filter.Filter
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCreateASCII85Encode(TRN_Filter input_filter, uint line_width, UIntPtr buf_sz, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCreateFlateEncode(TRN_Filter input_filter, int compression_level, UIntPtr buf_sz, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCreateMappedFileFromUString(TRN_UString filename, ref TRN_Filter result);

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern TRN_Exception TRN_FilterCreateFromStream(ref IntPtr stm, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCreateMemoryFilter(UIntPtr buf_sz, [MarshalAs(UnmanagedType.U1)] bool is_input, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCreateImage2RGBFromElement(TRN_Element elem, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCreateImage2RGBFromObj(TRN_Obj obj, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCreateImage2RGB(TRN_Image img, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCreateImage2RGBAFromElement(TRN_Element elem, [MarshalAs(UnmanagedType.U1)] bool premultiply, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCreateImage2RGBAFromObj(TRN_Obj obj, [MarshalAs(UnmanagedType.U1)] bool premultiply, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCreateImage2RGBA(TRN_Image img, [MarshalAs(UnmanagedType.U1)] bool premultiply, ref TRN_Filter result);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_FilterCreateCustom (TRN_FilterStdFileOpenMode mode, ref IntPtr user_data, TRN_SeekProc _seek, TRN_TellProc _tell, TRN_FlushProc _flush, TRN_ReadProc _read, TRN_WriteProc _write, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterDestroy(TRN_Filter filter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterMappedFileFileSize(TRN_Filter filter, ref UIntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterMappedFileCompare(TRN_Filter mf1, TRN_Filter mf2, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterAttachFilter(TRN_Filter filter, TRN_Filter attach_filter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReleaseAttachedFilter(TRN_Filter filter, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterGetAttachedFilter(TRN_Filter filter, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterGetSourceFilter(TRN_Filter filter, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterGetName(TRN_Filter filter, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterGetDecodeName(TRN_Filter filter, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterBegin(TRN_Filter filter, ref byte result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterSize(TRN_Filter filter, ref UIntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterConsume(TRN_Filter filter, UIntPtr num_bytes);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCount(TRN_Filter filter, ref UIntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterSetCount(TRN_Filter filter, UIntPtr new_count, ref UIntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterSetStreamLength(TRN_Filter filter, UIntPtr bytes);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterFlush(TRN_Filter filter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterFlushAll(TRN_Filter filter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterIsInputFilter(TRN_Filter filter, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCanSeek(TRN_Filter filter, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriteToFile(TRN_Filter filter, TRN_UString path, [MarshalAs(UnmanagedType.U1)] bool append);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterSeek(TRN_Filter filter, IntPtr offset, Filters.Filter.ReferencePos origin);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterTell(TRN_Filter filter, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterCreateInputIterator(TRN_Filter filter, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterGetFilePath(TRN_Filter filter, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterMemoryFilterGetBuffer(TRN_Filter filter, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterMemoryFilterSetAsInputFilter(TRN_Filter filter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PathCompare(TRN_UString p1, TRN_UString p2, ref IntPtr result);
        #endregion

        #region Filter.FilterRead
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReaderCreate(TRN_Filter filter, ref TRN_FilterReader result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReaderDestroy(TRN_FilterReader reader);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReaderAttachFilter(TRN_FilterReader reader, TRN_Filter filter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReaderGetAttachedFilter(TRN_FilterReader reader, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReaderSeek(TRN_FilterReader reader, IntPtr offset, Filters.Filter.ReferencePos origin);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReaderTell(TRN_FilterReader reader, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReaderCount(TRN_FilterReader reader, ref UIntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReaderFlush(TRN_FilterReader reader);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReaderFlushAll(TRN_FilterReader reader);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReaderGet(TRN_FilterReader reader, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReaderPeek(TRN_FilterReader reader, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterReaderRead(TRN_FilterReader reader, IntPtr buf, UIntPtr buf_size, ref UIntPtr result);
        #endregion

        #region Filter.FilterWriter
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterCreate (TRN_Filter filter, ref TRN_FilterWriter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterDestroy (TRN_FilterWriter writer);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterAttachFilter (TRN_FilterWriter writer, TRN_Filter filter);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterGetAttachedFilter (TRN_FilterWriter writer, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterSeek (TRN_FilterWriter writer, IntPtr offset, Filters.Filter.ReferencePos origin);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterTell (TRN_FilterWriter writer, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterCount (TRN_FilterWriter writer, ref UIntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterFlush (TRN_FilterWriter writer);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterFlushAll (TRN_FilterWriter writer);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterWriteUChar (TRN_FilterWriter writer, byte ch);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterWriteInt16 (TRN_FilterWriter writer, Int16 num);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterWriteUInt16 (TRN_FilterWriter writer, UInt16 num);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterWriteInt32 (TRN_FilterWriter writer, int num);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterWriteUInt32 (TRN_FilterWriter writer, uint num);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterWriteInt64 (TRN_FilterWriter writer, Int64 num);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterWriteUInt64 (TRN_FilterWriter writer, UInt64 num);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterWriteString (TRN_FilterWriter writer, string str);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterWriteFilter (TRN_FilterWriter writer, TRN_FilterReader reader);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterWriteLine (TRN_FilterWriter writer, string line, sbyte eol);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FilterWriterWriteBuffer (TRN_FilterWriter writer, IntPtr buf, UIntPtr buf_size, ref UIntPtr result);

        #endregion

        #region FDF.FDFDoc
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocCreate(ref TRN_FDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocCreateFromSDFDoc(TRN_SDFDoc sdfdoc, ref TRN_FDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocCreateFromFilePath(string filepath, ref TRN_FDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocCreateFromUFilePath(TRN_UString filepath, ref TRN_FDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocCreateFromStream(TRN_Filter stream, ref TRN_FDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocCreateFromMemoryBuffer(IntPtr buf, UIntPtr buf_size, ref TRN_FDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocDestroy(TRN_FDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocIsModified(TRN_FDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocSave(TRN_FDFDoc doc, TRN_UString path);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocSaveMemoryBuffer(TRN_FDFDoc doc, ref IntPtr out_buf, ref UIntPtr out_buf_size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocGetTrailer(TRN_FDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocGetRoot(TRN_FDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocGetFDF(TRN_FDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocGetPDFFileName(TRN_FDFDoc doc, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocSetPDFFileName(TRN_FDFDoc doc, TRN_UString filepath);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocGetID(TRN_FDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocSetID(TRN_FDFDoc doc, TRN_Obj id);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocGetFieldIteratorBegin(TRN_FDFDoc doc, ref TRN_Iterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocGetFieldIterator(TRN_FDFDoc doc, TRN_UString field_name, ref TRN_Iterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocGetField(TRN_FDFDoc doc, TRN_UString field_name, ref BasicTypes.TRN_FDFField result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocFieldCreate(TRN_FDFDoc doc, TRN_UString field_name, Field.Type type, TRN_Obj field_value, ref BasicTypes.TRN_FDFField result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocFieldCreateFromString(TRN_FDFDoc doc, TRN_UString field_name, Field.Type type, TRN_UString field_value, ref BasicTypes.TRN_FDFField result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocGetSDFDoc(TRN_FDFDoc doc, ref TRN_SDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocCreateFromXFDF(TRN_UString file_name, ref TRN_FDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocSaveAsXFDF(TRN_FDFDoc doc, TRN_UString file_name);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocSaveAsXFDFAsString(TRN_FDFDoc doc, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFDocMergeAnnots(TRN_FDFDoc doc, TRN_UString command_file, TRN_UString permitted_user);

        #endregion

        #region FDF.FDFField
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFFieldCreate(TRN_Obj field_dict, TRN_Obj fdf_dict, ref BasicTypes.TRN_FDFField result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFFieldAssign(ref BasicTypes.TRN_FDFField left, ref BasicTypes.TRN_FDFField right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFFieldGetValue(ref BasicTypes.TRN_FDFField field, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFFieldSetValue(ref BasicTypes.TRN_FDFField field, TRN_Obj value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFFieldGetName(ref BasicTypes.TRN_FDFField field, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFFieldGetPartialName(ref BasicTypes.TRN_FDFField field, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFFieldGetSDFObj(ref BasicTypes.TRN_FDFField field, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FDFFieldFindAttribute(ref BasicTypes.TRN_FDFField field, string attrib, ref TRN_Obj result);

        #endregion

        #region PDF.Action
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCreateGoto(TRN_Destination dest, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCreateGotoWithKey(BasicTypes.TRN_String key, TRN_Destination dest, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCreateGotoRemote(TRN_FileSpec file, int page_num, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCreateGotoRemoteSetNewWindow(TRN_FileSpec file, int page_num, [MarshalAs(UnmanagedType.U1)] bool new_window, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCreateURI(TRN_SDFDoc sdfdoc, string uri, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCreateSubmitForm(TRN_FileSpec url, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCreateLaunch(TRN_SDFDoc sdfdoc, string path, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCreateHideField(TRN_SDFDoc sdfdoc, int list_length, [MarshalAsAttribute(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] field_list, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCreateImportData(TRN_SDFDoc sdfdoc, string path, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCreateResetForm(TRN_SDFDoc sdfdoc, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCreateJavaScript(TRN_SDFDoc sdfdoc, string script, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCreate(TRN_Obj in_obj, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCopy(TRN_Action in_action, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionCompare(TRN_Action action, TRN_Action in_action, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionIsValid(TRN_Action action, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionGetType(TRN_Action action, ref pdftron.PDF.Action.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionGetDest(TRN_Action action, ref TRN_Destination result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionGetNext(TRN_Action action, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ActionGetSDFObj(TRN_Action action, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Action_GetFormActionFlag(TRN_Action action, pdftron.PDF.Action.FormActionFlag flag, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Action_SetFormActionFlag(TRN_Action action, pdftron.PDF.Action.FormActionFlag flag, [MarshalAs(UnmanagedType.U1)] bool value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Action_Execute(TRN_Action action);
        #endregion

		#region PDF.ActionParameter
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ActionParameterCreate(TRN_Action action, ref TRN_ActionParameter result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ActionParameterCreateWithField(TRN_Action action, ref BasicTypes.TRN_Field field, ref TRN_ActionParameter result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ActionParameterCreateWithAnnot(TRN_Action action, TRN_Annot annot, ref TRN_ActionParameter result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ActionParameterCreateWithPage(TRN_Action action, TRN_Page page, ref TRN_ActionParameter result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ActionParameterDestroy(TRN_ActionParameter ap);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ActionParameterGetAction(TRN_ActionParameter ap, ref TRN_Action result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ActionParameterAssign(TRN_ActionParameter left, TRN_ActionParameter right);
		#endregion

        #region PDF.Annot
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotCreate(TRN_SDFDoc doc, Annot.Type type, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotCopy(TRN_Annot d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotCompare(TRN_Annot annot, TRN_Annot d, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotIsValid(TRN_Annot annot, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetSDFObj(TRN_Annot annot, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetType(TRN_Annot annot, ref Annot.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotIsMarkup(TRN_Annot annot, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetRect(TRN_Annot annot, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetVisibleContentBox(TRN_Annot annot, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetRect(TRN_Annot annot, ref BasicTypes.TRN_Rect pos);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotResize(TRN_Annot annot, ref BasicTypes.TRN_Rect rect);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_AnnotGetRotation(TRN_Annot annot, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_AnnotSetRotation(TRN_Annot annot, int rotation);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetContents(TRN_Annot annot, TRN_UString cont);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetContents(TRN_Annot annot, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetCustomData(TRN_Annot annot, TRN_UString key, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetCustomData(TRN_Annot annot, TRN_UString key, TRN_UString value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotDeleteCustomData(TRN_Annot annot, TRN_UString key);


        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetPage(TRN_Annot annot, ref TRN_Page result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetPage(TRN_Annot annot, TRN_Page pg);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetUniqueID(TRN_Annot annot, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetUniqueID(TRN_Annot annot, string id, int id_buf_sz);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetDate(TRN_Annot annot, ref BasicTypes.TRN_Date result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetDate(TRN_Annot annot, ref BasicTypes.TRN_Date date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetFlag(TRN_Annot annot, Annot.Flag flag, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetFlag(TRN_Annot annot, Annot.Flag flag, [MarshalAs(UnmanagedType.U1)] bool value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleCreate(Annot.BorderStyle.Style s, double b_width, double b_hr, double b_vr, ref TRN_AnnotBorderStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleCreateWithDashPattern(Annot.BorderStyle.Style s, double b_width, double b_hr, double b_vr, double[] buffer, int buf_length, ref TRN_AnnotBorderStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleCopy(TRN_AnnotBorderStyle bs, ref TRN_AnnotBorderStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleGetStyle(TRN_AnnotBorderStyle bs, ref Annot.BorderStyle.Style result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleSetStyle(TRN_AnnotBorderStyle bs, Annot.BorderStyle.Style style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleDestroy(TRN_AnnotBorderStyle bs);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetAppearance(TRN_Annot annot, Annot.AnnotationState annot_state, string app_state, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetAppearance(TRN_Annot annot, TRN_Obj app_stream, Annot.AnnotationState annot_state, string app_state);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotRemoveAppearance(TRN_Annot annot, Annot.AnnotationState annot_state, string app_state);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotFlatten(TRN_Annot annot, TRN_Page page);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetActiveAppearanceState(TRN_Annot annot, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetActiveAppearanceState(TRN_Annot annot, string astate);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetColor(TRN_Annot annot, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetColorAsRGB(TRN_Annot annot, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetColorAsCMYK(TRN_Annot annot, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetColorAsGray(TRN_Annot annot, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetColorCompNum(TRN_Annot annot, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetColorDefault(TRN_Annot annot, TRN_ColorPt col);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetColor(TRN_Annot annot, TRN_ColorPt col, int numcomp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetStructParent(TRN_Annot annot, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetStructParent(TRN_Annot annot, int pakeyval);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetOptionalContent(TRN_Annot annot, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetOptionalContent(TRN_Annot annot, TRN_Obj content);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotRefreshAppearance(TRN_Annot annot);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleGetWidth(TRN_AnnotBorderStyle bs, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleSetWidth(TRN_AnnotBorderStyle bs, double width);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleGetHR(TRN_AnnotBorderStyle bs, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleSetHR(TRN_AnnotBorderStyle bs, double horizontal_radius);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleGetVR(TRN_AnnotBorderStyle bs, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleSetVR(TRN_AnnotBorderStyle bs, double vertical_radius);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleGetDashPattern(TRN_AnnotBorderStyle bs, ref IntPtr out_buf, ref int buf_length);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleSetDashPattern(TRN_AnnotBorderStyle bs, IntPtr buffer, int buf_length);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetBorderStyle(TRN_Annot annot, ref TRN_AnnotBorderStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetBorderStyle(TRN_Annot annot, TRN_AnnotBorderStyle bs, [MarshalAs(UnmanagedType.U1)] bool oldStyleOnly);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotGetBorderStyleStyle(TRN_AnnotBorderStyle bs, ref Annot.BorderStyle.Style result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotSetBorderStyleStyle(TRN_AnnotBorderStyle bs, Annot.BorderStyle.Style bst);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleAssign(TRN_AnnotBorderStyle from, TRN_AnnotBorderStyle to);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_AnnotBorderStyleCompare(TRN_AnnotBorderStyle a, TRN_AnnotBorderStyle b, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CaretAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CaretAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CaretAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CaretAnnotGetSymbol(TRN_Annot caret, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CaretAnnotSetSymbol(TRN_Annot caret, string content);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotGetStartPoint(TRN_Annot line, ref BasicTypes.TRN_Point result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotSetStartPoint(TRN_Annot line, ref BasicTypes.TRN_Point stp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotGetEndPoint(TRN_Annot line, ref BasicTypes.TRN_Point result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotSetEndPoint(TRN_Annot line, ref BasicTypes.TRN_Point etp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotGetStartStyle(TRN_Annot line, ref PDF.Annots.Line.EndingStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotSetStartStyle(TRN_Annot line, PDF.Annots.Line.EndingStyle ss);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotGetEndStyle(TRN_Annot line, ref PDF.Annots.Line.EndingStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotSetEndStyle(TRN_Annot line, PDF.Annots.Line.EndingStyle es);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotGetLeaderLineLength(TRN_Annot line, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotSetLeaderLineLength(TRN_Annot line, double length);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotGetLeaderLineExtensionLength(TRN_Annot line, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotSetLeaderLineExtensionLength(TRN_Annot line, double length);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotGetShowCaption(TRN_Annot line, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotSetShowCaption(TRN_Annot line, [MarshalAs(UnmanagedType.U1)] bool sorn);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotGetIntentType(TRN_Annot line, ref PDF.Annots.Line.IntentType result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotSetIntentType(TRN_Annot line, PDF.Annots.Line.IntentType it);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotGetCapPos(TRN_Annot line, ref PDF.Annots.Line.CapPos result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotSetCapPos(TRN_Annot line, PDF.Annots.Line.CapPos it);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotGetLeaderLineOffset(TRN_Annot line, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotSetLeaderLineOffset(TRN_Annot line, double length);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotGetTextHOffset(TRN_Annot line, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotSetTextHOffset(TRN_Annot line, double offset);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotGetTextVOffset(TRN_Annot line, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LineAnnotSetTextVOffset(TRN_Annot line, double offset);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CircleAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CircleAnnotCreateFromAnnot(TRN_Annot circle, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CircleAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CircleAnnotGetInteriorColor(TRN_Annot circle, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CircleAnnotGetInteriorColorCompNum(TRN_Annot circle, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CircleAnnotSetInteriorColorDefault(TRN_Annot circle, TRN_ColorPt col);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CircleAnnotSetInteriorColor(TRN_Annot circle, TRN_ColorPt col, int numcomp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CircleAnnotGetContentRect(TRN_Annot circle, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CircleAnnotSetContentRect(TRN_Annot circle, ref BasicTypes.TRN_Rect cr);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CircleAnnotGetPadding(TRN_Annot circle, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CircleAnnotSetPadding(TRN_Annot circle, ref BasicTypes.TRN_Rect cr);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotExport(TRN_Annot fileatt, TRN_UString save_as, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotCreateFromAnnot(TRN_Annot fileatt, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotCreateWithFileSpec(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, TRN_FileSpec fs, PDF.Annots.FileAttachment.Icon ic, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotCreateWithIcon(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, TRN_UString path, PDF.Annots.FileAttachment.Icon ic, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, TRN_UString path, string iconname, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotCreateDefault(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, TRN_UString path, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotGetFileSpec(TRN_Annot fileatt, ref TRN_FileSpec result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotSetFileSpec(TRN_Annot fileatt, TRN_FileSpec fspec);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotGetIcon(TRN_Annot fileatt, ref PDF.Annots.FileAttachment.Icon result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotSetIcon(TRN_Annot fileatt, PDF.Annots.FileAttachment.Icon icon);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotGetIconName(TRN_Annot fileatt, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileAttachmentAnnotSetIconName(TRN_Annot fileatt, string iname);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotGetDefaultAppearance(TRN_Annot ft, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotSetDefaultAppearance(TRN_Annot ft, string defApp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotGetQuaddingFormat(TRN_Annot ft, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotSetQuaddingFormat(TRN_Annot ft, int format);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotGetCalloutLinePoints(TRN_Annot ft, ref BasicTypes.TRN_Point p1, ref BasicTypes.TRN_Point p2, ref BasicTypes.TRN_Point p3);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotSetCalloutLinePoints(TRN_Annot ft, ref BasicTypes.TRN_Point p1, ref BasicTypes.TRN_Point p2, ref BasicTypes.TRN_Point p3);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotSetCalloutLinePointsTwo(TRN_Annot ft, ref BasicTypes.TRN_Point p1, ref BasicTypes.TRN_Point p2);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotGetIntentName(TRN_Annot ft, ref PDF.Annots.FreeText.IntentName result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotSetIntentName(TRN_Annot ft, PDF.Annots.FreeText.IntentName mode);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotSetIntentNameDefault(TRN_Annot ft);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotGetEndingStyle(TRN_Annot ft, ref PDF.Annots.Line.EndingStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotSetEndingStyle(TRN_Annot ft, PDF.Annots.Line.EndingStyle style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotSetEndingStyleName(TRN_Annot ft, string est);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotSetTextColor(TRN_Annot ft, TRN_ColorPt color, int col_components);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotGetTextColor(TRN_Annot ft, TRN_ColorPt out_color, ref int col_components);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotSetLineColor(TRN_Annot ft, TRN_ColorPt color, int col_components);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotGetLineColor(TRN_Annot ft, TRN_ColorPt out_color, ref int col_components);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotSetFontSize(TRN_Annot ft, double font_size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FreeTextAnnotGetFontSize(TRN_Annot ft, ref double result);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_FreeTextAnnotSetFont(TRN_Annot ft, TRN_Font font);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_FreeTextAnnotGetFont(TRN_Annot ft, ref TRN_Font result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_InkAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_InkAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_InkAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_InkAnnotGetPathCount(TRN_Annot ink, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_InkAnnotGetPointCount(TRN_Annot ink, int pathindex, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_InkAnnotGetPoint(TRN_Annot ink, uint pathindex, uint pointindex, ref BasicTypes.TRN_Point result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_InkAnnotSetPoint(TRN_Annot ink, uint pathindex, uint pointindex, ref BasicTypes.TRN_Point point);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_InkAnnotErase(TRN_Annot ink, ref BasicTypes.TRN_Point pt1, ref BasicTypes.TRN_Point pt2, double eraserHalfWidth, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LinkAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LinkAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LinkAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LinkAnnotRemoveAction(TRN_Annot link);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LinkAnnotGetAction(TRN_Annot link, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LinkAnnotSetAction(TRN_Annot link, TRN_Action action);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LinkAnnotGetHighlightingMode(TRN_Annot link, ref PDF.Annots.Link.HighlightingMode result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LinkAnnotSetHighlightingMode(TRN_Annot link, PDF.Annots.Link.HighlightingMode value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LinkAnnotGetQuadPointCount(TRN_Annot link, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LinkAnnotGetQuadPoint(TRN_Annot link, int idx, ref BasicTypes.TRN_QuadPoint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_LinkAnnotSetQuadPoint(TRN_Annot link, int idx, ref BasicTypes.TRN_QuadPoint qp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotGetTitle(TRN_Annot markup, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotSetTitle(TRN_Annot markup, string title);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotSetTitleUString(TRN_Annot markup, TRN_UString title);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotGetPopup(TRN_Annot markup, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotSetPopup(TRN_Annot markup, TRN_Annot ppup);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotGetOpacity(TRN_Annot markup, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotSetOpacity(TRN_Annot markup, double op);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotGetSubject(TRN_Annot markup, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotSetSubject(TRN_Annot markup, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotGetCreationDates(TRN_Annot markup, ref BasicTypes.TRN_Date result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotGetBorderEffect(TRN_Annot markup, ref PDF.Annots.Markup.BorderEffect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotSetBorderEffect(TRN_Annot markup, PDF.Annots.Markup.BorderEffect effect);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotGetBorderEffectIntensity(TRN_Annot markup, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotSetBorderEffectIntensity(TRN_Annot markup, double intensity);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotSetCreationDates(TRN_Annot markup, ref BasicTypes.TRN_Date date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotGetInteriorColor(TRN_Annot markup, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotGetInteriorColorCompNum(TRN_Annot markup, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotSetInteriorColorRGB(TRN_Annot markup, TRN_ColorPt col);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotSetInteriorColor(TRN_Annot markup, TRN_ColorPt col, int numcomp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotGetContentRect(TRN_Annot markup, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotSetContentRect(TRN_Annot markup, ref BasicTypes.TRN_Rect cr);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotGetPadding(TRN_Annot markup, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotSetPadding(TRN_Annot markup, ref BasicTypes.TRN_Rect cr);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MarkupAnnotRotateAppearance(TRN_Annot markup, Double angle);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MovieAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MovieAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MovieAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MovieAnnotGetTitle(TRN_Annot movie, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MovieAnnotSetTitle(TRN_Annot movie, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MovieAnnotIsToBePlayed(TRN_Annot movie, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_MovieAnnotSetToBePlayed(TRN_Annot movie, [MarshalAs(UnmanagedType.U1)] bool playono);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolyLineAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolyLineAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolyLineAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolyLineAnnotGetVertexCount(TRN_Annot polyline, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolyLineAnnotGetVertex(TRN_Annot polyline, int idx, ref BasicTypes.TRN_Point result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolyLineAnnotSetVertex(TRN_Annot polyline, int idx, ref BasicTypes.TRN_Point pt);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolyLineAnnotGetStartStyle(TRN_Annot polyline, ref PDF.Annots.Line.EndingStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolyLineAnnotSetStartStyle(TRN_Annot polyline, PDF.Annots.Line.EndingStyle style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolyLineAnnotGetEndStyle(TRN_Annot polyline, ref PDF.Annots.Line.EndingStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolyLineAnnotSetEndStyle(TRN_Annot polyline, PDF.Annots.Line.EndingStyle style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolyLineAnnotGetIntentName(TRN_Annot polyline, ref PDF.Annots.PolyLine.IntentType result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolyLineAnnotSetIntentName(TRN_Annot polyline, PDF.Annots.PolyLine.IntentType style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolygonAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolygonAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PolygonAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PopupAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PopupAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PopupAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PopupAnnotGetParent(TRN_Annot popup, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PopupAnnotSetParent(TRN_Annot popup, TRN_Annot parent);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PopupAnnotIsOpen(TRN_Annot popup, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PopupAnnotSetOpen(TRN_Annot popup, [MarshalAs(UnmanagedType.U1)] bool closeono);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotGetQuadPointCount(TRN_Annot redaction, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotGetQuadPoint(TRN_Annot redaction, int idx, ref BasicTypes.TRN_QuadPoint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotSetQuadPoint(TRN_Annot redaction, int idx, ref BasicTypes.TRN_QuadPoint qp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotSetAppFormXO(TRN_Annot redaction, TRN_Obj formxo);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotGetOverlayText(TRN_Annot redaction, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotSetOverlayText(TRN_Annot redaction, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotGetUseRepeat(TRN_Annot redaction, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotSetUseRepeat(TRN_Annot redaction, [MarshalAs(UnmanagedType.U1)] bool closeono);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotGetOverlayTextAppearance(TRN_Annot redaction, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotSetOverlayTextAppearance(TRN_Annot redaction, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotGetQuadForm(TRN_Annot redaction, ref PDF.Annots.Redaction.QuadForm result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotSetQuadForm(TRN_Annot redaction, PDF.Annots.Redaction.QuadForm style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RedactionAnnotGetAppFormXO(TRN_Annot redaction, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RubberStampAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RubberStampAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RubberStampAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RubberStampAnnotGetIcon(TRN_Annot stamp, ref PDF.Annots.RubberStamp.Icon result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RubberStampAnnotSetIcon(TRN_Annot stamp, PDF.Annots.RubberStamp.Icon style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RubberStampAnnotSetIconDefault(TRN_Annot stamp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RubberStampAnnotGetIconName(TRN_Annot stamp, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RubberStampAnnotSetIconName(TRN_Annot stamp, string style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetTitle(TRN_Annot s, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetTitle(TRN_Annot s, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetAction(TRN_Annot s, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetAction(TRN_Annot s, TRN_Action action);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetBorderColor(TRN_Annot s, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetBorderColor(TRN_Annot s, TRN_ColorPt col, int numcomp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetBorderColorCompNum(TRN_Annot s, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetBackgroundColorCompNum(TRN_Annot s, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetBackgroundColor(TRN_Annot s, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetBackgroundColor(TRN_Annot s, TRN_ColorPt col, int numcomp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetStaticCaptionText(TRN_Annot s, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetStaticCaptionText(TRN_Annot s, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetRolloverCaptionText(TRN_Annot s, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetRolloverCaptionText(TRN_Annot s, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetMouseDownCaptionText(TRN_Annot s, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetMouseDownCaptionText(TRN_Annot s, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetStaticIcon(TRN_Annot s, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetStaticIcon(TRN_Annot s, TRN_Obj icon);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetRolloverIcon(TRN_Annot s, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetRolloverIcon(TRN_Annot s, TRN_Obj icon);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetMouseDownIcon(TRN_Annot s, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetMouseDownIcon(TRN_Annot s, TRN_Obj icon);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetScaleType(TRN_Annot s, ref PDF.Annots.Screen.ScaleType result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetScaleType(TRN_Annot s, PDF.Annots.Screen.ScaleType style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetIconCaptionRelation(TRN_Annot s, ref PDF.Annots.Screen.IconCaptionRelation result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetIconCaptionRelation(TRN_Annot s, PDF.Annots.Screen.IconCaptionRelation style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetScaleCondition(TRN_Annot s, ref PDF.Annots.Screen.ScaleCondition result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetScaleCondition(TRN_Annot s, PDF.Annots.Screen.ScaleCondition style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetFitFull(TRN_Annot s, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetFitFull(TRN_Annot s, [MarshalAs(UnmanagedType.U1)] bool ff);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetHIconLeftOver(TRN_Annot s, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetHIconLeftOver(TRN_Annot s, double hl);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotGetVIconLeftOver(TRN_Annot s, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ScreenAnnotSetVIconLeftOver(TRN_Annot s, double vl);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SoundAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SoundAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SoundAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SoundAnnotCreateAtPoint(TRN_SDFDoc doc, ref BasicTypes.TRN_Point pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SoundAnnotGetSoundStream(TRN_Annot sound, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SoundAnnotSetSoundStream(TRN_Annot sound, TRN_Obj icon);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SoundAnnotGetIcon(TRN_Annot sound, ref PDF.Annots.Sound.Icon result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SoundAnnotSetIcon(TRN_Annot sound, PDF.Annots.Sound.Icon style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SoundAnnotGetIconName(TRN_Annot sound, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SoundAnnotSetIconName(TRN_Annot sound, string style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquareAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquareAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquareAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquareAnnotGetInteriorColor(TRN_Annot square, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquareAnnotGetInteriorColorCompNum(TRN_Annot square, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquareAnnotSetInteriorColorDefault(TRN_Annot square, TRN_ColorPt col);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquareAnnotSetInteriorColor(TRN_Annot square, TRN_ColorPt col, int numcomp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquareAnnotGetContentRect(TRN_Annot square, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquareAnnotSetContentRect(TRN_Annot square, ref BasicTypes.TRN_Rect cr);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquareAnnotGetPadding(TRN_Annot square, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquareAnnotSetPadding(TRN_Annot square, ref BasicTypes.TRN_Rect cr);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquigglyAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquigglyAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SquigglyAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StrikeOutAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StrikeOutAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StrikeOutAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotCreateAtPoint(TRN_SDFDoc doc, ref BasicTypes.TRN_Point pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotIsOpen(TRN_Annot text, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotSetOpen(TRN_Annot text, [MarshalAs(UnmanagedType.U1)] bool closeono);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotGetIcon(TRN_Annot text, ref PDF.Annots.Text.Icon result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotSetIcon(TRN_Annot text, PDF.Annots.Text.Icon icon);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotSetIconDefault(TRN_Annot text);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotGetIconName(TRN_Annot text, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotSetIconName(TRN_Annot text, string style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotGetState(TRN_Annot text, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotSetState(TRN_Annot text, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotGetStateModel(TRN_Annot text, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextAnnotSetStateModel(TRN_Annot text, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UnderlineAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UnderlineAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UnderlineAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WatermarkAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WatermarkAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WatermarkAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextMarkupAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextMarkupAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextMarkupAnnotGetQuadPointCount(TRN_Annot textmarkup, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextMarkupAnnotGetQuadPoint(TRN_Annot textmarkup, int idx, ref BasicTypes.TRN_QuadPoint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextMarkupAnnotSetQuadPoint(TRN_Annot textmarkup, int idx, ref BasicTypes.TRN_QuadPoint qp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotCreate(TRN_SDFDoc doc, ref BasicTypes.TRN_Rect pos, ref BasicTypes.TRN_Field fd, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotCreateFromObj(TRN_Obj d, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotCreateFromAnnot(TRN_Annot annot, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetField(TRN_Annot widget, ref BasicTypes.TRN_Field result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetHighlightingMode(TRN_Annot widget, ref PDF.Annots.Widget.HighlightingMode result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetHighlightingMode(TRN_Annot widget, PDF.Annots.Widget.HighlightingMode value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetAction(TRN_Annot widget, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetAction(TRN_Annot widget, TRN_Action action);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetBorderColor(TRN_Annot widget, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetBorderColor(TRN_Annot widget, TRN_ColorPt col, int numcomp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetBorderColorCompNum(TRN_Annot widget, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetBackgroundColorCompNum(TRN_Annot widget, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetBackgroundColor(TRN_Annot widget, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetBackgroundColor(TRN_Annot widget, TRN_ColorPt col, int numcomp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetStaticCaptionText(TRN_Annot widget, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetStaticCaptionText(TRN_Annot widget, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetRolloverCaptionText(TRN_Annot widget, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetRolloverCaptionText(TRN_Annot widget, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetMouseDownCaptionText(TRN_Annot widget, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetMouseDownCaptionText(TRN_Annot widget, TRN_UString contents);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetStaticIcon(TRN_Annot widget, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetStaticIcon(TRN_Annot widget, TRN_Obj icon);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetRolloverIcon(TRN_Annot widget, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetRolloverIcon(TRN_Annot widget, TRN_Obj icon);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetMouseDownIcon(TRN_Annot widget, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetMouseDownIcon(TRN_Annot widget, TRN_Obj icon);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetScaleType(TRN_Annot widget, ref PDF.Annots.Widget.ScaleType result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetScaleType(TRN_Annot widget, PDF.Annots.Widget.ScaleType style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetIconCaptionRelation(TRN_Annot widget, ref PDF.Annots.Widget.IconCaptionRelation result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetIconCaptionRelation(TRN_Annot widget, PDF.Annots.Widget.IconCaptionRelation style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetScaleCondition(TRN_Annot widget, ref PDF.Annots.Widget.ScaleCondition result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetScaleCondition(TRN_Annot widget, PDF.Annots.Widget.ScaleCondition style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetFitFull(TRN_Annot widget, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetFitFull(TRN_Annot widget, [MarshalAs(UnmanagedType.U1)] bool ff);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetFontSize(TRN_Annot widget, double font_size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetTextColor(TRN_Annot widget, TRN_ColorPt col, int col_comp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetFont(TRN_Annot widget, TRN_Font font);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetFontSize(TRN_Annot widget, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetTextColor(TRN_Annot widget, TRN_ColorPt col, ref int col_comp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetFont(TRN_Annot widget, ref TRN_Font result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetHIconLeftOver(TRN_Annot widget, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetHIconLeftOver(TRN_Annot widget, double hl);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotGetVIconLeftOver(TRN_Annot widget, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WidgetAnnotSetVIconLeftOver(TRN_Annot widget, double vl);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureWidgetCreate(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, TRN_UString field_name, ref TRN_SignatureWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureWidgetCreateWithField(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, ref BasicTypes.TRN_Field field, ref TRN_SignatureWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureWidgetCreateWithDigitalSignatureField(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, ref BasicTypes.TRN_DigitalSignatureField field, ref TRN_SignatureWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureWidgetCreateFromObj(TRN_Obj obj, ref TRN_SignatureWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureWidgetCreateFromAnnot(TRN_Annot annot, ref TRN_SignatureWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureWidgetCreateSignatureAppearance(TRN_SignatureWidget self, TRN_Image img);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureWidgetGetDigitalSignatureField(TRN_SignatureWidget self, ref BasicTypes.TRN_DigitalSignatureField result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ComboBoxWidgetCreate(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, TRN_UString field_name, ref TRN_ComboBoxWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ComboBoxWidgetCreateWithField(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, ref BasicTypes.TRN_Field field, ref TRN_ComboBoxWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ComboBoxWidgetCreateFromObj(TRN_Obj obj, ref TRN_ComboBoxWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ComboBoxWidgetCreateFromAnnot(TRN_Annot annot, ref TRN_ComboBoxWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ComboBoxWidgetAddOption(TRN_ComboBoxWidget combobox, TRN_UString value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ComboBoxWidgetAddOptions(TRN_ComboBoxWidget combobox, IntPtr options, IntPtr options_size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ComboBoxWidgetSetSelectedOption(TRN_ComboBoxWidget combobox, TRN_UString value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ComboBoxWidgetGetSelectedOption(TRN_ComboBoxWidget combobox, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ListBoxWidgetCreate(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, TRN_UString field_name, ref TRN_ListBoxWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ListBoxWidgetCreateWithField(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, ref BasicTypes.TRN_Field field, ref TRN_ListBoxWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ListBoxWidgetCreateFromObj(TRN_Obj obj, ref TRN_ListBoxWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ListBoxWidgetCreateFromAnnot(TRN_Annot annot, ref TRN_ListBoxWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ListBoxWidgetAddOption(TRN_ListBoxWidget listbox, TRN_UString value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ListBoxWidgetAddOptions(TRN_ListBoxWidget listbox, IntPtr options, IntPtr options_size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ListBoxWidgetSetSelectedOptions(TRN_ListBoxWidget listbox, IntPtr options, IntPtr options_size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ListBoxWidgetGetSelectedOptions(TRN_ListBoxWidget listbox, ref TRN_Vector result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextWidgetCreate(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, TRN_UString field_name, ref TRN_TextWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextWidgetCreateWithField(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, ref BasicTypes.TRN_Field field, ref TRN_TextWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextWidgetCreateFromObj(TRN_Obj obj, ref TRN_TextWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextWidgetCreateFromAnnot(TRN_Annot annot, ref TRN_TextWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextWidgetSetText(TRN_TextWidget widget, TRN_UString text);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextWidgetGetText(TRN_TextWidget widget, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CheckBoxWidgetCreate(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, TRN_UString field_name, ref TRN_CheckBoxWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CheckBoxWidgetCreateWithField(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, ref BasicTypes.TRN_Field field, ref TRN_CheckBoxWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CheckBoxWidgetCreateFromObj(TRN_Obj obj, ref TRN_CheckBoxWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CheckBoxWidgetCreateFromAnnot(TRN_Annot annot, ref TRN_CheckBoxWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CheckBoxWidgetIsChecked(TRN_CheckBoxWidget button, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CheckBoxWidgetSetChecked(TRN_CheckBoxWidget button, [MarshalAs(UnmanagedType.U1)] bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonWidgetCreateFromObj(TRN_Obj obj, ref TRN_RadioButtonWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonWidgetCreateFromAnnot(TRN_Annot annot, ref TRN_RadioButtonWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonWidgetIsEnabled(TRN_RadioButtonWidget button, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonWidgetEnableButton(TRN_RadioButtonWidget button);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonWidgetGetGroup(TRN_RadioButtonWidget button, ref TRN_RadioButtonGroup result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PushButtonWidgetCreate(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, TRN_UString field_name, ref TRN_PushButtonWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PushButtonWidgetCreateWithField(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect pos, ref BasicTypes.TRN_Field field, ref TRN_PushButtonWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PushButtonWidgetCreateFromObj(TRN_Obj obj, ref TRN_PushButtonWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PushButtonWidgetCreateFromAnnot(TRN_Annot annot, ref TRN_PushButtonWidget result);

        #endregion

        #region PDF.Annots.RadioButtonGroup

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonGroupCreateFromField(ref BasicTypes.TRN_Field field, ref TRN_RadioButtonGroup result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonGroupCreate(TRN_PDFDoc doc, TRN_UString field_name, ref TRN_RadioButtonGroup result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonGroupDestroy(TRN_RadioButtonGroup group);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonGroupAdd(TRN_RadioButtonGroup group, ref BasicTypes.TRN_Rect pos, string onstate, ref TRN_RadioButtonWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonGroupGetNumButtons(TRN_RadioButtonGroup group, ref UIntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonGroupGetButton(TRN_RadioButtonGroup group, UIntPtr index, ref TRN_RadioButtonWidget result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonGroupGetField(TRN_RadioButtonGroup group, ref BasicTypes.TRN_Field result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RadioButtonGroupAddGroupButtonsToPage(TRN_RadioButtonGroup group, TRN_Page page);

        #endregion

        #region PDF.Bookmark
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkCreate(TRN_PDFDoc in_doc, TRN_UString in_title, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkCreateFromObj(TRN_Obj in_bookmark_dict, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkCopy(TRN_Bookmark in_bookmark, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkCompare(TRN_Bookmark bm, TRN_Bookmark in_bookmark, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkIsValid(TRN_Bookmark bm, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkHasChildren(TRN_Bookmark bm, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetNext(TRN_Bookmark bm, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetPrev(TRN_Bookmark bm, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetFirstChild(TRN_Bookmark bm, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetLastChild(TRN_Bookmark bm, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetParent(TRN_Bookmark bm, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkFind(TRN_Bookmark bm, TRN_UString in_title, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkAddNewChild(TRN_Bookmark bm, TRN_UString in_title, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkAddChild(TRN_Bookmark bm, TRN_Bookmark in_bookmark);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkAddNewNext(TRN_Bookmark bm, TRN_UString in_title, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkAddNext(TRN_Bookmark bm, TRN_Bookmark in_bookmark);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkAddNewPrev(TRN_Bookmark bm, TRN_UString in_title, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkAddPrev(TRN_Bookmark bm, TRN_Bookmark in_bookmark);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkDelete(TRN_Bookmark bm);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkUnlink(TRN_Bookmark bm);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetIndent(TRN_Bookmark bm, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkIsOpen(TRN_Bookmark bm, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkSetOpen(TRN_Bookmark bm, [MarshalAs(UnmanagedType.U1)] bool in_open);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetOpenCount(TRN_Bookmark bm, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetTitle(TRN_Bookmark bm, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetTitleObj(TRN_Bookmark bm, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkSetTitle(TRN_Bookmark bm, TRN_UString title);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetAction(TRN_Bookmark bm, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkSetAction(TRN_Bookmark bm, TRN_Action in_action);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkRemoveAction(TRN_Bookmark bm);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetFlags(TRN_Bookmark bm, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkSetFlags(TRN_Bookmark bm, int in_flags);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetColor(TRN_Bookmark bm, ref double out_r, ref double out_g, ref double out_b);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkSetColor(TRN_Bookmark bm, double in_r, double in_g, double in_b);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_BookmarkGetSDFObj(TRN_Bookmark bm, ref TRN_Obj result);

        #endregion

        #region PDF.ColorPt
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorPtCreate(ref TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorPtDestroy2(TRN_ColorPt cp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorPtInit(double x, double y, double z, double w, TRN_ColorPt result);//BasicTypes.TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorPtAssign(TRN_ColorPt left, TRN_ColorPt right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorPtCompare(TRN_ColorPt left, TRN_ColorPt right, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorPtDestroy(TRN_ColorPt cp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorPtSet(TRN_ColorPt cp, double x, double y, double z, double w);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorPtSetByIndex(TRN_ColorPt cp, int colorant_index, double colorant_value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorPtGet(TRN_ColorPt cp, int colorant_index, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorPtSetColorantNum(TRN_ColorPt cp, int num);

        #endregion

        #region PDF.ColorSpace
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceCreateDeviceGray(ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceCreateDeviceRGB(ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceCreateDeviceCMYK(ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceCreatePattern(ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceCreate(TRN_Obj color_space, ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceCreateICCFromFile(TRN_SDFDoc doc, TRN_UString filepath, ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceCreateICCFromFilter(TRN_SDFDoc doc, TRN_Filter filter, ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceCreateICCFromBuffer(TRN_SDFDoc doc, byte[] buf, uint buf_sz, ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceAssign(TRN_ColorSpace left, TRN_ColorSpace right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceDestroy(TRN_ColorSpace cs);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceGetComponentNumFromObj(ColorSpace.Type cs_type, TRN_Obj cs_obj, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceGetTypeFromObj(TRN_Obj cs_obj, ref ColorSpace.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceGetType(TRN_ColorSpace cs, ref ColorSpace.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceGetSDFObj(TRN_ColorSpace cs, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceGetComponentNum(TRN_ColorSpace cs, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceInitColor(TRN_ColorSpace cs, TRN_ColorPt out_colorants);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceInitComponentRanges(TRN_ColorSpace cs, double[] out_decode_low, double[] out_decode_range, int num_comps);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceConvert2Gray(TRN_ColorSpace cs, TRN_ColorPt in_color, TRN_ColorPt out_color);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceConvert2RGB(TRN_ColorSpace cs, TRN_ColorPt in_color, TRN_ColorPt out_color);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceConvert2CMYK(TRN_ColorSpace cs, TRN_ColorPt in_color, TRN_ColorPt out_color);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceGetAlternateColorSpace(TRN_ColorSpace cs, ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceGetBaseColorSpace(TRN_ColorSpace cs, ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceGetHighVal(TRN_ColorSpace cs, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceGetLookupTable(TRN_ColorSpace cs, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceGetBaseColor(TRN_ColorSpace cs, byte color_idx, TRN_ColorPt out_color);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceGetTintFunction(TRN_ColorSpace cs, ref TRN_Function result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceIsAll(TRN_ColorSpace cs, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ColorSpaceIsNone(TRN_ColorSpace cs, [MarshalAs(UnmanagedType.U1)] ref bool result);
        #endregion

        #region PDF.ContentReplacer
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentReplacerCreate(ref TRN_ContentReplacer result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentReplacerDestroy(TRN_ContentReplacer cr);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentReplacer_AddImage(TRN_ContentReplacer cr, ref BasicTypes.TRN_Rect target_region, TRN_Obj replacement_image);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentReplacer_AddText(TRN_ContentReplacer cr, ref BasicTypes.TRN_Rect target_region, TRN_UString replacement_text);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentReplacer_AddString(TRN_ContentReplacer cr, TRN_UString template_text, TRN_UString replacement_text);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentReplacer_Process(TRN_ContentReplacer cr, TRN_Page page);
        #endregion

        #region PDF.Date
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DateInit(UInt16 year, byte month, byte day, byte hour, byte minute, byte second, ref BasicTypes.TRN_Date result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DateAssign(ref BasicTypes.TRN_Date left, ref BasicTypes.TRN_Date right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DateIsValid(ref BasicTypes.TRN_Date date, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DateAttach(ref BasicTypes.TRN_Date date, TRN_Obj d);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DateUpdate(ref BasicTypes.TRN_Date date, TRN_Obj d, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TRN_DateSetCurrentTime(ref BasicTypes.TRN_Date date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 TRN_DateGetYear(ref BasicTypes.TRN_Date date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte TRN_DateGetMonth(ref BasicTypes.TRN_Date date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte TRN_DateGetDay(ref BasicTypes.TRN_Date date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte TRN_DateGetHour(ref BasicTypes.TRN_Date date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte TRN_DateGetMinute(ref BasicTypes.TRN_Date date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte TRN_DateGetSecond(ref BasicTypes.TRN_Date date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte TRN_DateGetUT(ref BasicTypes.TRN_Date date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte TRN_DateGetUTHour(ref BasicTypes.TRN_Date date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte TRN_DateGetUTMin(ref BasicTypes.TRN_Date date);
        #endregion

        #region PDF.Destination
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationCreateXYZ(TRN_Page page, double left, double top, double zoom, ref TRN_Destination result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationCreateFit(TRN_Page page, ref TRN_Destination result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationCreateFitH(TRN_Page page, double top, ref TRN_Destination result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationCreateFitV(TRN_Page page, double left, ref TRN_Destination result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationCreateFitR(TRN_Page page, double left, double bottom, double right, double top, ref TRN_Destination result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationCreateFitB(TRN_Page page, ref TRN_Destination result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationCreateFitBH(TRN_Page page, double top, ref TRN_Destination result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationCreateFitBV(TRN_Page page, double left, ref TRN_Destination result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationCreate(TRN_Obj dest, ref TRN_Destination result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationCopy(TRN_Destination d, ref TRN_Destination result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationIsValid(TRN_Destination dest, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationGetFitType(TRN_Destination dest, ref Destination.FitType result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationGetPage(TRN_Destination dest, ref TRN_Page result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationSetPage(TRN_Destination dest, TRN_Page page);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationGetSDFObj(TRN_Destination dest, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DestinationGetExplicitDestObj(TRN_Destination dest, ref TRN_Obj result);

        #endregion

        #region PDF.PDFDraw
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawCreate(double dpi, ref TRN_PDFDraw result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawDestroy(TRN_PDFDraw d);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetRasterizerType(TRN_PDFDraw d, PDFRasterizer.Type type);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetDPI(TRN_PDFDraw d, double dpi);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetImageSize(TRN_PDFDraw d, int width, int height, [MarshalAs(UnmanagedType.U1)] bool preserve_aspect_ratio);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetPageBox(TRN_PDFDraw d, Page.Box region);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetClipRect(TRN_PDFDraw d, ref BasicTypes.TRN_Rect clip_rect);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetFlipYAxis(TRN_PDFDraw d, [MarshalAs(UnmanagedType.U1)] bool flip_y);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetRotate(TRN_PDFDraw d, Page.Rotate r);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetDrawAnnotations(TRN_PDFDraw d, [MarshalAs(UnmanagedType.U1)] bool render_annots);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetHighlightFields(TRN_PDFDraw d, [MarshalAs(UnmanagedType.U1)] bool highlight);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetAntiAliasing(TRN_PDFDraw d, [MarshalAs(UnmanagedType.U1)] bool enable_aa);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetPathHinting(TRN_PDFDraw d, [MarshalAs(UnmanagedType.U1)] bool enable_ph);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetThinLineAdjustment(TRN_PDFDraw d, [MarshalAs(UnmanagedType.U1)] bool grid_fit, [MarshalAs(UnmanagedType.U1)] bool stroke_adjust);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetGamma(TRN_PDFDraw d, double gamma);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetOCGContext(TRN_PDFDraw d, TRN_OCGContext ctx);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetPrintMode(TRN_PDFDraw d, [MarshalAs(UnmanagedType.U1)] bool is_printing);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetPageTransparent(TRN_PDFDraw d, [MarshalAs(UnmanagedType.U1)] bool is_transp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetDefaultPageColor(TRN_PDFDraw d, byte r, byte g, byte b);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetOverprint(TRN_PDFDraw d, PDFRasterizer.OverprintPreviewMode op);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetImageSmoothing(TRN_PDFDraw d, [MarshalAs(UnmanagedType.U1)] bool smoothing_enabled, bool hq_image_resampling);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetCaching(TRN_PDFDraw d, [MarshalAs(UnmanagedType.U1)] bool enabled);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawExport(TRN_PDFDraw d, TRN_Page page, TRN_UString filename, string format, TRN_Obj encoder_params);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawExportStream(TRN_PDFDraw d, TRN_Page page, TRN_Filter stream, ref sbyte format, TRN_Obj encoder_params);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawSetColorPostProcessMode(TRN_PDFDraw d, PDFRasterizer.ColorPostProcessMode mode);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDrawGetBitmap(TRN_PDFDraw d, TRN_Page page, ref int out_width, ref int out_height, ref int out_stride, ref double out_dpi, PDFDraw.PixelFormat pix_fmt, [MarshalAs(UnmanagedType.U1)] bool demult, ref IntPtr result);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_PDFDrawSetErrorReportProc(TRN_PDFDraw d, PDFRasterizer.ErrorReportProc error_proc, ref IntPtr data);

        #endregion

        #region PDF.Element
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetType(TRN_Element e, ref Element.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetGState(TRN_Element e, ref TRN_GState result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetCTM(TRN_Element e, ref BasicTypes.TRN_Matrix2D result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetBBox(TRN_Element e, ref BasicTypes.TRN_Rect out_bbox, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetParentStructElement(TRN_Element e, ref BasicTypes.TRN_SElement result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetStructMCID(TRN_Element e, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementIsOCVisible(TRN_Element e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementIsClippingPath(TRN_Element e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementIsStroked(TRN_Element e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementIsFilled(TRN_Element e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementIsWindingFill(TRN_Element e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementIsClipWindingFill(TRN_Element e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetPathTypes(TRN_Element e,  ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetPathTypesCount(TRN_Element e, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetPathPoints(TRN_Element e, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetPathPointCount(TRN_Element e, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementSetPathClip(TRN_Element e, [MarshalAs(UnmanagedType.U1)] bool clip);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementSetPathStroke(TRN_Element e, [MarshalAs(UnmanagedType.U1)] bool stroke);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementSetPathFill(TRN_Element e, [MarshalAs(UnmanagedType.U1)] bool fill);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementSetWindingFill(TRN_Element e, [MarshalAs(UnmanagedType.U1)] bool winding_rule);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementSetClipWindingFill(TRN_Element e, [MarshalAs(UnmanagedType.U1)] bool winding_rule);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementSetPathPoints(TRN_Element e, double[] in_points, int count);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementSetPathTypes(TRN_Element e, IntPtr in_seg_types, int count);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetXObject(TRN_Element e, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetImageData(TRN_Element e, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetImageDataSize(TRN_Element e, ref int result);

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern TRN_Exception TRN_ElementGetBitmap(TRN_Element e, ref TRN_GDIPlusBitmap result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetImageColorSpace(TRN_Element e, ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetImageWidth(TRN_Element e, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetImageHeight(TRN_Element e, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetDecodeArray(TRN_Element e, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetBitsPerComponent(TRN_Element e, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetComponentNum(TRN_Element e, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementIsImageMask(TRN_Element e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementIsImageInterpolate(TRN_Element e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetMask(TRN_Element e, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetImageRenderingIntent(TRN_Element e, ref GState.RenderingIntent result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetTextString(TRN_Element e, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetTextData(TRN_Element e, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetTextDataSize(TRN_Element e, ref uint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetTextMatrix(TRN_Element e, ref BasicTypes.TRN_Matrix2D result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetCharIterator(TRN_Element e, ref TRN_Iterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetTextLength(TRN_Element e, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetPosAdjustment(TRN_Element e, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetNewTextLineOffset(TRN_Element e, ref double out_x, ref double out_y);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementHasTextMatrix(TRN_Element e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementSetTextData(TRN_Element e, byte[] text_data, int text_data_size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementSetTextMatrix(TRN_Element e, ref BasicTypes.TRN_Matrix2D mtx);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementSetTextMatrixEntries(TRN_Element e, double a, double b, double c, double d, double h, double v);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementSetPosAdjustment(TRN_Element e, double adjust);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementUpdateTextMetrics(TRN_Element e);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementSetNewTextLineOffset(TRN_Element e, double dx, double dy);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetShading(TRN_Element e, ref TRN_Shading result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetMCPropertyDict(TRN_Element e, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementGetMCTag(TRN_Element e, ref TRN_Obj result);

        #endregion

        #region PDF.ElementBuilder
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreate(ref TRN_ElementBuilder result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderDestroy(TRN_ElementBuilder b);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderReset(TRN_ElementBuilder b, TRN_GState gs);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateImage(TRN_ElementBuilder b, TRN_Image img, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateImageFromMatrix(TRN_ElementBuilder b, TRN_Image img, ref BasicTypes.TRN_Matrix2D mtx, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateImageScaled(TRN_ElementBuilder b, TRN_Image img, Double x, Double y, Double hscale, Double vscale, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateGroupBegin(TRN_ElementBuilder b, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateGroupEnd(TRN_ElementBuilder b, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateShading(TRN_ElementBuilder b, TRN_Shading sh, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateFormFromStream(TRN_ElementBuilder b, TRN_Obj form, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateFormFromPage(TRN_ElementBuilder b, TRN_Page page, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateFormFromDoc(TRN_ElementBuilder b, TRN_Page page, TRN_PDFDoc doc, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateTextBeginWithFont(TRN_ElementBuilder b, TRN_Font font, Double font_sz, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateTextBegin(TRN_ElementBuilder b, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateTextEnd(TRN_ElementBuilder b, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateTextRun(TRN_ElementBuilder b, string text_data, TRN_Font font, Double font_sz, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateTextRunWithSize(TRN_ElementBuilder b, string text_data, uint text_data_sz, TRN_Font font, Double font_sz, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateTextRunUnsigned(TRN_ElementBuilder b, ref byte text_data, uint text_data_sz, TRN_Font font, Double font_sz, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateNewTextRun(TRN_ElementBuilder b, string text_data, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateNewTextRunWithSize(TRN_ElementBuilder b, string text_data, uint text_data_sz, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateNewTextRunUnsigned(TRN_ElementBuilder b, ref byte text_data, uint text_data_sz, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateUnicodeTextRun(TRN_ElementBuilder b, IntPtr text_data, uint text_data_sz, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateTextNewLineWithOffset(TRN_ElementBuilder b, Double dx, Double dy, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateTextNewLine(TRN_ElementBuilder b, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreatePath(TRN_ElementBuilder b, IntPtr points, int point_count, IntPtr seg_types, int seg_types_count, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateRect(TRN_ElementBuilder b, Double x, Double y, Double width, Double height, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCreateEllipse(TRN_ElementBuilder b, Double cx, Double cy, Double rx, Double ry, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderPathBegin(TRN_ElementBuilder b);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderPathEnd(TRN_ElementBuilder b, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderRect(TRN_ElementBuilder b, Double cx, Double cy, Double rx, Double ry);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderEllipse(TRN_ElementBuilder b, Double x, Double y, Double width, Double height);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderMoveTo(TRN_ElementBuilder b, Double x, Double y);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderLineTo(TRN_ElementBuilder b, Double x, Double y);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderCurveTo(TRN_ElementBuilder b, Double cx1, Double cy1, Double cx2, Double cy2, Double x2, Double y2);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderArcTo(TRN_ElementBuilder b, Double x, Double y, Double width, Double height, Double start, Double extent);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderArcTo2(TRN_ElementBuilder b, Double xr, Double yr, Double rx, [MarshalAs(UnmanagedType.U1)] bool isLargeArc, [MarshalAs(UnmanagedType.U1)] bool sweep, Double endX, Double endY);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementBuilderClosePath(TRN_ElementBuilder b);

        #endregion

        #region PDF.ElementReader
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderCreate(ref TRN_ElementReader result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderDestroy(TRN_ElementReader r);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderBeginOnPage(TRN_ElementReader r, TRN_Page page, TRN_OCGContext ctx);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderBegin(TRN_ElementReader r, TRN_Obj content_stream, TRN_Obj resource_dict, TRN_OCGContext ctx);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderAppendResource(TRN_ElementReader r, TRN_Obj res);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderNext(TRN_ElementReader r, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderCurrent(TRN_ElementReader r, ref TRN_Element result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderFormBegin(TRN_ElementReader r);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderPatternBegin(TRN_ElementReader r, [MarshalAs(UnmanagedType.U1)] bool fill_pattern, [MarshalAs(UnmanagedType.U1)] bool reset_ctm_tfm);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderType3FontBegin(TRN_ElementReader r, ref BasicTypes.TRN_CharData char_data, TRN_Obj resource_dict);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderEnd(TRN_ElementReader r, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderGetChangesIterator(TRN_ElementReader r, ref TRN_Iterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderIsChanged(TRN_ElementReader r, GState.GStateAttribute attrib, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderClearChangeList(TRN_ElementReader r);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderGetFont(TRN_ElementReader r, string name, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderGetXObject(TRN_ElementReader r, string name, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderGetShading(TRN_ElementReader r, string name, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderGetColorSpace(TRN_ElementReader r, string name, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderGetPattern(TRN_ElementReader r, string name, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementReaderGetExtGState(TRN_ElementReader r, string name, ref TRN_Obj result);
        #endregion

        #region PDF.ElementWriter
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementWriterCreate(ref TRN_ElementWriter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementWriterDestroy(TRN_ElementWriter w);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementWriterBeginOnPage(TRN_ElementWriter w, TRN_Page page, ElementWriter.WriteMode placement, [MarshalAs(UnmanagedType.U1)] bool page_coord_sys, [MarshalAs(UnmanagedType.U1)] bool compress, TRN_Obj resources);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementWriterBegin(TRN_ElementWriter w, TRN_SDFDoc doc, [MarshalAs(UnmanagedType.U1)] bool compress);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementWriterBeginOnObj(TRN_ElementWriter w, TRN_Obj obj, [MarshalAs(UnmanagedType.U1)] bool compress, TRN_Obj resources);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementWriterEnd(TRN_ElementWriter w, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementWriterWriteElement(TRN_ElementWriter w, TRN_Element element);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementWriterWritePlacedElement(TRN_ElementWriter w, TRN_Element element);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementWriterFlush(TRN_ElementWriter w);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementWriterWriteBuffer(TRN_ElementWriter w, IntPtr data, int data_sz);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementWriterWriteString(TRN_ElementWriter w, string str);
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ElementWriterSetDefaultGState(TRN_ElementWriter w, TRN_ElementReader r);


        #endregion

        #region PDF.Field
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldCreate(TRN_Obj field_dict, ref BasicTypes.TRN_Field result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldAssign(ref BasicTypes.TRN_Field left, ref BasicTypes.TRN_Field right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldDestroy(ref BasicTypes.TRN_Field field);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldIsValid(ref BasicTypes.TRN_Field field, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetType(ref BasicTypes.TRN_Field field, ref Field.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetValue(ref BasicTypes.TRN_Field field, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetValueAsString(ref BasicTypes.TRN_Field field, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetDefaultValueAsString(ref BasicTypes.TRN_Field field, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_FieldSetValueAsString(ref BasicTypes.TRN_Field field, TRN_UString value, ref TRN_ViewChangeCollection result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_FieldSetValue(ref BasicTypes.TRN_Field field, TRN_Obj value, ref TRN_ViewChangeCollection result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_FieldSetValueAsBool(ref BasicTypes.TRN_Field field, [MarshalAs(UnmanagedType.U1)] bool value, ref TRN_ViewChangeCollection result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetValueAsBool(ref BasicTypes.TRN_Field field, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldRefreshAppearance(ref BasicTypes.TRN_Field field);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldEraseAppearance(ref BasicTypes.TRN_Field field);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetDefaultValue(ref BasicTypes.TRN_Field field, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetName(ref BasicTypes.TRN_Field field, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetPartialName(ref BasicTypes.TRN_Field field, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldRename(ref BasicTypes.TRN_Field field, TRN_UString field_name);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldIsAnnot(ref BasicTypes.TRN_Field field, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldUseSignatureHandler(ref BasicTypes.TRN_Field field, SignatureHandlerId signature_handler_id, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetFlag(ref BasicTypes.TRN_Field field, Field.Flag flag, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldSetFlag(ref BasicTypes.TRN_Field field, Field.Flag flag, [MarshalAs(UnmanagedType.U1)] bool value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetJustification(ref BasicTypes.TRN_Field field, ref Field.TextJustification result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldSetJustification(ref BasicTypes.TRN_Field field, Field.TextJustification j);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldSetMaxLen(ref BasicTypes.TRN_Field field, int max_len);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetMaxLen(ref BasicTypes.TRN_Field field, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetDefaultAppearance(ref BasicTypes.TRN_Field field, ref TRN_GState result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetUpdateRect(ref BasicTypes.TRN_Field field, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldFlatten(ref BasicTypes.TRN_Field field, TRN_Page page);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldFindInheritedAttribute(ref BasicTypes.TRN_Field field, string attrib, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetSDFObj(ref BasicTypes.TRN_Field field, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetOptCount(ref BasicTypes.TRN_Field field, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldGetOpt(ref BasicTypes.TRN_Field field, int index, ref TRN_UString result);
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FieldIsLockedByDigitalSignature(ref BasicTypes.TRN_Field field, [MarshalAs(UnmanagedType.U1)] ref bool result);

        #endregion

        #region PDF.FileSpec
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileSpecCreate(TRN_SDFDoc doc, TRN_UString path, [MarshalAs(UnmanagedType.U1)] bool embed, ref TRN_FileSpec result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileSpecCreateURL(TRN_SDFDoc doc, string url, ref TRN_FileSpec result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileSpecCreateFromObj(TRN_Obj f, ref TRN_FileSpec result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileSpecCopy(TRN_FileSpec d, ref TRN_FileSpec result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileSpecCompare(TRN_FileSpec fs, TRN_FileSpec d, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileSpecIsValid(TRN_FileSpec fs, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileSpecExport(TRN_FileSpec fs, TRN_UString save_as, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileSpecGetFileData(TRN_FileSpec fs, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileSpecGetFilePath(TRN_FileSpec fs, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileSpecSetDesc(TRN_FileSpec fs, TRN_UString desc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FileSpecGetSDFObj(TRN_FileSpec fs, ref TRN_Obj result);

        #endregion

		#region PDF.Flattener
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FlattenerCreate (ref TRN_Flattener result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FlattenerDestroy (TRN_Flattener flattener);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FlattenerSetDPI (TRN_Flattener flattener, uint dpi);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FlattenerSetThreshold (TRN_Flattener flattener, PDF.Flattener.Threshold threshold);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FlattenerSetMaximumImagePixels (TRN_Flattener flattener, uint max_pixels);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FlattenerSetPreferJPG (TRN_Flattener flattener, [MarshalAs(UnmanagedType.U1)] bool jpg);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FlattenerSetJPGQuality (TRN_Flattener flattener, uint quality);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FlattenerSetPathHinting (TRN_Flattener flattener, [MarshalAs(UnmanagedType.U1)] bool hinting);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FlattenerProcess (TRN_Flattener flattener, TRN_PDFDoc doc, PDF.Flattener.FlattenMode mode);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FlattenerProcessPage(TRN_Flattener flattener, TRN_Page page, PDF.Flattener.FlattenMode mode);
        #endregion

        #region PDF.Font
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontCreateFromObj (TRN_Obj font_dict, ref TRN_Font result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontCreate (TRN_SDFDoc doc, PDF.Font.StandardType1Font type, ref TRN_Font result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontCreateFromFontDescriptor (TRN_SDFDoc doc, TRN_Font from, TRN_UString char_set, ref TRN_Font result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontCreateFromName (TRN_SDFDoc doc, string name, TRN_UString char_set, ref TRN_Font result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontCreateAndEmbed (TRN_SDFDoc doc, PDF.Font.StandardType1Font type, ref TRN_Font result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontCreateTrueTypeFont (TRN_SDFDoc doc, TRN_UString font_path, [MarshalAs(UnmanagedType.U1)] bool embed, [MarshalAs(UnmanagedType.U1)] bool subset, ref TRN_Font result);

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern TRN_Exception TRN_FontCreateTrueTypeFont2 (TRN_SDFDoc doc, ref IntPtr type, [MarshalAs(UnmanagedType.U1)] bool embed, [MarshalAs(UnmanagedType.U1)] bool subset, ref TRN_Font result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_FontCreateCIDTrueTypeFont (TRN_SDFDoc doc, TRN_UString type, [MarshalAs(UnmanagedType.U1)] bool embed, [MarshalAs(UnmanagedType.U1)] bool subset, PDF.Font.Encoding encoding, UInt32 ttc_font_index, ref TRN_Font result);

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern TRN_Exception TRN_FontCreateCIDTrueTypeFont2 (TRN_SDFDoc doc, ref IntPtr logfont, [MarshalAs(UnmanagedType.U1)] bool embed, [MarshalAs(UnmanagedType.U1)] bool subset, PDF.Font.Encoding encoding, ref TRN_Font result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontCreateType1Font (TRN_SDFDoc doc, TRN_UString type, [MarshalAs(UnmanagedType.U1)] bool embed, ref TRN_Font result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontAssign (TRN_Font left, TRN_Font right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontDestroy (TRN_Font font);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetType (TRN_Font font, ref PDF.Font.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontIsSimple (TRN_Font font, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetTypeFromObj (TRN_Obj font_dict, ref PDF.Font.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetSDFObj (TRN_Font font, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetDescriptor (TRN_Font font, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetName (TRN_Font font, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetFamilyName (TRN_Font font, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontIsFixedWidth (TRN_Font font, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontIsSerif (TRN_Font font, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontIsSymbolic (TRN_Font font, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontIsItalic (TRN_Font font, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontIsAllCap (TRN_Font font, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontIsForceBold (TRN_Font font, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontIsHorizontalMode (TRN_Font font, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetWidth (TRN_Font font, uint char_code, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetMaxWidth (TRN_Font font, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetMissingWidth (TRN_Font font, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetCharCodeIterator (TRN_Font font, ref TRN_Iterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetGlyphPath(TRN_Font font, uint char_code, IntPtr buf_oprs, ref int out_buf_oprs_sz, IntPtr buf_data, ref int out_buf_data_sz, ref int glyp_idx, [MarshalAs(UnmanagedType.U1)] bool conics2cubics, IntPtr transform, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontMapToUnicode (TRN_Font font, uint char_code, UInt16[] out_uni_arr, int in_uni_sz, ref int out_chars, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetEncoding (TRN_Font font, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontIsEmbedded (TRN_Font font, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetEmbeddedFontName (TRN_Font font, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetEmbeddedFont (TRN_Font font, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetEmbeddedFontBufSize (TRN_Font font, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetUnitsPerEm (TRN_Font font, ref UInt16 result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetBBox (TRN_Font font, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetAscent (TRN_Font font, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetDescent (TRN_Font font, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetStandardType1FontType (TRN_Font font, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontIsCFF (TRN_Font font, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetType3FontMatrix(TRN_Font font, ref BasicTypes.TRN_Matrix2D result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetType3GlyphStream (TRN_Font font, uint char_code, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetVerticalAdvance (TRN_Font font, uint char_code, ref Double out_pos_vect_x, ref Double out_pos_vect_y, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontGetDescendant (TRN_Font font, ref TRN_Font result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontMapToCID (TRN_Font font, uint char_code, ref uint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FontMapToCID2 (TRN_Font font, ref byte char_data, int char_data_avail, ref uint out_charcode, ref uint out_cid, ref int result);

        #endregion

        #region PDF.Function
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FunctionCreate (TRN_Obj funct_dict, ref TRN_Function result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FunctionAssign (TRN_Function left, TRN_Function right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FunctionDestroy (TRN_Function f);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FunctionGetType (TRN_Function f, ref PDF.Function.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FunctionGetInputCardinality (TRN_Function f, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FunctionGetOutputCardinality (TRN_Function f, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FunctionEval (TRN_Function f, double[] @in, double[] @out);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_FunctionGetSDFObj (TRN_Function f, ref TRN_Obj result);
        #endregion

        #region PDF.GState
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetTransform (TRN_GState gs, ref BasicTypes.TRN_Matrix2D result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetStrokeColorSpace (TRN_GState gs, ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetFillColorSpace (TRN_GState gs, ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetStrokeColor (TRN_GState gs, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetStrokePattern (TRN_GState gs, ref TRN_PatternColor result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetFillColor (TRN_GState gs, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetFillPattern (TRN_GState gs, ref TRN_PatternColor result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetFlatness (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetLineCap (TRN_GState gs, ref PDF.GState.LineCap result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetLineJoin (TRN_GState gs, ref PDF.GState.LineJoin result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetLineWidth (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetMiterLimit (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetDashes (TRN_GState gs, ref IntPtr dashes, ref int ret_dashes_sz);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetPhase (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetCharSpacing (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetWordSpacing (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetHorizontalScale (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetLeading (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetFont (TRN_GState gs, ref TRN_Font result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetFontSize (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetTextRenderMode (TRN_GState gs, ref PDF.GState.TextRenderingMode result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetTextRise (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateIsTextKnockout (TRN_GState gs, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetRenderingIntent (TRN_GState gs, ref PDF.GState.RenderingIntent result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetRenderingIntentType (string name, ref PDF.GState.RenderingIntent result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetBlendMode (TRN_GState gs, ref PDF.GState.BlendMode result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetFillOpacity (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetStrokeOpacity (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetAISFlag (TRN_GState gs, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetSoftMask (TRN_GState gs, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetSoftMaskTransform(TRN_GState gs, ref BasicTypes.TRN_Matrix2D result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetStrokeOverprint (TRN_GState gs, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetFillOverprint (TRN_GState gs, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetOverprintMode (TRN_GState gs, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetAutoStrokeAdjust (TRN_GState gs, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetSmoothnessTolerance (TRN_GState gs, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetTransferFunct (TRN_GState gs, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetBlackGenFunct (TRN_GState gs, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetUCRFunct (TRN_GState gs, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateGetHalftone (TRN_GState gs, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetTransformMatrix(TRN_GState gs, ref BasicTypes.TRN_Matrix2D mtx);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetTransform (TRN_GState gs, Double a, Double b, Double c, Double d, Double h, Double v);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateConcatMatrix(TRN_GState gs, ref BasicTypes.TRN_Matrix2D mtx);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateConcat (TRN_GState gs, Double a, Double b, Double c, Double d, Double h, Double v);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetStrokeColorSpace (TRN_GState gs, TRN_ColorSpace cs);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetFillColorSpace (TRN_GState gs, TRN_ColorSpace cs);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetStrokeColorWithColorPt (TRN_GState gs, TRN_ColorPt c);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetStrokeColorWithPattern (TRN_GState gs, TRN_PatternColor pattern);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetStrokeColor (TRN_GState gs, TRN_PatternColor pattern, TRN_ColorPt c);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetFillColorWithColorPt(TRN_GState gs, TRN_ColorPt c);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetFillColorWithPattern (TRN_GState gs, TRN_PatternColor pattern);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetFillColor(TRN_GState gs, TRN_PatternColor pattern, TRN_ColorPt c);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetFlatness (TRN_GState gs, Double flatness);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetLineCap (TRN_GState gs, PDF.GState.LineCap cap);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetLineJoin (TRN_GState gs, PDF.GState.LineJoin join);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetLineWidth (TRN_GState gs, Double width);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetMiterLimit (TRN_GState gs, Double miter_limit);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetDashPattern (TRN_GState gs, Double[] dash_array, int dash_array_sz, Double phase);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetCharSpacing (TRN_GState gs, Double char_spacing);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetWordSpacing (TRN_GState gs, Double word_spacing);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetHorizontalScale (TRN_GState gs, Double hscale);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetLeading (TRN_GState gs, Double leading);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetFont (TRN_GState gs, TRN_Font font, Double font_sz);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetTextRenderMode (TRN_GState gs, PDF.GState.TextRenderingMode rmode);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetTextRise (TRN_GState gs, Double rise);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetTextKnockout (TRN_GState gs, [MarshalAs(UnmanagedType.U1)] bool knockout);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetRenderingIntent (TRN_GState gs, PDF.GState.RenderingIntent intent);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetBlendMode (TRN_GState gs, PDF.GState.BlendMode BM);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetFillOpacity (TRN_GState gs, Double ca);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetStrokeOpacity (TRN_GState gs, Double CA);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetAISFlag (TRN_GState gs, [MarshalAs(UnmanagedType.U1)] bool AIS);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetSoftMask (TRN_GState gs, TRN_Obj SM);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetStrokeOverprint (TRN_GState gs, [MarshalAs(UnmanagedType.U1)] bool OP);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetFillOverprint (TRN_GState gs, [MarshalAs(UnmanagedType.U1)] bool op);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetOverprintMode (TRN_GState gs, int OPM);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetAutoStrokeAdjust (TRN_GState gs, [MarshalAs(UnmanagedType.U1)] bool SA);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetSmoothnessTolerance (TRN_GState gs, Double SM);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetBlackGenFunct (TRN_GState gs, TRN_Obj BG);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetUCRFunct (TRN_GState gs, TRN_Obj UCR);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetTransferFunct (TRN_GState gs, TRN_Obj TR);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GStateSetHalftone (TRN_GState gs, TRN_Obj HT);
        #endregion

        #region PDF.Highlights
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsCreate (ref TRN_Highlights result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsDestroy (TRN_Highlights hlts);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsCopyCtor(TRN_Highlights hlts, ref TRN_Highlights result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsAssign (TRN_Highlights result, TRN_Highlights hlts);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsAdd (TRN_Highlights hlts, TRN_Highlights hlt2);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsLoad (TRN_Highlights hlts, TRN_UString file_name);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsSave (TRN_Highlights hlts, TRN_UString file_name);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsClear (TRN_Highlights hlts);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsBegin (TRN_Highlights hlts, TRN_PDFDoc doc);

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern TRN_Exception TRN_HighlightsBegin2 (TRN_Highlights hlts);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsHasNext (TRN_Highlights hlts, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsNext (TRN_Highlights hlts);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsGetCurrentPageNumber (TRN_Highlights hlts, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HighlightsGetCurrentQuads (TRN_Highlights hlts, ref IntPtr quads, ref int result);

        #endregion


#if (__DESKTOP__)
        #region HTML2PDF
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_ProxyCreate(ref TRN_HTML2PDF_Proxy result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_ProxyDestroy(TRN_HTML2PDF_Proxy proxy);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_ProxySetType(TRN_HTML2PDF_Proxy proxy, HTML2PDF.Proxy.Type type);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_ProxySetPort(TRN_HTML2PDF_Proxy proxy, int port);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_ProxySetHost(TRN_HTML2PDF_Proxy proxy, TRN_UString host);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_ProxySetUsername(TRN_HTML2PDF_Proxy proxy, TRN_UString username);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_ProxySetPassword(TRN_HTML2PDF_Proxy proxy, TRN_UString password);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsCreate(ref TRN_HTML2PDF_WebPageSettings result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsDestroy(TRN_HTML2PDF_WebPageSettings settings);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetPrintBackground(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool background);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetLoadImages(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool load);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetAllowJavaScript(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool enable);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetSmartShrinking(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool enable);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetMinimumFontSize(TRN_HTML2PDF_WebPageSettings settings, int size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetDefaultEncoding(TRN_HTML2PDF_WebPageSettings settings, TRN_UString encoding);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetUserStyleSheet(TRN_HTML2PDF_WebPageSettings settings, TRN_UString url);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetAllowPlugins(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool enable);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetPrintMediaType(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool print);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetIncludeInOutline(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool include);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetUsername(TRN_HTML2PDF_WebPageSettings settings, TRN_UString username);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetPassword(TRN_HTML2PDF_WebPageSettings settings, TRN_UString password);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetJavaScriptDelay(TRN_HTML2PDF_WebPageSettings settings, int msec);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetZoom(TRN_HTML2PDF_WebPageSettings settings, Double zoom);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetBlockLocalFileAccess(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool block);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetStopSlowScripts(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool stop);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetDebugJavaScriptOutput(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool forward);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetLoadErrorHandling(TRN_HTML2PDF_WebPageSettings settings, HTML2PDF.WebPageSettings.ErrorHandling val);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetExternalLinks(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool convert);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetInternalLinks(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool convert);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetProduceForms(TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] bool forms);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_WebPageSettingsSetProxy(TRN_HTML2PDF_WebPageSettings settings, TRN_HTML2PDF_Proxy proxy);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_TOCSettingsCreate(ref TRN_HTML2PDF_TOCSettings result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_TOCSettingsDestroy(TRN_HTML2PDF_TOCSettings settings);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_TOCSettingsSetDottedLines(TRN_HTML2PDF_TOCSettings settings, [MarshalAs(UnmanagedType.U1)] bool enable);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_TOCSettingsSetLinks(TRN_HTML2PDF_TOCSettings settings, [MarshalAs(UnmanagedType.U1)] bool enable);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_TOCSettingsSetCaptionText(TRN_HTML2PDF_TOCSettings settings, TRN_UString caption);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_TOCSettingsSetLevelIndentation(TRN_HTML2PDF_TOCSettings settings, int indentation);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_TOCSettingsSetTextSizeShrink(TRN_HTML2PDF_TOCSettings settings, Double shrink);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDF_TOCSettingsSetXsl(TRN_HTML2PDF_TOCSettings settings, TRN_UString style_sheet);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFStaticConvert(TRN_PDFDoc doc, TRN_UString url, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFStaticConvert2(TRN_PDFDoc doc, TRN_UString url, TRN_HTML2PDF_WebPageSettings settings, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFCreate(ref TRN_HTML2PDF result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFDestroy(TRN_HTML2PDF html2pdf);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFInsertFromUrl(TRN_HTML2PDF html2pdf, TRN_UString url);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFInsertFromUrl2(TRN_HTML2PDF html2pdf, TRN_UString url, TRN_HTML2PDF_WebPageSettings settings);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFInsertFromHtmlString(TRN_HTML2PDF html2pdf, TRN_UString html);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFInsertFromHtmlString2(TRN_HTML2PDF html2pdf, TRN_UString html, TRN_HTML2PDF_WebPageSettings settings);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFInsertTOC(TRN_HTML2PDF html2pdf);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFInsertTOC2(TRN_HTML2PDF html2pdf, TRN_HTML2PDF_TOCSettings settings);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFConvert(TRN_HTML2PDF html2pdf, TRN_PDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFGetHttpErrorCode(TRN_HTML2PDF html2pdf, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFGetLog(TRN_HTML2PDF html2pdf, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFSetPaperSize(TRN_HTML2PDF html2pdf, PrinterMode.PaperSize size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFSetPaperSize2(TRN_HTML2PDF html2pdf, TRN_UString width, TRN_UString height);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFSetLandscape(TRN_HTML2PDF html2pdf, [MarshalAs(UnmanagedType.U1)] bool enable);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFSetDPI(TRN_HTML2PDF html2pdf, int dpi);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFSetOutline(TRN_HTML2PDF html2pdf, [MarshalAs(UnmanagedType.U1)] bool enable, int depth);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFDumpOutline(TRN_HTML2PDF html2pdf, TRN_UString xml_file);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFSetPDFCompression(TRN_HTML2PDF html2pdf, [MarshalAs(UnmanagedType.U1)] bool enable);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFSetMargins(TRN_HTML2PDF html2pdf, TRN_UString top, TRN_UString bottom, TRN_UString left, TRN_UString right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFSetImageDPI(TRN_HTML2PDF html2pdf, int dpi);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFSetImageQuality(TRN_HTML2PDF html2pdf, int quality);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFSetCookieJar(TRN_HTML2PDF html2pdf, TRN_UString path);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFSetQuiet(TRN_HTML2PDF html2pdf, [MarshalAs(UnmanagedType.U1)] bool quiet);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_HTML2PDFSetModulePath(TRN_UString path);
        #endregion

        #region PDF.Print

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PrintStartPrintJob(TRN_PDFDoc in_pdfdoc, TRN_UString in_filename, TRN_UString in_jobname, TRN_UString in_outputFileName, TRN_PageSet in_pagesToPrint, TRN_Obj in_printerMode, [MarshalAs(UnmanagedType.U1)] out bool in_cancel, TRN_OCGContext in_ctx);

        #endregion
#endif

        #region PDF.Image
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageCreateFromFile (TRN_SDFDoc doc, TRN_UString filename, TRN_Obj encoder_hints, ref TRN_Image result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ImageCreateFromMemory (TRN_SDFDoc doc, IntPtr image_data, UIntPtr image_data_size, int width, int height, int bpc, TRN_ColorSpace color_space, TRN_Obj encoder_hints, ref TRN_Image result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageCreateFromStream (TRN_SDFDoc doc, TRN_FilterReader image_data, int width, int height, int bpc, TRN_ColorSpace color_space, TRN_Obj encoder_hints, ref TRN_Image result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ImageCreateFromMemory2 (TRN_SDFDoc doc, IntPtr image_data, UIntPtr image_data_size, TRN_Obj encoder_hints, ref TRN_Image result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ImageCreateFromStream2 (TRN_SDFDoc doc, TRN_FilterReader image_data, TRN_Obj encoder_hints, ref TRN_Image result);

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern TRN_Exception TRN_ImageCreateFromBitmap (TRN_SDFDoc doc, TRN_GDIPlusBitmap bmp, TRN_Obj encoder_hints, ref TRN_Image result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageCreateImageMask (TRN_SDFDoc doc, byte[] image_data, UIntPtr image_data_size, int width, int height, TRN_Obj encoder_hints, ref TRN_Image result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageCreateImageMaskFromStream (TRN_SDFDoc doc, TRN_FilterReader image_data, int width, int height, TRN_Obj encoder_hints, ref TRN_Image result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageCreateSoftMask (TRN_SDFDoc doc, IntPtr image_data, UIntPtr image_data_size, int width, int height, int bpc, TRN_Obj encoder_hints, ref TRN_Image result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageCreateSoftMaskFromStream (TRN_SDFDoc doc, TRN_FilterReader image_data, int width, int height, int bpc, TRN_Obj encoder_hints, ref TRN_Image result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageCreateDirectFromMemory (TRN_SDFDoc doc, IntPtr image_data, UIntPtr image_data_size, int width, int height, int bpc, TRN_ColorSpace color_space, PDF.Image.InputFilter input_format, ref TRN_Image result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageCreateDirectFromStream (TRN_SDFDoc doc, TRN_FilterReader image_data, int width, int height, int bpc, TRN_ColorSpace color_space, PDF.Image.InputFilter input_format, ref TRN_Image result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageCreateFromObj (TRN_Obj image_xobject, ref TRN_Image result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageCopy (TRN_Image c, ref TRN_Image result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageGetSDFObj (TRN_Image img, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageIsValid (TRN_Image img, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageGetImageData (TRN_Image img, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageGetImageDataSize (TRN_Image img, ref int result);

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern TRN_Exception TRN_ImageGetBitmap (TRN_Image img, ref TRN_GDIPlusBitmap result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageGetImageColorSpace (TRN_Image img, ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageGetImageWidth (TRN_Image img, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageGetImageHeight (TRN_Image img, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageGetDecodeArray (TRN_Image img, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageGetBitsPerComponent (TRN_Image img, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageGetComponentNum (TRN_Image img, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageIsImageMask (TRN_Image img, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageIsImageInterpolate (TRN_Image img, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageGetMask (TRN_Image img, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageSetMask (TRN_Image img, TRN_Image image_mask);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageSetMaskWithObj (TRN_Image img, TRN_Obj mask);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageGetSoftMask (TRN_Image img, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageSetSoftMask (TRN_Image img, TRN_Image soft_mask);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageGetImageRenderingIntent (TRN_Image img, ref PDF.GState.RenderingIntent result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageExport (TRN_Image img, TRN_UString filename, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageExportFromStream (TRN_Image img, TRN_FilterWriter writer, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageExportAsTiff (TRN_Image img, TRN_UString filename);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageExportAsTiffFromStream (TRN_Image img, TRN_FilterWriter writer);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageExportAsPng (TRN_Image img, TRN_UString filename);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ImageExportAsPngFromStream (TRN_Image img, TRN_FilterWriter writer);
        #endregion

        #region PDF.OCG.Config
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigCreateFromObj(TRN_Obj dict, ref TRN_OCGConfig result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigCreate(TRN_PDFDoc pdfdoc, [MarshalAs(UnmanagedType.U1)] bool default_config, ref TRN_OCGConfig result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigCopy(TRN_OCGConfig c, ref TRN_OCGConfig result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigGetSDFObj(TRN_OCGConfig c, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigGetOrder(TRN_OCGConfig c, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigSetOrder(TRN_OCGConfig c, TRN_Obj value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigGetName(TRN_OCGConfig c, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigSetName(TRN_OCGConfig c, TRN_UString value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigGetCreator(TRN_OCGConfig c, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigSetCreator(TRN_OCGConfig c, TRN_UString value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigGetInitBaseState(TRN_OCGConfig c, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigSetInitBaseState(TRN_OCGConfig c, string value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigGetInitOnStates(TRN_OCGConfig c, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigSetInitOnStates(TRN_OCGConfig c, TRN_Obj value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigGetInitOffStates(TRN_OCGConfig c, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigSetInitOffStates(TRN_OCGConfig c, TRN_Obj value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigGetIntent(TRN_OCGConfig c, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigSetIntent(TRN_OCGConfig c, TRN_Obj value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigGetLockedOCGs(TRN_OCGConfig c, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGConfigSetLockedOCGs(TRN_OCGConfig c, TRN_Obj value);
    #endregion

        #region PDF.OCG.Context
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGContextCreateFromConfig(TRN_OCGConfig cfg, ref TRN_OCGContext result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGContextCopy(TRN_OCGContext c, ref TRN_OCGContext result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGContextDestroy(TRN_OCGContext ctx);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGContextGetState(TRN_OCGContext c, TRN_OCG grp, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGContextSetState(TRN_OCGContext c, TRN_OCG grp, [MarshalAs(UnmanagedType.U1)] bool state);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGContextResetStates(TRN_OCGContext c, [MarshalAs(UnmanagedType.U1)] bool all_on);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGContextSetNonOCDrawing(TRN_OCGContext c, [MarshalAs(UnmanagedType.U1)] bool draw_non_OC);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGContextGetNonOCDrawing(TRN_OCGContext c, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGContextSetOCDrawMode(TRN_OCGContext c, pdftron.PDF.OCG.Context.OCDrawMode oc_draw_mode);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGContextGetOCMode(TRN_OCGContext c, ref pdftron.PDF.OCG.Context.OCDrawMode result);
        #endregion

        #region PDF.OCG.Group
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGCreate (TRN_PDFDoc pdfdoc, TRN_UString name, ref TRN_OCG result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGCreateFromObj (TRN_Obj ocg_dict, ref TRN_OCG result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGCopy (TRN_OCG ocg, ref TRN_OCG result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGGetSDFObj (TRN_OCG ocg, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGIsValid (TRN_OCG ocg, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGGetName (TRN_OCG c, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGSetName (TRN_OCG c, TRN_UString value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGGetIntent (TRN_OCG c, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGSetIntent (TRN_OCG c, TRN_Obj value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGHasUsage (TRN_OCG c, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGGetUsage (TRN_OCG c, string key, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGGetCurrentState (TRN_OCG c, TRN_OCGContext ctx, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGSetCurrentState (TRN_OCG c, TRN_OCGContext ctx, [MarshalAs(UnmanagedType.U1)] bool state);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGGetInitialState (TRN_OCG c, TRN_OCGConfig cfg, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGSetInitialState (TRN_OCG c, TRN_OCGConfig cfg, [MarshalAs(UnmanagedType.U1)] bool state);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGIsLocked (TRN_OCG c, TRN_OCGConfig cfg, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCGSetLocked (TRN_OCG c, TRN_OCGConfig cfg, [MarshalAs(UnmanagedType.U1)] bool state);
        #endregion

        #region PDF.OCG.OCMD
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCMDCreateFromObj (TRN_Obj ocmd_dict, ref TRN_OCMD result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCMDCreate (TRN_PDFDoc pdfdoc, TRN_Obj ocgs, PDF.OCG.OCMD.VisibilityPolicyType vis_policy, ref TRN_OCMD result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCMDCopy (TRN_OCMD ocmd, ref TRN_OCMD result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCMDGetSDFObj (TRN_OCMD ocmd, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCMDGetOCGs (TRN_OCMD ocmd, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCMDGetVisibilityExpression (TRN_OCMD ocmd, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCMDIsValid(TRN_OCMD ocmd, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCMDIsCurrentlyVisible(TRN_OCMD ocmd, TRN_OCGContext ctx, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCMDGetVisibilityPolicy (TRN_OCMD ocmd, ref PDF.OCG.OCMD.VisibilityPolicyType result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCMDSetVisibilityPolicy(TRN_OCMD ocmd, PDF.OCG.OCMD.VisibilityPolicyType vis_policy);
        #endregion

		#region PDF.Optimizer
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OptimizerImageSettingsInit(TRN_OptimizerImageSettings result);
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OptimizerMonoImageSettingsInit(TRN_OptimizerMonoImageSettings result);
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OptimizerTextSettingsInit(TRN_OptimizerTextSettings result);
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OptimizerOptimize(TRN_PDFDoc doc, TRN_OptimizerImageSettings color_image_settings, TRN_OptimizerImageSettings grayscale_image_settings, TRN_OptimizerMonoImageSettings mono_image_settings, TRN_OptimizerTextSettings text_settings, [MarshalAs(UnmanagedType.U1)] bool remove_custom);
        #endregion

        #region PDF.Page
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageCreate(TRN_Obj page_dict, ref TRN_Page result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageCopy(TRN_Page p, ref TRN_Page result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageIsValid(TRN_Page page, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetIndex(TRN_Page page, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetBox(TRN_Page page, Page.Box type, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSetBox(TRN_Page page, Page.Box type, ref BasicTypes.TRN_Rect box);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetCropBox(TRN_Page page, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSetCropBox(TRN_Page page, ref BasicTypes.TRN_Rect box);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetMediaBox(TRN_Page page, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSetMediaBox(TRN_Page page, ref BasicTypes.TRN_Rect box);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetVisibleContentBox(TRN_Page page, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageAddRotations(Page.Rotate r0, Page.Rotate r1, ref Page.Rotate result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSubtractRotations(Page.Rotate r0, Page.Rotate r1, ref Page.Rotate result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageRotationToDegree(Page.Rotate r, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageDegreeToRotation(int r, ref Page.Rotate result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetRotation(TRN_Page page, ref Page.Rotate result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSetRotation(TRN_Page page, Page.Rotate angle);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetPageWidth(TRN_Page page, Page.Box box_type, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetPageHeight(TRN_Page page, Page.Box box_type, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetDefaultMatrix(TRN_Page page, [MarshalAs(UnmanagedType.U1)] bool flip_y, Page.Box box_type, Page.Rotate angle, ref BasicTypes.TRN_Matrix2D result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetAnnots(TRN_Page page, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetNumAnnots(TRN_Page page, ref uint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetAnnot(TRN_Page page, uint index, ref TRN_Annot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageAnnotInsert(TRN_Page page, uint pos, TRN_Annot annot);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageAnnotPushBack(TRN_Page page, TRN_Annot annot);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageAnnotPushFront(TRN_Page page, TRN_Annot annot);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageAnnotRemove(TRN_Page page, TRN_Annot annot);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageAnnotRemoveByIndex(TRN_Page page, uint index);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageScale(TRN_Page page, double scale);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageFlattenField(TRN_Page page, ref BasicTypes.TRN_Field filed);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageHasTransition(TRN_Page page, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetUserUnitSize(TRN_Page page, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSetUserUnitSize(TRN_Page page, double unit_size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetResourceDict(TRN_Page page, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetContents(TRN_Page page, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetThumb(TRN_Page page, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageGetSDFObj(TRN_Page page, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageFindInheritedAttribute(TRN_Page page, string attrib, ref TRN_Obj result);

        #endregion

        #region PDF.PageLabel
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelCreate(TRN_SDFDoc doc, PageLabel.Style style, TRN_UString prefix, int start_at, ref BasicTypes.TRN_PageLabel result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelCreateFromObj(TRN_Obj l, int first_page, int last_page, ref BasicTypes.TRN_PageLabel result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelAssign(ref BasicTypes.TRN_PageLabel left, ref BasicTypes.TRN_PageLabel right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelCompare(ref BasicTypes.TRN_PageLabel l, ref BasicTypes.TRN_PageLabel d, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelIsValid(ref BasicTypes.TRN_PageLabel l, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelGetLabelTitle(ref BasicTypes.TRN_PageLabel l, int page_num, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelSetStyle(ref BasicTypes.TRN_PageLabel l, PageLabel.Style style);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelGetStyle(ref BasicTypes.TRN_PageLabel l, ref PageLabel.Style result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelGetPrefix(ref BasicTypes.TRN_PageLabel l, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelSetPrefix(ref BasicTypes.TRN_PageLabel l, TRN_UString prefix);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelGetStart(ref BasicTypes.TRN_PageLabel l, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelSetStart(ref BasicTypes.TRN_PageLabel l, int start_at);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelGetFirstPageNum(ref BasicTypes.TRN_PageLabel l, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelGetLastPageNum(ref BasicTypes.TRN_PageLabel l, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageLabelGetSDFObj(ref BasicTypes.TRN_PageLabel l, ref TRN_Obj result);

        #endregion

        #region PDF.PageSet
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSetCreate(ref TRN_PageSet result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSetCreateSinglePage(int one_page, ref TRN_PageSet result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSetCreateRange(ref TRN_PageSet result, int range_start, int range_end);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSetCreateFilteredRange(int range_start, int range_end, PageSet.Filter filter, ref TRN_PageSet result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSetDestroy(TRN_PageSet page_set);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSetAddPage(TRN_PageSet page_set, int one_page);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PageSetAddRange(TRN_PageSet page_set, int range_start, int range_end, PageSet.Filter filter);
        #endregion

        #region PDF.PatternColor
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PatternColorCreate (TRN_Obj pattern, ref TRN_PatternColor result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PatternColorDestroy (TRN_PatternColor pattern);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PatternColorAssign (TRN_PatternColor left, TRN_PatternColor right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PatternColorGetTypeFromObj (TRN_Obj pattern, ref PDF.PatternColor.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PatternColorGetType (TRN_PatternColor pc, ref PDF.PatternColor.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PatternColorGetSDFObj (TRN_PatternColor pc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PatternColorGetMatrix (TRN_PatternColor pc, ref BasicTypes.TRN_Matrix2D result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PatternColorGetShading (TRN_PatternColor pc, ref TRN_Shading result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PatternColorGetTilingType (TRN_PatternColor pc, ref PDF.PatternColor.TilingType result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PatternColorGetBBox (TRN_PatternColor pc, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PatternColorGetXStep (TRN_PatternColor pc, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PatternColorGetYStep (TRN_PatternColor pc, ref Double result);
        #endregion

        #region PDF.PDFA.PDFACompliance

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFAComplianceCreateFromFile([MarshalAs(UnmanagedType.U1)] bool convert, TRN_UString file_path, string password, PDF.PDFA.PDFACompliance.Conformance conf, PDF.PDFA.PDFACompliance.ErrorCode[] exceptions, int exlength, int max_ref_objs, [MarshalAs(UnmanagedType.U1)] bool first_stop, ref TRN_PDFACompliance result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFAComplianceCreateFromBuffer([MarshalAs(UnmanagedType.U1)] bool convert, IntPtr buf, UIntPtr buf_size, string password, PDF.PDFA.PDFACompliance.Conformance conf, PDF.PDFA.PDFACompliance.ErrorCode[] exceptions, int exlength, int max_ref_objs, [MarshalAs(UnmanagedType.U1)] bool first_stop, ref TRN_PDFACompliance result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFAComplianceDestroy (TRN_PDFACompliance pdfac);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFAComplianceGetErrorCount (TRN_PDFACompliance pdfac, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFAComplianceGetError(TRN_PDFACompliance pdfac, int idx, ref PDF.PDFA.PDFACompliance.ErrorCode result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFAComplianceGetRefObjCount(TRN_PDFACompliance pdfac, PDF.PDFA.PDFACompliance.ErrorCode id, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFAComplianceGetRefObj(TRN_PDFACompliance pdfac, PDF.PDFA.PDFACompliance.ErrorCode id, int obj_idx, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFAComplianceGetPDFAErrorMessage(PDF.PDFA.PDFACompliance.ErrorCode id, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFAComplianceSaveAsFromFileName (TRN_PDFACompliance pdfac, TRN_UString file_path, [MarshalAs(UnmanagedType.U1)] bool linearized);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFAComplianceSaveAsFromBuffer(TRN_PDFACompliance pdfac, ref IntPtr result_buf, ref UIntPtr out_buf_size, [MarshalAs(UnmanagedType.U1)] bool linearized);

        #endregion

        #region PDF.PDFDoc
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreate(ref TRN_PDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateFromSDFDoc(TRN_SDFDoc sdfdoc, ref TRN_PDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateFromUFilePath(TRN_UString filepath, ref TRN_PDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateFromFilePath(string filepath, ref TRN_PDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateFromFilter(TRN_Filter stream, ref TRN_PDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateFromBuffer(IntPtr buf, UIntPtr buf_size, ref TRN_PDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocIsEncrypted(TRN_PDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInitSecurityHandler(TRN_PDFDoc doc, IntPtr custom_data, [MarshalAs(UnmanagedType.U1)] ref bool result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_PDFDocInitStdSecurityHandlerUString(TRN_PDFDoc doc, TRN_UString password, [MarshalAs(UnmanagedType.U1)] ref bool result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_PDFDocInitStdSecurityHandlerBuffer(TRN_PDFDoc doc, IntPtr password_buf, UIntPtr password_buf_size, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetSecurityHandler(TRN_PDFDoc doc, ref TRN_SecurityHandler result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocSetSecurityHandler(TRN_PDFDoc doc, TRN_SecurityHandler handler);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocRemoveSecurity(TRN_PDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocHasSignatures(TRN_PDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocAddSignatureHandler(TRN_PDFDoc doc, TRN_SignatureHandler signature_handler, ref SignatureHandlerId result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocAddStdSignatureHandlerFromFile(TRN_PDFDoc doc, TRN_UString pkcs12_file, TRN_UString pkcs12_pass, ref SignatureHandlerId result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocAddStdSignatureHandlerFromBuffer(TRN_PDFDoc doc, IntPtr pkcs12_buffer, UIntPtr pkcs12_buffsize, TRN_UString pkcs12_pass, ref SignatureHandlerId result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocRemoveSignatureHandler(TRN_PDFDoc doc, SignatureHandlerId signature_handler_id);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetSignatureHandler(TRN_PDFDoc doc, SignatureHandlerId signature_handler_id, ref TRN_SignatureHandler result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_PDFDocGenerateThumbnails (TRN_PDFDoc doc, UInt32 size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocAppendVisualDiffWithOptsObj(TRN_PDFDoc doc, TRN_Page p1, TRN_Page p2, TRN_Obj opts);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetGeometryCollectionForPage(TRN_PDFDoc in_pdfdoc, int page_num, ref TRN_GeometryCollection result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetUndoManager(TRN_PDFDoc doc, ref TRN_UndoManager result);
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateDigitalSignatureField(TRN_PDFDoc doc, TRN_UString in_sig_field_name, ref BasicTypes.TRN_DigitalSignatureField result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetDigitalSignatureFieldBegin(TRN_PDFDoc doc, ref TRN_Iterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetDigitalSignaturePermissions(TRN_PDFDoc doc, ref pdftron.PDF.DigitalSignatureField.DocumentPermissions result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocDestroy(TRN_PDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetSDFDoc(TRN_PDFDoc doc, ref TRN_SDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocLock(TRN_PDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocUnlock(TRN_PDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocLockRead(TRN_PDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocUnlockRead(TRN_PDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocTryLock(TRN_PDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocTimedLock(TRN_PDFDoc doc, int milliseconds, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocTryLockRead(TRN_PDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocTimedLockRead(TRN_PDFDoc doc, int milliseconds, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetDocInfo(TRN_PDFDoc doc, ref TRN_PDFDocInfo result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocIsModified(TRN_PDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_PDFDocHasRepairedXRef(TRN_PDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocIsLinearized(TRN_PDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocSave(TRN_PDFDoc doc, TRN_UString path, uint flags);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_PDFDocSaveMemoryBuffer(TRN_PDFDoc doc, uint flags, out IntPtr result_buf, out UIntPtr out_buf_size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocSaveStream(TRN_PDFDoc doc, TRN_Filter stream, uint flags);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetPageIterator(TRN_PDFDoc doc, uint page_number, ref TRN_Iterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetPage(TRN_PDFDoc doc, uint page_number, ref TRN_Page result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocPageRemove(TRN_PDFDoc doc, TRN_Iterator page_itr);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocPageInsert(TRN_PDFDoc doc, TRN_Iterator where, TRN_Page page);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInsertPages(TRN_PDFDoc dest_doc, uint insert_before_page_number, TRN_PDFDoc src_doc, uint start_page, uint end_page, uint flag, TRN_ProgressMonitor progress);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInsertPageSet(TRN_PDFDoc dest_doc, uint insert_before_page_number, TRN_PDFDoc src_doc, TRN_PageSet source_page_set, uint flag, TRN_ProgressMonitor progress);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocMovePages(TRN_PDFDoc dest_doc, uint move_before_page_number, TRN_PDFDoc src_doc, uint start_page, uint end_page, uint flag, TRN_ProgressMonitor progress);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocMovePageSet(TRN_PDFDoc dest_doc, uint move_before_page_number, TRN_PDFDoc src_doc, TRN_PageSet source_page_set, uint flag, TRN_ProgressMonitor progress);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocPagePushFront(TRN_PDFDoc doc, TRN_Page page);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocPagePushBack(TRN_PDFDoc doc, TRN_Page page);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocImportPages(TRN_PDFDoc doc, TRN_Page page_buf, int buf_size, [MarshalAs(UnmanagedType.U1)] bool import_bookmarks, TRN_Page buf_result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocPageCreate(TRN_PDFDoc doc, ref BasicTypes.TRN_Rect media_box, ref TRN_Page result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetFirstBookmark(TRN_PDFDoc doc, ref TRN_Bookmark result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocAddRootBookmark(TRN_PDFDoc doc, TRN_Bookmark root_bookmark);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetTrailer(TRN_PDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetRoot(TRN_PDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetPages(TRN_PDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetPageCount(TRN_PDFDoc doc, ref int count);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetFieldIteratorBegin(TRN_PDFDoc doc, ref TRN_Iterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetFieldIterator(TRN_PDFDoc doc, TRN_UString field_name, ref TRN_Iterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetField(TRN_PDFDoc doc, TRN_UString field_name, ref BasicTypes.TRN_Field result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocFieldCreate(TRN_PDFDoc doc, TRN_UString field_name, Field.Type type, TRN_Obj field_value, TRN_Obj def_field_value, ref BasicTypes.TRN_Field result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocFieldCreateFromStrings(TRN_PDFDoc doc, TRN_UString field_name, Field.Type type, TRN_UString field_value, TRN_UString def_field_value, ref BasicTypes.TRN_Field result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocRefreshFieldAppearances(TRN_PDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocFlattenAnnotations(TRN_PDFDoc doc, [MarshalAs(UnmanagedType.U1)] bool forms_only);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocFlattenAnnotationsAdvanced(TRN_PDFDoc doc, uint flags);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetAcroForm(TRN_PDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocFDFExtract(TRN_PDFDoc doc, uint flag, ref TRN_FDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocFDFExtractAnnots(TRN_PDFDoc doc, IntPtr annot_buf, int buf_size, ref TRN_FDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocFDFExtractCommand(TRN_PDFDoc doc, IntPtr annot_added_buf, int annot_added_buf_size, IntPtr annot_modified_buf, int annot_modified_buf_size, IntPtr annot_deleted_buf, int annot_deleted_buf_size, ref TRN_FDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocFDFMerge(TRN_PDFDoc doc, TRN_FDFDoc fdf_doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocFDFUpdate(TRN_PDFDoc doc, TRN_FDFDoc fdf_doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetOpenAction(TRN_PDFDoc doc, ref TRN_Action result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocSetOpenAction(TRN_PDFDoc doc, TRN_Action action);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocAddFileAttachment(TRN_PDFDoc doc, TRN_UString file_key, TRN_FileSpec embeded_file);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateIndirectName(TRN_PDFDoc doc, string name, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateIndirectArray(TRN_PDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateIndirectBool(TRN_PDFDoc doc, [MarshalAs(UnmanagedType.U1)] bool value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateIndirectDict(TRN_PDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateIndirectNull(TRN_PDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateIndirectNumber(TRN_PDFDoc doc, double value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateIndirectString(TRN_PDFDoc doc, byte[] value, uint size, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateIndirectStringFromUString(TRN_PDFDoc doc, TRN_UString str, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateIndirectStreamFromFilter(TRN_PDFDoc doc, TRN_FilterReader data, TRN_Filter filter_chain, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocCreateIndirectStream(TRN_PDFDoc doc, IntPtr data, UIntPtr data_size, TRN_Filter filter_chain, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetPageLabel(TRN_PDFDoc doc, int page_num, ref BasicTypes.TRN_PageLabel result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocSetPageLabel(TRN_PDFDoc doc, int page_num, ref BasicTypes.TRN_PageLabel label);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocRemovePageLabel(TRN_PDFDoc doc, int page_num);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetStructTree(TRN_PDFDoc doc, ref TRN_STree result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetViewPrefs(TRN_PDFDoc doc, ref TRN_PDFDocViewPrefs result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocHasOC(TRN_PDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetOCGs(TRN_PDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGetOCGConfig(TRN_PDFDoc doc, ref TRN_OCGConfig result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocAddHighlights(TRN_PDFDoc doc, TRN_UString hilite);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocIsTagged(TRN_PDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);
        #endregion

        #region PDF.PDFDocInfo
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetTitle(TRN_PDFDocInfo info, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetTitleObj(TRN_PDFDocInfo info, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoSetTitle(TRN_PDFDocInfo info, TRN_UString title);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetAuthor(TRN_PDFDocInfo info, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetAuthorObj(TRN_PDFDocInfo info, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoSetAuthor(TRN_PDFDocInfo info, TRN_UString author);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetSubject(TRN_PDFDocInfo info, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetSubjectObj(TRN_PDFDocInfo info, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoSetSubject(TRN_PDFDocInfo info, TRN_UString subject);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetKeywords(TRN_PDFDocInfo info, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetKeywordsObj(TRN_PDFDocInfo info, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoSetKeywords(TRN_PDFDocInfo info, TRN_UString keywords);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetCreator(TRN_PDFDocInfo info, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetCreatorObj(TRN_PDFDocInfo info, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoSetCreator(TRN_PDFDocInfo info, TRN_UString creator);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetProducer(TRN_PDFDocInfo info, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetProducerObj(TRN_PDFDocInfo info, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoSetProducer(TRN_PDFDocInfo info, TRN_UString producer);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetCreationDate(TRN_PDFDocInfo info, ref BasicTypes.TRN_Date result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoSetCreationDate(TRN_PDFDocInfo info, ref BasicTypes.TRN_Date creation_date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetModDate(TRN_PDFDocInfo info, ref BasicTypes.TRN_Date result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoSetModDate(TRN_PDFDocInfo info, ref BasicTypes.TRN_Date mod_date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoGetSDFObj(TRN_PDFDocInfo info, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoCreate(TRN_Obj tr, ref TRN_PDFDocInfo result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocInfoCopy(TRN_PDFDocInfo info, ref TRN_PDFDocInfo result);
        #endregion

        #region PDF.PDFDocViewPrefs
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsSetInitialPage(TRN_PDFDocViewPrefs p, TRN_Destination dest);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsSetPageMode(TRN_PDFDocViewPrefs p, PDFDocViewPrefs.PageMode mode);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsGetPageMode(TRN_PDFDocViewPrefs p, ref PDFDocViewPrefs.PageMode result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsSetLayoutMode(TRN_PDFDocViewPrefs p, PDFDocViewPrefs.PageLayout layout);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsGetLayoutMode(TRN_PDFDocViewPrefs p, ref PDFDocViewPrefs.PageLayout result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsSetPref(TRN_PDFDocViewPrefs p, PDFDocViewPrefs.ViewerPref pref, [MarshalAs(UnmanagedType.U1)] bool value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsGetPref(TRN_PDFDocViewPrefs p, PDFDocViewPrefs.ViewerPref pref, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsSetNonFullScreenPageMode(TRN_PDFDocViewPrefs p, PDFDocViewPrefs.PageMode mode);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsGetNonFullScreenPageMode(TRN_PDFDocViewPrefs p, ref PDFDocViewPrefs.PageMode result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsSetDirection(TRN_PDFDocViewPrefs p, [MarshalAs(UnmanagedType.U1)] bool left_to_right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsGetDirection(TRN_PDFDocViewPrefs p, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsSetViewArea(TRN_PDFDocViewPrefs p, Page.Box box);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsGetViewArea(TRN_PDFDocViewPrefs p, ref Page.Box result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsSetViewClip(TRN_PDFDocViewPrefs p, Page.Box box);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsGetViewClip(TRN_PDFDocViewPrefs p, ref Page.Box result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsSetPrintArea(TRN_PDFDocViewPrefs p, Page.Box box);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsGetPrintArea(TRN_PDFDocViewPrefs p, ref Page.Box result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsSetPrintClip(TRN_PDFDocViewPrefs p, Page.Box box);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsGetPrintClip(TRN_PDFDocViewPrefs p, ref Page.Box result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsGetSDFObj(TRN_PDFDocViewPrefs p, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsCreate(TRN_Obj tr, ref TRN_PDFDocViewPrefs result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocViewPrefsCopy(TRN_PDFDocViewPrefs prefs, ref TRN_PDFDocViewPrefs result);

        #endregion

        #region PDF.PDFRasterizer
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerCreate (PDFRasterizer.Type type, ref TRN_PDFRasterizer result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerDestroy (TRN_PDFRasterizer r);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerRasterizeToMemory(TRN_PDFRasterizer r, TRN_Page page, IntPtr in_out_image_buffer, int width, int height, int stride, int num_comps, [MarshalAs(UnmanagedType.U1)] bool demult, ref BasicTypes.TRN_Matrix2D device_mtx, IntPtr clip, IntPtr scrl_clp_regions, IntPtr cancel);

		//[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		//public static extern TRN_Exception TRN_PDFRasterizerRasterizeToDevice(TRN_PDFRasterizer r, TRN_Page page, IntPtr hdc, ref BasicTypes.TRN_Matrix2D device_mtx, ref BasicTypes.TRN_Rect clip, int dpi, [MarshalAs(UnmanagedType.U1)] bool cancel);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetDrawAnnotations(TRN_PDFRasterizer r, [MarshalAs(UnmanagedType.U1)] bool render_annots);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetHighlightFields(TRN_PDFRasterizer r, [MarshalAs(UnmanagedType.U1)] bool highlight);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetAntiAliasing(TRN_PDFRasterizer r, [MarshalAs(UnmanagedType.U1)] bool enable_aa);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetPathHinting(TRN_PDFRasterizer r, [MarshalAs(UnmanagedType.U1)] bool enable_ph);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetThinLineAdjustment(TRN_PDFRasterizer r, [MarshalAs(UnmanagedType.U1)] bool grid_fit, [MarshalAs(UnmanagedType.U1)] bool stroke_adjust);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetGamma (TRN_PDFRasterizer r, double gamma);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetOCGContext (TRN_PDFRasterizer r, TRN_OCGContext ctx);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetPrintMode(TRN_PDFRasterizer r, [MarshalAs(UnmanagedType.U1)] bool is_printing);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetImageSmoothing(TRN_PDFRasterizer r, [MarshalAs(UnmanagedType.U1)] bool smoothing_enabled, bool hq_image_resampling);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetOverprint (TRN_PDFRasterizer r, PDFRasterizer.OverprintPreviewMode op);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetCaching (TRN_PDFRasterizer r, [MarshalAs(UnmanagedType.U1)] bool enabled);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetRasterizerType (TRN_PDFRasterizer r, PDFRasterizer.Type type);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerGetRasterizerType (TRN_PDFRasterizer r, ref PDFRasterizer.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerSetColorPostProcessMode (TRN_PDFRasterizer r, PDFRasterizer.ColorPostProcessMode mode);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFRasterizerGetColorPostProcessMode (TRN_PDFRasterizer r, ref PDFRasterizer.ColorPostProcessMode result);
        #endregion

        #region PDF.Rect
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectInit(double x1, double y1, double x2, double y2, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectAssign(ref BasicTypes.TRN_Rect left, ref BasicTypes.TRN_Rect right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectAttach(ref BasicTypes.TRN_Rect rect, TRN_Obj obj);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectUpdate(ref BasicTypes.TRN_Rect rect, TRN_Obj obj, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectGet(ref BasicTypes.TRN_Rect rect, ref double out_x1, ref double out_y1, ref double out_x2, ref double out_y2);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectSet(ref BasicTypes.TRN_Rect rect, double x1, double y1, double x2, double y2);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TRN_RectSetX1(ref BasicTypes.TRN_Rect rect, double x1);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TRN_RectSetY1(ref BasicTypes.TRN_Rect rect, double y1);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TRN_RectSetX2(ref BasicTypes.TRN_Rect rect, double x2);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void TRN_RectSetY2(ref BasicTypes.TRN_Rect rect, double y2);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern double TRN_RectGetX1(ref BasicTypes.TRN_Rect rect);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern double TRN_RectGetY1(ref BasicTypes.TRN_Rect rect);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern double TRN_RectGetX2(ref BasicTypes.TRN_Rect rect);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern double TRN_RectGetY2(ref BasicTypes.TRN_Rect rect);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectWidth(ref BasicTypes.TRN_Rect rect, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectHeight(ref BasicTypes.TRN_Rect rect, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectContains(ref BasicTypes.TRN_Rect rect, double x, double y, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectIntersectRect(ref BasicTypes.TRN_Rect rect, ref BasicTypes.TRN_Rect rect1, ref BasicTypes.TRN_Rect rect2, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectNormalize(ref BasicTypes.TRN_Rect rect);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectInflate1(ref BasicTypes.TRN_Rect rect, double amount);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_RectInflate2(ref BasicTypes.TRN_Rect rect, double x, double y);

        #endregion

		#region PDF.Redactor
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Redactor_RedactionCreate(int page_num, IntPtr bbox, [MarshalAs(UnmanagedType.U1)] bool negative, TRN_UString text, ref TRN_Redaction result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Redactor_RedactionDestroy(TRN_Redaction redaction);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_Redactor_RedactionCopy(TRN_Redaction other, ref TRN_Redaction result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_RedactionAppearanceCreate([MarshalAs(UnmanagedType.U1)] bool redaction_overlay, TRN_ColorPt positive_overlay_color, TRN_ColorPt negative_overlay_color, [MarshalAs(UnmanagedType.U1)] bool border, [MarshalAs(UnmanagedType.U1)] bool use_overlay_text, TRN_Font font, Double min_font_size, Double max_font_size, TRN_ColorPt text_color, int horiz_text_alignment, int vert_text_alignment, [MarshalAs(UnmanagedType.U1)] bool show_redacted_content_regions, TRN_ColorPt redacted_content_color, ref TRN_RedactionAppearance result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_RedactionAppearanceDestroy(TRN_RedactionAppearance app);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_RedactorRedact(TRN_PDFDoc doc, IntPtr red_arr, int buf_size, TRN_RedactionAppearance appearance, [MarshalAs(UnmanagedType.U1)] bool ext_neg_mode, [MarshalAs(UnmanagedType.U1)] bool page_coord_sys);
        #endregion

        #region PDF.Shading
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingCreate (TRN_Obj shading_dict, ref TRN_Shading result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingAssign (TRN_Shading left, TRN_Shading right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingDestroy (TRN_Shading s);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetTypeFromObj (TRN_Obj shading_dict, ref Shading.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetType (TRN_Shading s, ref Shading.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetSDFObj (TRN_Shading s, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetBaseColorSpace (TRN_Shading s, ref TRN_ColorSpace result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingHasBBox (TRN_Shading s, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetBBox (TRN_Shading s, ref BasicTypes.TRN_Rect result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingHasBackground (TRN_Shading s, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetBackground (TRN_Shading s, TRN_ColorPt result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetAntialias (TRN_Shading s, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetParamStart (TRN_Shading s, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetParamEnd (TRN_Shading s, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingIsExtendStart (TRN_Shading s, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingIsExtendEnd (TRN_Shading s, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetColor(TRN_Shading s, Double t, TRN_ColorPt out_color);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetCoords (TRN_Shading s, ref Double out_x0, ref Double out_y0, ref Double out_x1, ref Double out_y1);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetCoordsRadial (TRN_Shading s, ref Double out_x0, ref Double out_y0, ref Double out_r0, ref Double out_x1, ref Double out_y1, ref Double out_r1);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetDomain (TRN_Shading s, ref Double out_xmin, ref Double out_xmax, ref Double out_ymin, ref Double out_ymax);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetMatrix(TRN_Shading s, ref BasicTypes.TRN_Matrix2D result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ShadingGetColorForFunction(TRN_Shading s, Double t1, Double t2, TRN_ColorPt out_color);
        #endregion

        #region PDF.TextExtractor
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorCreate(ref TRN_TextExtractor result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorDestroy(TRN_TextExtractor te);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorBegin(TRN_TextExtractor te, TRN_Page page, IntPtr clip_ptr, int flags);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorGetWordCount(TRN_TextExtractor te, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorGetAsText(TRN_TextExtractor te, [MarshalAs(UnmanagedType.U1)] bool dehyphen, TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorGetTextUnderAnnot(TRN_TextExtractor te, TRN_Annot annot, TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorGetAsXML(TRN_TextExtractor te, int xml_output_flags, TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorStyleGetFont(ref BasicTypes.TRN_TextExtractorStyle tes, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorStyleGetFontName(ref BasicTypes.TRN_TextExtractorStyle tes, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorStyleGetFontSize(ref BasicTypes.TRN_TextExtractorStyle tes, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorStyleGetWeight(ref BasicTypes.TRN_TextExtractorStyle tes, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorStyleIsItalic(ref BasicTypes.TRN_TextExtractorStyle tes, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorStyleIsSerif(ref BasicTypes.TRN_TextExtractorStyle tes, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorStyleGetColor(ref BasicTypes.TRN_TextExtractorStyle tes, byte[] rgb);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorStyleCompare(ref BasicTypes.TRN_TextExtractorStyle tes, ref BasicTypes.TRN_TextExtractorStyle s, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorStyleCreate(ref BasicTypes.TRN_TextExtractorStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorStyleCopy(ref BasicTypes.TRN_TextExtractorStyle s, ref BasicTypes.TRN_TextExtractorStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordGetNumGlyphs(ref BasicTypes.TRN_TextExtractorWord tew, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordGetBBox(ref BasicTypes.TRN_TextExtractorWord tew, double[] out_bbox);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordGetQuad(ref BasicTypes.TRN_TextExtractorWord tew, double[] out_quad);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordGetGlyphQuad(ref BasicTypes.TRN_TextExtractorWord tew, int glyph_idx, double[] out_quad);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordGetCharStyle(ref BasicTypes.TRN_TextExtractorWord tew, int char_idx, ref BasicTypes.TRN_TextExtractorStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordGetStyle(ref BasicTypes.TRN_TextExtractorWord tew, ref BasicTypes.TRN_TextExtractorStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordGetStringLen(ref BasicTypes.TRN_TextExtractorWord tew, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordGetString(ref BasicTypes.TRN_TextExtractorWord tew, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordGetNextWord(ref BasicTypes.TRN_TextExtractorWord tew, ref BasicTypes.TRN_TextExtractorWord result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordGetCurrentNum(ref BasicTypes.TRN_TextExtractorWord tew, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordCompare(ref BasicTypes.TRN_TextExtractorWord tew, ref BasicTypes.TRN_TextExtractorWord word, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordCreate(ref BasicTypes.TRN_TextExtractorWord result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorWordIsValid(ref BasicTypes.TRN_TextExtractorWord tew, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineGetNumWords(ref BasicTypes.TRN_TextExtractorLine line, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineIsSimpleLine(ref BasicTypes.TRN_TextExtractorLine line, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineGetBBox(ref BasicTypes.TRN_TextExtractorLine line, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineGetQuad(ref BasicTypes.TRN_TextExtractorLine line, double[] out_quad);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineGetFirstWord(ref BasicTypes.TRN_TextExtractorLine line, ref BasicTypes.TRN_TextExtractorWord result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineGetWord(ref BasicTypes.TRN_TextExtractorLine line, int word_idx, ref BasicTypes.TRN_TextExtractorWord result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineGetNextLine(ref BasicTypes.TRN_TextExtractorLine line, ref BasicTypes.TRN_TextExtractorLine result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineGetCurrentNum(ref BasicTypes.TRN_TextExtractorLine line, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineGetStyle(ref BasicTypes.TRN_TextExtractorLine line, ref BasicTypes.TRN_TextExtractorStyle result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineGetParagraphID(ref BasicTypes.TRN_TextExtractorLine line, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineGetFlowID(ref BasicTypes.TRN_TextExtractorLine line, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineEndsWithHyphen(ref BasicTypes.TRN_TextExtractorLine line, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineCompare(ref BasicTypes.TRN_TextExtractorLine line, ref BasicTypes.TRN_TextExtractorLine line2, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineCreate(ref BasicTypes.TRN_TextExtractorLine result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorLineIsValid(ref BasicTypes.TRN_TextExtractorLine line, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorGetNumLines(TRN_TextExtractor te, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextExtractorGetFirstLine(TRN_TextExtractor te, ref BasicTypes.TRN_TextExtractorLine result);
        #endregion

        #region PDF.TextSearch
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextSearchCreate (ref TRN_TextSearch result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextSearchDestroy (TRN_TextSearch ts);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextSearchBegin(TRN_TextSearch ts, TRN_PDFDoc doc, TRN_UString pattern, uint mode, int start_page, int end_page, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextSearchRun (TRN_TextSearch ts, ref int page_num, TRN_UString result_str, TRN_UString ambient_str, TRN_Highlights hlts, ref PDF.TextSearch.ResultCode result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextSearchSetPattern(TRN_TextSearch ts, TRN_UString pattern, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextSearchGetMode (TRN_TextSearch ts, ref uint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_TextSearchSetMode (TRN_TextSearch ts, uint mode);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_TextSearchGetCurrentPage(TRN_TextSearch ts, ref int page);

        #endregion

		#region PDF.ViewChangeCollection
		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ViewChangeCollectionCreate(ref TRN_ViewChangeCollection result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ViewChangeCollectionDestroy(TRN_ViewChangeCollection v);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_ViewChangeCollectionAssign(TRN_ViewChangeCollection left, TRN_ViewChangeCollection right);
		#endregion

		#region PDF.Stamper
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperCreate(PDF.Stamper.SizeType size_type, Double a, Double b, ref TRN_Stamper result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperDestroy (TRN_Stamper stamp);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperStampImage (TRN_Stamper stamp, TRN_PDFDoc dest_doc, TRN_Image img, TRN_PageSet page_set);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperStampPage (TRN_Stamper stamp, TRN_PDFDoc dest_doc, TRN_Page page, TRN_PageSet page_set);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperStampText (TRN_Stamper stamp, TRN_PDFDoc dest_doc, TRN_UString txt, TRN_PageSet page_set);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperSetFont (TRN_Stamper stamp, TRN_Font font);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperSetFontColor(TRN_Stamper stamp, TRN_ColorPt font_color);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperSetTextAlignment (TRN_Stamper stamp, PDF.Stamper.TextAlignment text_alignment);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperSetOpacity (TRN_Stamper stamp, double opacity);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperSetRotation (TRN_Stamper stamp, double rotation);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperSetAsBackground(TRN_Stamper stamp, [MarshalAs(UnmanagedType.U1)] bool background);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperSetAsAnnotation(TRN_Stamper stamp, [MarshalAs(UnmanagedType.U1)] bool annotation);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperShowsOnScreen(TRN_Stamper stamp, [MarshalAs(UnmanagedType.U1)] bool on_screen);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperShowsOnPrint(TRN_Stamper stamp, [MarshalAs(UnmanagedType.U1)] bool on_print);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperSetAlignment (TRN_Stamper stamp, PDF.Stamper.HorizontalAlignment horizontal_alignment, PDF.Stamper.VerticalAlignment vertical_alignment);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperSetPosition(TRN_Stamper stamp, double x, double y, [MarshalAs(UnmanagedType.U1)] bool percentage);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperSetSize (TRN_Stamper stamp, PDF.Stamper.SizeType size_type, double a, double b);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperDeleteStamps (TRN_PDFDoc doc, TRN_PageSet page_set);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_StamperHasStamps(TRN_PDFDoc doc, TRN_PageSet page_set, [MarshalAs(UnmanagedType.U1)] ref bool result);

        #endregion

        #region PDF.Struct.ContentItem
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentItemCopy(ref BasicTypes.TRN_ContentItem c, ref BasicTypes.TRN_ContentItem result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentItemGetType (ref BasicTypes.TRN_ContentItem item, ref PDF.Struct.ContentItem.Type result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentItemGetParent(ref BasicTypes.TRN_ContentItem item, ref BasicTypes.TRN_SElement result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentItemGetPage (ref BasicTypes.TRN_ContentItem item, ref TRN_Page result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentItemGetSDFObj (ref BasicTypes.TRN_ContentItem item, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentItemGetMCID (ref BasicTypes.TRN_ContentItem item, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentItemGetContainingStm (ref BasicTypes.TRN_ContentItem item, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentItemGetStmOwner (ref BasicTypes.TRN_ContentItem item, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ContentItemGetRefObj (ref BasicTypes.TRN_ContentItem item, ref TRN_Obj result);

        #endregion

        #region PDF.Struct.SElement
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementCreate(TRN_Obj dict, ref BasicTypes.TRN_SElement result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementAssign(ref BasicTypes.TRN_SElement left, ref BasicTypes.TRN_SElement right);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementCreateFromPDFDoc(TRN_PDFDoc doc, string struct_type, ref BasicTypes.TRN_SElement result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementInsert(ref BasicTypes.TRN_SElement e, ref BasicTypes.TRN_SElement kid, int insert_before);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementCreateContentItem(ref BasicTypes.TRN_SElement e, TRN_PDFDoc doc, TRN_Page page, int insert_before, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementIsValid(ref BasicTypes.TRN_SElement e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementGetType(ref BasicTypes.TRN_SElement e, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementGetNumKids(ref BasicTypes.TRN_SElement e, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementIsContentItem(ref BasicTypes.TRN_SElement e, int index, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementGetAsContentItem(ref BasicTypes.TRN_SElement e, int index, ref BasicTypes.TRN_ContentItem result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementGetAsStructElem(ref BasicTypes.TRN_SElement e, int index, ref BasicTypes.TRN_SElement result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementGetParent(ref BasicTypes.TRN_SElement e, ref BasicTypes.TRN_SElement result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementGetStructTreeRoot(ref BasicTypes.TRN_SElement e, ref TRN_STree result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementHasTitle(ref BasicTypes.TRN_SElement e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementGetTitle(ref BasicTypes.TRN_SElement e, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementGetID(ref BasicTypes.TRN_SElement e, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementHasActualText(ref BasicTypes.TRN_SElement e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementGetActualText(ref BasicTypes.TRN_SElement e, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementHasAlt(ref BasicTypes.TRN_SElement e, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementGetAlt(ref BasicTypes.TRN_SElement e, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SElementGetSDFObj(ref BasicTypes.TRN_SElement e, ref TRN_Obj result);

        #endregion

        #region PDF.Struct.STree
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_STreeCreate(TRN_Obj struct_dict, ref TRN_STree result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_STreeCreateFromPDFDoc(TRN_PDFDoc doc, ref TRN_STree result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_STreeInsert(TRN_STree tree, ref BasicTypes.TRN_SElement kid, int insert_before);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_STreeCopy(TRN_STree c, ref TRN_STree result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_STreeIsValid(TRN_STree tree, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_STreeGetNumKids(TRN_STree tree, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_STreeGetKid(TRN_STree tree, int index, ref BasicTypes.TRN_SElement result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_STreeGetElement(TRN_STree tree, byte[] id_buf, int id_buf_sz, ref BasicTypes.TRN_SElement result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_STreeGetRoleMap(TRN_STree tree, ref TRN_RoleMap result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_STreeGetClassMap(TRN_STree tree, ref TRN_ClassMap result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_STreeGetSDFObj(TRN_STree tree, ref TRN_Obj result);
        #endregion

        #region SDF.NameTree
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NameTreeCreate(TRN_SDFDoc doc, string name, ref TRN_NameTree result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NameTreeFind(TRN_SDFDoc doc, string name, ref TRN_NameTree result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NameTreeCreateFromObj(TRN_Obj name_tree, ref TRN_NameTree result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NameTreeCopy(TRN_NameTree d, ref TRN_NameTree result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NameTreeIsValid(TRN_NameTree tree, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NameTreeGetIterator(TRN_NameTree tree, BasicTypes.TRN_String key, ref TRN_DictIterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NameTreeGetValue(TRN_NameTree tree, BasicTypes.TRN_String key, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NameTreeGetIteratorBegin(TRN_NameTree tree, ref TRN_DictIterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NameTreePut(TRN_NameTree tree, byte[] key, int key_sz, TRN_Obj value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NameTreeEraseKey(TRN_NameTree tree, byte[] key, int key_sz);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NameTreeErase(TRN_NameTree tree, TRN_DictIterator pos);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NameTreeGetSDFObj(TRN_NameTree tree, ref TRN_Obj result);
        #endregion

        #region SDF.NumberTree
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NumberTreeCreate(TRN_Obj number_tree, ref TRN_NumberTree result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NumberTreeCopy(TRN_NumberTree tree, ref TRN_NumberTree result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NumberTreeIsValid(TRN_NumberTree tree, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NumberTreeGetIterator(TRN_NumberTree tree, int key, ref TRN_DictIterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NumberTreeGetValue(TRN_NumberTree tree, int key, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NumberTreeGetIteratorBegin(TRN_NumberTree tree, ref TRN_DictIterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NumberTreePut(TRN_NumberTree tree, int key, TRN_Obj value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NumberTreeEraseKey(TRN_NumberTree tree, int key);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NumberTreeErase(TRN_NumberTree tree, TRN_DictIterator pos);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_NumberTreeGetSDFObj(TRN_NumberTree tree, ref TRN_Obj result);
        #endregion

        #region SDF.Obj
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetType(TRN_Obj o, ref Obj.ObjType result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetDoc(TRN_Obj o, ref TRN_SDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjWrite(TRN_Obj o, TRN_FilterWriter stream);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsEqual(TRN_Obj o, TRN_Obj to, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsBool(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetBool(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetBool(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] bool b);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsNumber(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetNumber(TRN_Obj o, ref Double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetNumber(TRN_Obj o, Double n);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsNull(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsString(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetBuffer(TRN_Obj o, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetAsPDFText(TRN_Obj o, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetString(TRN_Obj o, BasicTypes.TRN_String value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetUString(TRN_Obj o, TRN_UString value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsName(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetName(TRN_Obj o, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetName(TRN_Obj o, string name);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsIndirect(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetObjNum(TRN_Obj o, ref uint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetGenNum(TRN_Obj o, ref UInt16 result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetOffset(TRN_Obj o, ref UIntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsFree(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetMark(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] bool mark);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsMarked(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsLoaded(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsContainer(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSize(TRN_Obj o, ref UIntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetDictIterator(TRN_Obj o, ref TRN_DictIterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsDict(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjFind(TRN_Obj o, string key, ref TRN_DictIterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjFindObj(TRN_Obj o, string key, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGet(TRN_Obj o, string key, ref TRN_DictIterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPutName(TRN_Obj o, string key, string name, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPutArray(TRN_Obj o, string key, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPutBool(TRN_Obj o, string key, [MarshalAs(UnmanagedType.U1)] bool value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPutDict(TRN_Obj o, string key, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPutNumber(TRN_Obj o, string key, Double value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPutString(TRN_Obj o, string key, string value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPutStringWithSize(TRN_Obj o, string key, IntPtr value, int size, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPutText(TRN_Obj o, string key, TRN_UString t, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPutNull(TRN_Obj o, string key);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPut(TRN_Obj o, string key, TRN_Obj input_obj, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPutRect(TRN_Obj o, string key, Double x1, Double y1, Double x2, Double y2, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPutMatrix(TRN_Obj o, string key, ref BasicTypes.TRN_Matrix2D mtx, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjEraseFromKey(TRN_Obj o, string key);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjErase(TRN_Obj o, TRN_DictIterator pos);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjRename(TRN_Obj o, string old_key, string new_key, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsArray(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetAt(TRN_Obj o, UIntPtr index, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjInsertName(TRN_Obj o, UIntPtr pos, string name, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjInsertArray(TRN_Obj o, UIntPtr pos, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjInsertBool(TRN_Obj o, UIntPtr pos, [MarshalAs(UnmanagedType.U1)] bool value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjInsertDict(TRN_Obj o, UIntPtr pos, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjInsertNumber(TRN_Obj o, UIntPtr pos, Double value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjInsertString(TRN_Obj o, UIntPtr pos, string value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjInsertStringWithSize(TRN_Obj o, UIntPtr pos, IntPtr value, int size, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjInsertText(TRN_Obj o, UIntPtr pos, TRN_UString t, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjInsertNull(TRN_Obj o, UIntPtr pos, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjInsert(TRN_Obj o, UIntPtr pos, TRN_Obj input_obj, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjInsertRect(TRN_Obj o, UIntPtr pos, Double x1, Double y1, Double x2, Double y2, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjInsertMatrix(TRN_Obj o, UIntPtr pos, ref BasicTypes.TRN_Matrix2D mtx, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPushBackName(TRN_Obj o, string name, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPushBackArray(TRN_Obj o, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPushBackBool(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] bool value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPushBackDict(TRN_Obj o, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPushBackNumber(TRN_Obj o, Double value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPushBackString(TRN_Obj o, string value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPushBackStringWithSize(TRN_Obj o, IntPtr value, int size, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPushBackText(TRN_Obj o, TRN_UString t, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPushBackNull(TRN_Obj o, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPushBack(TRN_Obj o, TRN_Obj input_obj, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPushBackRect(TRN_Obj o, Double x1, Double y1, Double x2, Double y2, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjPushBackMatrix(TRN_Obj o, ref BasicTypes.TRN_Matrix2D mtx, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjEraseAt(TRN_Obj o, UIntPtr pos);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjIsStream(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetRawStreamLength(TRN_Obj o, ref UIntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetStreamData(TRN_Obj obj, string data, uint data_size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetStreamDataWithFilter(TRN_Obj obj, IntPtr data, uint data_size, TRN_Filter filter_chain);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetRawStream(TRN_Obj o, [MarshalAs(UnmanagedType.U1)] bool decrypt, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjGetDecodedStream(TRN_Obj o, ref TRN_Filter result);
        #endregion

        #region SDF.ObjSet
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetCreate(ref TRN_ObjSet result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetDestroy(TRN_ObjSet set);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetCreateName(TRN_ObjSet set, string name, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetCreateArray(TRN_ObjSet set, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetCreateBool(TRN_ObjSet set, [MarshalAs(UnmanagedType.U1)] bool value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetCreateDict(TRN_ObjSet set, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetCreateNull(TRN_ObjSet set, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetCreateNumber(TRN_ObjSet set, Double value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ObjSetCreateString(TRN_ObjSet set, TRN_UString value, ref TRN_Obj result);
        #endregion

        #region SDF.SDFDoc
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreate(ref TRN_SDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateFromFileUString(TRN_UString filepath, ref TRN_SDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateFromFileString(string filepath, ref TRN_SDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateFromFilter(TRN_Filter stream, ref TRN_SDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateFromMemoryBuffer(IntPtr buf, UIntPtr buf_size, ref TRN_SDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocDestroy(TRN_SDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocReleaseFileHandles(TRN_SDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocIsEncrypted(TRN_SDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocInitSecurityHandler(TRN_SDFDoc doc, IntPtr custom_data, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocInitStdSecurityHandler(TRN_SDFDoc doc, string password, int password_sz, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocIsModified(TRN_SDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_SDFDocHasRepairedXRef(TRN_SDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocIsFullSaveRequired(TRN_SDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocGetTrailer(TRN_SDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocGetObj(TRN_SDFDoc doc, uint obj_num, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocImportObj(TRN_SDFDoc doc, TRN_Obj obj, [MarshalAs(UnmanagedType.U1)] bool deep_copy, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocImportObjs(TRN_SDFDoc doc, ref TRN_Obj obj_list, int length, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocXRefSize(TRN_SDFDoc doc, ref uint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocClearMarks(TRN_SDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocSave(TRN_SDFDoc doc, TRN_UString path, uint flags, TRN_ProgressMonitor progress, string header);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocSaveMemory(TRN_SDFDoc doc, ref IntPtr out_buf, ref UIntPtr out_buf_size, uint flags, TRN_ProgressMonitor progress, string header);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocSaveStream(TRN_SDFDoc doc, TRN_Filter stream, uint flags, string header);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocGetHeader(TRN_SDFDoc doc, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocGetSecurityHandler(TRN_SDFDoc doc, ref TRN_SecurityHandler result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocSetSecurityHandler(TRN_SDFDoc doc, TRN_SecurityHandler handler);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocRemoveSecurity(TRN_SDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocSwap(TRN_SDFDoc doc, uint obj_num1, uint obj_num2);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocIsLinearized(TRN_SDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocGetLinearizationDict(TRN_SDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocGetHintStream(TRN_SDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocEnableDiskCaching(TRN_SDFDoc doc, [MarshalAs(UnmanagedType.U1)] bool use_cache_flag);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocLock(TRN_SDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocUnlock(TRN_SDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocLockRead(TRN_SDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocUnlockRead(TRN_SDFDoc doc);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocTryLock(TRN_SDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocTimedLock(TRN_SDFDoc doc, int milliseconds, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocTryLockRead(TRN_SDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocTimedLockRead(TRN_SDFDoc doc, int milliseconds, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocGetFileName(TRN_SDFDoc doc, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateIndirectName(TRN_SDFDoc doc, string name, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateIndirectArray(TRN_SDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateIndirectBool(TRN_SDFDoc doc, [MarshalAs(UnmanagedType.U1)] bool value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateIndirectDict(TRN_SDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateIndirectNull(TRN_SDFDoc doc, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateIndirectNumber(TRN_SDFDoc doc, double value, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateIndirectString(TRN_SDFDoc doc, byte[] value, uint size, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateIndirectStringFromUString(TRN_SDFDoc doc, TRN_UString str, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateIndirectStreamFromFilter(TRN_SDFDoc doc, TRN_FilterReader data, TRN_Filter filter_chain, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SDFDocCreateIndirectStream(TRN_SDFDoc doc, IntPtr data, uint data_size, TRN_Filter filter_chain, ref TRN_Obj result);
        #endregion

        #region SDF.SecurityHandler
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerClone (TRN_SecurityHandler sh, ref TRN_SecurityHandler result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerDestroy (TRN_SecurityHandler sh);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerInitialize (TRN_SecurityHandler sh, TRN_SDFDoc doc, TRN_Obj encrypt_dict, ref IntPtr custom_data);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerGetPermission (TRN_SecurityHandler sh, SDF.SecurityHandler.Permission p, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerGetKeyLength (TRN_SecurityHandler sh, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerGetEncryptionAlgorithmID (TRN_SecurityHandler sh, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerGetHandlerDocName (TRN_SecurityHandler sh, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerIsModified (TRN_SecurityHandler sh, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerSetModified (TRN_SecurityHandler sh, [MarshalAs(UnmanagedType.U1)] bool is_modified);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerCreate (SDF.SecurityHandler.AlgorithmType crypt_type, ref TRN_SecurityHandler result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerCreateFromEncCode (string name, int key_len, int enc_code, ref TRN_SecurityHandler result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerCreateDefault (ref TRN_SecurityHandler result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerChangeUserPassword (TRN_SecurityHandler sh, string password);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerChangeUserPasswordNonAscii (TRN_SecurityHandler sh, string password, UIntPtr pwd_length);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerGetUserPassword (TRN_SecurityHandler sh, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerGetUserPasswordSize (TRN_SecurityHandler sh, ref uint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerChangeMasterPassword (TRN_SecurityHandler sh, string password);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerChangeMasterPasswordNonAscii (TRN_SecurityHandler sh, string password, UIntPtr pwd_length);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerGetMasterPassword (TRN_SecurityHandler sh, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerGetMasterPasswordSize (TRN_SecurityHandler sh, ref uint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerSetPermission(TRN_SecurityHandler sh, SDF.SecurityHandler.Permission perm, [MarshalAs(UnmanagedType.U1)] bool value);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerChangeRevisionNumber (TRN_SecurityHandler sh, int rev_num);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerSetEncryptMetadata(TRN_SecurityHandler sh, [MarshalAs(UnmanagedType.U1)] bool encrypt_metadata);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerGetRevisionNumber (TRN_SecurityHandler sh, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerIsUserPasswordRequired (TRN_SecurityHandler sh, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerIsMasterPasswordRequired (TRN_SecurityHandler sh, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerIsAES (TRN_SecurityHandler sh, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerIsAESObj (TRN_SecurityHandler sh, TRN_Obj stream, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerIsRC4 (TRN_SecurityHandler sh, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerInitPassword (TRN_SecurityHandler sh, string password);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerInitPasswordNonAscii (TRN_SecurityHandler sh, string password, UIntPtr pwd_length);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerAuthorize (TRN_SecurityHandler sh, SDF.SecurityHandler.Permission req_opr, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerAuthorizeFailed (TRN_SecurityHandler sh);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerGetAuthorizationData (TRN_SecurityHandler sh, SDF.SecurityHandler.Permission req_opr, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerEditSecurityData (TRN_SecurityHandler sh, TRN_SDFDoc doc, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SecurityHandlerFillEncryptDict (TRN_SecurityHandler sh, TRN_SDFDoc doc, ref TRN_Obj result);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_SecurityHandlerSetDerived(IntPtr sh, IntPtr derived, [MarshalAs(UnmanagedType.FunctionPtr)]TRN_SecurityHandlerDerivedDestroyProc d, [MarshalAs(UnmanagedType.FunctionPtr)]TRN_SecurityHandlerDerivedCloneProc c, [MarshalAs(UnmanagedType.FunctionPtr)]TRN_SecurityHandlerAuthProc a, [MarshalAs(UnmanagedType.FunctionPtr)]TRN_SecurityHandlerAuthFailedProc af, [MarshalAs(UnmanagedType.FunctionPtr)]TRN_SecurityHandlerGetAuthDataProc ga, [MarshalAs(UnmanagedType.FunctionPtr)]TRN_SecurityHandlerEditSecurDataProc es, [MarshalAs(UnmanagedType.FunctionPtr)]TRN_SecurityHandlerFillEncryptDictProc fd);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_SecurityHandlerGetDerived(IntPtr sh, ref IntPtr derived, [MarshalAs(UnmanagedType.FunctionPtr)] ref TRN_SecurityHandlerDerivedDestroyProc d, [MarshalAs(UnmanagedType.FunctionPtr)] ref TRN_SecurityHandlerDerivedCloneProc c, [MarshalAs(UnmanagedType.FunctionPtr)] ref TRN_SecurityHandlerAuthProc a, [MarshalAs(UnmanagedType.FunctionPtr)] ref TRN_SecurityHandlerAuthFailedProc af, [MarshalAs(UnmanagedType.FunctionPtr)] ref TRN_SecurityHandlerGetAuthDataProc ga, [MarshalAs(UnmanagedType.FunctionPtr)] ref TRN_SecurityHandlerEditSecurDataProc es, [MarshalAs(UnmanagedType.FunctionPtr)] ref TRN_SecurityHandlerFillEncryptDictProc fd);

        /*[UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate TRN_Exception TRN_SecurityHandlerDerivedDestroyProc(IntPtr derived);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate TRN_Exception TRN_SecurityHandlerDerivedCloneProc(IntPtr derived, TRN_SecurityHandler baseHandler);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate TRN_Exception TRN_SecurityHandlerAuthProc(IntPtr derived, SecurityHandler.Permission p);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate TRN_Exception TRN_SecurityHandlerAuthFailedProc(IntPtr derived);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate TRN_Exception TRN_SecurityHandlerGetAuthDataProc(IntPtr derived, SecurityHandler.Permission p);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate TRN_Exception TRN_SecurityHandlerEditSecurDataProc(IntPtr derived, TRN_SDFDoc doc);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate TRN_Exception TRN_SecurityHandlerFillEncryptDictProc(IntPtr derived, TRN_SDFDoc doc);*/
        #endregion

        #region SDF.SignatureHandler
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate TRN_Exception TRN_SignatureHandlerGetNameFunction(ref TRN_UString name, IntPtr unused);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate TRN_Exception TRN_SignatureHandlerAppendDataFunction(BasicTypes.TRN_SignatureData data, IntPtr unused);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate TRN_Exception TRN_SignatureHandlerResetFunction([MarshalAs(UnmanagedType.U1)] ref bool result, IntPtr unused);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate TRN_Exception TRN_SignatureHandlerCreateSignatureFunction(ref BasicTypes.TRN_SignatureData signature, IntPtr unused);
        [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
        public delegate TRN_Exception TRN_SignatureHandlerDestructorFunction(IntPtr unused);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_TestRegisterFunctionCallback([MarshalAs(UnmanagedType.FunctionPtr)]TRN_SignatureHandlerGetNameFunction get_name);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_TestInvokeCallback(ref TRN_UString name);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureHandlerCreate([MarshalAs(UnmanagedType.FunctionPtr)]TRN_SignatureHandlerGetNameFunction get_name, [MarshalAs(UnmanagedType.FunctionPtr)]TRN_SignatureHandlerAppendDataFunction append_data, [MarshalAs(UnmanagedType.FunctionPtr)]TRN_SignatureHandlerResetFunction reset, [MarshalAs(UnmanagedType.FunctionPtr)]TRN_SignatureHandlerCreateSignatureFunction create_signature, [MarshalAs(UnmanagedType.FunctionPtr)]TRN_SignatureHandlerDestructorFunction destructor, IntPtr unused, ref TRN_SignatureHandler signature_handler);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureHandlerDestroy(TRN_SignatureHandler signature_handler, ref IntPtr unused);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureHandlerGetName(TRN_SignatureHandler signature_handler, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureHandlerAppendData(TRN_SignatureHandler signature_handler, TRN_Exception data);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureHandlerReset(TRN_SignatureHandler signature_handler, ref byte result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureHandlerCreateSignature(TRN_SignatureHandler signature_handler, ref TRN_Exception result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureHandlerDestructor(TRN_SignatureHandler signature_handler);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_SignatureHandlerGetUserImpl(TRN_SignatureHandler signature_handler, ref IntPtr result);
        #endregion

        #region trn.Iterator
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_IteratorHasNext(TRN_Iterator itr, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_IteratorCurrent(TRN_Iterator itr, ref TRN_ItrData result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_IteratorNext(TRN_Iterator itr);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_IteratorAssign(TRN_Iterator right, ref TRN_Iterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_IteratorDestroy(TRN_Iterator itr);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DictIteratorHasNext(TRN_DictIterator itr, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DictIteratorKey(TRN_DictIterator itr, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DictIteratorValue(TRN_DictIterator itr, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DictIteratorNext(TRN_DictIterator itr);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DictIteratorAssign(TRN_DictIterator right, ref TRN_DictIterator result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DictIteratorDestroy(TRN_DictIterator itr);

        #endregion

        #region PDF.Convert

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertFromXps(TRN_PDFDoc in_pdfdoc, TRN_UString in_filename);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertFromXpsMem(TRN_PDFDoc in_pdfdoc, byte[] buf, uint buf_sz);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertFromEmf(TRN_PDFDoc in_pdfdoc, TRN_UString in_filename);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertPageToEmf(TRN_Page in_page, TRN_UString in_filename);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertDocToEmf(TRN_PDFDoc in_pdfdoc, TRN_UString in_filename);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertFromText(TRN_PDFDoc in_pdfdoc, TRN_UString in_filename, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertPageToSvg(TRN_Page in_page, TRN_UString in_filename);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertPageToSvgWithOptions(TRN_Page in_page, TRN_UString in_filename, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertDocToSvg(TRN_PDFDoc in_pdfdoc, TRN_UString in_filename);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertDocToSvgWithOptions(TRN_PDFDoc in_pdfdoc, TRN_UString in_filename, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertToXps(TRN_PDFDoc in_pdfdoc, TRN_UString in_filename, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertFileToXps(TRN_UString in_inputFilename, TRN_UString in_outputFilename, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertFileToXod(TRN_UString in_filename, TRN_UString out_filename, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertToXod(TRN_PDFDoc in_pdfdoc, TRN_UString out_filename, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertFileToXodStream(TRN_UString in_filename, TRN_Obj options, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertToXodStream(TRN_PDFDoc in_pdfdoc, TRN_Obj options, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertToXodWithMonitor(TRN_PDFDoc in_pdfdoc, TRN_Obj options, ref TRN_ConversionMonitor result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConversionMonitorNext(TRN_ConversionMonitor conversionMonitor, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConversionMonitorReady(TRN_ConversionMonitor conversionMonitor, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConversionMonitorProgress(TRN_ConversionMonitor conversionMonitor, ref UInt32 result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConversionMonitorFilter(TRN_ConversionMonitor conversionMonitor, ref TRN_Filter result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConversionMonitorDestroy(TRN_ConversionMonitor conversionMonitor);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertWordToPdf(TRN_PDFDoc in_pdfdoc, TRN_UString in_filename, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertWordToPdfConversion(ref TRN_PDFDoc in_pdfdoc, TRN_UString in_filename, TRN_Obj options, ref TRN_DocumentConversion result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertWordToPdfWithFilter(TRN_PDFDoc in_pdfdoc, TRN_Filter no_own_in_data, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertWordToPdfConversionWithFilter(ref TRN_PDFDoc in_pdfdoc, TRN_Filter no_own_in_data, TRN_Obj options, ref TRN_DocumentConversion result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertOfficeToPdfWithPath(TRN_PDFDoc in_pdfdoc, TRN_UString in_filename, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertStreamingPdfConversionWithPath(TRN_UString in_filename, TRN_Obj options, ref TRN_DocumentConversion result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertStreamingPdfConversionWithPdfAndPath(ref TRN_PDFDoc in_pdfdoc, TRN_UString in_filename, TRN_Obj options, ref TRN_DocumentConversion result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertOfficeToPdfWithFilter(TRN_PDFDoc in_pdfdoc, TRN_Filter no_own_in_data, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertStreamingPdfConversionWithFilter(TRN_Filter no_own_in_data, TRN_Obj options, ref TRN_DocumentConversion result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertStreamingPdfConversionWithPdfAndFilter(ref TRN_PDFDoc in_pdfdoc, TRN_Filter no_own_in_data, TRN_Obj options, ref TRN_DocumentConversion result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertToPdf(TRN_PDFDoc in_pdfdoc, TRN_UString in_filename);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertFromCAD(TRN_PDFDoc in_pdfdoc, TRN_UString in_filename, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertRequiresPrinter(TRN_UString in_filename, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertPrinterInstall(TRN_UString in_printerName);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertPrinterUninstall();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertPrinterGetPrinterName(ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertPrinterSetPrinterName(TRN_UString in_printerName);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertPrinterIsInstalled(TRN_UString in_printerName, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertPrinterSetMode(pdftron.PDF.Convert.Printer.Mode mode);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertPrinterGetMode(ref pdftron.PDF.Convert.Printer.Mode result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertFileToHtml(TRN_UString in_filename, TRN_UString out_filename, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertToHtml(TRN_PDFDoc in_pdfdoc, TRN_UString out_path, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertFileToEpub(TRN_UString in_filename, TRN_UString out_path, TRN_Obj html_options, TRN_Obj epub_options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertToEpub(TRN_PDFDoc in_pdfdoc, TRN_UString out_path, TRN_Obj html_options, TRN_Obj epub_options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertFileToTiff(TRN_UString in_filename, TRN_UString out_filename, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ConvertToTiff(TRN_PDFDoc in_pdfdoc, TRN_UString out_path, TRN_Obj options);


        #endregion

        #region trn.UString
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UStringConvertToUtf8(IntPtr str, IntPtr in_out_buf, int buf_sz, [MarshalAs(UnmanagedType.U1)] bool null_term, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UStringConvertToAscii(IntPtr str, IntPtr in_out_buf, int buf_sz, [MarshalAs(UnmanagedType.U1)] bool null_term, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UStringCreate(ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UStringDestroy(TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UStringCreateFromCharString(byte[] buf, int buf_sz, UString.TextEncoding enc, ref TRN_UString result);

		[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
		public static extern TRN_Exception TRN_UStringCreateFromSubstring(IntPtr input, int length, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UStringGetLength(TRN_UString str, ref int result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UStringIsEmpty(TRN_UString str, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UStringGetBuffer(TRN_UString str, ref IntPtr result); // TRN_Unicode

        #endregion
        /* {{codegen:PDFNetInternalTools pinvoke decl}} */
        #region PDFNetInternalTools
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsIsLogSystemAvailable( [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsConfigureLogFromJsonString(TRN_UString config_string,  [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsGetDefaultConfigFile(ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsRunUniversalConversionTests(TRN_UString path_with_docs, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsLogMessage(PDFNetInternalToolsLogLevel threshold, TRN_UString message, TRN_UString filename, uint line_number);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsLogStreamMessage(PDFNetInternalToolsLogLevel threshold, TRN_UString stream, TRN_UString message, TRN_UString filename, uint line_number);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsSetLogLocation(TRN_UString log_directory, TRN_UString log_filename,  [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsSetLogFileName(TRN_UString log_filename,  [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsSetThresholdForLogStream(TRN_UString stream_name, PDFNetInternalToolsLogLevel stream_threshold);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsSetDefaultLogThreshold(PDFNetInternalToolsLogLevel threshold);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsSetCutoffLogThreshold(PDFNetInternalToolsLogLevel threshold);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsEnableLogBackend(PDFNetInternalToolsLogBackend backend,  [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsDisableLogBackend(PDFNetInternalToolsLogBackend backend);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFNetInternalToolsGetPDFViewTileSummary(ref TRN_UString result);

        //[DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        //public static extern TRN_Exception TRN_PDFNetInternalToolsCheckDocIntegrity(TRN_PDFDoc doc,  [MarshalAs(UnmanagedType.U1)] ref bool result);

        #endregion
        /* {{codegen:PDFNetInternalTools pinvoke decl}} */
        /* {{codegen:WebFontDownloader pinvoke decl}} */
        #region WebFontDownloader
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WebFontDownloaderIsAvailable( [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WebFontDownloaderEnableDownloads();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WebFontDownloaderDisableDownloads();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WebFontDownloaderPreCacheAsync();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WebFontDownloaderClearCache();

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_WebFontDownloaderSetCustomWebFontURL(TRN_UString url);

        #endregion
        /* {{codegen:WebFontDownloader pinvoke decl}} */
        /* {{codegen:DocumentConversion pinvoke decl}} */
        #region DocumentConversion
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionTryConvert(TRN_DocumentConversion self, ref DocumentConversionResult result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionConvert(TRN_DocumentConversion self);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionConvertNextPage(TRN_DocumentConversion self);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionGetDoc(TRN_DocumentConversion self, ref TRN_PDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionGetConversionStatus(TRN_DocumentConversion self, ref DocumentConversionResult result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionCancelConversion(TRN_DocumentConversion self);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionIsCancelled(TRN_DocumentConversion self,  [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionHasProgressTracking(TRN_DocumentConversion self,  [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionGetProgress(TRN_DocumentConversion self, ref double result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionGetProgressLabel(TRN_DocumentConversion self, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionGetNumConvertedPages(TRN_DocumentConversion self, ref uint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionGetErrorString(TRN_DocumentConversion self, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionGetNumWarnings(TRN_DocumentConversion self, ref uint result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionGetWarningString(TRN_DocumentConversion self, uint index, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocumentConversionDestroy(ref TRN_DocumentConversion self);

        #endregion
        /* {{codegen:DocumentConversion pinvoke decl}} */
        /* {{codegen:PDFDocGenerator pinvoke decl}} */
        #region PDFDocGenerator
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGeneratorGenerateBlankPaperDoc(double width, double height, double background_red, double background_green, double background_blue, ref TRN_PDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGeneratorGenerateGridPaperDoc(double width, double height, double grid_spacing, double line_thickness, double red, double green, double blue, double background_red, double background_green, double background_blue, ref TRN_PDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGeneratorGenerateLinedPaperDoc(double width, double height, double line_spacing, double line_thickness, double red, double green, double blue, double left_margin_distance, double left_margin_red, double left_margin_green, double left_margin_blue, double right_margin_red, double right_margin_green, double right_margin_blue, double background_red, double background_green, double background_blue, double top_margin_distance, double bottom_margin_distance, ref TRN_PDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGeneratorGenerateGraphPaperDoc(double width, double height, double grid_spacing, double line_thickness, double weighted_line_thickness, int weighted_line_freq, double red, double green, double blue, double background_red, double background_green, double background_blue, ref TRN_PDFDoc result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_PDFDocGeneratorGenerateMusicPaperDoc(double width, double height, double margin, int staves, double linespace_size_pts, double line_thickness, double red, double green, double blue, double background_red, double background_green, double background_blue, ref TRN_PDFDoc result);

        #endregion
        /* {{codegen:PDFDocGenerator pinvoke decl}} */
        /* {{codegen:UndoManager pinvoke decl}} */
        #region UndoManager
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UndoManagerDiscardAllSnapshots(TRN_UndoManager self, ref TRN_DocSnapshot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UndoManagerUndo(TRN_UndoManager self, ref TRN_ResultSnapshot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UndoManagerCanUndo(TRN_UndoManager self,  [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UndoManagerGetNextUndoSnapshot(TRN_UndoManager self, ref TRN_DocSnapshot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UndoManagerRedo(TRN_UndoManager self, ref TRN_ResultSnapshot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UndoManagerCanRedo(TRN_UndoManager self,  [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UndoManagerGetNextRedoSnapshot(TRN_UndoManager self, ref TRN_DocSnapshot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UndoManagerTakeSnapshot(TRN_UndoManager self, ref TRN_ResultSnapshot result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_UndoManagerDestroy(ref TRN_UndoManager self);

        #endregion
        /* {{codegen:UndoManager pinvoke decl}} */
        /* {{codegen:DocSnapshot pinvoke decl}} */
        #region DocSnapshot
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocSnapshotGetHash(TRN_DocSnapshot self, ref uint result);
        
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocSnapshotIsValid(TRN_DocSnapshot self,  [MarshalAs(UnmanagedType.U1)] ref bool result);
        
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocSnapshotEquals(TRN_DocSnapshot self, TRN_DocSnapshot snapshot,  [MarshalAs(UnmanagedType.U1)] ref bool result);
        
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DocSnapshotDestroy(ref TRN_DocSnapshot self);
        
        #endregion
        /* {{codegen:DocSnapshot pinvoke decl}} */
        /* {{codegen:ResultSnapshot pinvoke decl}} */
        #region ResultSnapshot
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ResultSnapshotCurrentState(TRN_ResultSnapshot self, ref TRN_DocSnapshot result);
        
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ResultSnapshotPreviousState(TRN_ResultSnapshot self, ref TRN_DocSnapshot result);
        
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ResultSnapshotIsOk(TRN_ResultSnapshot self,  [MarshalAs(UnmanagedType.U1)] ref bool result);
        
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ResultSnapshotIsNullTransition(TRN_ResultSnapshot self,  [MarshalAs(UnmanagedType.U1)] ref bool result);
        
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_ResultSnapshotDestroy(ref TRN_ResultSnapshot self);

        #endregion
        /* {{codegen:ResultSnapshot pinvoke decl}} */
        /* {{codegen:DigitalSignatureField pinvoke decl}} */
        #region DigitalSignatureField
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldHasCryptographicSignature(ref BasicTypes.TRN_DigitalSignatureField self, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldGetSubFilter(ref BasicTypes.TRN_DigitalSignatureField self, ref DigitalSignatureField.SubFilterType result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldGetSignatureName(ref BasicTypes.TRN_DigitalSignatureField self, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldGetLocation(ref BasicTypes.TRN_DigitalSignatureField self, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldGetReason(ref BasicTypes.TRN_DigitalSignatureField self, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldGetContactInfo(ref BasicTypes.TRN_DigitalSignatureField self, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldGetCertCount(ref BasicTypes.TRN_DigitalSignatureField self, ref IntPtr result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldHasVisibleAppearance(ref BasicTypes.TRN_DigitalSignatureField self, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldSetContactInfo(ref BasicTypes.TRN_DigitalSignatureField self, TRN_UString in_contact_info);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldSetLocation(ref BasicTypes.TRN_DigitalSignatureField self, TRN_UString in_location);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldSetReason(ref BasicTypes.TRN_DigitalSignatureField self, TRN_UString in_reason);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldSetDocumentPermissions(ref BasicTypes.TRN_DigitalSignatureField self, DigitalSignatureField.DocumentPermissions in_perms);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldSignOnNextSave(ref BasicTypes.TRN_DigitalSignatureField self, TRN_UString in_pkcs12_keyfile_path, TRN_UString in_password);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldCertifyOnNextSave(ref BasicTypes.TRN_DigitalSignatureField self, TRN_UString in_pkcs12_keyfile_path, TRN_UString in_password);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldIsLockedByDigitalSignature(ref BasicTypes.TRN_DigitalSignatureField self, [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldGetDocumentPermissions(ref BasicTypes.TRN_DigitalSignatureField self, ref DigitalSignatureField.DocumentPermissions result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldClearSignature(ref BasicTypes.TRN_DigitalSignatureField self);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldCreateFromField(ref BasicTypes.TRN_Field d, ref BasicTypes.TRN_DigitalSignatureField result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldGetSigningTime(ref BasicTypes.TRN_DigitalSignatureField self, ref BasicTypes.TRN_Date in_date);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldGetCert(ref BasicTypes.TRN_DigitalSignatureField self, IntPtr in_index, ref TRN_Vector result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldSetFieldPermissions(ref BasicTypes.TRN_DigitalSignatureField self, DigitalSignatureField.FieldPermissions in_action, TRN_UString[] in_field_names_list, IntPtr in_field_names_size);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldSignOnNextSaveFromBuffer(ref BasicTypes.TRN_DigitalSignatureField self, IntPtr in_pkcs12_buffer, IntPtr in_pkcs12_buf_size, TRN_UString in_password);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldSignOnNextSaveWithCustomHandler(ref BasicTypes.TRN_DigitalSignatureField self, UIntPtr in_signature_handler_id);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldCertifyOnNextSaveFromBuffer(ref BasicTypes.TRN_DigitalSignatureField self, IntPtr in_pkcs12_buffer, IntPtr in_pkcs12_buf_size, TRN_UString in_password);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldCertifyOnNextSaveWithCustomHandler(ref BasicTypes.TRN_DigitalSignatureField self, UIntPtr in_signature_handler_id);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldGetSDFObj(ref BasicTypes.TRN_DigitalSignatureField self, ref TRN_Obj result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_DigitalSignatureFieldGetLockedFields(ref BasicTypes.TRN_DigitalSignatureField self, ref TRN_Vector result);

        #endregion
        /* {{codegen:DigitalSignatureField pinvoke decl}} */
        /* {{codegen:GeometryCollection pinvoke decl}} */
        #region GeometryCollection
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GeometryCollectionSnapToNearest(TRN_GeometryCollection self, double x, double y, uint mode, ref BasicTypes.TRN_Point result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GeometryCollectionSnapToNearestPixel(TRN_GeometryCollection self, double x, double y, double dpi, uint mode, ref BasicTypes.TRN_Point result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_GeometryCollectionDestroy(TRN_GeometryCollection self);

        #endregion
        /* {{codegen:GeometryCollection pinvoke decl}} */
        /* {{codegen:CADModule pinvoke decl}} */
        #region CADModule
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_CADModuleIsModuleAvailable( [MarshalAs(UnmanagedType.U1)] ref bool result);

        #endregion
        /* {{codegen:CADModule pinvoke decl}} */
        /* {{codegen:OCRModule pinvoke decl}} */
        #region OCRModule
        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCRModuleIsModuleAvailable( [MarshalAs(UnmanagedType.U1)] ref bool result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCRModuleImageToPDF(TRN_PDFDoc dst, TRN_UString src, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCRModuleProcessPDF(TRN_PDFDoc dst, TRN_Obj options);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCRModuleGetOCRJsonFromImage(TRN_PDFDoc dst, TRN_UString src, TRN_Obj options, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCRModuleGetOCRJsonFromPDF(TRN_PDFDoc src, TRN_Obj options, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCRModuleApplyOCRJsonToPDF(TRN_PDFDoc dst, TRN_UString json);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCRModuleGetOCRXmlFromImage(TRN_PDFDoc dst, TRN_UString src, TRN_Obj options, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCRModuleGetOCRXmlFromPDF(TRN_PDFDoc src, TRN_Obj options, ref TRN_UString result);

        [DllImport(dllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern TRN_Exception TRN_OCRModuleApplyOCRXmlToPDF(TRN_PDFDoc dst, TRN_UString xml);

        #endregion
        /* {{codegen:OCRModule pinvoke decl}} */
        // {{codegen: xamarin_pinvoke}}
    }
}
