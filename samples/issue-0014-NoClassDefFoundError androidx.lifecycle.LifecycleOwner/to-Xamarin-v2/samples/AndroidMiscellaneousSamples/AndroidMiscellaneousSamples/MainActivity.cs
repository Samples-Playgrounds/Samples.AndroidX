using System.Reflection;
using Android.App;
using Android.OS;
using Xamarin.Android.NUnitLite;

namespace MiscellaneousSamples
{
	[Activity (Label = "MiscellaneousSamples", MainLauncher = true)]
	public class MainActivity : TestSuiteActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			// tests can be inside the main assembly
			AddTest (Assembly.GetExecutingAssembly ());
			// or in any reference assemblies
			// AddTest (typeof (Your.Library.TestClass).Assembly);

			// Once you called base.OnCreate(), you cannot add more assemblies.
			base.OnCreate (bundle);

            string licenseKey = pdftron.Demo.Key.LicenseKey;
            pdftron.PDFNet.Initialize (this, Resource.Raw.pdfnet, licenseKey);
		}

    }
}

