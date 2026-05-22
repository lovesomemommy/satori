using Satori.Core.Models.Precepts;
using Satori.Core.Models.Progression;
using Satori.Core.Models.Settings;
using Satori.Core.Models.Wisdom;

namespace Satori.Core.Models.Save;

public sealed class SaveGameModel
{
	public int Version { get; set; } = 1;

	public PlayerMetaState Meta { get; set; } = new PlayerMetaState();

	public WisdomLibraryState Wisdom { get; set; } = new WisdomLibraryState();

	public PilgrimageSaveState Pilgrimage { get; set; } = new PilgrimageSaveState();

	public PreceptProgressModel Precepts { get; set; } = new PreceptProgressModel();

	public GameSettingsModel Settings { get; set; } = new GameSettingsModel();
}
