namespace Satori.Core.Models.Lotus;

public sealed class LotusModel
{
	public int Id { get; set; }

	public int SegmentIndex { get; set; }

	public int TileX { get; set; }

	public int TileY { get; set; }

	public bool IsRevealed { get; set; }

	public bool HasQuote { get; set; }
}
