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

	public static string GetQuoteIdForDifficulty(int difficulty) => $"quote.lotus.{5 + difficulty:00}";

	public static WheelLotusRewardOutcome TryGrantFirstClearReward(
		int difficulty,
		PlayerMetaState meta,
		PreceptProgressModel progress,
		WisdomLibraryState wisdom,
		GardenPlantingSystem gardenPlanting,
		DateTimeOffset grantedAt)
	{
		if (!progress.WheelLotusRewardsClaimed.Add(difficulty))
		{
			return WheelLotusRewardOutcome.NotGranted;
		}

		int lotusId = GetLotusIdForDifficulty(difficulty);
		string quoteId = GetQuoteIdForDifficulty(difficulty);
		meta.UnlockedLotusIds.Add(lotusId);
		if (wisdom.Quotes.All(quote => quote.QuoteId != quoteId))
		{
			wisdom.Quotes.Add(new QuoteModel
			{
				QuoteId = quoteId,
				FoundAt = grantedAt,
				SourceSegmentIndex = -1
			});
		}

		gardenPlanting.PlantCollectedLotuses(meta, [lotusId], grantedAt);
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
