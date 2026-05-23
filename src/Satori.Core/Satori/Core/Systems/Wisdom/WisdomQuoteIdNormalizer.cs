namespace Satori.Core.Systems.Wisdom;

public static class WisdomQuoteIdNormalizer
{
	private static readonly HashSet<string> CanonicalQuoteIds = new(StringComparer.Ordinal)
	{
		"quote.lotus.01",
		"quote.lotus.02",
		"quote.lotus.03",
		"quote.lotus.04",
		"quote.lotus.05",
		"quote.lotus.06",
		"quote.lotus.07",
		"quote.lotus.08",
		"quote.lotus.09",
		"quote.lotus.10"
	};

	private static readonly Dictionary<string, string> LegacyMap = new(StringComparer.Ordinal)
	{
		["quote.lotus.04"] = "quote.lotus.02",
		["quote.lotus.07"] = "quote.lotus.03",
		["quote.lotus.13"] = "quote.lotus.05",
		["quote.lotus.16"] = "quote.lotus.06",
		["quote.lotus.17"] = "quote.lotus.07",
		["quote.lotus.18"] = "quote.lotus.08",
		["quote.lotus.19"] = "quote.lotus.09",
		["quote.lotus.20"] = "quote.lotus.10"
	};

	public static string Normalize(string quoteId)
	{
		if (CanonicalQuoteIds.Contains(quoteId))
		{
			return quoteId;
		}

		return LegacyMap.TryGetValue(quoteId, out string? mapped) ? mapped : quoteId;
	}
}
