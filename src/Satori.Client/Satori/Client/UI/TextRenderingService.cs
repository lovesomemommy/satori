using System;
using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Satori.Client.UI;

public sealed class TextRenderingService : IDisposable
{
	private readonly FontSystem _fontSystem = new FontSystem(new FontSystemSettings
	{
		FontResolutionFactor = 3,
		KernelWidth = 0,
		KernelHeight = 0
	});

	private DynamicSpriteFont? _bodyFont;

	private DynamicSpriteFont? _compactFont;

	private DynamicSpriteFont? _titleFont;

	private bool _initialized;

	public bool IsReady => _initialized;

	public void Initialize()
	{
		if (!_initialized)
		{
			string? fontPath = ResolveFontPath();
			if (fontPath != null && File.Exists(fontPath))
			{
				_fontSystem.AddFont(File.ReadAllBytes(fontPath));
				_bodyFont = _fontSystem.GetFont(9f);
				_compactFont = _fontSystem.GetFont(8f);
				_titleFont = _fontSystem.GetFont(14f);
				_initialized = true;
			}
		}
	}

	public Vector2 MeasureText(string text, bool title = false, bool compact = false)
	{
		if (!_initialized)
		{
			return Vector2.Zero;
		}

		var font = ResolveFont(title, compact);
		return font?.MeasureString(text, null) ?? Vector2.Zero;
	}

	public int MeasureWrappedHeight(string text, int maxWidth, int lineHeight = 10, bool compact = false)
	{
		if (!_initialized || string.IsNullOrWhiteSpace(text))
		{
			return lineHeight;
		}

		var font = ResolveFont(false, compact);
		if (font == null)
		{
			return lineHeight;
		}

		string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		string line = string.Empty;
		int lines = 1;
		foreach (string word in words)
		{
			string candidate = string.IsNullOrEmpty(line) ? word : line + " " + word;
			if (font.MeasureString(candidate, null).X > maxWidth && !string.IsNullOrEmpty(line))
			{
				lines++;
				line = word;
			}
			else
			{
				line = candidate;
			}
		}

		return lines * lineHeight;
	}

	public void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, bool title = false, bool compact = false)
	{
		if (!_initialized)
		{
			return;
		}

		var font = ResolveFont(title, compact);
		if (font == null)
		{
			return;
		}

		var snapped = new Vector2(MathF.Round(position.X), MathF.Round(position.Y));
		DrawShadow(spriteBatch, font, text, snapped);
		font.DrawText(spriteBatch, text, snapped, color, 0f, default(Vector2), null);
	}

	public void DrawWrappedText(
		SpriteBatch spriteBatch,
		string text,
		Rectangle bounds,
		Color color,
		int lineHeight = 10,
		bool compact = false)
	{
		if (!_initialized || string.IsNullOrWhiteSpace(text))
		{
			return;
		}

		var font = ResolveFont(false, compact);
		if (font == null)
		{
			return;
		}

		string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		string line = string.Empty;
		int y = bounds.Y;
		foreach (string word in words)
		{
			string candidate = string.IsNullOrEmpty(line) ? word : line + " " + word;
			if (font.MeasureString(candidate, null).X > bounds.Width && !string.IsNullOrEmpty(line))
			{
				var pos = new Vector2(MathF.Round(bounds.X), MathF.Round(y));
				DrawShadow(spriteBatch, font, line, pos);
				font.DrawText(spriteBatch, line, pos, color, 0f, default(Vector2), null);
				y += lineHeight;
				line = word;
			}
			else
			{
				line = candidate;
			}

			if (y > bounds.Bottom - lineHeight)
			{
				break;
			}
		}

		if (!string.IsNullOrEmpty(line) && y <= bounds.Bottom - lineHeight)
		{
			var pos = new Vector2(MathF.Round(bounds.X), MathF.Round(y));
			DrawShadow(spriteBatch, font, line, pos);
			font.DrawText(spriteBatch, line, pos, color, 0f, default(Vector2), null);
		}
	}

	public void Dispose()
	{
		_bodyFont = null;
		_compactFont = null;
		_titleFont = null;
	}

	private DynamicSpriteFont? ResolveFont(bool title, bool compact)
	{
		if (compact)
		{
			return _compactFont;
		}

		return title ? _titleFont : _bodyFont;
	}

	private static void DrawShadow(SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 position)
	{
		var shadow = new Vector2(position.X + 1f, position.Y + 1f);
		font.DrawText(spriteBatch, text, shadow, new Color(0, 0, 0, 120), 0f, default(Vector2), null);
	}

	private static string? ResolveFontPath()
	{
		string bundledPixel = Path.Combine(AppContext.BaseDirectory, "Fonts", "PressStart2P-Regular.ttf");
		if (File.Exists(bundledPixel))
		{
			return bundledPixel;
		}

		string bundled = Path.Combine(AppContext.BaseDirectory, "Fonts", "DejaVuSansMono.ttf");
		if (File.Exists(bundled))
		{
			return bundled;
		}

		if (OperatingSystem.IsWindows())
		{
			string[] windowsCandidates =
			[
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "consola.ttf"),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "cour.ttf")
			];
			foreach (string candidate in windowsCandidates)
			{
				if (File.Exists(candidate))
				{
					return candidate;
				}
			}
		}

		if (OperatingSystem.IsMacOS())
		{
			string[] macCandidates =
			[
				"/System/Library/Fonts/Supplemental/Courier New.ttf",
				"/System/Library/Fonts/Menlo.ttc"
			];
			foreach (string candidate in macCandidates)
			{
				if (File.Exists(candidate))
				{
					return candidate;
				}
			}
		}

		if (OperatingSystem.IsLinux())
		{
			string[] linuxCandidates =
			[
				"/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf",
				"/usr/share/fonts/TTF/DejaVuSansMono.ttf"
			];
			foreach (string candidate in linuxCandidates)
			{
				if (File.Exists(candidate))
				{
					return candidate;
				}
			}
		}

		return null;
	}
}
