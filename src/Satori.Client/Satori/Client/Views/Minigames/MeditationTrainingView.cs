using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.UI;
using Satori.Client.Views.PilgrimTrials;
using Satori.Client.Views.Rendering;
using Satori.Core.Models.Minigames;
using Satori.Core.Systems.Minigames;

namespace Satori.Client.Views.Minigames;

public static class MeditationTrainingView
{
	public static void Draw(
		SpriteBatch spriteBatch,
		Texture2D pixel,
		MeditationTrainingSystem training,
		float glowPhase,
		Texture2D? monkMeditating)
	{
		var center = new Point(160, 96);
		float glowStrength = training.GetGlowStrength();
		float breathScale = training.GetBreathScale();
		var body = new Rectangle(center.X - 14, center.Y - 14, 28, 28);

		if (glowStrength > 0f)
		{
			int glowPad = (int)(6f * glowStrength);
			var glowRect = new Rectangle(body.X - glowPad, body.Y - glowPad, body.Width + glowPad * 2, body.Height + glowPad * 2);
			byte alpha = (byte)(60f + 120f * glowStrength);
			spriteBatch.Draw(pixel, glowRect, UiPalette.PinkSoft * (alpha / 255f));
		}

		if (monkMeditating != null)
		{
			int scaledHeight = Math.Max(28, (int)(56f * Math.Clamp(breathScale, 0.88f, 1.12f)));
			var destination = new Rectangle(center.X - 24, center.Y + 28 - scaledHeight, 48, scaledHeight);
			SpriteDrawHelper.DrawContainedBottomScaled(spriteBatch, monkMeditating, destination, 1.4f, 1f, Color.White);
		}
		else
		{
			int radius = (int)(28f * breathScale);
			body = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);
			var bodyColor = training.State.Phase == MeditationPhase.Hold
				? UiPalette.Pink
				: UiPalette.PinkDim;
			spriteBatch.Draw(pixel, body, bodyColor);
			spriteBatch.Draw(pixel, new Rectangle(body.X + 4, body.Y - 4, body.Width - 8, 4), UiPalette.GrayMid);
		}

		float pulse = 0.35f + 0.25f * MathF.Sin(glowPhase);
		var pulseColor = new Color(
			(byte)Math.Clamp(UiPalette.Pink.R * pulse, 0f, 255f),
			(byte)Math.Clamp(UiPalette.Pink.G * pulse, 0f, 255f),
			(byte)Math.Clamp(UiPalette.Pink.B * pulse, 0f, 255f));
		spriteBatch.Draw(pixel, new Rectangle(center.X - 40, center.Y + 40, 80, 2), pulseColor);
	}
}
