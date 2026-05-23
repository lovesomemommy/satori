using Satori.Core.Models.Save;

namespace Satori.Core.Systems.Progression;

public sealed class WinConditionStatus
{
	public bool PilgrimageCompleted { get; init; }

	public bool GardenFull { get; init; }

	public bool WisdomGathered { get; init; }

	public bool EnlightenmentReached { get; init; }

	public bool IsComplete =>
		PilgrimageCompleted &&
		GardenFull &&
		WisdomGathered &&
		EnlightenmentReached;
}

public static class WinConditionSystem
{
	public const int RequiredQuoteCount = 10;

	public const float RequiredEnlightenment = 0.95f;

	public static WinConditionStatus Evaluate(SaveGameModel save)
	{
		return new WinConditionStatus
		{
			PilgrimageCompleted = save.Meta.PilgrimageCompleted,
			GardenFull = save.Meta.PlantedLotuses.Count >= GardenPlantingSystem.MaxSlots,
			WisdomGathered = CountDistinctCanonicalQuotes(save) >= RequiredQuoteCount,
			EnlightenmentReached = save.Meta.Enlightenment >= RequiredEnlightenment
		};
	}

	public static int CountDistinctCanonicalQuotes(SaveGameModel save) =>
		save.Wisdom.Quotes
			.Select(quote => quote.QuoteId)
			.Distinct(StringComparer.Ordinal)
			.Count();
}
