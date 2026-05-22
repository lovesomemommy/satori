using System;
using Microsoft.Extensions.DependencyInjection;
using Satori.Client.Controllers;
using Satori.Client.Input;
using Satori.Client.Scenes;
using Satori.Client.Scenes.Minigames;
using Satori.Client.Scenes.PilgrimTrials;
using Satori.Client.Scenes.Wisdom;
using Satori.Client.Services.Audio;
using Satori.Client.Services.Hub;
using Satori.Client.Services.Menu;
using Satori.Client.Services.PilgrimTrials;
using Satori.Client.Services.Wisdom;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.Views.Rendering;

namespace Satori.Client.Composition;

public static class ClientServiceCollectionExtensions
{
	public static IServiceCollection AddSatoriClient(this IServiceCollection services)
	{
		services.AddSingleton<GameStateMachine>();
		services.AddSingleton<GameSession>();
		services.AddSingleton<SceneManager>();
		services.AddSingleton<InputBindingService>();
		services.AddSingleton<GameplayInputController>();
		services.AddSingleton((Func<IServiceProvider, IInputController>)((IServiceProvider sp) => sp.GetRequiredService<RebindingInputController>()));
		services.AddSingleton<RebindingInputController>();
		services.AddSingleton<TextRenderingService>();
		services.AddSingleton<QuoteImageCatalog>();
		services.AddSingleton<ObstacleSpriteCatalog>();
		services.AddSingleton<PilgrimSpriteCatalog>();
		services.AddSingleton<GardenSpriteCatalog>();
		services.AddSingleton<IAudioService, AudioService>();
		services.AddSingleton<DisplaySettingsApplier>();
		services.AddSingleton<HubBackgroundCatalog>();
		services.AddSingleton<MenuBackgroundCatalog>();
		services.AddSingleton<BootScene>();
		services.AddSingleton<MainMenuScene>();
		services.AddSingleton<HubScene>();
		services.AddSingleton<GardenScene>();
		services.AddSingleton<PilgrimTrialScene>();
		services.AddSingleton<WisdomLibraryScene>();
		services.AddSingleton<MinigamesHubScene>();
		services.AddSingleton<RightSpeechScene>();
		services.AddSingleton<WheelOfDharmaScene>();
		services.AddSingleton<MeditationTrainingScene>();
		services.AddSingleton<SettingsScene>();
		services.AddSingleton<FinaleScene>();
		return services;
	}
}
