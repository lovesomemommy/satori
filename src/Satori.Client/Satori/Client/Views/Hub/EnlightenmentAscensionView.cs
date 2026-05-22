using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.UI;

namespace Satori.Client.Views.Hub;

public static class EnlightenmentAscensionView
{
	public const int SegmentCount = 8;

	private const int PanelPadding = 6;

	public static Rectangle GetBounds() =>
		new(HubLayout.EnlightenmentPanelX, HubLayout.ContentTop, 56, HubLayout.ContentHeight);

	public static void Draw(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		float enlightenment,
		float glowPhase)
	{
		var bounds = GetBounds();
		spriteBatch.Draw(pixel, bounds, new Color(32, 32, 36, 90));

		int litSegments = (int)MathF.Floor(Math.Clamp(enlightenment, 0f, 1f) * SegmentCount);
		bool isFull = litSegments >= SegmentCount;
		int innerHeight = bounds.Height - PanelPadding * 2 - 10;
		int segmentGap = 2;
		int segmentHeight = Math.Max(6, (innerHeight - (SegmentCount - 1) * segmentGap) / SegmentCount);
		int stackHeight = SegmentCount * segmentHeight + (SegmentCount - 1) * segmentGap;
		int stackTop = bounds.Y + PanelPadding + 8 + (innerHeight - stackHeight);
		int segmentWidth = bounds.Width - PanelPadding * 2;

		for (int segmentIndex = 0; segmentIndex < SegmentCount; segmentIndex++)
		{
			int visualIndex = SegmentCount - 1 - segmentIndex;
			bool isLit = visualIndex < litSegments;
			var segmentBounds = new Rectangle(
				bounds.X + PanelPadding,
				stackTop + segmentIndex * (segmentHeight + segmentGap),
				segmentWidth,
				segmentHeight);
			Color baseColor = isLit ? UiPalette.SegmentLit : UiPalette.SegmentDim;
			if (isLit && isFull)
			{
				float shimmer = 0.82f + 0.18f * MathF.Sin(glowPhase * 0.55f + visualIndex * 0.45f);
				baseColor = new Color(
					(byte)Math.Clamp(UiPalette.Pink.R * shimmer, 0f, 255f),
					(byte)Math.Clamp(UiPalette.Pink.G * shimmer, 0f, 255f),
					(byte)Math.Clamp(UiPalette.Pink.B * shimmer, 0f, 255f));
			}

			spriteBatch.Draw(pixel, segmentBounds, baseColor);
			Color border = isLit && isFull ? UiPalette.PinkSoft : isLit ? UiPalette.GrayLight : UiPalette.GrayDark;
			DrawBorder(spriteBatch, pixel, segmentBounds, border);
		}
	}

	private static void DrawBorder(SpriteBatch spriteBatch, Texture2D pixel, Rectangle rect, Color color)
	{
		spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, 1), color);
		spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - 1, rect.Width, 1), color);
		spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, 1, rect.Height), color);
		spriteBatch.Draw(pixel, new Rectangle(rect.Right - 1, rect.Y, 1, rect.Height), color);
	}
}
