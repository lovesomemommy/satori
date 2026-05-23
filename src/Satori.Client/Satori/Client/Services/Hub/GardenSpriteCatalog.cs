using Microsoft.Xna.Framework.Graphics;
using Satori.Client.Views.Rendering;

namespace Satori.Client.Services.Hub;

public sealed class GardenSpriteCatalog : IDisposable
{
	private Texture2D? _floorTile;

	public Texture2D? FloorTile => _floorTile;

	public void Initialize(GraphicsDevice graphicsDevice)
	{
		_floorTile?.Dispose();
		_floorTile = TextureLoadHelper.TryLoadFirstExisting(
			graphicsDevice,
			ClientAssetPaths.InFolder("GardenImages", "garden.floor.png"));
	}

	public void Dispose()
	{
		_floorTile?.Dispose();
		_floorTile = null;
	}
}
