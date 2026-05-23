using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Core.Models.Settings;

namespace Satori.Client.Views.Rendering;

public sealed class DisplaySettingsApplier
{
	private Game? _game;

	private GraphicsDeviceManager? _graphics;

	private Action? _onSettingsChanged;

	private bool _f11WasDown;

	private bool _altEnterWasDown;

	private bool _applyingDisplayChanges;

	private int _windowedWidth = 1280;

	private int _windowedHeight = 720;

	public void Initialize(Game game, GraphicsDeviceManager graphics, Action? onSettingsChanged = null)
	{
		if (_game != null)
		{
			_game.Window.ClientSizeChanged -= OnClientSizeChanged;
		}

		_game = game;
		_graphics = graphics;
		_onSettingsChanged = onSettingsChanged;
		_windowedWidth = graphics.PreferredBackBufferWidth;
		_windowedHeight = graphics.PreferredBackBufferHeight;
		_game.Window.ClientSizeChanged += OnClientSizeChanged;
	}

	public void Apply(GameSettingsModel settings)
	{
		if (_game == null || _graphics == null)
		{
			return;
		}

		if (settings.IsFullscreen)
		{
			if (!_graphics.IsFullScreen)
			{
				_windowedWidth = _graphics.PreferredBackBufferWidth;
				_windowedHeight = _graphics.PreferredBackBufferHeight;
			}

			_game.Window.AllowUserResizing = false;
			_graphics.IsFullScreen = true;
		}
		else
		{
			_graphics.IsFullScreen = false;
			_graphics.PreferredBackBufferWidth = _windowedWidth;
			_graphics.PreferredBackBufferHeight = _windowedHeight;
			_game.Window.AllowUserResizing = true;
		}

		_game.IsMouseVisible = true;
		SdlNative.EnsureCursorVisible();
		ApplyChangesSafely();
	}

	public void SetFullscreen(GameSettingsModel settings, bool isFullscreen)
	{
		settings.IsFullscreen = isFullscreen;
		Apply(settings);
		_onSettingsChanged?.Invoke();
	}

	public void ToggleFullscreen(GameSettingsModel settings)
	{
		SetFullscreen(settings, !settings.IsFullscreen);
	}

	public void Update(KeyboardState keyboard, GameSettingsModel settings)
	{
		bool f11Down = keyboard.IsKeyDown(Keys.F11);
		if (WasEdgePressed(f11Down, _f11WasDown))
		{
			ToggleFullscreen(settings);
		}

		_f11WasDown = f11Down;
		bool altEnterDown = IsAltEnterDown(keyboard);
		if (WasEdgePressed(altEnterDown, _altEnterWasDown))
		{
			ToggleFullscreen(settings);
		}

		_altEnterWasDown = altEnterDown;
	}

	public static bool WasEdgePressed(bool isDown, bool wasDown) => isDown && !wasDown;

	public static bool IsAltEnterDown(KeyboardState keyboard)
	{
		bool alt = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
		return alt && keyboard.IsKeyDown(Keys.Enter);
	}

	private void OnClientSizeChanged(object? sender, EventArgs e)
	{
		if (_game == null || _graphics == null || _applyingDisplayChanges || _graphics.IsFullScreen)
		{
			return;
		}

		if (_game.Window.ClientBounds.Width > 0 && _game.Window.ClientBounds.Height > 0)
		{
			_windowedWidth = _game.Window.ClientBounds.Width;
			_windowedHeight = _game.Window.ClientBounds.Height;
			_graphics.PreferredBackBufferWidth = _windowedWidth;
			_graphics.PreferredBackBufferHeight = _windowedHeight;
			ApplyChangesSafely();
		}
	}

	private void ApplyChangesSafely()
	{
		if (_graphics == null || _applyingDisplayChanges)
		{
			return;
		}

		try
		{
			_applyingDisplayChanges = true;
			_graphics.ApplyChanges();
		}
		finally
		{
			_applyingDisplayChanges = false;
		}
	}
}
