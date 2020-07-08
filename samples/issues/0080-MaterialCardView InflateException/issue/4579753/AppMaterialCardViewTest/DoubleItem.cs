using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AppMaterialCardViewTest
{
    public class DoubleItem
    {
        public DoubleItem(string primary, string secondary)
        {
            Primary = primary;
            Secondary = secondary;
        }

        public string Primary { get; set; }
        public string Secondary { get; set; }
    }
}