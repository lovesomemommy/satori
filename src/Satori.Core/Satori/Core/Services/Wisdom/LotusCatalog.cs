using System.Collections.Generic;
using Satori.Core.Models.Lotus;

namespace Satori.Core.Services.Wisdom;

public sealed class LotusCatalog
{
	private readonly Dictionary<int, LotusType> _types = new Dictionary<int, LotusType>();

	public LotusCatalog Register(int lotusId, LotusType type)
	{
		_types[lotusId] = type;
		return this;
	}

	public LotusType GetType(int lotusId) =>
		_types.TryGetValue(lotusId, out LotusType type) ? type : LotusType.Common;

	public static LotusCatalog CreateDefault()
	{
		var catalog = new LotusCatalog();
		for (int lotusId = 1; lotusId <= 20; lotusId++)
		{
			catalog.Register(lotusId, LotusType.Common);
		}

		return catalog;
	}
}
