using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Client.Services.Menu;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.UI.Framework;

namespace Satori.Client.Scenes;

public sealed class MainMenuScene : IScene
{
	private const int VirtualWidth = 320;

	private const int ButtonWidth = 132;

	private const int ButtonHeight = 20;

	private const int ButtonGap = 12;

	private const int ButtonRowY = 150;

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
		if (_context != null)
		{
			MouseState mouse = VirtualInput.ToVirtualMouse(_context.Viewport, Mouse.GetState());
			KeyboardState state = Keyboard.GetState();
			_screen.Update(gameTime, mouse, state);
		}
	}

	public void Draw(GameTime gameTime)
	{
		if (_context != null)
		{
			float glowPhase = UiAnimator.GlowPhase(gameTime);
			_context.Services.GetRequiredService<MenuBackgroundCatalog>().Draw(
				_context.SpriteBatch,
				_context.Pixel,
				_context.Viewport.VirtualWidth,
				_context.Viewport.VirtualHeight);
			_screen.Draw(_context.SpriteBatch, _context.Pixel, _context.Text, glowPhase);
		}
	}

	private void BuildUi()
	{
		if (_context == null)
		{
			return;
		}

		_screen.Clear();
		int rowWidth = ButtonWidth * 2 + ButtonGap;
		int rowStartX = (VirtualWidth - rowWidth) / 2;

		_screen.Add(new UiLabel
		{
			Text = _context.Localization.Get("game.title"),
			Bounds = new Rectangle(0, 24, VirtualWidth, 18),
			UseTitleFont = true,
			CenterHorizontally = true,
			Color = new Color(58, 42, 82)
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("menu.start"),
			Bounds = new Rectangle(rowStartX, ButtonRowY, ButtonWidth, ButtonHeight),
			FillAlpha = 0.55f,
			UseCompactFont = true,
			OnClick = () => _context.SceneManager.ChangeTo(GameStateType.Hub)
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("settings.title"),
			Bounds = new Rectangle(rowStartX + ButtonWidth + ButtonGap, ButtonRowY, ButtonWidth, ButtonHeight),
			FillAlpha = 0.55f,
			OnClick = () => SettingsSceneStarter.Open(_context, GameStateType.MainMenu)
		});
	}
}
