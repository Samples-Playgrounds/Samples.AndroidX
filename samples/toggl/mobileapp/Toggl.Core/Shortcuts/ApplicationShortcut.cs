namespace Toggl.Core.Shortcuts
{
    public struct ApplicationShortcut
    {
        public string Url { get; }

        public string Title { get; }

        public string Subtitle { get; }

        public ShortcutType Type { get; }

        public ApplicationShortcut(
            string url,
            string title,
            string subtitle,
            ShortcutType type)
        {
            Url = url;
            Type = type;
            Title = title;
            Subtitle = subtitle;
        }

        public ApplicationShortcut(string url, string title, ShortcutType type)
            : this(url, title, null, type)
        {
        }
    }
}
