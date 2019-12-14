namespace Toggl.Core.UI.Collections.Diffing
{
    public sealed class SectionAssociatedData
    {
        public EditEvent EditEvent { get; set; }
        public int? IndexAfterDelete { get; set; }
        public int? MoveIndex { get; set; }
        public int? ItemCount { get; set; }

        public SectionAssociatedData(EditEvent editEvent, int? indexAfterDelete, int? moveIndex, int? itemCount)
        {
            EditEvent = editEvent;
            IndexAfterDelete = indexAfterDelete;
            MoveIndex = moveIndex;
            ItemCount = itemCount;
        }

        public static SectionAssociatedData Initial()
        {
            return new SectionAssociatedData(EditEvent.Untouched, null, null, 0);
        }
    }
}
