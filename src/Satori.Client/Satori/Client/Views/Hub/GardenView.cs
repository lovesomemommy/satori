using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.UI;
using Satori.Client.Views.Lotus;
using Satori.Client.Views.Rendering;
using Satori.Core.Models.Progression;
using Satori.Core.Systems.Progression;

namespace Satori.Client.Views.Hub;

public static class GardenView
{
	public const int SlotSize = 18;

	public const int SlotGap = 6;

	public const int Columns = 5;

	private const int BackgroundPadding = 10;

	private const int FloorTileSize = 12;

	public static Point GetCenteredOrigin(int virtualWidth)
	{
		int width = Columns * SlotSize + (Columns - 1) * SlotGap;
		return new Point((virtualWidth - width) / 2, 56);
	}

	public static Rectangle GetGardenBounds(Point origin)
	{
		int rows = (GardenPlantingSystem.MaxSlots + Columns - 1) / Columns;
		int width = Columns * SlotSize + (Columns - 1) * SlotGap;
		int height = rows * SlotSize + (rows - 1) * SlotGap;
		return new Rectangle(origin.X, origin.Y, width, height);
	}

	public static Rectangle GetBackgroundBounds(Point origin)
	{
		var gardenBounds = GetGardenBounds(origin);
		return new Rectangle(
			gardenBounds.X - BackgroundPadding,
			gardenBounds.Y - BackgroundPadding,
			gardenBounds.Width + BackgroundPadding * 2,
			gardenBounds.Height + BackgroundPadding * 2);
	}

	public static void Draw(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		PlayerMetaState meta,
		float glowPhase,
		Point origin,
		Texture2D? gardenFloor,
		Texture2D? lotusSprite,
		Texture2D? wallTile = null)
	{
		var backgroundBounds = GetBackgroundBounds(origin);
		if (gardenFloor != null)
		{
			DrawTiledFloor(spriteBatch, gardenFloor, backgroundBounds);
		}
		else
		{
			spriteBatch.Draw(pixel, backgroundBounds, UiPalette.Panel);
		}

		DrawFence(spriteBatch, pixel, wallTile, backgroundBounds);

		for (int slotIndex = 0; slotIndex < GardenPlantingSystem.MaxSlots; slotIndex++)
		{
			var slotBounds = GetSlotBounds(slotIndex, origin);
			var planted = FindPlantedSlot(meta, slotIndex);

			if (planted == null)
			{
				continue;
			}

			if (lotusSprite != null)
			{
				LotusView.Draw(spriteBatch, pixel, slotBounds, lotusSprite, 1.25f);
			}
			else
			{
				var lotusColor = UiPalette.PinkDim;
				var pulse = 0.85f + 0.15f * MathF.Sin(glowPhase + slotIndex * 0.7f);
				var inner = new Rectangle(slotBounds.X + 4, slotBounds.Y + 3, slotBounds.Width - 8, slotBounds.Height - 6);
				spriteBatch.Draw(
					pixel,
					inner,
					new Color(
						(byte)(lotusColor.R * pulse),
						(byte)(lotusColor.G * pulse),
						(byte)(lotusColor.B * pulse)));
			}

		}
	}

	public static Rectangle GetSlotBounds(int slotIndex, Point origin)
	{
		int row = slotIndex / Columns;
		int column = slotIndex % Columns;
		int x = origin.X + column * (SlotSize + SlotGap);
		int y = origin.Y + row * (SlotSize + SlotGap);
		return new Rectangle(x, y, SlotSize, SlotSize);
	}

	private static void DrawFence(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		Texture2D? wallTile,
		Rectangle bounds)
	{
		int tile = FloorTileSize;
		int outerLeft = bounds.X - tile;
		int outerRight = bounds.Right;
		int outerTop = bounds.Y - tile;
		int outerBottom = bounds.Bottom;

		for (int x = outerLeft; x <= outerRight; x += tile)
		{
			DrawFenceTile(spriteBatch, pixel, wallTile, new Rectangle(x, outerTop, tile, tile));
			DrawFenceTile(spriteBatch, pixel, wallTile, new Rectangle(x, outerBottom, tile, tile));
		}

		for (int y = bounds.Y; y < bounds.Bottom; y += tile)
		{
			int height = Math.Min(tile, bounds.Bottom - y);
			DrawFenceTile(spriteBatch, pixel, wallTile, new Rectangle(outerLeft, y, tile, height));
			DrawFenceTile(spriteBatch, pixel, wallTile, new Rectangle(outerRight, y, tile, height));
		}
	}

	private static void DrawFenceTile(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		Texture2D? wallTile,
		Rectangle rect)
	{
		if (wallTile != null)
		{
			SpriteDrawHelper.DrawStretched(spriteBatch, wallTile, rect, Color.White);
			return;
		}

		spriteBatch.Draw(pixel, rect, new Color(52, 44, 60));
	}

	private static void DrawTiledFloor(SpriteBatch spriteBatch, Texture2D floor, Rectangle bounds)
	{
		for (int y = bounds.Y; y < bounds.Bottom; y += FloorTileSize)
		{
			int tileHeight = Math.Min(FloorTileSize, bounds.Bottom - y);
			for (int x = bounds.X; x < bounds.Right; x += FloorTileSize)
			{
				int tileWidth = Math.Min(FloorTileSize, bounds.Right - x);
				SpriteDrawHelper.DrawStretched(spriteBatch, floor, new Rectangle(x, y, tileWidth, tileHeight), Color.White);
			}
		}
	}

	private static GardenSlotModel? FindPlantedSlot(PlayerMetaState meta, int slotIndex) =>
		meta.PlantedLotuses.Find(slot => slot.SlotIndex == slotIndex);

}
