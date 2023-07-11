using Android.Util;
using Android.Views;
using AndroidX.Wear.WatchFace;
using AndroidX.Wear.WatchFace.Style;
using Kotlin.Coroutines;
using Object = Java.Lang.Object;

namespace Sample.WearOS;

public class MyWatchFaceService : WatchFaceService
{
    protected override Object CreateWatchFace(ISurfaceHolder surfaceHolder, WatchState watchState, ComplicationSlotsManager complicationSlotsManager, CurrentUserStyleRepository currentUserStyleRepository, IContinuation p4)
    {
        Log.Info("myappwatch", "CreateWatchFace 1");
        var watchFace = new CustomCanvasRenderer(surfaceHolder, currentUserStyleRepository, watchState, CanvasType.Hardware);
        Log.Info("myappwatch", "CreateWatchFace 2");

        return new WatchFace(WatchFaceType.Digital, watchFace).SetTapListener(watchFace);
    }

}