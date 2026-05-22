using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Views.Rendering;

namespace Satori.Client.Input;

public static class VirtualInput
{
	public static MouseState ToVirtualMouse(FixedViewportRenderer viewport, MouseState mouse)
	{
		int num = Math.Max(1, viewport.Scale);
		Rectangle letterboxBounds = viewport.LetterboxBounds;
		int x = (mouse.X - letterboxBounds.X) / num;
		int y = (mouse.Y - letterboxBounds.Y) / num;
		return new MouseState(x, y, mouse.ScrollWheelValue, mouse.LeftButton, mouse.MiddleButton, mouse.RightButton, mouse.XButton1, mouse.XButton2);
	}
}
