using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.Wisdom;

namespace Satori.Core.Services.Wisdom;

public sealed class QuoteCatalog
{
	private readonly Dictionary<string, string> _texts;

	public QuoteCatalog(Dictionary<string, string> texts)
	{
		_texts = texts;
	}

	public static QuoteCatalog LoadFromFile(string path)
	{
		string json = File.ReadAllText(path);
		Dictionary<string, string> texts = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
		return new QuoteCatalog(texts);
	}

	public static QuoteCatalog CreateDefault()
	{
		string path = Path.Combine(AppContext.BaseDirectory, "Quotes", "quotes.ru.json");
		if (File.Exists(path))
		{
			return LoadFromFile(path);
		}
		return new QuoteCatalog(new Dictionary<string, string> { ["quote.lotus.01"] = "Ты сам должен приложить усилие. Будды лишь указывают путь." });
	}

	public string GetQuoteIdForLotus(int lotusId)
	{
		return $"quote.lotus.{lotusId:00}";
	}

	public bool TryGetText(string quoteId, out string text)
	{
		if (_texts.TryGetValue(quoteId, out var found))
		{
			text = found;
			return true;
		}

		text = string.Empty;
		return false;
	}

	public QuoteRarity RarityForLotus(LotusType type)
	{
		if (1 == 0)
		{
		}
		QuoteRarity result = type switch
		{
			LotusType.Rare => QuoteRarity.Rare, 
			LotusType.Golden => QuoteRarity.Golden, 
			LotusType.Spiritual => QuoteRarity.Spiritual, 
			_ => QuoteRarity.Common, 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
