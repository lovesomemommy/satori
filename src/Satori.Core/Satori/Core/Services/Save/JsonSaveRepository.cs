using System.IO;
using System.Text.Json;
using Satori.Core.Interfaces.Services;
using Satori.Core.Models.Save;

namespace Satori.Core.Services.Save;

public sealed class JsonSaveRepository : ISaveRepository
{
	private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
	{
		WriteIndented = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	private readonly SaveMigrator _migrator = new SaveMigrator();

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
		string text = path + ".tmp";
		string destFileName = path + ".bak";
		File.WriteAllText(text, contents);
		if (File.Exists(path))
		{
			File.Copy(path, destFileName, overwrite: true);
		}
		if (File.Exists(path))
		{
			File.Delete(path);
		}
		File.Move(text, path);
	}

	public bool Exists(string path)
	{
		return File.Exists(path);
	}
}
