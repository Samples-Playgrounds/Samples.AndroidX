using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Content;
using Java.Lang;
using Java.Util;
using AndroidX.ViewPager.Widget;

namespace AppMaterialCardViewTest
{
    public class CustomPagerAdapter : PagerAdapter
    {
        private Context mContext;
        private List<View> pages;

        public CustomPagerAdapter(Context context, List<View> pages)
        {
            this.mContext = context;
            this.pages = pages;
        }


        public override Java.Lang.Object InstantiateItem(ViewGroup container, int position)
        {
            View page = pages[position];
            container.AddView(page);
            return (Java.Lang.Object)page;
        }

        public override bool IsViewFromObject(View view, Java.Lang.Object objectValue)
        {
            return view.Equals(objectValue);
        }


        public override int Count
        {
            get
            {
                return pages.Count;
            }
        }

        public override void DestroyItem(ViewGroup container, int position, Java.Lang.Object objectValue)
        {
            container.RemoveView((View)objectValue);
        }


    }


}