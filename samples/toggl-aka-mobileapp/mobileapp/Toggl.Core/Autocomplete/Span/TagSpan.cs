using Toggl.Core.Models.Interfaces;

namespace Toggl.Core.Autocomplete.Span
{
    public sealed class TagSpan : ISpan
    {
        public long TagId { get; set; }

        public string TagName { get; set; }

        public TagSpan(long tagId, string tagName)
        {
            TagId = tagId;
            TagName = tagName;
        }

        public TagSpan(IThreadSafeTag tag)
            : this(tag.Id, tag.Name)
        {
        }
    }
}
