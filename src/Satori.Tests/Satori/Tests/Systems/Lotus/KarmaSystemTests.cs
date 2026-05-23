using Satori.Core.Models.Progression;
using Satori.Core.Systems.Progression;
using Xunit;

namespace Satori.Tests.Systems.Lotus;

public sealed class KarmaSystemTests
{
	[Fact]
	public void ApplyMetaPenalty_DoesNotGoBelowZero()
	{
		KarmaSystem karmaSystem = new KarmaSystem();
		PlayerMetaState playerMetaState = new PlayerMetaState
		{
			Karma = 2
		};
		karmaSystem.ApplyMetaPenalty(playerMetaState, 5);
		Assert.Equal(0, playerMetaState.Karma);
	}

	[Fact]
	public void GetLotusKarma_ReturnsFlatReward()
	{
		var karmaSystem = new KarmaSystem();
		Assert.Equal(KarmaValues.LotusCollectReward, karmaSystem.GetLotusKarma());
	}
}
