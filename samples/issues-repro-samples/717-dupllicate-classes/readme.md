# Readme

readme.md

## tipa

```
dotnet build user-tipa/AndroidApp1/AndroidApp1.csproj
```

could reproduce.

```
    <PackageReference Include="Xamarin.AndroidX.Camera.Camera2" Version="1.2.2" />
```

Published month ago (message on 20230430)


```
    <PackageReference Include="Xamarin.AndroidX.Preference" Version="1.2.0.4" />
```

Published 3 months ago (message on 20230430)


## XDarinor

```
dotnet build user-XDarinor/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj
```

Could NOT reproduce.

Warnings only

```
./717-dupllicate-classes/user-XDarinor/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj : 
    warning NU1608: Detected package version outside of dependency constraint: Xamarin.Essentials 1.7.5 requires Xamarin.AndroidX.Browser (>= 1.3.0.5 && < 1.4.0.3) but version Xamarin.AndroidX.Browser 1.4.0.3 was resolved. 
[TargetFramework=net7.0-android]
./717-dupllicate-classes/user-XDarinor/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj : 
    warning NU1608: Detected package version outside of dependency constraint: Xamarin.Essentials 1.7.5 requires Xamarin.AndroidX.Browser (>= 1.3.0.5 && < 1.4.0.3) but version Xamarin.AndroidX.Browser 1.4.0.3 was resolved. 
[TargetFramework=net7.0-android]
```


## tbalcom

```
dotnet build user-tbalcom/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj 
```

Could NOT reproduce.

Warnings only

```
./717-dupllicate-classes/user-tbalcom/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj : 
    warning NU1608: Detected package version outside of dependency constraint: Xamarin.Essentials 1.7.5 requires Xamarin.AndroidX.Browser (>= 1.3.0.5 && < 1.4.0.3) but version Xamarin.AndroidX.Browser 1.4.0.3 was resolved. 
[TargetFramework=net7.0-android]
./717-dupllicate-classes/user-tbalcom/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj : 
    warning NU1608: Detected package version outside of dependency constraint: Xamarin.Essentials 1.7.5 requires Xamarin.AndroidX.Browser (>= 1.3.0.5 && < 1.4.0.3) but version Xamarin.AndroidX.Browser 1.4.0.3 was resolved. 
[TargetFramework=net7.0-android]
./717-dupllicate-classes/user-tbalcom/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj : 
    warning NU1608: Detected package version outside of dependency constraint: Xamarin.Essentials 1.7.5 requires Xamarin.AndroidX.Browser (>= 1.3.0.5 && < 1.4.0.3) but version Xamarin.AndroidX.Browser 1.4.0.3 was resolved. 
[TargetFramework=net7.0-android]
```




## wfhm

Could reproduce

```
dotnet build user-wfhm/AppMAUI.Issue717.DuplicateClasses/AppMAUI.Issue717.DuplicateClasses.csproj
```

Could NOT reproduce.

```
dotnet build user-wfhm/AppAndroid.Issue717.DuplicateClasses/AppAndroid.Issue717.DuplicateClasses.csproj
```

and 

```
dotnet build user-wfhm/LibAndroid.Issue717.DuplicateClasses/LibAndroid.Issue717.DuplicateClasses.csproj
```
