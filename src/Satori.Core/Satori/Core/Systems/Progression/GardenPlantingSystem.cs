using System;
using System.Collections.Generic;
using System.Linq;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.Progression;
using Satori.Core.Services.Wisdom;

namespace Satori.Core.Systems.Progression;

public sealed class GardenPlantingSystem
{
	public const int MaxSlots = 20;

	public int PlantCollectedLotuses(
		PlayerMetaState meta,
		IReadOnlyCollection<int> collectedLotusIds,
		LotusCatalog lotusCatalog,
		DateTimeOffset plantedAt)
	{
		if (collectedLotusIds.Count == 0)
		{
			return 0;
		}

		int plantedCount = 0;
		foreach (int lotusId in collectedLotusIds.OrderBy(id => id))
		{
			if (meta.PlantedLotuses.Any(slot => slot.LotusId == lotusId))
			{
				continue;
			}

			if (meta.PlantedLotuses.Count >= MaxSlots)
			{
				break;
			}

			meta.PlantedLotuses.Add(new GardenSlotModel
			{
				SlotIndex = FindNextFreeSlot(meta),
				LotusId = lotusId,
				LotusType = lotusCatalog.GetType(lotusId),
				PlantedAt = plantedAt
			});
			plantedCount++;
		}

		return plantedCount;
	}

	private static int FindNextFreeSlot(PlayerMetaState meta)
	{
		for (int slotIndex = 0; slotIndex < MaxSlots; slotIndex++)
		{
			if (meta.PlantedLotuses.All(slot => slot.SlotIndex != slotIndex))
			{
				return slotIndex;
			}
		}

		return meta.PlantedLotuses.Count;
	}
}
