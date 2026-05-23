using System;
using Satori.Core.Models.Progression;
using Satori.Core.Systems.Progression;
using Xunit;

namespace Satori.Tests.Systems.Progression;

public sealed class GardenPlantingSystemTests
{
	[Fact]
	public void PlantCollectedLotuses_AddsOnlyNewLotusesToFreeSlots()
	{
		var meta = new PlayerMetaState();
		meta.PlantedLotuses.Add(new GardenSlotModel
		{
			SlotIndex = 0,
			LotusId = 1,
			PlantedAt = DateTimeOffset.UtcNow
		});
		var system = new GardenPlantingSystem();

		int planted = system.PlantCollectedLotuses(meta, new[] { 1, 2, 3 }, DateTimeOffset.UtcNow);

		Assert.Equal(2, planted);
		Assert.Equal(3, meta.PlantedLotuses.Count);
		Assert.Contains(meta.PlantedLotuses, slot => slot.LotusId == 2 && slot.SlotIndex == 1);
		Assert.Contains(meta.PlantedLotuses, slot => slot.LotusId == 3 && slot.SlotIndex == 2);
	}

	[Fact]
	public void PlantCollectedLotuses_StopsAtMaxSlots()
	{
		var meta = new PlayerMetaState();
		var system = new GardenPlantingSystem();
		var lotusIds = new int[GardenPlantingSystem.MaxSlots + 3];
		for (int i = 0; i < lotusIds.Length; i++)
		{
			lotusIds[i] = i + 1;
		}

		int planted = system.PlantCollectedLotuses(meta, lotusIds, DateTimeOffset.UtcNow);

		Assert.Equal(GardenPlantingSystem.MaxSlots, planted);
		Assert.Equal(GardenPlantingSystem.MaxSlots, meta.PlantedLotuses.Count);
	}
}
