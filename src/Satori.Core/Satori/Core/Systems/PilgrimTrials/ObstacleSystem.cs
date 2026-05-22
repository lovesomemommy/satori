using System.Linq;
using Satori.Core.Models.PilgrimTrials;

namespace Satori.Core.Systems.PilgrimTrials;

public sealed class ObstacleSystem
{
	public bool HasObstacle(TrialSegmentDefinition segment, int tileX, int tileY, ObstacleType type)
	{
		return segment.Obstacles.Any((ObstacleModel obstacle) => obstacle.Type == type && obstacle.Tile.X == tileX && obstacle.Tile.Y == tileY);
	}

	public ObstacleModel? GetObstacle(TrialSegmentDefinition segment, int tileX, int tileY)
	{
		return segment.Obstacles.FirstOrDefault((ObstacleModel obstacle) => obstacle.Tile.X == tileX && obstacle.Tile.Y == tileY);
	}
}
