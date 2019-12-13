using System;

using pdftron.SDF;

namespace pdftron.PDF
{
    public class OptionsBase : IDisposable
    {
        protected ObjSet mObjSet;
        protected Obj mDict;

        public OptionsBase()
        {
            mObjSet = new ObjSet();
            mDict = mObjSet.CreateDict();
        }

        ~OptionsBase()
        {
            Dispose(false);
        }

        public static double ColorPtToNumber(ColorPt cp)
        {
            int num_comp = 3;
            int red = 0;
            int green = 0;
            int blue = 0;
            int alpha = 255;
            if (num_comp == 1)
            {
                red = (int)(255 * cp.Get(0));
                green = red;
                blue = red;
            }
            else if (num_comp == 3 || num_comp == 4)
            {
                red = (int)(255 * cp.Get(0));
                green = (int)(255 * cp.Get(1));
                blue = (int)(255 * cp.Get(2));
            }
            if (num_comp == 4)
            {
                alpha = (int)(255 * cp.Get(3));
            }
            var num = ((0xFF & alpha) << 24)
                    | ((0xFF & red) << 16)
                    | ((0xFF & green) << 8)
                    | ((0xFF & blue) << 0);
            return (double)num;
        }

        public static ColorPt ColorPtFromNumber(double dnum)
        {
            var num = (System.UInt32)dnum;
            return new ColorPt(((num >> 16) & 0xFF) / 255.0,
                ((num >> 8) & 0xFF) / 255.0,
                ((num >> 0) & 0xFF) / 255.0,
                ((num >> 24) & 0xFF) / 255.0);
        }

        protected SDF.Obj GetArray(String key)
        {
            Obj found = mDict.FindObj(key);
            if(found == null)
            {
                found = mDict.PutArray(key);
            }
            return found;
        }

        protected void PutNumber(String key, double num)
        {
            mDict.PutNumber(key, num);
        }

        protected void PutBool(String key, bool val)
        {
            mDict.PutBool(key, val);
        }

        protected void PutText(String key, String text)
        {
            mDict.PutText(key, text);
        }

        protected void PutRect(String key, Rect rect)
        {
            mDict.PutRect(key, rect.x1, rect.y1, rect.x2, rect.y2);
        }

        protected void PushBackNumber(String key, double num)
        {
            SDF.Obj arr = GetArray(key);
            arr.PushBackNumber(num);
        }

        protected void PushBackBool(String key, bool val)
        {
            SDF.Obj arr = GetArray(key);
            arr.PushBackBool(val);
        }

        protected void PushBackText(String key, String text)
        {
            SDF.Obj arr = GetArray(key);
            arr.PushBackText(text);
        }

        protected void PushBackRect(String key, Rect rect)
        {
            SDF.Obj arr = GetArray(key);
            arr.PushBackRect(rect.x1, rect.y1, rect.x2, rect.y2);
        }

        protected Rect RectFromArray(SDF.Obj nums)
        {
            if(nums == null)
            {
                return new Rect();
            }
            return new Rect(nums.GetAt(0).GetNumber(), nums.GetAt(1).GetNumber(),
                nums.GetAt(2).GetNumber(), nums.GetAt(3).GetNumber());
        }

        protected void insertRectCollection(String key, RectCollection rects, int index)
        {
            SDF.Obj arr = GetArray(key);
            while(arr.Size() <= index)
            {
                arr.PushBackArray();
            }
            SDF.Obj innerArray = arr.GetAt(index);
            for (int i = 0; i < rects.GetNumRects(); ++i)
            {
                Rect rect = rects.GetRectAt(i);
                innerArray.PushBackRect(rect.x1, rect.y1, rect.x2, rect.y2);
            }
        }
        

        public Obj GetInternalObj()
        {
            return mDict;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            Destroy();
        }

        public void Destroy()
        {
        }
    }
}
