using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System.Collections.Generic;

namespace RoomExample
{
    public class NoteAdapter : RecyclerView.Adapter
    {
        private readonly IItemClickListener listener;
        List<Note> notes;

        public NoteAdapter(Context context, IItemClickListener listener)
        {
            this.listener = listener;
            notes = new List<Note>();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item, parent, false);
            NoteAdapterViewHolder vh = new NoteAdapterViewHolder(listener, itemView);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            NoteAdapterViewHolder vh = holder as NoteAdapterViewHolder;

            vh.NoteText.Text = notes[position].Text;
            vh.NoteDate.Text = notes[position].Timestamp;
        }

        public override int ItemCount
        {
            get
            {
                return notes.Count;
            }
        }

        public Note GetNote(int position)
        {
            return notes[position];
        }

        public void ReplaceNotes(List<Note> notes)
        {
            this.notes = notes;
            NotifyDataSetChanged();
        }

    }

    internal class NoteAdapterViewHolder : RecyclerView.ViewHolder,
        View.IOnClickListener
    {

        private readonly IItemClickListener listener;
        public TextView NoteText { get; set; }
        public TextView NoteDate { get; set; }

        internal NoteAdapterViewHolder(IItemClickListener clickHandler, View itemView) : base(itemView)
        {
            listener = clickHandler;
            NoteText = itemView.FindViewById<TextView>(Resource.Id.noteText);
            NoteDate = itemView.FindViewById<TextView>(Resource.Id.noteDate);

            itemView.SetOnClickListener(this);
        }

        public void OnClick(View v)
        {
            listener.OnItemClicked(v, LayoutPosition);
        }
    }
}