using System;
using Satori.Core.Models.Input;
using Satori.Core.Models.Player;

namespace Satori.Core.Utilities;

public static class InputIntentResolver
{
	public static PlayerIntent Resolve(Func<string, bool> isKeyDown, InputBindingModel bindings, bool meditateHold, bool pausePressed, bool interactPressed)
	{
		float num = 0f;
		float num2 = 0f;
		if (isKeyDown(bindings.MoveLeft))
		{
			num -= 1f;
		}
		if (isKeyDown(bindings.MoveRight))
		{
			num += 1f;
		}
		if (isKeyDown(bindings.MoveUp))
		{
			num2 -= 1f;
		}
		if (isKeyDown(bindings.MoveDown))
		{
			num2 += 1f;
		}
		if (num != 0f && num2 != 0f)
		{
			float num3 = 1f / MathF.Sqrt(2f);
			num *= num3;
			num2 *= num3;
		}
		return new PlayerIntent(new MovementVector(num, num2), meditateHold, pausePressed, interactPressed);
	}

	public static MovementVector NormalizeDiagonal(MovementVector move)
	{
		if (move.X == 0f || move.Y == 0f)
		{
			return move;
		}
		float num = 1f / MathF.Sqrt(2f);
		return new MovementVector(move.X * num, move.Y * num);
	}
}
