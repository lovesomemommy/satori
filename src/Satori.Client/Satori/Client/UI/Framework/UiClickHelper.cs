using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Satori.Client.UI.Framework;

internal static class UiClickHelper
{
	public static bool TryHandleClick(Rectangle bounds, Point mousePoint, ButtonState leftButton, ref bool pointerDown)
	{
		var hovered = bounds.Contains(mousePoint);
		if (hovered && leftButton == ButtonState.Pressed)
		{
			pointerDown = true;
		}

		if (!pointerDown || leftButton != ButtonState.Released)
		{
			return false;
		}

		pointerDown = false;
		return hovered;
	}
}
