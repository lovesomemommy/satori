using Satori.Core.Models.PilgrimTrials;

namespace Satori.Core.Systems.PilgrimTrials;

public sealed class SegmentTransitionSystem
{
	public SegmentTransitionResult TryAdvance(TrialRunState run)
	{
		if (!run.IsActive)
		{
			return SegmentTransitionResult.None;
		}
		if (run.CurrentSegmentIndex < 4)
		{
			run.CurrentSegmentIndex++;
			return SegmentTransitionResult.Advanced;
		}
		return SegmentTransitionResult.PilgrimageComplete;
	}

	public bool IsPilgrimageComplete(TrialRunState run)
	{
		return run.CurrentSegmentIndex >= 4 && run.Outcome == TrialOutcome.Success;
	}
}
