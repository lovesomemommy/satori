using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.UI;

namespace Satori.Client.UI.Framework;

public sealed class UiButton : UiElement
{
	private bool _pointerDownOnButton;

	public string Text { get; set; } = string.Empty;

	public Action? OnClick { get; set; }

	public bool WrapText { get; set; }

	public bool AlignLeft { get; set; }

	public bool UseCompactFont { get; set; }

	public float FillAlpha { get; set; } = 1f;

	public int LineHeight { get; set; } = 9;

	public bool IsHovered { get; private set; }

	public bool IsPressed { get; private set; }

	public void ResetPointerState()
	{
		_pointerDownOnButton = false;
	}

	public override void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
	{
		if (!IsVisible)
		{
			_pointerDownOnButton = false;
			return;
		}

		var point = new Point(mouse.X, mouse.Y);
		IsHovered = Bounds.Contains(point);
		IsPressed = IsHovered && mouse.LeftButton == ButtonState.Pressed;
		if (UiClickHelper.TryHandleClick(Bounds, point, mouse.LeftButton, ref _pointerDownOnButton))
		{
			OnClick?.Invoke();
		}
	}

	public override void Draw(SpriteBatch spriteBatch, Texture2D pixel, TextRenderingService text, float glowPhase)
	{
		if (!IsVisible)
		{
			return;
		}

		var fill = IsHovered ? UiPalette.ButtonHover : UiPalette.ButtonFill;
		if (FillAlpha < 0.999f)
		{
			fill = new Color(fill.R, fill.G, fill.B, (byte)(fill.A * FillAlpha));
		}

		var border = IsHovered ? UiPalette.PinkSoft : UiPalette.ButtonBorder;
		if (FillAlpha < 0.999f)
		{
			border = new Color(border.R, border.G, border.B, (byte)(border.A * FillAlpha));
		}

		spriteBatch.Draw(pixel, Bounds, fill);
		DrawBorder(spriteBatch, pixel, Bounds, border, 1);

		if (string.IsNullOrEmpty(Text))
		{
			return;
		}

		var labelColor = IsHovered ? UiPalette.TextPrimary : UiPalette.TextSecondary;
		const int padX = 4;
		const int padY = 2;
		var inner = new Rectangle(Bounds.X + padX, Bounds.Y + padY, Bounds.Width - padX * 2, Bounds.Height - padY * 2);
		var textSize = text.MeasureText(Text, compact: UseCompactFont);
		bool shouldWrap = WrapText || textSize.X > inner.Width;
		if (shouldWrap)
		{
			text.DrawWrappedText(spriteBatch, Text, inner, labelColor, LineHeight, UseCompactFont);
			return;
		}

		float textX = AlignLeft
			? inner.X
			: Bounds.X + (Bounds.Width - textSize.X) * 0.5f;
		float textY = Bounds.Y + (Bounds.Height - textSize.Y) * 0.5f;
		text.DrawText(spriteBatch, Text, new Vector2(textX, textY), labelColor, compact: UseCompactFont);
	}

	private static void DrawBorder(SpriteBatch spriteBatch, Texture2D pixel, Rectangle bounds, Color color, int thickness)
	{
		spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, thickness), color);
		spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Bottom - thickness, bounds.Width, thickness), color);
		spriteBatch.Draw(pixel, new Rectangle(bounds.X, bounds.Y, thickness, bounds.Height), color);
		spriteBatch.Draw(pixel, new Rectangle(bounds.Right - thickness, bounds.Y, thickness, bounds.Height), color);
	}
}
