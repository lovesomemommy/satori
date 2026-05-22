using System.Linq;
using Satori.Core.Models.PilgrimTrials;

namespace Satori.Core.Systems.PilgrimTrials;

public sealed class TrapSystem
{
	public bool IsTrap(TrialSegmentDefinition segment, int tileX, int tileY)
	{
		return segment.Traps.Any((TrapModel trap) => trap.Tile.X == tileX && trap.Tile.Y == tileY);
	}
}
