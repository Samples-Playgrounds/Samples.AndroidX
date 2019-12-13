using System;

namespace pdftron
{
    internal static class PlatformInfo
    {
#if (__IOS__)
        internal const string dllName = "__Internal";
#else
        internal const string dllName = "PDFNetC";
#endif
    }
}