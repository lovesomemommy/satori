using System;
using System.IO;

namespace Satori.Core.Utilities;

public static class PlatformPaths
{
	public static string GetSaveDirectory()
	{
		if (OperatingSystem.IsWindows())
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Satori", "saves");
		}
		if (OperatingSystem.IsMacOS())
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Application Support", "Satori", "saves");
		}
		string? environmentVariable = Environment.GetEnvironmentVariable("XDG_DATA_HOME");
		string path = (string.IsNullOrWhiteSpace(environmentVariable) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "share") : environmentVariable);
		return Path.Combine(path, "satori", "saves");
	}

	public static string GetDefaultSaveFilePath()
	{
		return Path.Combine(GetSaveDirectory(), "save_v1.json");
	}
}
