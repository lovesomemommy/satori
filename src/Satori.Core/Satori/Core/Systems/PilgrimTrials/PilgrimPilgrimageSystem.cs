using System;
using System.Linq;
using Satori.Core.Interfaces.Events;
using Satori.Core.Interfaces.Events.Events;
using Satori.Core.Interfaces.Systems;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Progression;
using Satori.Core.Models.Save;
using Satori.Core.Models.Wisdom;
using Satori.Core.Services.Wisdom;
using Satori.Core.Systems.Precepts;
using Satori.Core.Systems.Progression;
using Satori.Core.Systems.Wisdom;

namespace Satori.Core.Systems.PilgrimTrials;

public sealed class PilgrimPilgrimageSystem : IPilgrimPilgrimageSystem
{
	private readonly IGameEventBus _eventBus;

	private readonly TrialTimerSystem _timer;

	private readonly TrialRunSystem _runSystem;

	private readonly SegmentTransitionSystem _segmentTransition;

	private readonly TrapSystem _trapSystem;

	private readonly DecoyTrailSystem _decoySystem;

	private readonly PreceptViolationSystem _preceptSystem;

	private readonly KarmaSystem _karmaSystem;

	private readonly GardenPlantingSystem _gardenPlanting;

	private readonly LotusCatalog _lotusCatalog;

	private PilgrimPilgrimageDefinition? _definition;

	private PlayerMetaState? _meta;

	private PilgrimageSaveState? _pilgrimageSave;

	private WisdomLibraryState? _wisdom;

	public bool IsActive => Run.IsActive;

	public TrialRunState Run { get; } = new TrialRunState();

	public PilgrimPilgrimageSystem(IGameEventBus eventBus, TrialTimerSystem timer, TrialRunSystem runSystem, SegmentTransitionSystem segmentTransition, TrapSystem trapSystem, DecoyTrailSystem decoySystem, PreceptViolationSystem preceptSystem, KarmaSystem karmaSystem, GardenPlantingSystem gardenPlanting, LotusCatalog lotusCatalog)
	{
		_eventBus = eventBus;
		_timer = timer;
		_runSystem = runSystem;
		_segmentTransition = segmentTransition;
		_trapSystem = trapSystem;
		_decoySystem = decoySystem;
		_preceptSystem = preceptSystem;
		_karmaSystem = karmaSystem;
		_gardenPlanting = gardenPlanting;
		_lotusCatalog = lotusCatalog;
		_eventBus.Subscribe<TrialTimerExpiredEvent>(delegate
		{
			HandleTimerExpired();
		});
	}

	public void BindPersistence(PlayerMetaState meta, PilgrimageSaveState pilgrimageSave, WisdomLibraryState wisdom)
	{
		_meta = meta;
		_pilgrimageSave = pilgrimageSave;
		_wisdom = wisdom;
	}

	public TrialSegmentDefinition? GetCurrentSegment()
	{
		return GetCurrentSegmentInternal();
	}

	public void Start(PilgrimPilgrimageDefinition definition)
	{
		_definition = definition;
		_runSystem.StartRun(Run);
		_timer.Start(Run);
	}

	public void Update(TimeSpan delta)
	{
		if (Run.IsActive)
		{
			_timer.Tick(Run, delta);
		}
	}

	public void OnPlayerReachedExitPortal()
	{
		if (Run.IsActive && _definition != null)
		{
			switch (_segmentTransition.TryAdvance(Run))
			{
			case SegmentTransitionResult.Advanced:
				_eventBus.Publish(new SegmentCompletedEvent(Run.CurrentSegmentIndex));
				break;
			case SegmentTransitionResult.PilgrimageComplete:
				CompletePilgrimage();
				break;
			}
		}
	}

	public void OnPlayerEnteredTile(int tileX, int tileY)
	{
		if (!Run.IsActive || _definition == null)
		{
			return;
		}
		TrialSegmentDefinition? currentSegmentInternal = GetCurrentSegmentInternal();
		if (currentSegmentInternal == null)
		{
			return;
		}
		PreceptViolationResult preceptViolationResult = _preceptSystem.Evaluate(currentSegmentInternal, tileX, tileY);
		if (preceptViolationResult.IsViolated)
		{
			if (_meta != null)
			{
				_karmaSystem.ApplyMetaPenalty(_meta, KarmaValues.PreceptViolationPenalty);
				EnlightenmentSystem.Recalculate(_meta);
			}

			Run.LastDefeatMessageKey = preceptViolationResult.MessageKey;
			_runSystem.WipeRun(Run, TrialDefeatReason.PreceptViolation);
			_eventBus.Publish(new PreceptViolatedEvent(preceptViolationResult.PreceptType, preceptViolationResult.MessageKey, Run.CurrentSegmentIndex));
			return;
		}
		if (_decoySystem.IsDecoyTile(currentSegmentInternal, tileX, tileY))
		{
			_eventBus.Publish(new DecoyTrailEnteredEvent(Run.CurrentSegmentIndex));
		}
		OnPlayerEnteredTrapTile(tileX, tileY);
	}

	public void OnPlayerEnteredTrapTile(int tileX, int tileY)
	{
		if (Run.IsActive && _definition != null)
		{
			TrialSegmentDefinition? currentSegmentInternal = GetCurrentSegmentInternal();
			if (currentSegmentInternal != null && _trapSystem.IsTrap(currentSegmentInternal, tileX, tileY))
			{
				Run.LastDefeatMessageKey = "pilgrim.defeat.trap";
				_runSystem.WipeRun(Run, TrialDefeatReason.Trap);
				_eventBus.Publish(new TrapTriggeredEvent(Run.CurrentSegmentIndex));
			}
		}
	}

	private void HandleTimerExpired()
	{
		if (Run.IsActive)
		{
			Run.LastDefeatMessageKey = "pilgrim.defeat.timer";
			_runSystem.HandleTimerDefeat(Run);
		}
	}

	private void CompletePilgrimage()
	{
		if (_meta != null && _wisdom != null)
		{
			_runSystem.CommitRunToMeta(Run, _meta);
			WisdomCommitSystem.CommitRunQuotes(Run, _wisdom);
			WisdomCommitSystem.CommitRunLotuses(Run, _meta, _definition);
			_gardenPlanting.PlantCollectedLotuses(_meta, Run.CollectedLotusIds, _lotusCatalog, DateTimeOffset.UtcNow);
			EnlightenmentSystem.Recalculate(_meta);
			if (_pilgrimageSave != null)
			{
				_pilgrimageSave.Completed = _meta.PilgrimageCompleted;
			}
			_eventBus.Publish(new PilgrimageCompletedEvent());
		}
	}

	private TrialSegmentDefinition? GetCurrentSegmentInternal()
	{
		if (_definition == null)
		{
			return null;
		}
		return _definition.Segments.FirstOrDefault((TrialSegmentDefinition s) => s.SegmentIndex == Run.CurrentSegmentIndex);
	}
}
