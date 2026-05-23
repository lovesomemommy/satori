using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.UI;
using Satori.Client.Views.Hub;
using Satori.Client.Views.Rendering;

namespace Satori.Client.Services.Hub;

public sealed class HubBackgroundCatalog : IDisposable
{
	private Texture2D? _temple;

	public void Initialize(GraphicsDevice graphicsDevice)
	{
		_temple?.Dispose();
		_temple = TextureLoadHelper.TryLoadFirstExisting(
			graphicsDevice,
			ClientAssetPaths.InFolder("HubImages", "temple.png"));
	}

	public void Draw(SpriteBatch spriteBatch, Texture2D pixel, int virtualWidth, int virtualHeight, bool isNight)
	{
		var templeBounds = new Rectangle(HubLayout.TempleX, HubLayout.ContentTop, HubLayout.TempleWidth, HubLayout.ContentHeight);
		spriteBatch.Draw(pixel, templeBounds, new Color(32, 32, 36, 90));
		if (_temple != null)
		{
			SpriteDrawHelper.DrawContained(spriteBatch, _temple, templeBounds, Color.White);
			return;
		}

		DrawProceduralTemple(spriteBatch, pixel, templeBounds, isNight);
	}

	private static void DrawProceduralTemple(SpriteBatch spriteBatch, Texture2D pixel, Rectangle bounds, bool isNight)
	{
		Color stone = isNight ? new Color(48, 48, 56) : new Color(78, 78, 86);
		Color roof = isNight ? new Color(62, 62, 70) : new Color(102, 102, 110);
		Color accent = isNight ? UiPalette.PinkDim : UiPalette.PinkSoft;
		int centerX = bounds.X + bounds.Width / 2;
		int baseY = bounds.Bottom - 6;
		spriteBatch.Draw(pixel, new Rectangle(bounds.X + 8, baseY - 4, bounds.Width - 16, 4), stone);
		spriteBatch.Draw(pixel, new Rectangle(centerX - 28, baseY - 32, 56, 28), stone);
		spriteBatch.Draw(pixel, new Rectangle(centerX - 20, baseY - 48, 40, 16), stone);
		spriteBatch.Draw(pixel, new Rectangle(centerX - 12, baseY - 58, 24, 10), stone);
		spriteBatch.Draw(pixel, new Rectangle(centerX - 32, baseY - 36, 64, 4), roof);
		spriteBatch.Draw(pixel, new Rectangle(centerX - 24, baseY - 50, 48, 4), roof);
		spriteBatch.Draw(pixel, new Rectangle(centerX - 16, baseY - 60, 32, 4), roof);
		spriteBatch.Draw(pixel, new Rectangle(centerX - 2, baseY - 66, 4, 8), accent);
		spriteBatch.Draw(pixel, new Rectangle(centerX - 12, baseY - 24, 4, 12), new Color(24, 24, 28));
		spriteBatch.Draw(pixel, new Rectangle(centerX + 8, baseY - 24, 4, 12), new Color(24, 24, 28));
	}

	public void Dispose()
	{
		_temple?.Dispose();
		_temple = null;
	}
}
