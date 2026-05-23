using Satori.Client.Input;
using Satori.Core.Models.Input;

namespace Satori.Tests.Input;

public sealed class InputBindingServiceTests
{
	[Fact]
	public void LoadBindings_ReplacesCurrentBindings()
	{
		var service = new InputBindingService();
		var custom = new InputBindingModel
		{
			MoveUp = "I",
			MoveDown = "K",
			MoveLeft = "J",
			MoveRight = "L"
		};

		service.LoadBindings(custom);

		Assert.Equal("I", service.Bindings.MoveUp);
		Assert.Equal("K", service.Bindings.MoveDown);
	}

	[Fact]
	public void UpdateBinding_MutatesBindingsInPlace()
	{
		var service = new InputBindingService();
		service.UpdateBinding(bindings => bindings.Interact = "Q");

		Assert.Equal("Q", service.Bindings.Interact);
	}
}
