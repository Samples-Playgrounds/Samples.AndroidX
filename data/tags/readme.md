# Readme

AndroidX tags

Steps for issue 0064 Bundle Assemblies and 

1.  copy folder and rename it

    ```
    20200214-stable-release                         20200603-stable-release-20200214
    20200316-stable-release-previous-GPS-FB         20200603-stable-release-20200316
    20200318-stable-release                         20200603-stable-release-20200318
    ```

2.  rename

3.  check additional projects in `config.json`

    ```
		"source/migration/Dummy/Xamarin.AndroidX.Migration.Dummy.csproj",
		"source/androidx.appcompat/typeforwarders/androidx.appcompat.appcompat-resources-typeforwarders.cspro
    ```

4.  bump revision on all AndroidX packages

5.  bump `Xamarin.AndroidX.Migration`

