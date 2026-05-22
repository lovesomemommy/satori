using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Precepts;
using Satori.Core.Systems.PilgrimTrials;

namespace Satori.Core.Systems.Precepts.Handlers;

public sealed class ObstaclePreceptHandler : IPreceptHandler
{
	private readonly ObstacleSystem _obstacles;

	private readonly PreceptType _type;

	private readonly SegmentFocus _focus;

	private readonly ObstacleType _obstacleType;

	private readonly string _violationMessageKey;

	public PreceptType Type => _type;

	public ObstaclePreceptHandler(ObstacleSystem obstacles, PreceptType type, SegmentFocus focus, ObstacleType obstacleType, string violationMessageKey)
	{
		_obstacles = obstacles;
		_type = type;
		_focus = focus;
		_obstacleType = obstacleType;
		_violationMessageKey = violationMessageKey;
	}

	public bool AppliesTo(TrialSegmentDefinition segment)
	{
		return segment.Focus == _focus;
	}

	public PreceptViolationResult Evaluate(PreceptContext context)
	{
		if (!_obstacles.HasObstacle(context.Segment, context.TileX, context.TileY, _obstacleType))
		{
			return PreceptViolationResult.None;
		}
		return new PreceptViolationResult(isViolated: true, _type, _violationMessageKey);
	}
}
