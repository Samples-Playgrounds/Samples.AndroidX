using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Content.Res;
using Android.Graphics;

using pdftron.SDF;

namespace pdftron.PDF
{
    /// <summary>
    /// PDFViewCtrl is derived from android.view.ViewGroup and encapsulates a rich set
    /// of functionalities for interactive viewing of PDF documents. See parent class
    /// documentation here: https://www.pdftron.com/api/android/pdfnet/javadoc/reference/com/pdftron/pdf/PDFViewCtrl.html.
    /// <para>PDFViewCtrl makes use of PDFTron's PDFNet library that is compiled as a
    /// native library to ensure real-time viewing experience. It provides a
    /// comprehensive set of functionalities, including multi-threaded rendering, PDF
    /// rendering settings, scrolling, zooming, page navigation, different page
    /// viewing modes, coordinates conversion, text selection, text search, etc.
    /// In addition, PDFViewCtrl also allows for extension to accommodate a user's
    /// specific needs, which can be achieved in one of the following two ways:</para>
    /// 
    /// <list type="bullet">
    /// <item><description>Derive from PDFViewCtrl and override its functions such as
    /// <see cref="OnMeasure"/>, <see cref="OnDraw"/>, <see cref="OnScroll"/>, etc. However, note
    /// that since PDFViewCtrl implements these functions, it is necessary to call
    /// PDFViewCtrl's implementations first.</description></item>
    /// <item><description>The recommended way is to use PDFViewCtrl's <see cref="PDFViewCtrl.ITool"/>
    /// interface. The interface provides a way for users to interact with PDFViewCtrl, 
    /// including customizing responses to gestures and drawing extra contents on top of
    /// the PDF content. In fact, PDFNet SDK also ships with a separate library, Tools.dll,
    /// that is programmed via the interface. 
    /// This library utilizes functions offered by PDFNet and implements
    /// several ready-to-use UI modules such as annotation editing, text selection,
    /// text search, page navigation, etc. PDFNet customers can have access to the
    /// source code of Tools.jar for full customization.</description></item>
    /// </list>
    /// </summary>
	public class PDFViewCtrl : pdftronprivate.PDF.PDFViewCtrl
    {
        /// <summary>
        /// Gets or sets the PDF document to be displayed.
        /// </summary>
        /// <value>The document.</value>
        public virtual PDFDoc Doc
        {
            get
            {
                return GetDoc();
            }
            set
            {
                SetDoc(value);
            }
        }

        /// <summary>
		/// Gets or sets the current page.
		/// </summary>
		/// <value>The page number.</value>
        public virtual int CurrentPage
        {
            get
            {
                return base.CurrentPage;
            }
            set
            {
                SetCurrentPage(value);
            }
        }

        /// <summary>
        /// Gets the PDF document to be displayed.
        /// </summary>
        /// <returns>The PDFDoc</returns>
        public virtual pdftron.PDF.PDFDoc GetDoc()
        {
            return TypeConvertHelper.ConvPdfDocToManaged(base.InnerDoc);
        }

        /// <summary>
        /// Sets the PDF document to be displayed.
        /// </summary>
        /// <param name="doc">The PDFDoc</param>
        public virtual void SetDoc(pdftron.PDF.PDFDoc doc)
        {
            CloseDoc();
            base.InnerDoc = TypeConvertHelper.ConvPDFDocToNative(doc);
        }

        public override void CloseDoc()
        {
            var doc = base.InnerDoc;
            base.CloseDoc();
            if (doc != null)
            {
                // close the cloned doc
                doc.Close();
            }
        }

        public override void Destroy()
        {
            CloseDoc();
            base.Destroy();
        }

        public pdftron.PDF.PDFDoc OpenPDFUri(Android.Net.Uri uri, string password)
        {
            var pdfdoc = base.OpenPDFUri(uri, password);
            return TypeConvertHelper.ConvPdfDocToManaged(pdfdoc);
        }

        public virtual void SetProgressiveRendering(bool progressive)
        {
            base.ProgressiveRendering = progressive;
        }
        public virtual bool GetProgressiveRendering()
        {
            return base.ProgressiveRendering;
        }

        public pdftron.PDF.DocumentConversion OpenNonPDFUri(Android.Net.Uri uri, pdftronprivate.PDF.WordToPDFOptions options)
        {
            var documentConversion = base.OpenNonPDFUri(uri, options);
            return TypeConvertHelper.ConvDocumentConversionToManaged(documentConversion);
        }

        /// <summary>
        /// Gets the box used by PDFViewCtrl for rasterizing.
        /// </summary>
        /// <returns>The page box used to rasterize. See {@link #SetPageBox(Page.Box)}</returns>
        public virtual Page.Box GetPageBox()
        {
            return (Page.Box)this.PageBox;
        }

        /// <summary>
        /// Selects the PageBox to rasterize. PDFViewCtrl will clip pages according to their
        /// PageBox as selected here.
        /// </summary>
        /// <param name="box">the region to be used for clipping. Possible values are:
        /// <ul>
        /// <li>{@link Page#e_media}</li>
        /// <li>{@link Page#e_crop}</li>
        /// <li>{@link Page#e_bleed}</li>
        /// <li>{@link Page#e_trim}</li>
        /// <li>{@link Page#e_art}</li>
        /// <li>{@link Page#e_user_crop}</li>
        /// </ul>
        /// </param>
        public virtual void SetPageBox(Page.Box box)
        {
            this.PageBox = (int)box;
        }

        // Methods
        /// <summary>
        /// Constructor that is called when inflating a view from XML. This is called
        /// when a view is being constructed from an XML file, supplying attributes
        /// that were specified in the XML file. This version uses a default style of
        /// 0, so the only attribute values applied are those in the Context's Theme
        /// and the given AttributeSet.
        /// </summary>
        /// <param name="context">The Context the view is running in, through which it can
        /// access the current theme, resources, etc.</param>
        /// <param name="attrs">The attributes of the XML tag that is inflating the view.</param>
        public PDFViewCtrl(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }
        /// <summary>
        /// Perform inflation from XML and apply a class-specific base style. This
        /// constructor of View allows subclasses to use their own base style when
        /// they are inflating. For example, a Button class's constructor would call
        /// this version of the super class constructor and supply R.attr.buttonStyle
        /// for defStyle; this allows the theme's button style to modify all of the
        /// base view attributes (in particular its background) as well as the Button
        /// class's attributes.
        /// </summary>
        /// <param name="context">The Context the view is running in, through which it can
        /// access the current theme, resources, etc.</param>
        /// <param name="attrs">The attributes of the XML tag that is inflating the view.</param>
        /// <param name="defStyle">The default style to apply to this view. If 0, no style will
        /// be applied (beyond what is included in the theme). This may
        /// either be an attribute resource, whose value will be retrieved
        /// from the current theme, or an explicit style resource.</param>
        public PDFViewCtrl(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }
        public virtual Point ConvCanvasPtToScreenPt(Point pt)
        {
            double[] result = ConvCanvasPtToScreenPt(pt.x, pt.y);
            return new Point(result[0], result[1]);
        }
        /// <summary>
        /// Converts a point in canvas space to the current page space.
        /// </summary>
        /// <returns>The canvas point to page point.</returns>
        /// <param name="pt">The input point.</param>
        public virtual Point ConvCanvasPtToPagePt(Point pt)
        {
            double[] result = ConvCanvasPtToPagePt(pt.x, pt.y);
            return new Point(result[0], result[1]);
        }
        /// <summary>
        /// Converts a point in canvas space to the specified page space.
        /// </summary>
        /// <returns>The canvas point to page point.</returns>
        /// <param name="pt">The input point.</param>
        /// <param name="page_num">page number.</param>
        public virtual Point ConvCanvasPtToPagePt(Point pt, int page_num)
        {
            double[] result = ConvCanvasPtToPagePt(pt.x, pt.y, page_num);
            return new Point(result[0], result[1]);
        }
        /// <summary>
        /// Converts a point expressed in screen space to a point in (PDF) canvas space.
        /// <para>When converting points, PDFViewCtrl may refer to the following spaces:</para>
        /// <list type="bullet">
        /// <item><description>Screen Space is the virtual area occupied by PDFViewCtrl on a device,
        /// originated at its upper-left corner. Note that this space can extend
        /// beyond the visible region. Screen Space is in the same scale with the
        /// dimensions of android.view.View;</description></item>
        /// <item><description>Page Space is defined by a PDF page itself, originated at the
        /// lower-left corner. Unlike Screen Space, it is independent of how the page
        /// is viewed; namely, it does't change with the zoom factor, the scroll
        /// position, and etc.</description></item>
        /// <item><description>(PDF) Canvas Space is defined as the smallest axis-aligned rectangle
        /// surrounding all the pages in continuous page presentation mode, or the
        /// current page(s) in non-continuous page presentation mode. For example,
        /// PAGE_PRESENTATION_FACING_CONT will make a wider but shorter Canvas Space
        /// than what PAGE_PRESENTATION_SINGLE_CONT will do. Note that PDF Canvas
        /// Space is in the same scale of Page Space and also independent of zoom
        /// factor, scroll position, etc. For simplicity, PDF Canvas Space is
        /// referenced to as Canvas Space.</description></item>
        /// <item><description>Zoomed PDF Canvas Space is similar to PDF Canvas space, but after it
        /// is scaled by the current zoom factor, and page gaps are added between
        /// pages.</description></item>
        /// <item><description>View Canvas Space is the virtual region occupied by the
        /// android.graphics.Canvas object, the one passed in through the
        /// onDraw(Canvas) method. Note that View Canvas Space is in general
        /// different from Canvas Space. For example, when PDFViewCtrl is zoomed out
        /// so that all the pages are visible, View Canvas Space will be
        /// larger than Canvas Space.</description></item>
        /// </list>
        /// </summary>
        /// <returns>The client point to canvas point.</returns>
        /// <param name="pt">The input point.</param>
        public virtual Point ConvScreenPtToCanvasPt(Point pt)
        {
            double[] result = ConvScreenPtToCanvasPt(pt.x, pt.y);
            return new Point(result[0], result[1]);
        }
        /// <summary>
        /// Converts a point expressed in screen space to a point on a page space.
        /// </summary>
        /// <returns>The client point to page point.</returns>
        /// <param name="pt">The input point.</param>
        public virtual Point ConvScreenPtToPagePt(Point pt)
        {
            double[] result = ConvScreenPtToPagePt(pt.x, pt.y);
            return new Point(result[0], result[1]);
        }
        /// <summary>
        /// Converts a point expressed in screen space to a point on a page space.
        /// </summary>
        /// <returns>The client point to page point.</returns>
        /// <param name="pt">The input point.</param>
        /// <param name="page_num">page number.</param>
        public virtual Point ConvScreenPtToPagePt(Point pt, int page_num)
        {
            double[] result = ConvScreenPtToPagePt(pt.x, pt.y, page_num);
            return new Point(result[0], result[1]);
        }
        /// <summary>
        /// Converts a point in the current page space to canvas space.
        /// </summary>
        /// <returns>The page point to canvas point.</returns>
        /// <param name="pt">The input point.</param>
        public virtual Point ConvPagePtToCanvasPt(Point pt)
        {
            double[] result = ConvPagePtToCanvasPt(pt.x, pt.y);
            return new Point(result[0], result[1]);
        }
        /// <summary>
        /// Converts a point in the specified page space to canvas space.
        /// </summary>
        /// <returns>The page point to canvas point.</returns>
        /// <param name="pt">The input point.</param>
        /// <param name="page_num">page number.</param>
        public virtual Point ConvPagePtToCanvasPt(Point pt, int page_num)
        {
            double[] result = ConvPagePtToCanvasPt(pt.x, pt.y, page_num);
            return new Point(result[0], result[1]);
        }
        /// <summary>
        /// Converts a point from a page space to a point in screen space.
        /// </summary>
        /// <returns>The page point to client point.</returns>
        /// <param name="pt">The input point.</param>
        /// <param name="page_num">P2.</param>
        public virtual Point ConvPagePtToScreenPt(Point pt, int page_num)
        {
            double[] result = ConvPagePtToScreenPt(pt.x, pt.y, page_num);
            return new Point(result[0], result[1]);
        }

        public virtual Point ConvPagePtToHorizontalScrollingPt(Point pt, int page_num)
        {
            double[] result = ConvPagePtToHorizontalScrollingPt(pt.x, pt.y, page_num);
            return new Point(result[0], result[1]);
        }
        /// <summary>
        /// Sets the color post processing transformation.
        /// This transform is applied to the rasterized bitmap as the final step
        /// in the rasterization process, and is applied directly to the resulting
        /// bitmap (disregarding any color space information). Color post
        /// processing only supported for RGBA output.
        /// </summary>
        /// <param name="mode">the specific transform to be applied.</param>
        public virtual void SetColorPostProcessMode(PDFRasterizer.ColorPostProcessMode mode)
        {
            ColorPostProcessMode = (int)mode;
        }
        public virtual void SetColorPostProcessMapFile(pdftron.Filters.Filter imageFileContents)
        {
            base.SetColorPostProcessMapFile(TypeConvertHelper.ConvFilterToNative(imageFileContents));
        }
        /// <summary>
        /// Gets the color post processing transformation.
        /// </summary>
        /// <returns>the current color post processing mode..</returns>
        public virtual PDFRasterizer.ColorPostProcessMode GetColorPostProcessMode()
        {
            return (PDFRasterizer.ColorPostProcessMode)ColorPostProcessMode;
        }
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="action">The action.</param>
        public virtual void ExecuteAction(Action action)
        {
            base.ExecuteAction(TypeConvertHelper.ConvActionToNative(action));
        }

        /// <summary>
        /// Gets the annotation at x and y.
        /// </summary>
        /// <returns>The <see cref="pdftron.PDF.Annot"/>.</returns>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public virtual Annot GetAnnotationAt(int x, int y)
        {
            pdftronprivate.PDF.Annot annot = base.GetAnnotationAt(x, y);
            return TypeConvertHelper.ConvAnnotToManaged(annot);
        }
        /// <summary>
        /// Gets the annotation at the (x, y) position expressed in screen coordinates.
        /// </summary>
        /// <returns>The <see cref="pdftron.PDF.Annot"/>The annotation closest to the point. If no annotation was found, it returns a null pointer.</returns>
        /// <param name="x">x coordinate of the screen point.</param>
        /// <param name="y">y coordinate of the screen point.</param>
        /// <param name="distanceThreshold">Maximum distance from the point (x, y) to the annotation for the annot to be considered a hit.</param>
        /// <param name="minimumLineWeight">For very thin lines, it is almost impossible to hit the actual line.
        /// 								This specifies a minimum line thickness (in screen coordinates) for the purpose of calculating whether
        /// 								a point is inside the annotation or not.</param>
        public virtual Annot GetAnnotationAt(int x, int y, double distanceThreshold, double minimumLineWeight)
        {
            pdftronprivate.PDF.Annot annot = base.GetAnnotationAt(x, y, distanceThreshold, minimumLineWeight);
            return TypeConvertHelper.ConvAnnotToManaged(annot);
        }
        public virtual List<Annot> GetAnnotationListAt(int x1, int y1, int x2, int y2)
        {
            IList<pdftronprivate.PDF.Annot> annots = base.GetAnnotationListAt(x1, y1, x2, y2);
            List<Annot> results = new List<Annot>(annots.Count);
            foreach (pdftronprivate.PDF.Annot ann in annots)
            {
                results.Add(TypeConvertHelper.ConvAnnotToManaged(ann));
            }
            return results;
        }
        /// <summary>
        /// Selects texts identified by Highlights. Also scroll to the Highlights if selected.
        /// </summary>
        /// <returns><c>true</c>, if some text was selected, <c>false</c> otherwise.</returns>
        /// <param name="hlts">Highlights the Highlights to be selected.</param>
        public virtual bool SelectAndJumpWithHighlights(Highlights hlts)
        {
            return base.SelectAndJumpWithHighlights(TypeConvertHelper.ConvHighlightsToNative(hlts));
        }
        /// <summary>
        /// Selects texts identified by Highlights.
        /// </summary>
        /// <returns><c>true</c>, if by highlights was selected, <c>false</c> otherwise.</returns>
        /// <param name="hlts">The Highlights to be selected.</param>
        public virtual bool Select(Highlights hlts)
        {
            return base.SelectWithHighlights(TypeConvertHelper.ConvHighlightsToNative(hlts));
        }
        /// <summary>
        /// Disable rendering of a particular annotation. This does not change the
        /// annotation itself, just how it is displayed in this viewer instance.
        /// </summary>
        /// <param name="annot">The annotation object to cease drawing for.</param>
        public virtual void HideAnnotation(Annot annot)
        {
            base.HideAnnotation(TypeConvertHelper.ConvAnnotToNative(annot));
        }
        /// <summary>
        /// Enable rendering of a particular annotation. Only has an effect if
        /// HideAnnotation() has prviously been called on the same annot.
        /// </summary>
        /// <param name="annot">The annotation object to resume rendering.</param>
        public virtual void ShowAnnotation(Annot annot)
        {
            base.ShowAnnotation(TypeConvertHelper.ConvAnnotToNative(annot));
        }
        /// <summary>
        /// Gets the annotation bounding box in screen coordinates
        /// </summary>
        /// <returns>The screen rect for annotation.</returns>
        /// <param name="annot">target annotation.</param>
        /// <param name="page_num">the page number where the annotation is on.</param>
        public virtual Rect GetScreenRectForAnnot(Annot annot, int page_num)
        {
            pdftronprivate.PDF.Rect rect = base.GetScreenRectForAnnot(TypeConvertHelper.ConvAnnotToNative(annot), page_num);
            return new Rect(rect.X1, rect.Y1, rect.X2, rect.Y2);
        }
        /// <summary>
        /// Gets the annotation bounding box in page coordinates
        /// </summary>
        /// <returns>The page rect for annotation.</returns>
        /// <param name="annot">target annotation.</param>
        /// <param name="page_num">the page number where the annotation is on.</param>
        public virtual Rect GetPageRectForAnnot(Annot annot, int page_num)
        {
            pdftronprivate.PDF.Rect rect = base.GetPageRectForAnnot(TypeConvertHelper.ConvAnnotToNative(annot), page_num);
            return new Rect(rect.X1, rect.Y1, rect.X2, rect.Y2);
        }
        /// <summary>
        /// Sets the viewer cache.
        /// </summary>
        /// <param name="document">The maximum size, in bytes, of the entire document's page cache.</param>
        /// <param name="max_cache_size">Max cache size</param>
        /// <param name="on_disk"> Whether or not store the cache on disk.</param>
        /// <exception cref="PDFNetException"/>
        public static void SetViewerCache(SDFDoc document, int max_cache_size, bool on_disk)
        {
            pdftronprivate.PDF.PDFViewCtrl.SetViewerCache(TypeConvertHelper.ConvSDFDocToNative(document), max_cache_size, on_disk);
        }
        /// <summary>
        /// Adjusts the viewing area to fit a rectangle on a specified page.
        /// Rectangle must be specified in page space. This will adjust current page
        /// and zoom appropriately.
        /// </summary>
        /// <returns><c>true</c>, if successful, <c>false</c> otherwise.</returns>
        /// <param name="page_num">The specified page number.</param>
        /// <param name="rect">The rectangle to fit in.</param>
        public virtual bool ShowRect(int page_num, Rect rect)
        {
            return base.ShowRect(page_num, TypeConvertHelper.ConvRectToNative(rect));
        }
        /// <summary>
        /// Redraws the given area in the client space.
        /// </summary>
        /// <param name="rec">the rectangle specifying the given area.</param>
        public virtual void Update(Rect rect)
        {
            base.Update(TypeConvertHelper.ConvRectToNative(rect));
        }
        /// <summary>
        /// Redraws the area covered with a given annotation.
        /// </summary>
        /// <param name="annot">the annotation whose occupied region is to be updated.</param>
        /// <param name="page_num">the number of the page that the annotation belongs to.</param>
        public virtual void Update(Annot annot, int page_num)
        {
            base.Update(TypeConvertHelper.ConvAnnotToNative(annot), page_num);
        }
        /// <summary>
        /// Gets the page rotation.
        /// </summary>
        /// <returns>The page rotation.</returns>
        public virtual Page.Rotate GetRotation()
        {
            return (Page.Rotate)PageRotation;
        }
        /// <summary>
        /// Helper function that will refresh annotation and/or field appearances if needed, and then
        /// render modified page areas, all based on the contents of the view_change parameter.
        /// </summary>
        /// <param name="viewChange">contains all the updated fields and rectangles.</param>
        public virtual void RefreshAndUpdate(ViewChangeCollection viewChange)
        {
            base.RefreshAndUpdate(TypeConvertHelper.ConvViewChangeCollectionToNative(viewChange));
        }
    }
}