using Android.Text;
using Android.Text.Style;
using Android.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Extensions
{
    public static class SpannableExtensions
    {
        public static ISpannable SetClickableSpan(this ISpannable spannable, int indexOfWord, int wordLength, ViewAction action)
        {
            spannable.SetSpan(
                new ActionClickableSpan(action),
                indexOfWord,
                indexOfWord + wordLength,
                SpanTypes.ExclusiveExclusive);

            return spannable;
        }

        private sealed class ActionClickableSpan : ClickableSpan
        {
            private readonly ViewAction viewAction;

            public ActionClickableSpan(ViewAction viewAction)
            {
                Ensure.Argument.IsNotNull(viewAction, nameof(viewAction));

                this.viewAction = viewAction;
            }

            public override void OnClick(View widget)
            {
                viewAction.Execute();
            }
        }
    }
}
