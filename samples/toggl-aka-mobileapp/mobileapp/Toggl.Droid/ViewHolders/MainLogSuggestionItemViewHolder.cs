using System;
using System.Diagnostics;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Core.Suggestions;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.ViewHolders
{
    public sealed class MainLogSuggestionItemViewHolder : BaseRecyclerViewHolder<Suggestion>
    {
        private TextView descriptionLabel;
        private TextView projectLabel;
        private TextView clientLabel;

        public static MainLogSuggestionItemViewHolder Create(View item)
            => new MainLogSuggestionItemViewHolder(item);

        public MainLogSuggestionItemViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MainLogSuggestionItemViewHolder(View itemView) : base(itemView)
        {
        }

        protected override void InitializeViews()
        {
            descriptionLabel = ItemView.FindViewById<TextView>(Resource.Id.DescriptionLabel);
            projectLabel = ItemView.FindViewById<TextView>(Resource.Id.ProjectLabel);
            clientLabel = ItemView.FindViewById<TextView>(Resource.Id.ClientLabel);
        }

        protected override void UpdateView()
        {
            descriptionLabel.Text = Item.Description;
            prefixWithProviderNameInDebug();
            descriptionLabel.Visibility = (!string.IsNullOrWhiteSpace(Item.Description)).ToVisibility();

            projectLabel.Text = Item.ProjectName;
            projectLabel.SetTextColor(Color.ParseColor(Item.ProjectColor));
            projectLabel.Visibility = Item.HasProject.ToVisibility();

            clientLabel.Text = Item.ClientName;
            clientLabel.Visibility = Item.HasProject.ToVisibility();
        }

        [Conditional("DEBUG")]
        private void prefixWithProviderNameInDebug()
        {
            var prefix = Item.ProviderType.ToString().Substring(0, 4);
            descriptionLabel.Text = $"{prefix} {Item.Description}";
        }
    }
}
