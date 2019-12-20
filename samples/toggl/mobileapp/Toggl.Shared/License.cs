using System;

namespace Toggl.Shared
{
    public struct License : IEquatable<License>
    {
        public string LibraryName { get; set; }

        public string Text { get; set; }

        public License(string libraryName, string text)
        {
            LibraryName = libraryName;
            Text = text;
        }

        public bool Equals(License other)
            => LibraryName == other.LibraryName && Text == other.Text;
    }
}
