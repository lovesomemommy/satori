using Satori.Core.Models.Player;

namespace Satori.Core.Models.Input;

public readonly record struct PlayerIntent(MovementVector Move, bool MeditateHold, bool Pause, bool Interact)
{
	public static PlayerIntent Empty => new PlayerIntent(MovementVector.Zero, MeditateHold: false, Pause: false, Interact: false);
}
