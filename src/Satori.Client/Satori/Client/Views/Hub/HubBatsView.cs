using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.UI;

namespace Satori.Client.Views.Hub;

public static class HubBatsView
{
	private const int BatCount = 5;

	public static void Draw(SpriteBatch spriteBatch, Texture2D pixel, float timeSeconds, int virtualWidth)
	{
		for (int batIndex = 0; batIndex < BatCount; batIndex++)
		{
			float cycle = timeSeconds * (0.18f + batIndex * 0.03f) + batIndex * 1.7f;
			float normalized = cycle - MathF.Floor(cycle);
			float x = normalized * (virtualWidth + 24f) - 12f;
			float y = 18f + 14f * MathF.Sin(cycle * MathF.Tau);
			float wing = MathF.Sin(timeSeconds * (6f + batIndex) + batIndex);
			DrawBat(spriteBatch, pixel, (int)x, (int)y, wing);
		}
	}

	private static void DrawBat(SpriteBatch spriteBatch, Texture2D pixel, int x, int y, float wingPhase)
	{
		var body = new Color(210, 210, 220, 230);
		var wing = new Color(245, 210, 220, 220);
		spriteBatch.Draw(pixel, new Rectangle(x, y + 1, 4, 2), body);
		int wingSpread = wingPhase > 0f ? 3 : 1;
		spriteBatch.Draw(pixel, new Rectangle(x - wingSpread, y, wingSpread, 2), wing);
		spriteBatch.Draw(pixel, new Rectangle(x + 4, y, wingSpread, 2), wing);
	}
}
