using Microsoft.Xna.Framework.Input;
using Satori.Client.Views.Rendering;

namespace Satori.Tests.Rendering;

public sealed class DisplaySettingsApplierTests
{
	[Fact]
	public void WasEdgePressed_TriggersOnlyOnRisingEdge()
	{
		Assert.True(DisplaySettingsApplier.WasEdgePressed(isDown: true, wasDown: false));
		Assert.False(DisplaySettingsApplier.WasEdgePressed(isDown: true, wasDown: true));
		Assert.False(DisplaySettingsApplier.WasEdgePressed(isDown: false, wasDown: false));
	}

	[Fact]
	public void IsAltEnterDown_RequiresAltAndEnter()
	{
		Assert.True(DisplaySettingsApplier.IsAltEnterDown(new KeyboardState(Keys.LeftAlt, Keys.Enter)));
		Assert.False(DisplaySettingsApplier.IsAltEnterDown(new KeyboardState(Keys.Enter)));
		Assert.False(DisplaySettingsApplier.IsAltEnterDown(new KeyboardState(Keys.LeftAlt)));
	}
}
