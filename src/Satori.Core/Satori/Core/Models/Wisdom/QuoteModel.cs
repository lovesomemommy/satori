using System;

namespace Satori.Core.Models.Wisdom;

public sealed class QuoteModel
{
	public string QuoteId { get; set; } = string.Empty;

	public DateTimeOffset FoundAt { get; set; }

	public int SourceSegmentIndex { get; set; }
}
