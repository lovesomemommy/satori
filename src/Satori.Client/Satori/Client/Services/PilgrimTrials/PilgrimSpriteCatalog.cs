using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.Views.Rendering;

namespace Satori.Client.Services.PilgrimTrials;

public sealed class PilgrimSpriteCatalog : IDisposable
{
	private readonly Dictionary<string, Texture2D> _cache = new(StringComparer.OrdinalIgnoreCase);

	private GraphicsDevice? _graphicsDevice;

	private bool _initialized;

	public Texture2D? WallTile { get; private set; }

	public Texture2D? FloorTile { get; private set; }

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
		FloorTile = Load("tile.floor.png");
		PortalTile = Load("tile.portal.png");
		Lotus = Load("lotus.png") ?? LoadFromAlternateRoots("lotus.png");
		MonkWalking = Load("monk.02.png");
		MonkMeditating = Load("monk.01.png");
		Footprint = Load("footprint.png");
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
		FloorTile = null;
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

		foreach (var candidatePath in GetCandidatePaths(fileName))
		{
			if (!File.Exists(candidatePath))
			{
				continue;
			}

			try
			{
				var texture = TextureLoadHelper.FromFile(_graphicsDevice, candidatePath);
				_cache[fileName] = texture;
				return texture;
			}
			catch (Exception)
			{
				return null;
			}
		}

		return null;
	}

	private Texture2D? LoadFromAlternateRoots(string fileName)
	{
		if (_graphicsDevice == null)
		{
			return null;
		}

		string[] alternateRoots =
		[
			Path.Combine(AppContext.BaseDirectory, "Services"),
			Path.Combine(AppContext.BaseDirectory, "PilgrimImages")
		];

		foreach (var root in alternateRoots)
		{
			string path = Path.Combine(root, fileName);
			if (!File.Exists(path))
			{
				continue;
			}

			try
			{
				var texture = TextureLoadHelper.FromFile(_graphicsDevice, path);
				_cache[fileName] = texture;
				return texture;
			}
			catch (Exception)
			{
				return null;
			}
		}

		return null;
	}

	private static IEnumerable<string> GetCandidatePaths(string fileName)
	{
		yield return Path.Combine(AppContext.BaseDirectory, "PiligrimImages", fileName);
		yield return Path.Combine(AppContext.BaseDirectory, "PilgrimImages", fileName);
	}
}
