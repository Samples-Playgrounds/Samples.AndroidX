using System;

using pdftron.PDF;
using pdftron.PDF.Tools;
using pdftron.PDF.Tools.Utils;

using Polygon = pdftronprivate.PDF.Annots.Polygon;

namespace PDFNetAndroidXamarinSample
{
    public class CustomTool : RectCreate
    {
        public static ToolManager.IToolModeBase MODE = ToolManager.ToolMode.AddNewMode((int)Annot.Type.e_Polygon);

        public CustomTool(PDFViewCtrl ctrl) : base(ctrl)
        {

        }

        public override ToolManager.IToolModeBase ToolMode
        {
            get
            {
                return MODE;
            }
        }

        protected override pdftronprivate.PDF.Annot CreateMarkup(pdftronprivate.PDF.PDFDoc doc, pdftronprivate.PDF.Rect bbox)
        {
            var poly = new Polygon(Polygon.Create(doc, (int)Annot.Type.e_Polygon, bbox));
            var color = Utils.Color2ColorPt(Android.Graphics.Color.Red);
            poly.SetColor(color, 3);
            poly.SetVertex(0, new pdftronprivate.PDF.Point(bbox.X1, bbox.Y1));
            poly.SetVertex(1, new pdftronprivate.PDF.Point(bbox.X1, bbox.Y2));
            poly.SetVertex(2, new pdftronprivate.PDF.Point(bbox.X2, bbox.Y2));
            poly.SetVertex(3, new pdftronprivate.PDF.Point(bbox.X2, bbox.Y1));
            poly.IntentName = pdftronprivate.PDF.Annots.PolyLine.EPolygonCloud;
            poly.BorderEffect = pdftronprivate.PDF.Annots.Markup.ECloudy;
            poly.BorderEffectIntensity = 2.0;
            poly.Rect = bbox;

            return poly;
        }
    }
}