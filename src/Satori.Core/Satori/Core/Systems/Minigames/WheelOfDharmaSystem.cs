using Satori.Core.Models.Minigames;
using Satori.Core.Models.Precepts;
using Satori.Core.Models.Progression;
using Satori.Core.Systems.Progression;

namespace Satori.Core.Systems.Minigames;

public sealed class WheelOfDharmaSystem
{
	public const float ShowStepDurationSeconds = 1.35f;

	public const float ShowGapDurationSeconds = 0.6f;

	public const float InputHighlightDurationSeconds = 0.35f;

	public const int MinDifficulty = 1;

	public const int MaxDifficulty = 5;

	public WheelOfDharmaState State { get; } = new WheelOfDharmaState();

	public static int GetSequenceLength(int difficulty) => Math.Clamp(difficulty + 2, 3, 7);

	public void StartRound(int difficulty, Random random, bool shortenAfterFail = false)
	{
		difficulty = Math.Clamp(difficulty, MinDifficulty, MaxDifficulty);
		int length = GetSequenceLength(difficulty);
		if (shortenAfterFail && length > 3)
		{
			length--;
		}

		State.Difficulty = difficulty;
		State.Sequence = GenerateSequence(length, random);
		State.ShowIndex = 0;
		State.ShowStepTimer = 0f;
		State.ShowHighlightActive = State.Sequence.Count > 0;
		State.ActiveShowDirection = State.ShowHighlightActive ? State.Sequence[0] : null;
		State.ActiveInputDirection = null;
		State.InputHighlightTimer = 0f;
		State.InputIndex = 0;
		State.Phase = WheelOfDharmaPhase.Showing;
	}

	public void Update(float deltaSeconds)
	{
		if (State.Phase == WheelOfDharmaPhase.Input && State.InputHighlightTimer > 0f)
		{
			State.InputHighlightTimer = Math.Max(0f, State.InputHighlightTimer - deltaSeconds);
			if (State.InputHighlightTimer <= 0f)
			{
				State.ActiveInputDirection = null;
			}
		}

		if (State.Phase != WheelOfDharmaPhase.Showing || State.Sequence.Count == 0)
		{
			return;
		}

		State.ShowStepTimer += deltaSeconds;
		float stepDuration = State.ShowHighlightActive ? ShowStepDurationSeconds : ShowGapDurationSeconds;
		if (State.ShowStepTimer < stepDuration)
		{
			State.ActiveShowDirection = State.ShowHighlightActive && State.ShowIndex < State.Sequence.Count
				? State.Sequence[State.ShowIndex]
				: null;
			return;
		}

		State.ShowStepTimer = 0f;
		if (State.ShowHighlightActive)
		{
			State.ShowHighlightActive = false;
			State.ActiveShowDirection = null;
			return;
		}

		State.ShowIndex++;
		if (State.ShowIndex >= State.Sequence.Count)
		{
			State.Phase = WheelOfDharmaPhase.Input;
			State.ActiveShowDirection = null;
			State.ActiveInputDirection = null;
			State.InputHighlightTimer = 0f;
			State.InputIndex = 0;
			return;
		}

		State.ShowHighlightActive = true;
		State.ActiveShowDirection = State.Sequence[State.ShowIndex];
	}

	public WheelInputResult TryInput(WheelDirection direction)
	{
		if (State.Phase != WheelOfDharmaPhase.Input)
		{
			return WheelInputResult.Ignored;
		}

		State.ActiveInputDirection = direction;
		State.InputHighlightTimer = InputHighlightDurationSeconds;

		if (State.Sequence[State.InputIndex] != direction)
		{
			State.Phase = WheelOfDharmaPhase.Failed;
			State.ActiveShowDirection = null;
			return WheelInputResult.Failed;
		}

		State.InputIndex++;
		if (State.InputIndex >= State.Sequence.Count)
		{
			State.Phase = WheelOfDharmaPhase.Succeeded;
			State.ActiveShowDirection = null;
			return WheelInputResult.Completed;
		}

		return WheelInputResult.Advanced;
	}

	public WheelRoundOutcome ApplySuccess(PlayerMetaState meta, PreceptProgressModel progress)
	{
		int difficulty = State.Difficulty;
		meta.Karma += KarmaValues.WheelSuccessReward;
		progress.WheelSuccessCount++;
		int unlockedBefore = progress.WheelHighestDifficultyUnlocked;
		progress.WheelHighestDifficultyUnlocked = Math.Max(
			progress.WheelHighestDifficultyUnlocked,
			Math.Min(difficulty + 1, MaxDifficulty));

		EnlightenmentSystem.Recalculate(meta);
		return new WheelRoundOutcome
		{
			KarmaDelta = KarmaValues.WheelSuccessReward,
			UnlockedNextDifficulty = progress.WheelHighestDifficultyUnlocked > unlockedBefore
		};
	}

	public WheelRoundOutcome ApplyFailure(PlayerMetaState meta, PreceptProgressModel progress)
	{
		_ = progress;
		meta.Karma = Math.Max(0, meta.Karma - KarmaValues.WheelFailPenalty);
		EnlightenmentSystem.Recalculate(meta);
		return new WheelRoundOutcome
		{
			KarmaDelta = -KarmaValues.WheelFailPenalty,
			UnlockedNextDifficulty = false
		};
	}

	public void Reset()
	{
		State.Phase = WheelOfDharmaPhase.Idle;
		State.Sequence.Clear();
		State.ShowIndex = 0;
		State.ShowStepTimer = 0f;
		State.ShowHighlightActive = false;
		State.ActiveShowDirection = null;
		State.ActiveInputDirection = null;
		State.InputHighlightTimer = 0f;
		State.InputIndex = 0;
	}

	private static List<WheelDirection> GenerateSequence(int length, Random random)
	{
		var directions = new List<WheelDirection>(length);
		for (int i = 0; i < length; i++)
		{
			directions.Add((WheelDirection)random.Next(4));
		}

		return directions;
	}
}

public enum WheelInputResult
{
	Ignored,
	Advanced,
	Failed,
	Completed
}

public sealed class WheelRoundOutcome
{
	public int KarmaDelta { get; init; }

	public bool UnlockedNextDifficulty { get; init; }
}
