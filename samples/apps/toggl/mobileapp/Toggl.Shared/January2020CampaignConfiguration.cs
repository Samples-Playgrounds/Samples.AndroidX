using System;

namespace Toggl.Shared
{
    public struct January2020CampaignConfiguration
    {
        public enum AvailableOption
        {
            None,
            A,
            B
        }

        public AvailableOption Option { get; }

        public January2020CampaignConfiguration(string option)
        {
            Option = Enum.TryParse<AvailableOption>(option, true, out var result)
                ? result
                : AvailableOption.None;
        }
    }
}
