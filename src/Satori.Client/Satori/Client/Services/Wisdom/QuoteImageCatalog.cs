using System;
using System.Collections.Generic;
using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Satori.Client.Views.Rendering;
using Satori.Core.Services.Wisdom;

namespace Satori.Client.Services.Wisdom;

public sealed class QuoteImageCatalog : IDisposable
{
	private const int PlaceholderWidth = 160;

	private const int PlaceholderHeight = 96;

	private readonly QuoteCatalog _quoteCatalog;

	private readonly Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();

	private readonly FontSystem _fontSystem = new FontSystem();

	private GraphicsDevice? _graphicsDevice;

	private DynamicSpriteFont? _placeholderFont;

	private bool _initialized;

	public QuoteImageCatalog(QuoteCatalog quoteCatalog)
	{
		_quoteCatalog = quoteCatalog;
	}

	public void Initialize(GraphicsDevice graphicsDevice)
	{
		if (!_initialized)
		{
			_graphicsDevice = graphicsDevice;
			var fontPath = ResolveFontPath();
			if (fontPath != null && File.Exists(fontPath))
			{
				_fontSystem.AddFont(File.ReadAllBytes(fontPath));
				_placeholderFont = _fontSystem.GetFont(9f);
			}

			_initialized = true;
		}
	}

	public Texture2D? GetQuoteImage(string quoteId)
	{
		if (!_initialized || _graphicsDevice == null)
		{
			return null;
		}

		if (_cache.TryGetValue(quoteId, out var cached))
		{
			return cached;
		}

		var texture = TryLoadFromDisk(quoteId) ?? CreatePlaceholder(quoteId);
		_cache[quoteId] = texture;
		return texture;
	}

	public void Dispose()
	{
		foreach (Texture2D value in _cache.Values)
		{
			value.Dispose();
		}
		_cache.Clear();
		_placeholderFont = null;
	}

	private Texture2D? TryLoadFromDisk(string quoteId)
	{
		var graphicsDevice = _graphicsDevice;
		if (graphicsDevice == null)
		{
			return null;
		}

		foreach (string candidatePath in GetCandidatePaths(quoteId))
		{
			if (!File.Exists(candidatePath))
			{
				continue;
			}
			try
			{
				return TextureLoadHelper.FromFile(graphicsDevice, candidatePath);
			}
			catch (Exception)
			{
				return null;
			}
		}
		return null;
	}

	private Texture2D CreatePlaceholder(string quoteId)
	{
		var graphicsDevice = _graphicsDevice
			?? throw new InvalidOperationException("QuoteImageCatalog is not initialized.");

		var renderTarget = new RenderTarget2D(graphicsDevice, PlaceholderWidth, PlaceholderHeight, false, SurfaceFormat.Color, DepthFormat.None);
		var previousTargets = graphicsDevice.GetRenderTargets();
		graphicsDevice.SetRenderTarget(renderTarget);
		graphicsDevice.Clear(new Color(58, 72, 88));
		if (_placeholderFont != null && _quoteCatalog.TryGetText(quoteId, out var text))
		{
			using var spriteBatch = new SpriteBatch(graphicsDevice);
			spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
			const string label = "Заглушка";
			_placeholderFont.DrawText(spriteBatch, label, new Vector2(6f, 4f), new Color(180, 200, 210), 0f, default, null);
			DrawWrappedPlaceholderText(spriteBatch, text, new Rectangle(6, 18, 148, 72));
			spriteBatch.End();
		}

		graphicsDevice.SetRenderTargets(previousTargets);
		return renderTarget;
	}

	private void DrawWrappedPlaceholderText(SpriteBatch spriteBatch, string text, Rectangle bounds)
	{
		if (_placeholderFont == null || string.IsNullOrWhiteSpace(text))
		{
			return;
		}
		string[] array = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		string text2 = string.Empty;
		int num = bounds.Y;
		string[] array2 = array;
		foreach (string text3 in array2)
		{
			string text4 = (string.IsNullOrEmpty(text2) ? text3 : (text2 + " " + text3));
			if (_placeholderFont.MeasureString(text4, null).X > (float)bounds.Width && !string.IsNullOrEmpty(text2))
			{
				_placeholderFont.DrawText(spriteBatch, text2, new Vector2(bounds.X, num), new Color(230, 225, 210), 0f, default(Vector2), null);
				num += 11;
				text2 = text3;
			}
			else
			{
				text2 = text4;
			}
			if (num > bounds.Bottom - 11)
			{
				break;
			}
		}
		if (!string.IsNullOrEmpty(text2) && num <= bounds.Bottom - 11)
		{
			_placeholderFont.DrawText(spriteBatch, text2, new Vector2(bounds.X, num), new Color(230, 225, 210), 0f, default(Vector2), null);
		}
	}

	private static IEnumerable<string> GetCandidatePaths(string quoteId)
	{
		string fileName = quoteId + ".png";
		yield return Path.Combine(AppContext.BaseDirectory, "QuoteImages", fileName);
		yield return Path.Combine(AppContext.BaseDirectory, "assets", "quotes", fileName);
	}

	private static string? ResolveFontPath()
	{
		string text = Path.Combine(AppContext.BaseDirectory, "Fonts", "DejaVuSans.ttf");
		if (File.Exists(text))
		{
			return text;
		}
		if (OperatingSystem.IsWindows())
		{
			string text2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
			if (File.Exists(text2))
			{
				return text2;
			}
		}
		if (OperatingSystem.IsMacOS())
		{
			string text3 = "/System/Library/Fonts/Supplemental/Arial.ttf";
			if (File.Exists(text3))
			{
				return text3;
			}
		}
		if (OperatingSystem.IsLinux())
		{
			string[] array = new string[2] { "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", "/usr/share/fonts/TTF/DejaVuSans.ttf" };
			foreach (string text4 in array)
			{
				if (File.Exists(text4))
				{
					return text4;
				}
			}
		}
		return null;
	}
}
