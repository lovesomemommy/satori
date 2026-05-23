using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Composition;
using Satori.Client.Input;
using Satori.Client.Scenes;
using Satori.Client.Services.Audio;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.Views.Rendering;
using Satori.Core.Composition;
using Satori.Core.Interfaces.Services;

namespace Satori.Client;

public sealed class SatoriGame : Game
{
	private readonly GraphicsDeviceManager _graphics;

	private readonly ServiceProvider _serviceProvider;

	private SpriteBatch _spriteBatch = null!;

	private FixedViewportRenderer _viewport = null!;

	private DisplaySettingsApplier _displaySettings = null!;

	private IAudioService _audioService = null!;

	private Texture2D? _pixel;

	private SceneContext? _sceneContext;

	public SatoriGame(GameLaunchOptions? launchOptions = null)
	{
		_graphics = new GraphicsDeviceManager(this);
		base.Content.RootDirectory = "Content";
		ServiceCollection services = new ServiceCollection();
		services.AddSingleton(launchOptions ?? GameLaunchOptions.Default);
		services.AddSatoriCore();
		services.AddSatoriClient();
		_serviceProvider = services.BuildServiceProvider();
		_graphics.PreferredBackBufferWidth = 1280;
		_graphics.PreferredBackBufferHeight = 720;
		_graphics.SynchronizeWithVerticalRetrace = true;
		base.Window.Title = "Satori";
		base.IsMouseVisible = true;
		base.Window.AllowUserResizing = true;
	}

	protected override void Initialize()
	{
		_viewport = new FixedViewportRenderer();
		_displaySettings = _serviceProvider.GetRequiredService<DisplaySettingsApplier>();
		_audioService = _serviceProvider.GetRequiredService<IAudioService>();
		base.Initialize();
		SdlNative.ConfigureForGameplay();
	}

	protected override void LoadContent()
	{
		_spriteBatch = new SpriteBatch(base.GraphicsDevice);
		_pixel = new Texture2D(base.GraphicsDevice, 1, 1);
		_pixel.SetData(new Color[1] { Color.White });
		_sceneContext = GameBootstrap.CreateSceneContext(this, _spriteBatch, _pixel, _viewport, _serviceProvider);
		_displaySettings.Initialize(this, _graphics, PersistSettings);
		SdlNative.ConfigureForGameplay();
		_sceneContext.SceneManager.ChangeTo(GameStateType.Boot, useFade: false);
	}

	private void PersistSettings()
	{
		var session = _serviceProvider.GetRequiredService<GameSession>();
		_serviceProvider.GetRequiredService<ISaveLoadService>().SaveDefault(session.Save);
	}

	protected override void Update(GameTime gameTime)
	{
		base.IsMouseVisible = true;
		SdlNative.EnsureCursorVisible();
		KeyboardState state = Keyboard.GetState();
		if (_sceneContext != null)
		{
			_displaySettings.Update(state, _sceneContext.Session.Settings);
		}

		_audioService.Update(gameTime);
		_viewport.UpdateForBackbuffer(base.GraphicsDevice.PresentationParameters.BackBufferWidth, base.GraphicsDevice.PresentationParameters.BackBufferHeight);
		_sceneContext?.SceneManager.Update(gameTime);
		base.Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		base.GraphicsDevice.Clear(new Color(18, 18, 22));
		if (_sceneContext == null || _pixel == null)
		{
			base.Draw(gameTime);
			return;
		}
		_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, _viewport.GetTransformMatrix());
		_sceneContext.SceneManager.Draw(gameTime);
		_spriteBatch.End();
		DrawLetterboxBars();
		base.Draw(gameTime);
	}

	private void DrawLetterboxBars()
	{
		if (_pixel == null)
		{
			return;
		}

		var letterbox = _viewport.LetterboxBounds;
		int backbufferWidth = base.GraphicsDevice.PresentationParameters.BackBufferWidth;
		int backbufferHeight = base.GraphicsDevice.PresentationParameters.BackBufferHeight;
		var background = UiPalette.Background;
		_spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		if (letterbox.Y > 0)
		{
			_spriteBatch.Draw(_pixel, new Rectangle(0, 0, backbufferWidth, letterbox.Y), background);
		}

		if (letterbox.Bottom < backbufferHeight)
		{
			_spriteBatch.Draw(_pixel, new Rectangle(0, letterbox.Bottom, backbufferWidth, backbufferHeight - letterbox.Bottom), background);
		}

		if (letterbox.X > 0)
		{
			_spriteBatch.Draw(_pixel, new Rectangle(0, letterbox.Y, letterbox.X, letterbox.Height), background);
		}

		if (letterbox.Right < backbufferWidth)
		{
			_spriteBatch.Draw(_pixel, new Rectangle(letterbox.Right, letterbox.Y, backbufferWidth - letterbox.Right, letterbox.Height), background);
		}

		_spriteBatch.End();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_pixel?.Dispose();
			_serviceProvider.Dispose();
		}
		base.Dispose(disposing);
	}
}
