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
using AndroidX.RecyclerView.Widget;

namespace AppMaterialCardViewTest
{
    public class LessonViewHolder : RecyclerView.ViewHolder
    {
        public View rectangle_with_colorV;
        public TextView lesson_numTV;
        public TextView lesson_timeTV;
        public TextView lesson_teacher_cabTV;
        public TextView lesson_nameTV;
        public ImageView attach_iconIV;

        public LessonViewHolder(View itemView, Action<int, View> listener) : base(itemView)
        {
            // Locate and cache view references:
            rectangle_with_colorV = itemView.FindViewById<View>(Resource.Id.rectangle_with_colorView);
            lesson_numTV = itemView.FindViewById<TextView>(Resource.Id.lesson_num);
            lesson_timeTV = itemView.FindViewById<TextView>(Resource.Id.lesson_time);
            lesson_teacher_cabTV = itemView.FindViewById<TextView>(Resource.Id.lesson_teacher_cab);
            lesson_nameTV = itemView.FindViewById<TextView>(Resource.Id.lesson_name);
            attach_iconIV = itemView.FindViewById<ImageView>(Resource.Id.attach_icon);

            itemView.Click += (sender, e) => listener(base.Position, itemView);
        }
    }
}