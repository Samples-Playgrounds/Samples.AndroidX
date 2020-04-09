using Foundation;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CoreGraphics;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.Views
{
    [Register(nameof(TextViewWithPlaceholder))]
    public class TextViewWithPlaceholder : UITextView, IUITextViewDelegate
    {
        private readonly int defaultFontSize = 15;
        private readonly UIColor defaultPlaceholderColor = Colors.Common.PlaceholderText.ToNativeColor();
        private readonly UIColor defaultTextColor = Colors.Common.TextColor.ToNativeColor();
        private readonly ISubject<String> textSubject = new Subject<string>();
        private readonly BehaviorSubject<CGSize> sizeSubject = new BehaviorSubject<CGSize>(CGSize.Empty);

        private UILabel placeholderLabel = new UILabel();

        public IObservable<string> TextObservable { get; }
        public IObservable<Unit> SizeChangedObservable { get; }

        public string PlaceholderText
        {
            get => placeholderLabel.Text;
            set
            {
                placeholderLabel.Text = value;
                updatePlaceholderVisibility();
            }
        }

        public override string Text
        {
            get => base.Text;
            set
            {
                // Overriding the Text property will place the cursor at the end
                // of the new text. We don't want to control the position of the
                // cursor ourselves, so we'll simply remember the cursor position
                // before we overwrite the text and then we restore it.
                var originalSelectedRange = SelectedRange;
                base.Text = value;
                SelectedRange = originalSelectedRange;

                updatePlaceholderVisibility();
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Delegate = this;

            Font = Font.WithSize(defaultFontSize);
            TextColor = defaultTextColor;

            placeholderLabel.Font = Font.WithSize(defaultFontSize);
            placeholderLabel.TextColor = defaultPlaceholderColor;
            AddSubview(placeholderLabel);

            updatePlaceholderVisibility();
        }

        public TextViewWithPlaceholder(IntPtr handle) : base(handle)
        {
            TextObservable = textSubject.AsObservable();
            SizeChangedObservable = sizeSubject.SelectUnit().AsObservable();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            placeholderLabel.Frame = Bounds;
            if (sizeSubject.Value != Frame.Size)
            {
                sizeSubject.OnNext(Frame.Size);
            }
        }

        private void updatePlaceholderVisibility()
        {
            if (string.IsNullOrEmpty(Text))
            {
                placeholderLabel.Hidden = IsFirstResponder;
                return;
            }

            placeholderLabel.Hidden = true;
        }

        [Export("textView:shouldChangeTextInRange:replacementText:")]
        public virtual new bool ShouldChangeText(UITextView textView, NSRange range, string text)
        {
            if (text == Environment.NewLine)
            {
                ResignFirstResponder();
                return false;
            }

            return true;
        }

        [Export("textViewDidChange:")]
        public virtual new void Changed(UITextView textView)
        {
            // When the `MarkedTextRange` property of the UITextView is not null
            // then it means that the user is in the middle of inputting a multistage character.
            // Hold off on editing the attributedText until they are done.
            // Source: https://stackoverflow.com/questions/31430308/uitextview-attributedtext-with-japanese-keyboard-repeats-input
            if (textView.MarkedTextRange != null) return;

            Text = Text.Replace(Environment.NewLine, " ");
            textSubject.OnNext(Text);
        }

        [Export("textViewDidBeginEditing:")]
        public void EditingStarted(UITextView textView)
        {
            if (string.IsNullOrEmpty(Text))
            {
                // this will force the text view to change the color of the text
                // so if the person starts typing a multistage character, the color
                // of the text won't be the color of the placeholder anymore
                updatePlaceholderVisibility();
            }
            updatePlaceholderVisibility();
            textSubject.OnNext(Text);
        }

        [Export("textViewDidEndEditing:")]
        public void EditingEnded(UITextView view)
        {
            updatePlaceholderVisibility();
            textSubject.OnNext(Text);
        }
    }
}
