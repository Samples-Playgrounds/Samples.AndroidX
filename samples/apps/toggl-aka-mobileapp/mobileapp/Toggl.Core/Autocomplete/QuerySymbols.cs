namespace Toggl.Core.Autocomplete
{
    public static class QuerySymbols
    {
        public const char Projects = '@';
        public const string ProjectsString = "@";

        public const char Tags = '#';
        public const string TagsString = "#";

        public static readonly char[] All = { Projects, Tags };
        public static readonly char[] ProjectSelected = { Tags };
    }
}
