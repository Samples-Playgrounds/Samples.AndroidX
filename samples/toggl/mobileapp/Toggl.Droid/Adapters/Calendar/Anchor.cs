namespace Toggl.Droid.Adapters.Calendar
{
    public class Anchor : Java.Lang.Object
    {
        private readonly int height;
        public AnchorData[] AnchoredData { get; }

        public int Height => height;

        public Anchor(int height, AnchorData[] anchorData)
        {
            this.height = height;
            AnchoredData = anchorData;
        }

    }
}
