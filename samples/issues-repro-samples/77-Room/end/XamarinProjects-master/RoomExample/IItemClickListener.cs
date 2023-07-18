using Android.Views;

namespace RoomExample
{
    public interface IItemClickListener
    {
        public void OnItemClicked(View view, int position);
    }
}