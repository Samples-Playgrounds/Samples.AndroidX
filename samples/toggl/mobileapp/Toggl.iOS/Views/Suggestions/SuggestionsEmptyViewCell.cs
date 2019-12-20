using Foundation;
using System;
using UIKit;

namespace Toggl.iOS.Views
{
    public partial class SuggestionsEmptyViewCell : UITableViewCell
    {
        private static readonly Random Random = new Random();

        private const int minTaskWidth = 74;
        private const int maxTaskWidth = 84;
        private const int minProjectWidth = 42;
        private const int maxProjectWidth = 84;
        private const int minDescriptionWidth = 74;
        private const int maxDescriptionWidth = 110;

        private static readonly UIColor[] Colors =
        {
            UIColor.FromRGB(197f / 255f, 107f / 255f, 255f / 255f),
            UIColor.FromRGB(006f / 255f, 170f / 255f, 245f / 255f),
            UIColor.FromRGB(241f / 255f, 195f / 255f, 063f / 255f)
        };

        public static readonly NSString Key = new NSString(nameof(SuggestionsEmptyViewCell));
        public static readonly UINib Nib;

        static SuggestionsEmptyViewCell()
        {
            Nib = UINib.FromName(nameof(SuggestionsEmptyViewCell), NSBundle.MainBundle);
        }

        protected SuggestionsEmptyViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            TaskWidth.Constant = Random.Next(minTaskWidth, maxTaskWidth);
            ProjectWidth.Constant = Random.Next(minProjectWidth, maxProjectWidth);
            ProjectView.BackgroundColor = Colors[Random.Next(0, Colors.Length)];
            DescriptionWidth.Constant = Random.Next(minDescriptionWidth, maxDescriptionWidth);

            SetNeedsLayout();
            SetNeedsUpdateConstraints();
        }
    }
}
