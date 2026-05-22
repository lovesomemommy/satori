namespace Satori.Core.Models.Minigames;

public sealed class RightSpeechCatalogFileModel
{
	public int Version { get; set; } = 1;

	public List<RightSpeechQuestionModel> Questions { get; set; } = new List<RightSpeechQuestionModel>();
}
