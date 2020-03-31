using System;

namespace Toggl.Networking.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class IgnoreSerializationAttribute : Attribute
    {
    }
}
