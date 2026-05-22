using Satori.Core.Services.Localization;
using Xunit;

namespace Satori.Tests.Services;

public sealed class LocalizationServiceTests
{
	[Fact]
	public void Get_ReturnsRussianPilgrimTitle()
	{
		string json = "{\n  \"pilgrim.title\": \"Испытания паломника\",\n  \"minigames.right_speech.title\": \"Правая речь\"\n}";
		JsonLocalizationService jsonLocalizationService = JsonLocalizationService.LoadFromJson(json);
		Assert.Equal("Испытания паломника", jsonLocalizationService.Get("pilgrim.title"));
		Assert.Equal("Правая речь", jsonLocalizationService.Get("minigames.right_speech.title"));
	}

	[Fact]
	public void Get_UnknownKey_ReturnsKey()
	{
		JsonLocalizationService jsonLocalizationService = JsonLocalizationService.LoadFromJson("{}");
		Assert.Equal("missing.key", jsonLocalizationService.Get("missing.key"));
	}
}
