using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Satori.Client.Services.Hub;

public sealed class GardenSpriteCatalog : IDisposable
{
	private Texture2D? _floorTile;

	public Texture2D? FloorTile => _floorTile;

	public void Initialize(GraphicsDevice graphicsDevice)
	{
		_floorTile?.Dispose();
		_floorTile = null;

		foreach (var candidatePath in GetCandidatePaths("garden.floor.png"))
		{
			if (!File.Exists(candidatePath))
			{
				continue;
			}

			try
			{
				_floorTile = Texture2D.FromFile(graphicsDevice, candidatePath);
				return;
			}
			catch (Exception)
			{
				try
				{
					using var stream = File.OpenRead(candidatePath);
					_floorTile = Texture2D.FromStream(graphicsDevice, stream);
					return;
				}
				catch (Exception)
				{
					return;
				}
			}
		}
	}

	public void Dispose()
	{
		_floorTile?.Dispose();
		_floorTile = null;
	}

	private static IEnumerable<string> GetCandidatePaths(string fileName)
	{
		yield return Path.Combine(AppContext.BaseDirectory, "GardenImages", fileName);
	}
}
