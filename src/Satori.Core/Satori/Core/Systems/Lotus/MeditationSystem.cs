using System;
using Satori.Core.Interfaces.Events;
using Satori.Core.Interfaces.Events.Events;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.Minigames;
using Satori.Core.Models.PilgrimTrials;

namespace Satori.Core.Systems.Lotus;

public sealed class MeditationSystem
{
	public const float InhaleDurationSeconds = 1.5f;

	public const float HoldDurationSeconds = 1f;

	public const float ExhaleDurationSeconds = 1.5f;

	private readonly IGameEventBus _eventBus;

	public MeditationState State { get; } = new MeditationState();

	public MeditationSystem(IGameEventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public bool TryBegin(TrialRunState run, LotusModel lotus)
	{
		if (!run.IsActive || run.CollectedLotusIds.Contains(lotus.Id))
		{
			return false;
		}
		if (State.IsActive)
		{
			return false;
		}
		State.Phase = MeditationPhase.Inhale;
		State.TargetLotusId = lotus.Id;
		State.PhaseElapsedSeconds = 0f;
		State.TotalElapsedSeconds = 0f;
		return true;
	}

	public void Update(TrialRunState run, float deltaSeconds, bool meditateHold)
	{
		MeditationPhase phase = State.Phase;
		if ((phase == MeditationPhase.Idle || (uint)(phase - 4) <= 1u) ? true : false)
		{
			return;
		}
		if (!meditateHold)
		{
			Interrupt();
			return;
		}
		if (!run.IsActive)
		{
			Interrupt();
			return;
		}
		State.PhaseElapsedSeconds += deltaSeconds;
		State.TotalElapsedSeconds += deltaSeconds;
		switch (State.Phase)
		{
		case MeditationPhase.Inhale:
			if (State.PhaseElapsedSeconds >= 1.5f)
			{
				Advance(MeditationPhase.Hold);
			}
			break;
		case MeditationPhase.Hold:
			if (State.PhaseElapsedSeconds >= 1f)
			{
				Advance(MeditationPhase.Exhale);
			}
			break;
		case MeditationPhase.Exhale:
			if (State.PhaseElapsedSeconds >= 1.5f)
			{
				Complete();
			}
			break;
		}
	}

	public float GetBreathScale() =>
		State.Phase switch
		{
			MeditationPhase.Inhale => Lerp(0.85f, 1.15f, State.PhaseElapsedSeconds / 1.5f),
			MeditationPhase.Hold => 1.15f,
			MeditationPhase.Exhale => Lerp(1.15f, 0.85f, State.PhaseElapsedSeconds / 1.5f),
			_ => 1f
		};

	public float GetGlowStrength() =>
		State.Phase switch
		{
			MeditationPhase.Inhale => 0.4f + 0.4f * (State.PhaseElapsedSeconds / 1.5f),
			MeditationPhase.Hold => 0.85f,
			MeditationPhase.Exhale => 0.85f - 0.4f * (State.PhaseElapsedSeconds / 1.5f),
			_ => 0f
		};

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
		_eventBus.Publish(new MeditationCompletedEvent(State.TargetLotusId));
	}

	private void Interrupt()
	{
		MeditationPhase phase = State.Phase;
		if ((phase != 0 && phase != MeditationPhase.Completed) || 1 == 0)
		{
			int targetLotusId = State.TargetLotusId;
			State.Phase = MeditationPhase.Interrupted;
			State.PhaseElapsedSeconds = 0f;
			_eventBus.Publish(new MeditationInterruptedEvent(targetLotusId));
		}
	}

	private static float Lerp(float a, float b, float t)
	{
		return a + (b - a) * Math.Clamp(t, 0f, 1f);
	}
}
