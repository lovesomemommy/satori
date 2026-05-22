using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Controllers;
using Satori.Client.Input;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.UI.Framework;
using Satori.Client.Views.Minigames;
using Satori.Core.Models.Minigames;
using Satori.Core.Systems.Minigames;

namespace Satori.Client.Scenes.Minigames;

public sealed class MeditationTrainingScene : IScene
{
	private SceneContext? _context;

	private readonly UiScreen _screen = new UiScreen();

	private MeditationTrainingSystem? _training;

	private GameplayInputController? _input;

	private string _hintText = string.Empty;

	private KeyboardState _previousKeyboard;

	public void Load(SceneContext context)
	{
		_context = context;
		_training = context.Services.GetRequiredService<MeditationTrainingSystem>();
		_input = context.Services.GetRequiredService<GameplayInputController>();
		_training.Reset();
		_hintText = context.Localization.Get("minigames.meditation.training.hint");
		_previousKeyboard = default;
		BuildUi();
	}

	public void Unload()
	{
		_training?.Reset();
		_screen.Clear();
		_context = null;
		_training = null;
		_input = null;
	}

	public void Update(GameTime gameTime)
	{
		if (_context == null || _training == null || _input == null)
		{
			return;
		}

		float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
		var keyboard = Keyboard.GetState();
		var intent = _input.GetIntent(keyboard, _previousKeyboard);
		if (_training.State.Phase == MeditationPhase.Idle && intent.MeditateHold)
		{
			_training.TryBegin();
		}

		if (_training.State.IsActive)
		{
			_training.Update(delta, intent.MeditateHold);
			_hintText = GetPhaseLabel(_training.State.Phase);
		}
		else if (_training.State.Phase == MeditationPhase.Completed)
		{
			_hintText = _context.Localization.Get("minigames.meditation.training.complete");
			_training.Reset();
		}
		else if (_training.State.Phase == MeditationPhase.Interrupted)
		{
			_hintText = _context.Localization.Get("meditation.interrupted");
			_training.Reset();
		}
		else if (_training.State.Phase == MeditationPhase.Idle)
		{
			_hintText = _context.Localization.Get("minigames.meditation.training.hint");
		}

		var mouse = VirtualInput.ToVirtualMouse(_context.Viewport, Mouse.GetState());
		_screen.Update(gameTime, mouse, keyboard);
		_previousKeyboard = keyboard;
	}

	public void Draw(GameTime gameTime)
	{
		if (_context == null || _training == null)
		{
			return;
		}

		float glowPhase = UiAnimator.GlowPhase(gameTime);
		_context.SpriteBatch.Draw(_context.Pixel, new Rectangle(0, 0, _context.Viewport.VirtualWidth, _context.Viewport.VirtualHeight), new Color(14, 20, 26));
		MeditationTrainingView.Draw(
			_context.SpriteBatch,
			_context.Pixel,
			_training,
			glowPhase,
			_context.PilgrimSprites.MonkMeditating);
		var hintSize = _context.Text.MeasureText(_hintText);
		var hintPosition = new Vector2((_context.Viewport.VirtualWidth - hintSize.X) * 0.5f, 152f);
		_context.Text.DrawText(_context.SpriteBatch, _hintText, hintPosition, UiPalette.TextSecondary);
		_screen.Draw(_context.SpriteBatch, _context.Pixel, _context.Text, glowPhase);
	}

	private string GetPhaseLabel(MeditationPhase phase)
	{
		if (_context == null)
		{
			return string.Empty;
		}

		return phase switch
		{
			MeditationPhase.Inhale => _context.Localization.Get("minigames.meditation.training.inhale"),
			MeditationPhase.Hold => _context.Localization.Get("minigames.meditation.training.hold"),
			MeditationPhase.Exhale => _context.Localization.Get("minigames.meditation.training.exhale"),
			_ => _context.Localization.Get("minigames.meditation.training.hint")
		};
	}

	private void BuildUi()
	{
		if (_context == null)
		{
			return;
		}

		_screen.Clear();
		_screen.Add(new UiLabel
		{
			Text = _context.Localization.Get("minigames.meditation.title"),
			Bounds = new Rectangle(12, 8, 200, 14),
			Color = new Color(230, 220, 180)
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("hub.back"),
			Bounds = new Rectangle(230, 6, 78, 18),
			OnClick = () => _context.SceneManager.ChangeTo(GameStateType.MinigamesHub)
		});
	}
}
