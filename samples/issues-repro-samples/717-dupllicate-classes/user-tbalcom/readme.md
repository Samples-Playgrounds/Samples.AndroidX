# Issue 717 Duplicate classes

*   https://github.com/xamarin/AndroidX/issues/717


# Step 02 - fix errors


```xml
	<ItemGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'" >

		<!-- 
			Fix errors
            Add packages
		-->
		<PackageReference Include="Xamarin.AndroidX.Lifecycle.LiveData" Version="2.5.1.2" />
	</ItemGroup>
```

# Step 01 - use users package references

```xml
	<ItemGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'" >
		<PackageReference Include="Microsoft.AppCenter.Analytics" Version="5.0.1" />
		<PackageReference Include="Microsoft.AppCenter.Crashes" Version="5.0.1" />
		<PackageReference Include="Microsoft.AppCenter.Distribute" Version="5.0.1" />
		<PackageReference Include="Microsoft.Extensions.Logging" Version="7.0.0" />
		<PackageReference Include="MvvmCross" Version="8.0.2" />
		<PackageReference Include="MvvmCross.DroidX.Material" Version="8.0.2" />
		<PackageReference Include="MvvmCross.DroidX.RecyclerView" Version="8.0.2" />
		<PackageReference Include="MvvmCross.Plugin.Messenger" Version="8.0.2" />
		<PackageReference Include="MvvmCross.Plugin.ResxLocalization" Version="8.0.2" />
		<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
		<PackageReference Include="Serilog" Version="2.12.0" />
		<PackageReference Include="Serilog.Extensions.Logging" Version="3.1.0" />
		<PackageReference Include="Serilog.Formatting.Compact" Version="1.1.0" />
		<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
		<PackageReference Include="Serilog.Sinks.Xamarin" Version="1.0.0" />
		<PackageReference Include="sqlite-net-pcl" Version="1.8.116" />
		<PackageReference Include="System.Net.Http" Version="4.3.4" />
		<PackageReference Include="Xamarin.AndroidX.Core.SplashScreen" Version="1.0.0.3" />
		<PackageReference Include="Xamarin.AndroidX.Preference" Version="1.2.0.4" />
		<PackageReference Include="Xamarin.AndroidX.Work.Runtime" Version="2.7.1.6" />
		<PackageReference Include="Xamarin.AndroidX.ConstraintLayout" Version="2.1.4.3" />
		<PackageReference Include="Xamarin.AndroidX.CardView" Version="1.0.0.18" />
		<PackageReference Include="Xamarin.Essentials" Version="1.7.5" />
		<PackageReference Include="Xamarin.Firebase.Messaging" Version="123.1.1.1" />
		<PackageReference Include="Xamarin.Google.Android.Material" Version="1.8.0" />
		<PackageReference Include="Xamarin.Google.Dagger" Version="2.44.2.1" />
		<PackageReference Include="Xamarin.GooglePlayServices.Location" Version="120.0.0.1" />
		<PackageReference Include="Xamarin.GooglePlayServices.Maps" Version="118.1.0.1" />
		<PackageReference Include="Xamarin.Kotlin.StdLib.Jdk8" Version="1.8.10" />
		<PackageReference Include="ZXing.Net.Mobile" Version="3.1.0-beta2" />

		<!-- 
			Fix errors
            Add packages
		-->
		<PackageReference Include="Xamarin.AndroidX.Lifecycle.LiveData" Version="2.5.1.2" />
	</ItemGroup>
```

Warnings/Errors:

```
```