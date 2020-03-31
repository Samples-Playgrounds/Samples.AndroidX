using AndroidX.AppCompat.App;

namespace Toggl.Droid.Helper
{
    public static class ActiveTheme
    {
        public static class Is
        {
            public static bool DarkTheme
                => AppCompatDelegate.DefaultNightMode == AppCompatDelegate.ModeNightYes;

            public static class Not
            {
                public static bool DarkTheme
                    => AppCompatDelegate.DefaultNightMode != AppCompatDelegate.ModeNightYes;
            }
        }
    }
}
