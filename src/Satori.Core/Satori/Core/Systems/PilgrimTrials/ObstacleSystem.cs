using Satori.Core.Models.PilgrimTrials;

namespace Satori.Core.Systems.PilgrimTrials;

public sealed class ObstacleSystem
{
	public bool HasObstacle(TrialSegmentDefinition segment, int tileX, int tileY, ObstacleType type)
	{
		foreach (ObstacleModel obstacle in segment.Obstacles)
		{
			if (obstacle.Type == type && obstacle.Tile.X == tileX && obstacle.Tile.Y == tileY)
			{
				return true;
			}
		}

		return false;
	}

	public ObstacleModel? GetObstacle(TrialSegmentDefinition segment, int tileX, int tileY)
	{
		foreach (ObstacleModel obstacle in segment.Obstacles)
		{
			if (obstacle.Tile.X == tileX && obstacle.Tile.Y == tileY)
			{
				return obstacle;
			}
		}

		return null;
	}
}
