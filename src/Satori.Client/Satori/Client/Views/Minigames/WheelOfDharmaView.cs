using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.UI;
using Satori.Core.Models.Minigames;

namespace Satori.Client.Views.Minigames;

public static class WheelOfDharmaView
{
	private const int ButtonSize = 24;

	private const int PadSize = 48;

	private const int ArmGap = 4;

	private static readonly Point PadOrigin = new(136, 80);

	public static Rectangle PadCenter => new(PadOrigin.X, PadOrigin.Y, PadSize, PadSize);

	public static Rectangle GetSequenceBarBounds()
	{
		var pad = PadCenter;
		const int barHeight = 8;
		const int bottomInset = 3;
		int width = pad.Width - 8;
		int x = pad.X + (pad.Width - width) / 2;
		int y = pad.Bottom - bottomInset - barHeight;
		return new Rectangle(x, y, width, barHeight);
	}

	public static Rectangle GetDirectionButtonBounds(WheelDirection direction)
	{
		var pad = PadCenter;
		int centeredX = pad.X + (PadSize - ButtonSize) / 2;
		int centeredY = pad.Y + (PadSize - ButtonSize) / 2;
		return direction switch
		{
			WheelDirection.Up => new Rectangle(centeredX, pad.Y - ButtonSize - ArmGap, ButtonSize, ButtonSize),
			WheelDirection.Down => new Rectangle(centeredX, pad.Bottom + ArmGap, ButtonSize, ButtonSize),
			WheelDirection.Left => new Rectangle(pad.X - ButtonSize - ArmGap, centeredY, ButtonSize, ButtonSize),
			_ => new Rectangle(pad.Right + ArmGap, centeredY, ButtonSize, ButtonSize)
		};
	}

	public static void DrawPad(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		WheelOfDharmaState state,
		float glowPhase,
		TextRenderingService? text = null,
		string? headlineLabel = null)
	{
		spriteBatch.Draw(pixel, PadCenter, UiPalette.PanelDark);
		DrawDirectionButton(spriteBatch, pixel, WheelDirection.Up, state, glowPhase, state.Phase == WheelOfDharmaPhase.Showing);
		DrawDirectionButton(spriteBatch, pixel, WheelDirection.Down, state, glowPhase, state.Phase == WheelOfDharmaPhase.Showing);
		DrawDirectionButton(spriteBatch, pixel, WheelDirection.Left, state, glowPhase, state.Phase == WheelOfDharmaPhase.Showing);
		DrawDirectionButton(spriteBatch, pixel, WheelDirection.Right, state, glowPhase, state.Phase == WheelOfDharmaPhase.Showing);

		if (state.Phase == WheelOfDharmaPhase.Showing && state.Sequence.Count > 0)
		{
			DrawSequenceBar(spriteBatch, pixel, state, glowPhase);
		}

		if (text != null && !string.IsNullOrEmpty(headlineLabel))
		{
			var headlineSize = text.MeasureText(headlineLabel, compact: true);
			text.DrawText(
				spriteBatch,
				headlineLabel,
				new Vector2((320 - headlineSize.X) * 0.5f, 24),
				UiPalette.PinkSoft,
				compact: true);
		}
	}

	private static void DrawSequenceBar(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		WheelOfDharmaState state,
		float glowPhase)
	{
		var sequenceBar = GetSequenceBarBounds();
		spriteBatch.Draw(pixel, sequenceBar, UiPalette.Panel);
		int count = state.Sequence.Count;
		if (count == 0)
		{
			return;
		}

		int dotGap = 4;
		int dotSize = Math.Min(6, (sequenceBar.Width - (count - 1) * dotGap) / count);
		int totalWidth = count * dotSize + (count - 1) * dotGap;
		int startX = sequenceBar.X + (sequenceBar.Width - totalWidth) / 2;
		int dotY = sequenceBar.Y + (sequenceBar.Height - dotSize) / 2;
		for (int i = 0; i < count; i++)
		{
			var dot = new Rectangle(startX + i * (dotSize + dotGap), dotY, dotSize, dotSize);
			bool completed = i < state.ShowIndex || (i == state.ShowIndex && !state.ShowHighlightActive);
			bool active = i == state.ShowIndex && state.ShowHighlightActive;
			Color fill = completed ? UiPalette.PinkSoft : UiPalette.SegmentDim;
			if (active)
			{
				float pulse = 0.55f + 0.45f * MathF.Sin(glowPhase * 0.7f);
				fill = new Color(
					(byte)Math.Clamp(UiPalette.Pink.R * pulse, 0f, 255f),
					(byte)Math.Clamp(UiPalette.Pink.G * pulse, 0f, 255f),
					(byte)Math.Clamp(UiPalette.Pink.B * pulse, 0f, 255f));
			}

			spriteBatch.Draw(pixel, dot, fill);
			spriteBatch.Draw(pixel, new Rectangle(dot.X, dot.Y, dot.Width, 1), UiPalette.Border);
			spriteBatch.Draw(pixel, new Rectangle(dot.X, dot.Bottom - 1, dot.Width, 1), UiPalette.Border);
			spriteBatch.Draw(pixel, new Rectangle(dot.X, dot.Y, 1, dot.Height), UiPalette.Border);
			spriteBatch.Draw(pixel, new Rectangle(dot.Right - 1, dot.Y, 1, dot.Height), UiPalette.Border);
		}
	}

	private static void DrawDirectionButton(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		WheelDirection direction,
		WheelOfDharmaState state,
		float glowPhase,
		bool showingPhase)
	{
		var bounds = GetDirectionButtonBounds(direction);
		bool highlighted = state.ActiveShowDirection == direction || state.ActiveInputDirection == direction;
		var fill = showingPhase && !highlighted ? UiPalette.GrayDark : UiPalette.ButtonFill;
		if (highlighted)
		{
			fill = UiPalette.Pink;
			if (state.Phase == WheelOfDharmaPhase.Showing)
			{
				float pulse = 0.65f + 0.35f * MathF.Sin(glowPhase * 0.25f);
				fill = new Color(
					(byte)Math.Clamp(UiPalette.Pink.R * pulse, 0f, 255f),
					(byte)Math.Clamp(UiPalette.Pink.G * pulse, 0f, 255f),
					(byte)Math.Clamp(UiPalette.Pink.B * pulse, 0f, 255f));
			}
		}

		spriteBatch.Draw(pixel, bounds, fill);
		spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, 1), UiPalette.Border);
		spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Bottom - 1, bounds.Width, 1), UiPalette.Border);
		spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, 1, bounds.Height), UiPalette.Border);
		spriteBatch.Draw(pixel, new Rectangle(bounds.Right - 1, bounds.Y, 1, bounds.Height), UiPalette.Border);
	}
}
