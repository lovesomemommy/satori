using Microsoft.Xna.Framework.Input;
using Satori.Core.Models.Input;

namespace Satori.Client.Controllers;

public sealed class RebindingInputController : IInputController
{
	private readonly GameplayInputController _inner;

	public RebindingInputController(GameplayInputController inner)
	{
		_inner = inner;
	}

	public PlayerIntent GetIntent(KeyboardState keyboard, KeyboardState previousKeyboard)
	{
		return _inner.GetIntent(keyboard, previousKeyboard);
	}
}
