namespace Toggl.Core.UI.Collections.Diffing
{
    public sealed class ItemAssociatedData
    {
        public EditEvent EditEvent { get; set; }
        public int? IndexAfterDelete { get; set; }
        public ItemPath MoveIndex { get; set; }

        public ItemAssociatedData(EditEvent editEvent, int? indexAfterDelete, ItemPath moveIndex)
        {
            EditEvent = editEvent;
            IndexAfterDelete = indexAfterDelete;
            MoveIndex = moveIndex;
        }

        public static ItemAssociatedData Initial()
        {
            return new ItemAssociatedData(EditEvent.Untouched, null, null);
        }
    }
}
