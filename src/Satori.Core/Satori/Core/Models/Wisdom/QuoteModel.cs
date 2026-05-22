using System;
using Satori.Core.Models.Lotus;

namespace Satori.Core.Models.Wisdom;

public sealed class QuoteModel
{
	public string QuoteId { get; set; } = string.Empty;

	public QuoteRarity Rarity { get; set; }

	public DateTimeOffset FoundAt { get; set; }

	public int SourceSegmentIndex { get; set; }

	public LotusType SourceLotusType { get; set; }
}
