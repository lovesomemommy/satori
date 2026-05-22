using Satori.Core.Models.PilgrimTrials;

namespace Satori.Core.Systems.Precepts;

public sealed class PreceptContext
{
	public TrialSegmentDefinition Segment { get; }

	public int TileX { get; }

	public int TileY { get; }

	public PreceptContext(TrialSegmentDefinition segment, int tileX, int tileY)
	{
		Segment = segment;
		TileX = tileX;
		TileY = tileY;
	}
}
