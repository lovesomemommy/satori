using Satori.Core.Models.Input;

namespace Satori.Core.Models.Settings;

public sealed class GameSettingsModel
{
	public string Language { get; set; } = "ru";

	public float MasterVolume { get; set; } = 0.8f;

	public bool IsMuted { get; set; }

	public bool IsFullscreen { get; set; }

	public string SelectedMusicTrackId { get; set; } = "hub";

	public InputBindingModel Bindings { get; set; } = new InputBindingModel();
}
