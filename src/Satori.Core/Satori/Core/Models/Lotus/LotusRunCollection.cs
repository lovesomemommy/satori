using System.Collections.Generic;

namespace Satori.Core.Models.Lotus;

public sealed class LotusRunCollection
{
	public List<LotusModel> Collected { get; } = new List<LotusModel>();
}
