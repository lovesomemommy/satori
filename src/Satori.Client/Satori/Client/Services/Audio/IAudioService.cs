using Microsoft.Xna.Framework;
using Satori.Client.State;
using Satori.Core.Models.Settings;

namespace Satori.Client.Services.Audio;

public interface IAudioService
{
	void Initialize();

	void ApplySettings(GameSettingsModel settings);

	void Update(GameTime gameTime);

	void OnSceneChanged(GameStateType state);

	void PlayTrack(string trackId, bool fadeIn = true);

	void StopMusic(bool fadeOut = true);

	void PreviewCurrentTrack();
}
