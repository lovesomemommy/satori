using System;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Systems.PilgrimTrials;
using Satori.Core.Utilities;
using Xunit;

namespace Satori.Tests.Systems.PilgrimTrials;

public sealed class SegmentNavigationTests
{
	[Fact]
	public void IsWalkable_OutOfBounds_ReturnsFalse()
	{
		TrialSegmentDefinition trialSegmentDefinition = CreateSegment();
		Assert.False(SegmentNavigation.IsWalkable(trialSegmentDefinition, -1, 0));
		Assert.False(SegmentNavigation.IsWalkable(trialSegmentDefinition, trialSegmentDefinition.Width, 0));
	}

	[Fact]
	public void IsWalkable_WallTile_ReturnsFalse()
	{
		TrialSegmentDefinition trialSegmentDefinition = CreateSegment();
		trialSegmentDefinition.Walls.Add(new TilePoint(5, 5));
		Assert.False(SegmentNavigation.IsWalkable(trialSegmentDefinition, 5, 5));
		Assert.True(SegmentNavigation.IsWalkable(trialSegmentDefinition, 4, 5));
	}

	[Fact]
	public void IsPortal_MatchesExitCoordinates()
	{
		TrialSegmentDefinition trialSegmentDefinition = CreateSegment();
		Assert.True(SegmentNavigation.IsPortal(trialSegmentDefinition, trialSegmentDefinition.ExitPortal.X, trialSegmentDefinition.ExitPortal.Y));
		Assert.False(SegmentNavigation.IsPortal(trialSegmentDefinition, 0, 0));
	}

	[Fact]
	public void FormatCountdown_FormatsFourFortyFour()
	{
		Assert.Equal("4:44", TimeFormatting.FormatCountdown(TimeSpan.FromMinutes(4.0) + TimeSpan.FromSeconds(44.0)));
		Assert.Equal("0:00", TimeFormatting.FormatCountdown(TimeSpan.Zero));
	}

	private static TrialSegmentDefinition CreateSegment()
	{
		return new TrialSegmentDefinition
		{
			Width = 18,
			Height = 11,
			ExitPortal = new PortalPoint
			{
				X = 15,
				Y = 5
			}
		};
	}
}
