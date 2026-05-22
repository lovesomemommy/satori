using Satori.Core.Models.Progression;
using Satori.Core.Models.Save;
using Satori.Core.Models.Settings;

namespace Satori.Client.State;

public sealed class GameSession
{
	public SaveGameModel Save { get; private set; } = new SaveGameModel();

	public PlayerMetaState Meta => Save.Meta;

	public GameSettingsModel Settings => Save.Settings;

	public void ReplaceSave(SaveGameModel save)
	{
		Save = save ?? new SaveGameModel();
	}
}
