using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.Views.Rendering;
using Satori.Core.Models.PilgrimTrials;

namespace Satori.Client.Services.PilgrimTrials;

public sealed class ObstacleSpriteCatalog : IDisposable
{
	private readonly Dictionary<ObstacleType, Texture2D> _cache = new();

	private GraphicsDevice? _graphicsDevice;

	private bool _initialized;

	public void Initialize(GraphicsDevice graphicsDevice)
	{
		if (_initialized)
		{
			return;
		}

		_graphicsDevice = graphicsDevice;
		_initialized = true;
	}

	public Texture2D GetSprite(ObstacleType type)
	{
		if (!_initialized || _graphicsDevice == null)
		{
			throw new InvalidOperationException("ObstacleSpriteCatalog is not initialized.");
		}

		if (_cache.TryGetValue(type, out var cached))
		{
			return cached;
		}

		var texture = TryLoadFromDisk(type) ?? CreatePlaceholder(type);
		_cache[type] = texture;
		return texture;
	}

	public void Dispose()
	{
		foreach (var texture in _cache.Values)
		{
			texture.Dispose();
		}

		_cache.Clear();
	}

	private Texture2D? TryLoadFromDisk(ObstacleType type)
	{
		var fileName = type switch
		{
			ObstacleType.Harm => "beetle.png",
			ObstacleType.Temptation => "coin.png",
			ObstacleType.Mist => "cloud.png",
			_ => null
		};

		if (fileName == null || _graphicsDevice == null)
		{
			return null;
		}

		foreach (var candidatePath in GetCandidatePaths(fileName))
		{
			if (!File.Exists(candidatePath))
			{
				continue;
			}

			try
			{
				return TextureLoadHelper.FromFile(_graphicsDevice, candidatePath);
			}
			catch (Exception)
			{
				return null;
			}
		}

		return null;
	}

	private Texture2D CreatePlaceholder(ObstacleType type)
	{
		var graphicsDevice = _graphicsDevice
			?? throw new InvalidOperationException("ObstacleSpriteCatalog is not initialized.");

		var texture = new Texture2D(graphicsDevice, 14, 14);
		var pixels = new Color[196];
		for (var y = 0; y < 14; y++)
		{
			for (var x = 0; x < 14; x++)
			{
				pixels[y * 14 + x] = type switch
				{
					ObstacleType.Harm => PlaceholderBeetle(x, y, 14),
					ObstacleType.Temptation => PlaceholderCoin(x, y, 14),
					ObstacleType.Mist => Color.Transparent,
					ObstacleType.Distraction => PlaceholderDistraction(x, y, 14),
					_ => Color.Transparent
				};
			}
		}

		texture.SetData(pixels);
		return texture;
	}

	private static Color PlaceholderBeetle(int x, int y, int size)
	{
		var centerX = size / 2f;
		var centerY = size / 2f;
		var dx = x - centerX;
		var dy = y - centerY;
		if (dx * dx / 16f + dy * dy / 9f <= 1f)
		{
			return new Color(90, 50, 30);
		}

		if (MathF.Abs(dx) > 4f && MathF.Abs(dy) < 2f)
		{
			return new Color(60, 35, 20);
		}

		return Color.Transparent;
	}

	private static Color PlaceholderCoin(int x, int y, int size)
	{
		var dx = x - size / 2f;
		var dy = y - size / 2f;
		if (dx * dx + dy * dy <= 20f)
		{
			return (x == size / 2 - 2 && y == size / 2 - 2)
				? new Color(255, 240, 180)
				: new Color(210, 170, 40);
		}

		return Color.Transparent;
	}

	private static Color PlaceholderDistraction(int x, int y, int size)
	{
		var dx = x - size / 2f;
		var dy = y - size / 2f;
		return dx * dx + dy * dy <= 12f ? new Color(150, 90, 170) : Color.Transparent;
	}

	private static IEnumerable<string> GetCandidatePaths(string fileName)
	{
		yield return Path.Combine(AppContext.BaseDirectory, "ObstacleImages", fileName);
		yield return Path.Combine(AppContext.BaseDirectory, "QuoteImages", fileName);
		yield return Path.Combine(AppContext.BaseDirectory, "assets", "obstacles", fileName);
	}
}
