using Microsoft.Xna.Framework.Input;
using Satori.Core.Models.Input;

namespace Satori.Client.Controllers;

public interface IInputController
{
	PlayerIntent GetIntent(KeyboardState keyboard, KeyboardState previousKeyboard);
}
