using System;
using Microsoft.Xna.Framework;

namespace Satori.Client.Views.Rendering;

public sealed class FixedViewportRenderer
{
	public int VirtualWidth { get; }

	public int VirtualHeight { get; }

	public int Scale { get; private set; } = 1;

	public Rectangle LetterboxBounds { get; private set; }

	public FixedViewportRenderer(int virtualWidth = 320, int virtualHeight = 180)
	{
		VirtualWidth = virtualWidth;
		VirtualHeight = virtualHeight;
	}

	public void UpdateForBackbuffer(int backbufferWidth, int backbufferHeight)
	{
		if (backbufferWidth > 0 && backbufferHeight > 0)
		{
			int val = backbufferWidth / VirtualWidth;
			int val2 = backbufferHeight / VirtualHeight;
			Scale = Math.Max(1, Math.Min(val, val2));
			int num = VirtualWidth * Scale;
			int num2 = VirtualHeight * Scale;
			int x = (backbufferWidth - num) / 2;
			int y = (backbufferHeight - num2) / 2;
			LetterboxBounds = new Rectangle(x, y, num, num2);
		}
	}

	public Matrix GetTransformMatrix()
	{
		return Matrix.CreateScale(Scale) * Matrix.CreateTranslation(LetterboxBounds.X, LetterboxBounds.Y, 0f);
	}
}
