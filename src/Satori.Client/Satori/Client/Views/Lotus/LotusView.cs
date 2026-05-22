using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.Views.Rendering;

namespace Satori.Client.Views.Lotus;

public static class LotusView
{
	private const float MapScale = 1.38f;

	public static readonly Color PetalColor = new Color(190, 110, 150);

	public static readonly Color PetalShadowColor = new Color(140, 75, 110);

	public static readonly Color CenterColor = new Color(240, 210, 120);

	public static readonly Color InnerGlowColor = new Color(255, 235, 200);

	public static void Draw(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		Rectangle bounds,
		Texture2D? lotusSprite = null,
		float scale = 1f)
	{
		if (lotusSprite != null)
		{
			var drawBounds = Math.Abs(scale - 1f) < 0.001f ? bounds : SpriteDrawHelper.ExpandCentered(bounds, scale);
			SpriteDrawHelper.DrawContained(spriteBatch, lotusSprite, drawBounds, Color.White);
			return;
		}

		DrawProcedural(spriteBatch, pixel, bounds);
	}

	public static void DrawOnMap(SpriteBatch spriteBatch, Texture2D pixel, Rectangle tileBounds, Texture2D? lotusSprite) =>
		Draw(spriteBatch, pixel, tileBounds, lotusSprite, MapScale);

	public static void DrawInnerContentFrame(SpriteBatch spriteBatch, Texture2D pixel, Rectangle lotusBounds)
	{
		Rectangle destinationRectangle = new Rectangle(lotusBounds.X + lotusBounds.Width / 6, lotusBounds.Y + lotusBounds.Height / 5, lotusBounds.Width * 2 / 3, lotusBounds.Height * 3 / 5);
		spriteBatch.Draw(pixel, destinationRectangle, InnerGlowColor);
		Rectangle destinationRectangle2 = new Rectangle(destinationRectangle.X + 4, destinationRectangle.Y + 4, destinationRectangle.Width - 8, destinationRectangle.Height / 2);
		spriteBatch.Draw(pixel, destinationRectangle2, new Color(50, 70, 90));
	}

	private static void DrawProcedural(SpriteBatch spriteBatch, Texture2D pixel, Rectangle bounds)
	{
		int num = bounds.X + bounds.Width / 2;
		int num2 = bounds.Y + bounds.Height / 2;
		int num3 = Math.Max(4, bounds.Width / 3);
		int num4 = Math.Max(6, bounds.Height / 2);
		DrawPetal(spriteBatch, pixel, num - num3 / 2, num2 - num4, num3, num4, PetalShadowColor);
		DrawPetal(spriteBatch, pixel, num - num3, num2 - num4 / 2, num3, num4, PetalColor);
		DrawPetal(spriteBatch, pixel, num, num2 - num4 / 2, num3, num4, PetalColor);
		DrawPetal(spriteBatch, pixel, num - num3 / 2, num2, num3, num4, PetalColor);
		int num5 = Math.Max(4, bounds.Width / 4);
		Rectangle destinationRectangle = new Rectangle(num - num5 / 2, num2 - num5 / 2, num5, num5);
		spriteBatch.Draw(pixel, destinationRectangle, CenterColor);
	}

	private static void DrawPetal(SpriteBatch spriteBatch, Texture2D pixel, int x, int y, int width, int height, Color color)
	{
		spriteBatch.Draw(pixel, new Rectangle(x, y, width, height), color);
	}
}
