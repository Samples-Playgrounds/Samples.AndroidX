using Android.Runtime;
using Android.Views;
using System;
using Toggl.Core.Autocomplete.Suggestions;

namespace Toggl.Droid.ViewHolders
{

    public abstract class SuggestionRecyclerViewHolder<TSuggestion> : BaseRecyclerViewHolder<AutocompleteSuggestion>
        where TSuggestion : AutocompleteSuggestion
    {
        protected SuggestionRecyclerViewHolder(View itemView)
            : base(itemView)
        {
        }

        protected SuggestionRecyclerViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        public TSuggestion Suggestion => Item as TSuggestion;
    }
}
