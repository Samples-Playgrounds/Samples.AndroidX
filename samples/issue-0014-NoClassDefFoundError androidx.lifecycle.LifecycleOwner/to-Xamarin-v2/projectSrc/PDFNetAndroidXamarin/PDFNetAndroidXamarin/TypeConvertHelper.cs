using System;
using System.Collections.Generic;

namespace pdftron.PDF
{
    /// <summary>
    /// Utility class for type mapping between C# object and java object
    /// </summary>
    public class TypeConvertHelper
    {
        /// <summary>
        /// Convert java PDFDoc to C# PDFDoc
        /// </summary>
        public static pdftron.PDF.PDFDoc ConvPdfDocToManaged(pdftronprivate.PDF.PDFDoc inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            // clone the PDFDoc
            IntPtr sdfCopy = IntPtr.Zero;
            Int64 ptr_java = inObj.__GetHandle();
            IntPtr ptr_cs = new IntPtr(ptr_java);
            pdftron.Common.PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetSDFDoc(ptr_cs, ref sdfCopy));
            IntPtr pdfCopy = IntPtr.Zero;
            pdftron.Common.PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateFromSDFDoc(sdfCopy, ref pdfCopy));
            
            return pdftron.PDF.PDFDoc.CreateInternal(pdfCopy);
        }

        /// <summary>
        /// Convert C# PDFDoc to java PDFDoc
        /// </summary>
        public static pdftronprivate.PDF.PDFDoc ConvPDFDocToNative(pdftron.PDF.PDFDoc inObj)
        {
            if (inObj == null)
            {
                return null;
            }

            // clone the PDFDoc
            IntPtr sdfCopy = IntPtr.Zero;
            pdftron.Common.PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocGetSDFDoc(inObj.mp_doc, ref sdfCopy));
            IntPtr pdfCopy = IntPtr.Zero;
            pdftron.Common.PDFNetException.REX(PDFNetPINVOKE.TRN_PDFDocCreateFromSDFDoc(sdfCopy, ref pdfCopy));
            
            Int64 ptr_java = pdfCopy.ToInt64();
            return pdftronprivate.PDF.PDFDoc.__Create(ptr_java);
        }

        /// <summary>
        /// Convert java Annot to C# Annot
        /// </summary>
        public static pdftron.PDF.Annot ConvAnnotToManaged(pdftronprivate.PDF.Annot inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            Int64 ptr_java = inObj.__GetHandle();
            IntPtr ptr_cs = new IntPtr(ptr_java);
            return pdftron.PDF.Annot.CreateInternal(ptr_cs, null);
        }

        /// <summary>
        /// Convert C# Annot to java Annot
        /// </summary>
        public static pdftronprivate.PDF.Annot ConvAnnotToNative(pdftron.PDF.Annot inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            IntPtr ptr_cs = inObj.GetHandleInternal();
            Int64 ptr_java = ptr_cs.ToInt64();
            return pdftronprivate.PDF.Annot.__Create(ptr_java, null);
        }

        /// <summary>
        /// Convert java Page to C# Page
        /// </summary>
        public static pdftron.PDF.Page ConvPageToManaged(pdftronprivate.PDF.Page inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            Int64 ptr_java = inObj.__GetHandle();
            IntPtr ptr_cs = new IntPtr(ptr_java);
            return pdftron.PDF.Page.CreateInternal(ptr_cs, null);
        }

        /// <summary>
        /// Convert C# Filter to java Filter
        /// </summary>
        public static pdftronprivate.Filters.Filter ConvFilterToNative(pdftron.Filters.Filter inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            IntPtr ptr_cs = inObj.GetHandleInternal();
            Int64 ptr_java = ptr_cs.ToInt64();
            return pdftronprivate.Filters.Filter.__Create(ptr_java, null);
        }

        /// <summary>
        /// Convert C# Action to java Action
        /// </summary>
        public static pdftronprivate.PDF.Action ConvActionToNative(pdftron.PDF.Action inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            IntPtr ptr_cs = inObj.GetHandleInternal();
            Int64 ptr_java = ptr_cs.ToInt64();
            return pdftronprivate.PDF.Action.__Create(ptr_java, null);
        }

        /// <summary>
        /// Convert C# Highlights to java Highlights
        /// </summary>
        public static pdftronprivate.PDF.Highlights ConvHighlightsToNative(pdftron.PDF.Highlights inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            IntPtr ptr_cs = inObj.GetHandleInternal();
            Int64 ptr_java = ptr_cs.ToInt64();
            return new pdftronprivate.PDF.Highlights(ptr_java);
        }

        /// <summary>
        /// Convert C# SDFDoc to java SDFDoc
        /// </summary>
        public static pdftronprivate.SDF.SDFDoc ConvSDFDocToNative(pdftron.SDF.SDFDoc inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            IntPtr ptr_cs = inObj.GetHandleInternal();
            Int64 ptr_java = ptr_cs.ToInt64();
            return pdftronprivate.SDF.SDFDoc.__Create(ptr_java, null);
        }

        /// <summary>
        /// Convert C# Rect to java Rect
        /// </summary>
        public static pdftronprivate.PDF.Rect ConvRectToNative(pdftron.PDF.Rect inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            return new pdftronprivate.PDF.Rect(inObj.x1, inObj.y1, inObj.x2, inObj.y2);
        }

        /// <summary>
        /// Convert java Rect to C# Rect
        /// </summary>
        public static pdftron.PDF.Rect ConvRectToManaged(pdftronprivate.PDF.Rect inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            return new pdftron.PDF.Rect(inObj.X1, inObj.Y1, inObj.X2, inObj.Y2);
        }

        /// <summary>
        /// Convert C# ViewChangeCollection to java ViewChangeCollection
        /// </summary>
        public static pdftronprivate.PDF.ViewChangeCollection ConvViewChangeCollectionToNative(pdftron.PDF.ViewChangeCollection inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            IntPtr ptr_cs = inObj.GetHandleInternal();
            Int64 ptr_java = ptr_cs.ToInt64();
            return pdftronprivate.PDF.ViewChangeCollection.__Create(ptr_java);
        }

        /// <summary>
        /// Convert C# DocumentConversion to java DocumentConversion
        /// </summary>
        public static pdftronprivate.PDF.DocumentConversion ConvDocumentConversionToNative(pdftron.PDF.DocumentConversion inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            IntPtr ptr_cs = inObj.GetHandleInternal();
            Int64 ptr_java = ptr_cs.ToInt64();
            return pdftronprivate.PDF.DocumentConversion.__Create(ptr_java);
        }

        /// <summary>
        /// Convert java DocumentConversion to C# DocumentConversion
        /// </summary>
        public static pdftron.PDF.DocumentConversion ConvDocumentConversionToManaged(pdftronprivate.PDF.DocumentConversion inObj)
        {
            if (inObj == null)
            {
                return null;
            }
            Int64 ptr_java = inObj.__GetHandle();
            IntPtr ptr_cs = new IntPtr(ptr_java);
            return pdftron.PDF.DocumentConversion.CreateInternal(ptr_cs);
        }

        /// <summary>
        /// Convert java DocumentConversion to C# DocumentConversion
        /// </summary>
        public static QuadPoint[] ConvQuadDoubleArrayToQuadPointArray(double[] points)
        {
            List<QuadPoint> result = new List<QuadPoint>();
            for (int i = 0; i <= points.Length - 8; i += 8)
            {
                Point p1 = new Point(points[i], points[i + 1]);
                Point p2 = new Point(points[i + 2], points[i + 3]);
                Point p3 = new Point(points[i + 4], points[i + 5]);
                Point p4 = new Point(points[i + 6], points[i + 7]);
                QuadPoint qpt = new QuadPoint(p1, p2, p3, p4);
                result.Add(qpt);
            }
            return result.ToArray();
        }
    }
}