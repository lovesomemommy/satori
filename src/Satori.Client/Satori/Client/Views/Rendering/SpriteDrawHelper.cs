using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Satori.Client.Views.Rendering;

public static class SpriteDrawHelper
{
	public static void DrawStretched(SpriteBatch spriteBatch, Texture2D texture, Rectangle destination, Color color) =>
		spriteBatch.Draw(texture, destination, color);

	public static void DrawContained(SpriteBatch spriteBatch, Texture2D texture, Rectangle destination, Color color)
	{
		float scale = Math.Min(
			destination.Width / (float)texture.Width,
			destination.Height / (float)texture.Height);
		int width = Math.Max(1, (int)(texture.Width * scale));
		int height = Math.Max(1, (int)(texture.Height * scale));
		var drawRect = new Rectangle(
			destination.X + (destination.Width - width) / 2,
			destination.Y + (destination.Height - height) / 2,
			width,
			height);
		spriteBatch.Draw(texture, drawRect, color);
	}

	public static void DrawContainedBottom(SpriteBatch spriteBatch, Texture2D texture, Rectangle destination, Color color)
	{
		float scale = Math.Min(
			destination.Width / (float)texture.Width,
			destination.Height / (float)texture.Height);
		int width = Math.Max(1, (int)(texture.Width * scale));
		int height = Math.Max(1, (int)(texture.Height * scale));
		var drawRect = new Rectangle(
			destination.X + (destination.Width - width) / 2,
			destination.Bottom - height,
			width,
			height);
		spriteBatch.Draw(texture, drawRect, color);
	}

	public static void DrawContainedBottomScaled(
		SpriteBatch spriteBatch,
		Texture2D texture,
		Rectangle destination,
		float widthScale,
		float heightScale,
		Color color)
	{
		int width = Math.Max(1, (int)(destination.Width * widthScale));
		int height = Math.Max(1, (int)(destination.Height * heightScale));
		var expanded = new Rectangle(
			destination.X + (destination.Width - width) / 2,
			destination.Bottom - height,
			width,
			height);
		DrawContainedBottom(spriteBatch, texture, expanded, color);
	}

	public static void DrawContainedCropped(
		SpriteBatch spriteBatch,
		Texture2D texture,
		Rectangle destination,
		float cropFraction,
		Color color)
	{
		int cropX = (int)(texture.Width * cropFraction);
		int cropY = (int)(texture.Height * cropFraction);
		int sourceWidth = Math.Max(1, texture.Width - cropX * 2);
		int sourceHeight = Math.Max(1, texture.Height - cropY * 2);
		var source = new Rectangle(cropX, cropY, sourceWidth, sourceHeight);
		float scale = Math.Min(
			destination.Width / (float)sourceWidth,
			destination.Height / (float)sourceHeight);
		int width = Math.Max(1, (int)(sourceWidth * scale));
		int height = Math.Max(1, (int)(sourceHeight * scale));
		var drawRect = new Rectangle(
			destination.X + (destination.Width - width) / 2,
			destination.Y + (destination.Height - height) / 2,
			width,
			height);
		spriteBatch.Draw(texture, drawRect, source, color);
	}

	public static Rectangle ExpandCentered(Rectangle bounds, float scale)
	{
		int width = Math.Max(1, (int)(bounds.Width * scale));
		int height = Math.Max(1, (int)(bounds.Height * scale));
		return new Rectangle(
			bounds.X + (bounds.Width - width) / 2,
			bounds.Y + (bounds.Height - height) / 2,
			width,
			height);
	}
}
