using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Systems.PilgrimTrials;
using Satori.Tests.Helpers;
using Xunit;

namespace Satori.Tests.Systems.PilgrimTrials;

public sealed class TrapSystemTests
{
	[Fact]
	public void IsTrap_ReturnsTrueForConfiguredTile()
	{
		PilgrimPilgrimageDefinition pilgrimPilgrimageDefinition = PilgrimageTestData.CreateDefault();
		TrialSegmentDefinition segment = pilgrimPilgrimageDefinition.Segments[2];
		TrapSystem trapSystem = new TrapSystem();
		Assert.True(trapSystem.IsTrap(segment, 10, 2));
		Assert.False(trapSystem.IsTrap(segment, 1, 1));
	}
}
