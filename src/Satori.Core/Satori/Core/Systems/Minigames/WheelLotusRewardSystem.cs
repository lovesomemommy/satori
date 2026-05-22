using System;
using System.Linq;
using Satori.Core.Models.Precepts;
using Satori.Core.Models.Progression;
using Satori.Core.Models.Wisdom;
using Satori.Core.Services.Wisdom;
using Satori.Core.Systems.Progression;

namespace Satori.Core.Systems.Minigames;

public static class WheelLotusRewardSystem
{
	public const int FirstLotusId = 16;

	public const int LastLotusId = 20;

	public static int GetLotusIdForDifficulty(int difficulty) => FirstLotusId - 1 + difficulty;

	public static WheelLotusRewardOutcome TryGrantFirstClearReward(
		int difficulty,
		PlayerMetaState meta,
		PreceptProgressModel progress,
		WisdomLibraryState wisdom,
		LotusCatalog lotusCatalog,
		QuoteCatalog quoteCatalog,
		GardenPlantingSystem gardenPlanting,
		DateTimeOffset grantedAt)
	{
		if (!progress.WheelLotusRewardsClaimed.Add(difficulty))
		{
			return WheelLotusRewardOutcome.NotGranted;
		}

		int lotusId = GetLotusIdForDifficulty(difficulty);
		meta.UnlockedLotusIds.Add(lotusId);
		string quoteId = quoteCatalog.GetQuoteIdForLotus(lotusId);
		if (wisdom.Quotes.All(quote => quote.QuoteId != quoteId))
		{
			wisdom.Quotes.Add(new QuoteModel
			{
				QuoteId = quoteId,
				Rarity = quoteCatalog.RarityForLotus(lotusCatalog.GetType(lotusId)),
				FoundAt = grantedAt,
				SourceSegmentIndex = -1,
				SourceLotusType = lotusCatalog.GetType(lotusId)
			});
		}

		gardenPlanting.PlantCollectedLotuses(meta, [lotusId], lotusCatalog, grantedAt);
		EnlightenmentSystem.Recalculate(meta);
		return new WheelLotusRewardOutcome
		{
			Granted = true,
			LotusId = lotusId,
			QuoteId = quoteId
		};
	}
}

public sealed class WheelLotusRewardOutcome
{
	public bool Granted { get; init; }

	public int LotusId { get; init; }

	public string QuoteId { get; init; } = string.Empty;

	public static WheelLotusRewardOutcome NotGranted { get; } = new();
}
