using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Core.Models.Input;
using Satori.Core.Models.Player;

namespace Satori.Client.Controllers;

public sealed class GameplayInputController : IInputController
{
	private readonly InputBindingService _bindingService;

	public GameplayInputController(InputBindingService bindingService)
	{
		_bindingService = bindingService;
	}

	public PlayerIntent GetIntent(KeyboardState keyboard, KeyboardState previousKeyboard)
	{
		InputBindingModel bindings = _bindingService.Bindings;
		MovementVector move = MovementKeyReader.Read(keyboard, bindings);
		bool meditateHold = KeyMapper.IsDown(keyboard, bindings.Meditate);
		bool pause = KeyMapper.WasPausePressed(keyboard, previousKeyboard, bindings);
		bool interact = KeyMapper.WasPressed(keyboard, previousKeyboard, bindings.Interact);
		return new PlayerIntent(move, meditateHold, pause, interact);
	}
}
