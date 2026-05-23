using System.IO;
using System.Text.Json;
using Satori.Core.Models.Save;

namespace Satori.Core.Services.Save;

public sealed class JsonSaveRepository
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	private readonly SaveMigrator _migrator = new();

	public SaveGameModel Load(string path)
	{
		if (!File.Exists(path))
		{
			return new SaveGameModel();
		}

		string json = File.ReadAllText(path);
		SaveGameModel model = JsonSerializer.Deserialize<SaveGameModel>(json, JsonOptions) ?? new SaveGameModel();
		return _migrator.Migrate(model);
	}

	public void SaveAtomic(string path, SaveGameModel model)
	{
		string? directoryName = Path.GetDirectoryName(path);
		if (!string.IsNullOrWhiteSpace(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}

		model.Version = 1;
		SaveGameModel value = _migrator.Migrate(model);
		string contents = JsonSerializer.Serialize(value, JsonOptions);
		string tempPath = path + ".tmp";
		string backupPath = path + ".bak";
		File.WriteAllText(tempPath, contents);
		if (File.Exists(path))
		{
			File.Copy(path, backupPath, overwrite: true);
			File.Delete(path);
		}

		File.Move(tempPath, path);
	}

	public bool Exists(string path) => File.Exists(path);
}
