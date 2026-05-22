using System.Collections.Generic;

namespace Satori.Core.Models.PilgrimTrials;

public sealed class DecoyTrailModel
{
	public List<TilePoint> Path { get; set; } = new List<TilePoint>();
}
