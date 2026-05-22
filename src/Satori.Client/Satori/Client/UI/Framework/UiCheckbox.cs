using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.UI;

namespace Satori.Client.UI.Framework;

public sealed class UiCheckbox : UiElement
{
	private const int BoxSize = 10;

	private const int Gap = 5;

	private bool _pointerDownOnCheckbox;

	public string Label { get; set; } = string.Empty;

	public bool IsChecked { get; set; }

	public Action<bool>? OnChanged { get; set; }

	public bool IsHovered { get; private set; }

	public override void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
	{
		if (!IsVisible)
		{
			_pointerDownOnCheckbox = false;
			return;
		}

		var point = new Point(mouse.X, mouse.Y);
		IsHovered = Bounds.Contains(point);
		if (UiClickHelper.TryHandleClick(Bounds, point, mouse.LeftButton, ref _pointerDownOnCheckbox))
		{
			IsChecked = !IsChecked;
			OnChanged?.Invoke(IsChecked);
		}
	}

	public override void Draw(SpriteBatch spriteBatch, Texture2D pixel, TextRenderingService text, float glowPhase)
	{
		if (!IsVisible)
		{
			return;
		}

		int boxY = Bounds.Y + (Bounds.Height - BoxSize) / 2;
		var box = new Rectangle(Bounds.X, boxY, BoxSize, BoxSize);
		var border = IsHovered ? UiPalette.PinkSoft : UiPalette.Border;
		spriteBatch.Draw(pixel, box, UiPalette.ButtonFill);
		DrawBorder(spriteBatch, pixel, box, border, 1);
		if (IsChecked)
		{
			spriteBatch.Draw(pixel, new Rectangle(box.X + 2, box.Y + 2, box.Width - 4, box.Height - 4), UiPalette.Pink);
		}

		if (!string.IsNullOrEmpty(Label))
		{
			var labelColor = IsHovered ? UiPalette.TextPrimary : UiPalette.TextSecondary;
			text.DrawText(spriteBatch, Label, new Vector2(box.Right + Gap, Bounds.Y + 2), labelColor);
		}
	}

	private static void DrawBorder(SpriteBatch spriteBatch, Texture2D pixel, Rectangle bounds, Color color, int thickness)
	{
		spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, thickness), color);
		spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Bottom - thickness, bounds.Width, thickness), color);
		spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, thickness, bounds.Height), color);
		spriteBatch.Draw(pixel, new Rectangle(bounds.Right - thickness, bounds.Y, thickness, bounds.Height), color);
	}
}
