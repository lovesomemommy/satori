using Satori.Core.Models.PilgrimTrials;

namespace Satori.Core.Systems.PilgrimTrials;

public sealed class TrapSystem
{
	public bool IsTrap(TrialSegmentDefinition segment, int tileX, int tileY)
	{
		foreach (TrapModel trap in segment.Traps)
		{
			if (trap.Tile.X == tileX && trap.Tile.Y == tileY)
			{
				return true;
			}
		}

		return false;
	}
}
