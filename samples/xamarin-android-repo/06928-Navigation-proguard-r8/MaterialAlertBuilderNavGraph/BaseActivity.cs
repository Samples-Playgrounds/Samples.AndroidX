using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using AndroidX.AppCompat.App;
using AndroidX.Preference;

namespace com.companyname.MaterialAlertBuilderNavGraph
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : AppCompatActivity
    {
        private readonly string logTag = "GLM - BaseActivity";

        protected ISharedPreferences sharedPreferences;
        protected string currentTheme;
        
        // Added 18/04/2022
        //protected string selectedTheme;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            currentTheme = sharedPreferences.GetString("colorThemeValue", "1");
            Log.Debug(logTag, "OnCreate - calling SetAppTheme - currentTheme is " + currentTheme);
            SetAppTheme(currentTheme);
        }

        private void SetAppTheme(string currentTheme)
        {
            if (currentTheme == "1")
                SetTheme(Resource.Style.Theme_OBDNowPros_RedBmw);
            else
                SetTheme(Resource.Style.Theme_OBDNowPros_GreenLexus);

            //else if (currentTheme == "2")
            //    SetTheme(Resource.Style.Theme_OBDNowPros_BlueAudi);
        }

        protected override void OnResume()
        {
            // I don't think we even need this OnResume, as I've never seen Actiivty.Recreate being called from here.
            base.OnResume();

            string selectedTheme = sharedPreferences.GetString("colorThemeValue", "1");
            if (currentTheme != selectedTheme)
            {
                Log.Debug(logTag, "OnResume calling Recreate - selectedTheme is " + selectedTheme + " " + "currentTheme is " + currentTheme);
                Recreate();
            }
            else
                Log.Debug(logTag, "OnResume no theme change - selectedTheme is " + selectedTheme + " " + "currentTheme is " + currentTheme);


            //Recreate();

        }
    }
}