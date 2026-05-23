using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Satori.Core.Interfaces.Events;
using Satori.Core.Interfaces.Services;
using Satori.Core.Services.Localization;
using Satori.Core.Services.Minigames;
using Satori.Core.Services.Save;
using Satori.Core.Services.Wisdom;
using Satori.Core.Systems.Lotus;
using Satori.Core.Systems.Minigames;
using Satori.Core.Systems.PilgrimTrials;
using Satori.Core.Systems.Progression;
using Satori.Core.Systems.Wisdom;
using Satori.Core.Utilities;

namespace Satori.Core.Composition;

public static class CoreServiceCollectionExtensions
{
	public static IServiceCollection AddSatoriCore(this IServiceCollection services)
	{
		services.AddSingleton<IGameEventBus, GameEventBus>();
		services.AddSingleton<TrialTimerSystem>();
		services.AddSingleton<TrialRunSystem>();
		services.AddSingleton<SegmentTransitionSystem>();
		services.AddSingleton<TrapSystem>();
		services.AddSingleton<DecoyTrailSystem>();
		services.AddPreceptHandlers();
		services.AddSingleton<KarmaSystem>();
		services.AddSingleton<GardenPlantingSystem>();
		services.AddSingleton((IServiceProvider _) => QuoteCatalog.CreateDefault());
		services.AddSingleton<QuoteUnlockSystem>();
		services.AddSingleton<MeditationSystem>();
		services.AddSingleton<MeditationTrainingSystem>();
		services.AddSingleton<RightSpeechCatalog>(RightSpeechCatalog.CreateDefault());
		services.AddSingleton<RightSpeechSystem>();
		services.AddSingleton<WheelOfDharmaSystem>();
		services.AddSingleton<LotusCollectionSystem>();
		services.AddSingleton<PilgrimPilgrimageSystem>();
		services.AddSingleton<ISaveLoadService, SaveLoadService>();
		services.AddSingleton((IServiceProvider _) => CreateDefaultLocalization());
		return services;
	}

	public static ILocalizationService CreateDefaultLocalization()
	{
		string path = Path.Combine(AppContext.BaseDirectory, "Localization", "ru.json");
		if (File.Exists(path))
		{
			return JsonLocalizationService.LoadFromFile(path);
		}
		return JsonLocalizationService.LoadFromJson("{\n  \"hub.title\": \"Храм Satori\",\n  \"pilgrim.title\": \"Испытания паломника\"\n}");
	}
}
