using Satori.Core.Models.Save;

namespace Satori.Core.Interfaces.Services;

public interface ISaveRepository
{
	SaveGameModel Load(string path);

	void SaveAtomic(string path, SaveGameModel model);

	bool Exists(string path);
}
