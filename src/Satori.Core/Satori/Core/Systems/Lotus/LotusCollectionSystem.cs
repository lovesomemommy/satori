using Satori.Core.Interfaces.Events;
using Satori.Core.Interfaces.Events.Events;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Systems.Progression;

namespace Satori.Core.Systems.Lotus;

public sealed class LotusCollectionSystem
{
	private readonly IGameEventBus _eventBus;

	private readonly KarmaSystem _karmaSystem;

	public LotusCollectionSystem(IGameEventBus eventBus, KarmaSystem karmaSystem)
	{
		_eventBus = eventBus;
		_karmaSystem = karmaSystem;
	}

	public bool TryCollect(TrialRunState run, LotusModel lotus)
	{
		if (!run.IsActive || run.CollectedLotusIds.Contains(lotus.Id))
		{
			return false;
		}
		run.CollectedLotusIds.Add(lotus.Id);
		_karmaSystem.AddRunKarma(run, _karmaSystem.GetLotusKarma(lotus.Type));
		_eventBus.Publish(new LotusCollectedEvent(lotus.Id, lotus.Type));
		return true;
	}
}
