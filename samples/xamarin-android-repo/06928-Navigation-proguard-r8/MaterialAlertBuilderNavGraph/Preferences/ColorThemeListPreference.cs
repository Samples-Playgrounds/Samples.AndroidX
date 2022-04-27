using Android.Content;
using Android.Util;
using AndroidX.Preference;

namespace com.companyname.MaterialAlertBuilderNavGraph
{
    public class ColorThemeListPreference : ListPreference
    {

        internal readonly string DefaultThemeValue = "Select Color Theme";
        //internal string[] themeEntries = { "Red BMW" , "Blue Audi","Green Lexus" };
        //internal string[] themeValues = { "1", "2","3"  };

        internal string[] themeEntries = { "Red BMW", "Green Lexus" };
        internal string[] themeValues = { "1", "2"};

        #region Ctors
        public ColorThemeListPreference(Context context) : base(context, null)
        {

        }
        public ColorThemeListPreference(Context context, IAttributeSet attrs) : base(context, attrs)
        {

        }
        #endregion

        internal void Init()
        {
            // This is called from the SettingFragment, just to update the Summary. 
            SetEntries(themeEntries);
            SetEntryValues(themeValues);

            if (!string.IsNullOrEmpty(Value))
                Summary = themeEntries[FindIndexOfValue(Value)];  // Get the current theme
            else
                Summary = DefaultThemeValue;
        }
    }
}