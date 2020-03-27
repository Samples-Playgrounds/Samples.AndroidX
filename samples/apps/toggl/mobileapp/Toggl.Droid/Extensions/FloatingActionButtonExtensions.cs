using Android.Graphics.Drawables;
using System;
using Google.Android.Material.FloatingActionButton;

namespace Toggl.Droid.Extensions
{
    public static class FloatingActionButtonExtensions
    {
        public static void SetDrawableImageSafe(this FloatingActionButton button, Drawable drawable)
        {
            // HACK: This is a really nasty work around added in #4824
            // and changed in #6597 to try fixing a bug that exists in
            // the FloatingActionButton class. Don't remove it unless you know
            // exactly what you are doing.
            // Google Issue: https://issuetracker.google.com/issues/117476935
            button.Hide();
            button.SetImageDrawable(drawable);
            button.Show(new FabVisibilityListener(() =>
            {
                button.ImageMatrix.SetScale(1, 1);
            }));
        }

        public sealed class FabVisibilityListener : FloatingActionButton.OnVisibilityChangedListener
        {
            private readonly Action onFabHidden;

            public FabVisibilityListener(Action onFabHidden)
            {
                this.onFabHidden = onFabHidden;
            }

            public override void OnHidden(FloatingActionButton fab)
            {
                base.OnHidden(fab);
                onFabHidden();
            }
        }

        public static FabVisibilityListener ToFabVisibilityListener(this Action action)
            => new FabVisibilityListener(action);
    }
}
