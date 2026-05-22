using System.Collections.Generic;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.Precepts;

namespace Satori.Core.Models.PilgrimTrials;

public sealed class TrialSegmentDefinition
{
	public int SegmentIndex { get; set; }

	public string TitleKey { get; set; } = string.Empty;

	public SegmentFocus Focus { get; set; }

	public int Width { get; set; }

	public int Height { get; set; }

	public SpawnPoint Spawn { get; set; } = new SpawnPoint();

	public PortalPoint ExitPortal { get; set; } = new PortalPoint();

	public List<LotusModel> Lotuses { get; set; } = new List<LotusModel>();

	public List<DecoyTrailModel> DecoyTrails { get; set; } = new List<DecoyTrailModel>();

	public List<TrapModel> Traps { get; set; } = new List<TrapModel>();

	public List<TilePoint> Walls { get; set; } = new List<TilePoint>();

	public List<ObstacleModel> Obstacles { get; set; } = new List<ObstacleModel>();
}
