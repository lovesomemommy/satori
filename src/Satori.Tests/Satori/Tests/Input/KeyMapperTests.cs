using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Core.Models.Input;

namespace Satori.Tests.Input;

public sealed class KeyMapperTests
{
	[Theory]
	[InlineData("Ц", Keys.W)]
	[InlineData("ф", Keys.A)]
	[InlineData("Space", Keys.Space)]
	public void TryToKey_ResolvesLayoutAliasesAndStandardKeys(string keyName, Keys expected)
	{
		Assert.True(KeyMapper.TryToKey(keyName, out Keys key));
		Assert.Equal(expected, key);
	}

	[Fact]
	public void WasPressed_DetectsKeyDownEdge()
	{
		var previous = new KeyboardState(Keys.W);
		var current = new KeyboardState(Keys.W, Keys.E);

		Assert.True(KeyMapper.WasPressed(current, previous, "E"));
		Assert.False(KeyMapper.WasPressed(current, previous, "W"));
	}

	[Fact]
	public void WasPausePressed_AcceptsBindingOrEscape()
	{
		var bindings = new InputBindingModel { Pause = "P" };
		var previous = default(KeyboardState);
		var current = new KeyboardState(Keys.Escape);

		Assert.True(KeyMapper.WasPausePressed(current, previous, bindings));
	}

	[Theory]
	[InlineData("Escape", "Esc")]
	[InlineData("Space", "Space")]
	[InlineData("W", "W")]
	public void FormatBindingLabel_FormatsKnownKeys(string keyName, string expected)
	{
		Assert.Equal(expected, KeyMapper.FormatBindingLabel(keyName));
	}
}
