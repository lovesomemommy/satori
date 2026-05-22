using System.Collections.Generic;

namespace Satori.Core.Models.Precepts;

public sealed class PreceptProgressModel
{
	public Dictionary<string, int> SuccessfulTrials { get; set; } = new Dictionary<string, int>();

	public int RightSpeechSuccessCount { get; set; }

	public int WheelSuccessCount { get; set; }

	public int WheelHighestDifficultyUnlocked { get; set; } = 1;

	public int RightSpeechCatalogVersion { get; set; }

	public string? RightSpeechCatalogRevision { get; set; }

	public HashSet<string> CompletedRightSpeechQuestionIds { get; set; } = new HashSet<string>();

	public HashSet<int> WheelLotusRewardsClaimed { get; set; } = new HashSet<int>();
}
