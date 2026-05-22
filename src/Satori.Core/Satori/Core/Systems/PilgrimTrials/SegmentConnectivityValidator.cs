using Satori.Core.Models.PilgrimTrials;

namespace Satori.Core.Systems.PilgrimTrials;

/// <summary>
/// Проверяет, что старт и портал сегмента достижимы и связаны проходимым путём.
/// </summary>
public static class SegmentConnectivityValidator
{
    public static bool HasPathFromSpawnToPortal(TrialSegmentDefinition segment)
    {
        var spawnX = segment.Spawn.X;
        var spawnY = segment.Spawn.Y;
        var portalX = segment.ExitPortal.X;
        var portalY = segment.ExitPortal.Y;

        if (!SegmentNavigation.IsWalkable(segment, spawnX, spawnY)
            || !SegmentNavigation.IsWalkable(segment, portalX, portalY))
        {
            return false;
        }

        var visited = new HashSet<(int X, int Y)>();
        var queue = new Queue<(int X, int Y)>();
        queue.Enqueue((spawnX, spawnY));
        visited.Add((spawnX, spawnY));

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            if (x == portalX && y == portalY)
            {
                return true;
            }

            TryEnqueue(segment, x + 1, y, visited, queue);
            TryEnqueue(segment, x - 1, y, visited, queue);
            TryEnqueue(segment, x, y + 1, visited, queue);
            TryEnqueue(segment, x, y - 1, visited, queue);
        }

        return false;
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
