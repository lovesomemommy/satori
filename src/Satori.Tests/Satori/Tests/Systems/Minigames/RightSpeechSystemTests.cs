using Satori.Core.Models.Precepts;
using Satori.Core.Models.Progression;
using Satori.Core.Services.Minigames;
using Satori.Core.Systems.Minigames;
using Satori.Core.Systems.Progression;
using Xunit;

namespace Satori.Tests.Systems.Minigames;

public sealed class RightSpeechSystemTests
{
	[Fact]
	public void TryAnswer_WrongAnswer_DoesNotRevealCorrectAnswer()
	{
		var catalog = new RightSpeechCatalog(1, new[]
		{
			new Core.Models.Minigames.RightSpeechQuestionModel
			{
				Id = "test",
				Situation = "Test",
				Options = new List<string> { "A", "B", "C" },
				CorrectIndex = 1
			}
		});
		var system = new RightSpeechSystem(catalog, new KarmaSystem());
		var meta = new PlayerMetaState { Karma = 10 };
		var progress = new PreceptProgressModel { RightSpeechCatalogRevision = catalog.GetRevision() };
		Assert.True(system.TryStartQuestion(new Random(1), progress));

		var outcome = system.TryAnswer(meta, progress, selectedIndex: 0);

		Assert.False(outcome.IsCorrect);
		Assert.False(outcome.RevealCorrectAnswer);
		Assert.Equal(8, meta.Karma);
		Assert.Equal(0, progress.RightSpeechSuccessCount);
		Assert.Empty(progress.CompletedRightSpeechQuestionIds);
	}

	[Fact]
	public void TryAnswer_CorrectAnswer_IncreasesKarmaAndProgress()
	{
		var catalog = new RightSpeechCatalog(1, new[]
		{
			new Core.Models.Minigames.RightSpeechQuestionModel
			{
				Id = "test",
				Situation = "Test",
				Options = new List<string> { "A", "B" },
				CorrectIndex = 1
			}
		});
		var system = new RightSpeechSystem(catalog, new KarmaSystem());
		var meta = new PlayerMetaState { Karma = 5 };
		var progress = new PreceptProgressModel { RightSpeechCatalogRevision = catalog.GetRevision() };
		Assert.True(system.TryStartQuestion(new Random(1), progress));

		var outcome = system.TryAnswer(meta, progress, selectedIndex: 1);

		Assert.True(outcome.IsCorrect);
		Assert.Equal(KarmaValues.RightSpeechSuccessReward, outcome.KarmaDelta);
		Assert.Equal(7, meta.Karma);
		Assert.Equal(1, progress.RightSpeechSuccessCount);
		Assert.Contains("test", progress.CompletedRightSpeechQuestionIds);
		Assert.True(outcome.AllQuestionsComplete);
	}

	[Fact]
	public void TryStartQuestion_WhenAllCompleted_ReturnsFalse()
	{
		var catalog = new RightSpeechCatalog(1, new[]
		{
			new Core.Models.Minigames.RightSpeechQuestionModel
			{
				Id = "test",
				Situation = "Test",
				Options = new List<string> { "A", "B" },
				CorrectIndex = 1
			}
		});
		var system = new RightSpeechSystem(catalog, new KarmaSystem());
		var progress = new PreceptProgressModel { RightSpeechCatalogRevision = catalog.GetRevision() };
		progress.CompletedRightSpeechQuestionIds.Add("test");

		Assert.True(system.IsAllComplete(progress));
		Assert.False(system.TryStartQuestion(new Random(1), progress));
	}

	[Fact]
	public void SyncProgressWithCatalog_RemovesStaleQuestionIds()
	{
		var catalog = new RightSpeechCatalog(2, new[]
		{
			new Core.Models.Minigames.RightSpeechQuestionModel
			{
				Id = "rs2.01",
				Situation = "Test",
				Options = new List<string> { "A", "B" },
				CorrectIndex = 1
			}
		});
		var system = new RightSpeechSystem(catalog, new KarmaSystem());
		var progress = new PreceptProgressModel { RightSpeechCatalogRevision = "1:rs.01" };
		progress.CompletedRightSpeechQuestionIds.Add("rs.01");

		system.SyncProgressWithCatalog(progress);

		Assert.Equal(catalog.GetRevision(), progress.RightSpeechCatalogRevision);
		Assert.Empty(progress.CompletedRightSpeechQuestionIds);
		Assert.False(system.IsAllComplete(progress));
		Assert.True(system.TryStartQuestion(new Random(1), progress));
	}

	[Fact]
	public void SyncProgressWithCatalog_NewCatalogRevision_ClearsCompletedQuestions()
	{
		var catalog = new RightSpeechCatalog(4, new[]
		{
			new Core.Models.Minigames.RightSpeechQuestionModel
			{
				Id = "rs4.01",
				Situation = "Test",
				Options = new List<string> { "A", "B" },
				CorrectIndex = 1
			}
		});
		var system = new RightSpeechSystem(catalog, new KarmaSystem());
		var progress = new PreceptProgressModel
		{
			RightSpeechCatalogRevision = "3:rs3.01,rs3.02"
		};
		progress.CompletedRightSpeechQuestionIds.Add("rs3.01");
		progress.CompletedRightSpeechQuestionIds.Add("rs3.02");

		system.SyncProgressWithCatalog(progress);

		Assert.Equal(catalog.GetRevision(), progress.RightSpeechCatalogRevision);
		Assert.Empty(progress.CompletedRightSpeechQuestionIds);
		Assert.True(system.TryStartQuestion(new Random(1), progress));
	}
}
