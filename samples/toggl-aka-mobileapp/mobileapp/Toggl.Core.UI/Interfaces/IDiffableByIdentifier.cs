using System;

namespace Toggl.Core.UI.Interfaces
{
    public interface IDiffableByIdentifier<T> : IEquatable<T>
    {
        long Identifier { get; }
    }
}
