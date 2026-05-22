using System;
using Satori.Core.Models.Lotus;

namespace Satori.Core.Models.Progression;

public sealed class GardenSlotModel
{
	public int SlotIndex { get; set; }

	public int LotusId { get; set; }

	public LotusType LotusType { get; set; }

	public DateTimeOffset PlantedAt { get; set; }
}
