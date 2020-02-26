## Problem

Changing `type` (java) with managed FQCVlassName works:

https://github.com/moljac/Samples.AndroidX/blob/master/source/bindings/type-vs-managedType/source/androidx.leanback/leanback-preference/transforms/Metadata.xml#L6-L7

```
    <attr path="/api/package[@name='androidx.leanback.preference']/class[@name='LeanbackListPreferenceDialogFragment.AdapterMulti']/method[@name='onBindViewHolder']/parameter[1]" name="type">AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder</attr>
    <attr path="/api/package[@name='androidx.leanback.preference']/class[@name='LeanbackListPreferenceDialogFragment.AdapterSingle']/method[@name='onBindViewHolder']/parameter[1]" name="type">AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder</attr>
```

while changing `managedType` causes build errors (does not change managed type):

https://github.com/moljac/Samples.AndroidX/blob/master/source/bindings/type-vs-managedType/source/androidx.leanback/leanback-preference/transforms/Metadata.xml#L26-L36

```
    <attr 
        path="/api/package[@name='androidx.leanback.preference']/class[@name='LeanbackListPreferenceDialogFragment.AdapterMulti']/method[@name='onBindViewHolder']/parameter[1]" 
        name="managedType"
        >
        AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder</attr>
    <attr 
        path="/api/package[@name='androidx.leanback.preference']/class[@name='LeanbackListPreferenceDialogFragment.AdapterSingle']/method[@name='onBindViewHolder']/parameter[1]" 
        name="managedType"
        >
        AndroidX.RecyclerView.Widget.RecyclerView.ViewHolder
    </attr>
```
Errors:

```
```

## References Links

Solution (minimal repro) is zipped/archived in:

https://github.com/moljac/Samples.AndroidX/blob/master/source/bindings/type-vs-managedType.zip

`api.xml`

https://github.com/moljac/Samples.AndroidX/blob/master/source/bindings/type-vs-managedType/api.xml
