using System.Text.Json;
using System.Text.Json.Serialization;
using Satori.Core.Models.Minigames;

namespace Satori.Core.Services.Minigames;

public sealed class RightSpeechCatalog
{
	private List<RightSpeechQuestionModel> _questions = new List<RightSpeechQuestionModel>();

	public RightSpeechCatalog(int version, IReadOnlyList<RightSpeechQuestionModel> questions)
	{
		Version = version;
		_questions = questions.ToList();
	}

	public int Version { get; private set; }

	public IReadOnlyList<RightSpeechQuestionModel> Questions => _questions;

	public string GetRevision() =>
		$"{Version}:{string.Join(',', _questions.OrderBy(question => question.Id).Select(question => question.Id))}";

	public static RightSpeechCatalog CreateDefault()
	{
		string path = Path.Combine(AppContext.BaseDirectory, "RightSpeech", "right_speech.ru.json");
		return File.Exists(path) ? LoadFromFile(path) : CreateFallback();
	}

	public void Reload()
	{
		string path = Path.Combine(AppContext.BaseDirectory, "RightSpeech", "right_speech.ru.json");
		RightSpeechCatalog loaded = File.Exists(path) ? LoadFromFile(path) : CreateFallback();
		Version = loaded.Version;
		_questions = loaded._questions.ToList();
	}

	public static RightSpeechCatalog LoadFromFile(string path)
	{
		string json = File.ReadAllText(path);
		if (json.TrimStart().StartsWith('{'))
		{
			RightSpeechCatalogFileModel? fileModel = JsonSerializer.Deserialize<RightSpeechCatalogFileModel>(json, JsonOptions);
			if (fileModel?.Questions.Count > 0)
			{
				return new RightSpeechCatalog(fileModel.Version, fileModel.Questions);
			}
		}

		var legacyQuestions = JsonSerializer.Deserialize<List<RightSpeechQuestionModel>>(json, JsonOptions)
			?? new List<RightSpeechQuestionModel>();
		return new RightSpeechCatalog(1, legacyQuestions);
	}

	public RightSpeechQuestionModel? PickRandom(Random random, IReadOnlyCollection<string> completedIds)
	{
		if (_questions.Count == 0)
		{
			return null;
		}

		var pool = _questions.Where(question => !completedIds.Contains(question.Id)).ToList();
		if (pool.Count == 0)
		{
			return null;
		}

		return pool[random.Next(pool.Count)];
	}

	private static RightSpeechCatalog CreateFallback() =>
		new RightSpeechCatalog(1, new List<RightSpeechQuestionModel>
		{
			new RightSpeechQuestionModel
			{
				Id = "rs.fallback.01",
				Situation = "Друг просит тебя солгать ради него.",
				Options = new List<string> { "Согласиться", "Отказаться мягко и честно", "Рассказать всем правду" },
				CorrectIndex = 1
			}
		});

	private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
	{
		PropertyNameCaseInsensitive = true,
		ReadCommentHandling = JsonCommentHandling.Skip,
		AllowTrailingCommas = true,
		Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
	};
}
