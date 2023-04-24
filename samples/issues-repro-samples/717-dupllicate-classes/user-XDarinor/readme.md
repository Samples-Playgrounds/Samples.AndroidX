# Issue 717 Duplicate classes

*   https://github.com/xamarin/AndroidX/issues/717


# Step 01 - use users package references

```xml
	<ItemGroup Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'android'" >
		<PackageReference Include="Microsoft.AppCenter.Analytics">
		<Version>5.0.1</Version>
		</PackageReference>
		<PackageReference Include="Microsoft.AppCenter.Crashes">
		<Version>5.0.1</Version>
		</PackageReference>
		<PackageReference Include="NLog">
		<Version>5.1.3</Version>
		</PackageReference>
		<PackageReference Include="Polly">
		<Version>7.2.3</Version>
		</PackageReference>
		<PackageReference Include="Syncfusion.Licensing">
		<Version>21.1.38</Version>
		</PackageReference>
		<PackageReference Include="Syncfusion.Xamarin.SfAutoComplete.Android">
		<Version>21.1.38</Version>
		</PackageReference>
		<PackageReference Include="Syncfusion.Xamarin.SfRotator.Android">
		<Version>21.1.38</Version>
		</PackageReference>
		<PackageReference Include="Syncfusion.Xamarin.SfSchedule.Android">
		<Version>21.1.38</Version>
		</PackageReference>
		<PackageReference Include="Syncfusion.Xamarin.SfTreeView.Android">
		<Version>21.1.38</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.Android.Support.v7.RecyclerView">
		<Version>28.0.0.3</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.AndroidX.AppCompat">
		<Version>1.6.0.1</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.AndroidX.Fragment">
		<Version>1.5.5.1</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.AndroidX.Legacy.Support.Core.UI">
		<Version>1.0.0.17</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.AndroidX.Lifecycle.LiveData">
		<Version>2.5.1.2</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.AndroidX.Migration">
		<Version>1.0.10</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.AndroidX.Navigation.Fragment">
		<Version>2.5.3.1</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.AndroidX.Navigation.Runtime">
		<Version>2.5.3.1</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.AndroidX.Navigation.UI">
		<Version>2.5.3.1</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.AndroidX.Startup.StartupRuntime">
		<Version>1.1.1.4</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.Essentials" Version="1.7.5" />
		<PackageReference Include="Xamarin.Google.Android.DataTransport.TransportBackendCct">
		<Version>3.1.8.1</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.Google.Android.DataTransport.TransportRuntime">
		<Version>3.1.8.1</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.Google.Android.Material">
		<Version>1.8.0</Version>
		</PackageReference>
		<PackageReference Include="Realm">
		<Version>10.21.0</Version>
		</PackageReference>
		<PackageReference Include="Newtonsoft.Json">
		<Version>13.0.3</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.Firebase.Messaging">
		<Version>123.1.1.1</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.Google.Dagger">
		<Version>2.44.2.1</Version>
		</PackageReference>
		<PackageReference Include="Xamarin.GooglePlayServices.Base">
		<Version>118.1.0.1</Version>
		</PackageReference>
		<PackageReference Include="Microsoft.Identity.Client">
		<Version>4.52.0</Version>
		</PackageReference>
	</ItemGroup>
```


Warnings/Errors:

```
/usr/local/share/dotnet/sdk/7.0.203/Sdks/Microsoft.NET.Sdk/targets/Microsoft.NET.Sdk.DefaultItems.Shared.targets(152,5): warning NETSDK1023: A PackageReference for 'Xamarin.AndroidX.Lifecycle.LiveData' was included in your project. This package is implicitly referenced by the .NET SDK and you do not typically need to reference it from your project. For more information, see https://aka.ms/sdkimplicitrefs [./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj::TargetFramework=net7.0-android]
/usr/local/share/dotnet/sdk/7.0.203/Sdks/Microsoft.NET.Sdk/targets/Microsoft.NET.Sdk.DefaultItems.Shared.targets(152,5): warning NETSDK1023: A PackageReference for 'Xamarin.AndroidX.Navigation.Fragment' was included in your project. This package is implicitly referenced by the .NET SDK and you do not typically need to reference it from your project. For more information, see https://aka.ms/sdkimplicitrefs [./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj::TargetFramework=net7.0-android]
/usr/local/share/dotnet/sdk/7.0.203/Sdks/Microsoft.NET.Sdk/targets/Microsoft.NET.Sdk.DefaultItems.Shared.targets(152,5): warning NETSDK1023: A PackageReference for 'Xamarin.AndroidX.Navigation.Runtime' was included in your project. This package is implicitly referenced by the .NET SDK and you do not typically need to reference it from your project. For more information, see https://aka.ms/sdkimplicitrefs [./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj::TargetFramework=net7.0-android]
/usr/local/share/dotnet/sdk/7.0.203/Sdks/Microsoft.NET.Sdk/targets/Microsoft.NET.Sdk.DefaultItems.Shared.targets(152,5): warning NETSDK1023: A PackageReference for 'Xamarin.AndroidX.Navigation.UI' was included in your project. This package is implicitly referenced by the .NET SDK and you do not typically need to reference it from your project. For more information, see https://aka.ms/sdkimplicitrefs [./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj::TargetFramework=net7.0-android]
/usr/local/share/dotnet/sdk/7.0.203/Sdks/Microsoft.NET.Sdk/targets/Microsoft.NET.Sdk.DefaultItems.Shared.targets(152,5): warning NETSDK1023: A PackageReference for 'Xamarin.Google.Android.Material' was included in your project. This package is implicitly referenced by the .NET SDK and you do not typically need to reference it from your project. For more information, see https://aka.ms/sdkimplicitrefs [./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj::TargetFramework=net7.0-android]
./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj : warning NU1608: Detected package version outside of dependency constraint: Xamarin.Essentials 1.7.5 requires Xamarin.AndroidX.Browser (>= 1.3.0.5 && < 1.4.0.3) but version Xamarin.AndroidX.Browser 1.4.0.3 was resolved. [./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.sln]


./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj : error NU1605: Warning As Error: Detected package downgrade: Xamarin.AndroidX.Navigation.Common from 2.5.3.1 to 2.5.2.1. Reference the package directly from the project to select a different version.  [./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.sln]
./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj : error NU1605:  AppMAUI.Issue717.DuplicateClasses -> Xamarin.AndroidX.Navigation.Runtime 2.5.3.1 -> Xamarin.AndroidX.Navigation.Common (>= 2.5.3.1)  [./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.sln]
./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj : error NU1605:  AppMAUI.Issue717.DuplicateClasses -> Xamarin.AndroidX.Navigation.Common (>= 2.5.2.1) [./samples/issues-repro-samples/717-dupllicate-classes/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.sln]

```