using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Systems.PilgrimTrials;
using Xunit;

namespace Satori.Tests.Systems.PilgrimTrials;

public sealed class SegmentTransitionSystemTests
{
	[Fact]
	public void TryAdvance_AcrossFourSegments_ReachesLastIndex()
	{
		SegmentTransitionSystem segmentTransitionSystem = new SegmentTransitionSystem();
		TrialRunState trialRunState = new TrialRunState();
		for (int i = 0; i < 4; i++)
		{
			SegmentTransitionResult actual = segmentTransitionSystem.TryAdvance(trialRunState);
			Assert.Equal(SegmentTransitionResult.Advanced, actual);
			Assert.Equal(i + 1, trialRunState.CurrentSegmentIndex);
		}
	}

	[Fact]
	public void TryAdvance_OnLastSegment_ReturnsPilgrimageComplete()
	{
		SegmentTransitionSystem segmentTransitionSystem = new SegmentTransitionSystem();
		TrialRunState trialRunState = new TrialRunState
		{
			CurrentSegmentIndex = 4
		};
		SegmentTransitionResult actual = segmentTransitionSystem.TryAdvance(trialRunState);
		Assert.Equal(SegmentTransitionResult.PilgrimageComplete, actual);
		Assert.Equal(4, trialRunState.CurrentSegmentIndex);
	}
}
