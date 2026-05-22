using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Satori.Client.Services.Audio;
using Satori.Client.Scenes.Transitions;
using Satori.Client.State;

namespace Satori.Client.Scenes;

public sealed class SceneManager
{
	private readonly Dictionary<GameStateType, Func<IScene>> _sceneFactories = new Dictionary<GameStateType, Func<IScene>>();

	private SceneContext? _context;

	private IScene? _activeScene;

	private IScene? _overlayScene;

	private GameStateType? _pendingState;

	private readonly SceneTransitionController _transition = new SceneTransitionController();

	public SceneTransitionController Transition => _transition;

	public IScene? ActiveScene => _activeScene;

	public bool HasOverlay => _overlayScene != null;

	public void Initialize(SceneContext context)
	{
		_context = context;
	}

	public void Register(GameStateType state, Func<IScene> factory)
	{
		_sceneFactories[state] = factory;
	}

	public void ChangeTo(GameStateType state, bool useFade = true)
	{
		if (_context != null)
		{
			if (!useFade)
			{
				SwapScene(state);
				return;
			}
			_pendingState = state;
			_transition.BeginFadeOut();
		}
	}

	public void SetOverlay(IScene? overlay)
	{
		if (_overlayScene != null)
		{
			_overlayScene.Unload();
		}
		_overlayScene = overlay;
		if (_overlayScene != null && _context != null)
		{
			_overlayScene.Load(_context);
		}
	}

	public void ClearOverlay()
	{
		if (_overlayScene != null)
		{
			_overlayScene.Unload();
			_overlayScene = null;
		}
	}

	public void Update(GameTime gameTime)
	{
		float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
		GameStateType? pendingState = _pendingState;
		if ((pendingState.HasValue || _transition.IsActive) && _transition.Update(deltaSeconds))
		{
			pendingState = _pendingState;
			if (pendingState.HasValue)
			{
				GameStateType valueOrDefault = pendingState.GetValueOrDefault();
				if (true)
				{
					SwapScene(valueOrDefault);
					_pendingState = null;
				}
			}
		}
		_activeScene?.Update(gameTime);
		_overlayScene?.Update(gameTime);
	}

	public void Draw(GameTime gameTime)
	{
		_activeScene?.Draw(gameTime);
		_overlayScene?.Draw(gameTime);
		DrawFadeOverlay();
	}

	private void DrawFadeOverlay()
	{
		if (_context != null && _transition.IsActive)
		{
			byte alpha = (byte)(255f * _transition.Alpha);
			Color color = new Color((byte)8, (byte)10, (byte)18, alpha);
			_context.SpriteBatch.Draw(_context.Pixel, _context.Viewport.LetterboxBounds, color);
		}
	}

	private void SwapScene(GameStateType state)
	{
		if (_context != null && _sceneFactories.TryGetValue(state, out Func<IScene>? factory))
		{
			_activeScene?.Unload();
			_activeScene = factory();
			_activeScene.Load(_context);
			_context.StateMachine.TransitionTo(state);
			_context.Services.GetRequiredService<IAudioService>().OnSceneChanged(state);
		}
	}
}
