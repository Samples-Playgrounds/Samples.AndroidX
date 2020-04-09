﻿using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views
{
    [Register(nameof(LoginTextField))]
    public class LoginTextField : UITextField
    {
        [Obsolete("Prefer using the FirstResponder observable instead")]
        public event EventHandler IsFirstResponderChanged;

        public ISubject<bool> firstResponderSubject = new Subject<bool>();

        public IObservable<bool> FirstResponder { get; private set; }

        private const int textSize = 15;
        private const int textTopOffset = 22;
        private const int underlineHeight = 1;
        private const int bigPlaceholderSize = 15;
        private const int smallPlaceholderSize = 12;
        private const float placeholderAnimationDuration = 0.5f;

        private readonly CGColor placeholderColor
            = Colors.Login.TextViewPlaceholder.ToNativeColor().CGColor;
        private readonly CALayer underlineLayer = new CALayer();
        private readonly CATextLayer placeholderLayer = new CATextLayer();

        private bool placeholderDrawn;
        private bool placeholderIsUp;

        public LoginTextField(CGRect frame) : base(frame) { }

        public LoginTextField(IntPtr handle) : base(handle) { }

        public override string Text
        {
            get => base.Text;
            set
            {
                if (string.IsNullOrEmpty(base.Text))
                    movePlaceholderUp();
                if (string.IsNullOrEmpty(value) && !IsFirstResponder)
                    movePlaceholderDown();
                base.Text = value;
            }
        }

        public override string Placeholder
        {
            get => base.Placeholder;
            set
            {
                base.Placeholder = value;
                placeholderDrawn = false;
                DrawPlaceholder(new CGRect());
            }
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            FirstResponder = firstResponderSubject
                .AsObservable()
                .DistinctUntilChanged()
                .StartWith(false);

            Layer.AddSublayer(underlineLayer);
            Layer.AddSublayer(placeholderLayer);
            BorderStyle = UITextBorderStyle.None;
            Font = UIFont.SystemFontOfSize(textSize);
            underlineLayer.BackgroundColor = placeholderColor;
            VerticalAlignment = UIControlContentVerticalAlignment.Top;
            DrawPlaceholder(Frame);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            updateBottomLine();
        }

        public override CGRect EditingRect(CGRect forBounds)
            => new CGRect(0, textTopOffset, Frame.Width, Frame.Height - textTopOffset);

        public override CGRect TextRect(CGRect forBounds)
            => EditingRect(forBounds);

        public override void DrawPlaceholder(CGRect rect)
        {
            if (placeholderDrawn) return;

            placeholderLayer.String = Placeholder;
            placeholderLayer.ForegroundColor = placeholderColor;
            placeholderLayer.FontSize = bigPlaceholderSize;
            var frameY = (Frame.Height - bigPlaceholderSize) / 2;
            placeholderLayer.Frame = new CGRect(
                0,
                frameY,
                Frame.Width,
                Frame.Height - frameY
            );

            //For antialiasing
            placeholderLayer.ContentsScale = UIScreen.MainScreen.Scale;
            placeholderDrawn = true;
        }

        public override bool BecomeFirstResponder()
        {
            var becomeFirstResponder = base.BecomeFirstResponder();
            if (becomeFirstResponder)
            {
                firstResponderSubject.OnNext(true);
                IsFirstResponderChanged?.Invoke(this, new EventArgs());

                if (placeholderLayer.Frame.Top != 0)
                    movePlaceholderUp();
            }

            return becomeFirstResponder;
        }

        public override bool ResignFirstResponder()
        {
            var resignFirstResponder = base.ResignFirstResponder();
            if (resignFirstResponder)
            {
                firstResponderSubject.OnNext(false);
                IsFirstResponderChanged?.Invoke(this, new EventArgs());

                if (string.IsNullOrEmpty(Text))
                    movePlaceholderDown();
            }
            return resignFirstResponder;
        }

        private void updateBottomLine()
        {
            underlineLayer.Frame = new CGRect(
                0,
                Frame.Height - underlineHeight,
                Frame.Width,
                underlineHeight
            );
        }

        private void movePlaceholderUp()
        {
            if (placeholderIsUp) return;
            placeholderIsUp = true;

            var yOffset = -placeholderLayer.Frame.Top;
            CATransaction.Begin();
            CATransaction.AnimationDuration = placeholderAnimationDuration;
            placeholderLayer.AffineTransform = CGAffineTransform.MakeTranslation(0, yOffset);
            placeholderLayer.FontSize = smallPlaceholderSize;
            CATransaction.Commit();
        }

        private void movePlaceholderDown()
        {
            if (!placeholderIsUp) return;
            placeholderIsUp = false;

            CATransaction.Begin();
            CATransaction.AnimationDuration = placeholderAnimationDuration;
            placeholderLayer.AffineTransform = CGAffineTransform.MakeIdentity();
            placeholderLayer.FontSize = bigPlaceholderSize;
            CATransaction.Commit();
        }
    }
}
