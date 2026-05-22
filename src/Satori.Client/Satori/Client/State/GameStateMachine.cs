using System;

namespace Satori.Client.State;

public sealed class GameStateMachine
{
	public GameStateType Current { get; private set; } = GameStateType.Boot;

	public event Action<GameStateType>? StateChanged;

	public void TransitionTo(GameStateType next)
	{
		if (Current != next)
		{
			Current = next;
			this.StateChanged?.Invoke(next);
		}
	}
}
