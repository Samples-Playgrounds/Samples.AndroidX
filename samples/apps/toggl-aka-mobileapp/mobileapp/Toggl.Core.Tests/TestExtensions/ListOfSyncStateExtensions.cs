﻿using FluentAssertions;
using System;
using System.Collections.Generic;
using Toggl.Core.Sync;
using Toggl.Shared;

namespace Toggl.Core.Tests.TestExtensions
{
    internal static class ListOfSyncStateExtensions
    {
        internal static void ShouldBeSameEventsAs(this List<SyncState> actualEvents,
            params SyncState[] expectedEvents)
        {
            Ensure.Argument.IsNotNull(expectedEvents, nameof(expectedEvents));

            actualEvents.Should().HaveCount(expectedEvents.Length);

            for (var i = 0; i < expectedEvents.Length; i++)
            {
                var actual = actualEvents[i];
                var expected = expectedEvents[i];

                try
                {
                    actual.Should().Be(expected);
                }
                catch (Exception e)
                {
                    throw new Exception($"Found unexpected event at index {i}.", e);
                }
            }
        }
    }
}
