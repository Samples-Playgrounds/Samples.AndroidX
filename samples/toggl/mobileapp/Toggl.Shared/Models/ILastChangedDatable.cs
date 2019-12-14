using System;

namespace Toggl.Shared.Models
{
    public interface ILastChangedDatable
    {
        DateTimeOffset At { get; }
    }
}
