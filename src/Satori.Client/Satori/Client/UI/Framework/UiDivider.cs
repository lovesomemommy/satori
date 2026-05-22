using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.UI;

namespace Satori.Client.UI.Framework;

public sealed class UiDivider : UiElement
{
	public Color Color { get; set; } = UiPalette.GrayMid;

	public override void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
	{
	}

	public override void Draw(SpriteBatch spriteBatch, Texture2D pixel, TextRenderingService text, float glowPhase)
	{
		if (!IsVisible)
		{
			return;
		}

		int y = Bounds.Y + Bounds.Height / 2;
		spriteBatch.Draw(pixel, new Rectangle(Bounds.X, y, Bounds.Width, 1), Color);
	}
}
