using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.UI;

namespace Satori.Client.UI.Framework;

public sealed class UiPanel : UiElement
{
	public List<IUiElement> Children { get; } = new List<IUiElement>();

	public Color BackgroundColor { get; set; } = UiPalette.Panel;

	public override void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
	{
		if (!base.IsVisible)
		{
			return;
		}
		foreach (IUiElement child in Children)
		{
			child.Update(gameTime, mouse, keyboard);
		}
	}

	public override void Draw(SpriteBatch spriteBatch, Texture2D pixel, TextRenderingService text, float glowPhase)
	{
		if (!base.IsVisible)
		{
			return;
		}

		spriteBatch.Draw(pixel, base.Bounds, BackgroundColor);
		foreach (IUiElement child in Children)
		{
			child.Draw(spriteBatch, pixel, text, glowPhase);
		}
	}
}
