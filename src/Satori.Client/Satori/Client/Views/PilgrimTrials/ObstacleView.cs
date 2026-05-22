using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.Services.PilgrimTrials;
using Satori.Client.Views.Rendering;
using Satori.Client.UI;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Models.Precepts;

namespace Satori.Client.Views.PilgrimTrials;

public static class ObstacleView
{
    public static void Draw(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        ObstacleSpriteCatalog sprites,
        TrialSegmentDefinition segment,
        float pulse)
    {
        foreach (var obstacle in segment.Obstacles)
        {
            var tile = SegmentLayoutView.TileRect(obstacle.Tile.X, obstacle.Tile.Y);

            switch (obstacle.Type)
            {
                case ObstacleType.Distraction:
                    DrawDisciplineTemptation(spriteBatch, pixel, tile, obstacle.Tile.X, obstacle.Tile.Y, pulse);
                    break;
                case ObstacleType.Temptation when segment.Focus == SegmentFocus.Celibacy:
                    DrawHeartTemptation(spriteBatch, pixel, tile, pulse);
                    break;
                default:
                    DrawSpriteObstacle(spriteBatch, sprites, obstacle.Type, tile);
                    break;
            }
        }
    }

    private static void DrawHeartTemptation(SpriteBatch spriteBatch, Texture2D pixel, Rectangle tile, float pulse)
    {
        const int scale = 2;
        float beat = 0.82f + 0.18f * MathF.Sin(pulse * 1.6f);
        var heart = new Color(
            (byte)Math.Clamp(220 * beat, 0f, 255f),
            (byte)Math.Clamp(70 * beat, 0f, 255f),
            (byte)Math.Clamp(95 * beat, 0f, 255f));
        var outline = new Color((byte)90, (byte)24, (byte)40);
        int cx = tile.X + tile.Width / 2;
        int cy = tile.Y + tile.Height / 2 + 1;

        ReadOnlySpan<(int dx, int dy)> shape =
        [
            (-1, -2), (1, -2),
            (-2, -1), (-1, -1), (0, -1), (1, -1), (2, -1),
            (-1, 0), (0, 0), (1, 0),
            (0, 1)
        ];

        foreach (var (dx, dy) in shape)
        {
            DrawScaledPixel(spriteBatch, pixel, cx, cy, dx, dy, scale, outline);
        }

        foreach (var (dx, dy) in shape)
        {
            if (dx == 0 && dy == 1)
            {
                continue;
            }

            DrawScaledPixel(spriteBatch, pixel, cx, cy, dx, dy - 1, scale, heart);
        }

        DrawScaledPixel(spriteBatch, pixel, cx, cy, 0, 0, scale, heart);
        DrawScaledPixel(spriteBatch, pixel, cx, cy, 0, 1, scale, heart);
    }

    private static void DrawScaledPixel(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        int centerX,
        int centerY,
        int dx,
        int dy,
        int scale,
        Color color)
    {
        spriteBatch.Draw(
            pixel,
            new Rectangle(centerX + dx * scale - scale / 2, centerY + dy * scale - scale / 2, scale, scale),
            color);
    }

    private static void DrawSpriteObstacle(
        SpriteBatch spriteBatch,
        ObstacleSpriteCatalog sprites,
        ObstacleType type,
        Rectangle tile)
    {
        var sprite = sprites.GetSprite(type);
        var inset = new Rectangle(tile.X + 1, tile.Y + 1, tile.Width - 2, tile.Height - 2);
        SpriteDrawHelper.DrawContained(spriteBatch, sprite, inset, Color.White);
    }

    private static void DrawDisciplineTemptation(
        SpriteBatch spriteBatch,
        Texture2D pixel,
        Rectangle tile,
        int tileX,
        int tileY,
        float pulse)
    {
        switch ((tileX + tileY) % 3)
        {
            case 0:
                DrawIncenseShrine(spriteBatch, pixel, tile, pulse);
                break;
            case 1:
                DrawRestBench(spriteBatch, pixel, tile, pulse);
                break;
            default:
                DrawWhisperArrow(spriteBatch, pixel, tile, pulse);
                break;
        }
    }

    private static void DrawIncenseShrine(SpriteBatch spriteBatch, Texture2D pixel, Rectangle tile, float pulse)
    {
        var glow = (byte)(150 + 40 * MathF.Sin(pulse * 1.4f));
        var stone = new Color((byte)96, (byte)92, (byte)88);
        var ash = new Color((byte)72, (byte)68, (byte)64);
        var ember = new Color((byte)220, (byte)140, (byte)70, glow);

        spriteBatch.Draw(pixel, new Rectangle(tile.X + 4, tile.Y + 9, 7, 3), stone);
        spriteBatch.Draw(pixel, new Rectangle(tile.X + 5, tile.Y + 7, 5, 2), ash);
        spriteBatch.Draw(pixel, new Rectangle(tile.X + 6, tile.Y + 5, 3, 2), ember);
        spriteBatch.Draw(pixel, new Rectangle(tile.X + 7, tile.Y + 3, 1, 2), new Color((byte)200, (byte)200, (byte)205, (byte)(glow - 20)));
        spriteBatch.Draw(pixel, new Rectangle(tile.X + 6, tile.Y + 2, 1, 1), new Color((byte)210, (byte)210, (byte)215, (byte)(glow - 40)));
        spriteBatch.Draw(pixel, new Rectangle(tile.X + 8, tile.Y + 2, 1, 1), new Color((byte)210, (byte)210, (byte)215, (byte)(glow - 50)));
    }

    private static void DrawRestBench(SpriteBatch spriteBatch, Texture2D pixel, Rectangle tile, float pulse)
    {
        var wood = new Color((byte)118, (byte)82, (byte)52);
        var shadow = new Color((byte)78, (byte)54, (byte)34);
        var cushion = new Color((byte)(150 + (int)(10 * MathF.Sin(pulse))), (byte)58, (byte)58);

        spriteBatch.Draw(pixel, new Rectangle(tile.X + 2, tile.Y + 9, 11, 2), shadow);
        spriteBatch.Draw(pixel, new Rectangle(tile.X + 3, tile.Y + 7, 9, 2), wood);
        spriteBatch.Draw(pixel, new Rectangle(tile.X + 4, tile.Y + 5, 7, 2), cushion);
        spriteBatch.Draw(pixel, new Rectangle(tile.X + 3, tile.Y + 9, 1, 2), wood);
        spriteBatch.Draw(pixel, new Rectangle(tile.X + 11, tile.Y + 9, 1, 2), wood);
    }

    private static void DrawWhisperArrow(SpriteBatch spriteBatch, Texture2D pixel, Rectangle tile, float pulse)
    {
        var ink = new Color((byte)(170 + (int)(20 * MathF.Sin(pulse * 2f))), (byte)150, (byte)110, (byte)210);
        var tipX = tile.X + 10;
        var baseX = tile.X + 4;
        var y = tile.Y + 7;

        for (var x = baseX; x <= tipX - 2; x++)
        {
            spriteBatch.Draw(pixel, new Rectangle(x, y, 1, 1), ink);
            spriteBatch.Draw(pixel, new Rectangle(x, y + 1, 1, 1), ink);
        }

        spriteBatch.Draw(pixel, new Rectangle(tipX - 1, y - 1, 1, 1), ink);
        spriteBatch.Draw(pixel, new Rectangle(tipX, y, 1, 1), ink);
        spriteBatch.Draw(pixel, new Rectangle(tipX - 1, y + 2, 1, 1), ink);
        spriteBatch.Draw(pixel, new Rectangle(baseX, y - 2, 2, 1), ink);
        spriteBatch.Draw(pixel, new Rectangle(baseX + 1, y + 3, 2, 1), ink);
    }
}
