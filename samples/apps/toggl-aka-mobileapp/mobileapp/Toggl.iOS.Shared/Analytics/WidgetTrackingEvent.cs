using Foundation;
using System.Collections.Generic;
using System.Linq;
using Toggl.Shared.Models;

namespace Toggl.iOS.Shared.Analytics
{
    public enum WidgetTrackingEventType
    {
        StartTimer = 1,
        StopTimer = 2,
        Error = 3
    }

    public struct WidgetErrorEventKeys
    {
        public static string Message = "Message";
    }

    [Register("WidgetTrackingEvent")]
    public class WidgetTrackingEvent : NSObject, INSCoding
    {
        private static readonly string eventTypeEncodeKey = nameof(eventTypeEncodeKey);
        private static readonly string parametersEncodeKey = nameof(parametersEncodeKey);

        public readonly WidgetTrackingEventType EventType;
        public readonly Dictionary<string, string> Parameters;

        private WidgetTrackingEvent(WidgetTrackingEventType eventType, Dictionary<string, string> parameters)
        {
            EventType = eventType;
            Parameters = parameters;
        }

        public static WidgetTrackingEvent Error(string message)
            => new WidgetTrackingEvent(WidgetTrackingEventType.Error, new Dictionary<string, string>
            {
                [WidgetErrorEventKeys.Message] = message
            });

        public static WidgetTrackingEvent StartTimer()
            => new WidgetTrackingEvent(WidgetTrackingEventType.StartTimer, null);

        public static WidgetTrackingEvent StopTimer()
            => new WidgetTrackingEvent(WidgetTrackingEventType.StopTimer, null);

        #region INSCoding
        [Export("initWithCoder:")]
        public WidgetTrackingEvent(NSCoder coder)
        {
            EventType = (WidgetTrackingEventType) coder.DecodeInt(eventTypeEncodeKey);

            var nativeDict = (NSDictionary) coder.DecodeObject(parametersEncodeKey);

            if (nativeDict != null)
            {
                var dict = new Dictionary<string, string>();
                foreach (var item in nativeDict)
                {
                    dict.Add((NSString)item.Key, (NSString)item.Value);
                }
                Parameters = dict;
            }
        }

        [Export("encodeWithCoder:")]
        public void EncodeTo(NSCoder encoder)
        {
            encoder.Encode((int)EventType, eventTypeEncodeKey);

            if (Parameters != null)
            {
                var nativeDict = NSDictionary
                    .FromObjectsAndKeys(Parameters.Values.ToArray(), Parameters.Keys.ToArray());
                encoder.Encode(nativeDict, parametersEncodeKey);
            }
        }
        #endregion
    }
}
