using Satori.Core.Models.Minigames;

namespace Satori.Core.Systems.Minigames;

public sealed class MeditationTrainingSystem
{
	public const float InhaleDurationSeconds = 4f;

	public const float HoldDurationSeconds = 4f;

	public const float ExhaleDurationSeconds = 6f;

	public MeditationState State { get; } = new MeditationState();

	public bool TryBegin()
	{
		if (State.IsActive || State.Phase == MeditationPhase.Completed)
		{
			return false;
		}

		State.Phase = MeditationPhase.Inhale;
		State.TargetLotusId = 0;
		State.PhaseElapsedSeconds = 0f;
		State.TotalElapsedSeconds = 0f;
		return true;
	}

	public void Update(float deltaSeconds, bool meditateHold)
	{
		MeditationPhase phase = State.Phase;
		if (phase == MeditationPhase.Idle || phase == MeditationPhase.Completed || phase == MeditationPhase.Interrupted)
		{
			return;
		}

		if (!meditateHold)
		{
			Interrupt();
			return;
		}

		State.PhaseElapsedSeconds += deltaSeconds;
		State.TotalElapsedSeconds += deltaSeconds;
		switch (State.Phase)
		{
		case MeditationPhase.Inhale:
			if (State.PhaseElapsedSeconds >= InhaleDurationSeconds)
			{
				Advance(MeditationPhase.Hold);
			}

			break;
		case MeditationPhase.Hold:
			if (State.PhaseElapsedSeconds >= HoldDurationSeconds)
			{
				Advance(MeditationPhase.Exhale);
			}

			break;
		case MeditationPhase.Exhale:
			if (State.PhaseElapsedSeconds >= ExhaleDurationSeconds)
			{
				Complete();
			}

			break;
		}
	}

	public float GetBreathScale()
	{
		return State.Phase switch
		{
			MeditationPhase.Inhale => Lerp(0.85f, 1.15f, State.PhaseElapsedSeconds / InhaleDurationSeconds),
			MeditationPhase.Hold => 1.15f,
			MeditationPhase.Exhale => Lerp(1.15f, 0.85f, State.PhaseElapsedSeconds / ExhaleDurationSeconds),
			_ => 1f
		};
	}

	public float GetGlowStrength()
	{
		return State.Phase switch
		{
			MeditationPhase.Inhale => 0.4f + 0.4f * (State.PhaseElapsedSeconds / InhaleDurationSeconds),
			MeditationPhase.Hold => 0.85f,
			MeditationPhase.Exhale => 0.85f - 0.4f * (State.PhaseElapsedSeconds / ExhaleDurationSeconds),
			_ => 0f
		};
	}

	public void Reset()
	{
		State.Phase = MeditationPhase.Idle;
		State.TargetLotusId = 0;
		State.PhaseElapsedSeconds = 0f;
		State.TotalElapsedSeconds = 0f;
	}

	private void Advance(MeditationPhase next)
	{
		State.Phase = next;
		State.PhaseElapsedSeconds = 0f;
	}

	private void Complete()
	{
		State.Phase = MeditationPhase.Completed;
	}

	private void Interrupt()
	{
		State.Phase = MeditationPhase.Interrupted;
		State.PhaseElapsedSeconds = 0f;
	}

	private static float Lerp(float a, float b, float t) =>
		a + (b - a) * Math.Clamp(t, 0f, 1f);
}
