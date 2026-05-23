namespace Satori.Client.Views.Rendering;

public static class FontPathResolver
{
	public static string? ResolveUiFontPath()
	{
		string bundledPixel = Path.Combine(AppContext.BaseDirectory, "Fonts", "PressStart2P-Regular.ttf");
		if (File.Exists(bundledPixel))
		{
			return bundledPixel;
		}

		string bundledMono = Path.Combine(AppContext.BaseDirectory, "Fonts", "DejaVuSansMono.ttf");
		if (File.Exists(bundledMono))
		{
			return bundledMono;
		}

		return ResolveFirstExisting(GetSystemUiFontCandidates());
	}

	public static string? ResolveQuotePlaceholderFontPath()
	{
		string bundled = Path.Combine(AppContext.BaseDirectory, "Fonts", "DejaVuSans.ttf");
		if (File.Exists(bundled))
		{
			return bundled;
		}

		return ResolveFirstExisting(GetSystemQuoteFontCandidates());
	}

	private static string? ResolveFirstExisting(IEnumerable<string> candidates)
	{
		foreach (string candidate in candidates)
		{
			if (File.Exists(candidate))
			{
				return candidate;
			}
		}

		return null;
	}

	private static IEnumerable<string> GetSystemUiFontCandidates()
	{
		if (OperatingSystem.IsWindows())
		{
			yield return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "consola.ttf");
			yield return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "cour.ttf");
			yield break;
		}

		if (OperatingSystem.IsMacOS())
		{
			yield return "/System/Library/Fonts/Supplemental/Courier New.ttf";
			yield return "/System/Library/Fonts/Menlo.ttc";
			yield break;
		}

		if (OperatingSystem.IsLinux())
		{
			yield return "/usr/share/fonts/truetype/dejavu/DejaVuSansMono.ttf";
			yield return "/usr/share/fonts/TTF/DejaVuSansMono.ttf";
		}
	}

	private static IEnumerable<string> GetSystemQuoteFontCandidates()
	{
		if (OperatingSystem.IsWindows())
		{
			yield return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
			yield break;
		}

		if (OperatingSystem.IsMacOS())
		{
			yield return "/System/Library/Fonts/Supplemental/Arial.ttf";
			yield break;
		}

		if (OperatingSystem.IsLinux())
		{
			yield return "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf";
			yield return "/usr/share/fonts/TTF/DejaVuSans.ttf";
		}
	}
}
