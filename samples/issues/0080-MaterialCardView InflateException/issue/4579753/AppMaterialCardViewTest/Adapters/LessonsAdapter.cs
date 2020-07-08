using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;

namespace AppMaterialCardViewTest
{
    public class LessonsAdapter : RecyclerView.Adapter
    {
        public LessonRasp[] mLesItems;
        Activity actThis;
        public event EventHandler ItemWasClicked;

        public LessonsAdapter(LessonRasp[] data, Activity act)
        {
            mLesItems = data;
            actThis = act;
        }

        public override RecyclerView.ViewHolder
        OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                       Inflate(Resource.Layout.cv_lesson2, parent, false);

            LessonViewHolder vh = new LessonViewHolder(itemView, LessonOnClick);
            return vh;
        }

        public override void
            OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            LessonViewHolder vh = holder as LessonViewHolder;
            vh.rectangle_with_colorV.SetBackgroundColor(Color.ParseColor("#657688"));
            vh.lesson_numTV.Text = mLesItems[position].num.ToString();
            vh.lesson_timeTV.Text = mLesItems[position].time.ToString();
            vh.lesson_nameTV.Text = mLesItems[position].name.ToString();

            var lesson_teacher = "";
            if (mLesItems[position].teacherName != null)
                lesson_teacher += mLesItems[position].teacherName.ToString();
            if (mLesItems[position].cabinet != null)
            {
                if (lesson_teacher != "")
                    lesson_teacher += " • ";

                lesson_teacher += mLesItems[position].cabinet.ToString();
            }

            vh.lesson_teacher_cabTV.Text = lesson_teacher;

            if (!mLesItems[position].hasAttachment)
                vh.attach_iconIV.Visibility = ViewStates.Gone;
        }

        public override int ItemCount
        {
            get { return mLesItems.Length; }
        }

        private void LessonOnClick(int obj, View view)
        {
            LessonRasp itItem = mLesItems[obj];
            //var id = mLesItems[obj].id;    
            ItemWasClicked(this, new LessonItemClickedEventArgs(itItem));
        }

    }

    public class LessonItemClickedEventArgs : EventArgs
    {
        public LessonItemClickedEventArgs(LessonRasp item)
        {
            Item = item;
        }

        public LessonRasp Item { get; set; }
    }
}