using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.UI;
using Satori.Core.Utilities;

namespace Satori.Client.Views.PilgrimTrials;

public static class TrialTimerView
{
	public const int TimerWidth = 42;

	public const int HudHeight = 12;

	public const int HudY = 4;

	public const int HudRightMargin = 6;

	public const int PauseGap = 2;

	public const int PauseButtonSize = 12;

	public static Rectangle GetTimerBounds(int virtualWidth) =>
		new Rectangle(virtualWidth - TimerWidth - HudRightMargin, HudY, TimerWidth, HudHeight);

	public static Rectangle GetPauseButtonBounds(int virtualWidth)
	{
		var timer = GetTimerBounds(virtualWidth);
		return new Rectangle(timer.X - PauseGap - PauseButtonSize, HudY, PauseButtonSize, PauseButtonSize);
	}

	public static void Draw(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		TextRenderingService text,
		TimeSpan remaining,
		bool isLow,
		int virtualWidth,
		bool pauseHovered = false)
	{
		var rectangle = GetTimerBounds(virtualWidth);
		var background = isLow ? new Color(90, 40, 40, 220) : UiPalette.PanelDark;
		var foreground = isLow ? new Color(240, 120, 100) : UiPalette.PinkSoft;

		DrawPauseButton(spriteBatch, pixel, GetPauseButtonBounds(virtualWidth), foreground, pauseHovered);

		var label = TimeFormatting.FormatCountdown(remaining);
		spriteBatch.Draw(pixel, rectangle, background);
		DrawBorder(spriteBatch, pixel, rectangle, foreground);
		var textSize = text.MeasureText(label);
		var textPosition = new Vector2(
			rectangle.X + (rectangle.Width - textSize.X) * 0.5f,
			rectangle.Y + (rectangle.Height - textSize.Y) * 0.5f);
		text.DrawText(spriteBatch, label, textPosition, foreground);

		var progressSeconds = Math.Clamp((int)Math.Ceiling(remaining.TotalSeconds), 0, 284);
		var progressWidth = (int)(rectangle.Width * (progressSeconds / 284f));
		spriteBatch.Draw(
			pixel,
			new Rectangle(rectangle.X + 1, rectangle.Y + rectangle.Height - 2, progressWidth, 1),
			foreground);
	}

	private static void DrawPauseButton(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		Rectangle bounds,
		Color foreground,
		bool hovered)
	{
		var fill = hovered ? UiPalette.ButtonHover : UiPalette.PanelDark;
		spriteBatch.Draw(pixel, bounds, fill);
		DrawBorder(spriteBatch, pixel, bounds, foreground);

		var barWidth = 2;
		var barHeight = 6;
		var barY = bounds.Y + (bounds.Height - barHeight) / 2;
		var leftBarX = bounds.X + 3;
		var rightBarX = bounds.Right - 3 - barWidth;
		spriteBatch.Draw(pixel, new Rectangle(leftBarX, barY, barWidth, barHeight), foreground);
		spriteBatch.Draw(pixel, new Rectangle(rightBarX, barY, barWidth, barHeight), foreground);
	}

	private static void DrawBorder(SpriteBatch spriteBatch, Texture2D pixel, Rectangle rect, Color color)
	{
		spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, 1), color);
		spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1), color);
		spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, 1, rect.Height), color);
		spriteBatch.Draw(pixel, new Rectangle(rect.Right - 1, rect.Y, 1, rect.Height), color);
	}
}
