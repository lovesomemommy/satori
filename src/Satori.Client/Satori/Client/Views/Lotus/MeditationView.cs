using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.UI;
using Satori.Client.Views.PilgrimTrials;
using Satori.Core.Models.Minigames;

namespace Satori.Client.Views.Lotus;

public static class MeditationView
{
	public static void DrawLotusGlow(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		Texture2D? lotusSprite,
		int tileX,
		int tileY,
		float glowStrength)
	{
		if (glowStrength <= 0f || lotusSprite == null)
		{
			return;
		}

		var tile = SegmentLayoutView.TileRect(tileX, tileY);
		LotusView.DrawOnMap(spriteBatch, pixel, tile, lotusSprite);
	}

	public static void DrawMonkBreath(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		Texture2D? monkWalking,
		Texture2D? monkMeditating,
		int tileX,
		int tileY,
		float breathScale,
		MeditationPhase phase)
	{
		if (phase is MeditationPhase.Idle or MeditationPhase.Interrupted)
		{
			MonkView.DrawWalking(spriteBatch, pixel, monkWalking, tileX, tileY);
			return;
		}

		MonkView.DrawMeditating(spriteBatch, pixel, monkMeditating, tileX, tileY, breathScale);
	}
}
