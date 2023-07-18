# Room

readme.md


```csharp
using System;

namespace AndroidX.Room {

	[global::Android.Runtime.Annotation ("androidx.room.Database")]
	public partial class DatabaseAttribute : Attribute
	{
		[global::Android.Runtime.Register ("entities")]
		public global::Java.Lang.Class[]? Entities { get; set; }

		[global::Android.Runtime.Register ("exportSchema")]
		public bool ExportSchema { get; set; }

		[global::Android.Runtime.Register ("version")]
		public int Version { get; set; }

		[global::Android.Runtime.Register ("views")]
		public global::Java.Lang.Class[]? Views { get; set; }

	}
}

```