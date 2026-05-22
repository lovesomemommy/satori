using Microsoft.Xna.Framework.Input;
using Satori.Core.Models.Input;
using Satori.Core.Models.Player;
using Satori.Core.Utilities;

namespace Satori.Client.Input;

public static class MovementKeyReader
{
	private static readonly Keys[] UpKeys = new Keys[4]
	{
		Keys.W,
		Keys.Up,
		Keys.E,
		Keys.NumPad8
	};

	private static readonly Keys[] DownKeys = new Keys[3]
	{
		Keys.S,
		Keys.Down,
		Keys.NumPad2
	};

	private static readonly Keys[] LeftKeys = new Keys[3]
	{
		Keys.A,
		Keys.Left,
		Keys.NumPad4
	};

	private static readonly Keys[] RightKeys = new Keys[3]
	{
		Keys.D,
		Keys.Right,
		Keys.NumPad6
	};

	public static MovementVector Read(KeyboardState keyboard, InputBindingModel bindings)
	{
		float num = 0f;
		float num2 = 0f;
		if (IsUpActive(keyboard, bindings.MoveUp))
		{
			num2 -= 1f;
		}
		if (IsDownActive(keyboard, bindings.MoveDown))
		{
			num2 += 1f;
		}
		if (IsLeftActive(keyboard, bindings.MoveLeft))
		{
			num -= 1f;
		}
		if (IsRightActive(keyboard, bindings.MoveRight))
		{
			num += 1f;
		}
		return InputIntentResolver.NormalizeDiagonal(new MovementVector(num, num2));
	}

	private static bool IsUpActive(KeyboardState keyboard, string binding)
	{
		return IsDirectionActive(keyboard, binding, UpKeys) || SdlNative.IsUpDown();
	}

	private static bool IsDownActive(KeyboardState keyboard, string binding)
	{
		return IsDirectionActive(keyboard, binding, DownKeys) || SdlNative.IsDownDown();
	}

	private static bool IsLeftActive(KeyboardState keyboard, string binding)
	{
		return IsDirectionActive(keyboard, binding, LeftKeys) || SdlNative.IsLeftDown();
	}

	private static bool IsRightActive(KeyboardState keyboard, string binding)
	{
		return IsDirectionActive(keyboard, binding, RightKeys) || SdlNative.IsRightDown();
	}

	private static bool IsDirectionActive(KeyboardState keyboard, string binding, Keys[] physicalKeys)
	{
		if (KeyMapper.IsDown(keyboard, binding))
		{
			return true;
		}
		foreach (Keys key in physicalKeys)
		{
			if (keyboard.IsKeyDown(key))
			{
				return true;
			}
		}
		Keys[] pressedKeys = keyboard.GetPressedKeys();
		foreach (Keys keys in pressedKeys)
		{
			foreach (Keys keys2 in physicalKeys)
			{
				if (keys == keys2)
				{
					return true;
				}
			}
		}
		return false;
	}
}
