using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.Views.Rendering;

namespace Satori.Client.Views.PilgrimTrials;

public static class MonkView
{
	public static void DrawWalking(SpriteBatch spriteBatch, Texture2D pixel, Texture2D? monkSprite, int tileX, int tileY)
	{
		var tile = SegmentLayoutView.TileRect(tileX, tileY);
		if (monkSprite != null)
		{
			SpriteDrawHelper.DrawContainedBottomScaled(spriteBatch, monkSprite, tile, 1.55f, 2.35f, Color.White);
			return;
		}

		DrawProcedural(spriteBatch, pixel, tile);
	}

	public static void DrawMeditating(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		Texture2D? monkSprite,
		int tileX,
		int tileY,
		float breathScale)
	{
		var tile = SegmentLayoutView.TileRect(tileX, tileY);
		if (monkSprite != null)
		{
			float breath = Math.Clamp(breathScale, 0.88f, 1.12f);
			SpriteDrawHelper.DrawContainedBottomScaled(spriteBatch, monkSprite, tile, 1.65f, 2.75f * breath, Color.White);
			return;
		}

		DrawProceduralBreath(spriteBatch, pixel, tile, breathScale);
	}

	private static void DrawProcedural(SpriteBatch spriteBatch, Texture2D pixel, Rectangle tile)
	{
		var body = new Rectangle(tile.X + 3, tile.Y + 2, tile.Width - 6, tile.Height - 4);
		spriteBatch.Draw(pixel, body, new Color(230, 210, 150));
		spriteBatch.Draw(pixel, new Rectangle(body.X + 2, body.Y - 2, body.Width - 4, 4), new Color(200, 60, 60));
	}

	private static void DrawProceduralBreath(SpriteBatch spriteBatch, Texture2D pixel, Rectangle tile, float breathScale)
	{
		int centerX = tile.X + tile.Width / 2;
		int centerY = tile.Y + tile.Height / 2;
		int size = (int)(tile.Width * breathScale);
		var body = new Rectangle(centerX - size / 2, centerY - size / 2, size, size);
		spriteBatch.Draw(pixel, body, new Color(230, 210, 150));
		spriteBatch.Draw(pixel, new Rectangle(body.X + 2, body.Y - 3, body.Width - 4, 4), new Color(200, 60, 60));
	}
}
