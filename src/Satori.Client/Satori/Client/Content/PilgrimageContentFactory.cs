using Satori.Core.Models.Lotus;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Precepts;

namespace Satori.Client.Content;

public static class PilgrimageContentFactory
{
    public static PilgrimPilgrimageDefinition Create()
    {
        var nextLotusId = 1;

        return new PilgrimPilgrimageDefinition
        {
            TitleKey = "pilgrim.title",
            Segments =
            [
                BuildSegment(
                    0,
                    SegmentFocus.NoKilling,
                    ref nextLotusId,
                    lotuses: [(7, 7), (9, 3), (4, 9)],
                    obstacles:
                    [
                        Obstacle(ObstacleType.Harm, 4, 2),
                        Obstacle(ObstacleType.Harm, 10, 2),
                        Obstacle(ObstacleType.Harm, 13, 3),
                        Obstacle(ObstacleType.Harm, 6, 3),
                        Obstacle(ObstacleType.Harm, 11, 7)
                    ]),
                BuildSegment(
                    1,
                    SegmentFocus.NoStealing,
                    ref nextLotusId,
                    lotuses: [(3, 7), (11, 3), (1, 8)],
                    obstacles:
                    [
                        Obstacle(ObstacleType.Temptation, 5, 3),
                        Obstacle(ObstacleType.Temptation, 7, 4),
                        Obstacle(ObstacleType.Temptation, 8, 9),
                        Obstacle(ObstacleType.Temptation, 13, 3),
                        Obstacle(ObstacleType.Temptation, 9, 6)
                    ]),
                BuildSegment(
                    2,
                    SegmentFocus.DecoyTrails,
                    ref nextLotusId,
                    lotuses: [(7, 7), (10, 3), (1, 7)],
                    traps: [Point(6, 2), Point(8, 2)],
                    decoyPaths:
                    [
                        [
                            Point(3, 5), Point(2, 5), Point(1, 5), Point(1, 4), Point(1, 3), Point(1, 2),
                            Point(1, 1), Point(2, 1), Point(3, 1), Point(4, 1), Point(5, 1), Point(6, 1), Point(6, 2)
                        ],
                        [
                            Point(10, 5), Point(11, 5), Point(12, 5), Point(13, 5), Point(13, 6), Point(13, 7),
                            Point(13, 8), Point(13, 9), Point(12, 9), Point(11, 9), Point(10, 9), Point(9, 9),
                            Point(8, 9), Point(7, 9), Point(6, 9), Point(5, 9), Point(4, 9), Point(3, 9),
                            Point(2, 9), Point(1, 9), Point(1, 8), Point(1, 7), Point(1, 6), Point(1, 5),
                            Point(1, 4), Point(1, 3), Point(1, 2), Point(1, 1), Point(2, 1), Point(3, 1),
                            Point(4, 1), Point(5, 1), Point(6, 1), Point(7, 1), Point(8, 1), Point(8, 2)
                        ]
                    ]),
                BuildSegment(
                    3,
                    SegmentFocus.NoIntoxication,
                    ref nextLotusId,
                    lotuses: [(5, 7), (10, 3), (13, 8)],
                    obstacles:
                    [
                        Obstacle(ObstacleType.Mist, 5, 4),
                        Obstacle(ObstacleType.Mist, 7, 3),
                        Obstacle(ObstacleType.Mist, 11, 4),
                        Obstacle(ObstacleType.Mist, 16, 1),
                        Obstacle(ObstacleType.Mist, 16, 3)
                    ]),
                BuildSegment(
                    4,
                    SegmentFocus.Celibacy,
                    ref nextLotusId,
                    lotuses: [(3, 7), (11, 3), (8, 9)],
                    obstacles:
                    [
                        Obstacle(ObstacleType.Temptation, 4, 1),
                        Obstacle(ObstacleType.Temptation, 10, 1),
                        Obstacle(ObstacleType.Temptation, 16, 1),
                        Obstacle(ObstacleType.Temptation, 3, 3),
                        Obstacle(ObstacleType.Temptation, 14, 3),
                        Obstacle(ObstacleType.Temptation, 16, 9)
                    ])
            ]
        };
    }

    private static TrialSegmentDefinition BuildSegment(
        int index,
        SegmentFocus focus,
        ref int nextLotusId,
        (int x, int y)[] lotuses,
        ObstacleModel[]? obstacles = null,
        TilePoint[]? traps = null,
        TilePoint[][]? decoyPaths = null)
    {
        var segment = new TrialSegmentDefinition
        {
            SegmentIndex = index,
            TitleKey = $"pilgrim.segment.0{index + 1}.title",
            Focus = focus,
            Width = PilgrimageViewConstants.GridWidth,
            Height = PilgrimageViewConstants.GridHeight,
            Spawn = new SpawnPoint { X = 2, Y = 5 },
            ExitPortal = new PortalPoint { X = 15, Y = 5 },
            Walls = SegmentMazeLayouts.GetWalls(index),
            Lotuses = CreateLotuses(index, lotuses, ref nextLotusId)
        };

        if (obstacles is not null)
        {
            segment.Obstacles.AddRange(obstacles);
        }

        if (traps is not null)
        {
            foreach (var trap in traps)
            {
                segment.Traps.Add(new TrapModel { Tile = trap });
            }
        }

        if (decoyPaths is not null)
        {
            foreach (var path in decoyPaths)
            {
                segment.DecoyTrails.Add(new DecoyTrailModel
                {
                    Path = path.Select(p => new TilePoint(p.X, p.Y)).ToList()
                });
            }
        }

        return segment;
    }

    private static List<LotusModel> CreateLotuses(int segmentIndex, (int x, int y)[] lotuses, ref int nextLotusId)
    {
        var models = new List<LotusModel>(lotuses.Length);
        for (int lotusIndex = 0; lotusIndex < lotuses.Length; lotusIndex++)
        {
            var lotus = lotuses[lotusIndex];
            models.Add(new LotusModel
            {
                Id = nextLotusId++,
                Type = LotusType.Common,
                SegmentIndex = segmentIndex,
                TileX = lotus.x,
                TileY = lotus.y,
                HasQuote = lotusIndex == 0
            });
        }

        return models;
    }

    private static TilePoint Point(int x, int y) => new(x, y);

    private static ObstacleModel Obstacle(ObstacleType type, int x, int y) =>
        new() { Type = type, Tile = new TilePoint(x, y) };
}
