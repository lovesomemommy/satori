using Satori.Core.Models.Precepts;
using Satori.Core.Models.Progression;
using Satori.Core.Models.Wisdom;
using Satori.Core.Services.Wisdom;
using Satori.Core.Systems.Minigames;
using Satori.Core.Systems.Progression;
using Xunit;

namespace Satori.Tests.Systems.Minigames;

public sealed class WheelLotusRewardSystemTests
{
	[Fact]
	public void TryGrantFirstClearReward_FirstSuccess_GrantsLotusAndQuote()
	{
		var meta = new PlayerMetaState();
		var progress = new PreceptProgressModel();
		var wisdom = new WisdomLibraryState();
		var lotusCatalog = LotusCatalog.CreateDefault();
		var quoteCatalog = QuoteCatalog.CreateDefault();
		var garden = new GardenPlantingSystem();

		var outcome = WheelLotusRewardSystem.TryGrantFirstClearReward(
			1,
			meta,
			progress,
			wisdom,
			lotusCatalog,
			quoteCatalog,
			garden,
			DateTimeOffset.UtcNow);

		Assert.True(outcome.Granted);
		Assert.Equal(16, outcome.LotusId);
		Assert.Equal("quote.lotus.16", outcome.QuoteId);
		Assert.Contains(16, meta.UnlockedLotusIds);
		Assert.Single(wisdom.Quotes);
		Assert.Single(meta.PlantedLotuses);
		Assert.Contains(1, progress.WheelLotusRewardsClaimed);
	}

	[Fact]
	public void TryGrantFirstClearReward_RepeatSuccess_DoesNotGrantAgain()
	{
		var meta = new PlayerMetaState();
		var progress = new PreceptProgressModel { WheelLotusRewardsClaimed = { 1 } };
		var wisdom = new WisdomLibraryState();
		var lotusCatalog = LotusCatalog.CreateDefault();
		var quoteCatalog = QuoteCatalog.CreateDefault();
		var garden = new GardenPlantingSystem();

		var outcome = WheelLotusRewardSystem.TryGrantFirstClearReward(
			1,
			meta,
			progress,
			wisdom,
			lotusCatalog,
			quoteCatalog,
			garden,
			DateTimeOffset.UtcNow);

		Assert.False(outcome.Granted);
		Assert.Empty(meta.UnlockedLotusIds);
		Assert.Empty(wisdom.Quotes);
		Assert.Empty(meta.PlantedLotuses);
	}
}
