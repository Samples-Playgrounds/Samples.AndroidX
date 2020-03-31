using Android.Text;
using Android.Widget;
using Java.Lang;
using Toggl.Droid.Extensions;
using Object = Java.Lang.Object;

namespace Toggl.Droid.Views.EditDuration
{
    public partial class WheelDurationInput : EditText, ITextWatcher
    {
        private class InputFilter : Object, IInputFilter
        {
            public delegate void ActionEntered(int digit);
            public delegate void DeletionDetected();

            private readonly ActionEntered onDigitEntered;
            private readonly DeletionDetected onDeletionDetected;

            public InputFilter(ActionEntered onDigitEntered, DeletionDetected onDeletionDetected)
            {
                this.onDigitEntered = onDigitEntered;
                this.onDeletionDetected = onDeletionDetected;
            }

            public ICharSequence FilterFormatted(ICharSequence source, int start, int end, ISpanned dest, int dstart, int dend)
            {
                var empty = string.Empty.AsJavaString();
                var sourceLength = source.Length();

                if (sourceLength > 1)
                    return source.ToString().AsJavaString();

                if (sourceLength == 0)
                {
                    onDeletionDetected();
                    return "0".AsCharSequence();
                }

                var lastChar = source.CharAt(sourceLength - 1);

                if (char.IsDigit(lastChar))
                {
                    int digit = int.Parse(lastChar.ToString());
                    onDigitEntered(digit);

                    return empty;
                }

                return empty;
            }
        }
    }
}
