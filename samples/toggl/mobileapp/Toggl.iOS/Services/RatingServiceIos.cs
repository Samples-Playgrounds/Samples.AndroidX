using StoreKit;
using Toggl.Core.Services;

namespace Toggl.iOS.Services
{
    public sealed class RatingServiceIos : IRatingService
    {
        public void AskForRating()
        {
            SKStoreReviewController.RequestReview();
        }
    }
}
