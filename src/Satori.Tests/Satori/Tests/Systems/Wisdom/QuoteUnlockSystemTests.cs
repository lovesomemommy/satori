using System.Collections.Generic;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Wisdom;
using Satori.Core.Services.Wisdom;
using Satori.Core.Systems.Wisdom;
using Satori.Core.Utilities;
using Xunit;

namespace Satori.Tests.Systems.Wisdom;

public sealed class QuoteUnlockSystemTests
{
	[Fact]
	public void UnlockForRun_AddsQuoteToRun()
	{
		QuoteCatalog catalog = new QuoteCatalog(new Dictionary<string, string> { ["quote.lotus.01"] = "Тестовая цитата" });
		GameEventBus eventBus = new GameEventBus();
		QuoteUnlockSystem quoteUnlockSystem = new QuoteUnlockSystem(catalog, eventBus);
		TrialRunState trialRunState = new TrialRunState();
		LotusModel lotus = new LotusModel
		{
			Id = 1,
			SegmentIndex = 0
		};
		QuoteModel quoteModel = quoteUnlockSystem.UnlockForRun(trialRunState, lotus);
		Assert.NotNull(quoteModel);
		Assert.Single(trialRunState.UnlockedQuotes);
		Assert.Equal("quote.lotus.01", quoteModel.QuoteId);
	}

	[Fact]
	public void UnlockForRun_UsesSegmentQuoteId()
	{
		QuoteCatalog catalog = new QuoteCatalog(new Dictionary<string, string>
		{
			["quote.lotus.01"] = "Тестовая цитата",
			["quote.lotus.05"] = "Пятая тропа"
		});
		var quoteUnlockSystem = new QuoteUnlockSystem(catalog, new GameEventBus());
		var trialRunState = new TrialRunState();
		var lotus = new LotusModel
		{
			Id = 13,
			SegmentIndex = 4,
			HasQuote = true
		};

		var quoteModel = quoteUnlockSystem.UnlockForRun(trialRunState, lotus);

		Assert.NotNull(quoteModel);
		Assert.Equal("quote.lotus.05", quoteModel!.QuoteId);
	}

	[Fact]
	public void CommitRunQuotes_MergesIntoWisdomLibrary()
	{
		TrialRunState trialRunState = new TrialRunState();
		trialRunState.UnlockedQuotes.Add(new QuoteModel
		{
			QuoteId = "quote.lotus.01"
		});
		WisdomLibraryState wisdomLibraryState = new WisdomLibraryState();
		WisdomCommitSystem.CommitRunQuotes(trialRunState, wisdomLibraryState);
		Assert.Single(wisdomLibraryState.Quotes);
		Assert.Equal("quote.lotus.01", wisdomLibraryState.Quotes[0].QuoteId);
	}
}
