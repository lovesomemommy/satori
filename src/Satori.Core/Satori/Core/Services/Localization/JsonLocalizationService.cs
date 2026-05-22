using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Satori.Core.Interfaces.Services;

namespace Satori.Core.Services.Localization;

public sealed class JsonLocalizationService : ILocalizationService
{
	private readonly Dictionary<string, string> _strings;

	public JsonLocalizationService(Dictionary<string, string> strings)
	{
		_strings = strings;
	}

	public static JsonLocalizationService LoadFromJson(string json)
	{
		Dictionary<string, string> strings = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
		return new JsonLocalizationService(strings);
	}

	public static JsonLocalizationService LoadFromFile(string path)
	{
		string json = File.ReadAllText(path);
		return LoadFromJson(json);
	}

	public string Get(string key)
	{
		string value;
		return TryGet(key, out value) ? value : key;
	}

	public bool TryGet(string key, out string value)
	{
		if (_strings.TryGetValue(key, out var found))
		{
			value = found;
			return true;
		}

		value = string.Empty;
		return false;
	}
}
