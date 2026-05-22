using Satori.Core.Models.Minigames;
using Satori.Core.Models.Precepts;
using Satori.Core.Models.Progression;
using Satori.Core.Services.Minigames;
using Satori.Core.Systems.Progression;

namespace Satori.Core.Systems.Minigames;

public sealed class RightSpeechSystem
{
	private readonly RightSpeechCatalog _catalog;

	private readonly KarmaSystem _karmaSystem;

	public RightSpeechQuestionModel? CurrentQuestion { get; private set; }

	public RightSpeechSystem(RightSpeechCatalog catalog, KarmaSystem karmaSystem)
	{
		_catalog = catalog;
		_karmaSystem = karmaSystem;
	}

	public int TotalQuestionCount => _catalog.Questions.Count;

	public void ReloadCatalog() => _catalog.Reload();

	public void SyncProgressWithCatalog(PreceptProgressModel progress)
	{
		progress.CompletedRightSpeechQuestionIds ??= new HashSet<string>();
		string revision = _catalog.GetRevision();
		if (progress.RightSpeechCatalogRevision != revision)
		{
			progress.CompletedRightSpeechQuestionIds.Clear();
			progress.RightSpeechCatalogRevision = revision;
			progress.RightSpeechCatalogVersion = _catalog.Version;
		}

		if (_catalog.Questions.Count == 0)
		{
			progress.CompletedRightSpeechQuestionIds.Clear();
			return;
		}

		var validIds = _catalog.Questions.Select(question => question.Id).ToHashSet();
		progress.CompletedRightSpeechQuestionIds.RemoveWhere(id => !validIds.Contains(id));
	}

	public bool IsAllComplete(PreceptProgressModel progress) =>
		_catalog.Questions.Count > 0
		&& _catalog.Questions.All(question => progress.CompletedRightSpeechQuestionIds.Contains(question.Id));

	public bool TryStartQuestion(Random random, PreceptProgressModel progress)
	{
		SyncProgressWithCatalog(progress);

		if (IsAllComplete(progress))
		{
			CurrentQuestion = null;
			return false;
		}

		CurrentQuestion = _catalog.PickRandom(random, progress.CompletedRightSpeechQuestionIds);
		return CurrentQuestion != null;
	}

	public RightSpeechAttemptOutcome TryAnswer(PlayerMetaState meta, PreceptProgressModel progress, int selectedIndex)
	{
		if (CurrentQuestion == null)
		{
			return new RightSpeechAttemptOutcome { IsCorrect = false, KarmaDelta = 0 };
		}

		bool isCorrect = selectedIndex == CurrentQuestion.CorrectIndex;
		if (isCorrect)
		{
			bool firstTime = progress.CompletedRightSpeechQuestionIds.Add(CurrentQuestion.Id);
			if (firstTime)
			{
				meta.Karma += KarmaValues.RightSpeechSuccessReward;
				progress.RightSpeechSuccessCount++;
				EnlightenmentSystem.Recalculate(meta);
				return new RightSpeechAttemptOutcome
				{
					IsCorrect = true,
					KarmaDelta = KarmaValues.RightSpeechSuccessReward,
					AllQuestionsComplete = IsAllComplete(progress)
				};
			}

			return new RightSpeechAttemptOutcome
			{
				IsCorrect = true,
				KarmaDelta = 0,
				AllQuestionsComplete = IsAllComplete(progress)
			};
		}

		_karmaSystem.ApplyMetaPenalty(meta, KarmaValues.RightSpeechFailPenalty);
		EnlightenmentSystem.Recalculate(meta);
		return new RightSpeechAttemptOutcome
		{
			IsCorrect = false,
			KarmaDelta = -KarmaValues.RightSpeechFailPenalty
		};
	}

	public void ClearQuestion() => CurrentQuestion = null;
}
