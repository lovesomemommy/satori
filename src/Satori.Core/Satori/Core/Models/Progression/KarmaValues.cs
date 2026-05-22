using Satori.Core.Models.Lotus;

namespace Satori.Core.Models.Progression;

public static class KarmaValues
{
	public const int PreceptViolationPenalty = 3;

	public const int RightSpeechSuccessReward = 2;

	public const int RightSpeechFailPenalty = 2;

	public const int WheelSuccessReward = 3;

	public const int WheelFailPenalty = 1;

	public static int ForLotus(LotusType type)
	{
		if (1 == 0)
		{
		}
		int result = type switch
		{
			LotusType.Common => 1, 
			LotusType.Rare => 3, 
			LotusType.Golden => 5, 
			LotusType.Spiritual => 8, 
			_ => 0, 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
