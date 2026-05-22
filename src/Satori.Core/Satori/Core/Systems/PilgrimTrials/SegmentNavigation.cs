using System.Linq;
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
		return segment.Walls.All((TilePoint wall) => wall.X != tileX || wall.Y != tileY);
	}

	public static bool IsPortal(TrialSegmentDefinition segment, int tileX, int tileY)
	{
		return segment.ExitPortal.X == tileX && segment.ExitPortal.Y == tileY;
	}
}
