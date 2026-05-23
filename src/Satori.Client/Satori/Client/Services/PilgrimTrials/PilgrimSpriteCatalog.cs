using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.Views.Rendering;

namespace Satori.Client.Services.PilgrimTrials;

public sealed class PilgrimSpriteCatalog : IDisposable
{
	private readonly Dictionary<string, Texture2D> _cache = new(StringComparer.OrdinalIgnoreCase);

	private GraphicsDevice? _graphicsDevice;

	private bool _initialized;

	public Texture2D? WallTile { get; private set; }

	public Texture2D? PortalTile { get; private set; }

	public Texture2D? Lotus { get; private set; }

	public Texture2D? MonkWalking { get; private set; }

	public Texture2D? MonkMeditating { get; private set; }

	public Texture2D? Footprint { get; private set; }

	public void Initialize(GraphicsDevice graphicsDevice)
	{
		if (_initialized)
		{
			return;
		}

		_graphicsDevice = graphicsDevice;
		WallTile = Load("tile.wall.png");
		PortalTile = Load("tile.portal.png");
		Lotus = Load("lotus.png");
		MonkWalking = Load("monk.02.png");
		MonkMeditating = Load("monk.01.png");
		Footprint = Load("traces.png") ?? Load("footprint.png");
		_initialized = true;
	}

	public void Dispose()
	{
		foreach (var texture in _cache.Values)
		{
			texture.Dispose();
		}

		_cache.Clear();
		WallTile = null;
		PortalTile = null;
		Lotus = null;
		MonkWalking = null;
		MonkMeditating = null;
		Footprint = null;
		_initialized = false;
		_graphicsDevice = null;
	}

	private Texture2D? Load(string fileName)
	{
		if (_graphicsDevice == null)
		{
			return null;
		}

		if (_cache.TryGetValue(fileName, out var cached))
		{
			return cached;
		}

		var texture = TextureLoadHelper.TryLoadFirstExisting(_graphicsDevice, ClientAssetPaths.PilgrimImage(fileName));
		if (texture != null)
		{
			_cache[fileName] = texture;
		}

		return texture;
	}
}
