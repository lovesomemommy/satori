using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Systems.PilgrimTrials;
using Satori.Tests.Helpers;
using Xunit;

namespace Satori.Tests.Systems.PilgrimTrials;

public sealed class DecoyTrailSystemTests
{
	[Fact]
	public void IsDecoyTile_DetectsConfiguredTrail()
	{
		PilgrimPilgrimageDefinition pilgrimPilgrimageDefinition = PilgrimageTestData.CreateDefault();
		TrialSegmentDefinition segment = pilgrimPilgrimageDefinition.Segments[2];
		DecoyTrailSystem decoyTrailSystem = new DecoyTrailSystem();
		Assert.True(decoyTrailSystem.IsDecoyTile(segment, 9, 5));
		Assert.True(decoyTrailSystem.IsDecoyTile(segment, 10, 2));
		Assert.False(decoyTrailSystem.IsDecoyTile(segment, 1, 1));
	}

	[Fact]
	public void LeadsToTrap_WhenDecoyAndTrapsExist_ReturnsTrue()
	{
		PilgrimPilgrimageDefinition pilgrimPilgrimageDefinition = PilgrimageTestData.CreateDefault();
		TrialSegmentDefinition segment = pilgrimPilgrimageDefinition.Segments[2];
		DecoyTrailSystem decoyTrailSystem = new DecoyTrailSystem();
		TrapSystem traps = new TrapSystem();
		Assert.True(decoyTrailSystem.LeadsToTrap(segment, 9, 5, traps));
	}
}
