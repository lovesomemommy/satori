using System;
using Satori.Core.Interfaces.Events.Events;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Systems.PilgrimTrials;
using Satori.Core.Utilities;
using Xunit;

namespace Satori.Tests.Systems.PilgrimTrials;

public sealed class TrialTimerSystemTests
{
	[Fact]
	public void Tick_WhenDurationElapses_PublishesTrialTimerExpiredEvent()
	{
		GameEventBus gameEventBus = new GameEventBus();
		TrialTimerSystem trialTimerSystem = new TrialTimerSystem(gameEventBus);
		TrialRunState trialRunState = new TrialRunState();
		bool published = false;
		gameEventBus.Subscribe<TrialTimerExpiredEvent>(delegate
		{
			published = true;
		});
		trialTimerSystem.Start(trialRunState);
		trialTimerSystem.Tick(trialRunState, TrialTimerSystem.PilgrimageDuration);
		Assert.True(published);
		Assert.Equal(TimeSpan.Zero, trialRunState.RemainingTime);
		Assert.True(trialTimerSystem.IsExpired(trialRunState));
	}

	[Fact]
	public void PilgrimageDuration_IsFourMinutesFortyFourSeconds()
	{
		Assert.Equal(new TimeSpan(0, 4, 44), TrialTimerSystem.PilgrimageDuration);
	}
}
