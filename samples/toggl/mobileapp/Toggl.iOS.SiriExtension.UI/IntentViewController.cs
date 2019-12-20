using CoreAnimation;
using CoreGraphics;
using Foundation;
using Intents;
using IntentsUI;
using System;
using System.Globalization;
using Toggl.iOS.Intents;
using Toggl.iOS.Shared.Extensions;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.SiriExtension.UI
{
    public partial class IntentViewController : UIViewController, IINUIHostedViewControlling
    {

        static NSParagraphStyle paragraphStyle
        {
            get
            {
                var paragraphStyle = new NSMutableParagraphStyle();
                paragraphStyle.LineSpacing = 2;
                return paragraphStyle;
            }
        }

        static readonly UIStringAttributes boldAttributes = new UIStringAttributes
        {
            Font = UIFont.BoldSystemFontOfSize(15),
            ParagraphStyle = paragraphStyle
        };

        static readonly UIStringAttributes regularAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(15),
            ParagraphStyle = paragraphStyle
        };

        protected IntentViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        [Export("configureViewForParameters:ofInteraction:interactiveBehavior:context:completion:")]
        public void ConfigureView(
                 NSSet<INParameter> parameters,
                 INInteraction interaction,
                 INUIInteractiveBehavior interactiveBehavior,
                 INUIHostedViewContext context,
                 INUIHostedViewControllingConfigureViewHandler completion)
        {

            var success = true;
            var desiredSize = CGSize.Empty;

            switch (interaction.Intent)
            {
                case StartTimerIntent startTimerIntent:
                    if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Success)
                    {
                        desiredSize = showStartTimerSuccess(startTimerIntent.EntryDescription);
                    }

                    if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Ready)
                    {
                        var message = string.IsNullOrEmpty(startTimerIntent.EntryDescription)
                            ? Resources.SiriStartTimerWithEmptyDescConfirmationMessage
                            : string.Format(Resources.SiriStartTimerConfirmationMessage,
                                startTimerIntent.EntryDescription);
                        desiredSize = showMessage(message);
                    }

                    if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Failure)
                    {
                        if (interaction.IntentResponse is StartTimerIntentResponse response)
                        {
                            var message = response.UserActivity.GetResponseText();
                            desiredSize = showMessage(message);
                        }
                    }

                    break;
                case StartTimerFromClipboardIntent _:
                    var description = interaction.IntentResponse.UserActivity.GetResponseText();
                    if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Success)
                    {
                        desiredSize = showStartTimerSuccess(description);
                    }

                    if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Ready)
                    {
                        var message = string.IsNullOrEmpty(description)
                            ? Resources.SiriStartTimerWithEmptyDescConfirmationMessage
                            : string.Format(Resources.SiriStartTimerConfirmationMessage, description);
                        desiredSize = showMessage(message);
                    }

                    if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Failure)
                    {
                        if (interaction.IntentResponse is StartTimerFromClipboardIntentResponse response)
                        {
                            var message = response.UserActivity.GetResponseText();
                            desiredSize = showMessage(message);
                        }
                    }
                    break;
                case StopTimerIntent _:
                    if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Success)
                    {
                        if (interaction.IntentResponse is StopTimerIntentResponse response)
                        {
                            desiredSize = showStopResponse(response);
                        }
                    }

                    if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Ready)
                    {
                        var entryDescription = interaction.IntentResponse.UserActivity.GetEntryDescription();
                        var message = string.IsNullOrEmpty(entryDescription)
                            ? Resources.SiriStopTimerWithEmptyDescConfirmationMessage
                            : string.Format(Resources.SiriStopTimerConfirmationMessage, entryDescription);
                        desiredSize = showMessage(message);
                    }

                    if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Failure)
                    {
                        if (interaction.IntentResponse is StopTimerIntentResponse response)
                        {
                            var message = response.UserActivity.GetResponseText();
                            desiredSize = showMessage(message);
                        }
                    }

                    break;

                case ContinueTimerIntent _:
                    if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Success)
                    {
                        if (interaction.IntentResponse is ContinueTimerIntentResponse response)
                        {
                            desiredSize = showStartTimerSuccess(response.EntryDescription);
                        }
                    }

                    if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Ready)
                    {
                        var entryDescription = interaction.IntentResponse.UserActivity.GetEntryDescription();

                        var message = string.IsNullOrEmpty(entryDescription)
                            ? Resources.SiriStartTimerWithEmptyDescConfirmationMessage
                            : string.Format(Resources.SiriStartTimerConfirmationMessage, entryDescription);
                        desiredSize = showMessage(message);
                    }

                    break;
                default:
                    success = false;
                    break;
            }

            completion(success, parameters, desiredSize);
        }

        private CGSize showStartTimerSuccess(string description)
        {
            entryInfoView.DescriptionLabel.Text = string.Empty;
            entryInfoView.TimeLabel.Text = string.Empty;

            var attributedString = new NSMutableAttributedString(string.IsNullOrEmpty(description) ? Resources.NoDescription : description);
            entryInfoView.DescriptionLabel.AttributedText = attributedString;

            var start = DateTimeOffset.Now;
            var displayLink = CADisplayLink.Create(() =>
            {
                var passed = DateTimeOffset.Now - start;
                entryInfoView.TimeLabel.Text = secondsToString(passed.Seconds);
            });
            displayLink.AddToRunLoop(NSRunLoop.Current, NSRunLoopMode.Default);

            View.AddSubview(entryInfoView);
            var width = ExtensionContext?.GetHostedViewMaximumAllowedSize().Width ?? 320;
            var frame = new CGRect(0, 0, width, 60);
            entryInfoView.Frame = frame;

            return frame.Size;
        }

        private CGSize showMessage(string confirmationText)
        {
            confirmationView.ConfirmationLabel.Text = string.Empty;

            var attributedString = new NSMutableAttributedString(confirmationText, boldAttributes);

            var width = ExtensionContext?.GetHostedViewMaximumAllowedSize().Width ?? 320;
            var boundingRect = attributedString.GetBoundingRect(new CGSize(width - 16 * 2, nfloat.MaxValue),
                NSStringDrawingOptions.UsesLineFragmentOrigin | NSStringDrawingOptions.UsesFontLeading, null);

            var frame = new CGRect(0, 0, width, boundingRect.Height + 12 * 2);

            View.AddSubview(confirmationView);

            confirmationView.ConfirmationLabel.AttributedText = attributedString;
            confirmationView.Frame = frame;

            return frame.Size;
        }

        private CGSize showStopResponse(StopTimerIntentResponse response)
        {
            entryInfoView.TimeLabel.Text = secondsToString(response.EntryDuration.DoubleValue);

            var attributedString = new NSMutableAttributedString(response.EntryDescription ?? string.Empty, boldAttributes);

            var startTime = DateTimeOffset.FromUnixTimeSeconds(response.EntryStart.LongValue).ToLocalTime();
            var endTime = DateTimeOffset.FromUnixTimeSeconds(response.EntryStart.LongValue + response.EntryDuration.LongValue).ToLocalTime();
            var fromTime = startTime.ToString("HH:mm", CultureInfo.CurrentCulture);
            var toTime = endTime.ToString("HH:mm", CultureInfo.CurrentCulture);
            var timeFrameString = new NSAttributedString($"\n{fromTime} - {toTime}", regularAttributes);

            attributedString.Append(timeFrameString);
            entryInfoView.DescriptionLabel.AttributedText = attributedString;

            var width = ExtensionContext?.GetHostedViewMaximumAllowedSize().Width ?? 320;
            var boundingRect = attributedString.GetBoundingRect(new CGSize(width - 135 - 16 * 2, nfloat.MaxValue), NSStringDrawingOptions.UsesLineFragmentOrigin | NSStringDrawingOptions.UsesFontLeading, null);

            View.AddSubview(entryInfoView);
            var frame = new CGRect(0, 0, width, boundingRect.Height + 12 * 2);
            entryInfoView.Frame = frame;

            return frame.Size;
        }

        [Export("configureWithInteraction:context:completion:")]
        public void Configure(INInteraction interaction, INUIHostedViewContext context, Action<CGSize> completion)
        {
            throw new NotImplementedException();
        }

        private string secondsToString(Double seconds)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            return timeSpan.ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture);
        }
    }
}
