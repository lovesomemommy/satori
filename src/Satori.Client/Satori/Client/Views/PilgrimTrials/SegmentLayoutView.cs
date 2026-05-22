using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.Content;
using Satori.Client.Services.PilgrimTrials;
using Satori.Client.UI;
using Satori.Client.Views.Lotus;
using Satori.Client.Views.Rendering;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.PilgrimTrials;

namespace Satori.Client.Views.PilgrimTrials;

public static class SegmentLayoutView
{
    public static void Draw(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        ObstacleSpriteCatalog obstacleSprites,
        PilgrimSpriteCatalog pilgrimSprites,
        TrialSegmentDefinition segment,
        double totalSeconds,
        IReadOnlySet<int>? collectedLotusIds = null)
    {
        DrawFloor(spriteBatch, pixel, segment);
        DrawWalls(spriteBatch, pixel, pilgrimSprites, segment);
        DrawDecoys(spriteBatch, pixel, pilgrimSprites, segment, totalSeconds);
        DrawObstacles(spriteBatch, pixel, obstacleSprites, segment, (float)totalSeconds);
        DrawPortals(spriteBatch, pixel, pilgrimSprites, segment);
        DrawLotuses(spriteBatch, pixel, pilgrimSprites, segment, collectedLotusIds);
    }

    public static void DrawPortalTile(SpriteBatch spriteBatch, Texture2D pixel, PilgrimSpriteCatalog pilgrimSprites, int tileX, int tileY)
    {
        var rect = TileRect(tileX, tileY);
        if (pilgrimSprites.PortalTile != null)
        {
            SpriteDrawHelper.DrawStretched(spriteBatch, pilgrimSprites.PortalTile, rect, Color.White);
            return;
        }

        spriteBatch.Draw(pixel, rect, UiPalette.PinkDim);
        spriteBatch.Draw(pixel, Inset(rect, 2), UiPalette.PinkSoft);
    }

    private static void DrawFloor(SpriteBatch spriteBatch, Texture2D pixel, TrialSegmentDefinition segment)
    {
        _ = segment;
        spriteBatch.Draw(pixel, MapBounds(), Color.Black);
    }

    private static void DrawWalls(SpriteBatch spriteBatch, Texture2D pixel, PilgrimSpriteCatalog pilgrimSprites, TrialSegmentDefinition segment)
    {
        foreach (var wall in segment.Walls)
        {
            var rect = TileRect(wall.X, wall.Y);
            if (pilgrimSprites.WallTile != null)
            {
                SpriteDrawHelper.DrawStretched(spriteBatch, pilgrimSprites.WallTile, rect, Color.White);
                continue;
            }

            spriteBatch.Draw(pixel, rect, new Color(52, 44, 60));
        }
    }

    private static void DrawPortals(SpriteBatch spriteBatch, Texture2D pixel, PilgrimSpriteCatalog pilgrimSprites, TrialSegmentDefinition segment)
    {
        DrawPortalTile(spriteBatch, pixel, pilgrimSprites, segment.ExitPortal.X, segment.ExitPortal.Y);

        foreach (var trap in segment.Traps)
        {
            DrawPortalTile(spriteBatch, pixel, pilgrimSprites, trap.Tile.X, trap.Tile.Y);
        }
    }

    private static void DrawLotuses(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        PilgrimSpriteCatalog pilgrimSprites,
        TrialSegmentDefinition segment,
        IReadOnlySet<int>? collectedLotusIds)
    {
        foreach (var lotus in segment.Lotuses)
        {
            if (collectedLotusIds?.Contains(lotus.Id) == true)
            {
                continue;
            }

            LotusView.DrawOnMap(spriteBatch, pixel, TileRect(lotus.TileX, lotus.TileY), pilgrimSprites.Lotus);
        }
    }

    private static void DrawObstacles(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        ObstacleSpriteCatalog obstacleSprites,
        TrialSegmentDefinition segment,
        float totalSeconds)
    {
        ObstacleView.Draw(spriteBatch, pixel, obstacleSprites, segment, totalSeconds);
    }

    private static void DrawDecoys(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        PilgrimSpriteCatalog pilgrimSprites,
        TrialSegmentDefinition segment,
        double totalSeconds)
    {
        DecoyTrailView.Draw(spriteBatch, pixel, pilgrimSprites.Footprint, segment, totalSeconds);
    }

    public static Rectangle TileRect(int tileX, int tileY)
    {
        const int tileSize = 15;
        return new Rectangle(
            PilgrimageViewConstants.MapOffsetX + tileX * tileSize,
            PilgrimageViewConstants.MapOffsetY + tileY * tileSize,
            tileSize,
            tileSize);
    }

    public static Rectangle MapBounds() =>
        new(
            PilgrimageViewConstants.MapOffsetX,
            PilgrimageViewConstants.MapOffsetY,
            PilgrimageViewConstants.MapPixelWidth,
            PilgrimageViewConstants.MapPixelHeight);

    private static Rectangle Inset(Rectangle rect, int amount) =>
        new(rect.X + amount, rect.Y + amount, rect.Width - amount * 2, rect.Height - amount * 2);
}
