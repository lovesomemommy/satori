using System;
using Satori.Core.Interfaces.Events;
using Satori.Core.Interfaces.Events.Events;
using Satori.Core.Models.PilgrimTrials;

namespace Satori.Core.Systems.PilgrimTrials;

public sealed class TrialTimerSystem
{
	public static readonly TimeSpan PilgrimageDuration = TimeSpan.FromMinutes(4.0) + TimeSpan.FromSeconds(44.0);

	private readonly IGameEventBus _eventBus;

	private bool _expiredPublished;

	public TimeSpan Duration => PilgrimageDuration;

	public TrialTimerSystem(IGameEventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Start(TrialRunState run)
	{
		run.RemainingTime = PilgrimageDuration;
		_expiredPublished = false;
	}

	public void Tick(TrialRunState run, TimeSpan delta)
	{
		if (run.IsActive && !_expiredPublished)
		{
			run.RemainingTime -= delta;
			if (!(run.RemainingTime > TimeSpan.Zero))
			{
				run.RemainingTime = TimeSpan.Zero;
				_expiredPublished = true;
				_eventBus.Publish(new TrialTimerExpiredEvent());
			}
		}
	}

	public bool IsExpired(TrialRunState run)
	{
		return run.RemainingTime <= TimeSpan.Zero;
	}
}
