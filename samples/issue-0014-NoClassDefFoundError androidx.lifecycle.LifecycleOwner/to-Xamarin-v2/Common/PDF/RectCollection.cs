using System.Collections;
namespace pdftron.PDF
{
    public class RectCollection
    {
    	private ArrayList mRectList = new ArrayList();
        public RectCollection()
        {
        }
        
        public void AddRect(Rect to_add)
		{
			mRectList.Add(new Rect(to_add.x1, to_add.y1, to_add.x2, to_add.y2));
		}
		
		public void AddRect(double x1, double y1, double x2, double y2)
		{
			mRectList.Add(new Rect(x1, y1, x2, y2));
		}
		
		public Rect GetRectAt(int index)
		{
			return (Rect)(mRectList[index]);
		}
		
		public int GetNumRects()
		{
			return mRectList.Count;	
		}

		public void Clear()
		{
			mRectList.Clear();
		}
    }
}