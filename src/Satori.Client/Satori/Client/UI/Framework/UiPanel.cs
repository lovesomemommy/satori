using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.UI;

namespace Satori.Client.UI.Framework;

public sealed class UiPanel : UiElement
{
	public List<UiElement> Children { get; } = [];

	public Color BackgroundColor { get; set; } = UiPalette.Panel;

	public override void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
	{
		if (!IsVisible)
		{
			return;
		}

		foreach (UiElement child in Children)
		{
			child.Update(gameTime, mouse, keyboard);
		}
	}

	public override void Draw(SpriteBatch spriteBatch, Texture2D pixel, TextRenderingService text, float glowPhase)
	{
		if (!IsVisible)
		{
			return;
		}

		if (BackgroundColor.A > 0)
		{
			spriteBatch.Draw(pixel, Bounds, BackgroundColor);
		}

		foreach (UiElement child in Children)
		{
			child.Draw(spriteBatch, pixel, text, glowPhase);
		}
	}
}
