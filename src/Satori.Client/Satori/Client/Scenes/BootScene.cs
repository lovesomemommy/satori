using Satori.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Satori.Client.Input;
using Satori.Client.Services.Audio;
using Satori.Client.State;
using Satori.Client.Views.Rendering;
using Satori.Core.Interfaces.Services;

namespace Satori.Client.Scenes;

public sealed class BootScene : IScene
{
	private SceneContext? _context;

	private bool _handoffComplete;

	public void Load(SceneContext context)
	{
		_context = context;
		_context.StateMachine.TransitionTo(GameStateType.Boot);
	}

	public void Unload()
	{
		_context = null;
	}

	public void Update(GameTime gameTime)
	{
		if (_handoffComplete || _context == null)
		{
			return;
		}

		_handoffComplete = true;
		var saveLoad = (ISaveLoadService)_context.Services.GetRequiredService(typeof(ISaveLoadService));
		_context.Session.ReplaceSave(saveLoad.LoadOrCreateDefault());
		_context.Services.GetRequiredService<InputBindingService>().LoadBindings(_context.Session.Settings.Bindings);
		var display = _context.Services.GetRequiredService<DisplaySettingsApplier>();
		display.Apply(_context.Session.Settings);
		var audio = _context.Services.GetRequiredService<IAudioService>();
		audio.ApplySettings(_context.Session.Settings);
		var launchOptions = _context.Services.GetRequiredService<GameLaunchOptions>();
		if (launchOptions.PostBootState == GameStateType.Settings)
		{
			SettingsSceneStarter.Open(_context, launchOptions.SettingsReturnState);
		}
		else
		{
			_context.SceneManager.ChangeTo(launchOptions.PostBootState, useFade: false);
		}
	}

	public void Draw(GameTime gameTime)
	{
		if (_context != null)
		{
			_context.SpriteBatch.Draw(_context.Pixel, new Rectangle(0, 0, _context.Viewport.VirtualWidth, _context.Viewport.VirtualHeight), new Color(14, 18, 28));
		}
	}
}
