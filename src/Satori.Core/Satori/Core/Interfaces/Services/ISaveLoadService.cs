using Satori.Core.Models.Save;

namespace Satori.Core.Interfaces.Services;

public interface ISaveLoadService
{
	string DefaultSavePath { get; }

	bool HasSave();

	SaveGameModel LoadOrCreateDefault();

	void SaveDefault(SaveGameModel model);

	SaveGameModel ResetProgress(SaveGameModel current);
}
