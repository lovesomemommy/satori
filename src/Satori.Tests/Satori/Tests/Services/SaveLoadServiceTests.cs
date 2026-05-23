using System;
using System.IO;
using Satori.Core.Models.Progression;
using Satori.Core.Models.Save;
using Satori.Core.Models.Settings;
using Satori.Core.Models.Wisdom;
using Satori.Core.Services.Save;
using Xunit;

namespace Satori.Tests.Services;

public sealed class SaveLoadServiceTests
{
	[Fact]
	public void SaveAtomic_RoundTrip_PreservesMetaAndWisdom()
	{
		JsonSaveRepository jsonSaveRepository = new JsonSaveRepository();
		string text = Path.Combine(Path.GetTempPath(), $"satori_test_{Guid.NewGuid():N}.json");
		try
		{
			SaveGameModel model = new SaveGameModel
			{
				Meta = new PlayerMetaState
				{
					Karma = 15,
					Enlightenment = 0.5f,
					PilgrimageCompleted = true
				},
				Wisdom = new WisdomLibraryState
				{
					Quotes = 
					{
						new QuoteModel
						{
							QuoteId = "quote.01"
						}
					}
				},
				Settings = new GameSettingsModel
				{
					Language = "ru",
					MasterVolume = 0.6f,
					IsMuted = true,
					IsFullscreen = true,
					SelectedMusicTrackId = "hub",
					Bindings = new Satori.Core.Models.Input.InputBindingModel
					{
						Meditate = "Space",
						Pause = "P"
					}
				},
				Pilgrimage = new PilgrimageSaveState
				{
					Completed = true
				}
			};
			jsonSaveRepository.SaveAtomic(text, model);
			SaveGameModel saveGameModel = jsonSaveRepository.Load(text);
			Assert.Equal(SaveSchemaVersion.Current, saveGameModel.Version);
			Assert.Equal(15, saveGameModel.Meta.Karma);
			Assert.True(saveGameModel.Meta.PilgrimageCompleted);
			Assert.Single(saveGameModel.Wisdom.Quotes);
			Assert.Equal("quote.01", saveGameModel.Wisdom.Quotes[0].QuoteId);
			Assert.Equal("ru", saveGameModel.Settings.Language);
			Assert.Equal(0.6f, saveGameModel.Settings.MasterVolume);
			Assert.True(saveGameModel.Settings.IsMuted);
			Assert.True(saveGameModel.Settings.IsFullscreen);
			Assert.Equal("hub", saveGameModel.Settings.SelectedMusicTrackId);
			Assert.Equal("Space", saveGameModel.Settings.Bindings.Meditate);
			Assert.Equal("P", saveGameModel.Settings.Bindings.Pause);
			Assert.True(saveGameModel.Pilgrimage.Completed);
			jsonSaveRepository.SaveAtomic(text, saveGameModel);
			Assert.True(File.Exists(text + ".bak"));
		}
		finally
		{
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			if (File.Exists(text + ".bak"))
			{
				File.Delete(text + ".bak");
			}
		}
	}

	[Fact]
	public void SaveLoadService_RoundTrip_PreservesGarden()
	{
		var repository = new JsonSaveRepository();
		var service = new SaveLoadService();
		string path = Path.Combine(Path.GetTempPath(), $"satori_garden_{Guid.NewGuid():N}.json");
		try
		{
			var original = new SaveGameModel();
			original.Meta.PlantedLotuses.Add(new GardenSlotModel
			{
				SlotIndex = 0,
				LotusId = 2,
				PlantedAt = DateTimeOffset.UtcNow
			});
			original.Meta.UnlockedLotusIds.Add(2);
			repository.SaveAtomic(path, original);

			var loaded = repository.Load(path);

			Assert.Single(loaded.Meta.PlantedLotuses);
			Assert.Equal(2, loaded.Meta.PlantedLotuses[0].LotusId);
			Assert.Contains(2, loaded.Meta.UnlockedLotusIds);
		}
		finally
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
			if (File.Exists(path + ".bak"))
			{
				File.Delete(path + ".bak");
			}
		}
	}

	[Fact]
	public void ResetProgress_PreservesSettings_ClearsMeta()
	{
		var service = new SaveLoadService();
		var original = new SaveGameModel();
		original.Meta.Karma = 42;
		original.Meta.PilgrimageCompleted = true;
		original.Settings.MasterVolume = 0.4f;
		original.Settings.Bindings.Pause = "P";

		var reset = service.ResetProgress(original);

		Assert.Equal(0, reset.Meta.Karma);
		Assert.False(reset.Meta.PilgrimageCompleted);
		Assert.Equal(0.4f, reset.Settings.MasterVolume);
		Assert.Equal("Escape", reset.Settings.Bindings.Pause);
	}
}
