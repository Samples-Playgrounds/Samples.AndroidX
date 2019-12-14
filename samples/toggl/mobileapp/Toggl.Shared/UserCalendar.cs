namespace Toggl.Shared
{
    public struct UserCalendar
    {
        public string Id { get; }

        public string Name { get; }

        public string SourceName { get; }

        public bool IsSelected { get; }

        public UserCalendar(
            string id,
            string name,
            string sourceName)
        {
            Id = id;
            Name = name;
            SourceName = sourceName;
            IsSelected = false;
        }

        public UserCalendar(
            string id,
            string name,
            string sourceName,
            bool isSelected)
        {
            Id = id;
            Name = name;
            SourceName = sourceName;
            IsSelected = isSelected;
        }

        public UserCalendar WithSelected(bool selected)
            => new UserCalendar(Id, Name, SourceName, selected);
    }
}
