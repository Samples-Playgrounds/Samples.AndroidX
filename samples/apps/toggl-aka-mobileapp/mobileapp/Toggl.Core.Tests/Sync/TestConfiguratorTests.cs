using FluentAssertions;
using System;
using Toggl.Core.Sync;
using Xunit;

namespace Toggl.Core.Tests.Sync
{
    public class TestConfiguratorTests
    {
        public class GetAllLooseEndStateResults
        {
            private TestConfigurator configurator = new TestConfigurator();

            [Fact, LogIfTooSlow]
            public void ReturnsAnEmptyListByDefault()
            {
                var looseEnds = configurator.GetAllLooseEndStateResults();

                looseEnds.Should().BeEmpty();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsAnEmptyListForSimpleLoop()
            {
                var state = new SingleResultState();
                configurator.ConfigureTransition(state.Result, state);

                var looseEnds = configurator.GetAllLooseEndStateResults();

                looseEnds.Should().BeEmpty();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsAnEmptyListForLongerLoop()
            {
                var state1 = new SingleResultState();
                var state2 = new SingleResultState();
                var state3 = new SingleResultState();
                configurator.ConfigureTransition(state1.Result, state2);
                configurator.ConfigureTransition(state2.Result, state3);
                configurator.ConfigureTransition(state3.Result, state1);

                var looseEnds = configurator.GetAllLooseEndStateResults();

                looseEnds.Should().BeEmpty();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsAnEmptyListForSimpleTree()
            {
                var start = new SingleResultState();
                var deadEnd = new NoResultState();
                configurator.ConfigureTransition(start.Result, deadEnd);

                var looseEnds = configurator.GetAllLooseEndStateResults();

                looseEnds.Should().BeEmpty();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsAnEmptyListForBranchingTree()
            {
                var start = new SingleResultState();
                var branch = new TwoResultState();
                var branchContinuation = new SingleResultState();
                var deadEnd1 = new NoResultState();
                var deadEnd2 = new NoResultState();
                configurator.ConfigureTransition(start.Result, branch);
                configurator.ConfigureTransition(branch.Result1, branchContinuation);
                configurator.ConfigureTransition(branchContinuation.Result, deadEnd1);
                configurator.ConfigureTransition(branch.Result2, deadEnd2);

                var looseEnds = configurator.GetAllLooseEndStateResults();

                looseEnds.Should().BeEmpty();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsAnEmptyListForBranchingLoop()
            {
                var start = new SingleResultState();
                var branch = new TwoResultState();
                var branchContinuation = new SingleResultState();
                configurator.ConfigureTransition(start.Result, branch);
                configurator.ConfigureTransition(branch.Result1, branchContinuation);
                configurator.ConfigureTransition(branchContinuation.Result, start);
                configurator.ConfigureTransition(branch.Result2, start);

                var looseEnds = configurator.GetAllLooseEndStateResults();

                looseEnds.Should().BeEmpty();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsOneForSingleLooseEnd()
            {
                var state = new SingleResultState();
                configurator.AllDistinctStatesInOrder.Add(state);

                var looseEnds = configurator.GetAllLooseEndStateResults();

                looseEnds.Should().HaveCount(1);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsOneForLinearLooseEnd()
            {
                var state1 = new SingleResultState();
                var state2 = new SingleResultState();
                var state3 = new SingleResultState();
                configurator.ConfigureTransition(state1.Result, state2);
                configurator.ConfigureTransition(state2.Result, state3);

                var looseEnds = configurator.GetAllLooseEndStateResults();

                looseEnds.Should().HaveCount(1);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsOneForPartiallyLoopingBranch()
            {
                var state1 = new SingleResultState();
                var state2 = new SingleResultState();
                var branch = new TwoResultState();
                configurator.ConfigureTransition(state1.Result, state2);
                configurator.ConfigureTransition(state2.Result, branch);
                configurator.ConfigureTransition(branch.Result1, state1);

                var looseEnds = configurator.GetAllLooseEndStateResults();

                looseEnds.Should().HaveCount(1);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTwoForLooseEndBranch()
            {
                var state1 = new SingleResultState();
                var state2 = new SingleResultState();
                var branch = new TwoResultState();
                configurator.ConfigureTransition(state1.Result, state2);
                configurator.ConfigureTransition(state2.Result, branch);

                var looseEnds = configurator.GetAllLooseEndStateResults();

                looseEnds.Should().HaveCount(2);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTwoForDisjointLinearLooseEnds()
            {
                var start1 = new SingleResultState();
                var loose1 = new SingleResultState();
                var start2 = new SingleResultState();
                var loose2 = new SingleResultState();
                configurator.ConfigureTransition(start1.Result, loose1);
                configurator.ConfigureTransition(start2.Result, loose2);

                var looseEnds = configurator.GetAllLooseEndStateResults();

                looseEnds.Should().HaveCount(2);
            }


            private class NoResultState : ISyncState
            {
                public IObservable<ITransition> Start() => throw new NotImplementedException();
            }

            private class SingleResultState : ISyncState
            {
                public IStateResult Result { get; } = new StateResult();

                public IObservable<ITransition> Start() => throw new NotImplementedException();
            }

            private class TwoResultState : ISyncState
            {
                public IStateResult Result1 { get; } = new StateResult();
                public IStateResult Result2 { get; } = new StateResult();

                public IObservable<ITransition> Start() => throw new NotImplementedException();
            }
        }
    }
}
