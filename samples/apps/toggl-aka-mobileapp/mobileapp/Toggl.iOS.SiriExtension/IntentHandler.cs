using Foundation;
using Intents;
using Toggl.iOS.Intents;
using System;
using Toggl.iOS.Shared;

namespace SiriExtension
{
    [Register("IntentHandler")]
    public class IntentHandler : INExtension
    {
        protected IntentHandler(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override NSObject GetHandler(INIntent intent)
        {
            switch (intent)
            {
                case StopTimerIntent _:
                    return new StopTimerIntentHandler(APIHelper.GetTogglAPI());
                case StartTimerIntent _:
                    return new StartTimerIntentHandler(APIHelper.GetTogglAPI());
                case StartTimerFromClipboardIntent _:
                    return new StartTimerFromClipboardIntentHandler(APIHelper.GetTogglAPI());
                case ContinueTimerIntent _:
                    return new ContinueTimerIntentHandler(APIHelper.GetTogglAPI());
                default:
                    throw new Exception("Unhandled intent type: ${intent}");
            }
        }
    }
}
