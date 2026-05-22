using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.UI.Framework;

namespace Satori.Client.Scenes.Minigames;

public sealed class MinigamesHubScene : IScene
{
	private SceneContext? _context;

	private readonly UiScreen _screen = new UiScreen();

	public void Load(SceneContext context)
	{
		_context = context;
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

		var mouse = VirtualInput.ToVirtualMouse(_context.Viewport, Mouse.GetState());
		_screen.Update(gameTime, mouse, Keyboard.GetState());
	}

	public void Draw(GameTime gameTime)
	{
		if (_context == null)
		{
			return;
		}

		float glowPhase = UiAnimator.GlowPhase(gameTime);
		_context.SpriteBatch.Draw(_context.Pixel, new Rectangle(0, 0, _context.Viewport.VirtualWidth, _context.Viewport.VirtualHeight), UiPalette.Background);
		_screen.Draw(_context.SpriteBatch, _context.Pixel, _context.Text, glowPhase);
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
			Text = _context.Localization.Get("minigames.title"),
			Bounds = new Rectangle(12, 10, 200, 14)
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("hub.back"),
			Bounds = new Rectangle(230, 8, 78, 18),
			OnClick = () => _context.SceneManager.ChangeTo(GameStateType.Hub)
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("minigames.meditation.title"),
			Bounds = new Rectangle(12, 40, 200, 20),
			OnClick = () => _context.SceneManager.ChangeTo(GameStateType.MeditationTraining)
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("minigames.right_speech.title"),
			Bounds = new Rectangle(12, 68, 200, 20),
			OnClick = () => _context.SceneManager.ChangeTo(GameStateType.RightSpeech)
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("minigames.wheel.title"),
			Bounds = new Rectangle(12, 96, 200, 20),
			OnClick = () => _context.SceneManager.ChangeTo(GameStateType.WheelOfDharma)
		});
		_screen.Add(new UiLabel
		{
			Text = _context.Localization.Get("minigames.hub.hint"),
			Bounds = new Rectangle(12, 124, 296, 28),
			Color = UiPalette.TextMuted,
			WrapText = true,
			LineHeight = 11
		});
	}
}
