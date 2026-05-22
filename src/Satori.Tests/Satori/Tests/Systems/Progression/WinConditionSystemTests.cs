using Satori.Core.Models.Progression;
using Satori.Core.Models.Save;
using Satori.Core.Models.Wisdom;
using Satori.Core.Systems.Progression;
using Xunit;

namespace Satori.Tests.Systems.Progression;

public sealed class WinConditionSystemTests
{
	[Fact]
	public void Evaluate_IncompletePilgrimage_ReturnsFalse()
	{
		var save = CreateNearCompleteSave();
		save.Meta.PilgrimageCompleted = false;

		var status = WinConditionSystem.Evaluate(save);

		Assert.False(status.IsComplete);
		Assert.False(status.PilgrimageCompleted);
	}

	[Fact]
	public void Evaluate_AllCriteriaMet_ReturnsTrue()
	{
		var save = CreateNearCompleteSave();

		var status = WinConditionSystem.Evaluate(save);

		Assert.True(status.IsComplete);
		Assert.True(status.GardenFull);
		Assert.True(status.WisdomGathered);
		Assert.True(status.EnlightenmentReached);
	}

	private static SaveGameModel CreateNearCompleteSave()
	{
		var save = new SaveGameModel
		{
			Meta = new PlayerMetaState
			{
				Karma = 200,
				Enlightenment = 0.95f,
				PilgrimageCompleted = true
			}
		};

		for (int slotIndex = 0; slotIndex < GardenPlantingSystem.MaxSlots; slotIndex++)
		{
			save.Meta.PlantedLotuses.Add(new GardenSlotModel
			{
				SlotIndex = slotIndex,
				LotusId = slotIndex + 1
			});
		}

		for (int quoteIndex = 0; quoteIndex < WinConditionSystem.RequiredQuoteCount; quoteIndex++)
		{
			save.Wisdom.Quotes.Add(new QuoteModel
			{
				QuoteId = $"quote.lotus.{quoteIndex + 1:00}"
			});
		}

		return save;
	}
}
