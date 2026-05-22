using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.UI;
using Satori.Client.Views.Rendering;
using Satori.Core.Models.PilgrimTrials;

namespace Satori.Client.Views.PilgrimTrials;

public static class DecoyTrailView
{
	private const float StepDurationSeconds = 0.32f;

	private enum TrailDirection
	{
		North,
		South,
		East,
		West
	}

	public static void Draw(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		Texture2D? footprintSprite,
		TrialSegmentDefinition segment,
		double totalSeconds)
	{
		float time = (float)totalSeconds;
		foreach (var decoyTrail in segment.DecoyTrails)
		{
			if (decoyTrail.Path.Count == 0)
			{
				continue;
			}

			var headIndex = (int)(totalSeconds / StepDurationSeconds) % decoyTrail.Path.Count;
			for (var i = 0; i < decoyTrail.Path.Count; i++)
			{
				var tilePoint = decoyTrail.Path[i];
				if (IsTrapTile(segment, tilePoint.X, tilePoint.Y))
				{
					continue;
				}

				var direction = ResolveDirection(decoyTrail.Path, i);
				var tile = SegmentLayoutView.TileRect(tilePoint.X, tilePoint.Y);
				float bob = MathF.Sin(time * 4f + i * 0.9f) * 1.2f;
				tile.Offset(0, (int)MathF.Round(bob));
				if (footprintSprite != null)
				{
					DrawFootprintSprite(
						spriteBatch,
						footprintSprite,
						tile,
						direction,
						isLeftFoot: i % 2 == 0,
						isHead: i == headIndex);
				}
				else
				{
					DrawBootPrint(
						spriteBatch,
						pixel,
						tile,
						direction,
						isLeftFoot: i % 2 == 0,
						isHead: i == headIndex,
						time,
						i);
				}
			}
		}
	}

	private static bool IsTrapTile(TrialSegmentDefinition segment, int tileX, int tileY) =>
		segment.Traps.Any(trap => trap.Tile.X == tileX && trap.Tile.Y == tileY);

	private static TrailDirection ResolveDirection(IReadOnlyList<TilePoint> path, int index)
	{
		if (path.Count == 1)
		{
			return TrailDirection.North;
		}

		if (index < path.Count - 1)
		{
			return GetDirection(path[index], path[index + 1]);
		}

		return GetDirection(path[index - 1], path[index]);
	}

	private static TrailDirection GetDirection(TilePoint from, TilePoint to)
	{
		if (to.X > from.X)
		{
			return TrailDirection.East;
		}

		if (to.X < from.X)
		{
			return TrailDirection.West;
		}

		return to.Y > from.Y ? TrailDirection.South : TrailDirection.North;
	}

	private static void DrawFootprintSprite(
		SpriteBatch spriteBatch,
		Texture2D footprintSprite,
		Rectangle tile,
		TrailDirection direction,
		bool isLeftFoot,
		bool isHead)
	{
		float rotation = direction switch
		{
			TrailDirection.North => 0f,
			TrailDirection.South => MathF.PI,
			TrailDirection.East => MathF.PI / 2f,
			_ => -MathF.PI / 2f
		};
		var effects = isLeftFoot ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		var tint = isHead ? new Color(245, 210, 220) : new Color(150, 145, 138);
		var destination = new Rectangle(tile.X + 1, tile.Y + 1, tile.Width - 2, tile.Height - 2);
		var origin = new Vector2(footprintSprite.Width / 2f, footprintSprite.Height / 2f);
		var position = new Vector2(destination.X + destination.Width / 2f, destination.Y + destination.Height / 2f);
		float scale = Math.Min(destination.Width / (float)footprintSprite.Width, destination.Height / (float)footprintSprite.Height);
		spriteBatch.Draw(
			footprintSprite,
			position,
			null,
			tint,
			rotation,
			origin,
			scale,
			effects,
			0f);
	}

	private static void DrawBootPrint(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		Rectangle tile,
		TrailDirection direction,
		bool isLeftFoot,
		bool isHead,
		float time,
		int index)
	{
		Color sole = isHead
			? UiPalette.PinkSoft
			: new Color(150, 145, 138);
		Color upper = isHead
			? UiPalette.PinkDim
			: new Color(108, 102, 96);
		Color lace = isHead ? UiPalette.Pink : new Color(180, 175, 168);
		var centerX = tile.X + tile.Width / 2 + (isLeftFoot ? -2 : 2);
		var centerY = tile.Y + tile.Height / 2 + 1;

		var (forwardX, forwardY, lateralX, lateralY) = direction switch
		{
			TrailDirection.North => (0, -1, -1, 0),
			TrailDirection.South => (0, 1, 1, 0),
			TrailDirection.East => (1, 0, 0, -1),
			_ => (-1, 0, 0, 1)
		};

		DrawBootShape(spriteBatch, pixel, centerX, centerY, forwardX, forwardY, lateralX, lateralY, sole, upper, lace);
		if (isHead)
		{
			int sparkX = centerX + (int)(MathF.Sin(time * 5f + index) * 2f);
			int sparkY = centerY + forwardY * -5;
			spriteBatch.Draw(pixel, new Rectangle(sparkX, sparkY, 2, 2), UiPalette.Pink);
		}
	}

	private static void DrawBootShape(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		int centerX,
		int centerY,
		int forwardX,
		int forwardY,
		int lateralX,
		int lateralY,
		Color sole,
		Color upper,
		Color lace)
	{
		for (int f = -1; f <= 3; f++)
		{
			for (int l = -2; l <= 2; l++)
			{
				int x = centerX + forwardX * f + lateralX * l;
				int y = centerY + forwardY * f + lateralY * l;
				if (f <= 0)
				{
					spriteBatch.Draw(pixel, new Rectangle(x, y, 1, 1), sole);
				}
				else if (f <= 2)
				{
					spriteBatch.Draw(pixel, new Rectangle(x, y, 1, 1), upper);
				}
			}
		}

		for (int l = -1; l <= 1; l++)
		{
			int x = centerX + forwardX * 3 + lateralX * l;
			int y = centerY + forwardY * 3 + lateralY * l;
			spriteBatch.Draw(pixel, new Rectangle(x, y, 1, 1), upper);
		}

		for (int f = 1; f <= 2; f++)
		{
			int x = centerX + forwardX * f + lateralX * 0;
			int y = centerY + forwardY * f + lateralY * 0;
			spriteBatch.Draw(pixel, new Rectangle(x, y, 1, 1), lace);
		}

		for (int l = -1; l <= 1; l++)
		{
			int x = centerX + forwardX * -2 + lateralX * l;
			int y = centerY + forwardY * -2 + lateralY * l;
			spriteBatch.Draw(pixel, new Rectangle(x, y, 1, 1), sole);
		}
	}
}
