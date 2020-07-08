using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AppMaterialCardViewTest
{
    public class DayRasp
    {
        public DateTime date { get; set; }
        public string dateStr 
        {
            get
            {
                int DayOfWeek = (int)date.DayOfWeek;
                return date.ToString("M") + ", " + date.DayOfWeek.ToString();
            }

            set
            {
                dateStr = value;
            }
        }
        public List<LessonRasp> lessons { get; set; }
    }

    public class LessonRasp
    {
        public DateTime date { get; set; }
        public int num { get; set; }
        public string time { get; set; }
        public string name { get; set; }
        public string teacherName { get; set; }
        //public int weekDay { get; set; }
        //public DateTime date { get; set; }

        public string cabinet { get; set; }
        public string homeWork { get; set; }
        public string marks { get; set; }
        public bool hasAttachment { get; set; }
        public bool hasNote { get; set; }

        public void SetDemo()
        {
            //num = 1;
            time = "8:00 - 8:45";
            name = "Pyotr Tchaikovsky";
            teacherName = "composer";
            cabinet = "room 302";
            date = DateTime.Today;
            hasAttachment = true;
        }
    }
}
