using System;
namespace Toggl.iOS.Views.EditDuration
{
    public sealed class NumberKeyPressedEventArgs : EventArgs
    {
        public int Number { get; }

        public NumberKeyPressedEventArgs(int number)
        {
            Number = number;
        }
    }
}
