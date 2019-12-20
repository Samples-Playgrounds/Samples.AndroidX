using System;
using System.Linq;
using Toggl.Shared;

namespace Toggl.Core.Analytics
{
    internal abstract class BaseAnalyticsEvent
    {
        private readonly IAnalyticsService analyticsService;

        private readonly string name;

        private readonly string[] paramNames;

        protected BaseAnalyticsEvent(IAnalyticsService analyticsService, string name, string[] paramNames)
        {
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNullOrWhiteSpaceString(name, nameof(name));
            Ensure.Argument.IsNotNull(paramNames, nameof(paramNames));

            this.analyticsService = analyticsService;
            this.name = name;
            this.paramNames = paramNames;
        }

        protected void Track(params object[] parameters)
        {
            if (parameters.Length != paramNames.Length)
                throw new ArgumentException($"Cannot track {parameters.Length} parameters for the event {name} - the expected number of parameters is {paramNames.Length}");

            var pametersDictionary = parameters
                    .Select(parameter => parameter.ToString())
                    .Zip(paramNames, (parameter, parameterName) => (key: parameterName, value: parameter))
                    .ToDictionary(tuple => tuple.key, tuple => tuple.value);

            analyticsService.Track(name, pametersDictionary);
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed class AnalyticsEvent : BaseAnalyticsEvent, IAnalyticsEvent
    {
        public AnalyticsEvent(IAnalyticsService analyticsService, string name)
            : base(analyticsService, name, new string[0])
        {
        }

        public void Track()
        {
            base.Track();
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed class AnalyticsEvent<T> : BaseAnalyticsEvent, IAnalyticsEvent<T>
    {
        public AnalyticsEvent(IAnalyticsService analyticsService, string name, string parameter)
            : base(analyticsService, name, new[] { parameter })
        {
        }

        public void Track(T param)
        {
            base.Track(param);
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed class AnalyticsEvent<T1, T2> : BaseAnalyticsEvent, IAnalyticsEvent<T1, T2>
    {
        public AnalyticsEvent(IAnalyticsService analyticsService, string name, string parameter1, string parameter2)
            : base(analyticsService, name, new[] { parameter1, parameter2 })
        {
        }

        public void Track(T1 param1, T2 param2)
        {
            base.Track(param1, param2);
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed class AnalyticsEvent<T1, T2, T3> : BaseAnalyticsEvent, IAnalyticsEvent<T1, T2, T3>
    {
        public AnalyticsEvent(IAnalyticsService analyticsService, string name, string parameter1, string parameter2, string parameter3)
            : base(analyticsService, name, new[] { parameter1, parameter2, parameter3 })
        {
        }

        public void Track(T1 param1, T2 param2, T3 param3)
        {
            base.Track(param1, param2, param3);
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed class AnalyticsEvent<T1, T2, T3, T4> : BaseAnalyticsEvent, IAnalyticsEvent<T1, T2, T3, T4>
    {
        public AnalyticsEvent(IAnalyticsService analyticsService, string name, string parameter1, string parameter2, string parameter3, string parameter4)
            : base(analyticsService, name, new[] { parameter1, parameter2, parameter3, parameter4 })
        {
        }

        public void Track(T1 param1, T2 param2, T3 param3, T4 param4)
        {
            base.Track(param1, param2, param3, param4);
        }
    }
}
