using System.Collections.Generic;

namespace Satori.Core.Models.Progression;

public sealed class PlayerMetaState
{
	public int Karma { get; set; }

	public float Enlightenment { get; set; }

	public bool PilgrimageCompleted { get; set; }

	public List<GardenSlotModel> PlantedLotuses { get; set; } = new List<GardenSlotModel>();

	public HashSet<int> UnlockedLotusIds { get; set; } = new HashSet<int>();
}
