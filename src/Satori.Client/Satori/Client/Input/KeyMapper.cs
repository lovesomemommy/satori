using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Satori.Core.Models.Input;

namespace Satori.Client.Input;

public static class KeyMapper
{
	private static readonly Dictionary<string, Keys> LayoutAliases = new Dictionary<string, Keys>(StringComparer.OrdinalIgnoreCase)
	{
		["Ц"] = Keys.W,
		["Ф"] = Keys.A,
		["Ы"] = Keys.S,
		["В"] = Keys.D,
		["У"] = Keys.E,
		["ц"] = Keys.W,
		["ф"] = Keys.A,
		["ы"] = Keys.S,
		["в"] = Keys.D,
		["у"] = Keys.E
	};

	public static bool IsDown(KeyboardState keyboard, string keyName)
	{
		if (!TryToKey(keyName, out var key))
		{
			return false;
		}
		if (keyboard.IsKeyDown(key))
		{
			return true;
		}
		Keys[] pressedKeys = keyboard.GetPressedKeys();
		foreach (Keys keys in pressedKeys)
		{
			if (keys == key)
			{
				return true;
			}
		}
		return false;
	}

	public static bool WasPressed(KeyboardState current, KeyboardState previous, string keyName)
	{
		if (!TryToKey(keyName, out var key))
		{
			return false;
		}
		return current.IsKeyDown(key) && !previous.IsKeyDown(key);
	}

	public static bool WasPausePressed(KeyboardState current, KeyboardState previous, InputBindingModel bindings)
	{
		return WasPressed(current, previous, bindings.Pause) || WasPressed(current, previous, "Escape");
	}

	public static bool TryToKey(string keyName, out Keys key)
	{
		if (LayoutAliases.TryGetValue(keyName, out key))
		{
			return true;
		}
		return Enum.TryParse<Keys>(keyName, ignoreCase: true, out key);
	}

	public static string ToDisplayName(Keys key)
	{
		return key.ToString();
	}

	public static bool TryFromKey(Keys key, out string keyName)
	{
		keyName = key.ToString();
		return true;
	}

	public static string FormatBindingLabel(string keyName)
	{
		if (string.Equals(keyName, "Escape", StringComparison.OrdinalIgnoreCase))
		{
			return "Esc";
		}

		if (string.Equals(keyName, "Space", StringComparison.OrdinalIgnoreCase))
		{
			return "Space";
		}

		return keyName;
	}
}
