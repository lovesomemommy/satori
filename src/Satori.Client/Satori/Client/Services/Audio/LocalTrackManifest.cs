using System.Text.Json;

namespace Satori.Client.Services.Audio;

public sealed class LocalTrackManifest
{
	public string DefaultTrackId { get; set; } = "hub";

	public List<LocalTrackEntry> Tracks { get; set; } = new List<LocalTrackEntry>();

	public static LocalTrackManifest LoadFromDirectory(string directory)
	{
		LocalTrackManifest manifest = LoadManifestFile(directory);
		if (!Directory.Exists(directory))
		{
			return manifest;
		}

		manifest.MergeDiscoveredTracks(directory);
		manifest.EnsureDefaultTrack();
		return manifest;
	}

	public LocalTrackEntry? FindTrack(string trackId)
	{
		foreach (LocalTrackEntry track in Tracks)
		{
			if (string.Equals(track.Id, trackId, StringComparison.OrdinalIgnoreCase))
			{
				return track;
			}
		}

		return null;
	}

	public string ResolveTrackFilePath(string directory, string trackId)
	{
		LocalTrackEntry? track = FindTrack(trackId);
		if (track == null || string.IsNullOrWhiteSpace(track.File))
		{
			return string.Empty;
		}

		return Path.Combine(directory, track.File);
	}

	private static LocalTrackManifest LoadManifestFile(string directory)
	{
		string manifestPath = Path.Combine(directory, "manifest.json");
		if (!File.Exists(manifestPath))
		{
			return new LocalTrackManifest();
		}

		string json = File.ReadAllText(manifestPath);
		return JsonSerializer.Deserialize<LocalTrackManifest>(json, new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true
		}) ?? new LocalTrackManifest();
	}

	private void MergeDiscoveredTracks(string directory)
	{
		foreach (string filePath in Directory.GetFiles(directory, "*.ogg"))
		{
			string fileName = Path.GetFileName(filePath);
			if (Tracks.Exists(track => string.Equals(track.File, fileName, StringComparison.OrdinalIgnoreCase)))
			{
				continue;
			}

			string id = Path.GetFileNameWithoutExtension(fileName);
			Tracks.Add(new LocalTrackEntry
			{
				Id = id,
				File = fileName,
				Label = id
			});
		}
	}

	private void EnsureDefaultTrack()
	{
		if (Tracks.Count == 0)
		{
			return;
		}

		if (FindTrack(DefaultTrackId) == null)
		{
			DefaultTrackId = Tracks[0].Id;
		}
	}
}

public sealed class LocalTrackEntry
{
	public string Id { get; set; } = string.Empty;

	public string File { get; set; } = string.Empty;

	public string Label { get; set; } = string.Empty;
}
