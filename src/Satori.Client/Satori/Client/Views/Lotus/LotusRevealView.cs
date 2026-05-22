using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.UI;
using Satori.Client.Views.PilgrimTrials;
using Satori.Client.Views.Rendering;

namespace Satori.Client.Views.Lotus;

public static class LotusRevealView
{
	public const float ExpandDurationSeconds = 0.22f;

	private const int TargetMarginX = 6;

	private const int TargetMarginY = 8;

	public static Rectangle GetLotusBounds(float progress, int originTileX, int originTileY, int screenWidth, int screenHeight)
	{
		Rectangle rectangle = SegmentLayoutView.TileRect(originTileX, originTileY);
		Vector2 vector = new Vector2(rectangle.X + rectangle.Width / 2f, rectangle.Y + rectangle.Height / 2f);
		Rectangle target = GetTargetLotusBounds(screenWidth, screenHeight);
		float amount = EaseOutCubic(Math.Clamp(progress, 0f, 1f));
		int width = (int)MathHelper.Lerp(rectangle.Width, target.Width, amount);
		int height = (int)MathHelper.Lerp(rectangle.Height, target.Height, amount);
		int x = (int)(MathHelper.Lerp(vector.X, target.X + target.Width / 2f, amount) - width / 2f);
		int y = (int)(MathHelper.Lerp(vector.Y, target.Y + target.Height / 2f, amount) - height / 2f);
		return new Rectangle(x, y, width, height);
	}

	public static Rectangle GetCenteredLotusBounds(float progress, int screenWidth, int screenHeight)
	{
		Rectangle target = GetTargetLotusBounds(screenWidth, screenHeight);
		float amount = EaseOutCubic(Math.Clamp(progress, 0f, 1f));
		int width = (int)MathHelper.Lerp(12, target.Width, amount);
		int height = (int)MathHelper.Lerp(12, target.Height, amount);
		int x = (int)(MathHelper.Lerp(screenWidth / 2f, target.X + target.Width / 2f, amount) - width / 2f);
		int y = (int)(MathHelper.Lerp(screenHeight / 2f, target.Y + target.Height / 2f, amount) - height / 2f);
		return new Rectangle(x, y, width, height);
	}

	public static Rectangle GetQuoteBounds(Rectangle lotusBounds)
	{
		int width = Math.Max(8, lotusBounds.Width * 46 / 100);
		int height = Math.Max(8, lotusBounds.Height * 42 / 100);
		return new Rectangle(
			lotusBounds.X + (lotusBounds.Width - width) / 2,
			lotusBounds.Y + (lotusBounds.Height - height) / 2,
			width,
			height);
	}

	public static void DrawCentered(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		TextRenderingService text,
		float progress,
		int screenWidth,
		int screenHeight,
		Texture2D? quoteImage,
		Texture2D? lotusSprite,
		string dismissHint)
	{
		byte alpha = (byte)(180f * Math.Clamp(progress * 1.5f, 0f, 1f));
		spriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), new Color((byte)8, (byte)10, (byte)16, alpha));
		DrawRevealContents(spriteBatch, pixel, text, progress, GetCenteredLotusBounds(progress, screenWidth, screenHeight), quoteImage, lotusSprite, dismissHint, screenHeight);
	}

	public static void Draw(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		TextRenderingService text,
		float progress,
		int originTileX,
		int originTileY,
		int screenWidth,
		int screenHeight,
		Texture2D? quoteImage,
		Texture2D? lotusSprite,
		string dismissHint)
	{
		byte alpha = (byte)(180f * Math.Clamp(progress * 1.5f, 0f, 1f));
		spriteBatch.Draw(pixel, new Rectangle(0, 0, screenWidth, screenHeight), new Color((byte)8, (byte)10, (byte)16, alpha));
		DrawRevealContents(
			spriteBatch,
			pixel,
			text,
			progress,
			GetLotusBounds(progress, originTileX, originTileY, screenWidth, screenHeight),
			quoteImage,
			lotusSprite,
			dismissHint,
			screenHeight);
	}

	private static void DrawRevealContents(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		TextRenderingService text,
		float progress,
		Rectangle lotusBounds,
		Texture2D? quoteImage,
		Texture2D? lotusSprite,
		string dismissHint,
		int screenHeight)
	{
		LotusView.Draw(spriteBatch, pixel, lotusBounds, lotusSprite, 1.12f);

		if (progress >= 0.7f)
		{
			var quoteBounds = GetQuoteBounds(lotusBounds);
			if (quoteImage != null)
			{
				SpriteDrawHelper.DrawContained(spriteBatch, quoteImage, quoteBounds, Color.White);
			}
			else
			{
				spriteBatch.Draw(pixel, quoteBounds, new Color(58, 72, 88));
			}
		}

		if (progress >= 1f && !string.IsNullOrWhiteSpace(dismissHint))
		{
			var hintSize = text.MeasureText(dismissHint, compact: true);
			float hintY = Math.Min(lotusBounds.Bottom + 6, screenHeight - hintSize.Y - 4);
			text.DrawText(
				spriteBatch,
				dismissHint,
				new Vector2(lotusBounds.X + (lotusBounds.Width - hintSize.X) * 0.5f, hintY),
				UiPalette.TextPrimary,
				compact: true);
		}
	}

	private static Rectangle GetTargetLotusBounds(int screenWidth, int screenHeight)
	{
		int width = screenWidth - TargetMarginX * 2;
		int height = screenHeight - TargetMarginY * 2;
		return new Rectangle((screenWidth - width) / 2, (screenHeight - height) / 2, width, height);
	}

	private static float EaseOutCubic(float t) => 1f - MathF.Pow(1f - t, 3f);
}
