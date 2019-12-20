using Android.Content;
using Android.Net;
using Toggl.Core.Services;

namespace Toggl.Droid.Services
{
    public sealed class RatingServiceAndroid : IRatingService
    {
        private Context context;

        public RatingServiceAndroid(Context context)
        {
            this.context = context;
        }

        public void AskForRating()
        {
            var packageName = context.PackageName;

            try
            {
                var uriString = $"market://details?id={packageName}";
                executeRatingIntent(uriString);
            }
            catch (ActivityNotFoundException ex)
            {
                var fallbackUriString = $"http://play.google.com/store/apps/details?id={packageName}";
                executeRatingIntent(fallbackUriString);
            }
        }

        private void executeRatingIntent(string uriString)
        {
            Uri uri = Uri.Parse(uriString);
            Intent intent = new Intent(Intent.ActionView, uri);
            intent.AddFlags(ActivityFlags.NoHistory | ActivityFlags.NewDocument | ActivityFlags.MultipleTask);
            context.StartActivity(intent);
        }
    }
}
