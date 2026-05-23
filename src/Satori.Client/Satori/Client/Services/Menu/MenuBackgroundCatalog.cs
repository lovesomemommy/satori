using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.Views.Rendering;

namespace Satori.Client.Services.Menu;

public sealed class MenuBackgroundCatalog : IDisposable
{
	private Texture2D? _background;

	public void Initialize(GraphicsDevice graphicsDevice)
	{
		_background?.Dispose();
		_background = null;

		foreach (string fileName in new[] { "background.png", "menu.background.png" })
		{
			_background = TextureLoadHelper.TryLoadFirstExisting(
				graphicsDevice,
				ClientAssetPaths.InFolder("MenuImages", fileName));
			if (_background != null)
			{
				return;
			}
		}
	}

	public void Draw(SpriteBatch spriteBatch, Texture2D pixel, int virtualWidth, int virtualHeight)
	{
		var bounds = new Rectangle(0, 0, virtualWidth, virtualHeight);
		if (_background != null)
		{
			SpriteDrawHelper.DrawStretched(spriteBatch, _background, bounds, Color.White);
			return;
		}

		spriteBatch.Draw(pixel, bounds, new Color(14, 18, 24));
	}

	public void Dispose()
	{
		_background?.Dispose();
		_background = null;
	}
}
