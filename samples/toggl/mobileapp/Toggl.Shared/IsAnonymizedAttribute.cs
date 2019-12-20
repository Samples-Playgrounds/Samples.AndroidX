using System;

namespace Toggl.Shared
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class IsAnonymizedAttribute : Attribute
    {
    }
}
