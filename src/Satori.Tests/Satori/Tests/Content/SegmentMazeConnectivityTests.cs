using Satori.Client.Content;
using Satori.Core.Systems.PilgrimTrials;

namespace Satori.Tests.Content;

public sealed class SegmentMazeConnectivityTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void PilgrimageSegments_HavePathFromSpawnToPortal(int segmentIndex)
    {
        var segment = PilgrimageContentFactory.Create().Segments[segmentIndex];

        Assert.True(
            SegmentConnectivityValidator.HasPathFromSpawnToPortal(segment),
            $"Segment {segmentIndex} has no walkable path from spawn to portal.");
    }
}
