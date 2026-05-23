using System;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Progression;

namespace Satori.Core.Systems.Progression;

public static class KarmaValues
{
	public const int PreceptViolationPenalty = 3;

	public const int RightSpeechSuccessReward = 2;

	public const int RightSpeechFailPenalty = 2;

	public const int WheelSuccessReward = 3;

	public const int WheelFailPenalty = 1;

	public const int LotusCollectReward = 1;
}

public sealed class KarmaSystem
{
	public int GetLotusKarma() => KarmaValues.LotusCollectReward;

	public void AddRunKarma(TrialRunState run, int amount)
	{
		if (amount > 0 && run.IsActive)
		{
			run.RunKarma += amount;
		}
	}

	public void ApplyMetaPenalty(PlayerMetaState meta, int amount)
	{
		if (amount > 0)
		{
			meta.Karma = Math.Max(0, meta.Karma - amount);
		}
	}

	public void ApplyRunPenalty(TrialRunState run, int amount)
	{
		if (amount > 0 && run.IsActive)
		{
			run.RunKarma = Math.Max(0, run.RunKarma - amount);
		}
	}
}
