namespace Satori.Core.Interfaces.Services;

public interface ILocalizationService
{
	string Get(string key);

	bool TryGet(string key, out string value);
}
