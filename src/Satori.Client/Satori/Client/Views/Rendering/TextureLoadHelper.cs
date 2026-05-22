using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Satori.Client.Views.Rendering;

public static class TextureLoadHelper
{
	private const byte DefaultAlphaThreshold = 80;

	public static Texture2D FromFile(GraphicsDevice graphicsDevice, string path, byte alphaThreshold = DefaultAlphaThreshold)
	{
		Texture2D texture;
		try
		{
			texture = Texture2D.FromFile(graphicsDevice, path);
		}
		catch
		{
			using var stream = File.OpenRead(path);
			texture = Texture2D.FromStream(graphicsDevice, stream);
		}

		StripAlphaFringe(texture, alphaThreshold);
		return texture;
	}

	public static void StripAlphaFringe(Texture2D texture, byte alphaThreshold = DefaultAlphaThreshold)
	{
		var pixels = new Color[texture.Width * texture.Height];
		texture.GetData(pixels);
		var changed = false;
		for (int i = 0; i < pixels.Length; i++)
		{
			if (pixels[i].A >= alphaThreshold)
			{
				continue;
			}

			if (pixels[i].A != 0)
			{
				pixels[i] = Color.Transparent;
				changed = true;
			}
		}

		if (changed)
		{
			texture.SetData(pixels);
		}
	}
}
