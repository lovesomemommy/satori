using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.UI;

namespace Satori.Client.UI.Framework;

public sealed class UiLabel : UiElement
{
	public string Text { get; set; } = string.Empty;

	public Color Color { get; set; } = UiPalette.TextPrimary;

	public bool WrapText { get; set; }

	public bool UseTitleFont { get; set; }

	public bool CenterHorizontally { get; set; }

	public int LineHeight { get; set; } = 11;

	public override void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
	{
	}

	public override void Draw(SpriteBatch spriteBatch, Texture2D pixel, TextRenderingService text, float glowPhase)
	{
		if (!IsVisible || string.IsNullOrEmpty(Text))
		{
			return;
		}

		if (WrapText)
		{
			text.DrawWrappedText(spriteBatch, Text, Bounds, Color, LineHeight);
		}
		else if (CenterHorizontally)
		{
			var textSize = text.MeasureText(Text, title: UseTitleFont);
			float x = Bounds.X + (Bounds.Width - textSize.X) * 0.5f;
			text.DrawText(spriteBatch, Text, new Vector2(x, Bounds.Y), Color, title: UseTitleFont);
		}
		else
		{
			text.DrawText(spriteBatch, Text, new Vector2(Bounds.X, Bounds.Y), Color, title: UseTitleFont);
		}
	}
}
