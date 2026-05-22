using System;
using System.Collections.Generic;
using System.Linq;
using Satori.Core.Models.PilgrimTrials;

namespace Satori.Core.Systems.PilgrimTrials;

public sealed class DecoyTrailSystem
{
	public bool IsDecoyTile(TrialSegmentDefinition segment, int tileX, int tileY)
	{
		foreach (DecoyTrailModel decoyTrail in segment.DecoyTrails)
		{
			if (decoyTrail.Path.Any((TilePoint point) => point.X == tileX && point.Y == tileY))
			{
				return true;
			}
		}
		return false;
	}

	public bool LeadsToTrap(TrialSegmentDefinition segment, int tileX, int tileY, TrapSystem traps)
	{
		if (!IsDecoyTile(segment, tileX, tileY))
		{
			return false;
		}
		return segment.Traps.Count > 0;
	}

	public IReadOnlyList<TilePoint> GetPrimaryPath(TrialSegmentDefinition segment)
	{
		var path = segment.DecoyTrails.FirstOrDefault()?.Path;
		return path ?? (IReadOnlyList<TilePoint>)Array.Empty<TilePoint>();
	}
}
