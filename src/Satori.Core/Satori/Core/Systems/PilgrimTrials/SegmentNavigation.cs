using Satori.Core.Models.PilgrimTrials;

namespace Satori.Core.Systems.PilgrimTrials;

public static class SegmentNavigation
{
	public static bool IsWalkable(TrialSegmentDefinition segment, int tileX, int tileY)
	{
		if (tileX < 0 || tileY < 0 || tileX >= segment.Width || tileY >= segment.Height)
		{
			return false;
		}

		foreach (TilePoint wall in segment.Walls)
		{
			if (wall.X == tileX && wall.Y == tileY)
			{
				return false;
			}
		}

		return true;
	}

	public static bool IsPortal(TrialSegmentDefinition segment, int tileX, int tileY) =>
		segment.ExitPortal.X == tileX && segment.ExitPortal.Y == tileY;
}
