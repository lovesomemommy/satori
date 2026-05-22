namespace Satori.Core.Models.Minigames;

public sealed class RightSpeechQuestionModel
{
	public string Id { get; set; } = string.Empty;

	public string Situation { get; set; } = string.Empty;

	public List<string> Options { get; set; } = new List<string>();

	public int CorrectIndex { get; set; }
}
