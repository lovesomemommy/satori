using Satori.Core.Models.Minigames;
using Satori.Core.Models.Precepts;
using Satori.Core.Models.Progression;
using Satori.Core.Systems.Minigames;
using Xunit;

namespace Satori.Tests.Systems.Minigames;

public sealed class WheelOfDharmaSystemTests
{
	[Fact]
	public void GetSequenceLength_MapsFiveDifficultyLevels()
	{
		Assert.Equal(3, WheelOfDharmaSystem.GetSequenceLength(1));
		Assert.Equal(4, WheelOfDharmaSystem.GetSequenceLength(2));
		Assert.Equal(5, WheelOfDharmaSystem.GetSequenceLength(3));
		Assert.Equal(6, WheelOfDharmaSystem.GetSequenceLength(4));
		Assert.Equal(7, WheelOfDharmaSystem.GetSequenceLength(5));
	}

	[Fact]
	public void TryInput_WrongDirection_FailsRound()
	{
		var wheel = new WheelOfDharmaSystem();
		wheel.State.Sequence.Add(WheelDirection.Up);
		wheel.State.Phase = WheelOfDharmaPhase.Input;
		wheel.State.InputIndex = 0;

		var result = wheel.TryInput(WheelDirection.Down);

		Assert.Equal(WheelInputResult.Failed, result);
		Assert.Equal(WheelOfDharmaPhase.Failed, wheel.State.Phase);
	}

	[Fact]
	public void TryInput_CompletesSequence_Succeeds()
	{
		var wheel = new WheelOfDharmaSystem();
		wheel.State.Sequence.AddRange(new[] { WheelDirection.Left, WheelDirection.Right });
		wheel.State.Phase = WheelOfDharmaPhase.Input;
		wheel.State.InputIndex = 0;
		wheel.State.Difficulty = 1;

		Assert.Equal(WheelInputResult.Advanced, wheel.TryInput(WheelDirection.Left));
		Assert.Equal(WheelInputResult.Completed, wheel.TryInput(WheelDirection.Right));
		Assert.Equal(WheelOfDharmaPhase.Succeeded, wheel.State.Phase);
	}

	[Fact]
	public void ApplySuccess_UnlocksNextDifficulty()
	{
		var wheel = new WheelOfDharmaSystem();
		wheel.State.Difficulty = 1;
		wheel.State.Phase = WheelOfDharmaPhase.Succeeded;
		var meta = new PlayerMetaState();
		var progress = new PreceptProgressModel();

		var outcome = wheel.ApplySuccess(meta, progress);

		Assert.True(outcome.UnlockedNextDifficulty);
		Assert.Equal(2, progress.WheelHighestDifficultyUnlocked);
		Assert.Equal(1, progress.WheelSuccessCount);
	}
}
