using System;

namespace Satori.Client.Scenes.Transitions;

public sealed class SceneTransitionController
{
	public const float FadeDurationSeconds = 0.35f;

	public SceneTransitionPhase Phase { get; private set; } = SceneTransitionPhase.Idle;

	public float Alpha { get; private set; }

	public bool IsActive => Phase != SceneTransitionPhase.Idle;

	public void BeginFadeOut()
	{
		Phase = SceneTransitionPhase.FadeOut;
		Alpha = 0f;
	}

	public bool Update(float deltaSeconds)
	{
		if (Phase == SceneTransitionPhase.Idle)
		{
			return false;
		}
		float num = deltaSeconds / 0.35f;
		if (Phase == SceneTransitionPhase.FadeOut)
		{
			Alpha = Math.Min(1f, Alpha + num);
			if (Alpha < 1f)
			{
				return false;
			}
			Phase = SceneTransitionPhase.FadeIn;
			return true;
		}
		Alpha = Math.Max(0f, Alpha - num);
		if (Alpha > 0f)
		{
			return false;
		}
		Alpha = 0f;
		Phase = SceneTransitionPhase.Idle;
		return false;
	}

	public void Reset()
	{
		Phase = SceneTransitionPhase.Idle;
		Alpha = 0f;
	}
}
