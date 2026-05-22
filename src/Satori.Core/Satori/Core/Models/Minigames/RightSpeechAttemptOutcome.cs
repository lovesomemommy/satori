namespace Satori.Core.Models.Minigames;

public sealed class RightSpeechAttemptOutcome
{
	public bool IsCorrect { get; init; }

	public int KarmaDelta { get; init; }

	public bool RevealCorrectAnswer => false;

	public bool AllQuestionsComplete { get; init; }
}
