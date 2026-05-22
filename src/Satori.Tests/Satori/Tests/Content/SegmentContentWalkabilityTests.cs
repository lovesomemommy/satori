using Satori.Client.Content;
using Satori.Core.Systems.PilgrimTrials;

namespace Satori.Tests.Content;

public sealed class SegmentContentWalkabilityTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void PilgrimageContent_IsPlacedOnWalkableTiles(int segmentIndex)
    {
        var segment = PilgrimageContentFactory.Create().Segments[segmentIndex];

        Assert.True(SegmentNavigation.IsWalkable(segment, segment.Spawn.X, segment.Spawn.Y));
        Assert.True(SegmentNavigation.IsWalkable(segment, segment.ExitPortal.X, segment.ExitPortal.Y));

        foreach (var lotus in segment.Lotuses)
        {
            Assert.True(
                SegmentNavigation.IsWalkable(segment, lotus.TileX, lotus.TileY),
                $"Lotus {lotus.Id} is on a wall.");
        }

        foreach (var obstacle in segment.Obstacles)
        {
            Assert.True(
                SegmentNavigation.IsWalkable(segment, obstacle.Tile.X, obstacle.Tile.Y),
                $"{obstacle.Type} is on a wall.");
        }

        foreach (var trap in segment.Traps)
        {
            Assert.True(
                SegmentNavigation.IsWalkable(segment, trap.Tile.X, trap.Tile.Y),
                "Trap door is on a wall.");
        }

        foreach (var decoyTrail in segment.DecoyTrails)
        {
            foreach (var tile in decoyTrail.Path)
            {
                Assert.True(
                    SegmentNavigation.IsWalkable(segment, tile.X, tile.Y),
                    $"Decoy trail tile ({tile.X},{tile.Y}) is on a wall.");
            }
        }
    }
}
