using System;
using System.Linq;
using Satori.Core.Interfaces.Events;
using Satori.Core.Interfaces.Events.Events;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Wisdom;
using Satori.Core.Services.Wisdom;

namespace Satori.Core.Systems.Wisdom;

public sealed class QuoteUnlockSystem
{
	private readonly QuoteCatalog _catalog;

	private readonly IGameEventBus _eventBus;

	public QuoteUnlockSystem(QuoteCatalog catalog, IGameEventBus eventBus)
	{
		_catalog = catalog;
		_eventBus = eventBus;
	}

	public QuoteModel? UnlockForRun(TrialRunState run, LotusModel lotus)
	{
		string quoteId = _catalog.GetQuoteIdForLotus(lotus.Id);
		if (run.UnlockedQuotes.Any((QuoteModel q) => q.QuoteId == quoteId))
		{
			return null;
		}
		if (!_catalog.TryGetText(quoteId, out string text))
		{
			text = quoteId;
		}
		QuoteModel quoteModel = new QuoteModel
		{
			QuoteId = quoteId,
			Rarity = _catalog.RarityForLotus(lotus.Type),
			FoundAt = DateTimeOffset.UtcNow,
			SourceSegmentIndex = lotus.SegmentIndex,
			SourceLotusType = lotus.Type
		};
		run.UnlockedQuotes.Add(quoteModel);
		_eventBus.Publish(new QuoteUnlockedEvent(quoteId));
		return quoteModel;
	}
}
