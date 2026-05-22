using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.UI.Framework;

namespace Satori.Client.Scenes;

public sealed class PauseOverlayScene : IScene
{
	private enum PendingAction
	{
		None,
		Resume,
		ReturnToHub
	}

	private SceneContext? _context;

	private readonly UiScreen _screen = new UiScreen();

	private PendingAction _pendingAction;

	private KeyboardState _previousKeyboard;

	public void Load(SceneContext context)
	{
		_context = context;
		_pendingAction = PendingAction.None;
		_previousKeyboard = Keyboard.GetState();
		BuildUi();
	}

	public void Unload()
	{
		_screen.Clear();
		_context = null;
	}

	public void Update(GameTime gameTime)
	{
		if (_context == null)
		{
			return;
		}

		var keyboard = Keyboard.GetState();
		if (KeyMapper.WasPausePressed(keyboard, _previousKeyboard, _context.Session.Settings.Bindings))
		{
			Resume();
		}

		_previousKeyboard = keyboard;
		var mouse = VirtualInput.ToVirtualMouse(_context.Viewport, Mouse.GetState());
		_screen.Update(gameTime, mouse, keyboard);
		if (_pendingAction != PendingAction.None)
		{
			ExecutePendingAction();
		}
	}

	public void Draw(GameTime gameTime)
	{
		if (_context == null)
		{
			return;
		}

		var glowPhase = UiAnimator.GlowPhase(gameTime);
		_context.SpriteBatch.Draw(
			_context.Pixel,
			new Rectangle(0, 0, _context.Viewport.VirtualWidth, _context.Viewport.VirtualHeight),
			new Color(0, 0, 0, 140));
		_screen.Draw(_context.SpriteBatch, _context.Pixel, _context.Text, glowPhase);
	}

	private void BuildUi()
	{
		if (_context == null)
		{
			return;
		}

		_screen.Clear();
		var panel = new UiPanel
		{
			Bounds = new Rectangle(72, 56, 176, 68),
			BackgroundColor = UiPalette.PanelDark
		};
		panel.Children.Add(new UiLabel
		{
			Text = _context.Localization.Get("pause.title"),
			Bounds = new Rectangle(96, 64, 120, 14),
			Color = UiPalette.TextPrimary
		});
		panel.Children.Add(new UiButton
		{
			Text = _context.Localization.Get("pause.resume"),
			Bounds = new Rectangle(100, 82, 120, 18),
			OnClick = Resume
		});
		panel.Children.Add(new UiButton
		{
			Text = _context.Localization.Get("pilgrim.return_hub"),
			Bounds = new Rectangle(100, 104, 120, 18),
			OnClick = ReturnToHub
		});
		_screen.Add(panel);
	}

	private void Resume()
	{
		_pendingAction = PendingAction.Resume;
	}

	private void ReturnToHub()
	{
		_pendingAction = PendingAction.ReturnToHub;
	}

	private void ExecutePendingAction()
	{
		if (_context == null)
		{
			return;
		}

		var action = _pendingAction;
		_pendingAction = PendingAction.None;
		var sceneManager = _context.SceneManager;
		var stateMachine = _context.StateMachine;
		sceneManager.ClearOverlay();

		switch (action)
		{
		case PendingAction.Resume:
			stateMachine.TransitionTo(GameStateType.PilgrimTrial);
			break;
		case PendingAction.ReturnToHub:
			sceneManager.ChangeTo(GameStateType.Hub);
			break;
		}
	}
}
