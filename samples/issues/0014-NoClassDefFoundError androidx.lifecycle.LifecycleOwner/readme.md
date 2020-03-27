# Issue #14

```
Binding AAR library that contains AndroidX cause "NoClassDefFoundError" for "androidx/lifecycle/LifecycleOwner" #14
```

*   https://github.com/xamarin/AndroidX/issues/14

*   repro sample link

    *   http://pdftron.s3.amazonaws.com/custom/external/xamarin/androidx/to-Xamarin.zip

    
```
  <ItemGroup>
    <EmbeddedJar Include="..\..\commons-io-2.4.jar">
      <Link>Jars\commons-io-2.4.jar</Link>
    </EmbeddedJar>
  </ItemGroup>
  <ItemGroup>
    <EmbeddedJar Include="..\..\commons-lang3-3.5.jar">
      <Link>Jars\commons-lang3-3.5.jar</Link>
    </EmbeddedJar>
  </ItemGroup>
```1