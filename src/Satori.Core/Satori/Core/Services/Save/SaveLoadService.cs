using System;
using Satori.Core.Interfaces.Services;
using Satori.Core.Models.Input;
using Satori.Core.Models.Save;
using Satori.Core.Models.Settings;
using Satori.Core.Utilities;

namespace Satori.Core.Services.Save;

public sealed class SaveLoadService : ISaveLoadService
{
	private readonly JsonSaveRepository _repository = new();

	public string DefaultSavePath => PlatformPaths.GetDefaultSaveFilePath();

	public bool HasSave() => _repository.Exists(DefaultSavePath);

	public SaveGameModel LoadOrCreateDefault()
	{
		if (!HasSave())
		{
			return new SaveGameModel();
		}

		var save = _repository.Load(DefaultSavePath);
		if (NormalizeBindings(save.Settings.Bindings))
		{
			SaveDefault(save);
		}

		return save;
	}

	public void SaveDefault(SaveGameModel model) => _repository.SaveAtomic(DefaultSavePath, model);

	public SaveGameModel ResetProgress(SaveGameModel current)
	{
		var fresh = new SaveGameModel { Settings = CopySettings(current.Settings) };
		SaveDefault(fresh);
		return fresh;
	}

	private static GameSettingsModel CopySettings(GameSettingsModel source)
	{
		var bindings = new InputBindingModel
		{
			MoveUp = source.Bindings.MoveUp,
			MoveDown = source.Bindings.MoveDown,
			MoveLeft = source.Bindings.MoveLeft,
			MoveRight = source.Bindings.MoveRight,
			Meditate = source.Bindings.Meditate,
			Pause = source.Bindings.Pause,
			Interact = source.Bindings.Interact
		};
		NormalizeBindings(bindings);
		return new GameSettingsModel
		{
			Language = source.Language,
			MasterVolume = source.MasterVolume,
			IsMuted = source.IsMuted,
			IsFullscreen = source.IsFullscreen,
			SelectedMusicTrackId = source.SelectedMusicTrackId,
			Bindings = bindings
		};
	}

	private static bool NormalizeBindings(InputBindingModel bindings)
	{
		if (!string.Equals(bindings.Pause, "P", StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		bindings.Pause = "Escape";
		return true;
	}
}
