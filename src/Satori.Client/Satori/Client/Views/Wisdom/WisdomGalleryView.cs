using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.UI;
using Satori.Core.Systems.Progression;

namespace Satori.Client.Views.Wisdom;

public static class WisdomGalleryView
{
	public const int TotalSlots = WinConditionSystem.RequiredQuoteCount;

	public const int Columns = 5;

	public const int Rows = 2;

	public const int CellGap = 4;

	public const int Top = 28;

	public const int BottomControlsY = 158;

	public static readonly string[] SlotQuoteIds =
	[
		"quote.lotus.01",
		"quote.lotus.02",
		"quote.lotus.03",
		"quote.lotus.04",
		"quote.lotus.05",
		"quote.lotus.16",
		"quote.lotus.17",
		"quote.lotus.18",
		"quote.lotus.19",
		"quote.lotus.20"
	];

	public static int CellSize
	{
		get
		{
			int byWidth = (320 - 24 - (Columns - 1) * CellGap) / Columns;
			int availableHeight = BottomControlsY - Top - (Rows - 1) * CellGap - 8;
			int byHeight = availableHeight / Rows;
			return Math.Min(byWidth, byHeight);
		}
	}

	public static Rectangle GetSlotBounds(int slotIndex)
	{
		int row = slotIndex / Columns;
		int column = slotIndex % Columns;
		int gridWidth = Columns * CellSize + (Columns - 1) * CellGap;
		int gridHeight = Rows * CellSize + (Rows - 1) * CellGap;
		int startX = 12 + (296 - gridWidth) / 2;
		int startY = Top + Math.Max(0, (BottomControlsY - Top - gridHeight - 8) / 2);
		return new Rectangle(
			startX + column * (CellSize + CellGap),
			startY + row * (CellSize + CellGap),
			CellSize,
			CellSize);
	}

	public static Rectangle GetImageBounds(Rectangle cellBounds)
	{
		const int pad = 2;
		int size = cellBounds.Width - pad * 2;
		return new Rectangle(cellBounds.X + pad, cellBounds.Y + pad, size, size);
	}

	public static void DrawSquareImage(SpriteBatch spriteBatch, Texture2D image, Rectangle bounds)
	{
		int sourceSize = Math.Min(image.Width, image.Height);
		var source = new Rectangle(
			(image.Width - sourceSize) / 2,
			(image.Height - sourceSize) / 2,
			sourceSize,
			sourceSize);
		spriteBatch.Draw(image, bounds, source, Color.White);
	}

	public static void DrawSlotFrame(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		Rectangle cellBounds,
		bool isUnlocked,
		bool isSelected)
	{
		if (!isUnlocked)
		{
			spriteBatch.Draw(pixel, cellBounds, new Color(18, 20, 24, 180));
			return;
		}

		var fill = isSelected ? UiPalette.ButtonHover : UiPalette.PanelDark;
		spriteBatch.Draw(pixel, cellBounds, fill);
		if (isSelected)
		{
			var border = UiPalette.PinkSoft;
			DrawBorder(spriteBatch, pixel, cellBounds, border);
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
