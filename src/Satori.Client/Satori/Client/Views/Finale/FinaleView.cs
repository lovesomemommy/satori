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
		const int centerX = 160;
		float pulse = 0.88f + 0.12f * MathF.Sin(glowPhase * 0.45f);

		for (int y = 24; y < 168; y++)
		{
			float depth = (y - 24) / 144f;
			float sway = MathF.Sin(glowPhase * 0.55f + y * 0.07f) * (1.5f + depth * 2f);
			int halfWidth = (int)(10f + depth * 26f + MathF.Sin(glowPhase * 0.35f + y * 0.04f));
			byte alpha = (byte)(fadeIn * pulse * (28f + depth * 72f));
			var color = new Color(
				(byte)255,
				(byte)(188 + depth * 36),
				(byte)(204 + depth * 24),
				alpha);
			spriteBatch.Draw(pixel, new Rectangle(centerX - halfWidth + (int)sway, y, halfWidth * 2, 1), color);
		}

		for (int sparkle = 0; sparkle < 16; sparkle++)
		{
			float phase = glowPhase * 1.15f + sparkle * 0.75f;
			float shimmer = 0.35f + 0.65f * MathF.Sin(phase * 2.1f);
			if (shimmer < 0.45f)
			{
				continue;
			}

			int sparkleY = 32 + sparkle * 8 + (int)(MathF.Sin(phase) * 2f);
			int sparkleX = centerX + (int)(MathF.Sin(phase * 0.9f) * (6 + sparkle % 5));
			byte sparkleAlpha = (byte)(fadeIn * shimmer * 140f);
			spriteBatch.Draw(pixel, new Rectangle(sparkleX, sparkleY, 1, 1), new Color((byte)255, (byte)236, (byte)244, sparkleAlpha));
		}

		for (int x = 0; x < 320; x++)
		{
			float wave = MathF.Sin(glowPhase * 0.65f + x * 0.05f);
			int surfaceY = 156 + (int)(wave * 1.2f);
			byte surfaceAlpha = (byte)(fadeIn * (70f + 35f * wave));
			spriteBatch.Draw(pixel, new Rectangle(x, surfaceY, 1, 1), new Color((byte)255, (byte)196, (byte)214, surfaceAlpha));
		}

		spriteBatch.Draw(pixel, new Rectangle(0, 160, 320, 20), new Color((byte)255, (byte)168, (byte)190, (byte)(fadeIn * 120f)));
	}
}
