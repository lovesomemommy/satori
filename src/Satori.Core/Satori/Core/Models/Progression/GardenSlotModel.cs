using System;

namespace Satori.Core.Models.Progression;

public sealed class GardenSlotModel
{
	public int SlotIndex { get; set; }

	public int LotusId { get; set; }

	public DateTimeOffset PlantedAt { get; set; }
}
