using System.Collections.Generic;
using System.Collections.ObjectModel;
using Toggl.Shared.Extensions;

namespace Toggl.Shared
{
    public struct RatingViewConfiguration
    {
        public int DayCount { get; }

        public RatingViewCriterion Criterion { get; }

        public RatingViewConfiguration(int dayCount, RatingViewCriterion criterion)
        {
            DayCount = dayCount;
            Criterion = criterion;
        }
    }

    public enum RatingViewCriterion
    {
        None,
        Stop,
        Start,
        Continue
    }

    public static class RatingViewCriterionExtensions
    {
        private static readonly ReadOnlyDictionary<string, RatingViewCriterion> criterionMapping =
            new ReadOnlyDictionary<string, RatingViewCriterion>(new Dictionary<string, RatingViewCriterion>
            {
                { RatingViewCriterion.Stop.ToString().ToUpper(), RatingViewCriterion.Stop },
                { RatingViewCriterion.Start.ToString().ToUpper(), RatingViewCriterion.Start },
                { RatingViewCriterion.Continue.ToString().ToUpper(), RatingViewCriterion.Continue }
            });
        
        public static RatingViewCriterion ToRatingViewCriterion(this string criterion) =>
            criterionMapping.GetOrDefault((criterion != null ? criterion : string.Empty).ToUpper(), RatingViewCriterion.None);
    }
}
