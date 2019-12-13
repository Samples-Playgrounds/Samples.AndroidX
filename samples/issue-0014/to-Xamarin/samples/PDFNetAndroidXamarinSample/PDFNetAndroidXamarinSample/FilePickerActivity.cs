namespace com.xamarin.recipes.filepicker
{
    using Android.App;
    using Android.OS;
    using Android.Support.V4.App;
	using PDFNetAndroidXamarinSample;

	[Activity(Label = "@string/file_browser_name")]
    public class FilePickerActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			SetContentView(Resource.Layout.file_picker_activity);
        }
    }
}
