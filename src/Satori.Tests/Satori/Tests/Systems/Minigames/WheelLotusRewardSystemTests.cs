using Satori.Core.Models.Precepts;
using Satori.Core.Models.Progression;
using Satori.Core.Models.Wisdom;
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
		var garden = new GardenPlantingSystem();

		var outcome = WheelLotusRewardSystem.TryGrantFirstClearReward(
			1,
			meta,
			progress,
			wisdom,
			garden,
			DateTimeOffset.UtcNow);

		Assert.True(outcome.Granted);
		Assert.Equal(16, outcome.LotusId);
		Assert.Equal("quote.lotus.06", outcome.QuoteId);
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
		var garden = new GardenPlantingSystem();

		var outcome = WheelLotusRewardSystem.TryGrantFirstClearReward(
			1,
			meta,
			progress,
			wisdom,
			garden,
			DateTimeOffset.UtcNow);

		Assert.False(outcome.Granted);
		Assert.Empty(meta.UnlockedLotusIds);
		Assert.Empty(wisdom.Quotes);
		Assert.Empty(meta.PlantedLotuses);
	}

	[Fact]
	public void TryGrantFirstClearReward_PlantsWheelLotusEvenWhenPilgrimageLotusExists()
	{
		var meta = new PlayerMetaState();
		meta.PlantedLotuses.Add(new GardenSlotModel
		{
			SlotIndex = 0,
			LotusId = 6,
			PlantedAt = DateTimeOffset.UtcNow
		});
		var progress = new PreceptProgressModel();
		var wisdom = new WisdomLibraryState();
		var garden = new GardenPlantingSystem();

		var outcome = WheelLotusRewardSystem.TryGrantFirstClearReward(
			1,
			meta,
			progress,
			wisdom,
			garden,
			DateTimeOffset.UtcNow);

		Assert.True(outcome.Granted);
		Assert.Equal(16, outcome.LotusId);
		Assert.Equal(2, meta.PlantedLotuses.Count);
		Assert.Contains(meta.PlantedLotuses, slot => slot.LotusId == 16);
	}
}
