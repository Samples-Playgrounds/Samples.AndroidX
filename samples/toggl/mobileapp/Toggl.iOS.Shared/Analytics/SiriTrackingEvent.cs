using Foundation;
using System.Collections.Generic;
using System.Linq;
using Toggl.Shared.Models;

namespace Toggl.iOS.Shared.Analytics
{
    public enum SiriTrackingEventType
    {
        StartTimer = 1,
        StopTimer = 2,
        Error = 3
    }

    public struct SiriErrorEventKeys
    {
        public static string Message = "Message";
    }

    public struct SiriStartTimerEventKeys
    {
        public static string HasEmptyDescription = "HasEmptyDescription";
        public static string HasProject = "HasProject";
        public static string HasTask = "HasTask";
        public static string NumberOfTags = "NumberOfTags";
        public static string IsBillable = "IsBillable";
    }

    [Register("SiriTrackingEvent")]
    public class SiriTrackingEvent : NSObject, INSCoding
    {
        private static readonly string eventTypeEncodeKey = nameof(eventTypeEncodeKey);
        private static readonly string parametersEncodeKey = nameof(parametersEncodeKey);

        public readonly SiriTrackingEventType EventType;
        public readonly Dictionary<string, string> Parameters;

        public SiriTrackingEvent(SiriTrackingEventType eventType, Dictionary<string, string> parameters)
        {
            EventType = eventType;
            Parameters = parameters;
        }

        public static SiriTrackingEvent Error(string message)
        {
            return new SiriTrackingEvent(SiriTrackingEventType.Error, new Dictionary<string, string>
            {
                [SiriErrorEventKeys.Message] = message
            });
        }

        public static SiriTrackingEvent StartTimer(ITimeEntry te)
        {
            return new SiriTrackingEvent(SiriTrackingEventType.StartTimer, new Dictionary<string, string>
            {
                [SiriStartTimerEventKeys.HasEmptyDescription] = string.IsNullOrEmpty(te.Description).ToString(),
                [SiriStartTimerEventKeys.HasProject] = (te.ProjectId != null).ToString(),
                [SiriStartTimerEventKeys.HasTask] = (te.TaskId != null).ToString(),
                [SiriStartTimerEventKeys.NumberOfTags] = te.TagIds.Count().ToString(),
                [SiriStartTimerEventKeys.IsBillable] = te.Billable.ToString()
            });
        }

        public static SiriTrackingEvent StopTimer()
        {
            return new SiriTrackingEvent(SiriTrackingEventType.StopTimer, null);
        }

        #region INSCoding
        [Export("initWithCoder:")]
        public SiriTrackingEvent(NSCoder coder)
        {
            EventType = (SiriTrackingEventType)coder.DecodeInt(eventTypeEncodeKey);

            var nativeDict = (NSDictionary)coder.DecodeObject(parametersEncodeKey);

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
