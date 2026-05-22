using System.Linq;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Progression;
using Satori.Core.Models.Wisdom;

namespace Satori.Core.Systems.Wisdom;

public static class WisdomCommitSystem
{
	public static void CommitRunQuotes(TrialRunState run, WisdomLibraryState wisdom)
	{
		foreach (QuoteModel quote in run.UnlockedQuotes)
		{
			if (wisdom.Quotes.All((QuoteModel existing) => existing.QuoteId != quote.QuoteId))
			{
				wisdom.Quotes.Add(quote);
			}
		}
	}

	public static void CommitRunLotuses(TrialRunState run, PlayerMetaState meta, PilgrimPilgrimageDefinition? definition)
	{
		_ = definition;
		foreach (int collectedLotusId in run.CollectedLotusIds)
		{
			meta.UnlockedLotusIds.Add(collectedLotusId);
		}
	}
}
