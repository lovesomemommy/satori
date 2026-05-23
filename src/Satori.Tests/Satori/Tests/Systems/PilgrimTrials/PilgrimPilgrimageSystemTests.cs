using Satori.Core.Interfaces.Events;
using Satori.Core.Interfaces.Events.Events;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Precepts;
using Satori.Core.Models.Progression;
using Satori.Core.Models.Save;
using Satori.Core.Models.Wisdom;
using Satori.Core.Services.Wisdom;
using Satori.Core.Systems.PilgrimTrials;
using Satori.Core.Systems.Precepts;
using Satori.Core.Systems.Precepts.Handlers;
using Satori.Core.Systems.Progression;
using Satori.Core.Utilities;
using Satori.Tests.Helpers;
using Xunit;

namespace Satori.Tests.Systems.PilgrimTrials;

public sealed class PilgrimPilgrimageSystemTests
{
	[Fact]
	public void CompleteAllSegments_CommitsMetaAndPublishesEvent()
	{
		GameEventBus gameEventBus = new GameEventBus();
		PilgrimPilgrimageSystem pilgrimPilgrimageSystem = CreateSystem(gameEventBus);
		PlayerMetaState playerMetaState = new PlayerMetaState();
		PilgrimageSaveState pilgrimageSaveState = new PilgrimageSaveState();
		WisdomLibraryState wisdom = new WisdomLibraryState();
		pilgrimPilgrimageSystem.BindPersistence(playerMetaState, pilgrimageSaveState, wisdom);
		pilgrimPilgrimageSystem.Start(PilgrimageTestData.CreateDefault());
		bool completed = false;
		gameEventBus.Subscribe<PilgrimageCompletedEvent>(delegate
		{
			completed = true;
		});
		for (int i = 0; i < 5; i++)
		{
			pilgrimPilgrimageSystem.OnPlayerReachedExitPortal();
		}
		Assert.True(completed);
		Assert.Equal(TrialOutcome.Success, pilgrimPilgrimageSystem.Run.Outcome);
		Assert.True(playerMetaState.PilgrimageCompleted);
		Assert.True(pilgrimageSaveState.Completed);
	}

	[Fact]
	public void Trap_WipesRunWithoutTouchingMeta()
	{
		GameEventBus gameEventBus = new GameEventBus();
		PilgrimPilgrimageSystem pilgrimPilgrimageSystem = CreateSystem(gameEventBus);
		PlayerMetaState playerMetaState = new PlayerMetaState
		{
			Karma = 40
		};
		pilgrimPilgrimageSystem.BindPersistence(playerMetaState, new PilgrimageSaveState(), new WisdomLibraryState());
		pilgrimPilgrimageSystem.Start(PilgrimageTestData.CreateDefault());
		pilgrimPilgrimageSystem.Run.CurrentSegmentIndex = 2;
		pilgrimPilgrimageSystem.Run.RunKarma = 5;
		bool trapped = false;
		gameEventBus.Subscribe<TrapTriggeredEvent>(delegate
		{
			trapped = true;
		});
		pilgrimPilgrimageSystem.OnPlayerEnteredTile(10, 2);
		Assert.True(trapped);
		Assert.Equal(TrialOutcome.Defeat, pilgrimPilgrimageSystem.Run.Outcome);
		Assert.Equal(TrialDefeatReason.Trap, pilgrimPilgrimageSystem.Run.DefeatReason);
		Assert.Equal("pilgrim.defeat.trap", pilgrimPilgrimageSystem.Run.LastDefeatMessageKey);
		Assert.Equal(0, pilgrimPilgrimageSystem.Run.RunKarma);
		Assert.Equal(40, playerMetaState.Karma);
	}

	[Fact]
	public void PreceptViolation_WipesRunPublishesEventAndReducesMetaKarma()
	{
		GameEventBus gameEventBus = new GameEventBus();
		PilgrimPilgrimageSystem pilgrimPilgrimageSystem = CreateSystem(gameEventBus);
		PlayerMetaState playerMetaState = new PlayerMetaState { Karma = 10 };
		pilgrimPilgrimageSystem.BindPersistence(playerMetaState, new PilgrimageSaveState(), new WisdomLibraryState());
		pilgrimPilgrimageSystem.Start(PilgrimageTestData.CreateDefault());
		bool violated = false;
		gameEventBus.Subscribe<PreceptViolatedEvent>(delegate
		{
			violated = true;
		});
		pilgrimPilgrimageSystem.OnPlayerEnteredTile(6, 5);
		Assert.True(violated);
		Assert.Equal(TrialOutcome.Defeat, pilgrimPilgrimageSystem.Run.Outcome);
		Assert.Equal(TrialDefeatReason.PreceptViolation, pilgrimPilgrimageSystem.Run.DefeatReason);
		Assert.Equal("precept.no_killing.violation", pilgrimPilgrimageSystem.Run.LastDefeatMessageKey);
		Assert.Equal(7, playerMetaState.Karma);
	}

	[Fact]
	public void DecoyTile_PublishesWarningWithoutDefeat()
	{
		GameEventBus gameEventBus = new GameEventBus();
		PilgrimPilgrimageSystem pilgrimPilgrimageSystem = CreateSystem(gameEventBus);
		pilgrimPilgrimageSystem.Start(PilgrimageTestData.CreateDefault());
		pilgrimPilgrimageSystem.Run.CurrentSegmentIndex = 2;
		bool warned = false;
		gameEventBus.Subscribe<DecoyTrailEnteredEvent>(delegate
		{
			warned = true;
		});
		pilgrimPilgrimageSystem.OnPlayerEnteredTile(9, 5);
		Assert.True(warned);
		Assert.Equal(TrialOutcome.InProgress, pilgrimPilgrimageSystem.Run.Outcome);
	}

	[Fact]
	public void TimerExpiry_WipesRun()
	{
		GameEventBus bus = new GameEventBus();
		PilgrimPilgrimageSystem pilgrimPilgrimageSystem = CreateSystem(bus);
		pilgrimPilgrimageSystem.Start(PilgrimageTestData.CreateDefault());
		pilgrimPilgrimageSystem.Update(TrialTimerSystem.PilgrimageDuration);
		Assert.Equal(TrialOutcome.Defeat, pilgrimPilgrimageSystem.Run.Outcome);
		Assert.Equal(TrialDefeatReason.TimerExpired, pilgrimPilgrimageSystem.Run.DefeatReason);
		Assert.Equal("pilgrim.defeat.timer", pilgrimPilgrimageSystem.Run.LastDefeatMessageKey);
	}

	private static PilgrimPilgrimageSystem CreateSystem(IGameEventBus bus)
	{
		TrialTimerSystem timer = new TrialTimerSystem(bus);
		TrialRunSystem runSystem = new TrialRunSystem();
		SegmentTransitionSystem segmentTransition = new SegmentTransitionSystem();
		TrapSystem trapSystem = new TrapSystem();
		DecoyTrailSystem decoySystem = new DecoyTrailSystem();
		ObstacleSystem obstacles = new ObstacleSystem();
		PreceptViolationSystem preceptSystem = new PreceptViolationSystem(new IPreceptHandler[4]
		{
			new ObstaclePreceptHandler(obstacles, PreceptType.NoKilling, SegmentFocus.NoKilling, ObstacleType.Harm, "precept.no_killing.violation"),
			new ObstaclePreceptHandler(obstacles, PreceptType.NoStealing, SegmentFocus.NoStealing, ObstacleType.Temptation, "precept.no_stealing.violation"),
			new ObstaclePreceptHandler(obstacles, PreceptType.NoIntoxication, SegmentFocus.NoIntoxication, ObstacleType.Mist, "precept.no_intoxication.violation"),
			new ObstaclePreceptHandler(obstacles, PreceptType.Celibacy, SegmentFocus.Celibacy, ObstacleType.Temptation, "precept.celibacy.violation")
		});
		return new PilgrimPilgrimageSystem(
			bus,
			timer,
			runSystem,
			segmentTransition,
			trapSystem,
			decoySystem,
			preceptSystem,
			new KarmaSystem(),
			new GardenPlantingSystem());
	}
}
