using Foundation;
using Newtonsoft.Json.Converters;
using System;
using Toggl.iOS.Views;

namespace Toggl.iOS
{
    // This class is never actually executed, but when Xamarin linking is enabled it does ensure types and properties
    // are preserved in the deployed app
    [Preserve(AllMembers = true)]
    public sealed class LinkerPleaseInclude
    {
        public void Include(ConsoleColor color)
        {
            Console.Write("");
            Console.WriteLine("");
            color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.ForegroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        public void Include(TextViewWithPlaceholder textView)
        {
            textView.TextColor = textView.TextColor;
        }

        public void Include(StringEnumConverter converter)
        {
            converter = new StringEnumConverter(true);
        }
    }
}
