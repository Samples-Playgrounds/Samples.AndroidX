using Android.Graphics;
using Android.Util;
using Android.Views;
using AndroidX.Wear.WatchFace;
using AndroidX.Wear.WatchFace.Style;
using Java.IO;

namespace Sample.WearOS;

public class CustomCanvasRenderer : Renderer.CanvasRenderer, WatchFace.ITapListener
{
    public CustomCanvasRenderer(ISurfaceHolder surfaceHolder, CurrentUserStyleRepository currentUserStyleRepository, WatchState watchState, int canvasType)
        : base(surfaceHolder, currentUserStyleRepository, watchState, canvasType, 16L)
    {
        Log.Info("myappwatch", "CTOR");
    }

    //fun render(canvas: Canvas, bounds: Rect, zonedDateTime: ZonedDateTime): Unit
   /* public override void Render(Canvas canvas, Rect bounds, ZonedDateTime zonedDateTime)
    {
        
    }*/

    public void OnTapEvent(int tapType, TapEvent tapEvent, ComplicationSlot? complicationSlot)
    {
        Log.Info("myappwatch", "OnTapEvent");
    }
}