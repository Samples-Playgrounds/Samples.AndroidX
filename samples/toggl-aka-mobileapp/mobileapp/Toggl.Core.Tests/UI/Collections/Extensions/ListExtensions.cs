using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Collections.Diffing;

namespace Toggl.Core.Tests.UI.Collections.Extensions
{
    public static class ListExtensions
    {
        public static List<TSection> Apply<TSection, THeader, TElement, TKey>(
            this List<TSection> list,
            List<Diffing<TSection, THeader, TElement, TKey>.Changeset> changes)
            where TKey : IEquatable<TKey>
            where TSection : IAnimatableSectionModel<THeader, TElement, TKey>, new()
            where THeader : IDiffable<TKey>
            where TElement : IDiffable<TKey>, IEquatable<TElement>
        {
            return changes.Aggregate(list, (sections, changeset) =>
            {
                var newSections = changeset.Apply(original: sections);
                return newSections;
            });
        }
    }
}
