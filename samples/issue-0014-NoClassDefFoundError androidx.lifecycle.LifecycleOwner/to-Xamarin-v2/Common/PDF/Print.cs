#if (__DESKTOP__)

using System;
using pdftron.Common;
using pdftronprivate.trn;

namespace pdftron.PDF
{
    public class Print
    {
        public static void StartPrintJob(PDFDoc in_pdfdoc, string in_printerName, string in_jobName, string in_outputFileName, PageSet in_pagesToPrint, PrinterMode in_printerMode, OCG.Context in_ctx)
        {
            bool cancel = false;
            PDFNetException.REX(PDFNetPINVOKE.TRN_PrintStartPrintJob(
                in_pdfdoc.mp_doc,
                UString.ConvertToUString(in_printerName).mp_impl,
                UString.ConvertToUString(in_jobName).mp_impl,
                UString.ConvertToUString(in_outputFileName).mp_impl,
                in_pagesToPrint != null ? in_pagesToPrint.mp_imp : IntPtr.Zero,
                in_printerMode != null ? in_printerMode.mp_printerMode.mp_obj : IntPtr.Zero,
                out cancel,
                in_ctx != null ? in_ctx.mp_impl : IntPtr.Zero
                ));
        }
    }
}
#endif