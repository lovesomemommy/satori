using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Progression;

namespace Satori.Core.Systems.PilgrimTrials;

public sealed class TrialRunSystem
{
	public void StartRun(TrialRunState run)
	{
		run.Outcome = TrialOutcome.InProgress;
		run.DefeatReason = TrialDefeatReason.None;
		run.CurrentSegmentIndex = 0;
		run.RunKarma = 0;
		run.CollectedLotusIds.Clear();
		run.UnlockedQuotes.Clear();
		run.LastDefeatMessageKey = null;
		run.RemainingTime = TrialTimerSystem.PilgrimageDuration;
	}

	public void WipeRun(TrialRunState run, TrialDefeatReason reason)
	{
		run.Outcome = TrialOutcome.Defeat;
		run.DefeatReason = reason;
		run.RunKarma = 0;
		run.CollectedLotusIds.Clear();
		run.UnlockedQuotes.Clear();
	}

	public void CommitRunToMeta(TrialRunState run, PlayerMetaState meta)
	{
		if (run.Outcome == TrialOutcome.InProgress)
		{
			meta.Karma += run.RunKarma;
			run.Outcome = TrialOutcome.Success;
			run.DefeatReason = TrialDefeatReason.None;
			if (run.CurrentSegmentIndex >= 4)
			{
				meta.PilgrimageCompleted = true;
			}
		}
	}

	public void HandleTimerDefeat(TrialRunState run)
	{
		if (run.IsActive)
		{
			WipeRun(run, TrialDefeatReason.TimerExpired);
		}
	}
}
