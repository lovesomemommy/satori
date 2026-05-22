namespace Satori.Core.Models.Player;

public readonly record struct MovementVector(float X, float Y)
{
	public static MovementVector Zero => new MovementVector(0f, 0f);

	public float LengthSquared => X * X + Y * Y;

	public bool IsZero => LengthSquared <= float.Epsilon;
}
