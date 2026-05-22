using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Progression;
using Satori.Core.Models.Wisdom;
using Satori.Core.Systems.PilgrimTrials;
using Xunit;

namespace Satori.Tests.Systems.PilgrimTrials;

public sealed class TrialRunSystemTests
{
	[Fact]
	public void WipeRun_ClearsRunProgress_ButLeavesMetaUntouched()
	{
		TrialRunState trialRunState = new TrialRunState
		{
			RunKarma = 12
		};
		trialRunState.CollectedLotusIds.Add(1);
		trialRunState.CollectedLotusIds.Add(2);
		trialRunState.UnlockedQuotes.Add(new QuoteModel
		{
			QuoteId = "quote.lotus.01"
		});
		PlayerMetaState playerMetaState = new PlayerMetaState
		{
			Karma = 50,
			Enlightenment = 0.25f
		};
		TrialRunSystem trialRunSystem = new TrialRunSystem();
		trialRunSystem.WipeRun(trialRunState, TrialDefeatReason.Trap);
		Assert.Equal(TrialOutcome.Defeat, trialRunState.Outcome);
		Assert.Equal(TrialDefeatReason.Trap, trialRunState.DefeatReason);
		Assert.Empty(trialRunState.CollectedLotusIds);
		Assert.Empty(trialRunState.UnlockedQuotes);
		Assert.Equal(0, trialRunState.RunKarma);
		Assert.Equal(50, playerMetaState.Karma);
		Assert.Equal(0.25f, playerMetaState.Enlightenment);
	}

	[Fact]
	public void CommitRunToMeta_AddsRunKarmaAndMarksPilgrimageCompleted()
	{
		TrialRunState trialRunState = new TrialRunState
		{
			CurrentSegmentIndex = 4,
			RunKarma = 7
		};
		PlayerMetaState playerMetaState = new PlayerMetaState();
		TrialRunSystem trialRunSystem = new TrialRunSystem();
		trialRunSystem.StartRun(trialRunState);
		trialRunState.RunKarma = 7;
		trialRunState.CurrentSegmentIndex = 4;
		trialRunSystem.CommitRunToMeta(trialRunState, playerMetaState);
		Assert.Equal(TrialOutcome.Success, trialRunState.Outcome);
		Assert.Equal(7, playerMetaState.Karma);
		Assert.True(playerMetaState.PilgrimageCompleted);
	}
}
