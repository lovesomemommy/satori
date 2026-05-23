using System;
using System.Collections.Generic;
using System.Linq;
using Satori.Core.Models.Precepts;
using Satori.Core.Models.Progression;
using Satori.Core.Models.Save;
using Satori.Core.Models.Settings;
using Satori.Core.Models.Wisdom;
using Satori.Core.Systems.Minigames;
using Satori.Core.Systems.Progression;
using Satori.Core.Systems.Wisdom;

namespace Satori.Core.Services.Save;

public sealed class SaveMigrator
{
	public SaveGameModel Migrate(SaveGameModel model)
	{
		model.Version = model.Version <= 0 ? 1 : model.Version;
		model.Meta ??= new PlayerMetaState();
		model.Wisdom ??= new WisdomLibraryState();
		model.Pilgrimage ??= new PilgrimageSaveState();
		model.Precepts ??= new PreceptProgressModel();
		model.Settings ??= new GameSettingsModel();
		model.Meta.PlantedLotuses ??= new List<GardenSlotModel>();
		model.Meta.UnlockedLotusIds ??= new HashSet<int>();
		model.Precepts.CompletedRightSpeechQuestionIds ??= new HashSet<string>();
		model.Precepts.WheelLotusRewardsClaimed ??= new HashSet<int>();

		if (model.Version < SaveSchemaVersion.Current)
		{
			ApplyLegacyQuoteMigration(model);
			model.Version = SaveSchemaVersion.Current;
		}

		return model;
	}

	private static void ApplyLegacyQuoteMigration(SaveGameModel model)
	{
		NormalizeWisdomQuotes(model.Wisdom);
		RepairWheelRewards(model);
	}

	private static void NormalizeWisdomQuotes(WisdomLibraryState wisdom)
	{
		var uniqueQuotes = new Dictionary<string, QuoteModel>(StringComparer.Ordinal);
		foreach (QuoteModel quote in wisdom.Quotes)
		{
			string quoteId = WisdomQuoteIdNormalizer.Normalize(quote.QuoteId);
			if (uniqueQuotes.ContainsKey(quoteId))
			{
				continue;
			}

			uniqueQuotes[quoteId] = new QuoteModel
			{
				QuoteId = quoteId,
				FoundAt = quote.FoundAt,
				SourceSegmentIndex = quote.SourceSegmentIndex
			};
		}

		wisdom.Quotes.Clear();
		wisdom.Quotes.AddRange(uniqueQuotes.Values.OrderBy(quote => quote.QuoteId, StringComparer.Ordinal));
	}

	private static void RepairWheelRewards(SaveGameModel model)
	{
		if (model.Precepts.WheelLotusRewardsClaimed.Count == 0)
		{
			return;
		}

		var garden = new GardenPlantingSystem();
		foreach (int difficulty in model.Precepts.WheelLotusRewardsClaimed.OrderBy(level => level))
		{
			int lotusId = WheelLotusRewardSystem.GetLotusIdForDifficulty(difficulty);
			string quoteId = WheelLotusRewardSystem.GetQuoteIdForDifficulty(difficulty);
			model.Meta.UnlockedLotusIds.Add(lotusId);
			garden.PlantCollectedLotuses(model.Meta, [lotusId], DateTimeOffset.UtcNow);
			if (model.Wisdom.Quotes.All(quote => quote.QuoteId != quoteId))
			{
				model.Wisdom.Quotes.Add(new QuoteModel
				{
					QuoteId = quoteId,
					FoundAt = DateTimeOffset.UtcNow,
					SourceSegmentIndex = -1
				});
			}
		}

		NormalizeWisdomQuotes(model.Wisdom);
		EnlightenmentSystem.Recalculate(model.Meta);
	}
}
