using System;

namespace Toggl.Core.UI.Collections.Diffing
{
    public sealed class DuplicateSectionException<TKey> : Exception
    {
        public TKey DuplicatedIdentity { get; }

        public DuplicateSectionException(TKey identity)
        {
            DuplicatedIdentity = identity;
        }
    }
}
