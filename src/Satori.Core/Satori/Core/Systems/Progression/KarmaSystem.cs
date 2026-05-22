using System;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Progression;

namespace Satori.Core.Systems.Progression;

public sealed class KarmaSystem
{
	public int GetLotusKarma(LotusType type)
	{
		return KarmaValues.ForLotus(type);
	}

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
