using Satori.Core.Interfaces.Events.Events;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Systems.Lotus;
using Satori.Core.Systems.Progression;
using Satori.Core.Utilities;
using Xunit;

namespace Satori.Tests.Systems.Lotus;

public sealed class LotusCollectionSystemTests
{
	[Fact]
	public void TryCollect_AddsKarmaAndPublishesEvent()
	{
		GameEventBus gameEventBus = new GameEventBus();
		KarmaSystem karmaSystem = new KarmaSystem();
		LotusCollectionSystem lotusCollectionSystem = new LotusCollectionSystem(gameEventBus, karmaSystem);
		TrialRunState trialRunState = new TrialRunState();
		LotusModel lotus = new LotusModel
		{
			Id = 1,
			Type = LotusType.Rare
		};
		LotusCollectedEvent published = null;
		gameEventBus.Subscribe(delegate(LotusCollectedEvent e)
		{
			published = e;
		});
		bool condition = lotusCollectionSystem.TryCollect(trialRunState, lotus);
		Assert.True(condition);
		Assert.Contains(1, trialRunState.CollectedLotusIds);
		Assert.Equal(3, trialRunState.RunKarma);
		Assert.NotNull(published);
		Assert.Equal(LotusType.Rare, published.Type);
	}

	[Fact]
	public void TryCollect_DuplicateLotus_ReturnsFalse()
	{
		GameEventBus eventBus = new GameEventBus();
		LotusCollectionSystem lotusCollectionSystem = new LotusCollectionSystem(eventBus, new KarmaSystem());
		TrialRunState trialRunState = new TrialRunState();
		LotusModel lotus = new LotusModel
		{
			Id = 1,
			Type = LotusType.Common
		};
		trialRunState.CollectedLotusIds.Add(1);
		Assert.False(lotusCollectionSystem.TryCollect(trialRunState, lotus));
	}
}
