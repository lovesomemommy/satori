using System.Collections.Generic;

namespace Satori.Core.Models.Wisdom;

public sealed class WisdomLibraryState
{
	public List<QuoteModel> Quotes { get; set; } = new List<QuoteModel>();
}
