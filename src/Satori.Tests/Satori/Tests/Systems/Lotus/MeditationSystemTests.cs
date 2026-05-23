using Satori.Core.Interfaces.Events.Events;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.Minigames;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Systems.Lotus;
using Satori.Core.Utilities;
using Xunit;

namespace Satori.Tests.Systems.Lotus;

public sealed class MeditationSystemTests
{
	[Fact]
	public void Update_ReleaseBeforeComplete_InterruptsMeditation()
	{
		GameEventBus eventBus = new GameEventBus();
		MeditationSystem meditationSystem = new MeditationSystem(eventBus);
		TrialRunState trialRunState = new TrialRunState();
		LotusModel lotus = new LotusModel
		{
			Id = 1
		};
		Assert.True(meditationSystem.TryBegin(trialRunState, lotus));
		meditationSystem.Update(trialRunState, 0.5f, meditateHold: false);
		Assert.Equal(MeditationPhase.Interrupted, meditationSystem.State.Phase);
		Assert.DoesNotContain(1, trialRunState.CollectedLotusIds);
	}

	[Fact]
	public void Update_HoldThroughAllPhases_CompletesMeditation()
	{
		GameEventBus gameEventBus = new GameEventBus();
		MeditationSystem meditationSystem = new MeditationSystem(gameEventBus);
		TrialRunState run = new TrialRunState();
		LotusModel lotus = new LotusModel
		{
			Id = 2
		};
		bool completed = false;
		gameEventBus.Subscribe<MeditationCompletedEvent>(delegate
		{
			completed = true;
		});
		Assert.True(meditationSystem.TryBegin(run, lotus));
		for (int i = 0; i < 100; i++)
		{
			meditationSystem.Update(run, 0.05f, meditateHold: true);
			if (meditationSystem.State.Phase == MeditationPhase.Completed)
			{
				break;
			}
		}
		Assert.Equal(MeditationPhase.Completed, meditationSystem.State.Phase);
		Assert.True(completed);
	}

	[Fact]
	public void GetBreathScale_InhalePhase_IncreasesScale()
	{
		MeditationSystem meditationSystem = new MeditationSystem(new GameEventBus());
		meditationSystem.State.Phase = MeditationPhase.Inhale;
		meditationSystem.State.PhaseElapsedSeconds = 1.125f;
		float breathScale = meditationSystem.GetBreathScale();
		Assert.True(breathScale > 0.95f);
	}
}
