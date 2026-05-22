using Satori.Core.Models.Progression;

namespace Satori.Core.Systems.Progression;

public static class HubAmbienceSystem
{
	public const float NightEnlightenmentThreshold = 0.35f;

	public static bool IsNight(PlayerMetaState meta)
	{
		return meta.PilgrimageCompleted || meta.Enlightenment >= NightEnlightenmentThreshold;
	}
}
