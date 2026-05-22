using System.Collections.Generic;
using Satori.Core.Models.Precepts;
using Satori.Core.Models.Progression;
using Satori.Core.Models.Save;
using Satori.Core.Models.Settings;
using Satori.Core.Models.Wisdom;

namespace Satori.Core.Services.Save;

public sealed class SaveMigrator
{
	public SaveGameModel Migrate(SaveGameModel model)
	{
		if (model.Version <= 0)
		{
			model.Version = 1;
		}
		SaveGameModel saveGameModel = model;
		if (saveGameModel.Meta == null)
		{
			PlayerMetaState playerMetaState2 = (saveGameModel.Meta = new PlayerMetaState());
		}
		saveGameModel = model;
		if (saveGameModel.Wisdom == null)
		{
			WisdomLibraryState wisdomLibraryState2 = (saveGameModel.Wisdom = new WisdomLibraryState());
		}
		saveGameModel = model;
		if (saveGameModel.Pilgrimage == null)
		{
			PilgrimageSaveState pilgrimageSaveState2 = (saveGameModel.Pilgrimage = new PilgrimageSaveState());
		}
		saveGameModel = model;
		if (saveGameModel.Precepts == null)
		{
			PreceptProgressModel preceptProgressModel2 = (saveGameModel.Precepts = new PreceptProgressModel());
		}
		saveGameModel = model;
		if (saveGameModel.Settings == null)
		{
			GameSettingsModel gameSettingsModel2 = (saveGameModel.Settings = new GameSettingsModel());
		}

		model.Meta.PlantedLotuses ??= new List<GardenSlotModel>();
		model.Meta.UnlockedLotusIds ??= new HashSet<int>();
		model.Precepts.CompletedRightSpeechQuestionIds ??= new HashSet<string>();
		return model;
	}
}
