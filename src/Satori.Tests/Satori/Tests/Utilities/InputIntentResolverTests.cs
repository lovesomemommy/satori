using System;
using Satori.Core.Models.Input;
using Satori.Core.Utilities;
using Xunit;

namespace Satori.Tests.Utilities;

public sealed class InputIntentResolverTests
{
	[Fact]
	public void Resolve_WKeyPressed_ReturnsUpwardMovement()
	{
		InputBindingModel bindings = new InputBindingModel();
		PlayerIntent playerIntent = InputIntentResolver.Resolve((string key) => key == bindings.MoveUp, bindings, meditateHold: false, pausePressed: false, interactPressed: false);
		Assert.Equal(0f, playerIntent.Move.X);
		Assert.Equal(-1f, playerIntent.Move.Y);
	}

	[Fact]
	public void Resolve_DiagonalKeys_ReturnsNormalizedVector()
	{
		InputBindingModel bindings = new InputBindingModel();
		PlayerIntent playerIntent = InputIntentResolver.Resolve((string key) => (key == "W" || key == "D") ? true : false, bindings, meditateHold: false, pausePressed: false, interactPressed: false);
		Assert.Equal(MathF.Sqrt(2f) / 2f, playerIntent.Move.X, 3);
		Assert.Equal((0f - MathF.Sqrt(2f)) / 2f, playerIntent.Move.Y, 3);
	}

	[Fact]
	public void Resolve_PauseBinding_SetsPauseFlag()
	{
		InputBindingModel bindings = new InputBindingModel();
		Assert.True(InputIntentResolver.Resolve((string _) => false, bindings, meditateHold: false, pausePressed: true, interactPressed: false).Pause);
	}
}
