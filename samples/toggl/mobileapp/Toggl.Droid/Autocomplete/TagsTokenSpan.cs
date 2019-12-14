namespace Toggl.Droid.Autocomplete
{
    public sealed class TagsTokenSpan : TokenSpan
    {
        public long TagId { get; }

        public string TagName { get; }

        public TagsTokenSpan(long tagId, string tagName)
            : base(Resource.Color.tagToken, Resource.Color.tagToken, true)
        {
            TagId = tagId;
            TagName = tagName;
        }
    }
}
