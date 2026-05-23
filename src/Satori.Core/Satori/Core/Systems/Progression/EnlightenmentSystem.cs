using System;
using Satori.Core.Models.Progression;

namespace Satori.Core.Systems.Progression;

public static class EnlightenmentSystem
{
	public const float PerPlantedLotus = 0.08f;

	public const float MaxKarmaContribution = 0.3f;

	public const float PilgrimageCompleteBonus = 0.2f;

	public const int KarmaForFullContribution = 200;

	public const float NightEnlightenmentThreshold = 0.35f;

	public static void Recalculate(PlayerMetaState meta)
	{
		float fromLotuses = meta.PlantedLotuses.Count * PerPlantedLotus;
		float fromKarma = Math.Min(MaxKarmaContribution, meta.Karma / (float)KarmaForFullContribution);
		float fromCompletion = meta.PilgrimageCompleted ? PilgrimageCompleteBonus : 0f;
		meta.Enlightenment = Math.Clamp(fromLotuses + fromKarma + fromCompletion, 0f, 1f);
	}

	public static bool IsNight(PlayerMetaState meta) =>
		meta.PilgrimageCompleted || meta.Enlightenment >= NightEnlightenmentThreshold;
}
