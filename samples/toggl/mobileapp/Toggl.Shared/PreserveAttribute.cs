using System;

namespace Toggl.Shared
{
    public sealed class PreserveAttribute : Attribute
    {
#pragma warning disable SA1401 // Field must be private
        public bool AllMembers;
        public bool Conditional;
#pragma warning restore SA1401 // Field must be private
    }
}
