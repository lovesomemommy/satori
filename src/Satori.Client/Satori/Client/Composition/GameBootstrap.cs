using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework.Graphics;
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
using Satori.Core.Interfaces.Services;

namespace Satori.Client.Composition;

public static class GameBootstrap
{
	public static SceneContext CreateSceneContext(SatoriGame game, SpriteBatch spriteBatch, Texture2D pixel, FixedViewportRenderer viewport, IServiceProvider services)
	{
		SceneManager requiredService = services.GetRequiredService<SceneManager>();
		GameStateMachine requiredService2 = services.GetRequiredService<GameStateMachine>();
		GameSession requiredService3 = services.GetRequiredService<GameSession>();
		ILocalizationService requiredService4 = services.GetRequiredService<ILocalizationService>();
		InputBindingService requiredService5 = services.GetRequiredService<InputBindingService>();
		TextRenderingService requiredService6 = services.GetRequiredService<TextRenderingService>();
		QuoteImageCatalog requiredService7 = services.GetRequiredService<QuoteImageCatalog>();
		ObstacleSpriteCatalog requiredService8 = services.GetRequiredService<ObstacleSpriteCatalog>();
		PilgrimSpriteCatalog requiredService9 = services.GetRequiredService<PilgrimSpriteCatalog>();
		GardenSpriteCatalog requiredService10 = services.GetRequiredService<GardenSpriteCatalog>();
		requiredService5.LoadBindings(requiredService3.Settings.Bindings);
		requiredService6.Initialize();
		requiredService7.Initialize(game.GraphicsDevice);
		requiredService8.Initialize(game.GraphicsDevice);
		requiredService9.Initialize(game.GraphicsDevice);
		requiredService10.Initialize(game.GraphicsDevice);
		services.GetRequiredService<HubBackgroundCatalog>().Initialize(game.GraphicsDevice);
		services.GetRequiredService<MenuBackgroundCatalog>().Initialize(game.GraphicsDevice);
		SceneContext sceneContext = new SceneContext
		{
			Game = game,
			GraphicsDevice = game.GraphicsDevice,
			SpriteBatch = spriteBatch,
			Pixel = pixel,
			Viewport = viewport,
			GameplayBounds = GameplayBounds.FromViewport(viewport),
			Localization = requiredService4,
			SceneManager = requiredService,
			StateMachine = requiredService2,
			Session = requiredService3,
			Services = services,
			Text = requiredService6,
			ObstacleSprites = requiredService8,
			PilgrimSprites = requiredService9,
			GardenSprites = requiredService10
		};
		requiredService.Initialize(sceneContext);
		requiredService.Register(GameStateType.Boot, () => services.GetRequiredService<BootScene>());
		requiredService.Register(GameStateType.MainMenu, () => services.GetRequiredService<MainMenuScene>());
		requiredService.Register(GameStateType.Hub, () => services.GetRequiredService<HubScene>());
		requiredService.Register(GameStateType.Garden, () => services.GetRequiredService<GardenScene>());
		requiredService.Register(GameStateType.PilgrimTrial, () => services.GetRequiredService<PilgrimTrialScene>());
		requiredService.Register(GameStateType.WisdomLibrary, () => services.GetRequiredService<WisdomLibraryScene>());
		requiredService.Register(GameStateType.MinigamesHub, () => services.GetRequiredService<MinigamesHubScene>());
		requiredService.Register(GameStateType.RightSpeech, () => services.GetRequiredService<RightSpeechScene>());
		requiredService.Register(GameStateType.WheelOfDharma, () => services.GetRequiredService<WheelOfDharmaScene>());
		requiredService.Register(GameStateType.MeditationTraining, () => services.GetRequiredService<MeditationTrainingScene>());
		requiredService.Register(GameStateType.Settings, () => services.GetRequiredService<SettingsScene>());
		requiredService.Register(GameStateType.Finale, () => services.GetRequiredService<FinaleScene>());
		services.GetRequiredService<IAudioService>().Initialize();
		return sceneContext;
	}
}
