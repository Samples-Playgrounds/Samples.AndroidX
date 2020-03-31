using Firebase.Provider;
using Newtonsoft.Json.Converters;
using System;
using AndroidX.AppCompat.Widget;
using AndroidX.CardView.Widget;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Legacy.Widget;
using AndroidX.Lifecycle;
using Google.Android.Material.Internal;
using Google.Android.Material.Snackbar;

namespace Toggl.Droid
{
    // This class is never actually executed, but when Xamarin linking is enabled it does so to ensure types and properties
    // are preserved in the deployed app
    public class LinkerPleaseInclude
    {
        public void Include(FitWindowsLinearLayout linearLayout)
        {
            linearLayout = new FitWindowsLinearLayout(null);
            linearLayout = new FitWindowsLinearLayout(null, null);
        }

        public void Include(CardView cardView)
        {
            cardView = new CardView(null);
            cardView = new CardView(null, null);
            cardView = new CardView(null, null, 0);
        }

        public void Include(FitWindowsFrameLayout layout)
        {
            layout = new FitWindowsFrameLayout(null);
            layout = new FitWindowsFrameLayout(null, null);
        }

        public void Include(AlertDialogLayout layout)
        {
            layout = new AlertDialogLayout(null);
            layout = new AlertDialogLayout(null, null);
        }

        public void Include(DialogTitle title)
        {
            title = new DialogTitle(null);
            title = new DialogTitle(null, null);
            title = new DialogTitle(null, null, 0);
        }
        public void Include(Space space)
        {
            space = new Space(null);
            space = new Space(null, null);
            space = new Space(null, null, 0);
        }
        public void Include(ButtonBarLayout layout)
        {
            layout = new ButtonBarLayout(null, null);
        }

        public void Include(StringEnumConverter converter)
        {
            converter = new StringEnumConverter(true);
        }

        public void Include(ConsoleColor color)
        {
            Console.Write("");
            Console.WriteLine("");
            color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        public void Include(FirebaseInitProvider provider)
        {
            provider = new FirebaseInitProvider();
        }

        public void Include(ConstraintLayout constraintLayout, Guideline guideline)
        {
            constraintLayout = new ConstraintLayout(null);
            constraintLayout = new ConstraintLayout(null, null);
            constraintLayout = new ConstraintLayout(null, null, 1);
            guideline = new Guideline(null);
            guideline = new Guideline(null, null);
            guideline = new Guideline(null, null, 1);
            guideline = new Guideline(null, null, 1, 1);
        }

        public void Include(ProcessLifecycleOwner processLifecycleOwner, ProcessLifecycleOwnerInitializer processLifecycleOwnerInitializer)
        {
            var proccessLifecycleOwner = ProcessLifecycleOwner.Get();
            var initializer = new ProcessLifecycleOwnerInitializer();
        }

        public void Include(BaselineLayout baselineLayout)
        {
            var baseLine = new BaselineLayout(null, null, 0);
        }

        public void Include(Snackbar.SnackbarLayout snackbarLayout)
        {
            var include = new Snackbar.SnackbarLayout(null);
            var include2 = new Snackbar.SnackbarLayout(null, null);
        }
    }
}
