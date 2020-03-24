using System;
using Toggl.Shared;

namespace Toggl.Networking.Serialization.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    internal sealed class RequiresFeatureAttribute : Attribute
    {
        public WorkspaceFeatureId RequiredFeature { get; }

        public RequiresFeatureAttribute(WorkspaceFeatureId requiredFeature)
        {
            RequiredFeature = requiredFeature;
        }
    }
}
