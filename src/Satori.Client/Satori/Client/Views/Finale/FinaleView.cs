using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.UI;

namespace Satori.Client.Views.Finale;

public static class FinaleView
{
	public static void Draw(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		TextRenderingService text,
		float elapsedSeconds,
		float glowPhase,
		string title,
		string subtitle,
		string quote)
	{
		float fadeIn = Math.Clamp(elapsedSeconds / 2f, 0f, 1f);
		float titleAlpha = Math.Clamp((elapsedSeconds - 1.5f) / 1.5f, 0f, 1f);
		float quoteAlpha = Math.Clamp((elapsedSeconds - 3.5f) / 2f, 0f, 1f);
		DrawRiverLight(spriteBatch, pixel, glowPhase, fadeIn);
		var textPanel = new Rectangle(24, 44, 272, 108);
		spriteBatch.Draw(pixel, textPanel, UiPalette.PanelDark * fadeIn);
		if (titleAlpha > 0f)
		{
			text.DrawText(spriteBatch, title, new Vector2(96, 52), UiPalette.TextPrimary * titleAlpha, title: true);
			text.DrawText(spriteBatch, subtitle, new Vector2(108, 72), UiPalette.Pink * titleAlpha);
		}

		if (quoteAlpha > 0f)
		{
			text.DrawWrappedText(spriteBatch, quote, new Rectangle(36, 92, 248, 52), UiPalette.TextPrimary * quoteAlpha, 12);
		}
	}

	private static void DrawRiverLight(SpriteBatch spriteBatch, Texture2D pixel, float glowPhase, float fadeIn)
	{
		float pulse = 0.75f + 0.25f * MathF.Sin(glowPhase);
		int centerX = 160;
		for (int layer = 0; layer < 4; layer++)
		{
			int width = 12 + layer * 10;
			int height = 140 + layer * 8;
			int x = centerX - width / 2;
			int y = 24 - layer * 2;
			byte alpha = (byte)(fadeIn * pulse * (70 - layer * 12));
			spriteBatch.Draw(pixel, new Rectangle(x, y, width, height), new Color((byte)200, (byte)180, (byte)190, alpha));
		}

		spriteBatch.Draw(pixel, new Rectangle(0, 150, 320, 30), new Color((byte)28, (byte)28, (byte)32, (byte)(fadeIn * 180)));
		spriteBatch.Draw(pixel, new Rectangle(0, 168, 320, 12), new Color((byte)48, (byte)48, (byte)54, (byte)(fadeIn * 220)));
	}
}
