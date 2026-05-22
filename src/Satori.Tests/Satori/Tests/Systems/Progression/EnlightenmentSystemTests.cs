using Satori.Core.Models.Progression;
using Satori.Core.Systems.Progression;
using Xunit;

namespace Satori.Tests.Systems.Progression;

public sealed class EnlightenmentSystemTests
{
	[Fact]
	public void Recalculate_UsesPlantedLotusesKarmaAndCompletion()
	{
		var meta = new PlayerMetaState
		{
			Karma = 100,
			PilgrimageCompleted = true
		};
		meta.PlantedLotuses.Add(new GardenSlotModel { SlotIndex = 0, LotusId = 1 });
		meta.PlantedLotuses.Add(new GardenSlotModel { SlotIndex = 1, LotusId = 2 });

		EnlightenmentSystem.Recalculate(meta);

		Assert.Equal(0.66f, meta.Enlightenment, precision: 2);
	}
}
