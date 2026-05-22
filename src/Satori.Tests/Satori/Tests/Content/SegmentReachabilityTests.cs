using Satori.Client.Content;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Systems.PilgrimTrials;

namespace Satori.Tests.Content;

public sealed class SegmentReachabilityTests
{
    private const int MainCorridorY = 5;

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void PilgrimageSegments_HaveNoSealedFloorTiles(int segmentIndex)
    {
        var segment = PilgrimageContentFactory.Create().Segments[segmentIndex];
        var reachable = GetReachableTiles(segment);

        for (var y = 0; y < segment.Height; y++)
        {
            for (var x = 0; x < segment.Width; x++)
            {
                if (!SegmentNavigation.IsWalkable(segment, x, y))
                {
                    continue;
                }

                Assert.True(
                    reachable.Contains((x, y)),
                    $"Segment {segmentIndex} has unreachable floor at ({x},{y}).");
            }
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void PilgrimageLotuses_AreReachableFromSpawn(int segmentIndex)
    {
        var segment = PilgrimageContentFactory.Create().Segments[segmentIndex];
        var reachable = GetReachableTiles(segment);

        foreach (var lotus in segment.Lotuses)
        {
            Assert.True(
                reachable.Contains((lotus.TileX, lotus.TileY)),
                $"Lotus {lotus.Id} at ({lotus.TileX},{lotus.TileY}) is unreachable.");
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void PilgrimageSegments_DoNotHaveTrivialMainCorridor(int segmentIndex)
    {
        var segment = PilgrimageContentFactory.Create().Segments[segmentIndex];
        var wallCount = 0;

        for (var x = segment.Spawn.X + 1; x < segment.ExitPortal.X; x++)
        {
            if (!SegmentNavigation.IsWalkable(segment, x, MainCorridorY))
            {
                wallCount++;
            }
        }

        Assert.True(
            wallCount >= 4,
            $"Segment {segmentIndex} main corridor is too open ({wallCount} internal walls on row 5).");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void PilgrimageLotuses_AreReachableWithoutSteppingOnObstacles(int segmentIndex)
    {
        var segment = PilgrimageContentFactory.Create().Segments[segmentIndex];
        var blockedTiles = segment.Obstacles
            .Select(o => (o.Tile.X, o.Tile.Y))
            .ToHashSet();

        foreach (var lotus in segment.Lotuses)
        {
            Assert.True(
                HasPathAvoidingTiles(
                    segment,
                    (segment.Spawn.X, segment.Spawn.Y),
                    (lotus.TileX, lotus.TileY),
                    blockedTiles),
                $"Lotus {lotus.Id} at ({lotus.TileX},{lotus.TileY}) has no path that avoids obstacles.");
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void PilgrimageLotuses_AreReachableWithoutCrossingExitPortal(int segmentIndex)
    {
        var segment = PilgrimageContentFactory.Create().Segments[segmentIndex];
        var portal = (segment.ExitPortal.X, segment.ExitPortal.Y);

        foreach (var lotus in segment.Lotuses)
        {
            Assert.True(
                HasPathAvoidingTiles(
                    segment,
                    (segment.Spawn.X, segment.Spawn.Y),
                    (lotus.TileX, lotus.TileY),
                    [portal]),
                $"Lotus {lotus.Id} at ({lotus.TileX},{lotus.TileY}) requires crossing the exit portal.");
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void PilgrimageSegments_HavePathFromSpawnToPortalAvoidingObstacles(int segmentIndex)
    {
        var segment = PilgrimageContentFactory.Create().Segments[segmentIndex];
        var blockedTiles = segment.Obstacles
            .Select(obstacle => (obstacle.Tile.X, obstacle.Tile.Y))
            .ToHashSet();

        Assert.True(
            HasPathAvoidingTiles(
                segment,
                (segment.Spawn.X, segment.Spawn.Y),
                (segment.ExitPortal.X, segment.ExitPortal.Y),
                blockedTiles),
            $"Segment {segmentIndex} has no path from spawn to portal that avoids obstacles.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void PilgrimageObstacles_DoNotBlockMainCorridor(int segmentIndex)
    {
        var segment = PilgrimageContentFactory.Create().Segments[segmentIndex];

        foreach (var obstacle in segment.Obstacles)
        {
            Assert.False(
                obstacle.Tile.Y == MainCorridorY
                && obstacle.Tile.X >= segment.Spawn.X
                && obstacle.Tile.X <= segment.ExitPortal.X,
                $"{obstacle.Type} blocks the main corridor at ({obstacle.Tile.X},{obstacle.Tile.Y}).");
        }
    }

    private static bool HasPathAvoidingTiles(
        TrialSegmentDefinition segment,
        (int X, int Y) start,
        (int X, int Y) goal,
        HashSet<(int X, int Y)> blockedTiles)
    {
        var visited = new HashSet<(int X, int Y)>();
        var queue = new Queue<(int X, int Y)>();
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            if (x == goal.X && y == goal.Y)
            {
                return true;
            }

            TryEnqueueAvoiding(segment, x + 1, y, blockedTiles, visited, queue);
            TryEnqueueAvoiding(segment, x - 1, y, blockedTiles, visited, queue);
            TryEnqueueAvoiding(segment, x, y + 1, blockedTiles, visited, queue);
            TryEnqueueAvoiding(segment, x, y - 1, blockedTiles, visited, queue);
        }

        return false;
    }

    private static void TryEnqueueAvoiding(
        TrialSegmentDefinition segment,
        int x,
        int y,
        HashSet<(int X, int Y)> blockedTiles,
        HashSet<(int X, int Y)> visited,
        Queue<(int X, int Y)> queue)
    {
        if (blockedTiles.Contains((x, y)))
        {
            return;
        }

        TryEnqueue(segment, x, y, visited, queue);
    }

    private static HashSet<(int X, int Y)> GetReachableTiles(TrialSegmentDefinition segment)
    {
        var visited = new HashSet<(int X, int Y)>();
        var queue = new Queue<(int X, int Y)>();
        queue.Enqueue((segment.Spawn.X, segment.Spawn.Y));
        visited.Add((segment.Spawn.X, segment.Spawn.Y));

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            TryEnqueue(segment, x + 1, y, visited, queue);
            TryEnqueue(segment, x - 1, y, visited, queue);
            TryEnqueue(segment, x, y + 1, visited, queue);
            TryEnqueue(segment, x, y - 1, visited, queue);
        }

        return visited;
    }

    private static void TryEnqueue(
        TrialSegmentDefinition segment,
        int x,
        int y,
        HashSet<(int X, int Y)> visited,
        Queue<(int X, int Y)> queue)
    {
        if (!SegmentNavigation.IsWalkable(segment, x, y))
        {
            return;
        }

        if (!visited.Add((x, y)))
        {
            return;
        }

        queue.Enqueue((x, y));
    }
}
