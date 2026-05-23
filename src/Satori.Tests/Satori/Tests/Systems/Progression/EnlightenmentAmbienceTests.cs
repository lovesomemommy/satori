using Satori.Core.Models.Progression;
using Satori.Core.Systems.Progression;

namespace Satori.Tests.Systems.Progression;

public sealed class EnlightenmentAmbienceTests
{
	[Fact]
	public void IsNight_ReturnsFalseForFreshMetaState()
	{
		Assert.False(EnlightenmentSystem.IsNight(new PlayerMetaState()));
	}

	[Fact]
	public void IsNight_ReturnsTrueWhenPilgrimageCompleted()
	{
		var meta = new PlayerMetaState { PilgrimageCompleted = true };
		Assert.True(EnlightenmentSystem.IsNight(meta));
	}

	[Fact]
	public void IsNight_ReturnsTrueWhenEnlightenmentCrossesThreshold()
	{
		var meta = new PlayerMetaState { Enlightenment = EnlightenmentSystem.NightEnlightenmentThreshold };
		Assert.True(EnlightenmentSystem.IsNight(meta));
	}
}
