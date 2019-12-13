using System;
using System.IO;
using System.Collections.Generic;

namespace MiscellaneousSamples
{
	public class Utils 
	{
		public static Stream GetAssetStream(string filePath)
		{
			try
			{
				return Android.App.Application.Context.Resources.Assets.Open(filePath);
			}
			catch (Exception e) 
			{
				Console.WriteLine (e.Message);
				return null;
			}
		}

		public static string GetAssetTempFile(string filePath)
		{
            string path = null;
			try
			{
                var ext = Path.GetExtension(filePath);
                path = Path.Combine(Android.App.Application.Context.CacheDir.AbsolutePath, Path.GetRandomFileName());
                path = Path.ChangeExtension(path, ext);

                using (Stream source = GetAssetStream(filePath)) 
                {
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        source.CopyTo(fileStream);
                    }
                }
			}
			catch (Exception e) 
			{
				Console.WriteLine (e.Message);
				return null;
			}
            return path == null ? "" : path;
		}

		public static string CreateExternalFile(string fileName)
		{
			return new Java.IO.File(Android.App.Application.Context.GetExternalFilesDir(null), fileName).AbsolutePath;
		}
	}
}