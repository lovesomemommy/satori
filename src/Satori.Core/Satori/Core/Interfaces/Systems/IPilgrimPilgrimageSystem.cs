using System;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Progression;
using Satori.Core.Models.Save;
using Satori.Core.Models.Wisdom;

namespace Satori.Core.Interfaces.Systems;

public interface IPilgrimPilgrimageSystem
{
	bool IsActive { get; }

	TrialRunState Run { get; }

	TrialSegmentDefinition? GetCurrentSegment();

	void BindPersistence(PlayerMetaState meta, PilgrimageSaveState pilgrimageSave, WisdomLibraryState wisdom);

	void Start(PilgrimPilgrimageDefinition definition);

	void Update(TimeSpan delta);

	void OnPlayerReachedExitPortal();

	void OnPlayerEnteredTile(int tileX, int tileY);

	void OnPlayerEnteredTrapTile(int tileX, int tileY);
}
