using System;
using System.IO;
using System.Text.Json;
using Satori.Core.Utilities;

namespace Satori.Windows;

internal static class LauncherPreferences
{
	private sealed class LauncherConfig
	{
		public bool SkipLauncher { get; set; }
	}

	public static bool ShouldSkipLauncher()
	{
		try
		{
			string path = PlatformPaths.GetLauncherConfigPath();
			if (!File.Exists(path))
			{
				return false;
			}

			string json = File.ReadAllText(path);
			LauncherConfig? config = JsonSerializer.Deserialize<LauncherConfig>(json);
			return config?.SkipLauncher == true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static void SetSkipLauncher(bool skipLauncher)
	{
		string path = PlatformPaths.GetLauncherConfigPath();
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		var config = new LauncherConfig { SkipLauncher = skipLauncher };
		string json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
		File.WriteAllText(path, json);
	}
}
