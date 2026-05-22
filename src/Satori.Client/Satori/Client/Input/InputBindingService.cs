using System;
using Satori.Core.Models.Input;

namespace Satori.Client.Input;

public sealed class InputBindingService
{
	public InputBindingModel Bindings { get; private set; } = new InputBindingModel();

	public void LoadBindings(InputBindingModel bindings)
	{
		Bindings = bindings;
	}

	public void UpdateBinding(Action<InputBindingModel> configure)
	{
		configure(Bindings);
	}
}
