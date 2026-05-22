using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Client.Services.Audio;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.UI.Framework;
using Satori.Client.Views.Rendering;
using Satori.Core.Interfaces.Services;
using Satori.Core.Models.Input;

namespace Satori.Client.Scenes;

public sealed class SettingsScene : IScene
{
	private enum RebindTarget
	{
		None,
		Meditate,
		Pause
	}

	private SceneContext? _context;

	private readonly UiScreen _screen = new UiScreen();

	private readonly UiScreen _resetOverlay = new UiScreen();

	private UiLabel? _volumeLabel;

	private UiCheckbox? _muteCheckbox;

	private UiCheckbox? _fullscreenCheckbox;

	private UiButton? _meditateButton;

	private UiButton? _pauseButton;

	private RebindTarget _rebindTarget;

	private bool _resetConfirmVisible;

	private bool _awaitMouseReleaseAfterResetOpen;

	private int _resetOverlayCooldownFrames;

	private KeyboardState _previousKeyboard;

	private GameStateType _returnState = GameStateType.MainMenu;

	public void Load(SceneContext context)
	{
		_context = context;
		_rebindTarget = RebindTarget.None;
		_resetConfirmVisible = false;
		_awaitMouseReleaseAfterResetOpen = false;
		_resetOverlayCooldownFrames = 0;
		_previousKeyboard = Keyboard.GetState();
		_returnState = SettingsSceneStarter.ReturnState;
		BuildUi();
	}

	public void Unload()
	{
		_rebindTarget = RebindTarget.None;
		_resetConfirmVisible = false;
		_awaitMouseReleaseAfterResetOpen = false;
		_resetOverlayCooldownFrames = 0;
		_screen.Clear();
		_resetOverlay.Clear();
		_context = null;
	}

	public void Update(GameTime gameTime)
	{
		if (_context == null)
		{
			return;
		}

		var keyboard = Keyboard.GetState();
		if (!_resetConfirmVisible)
		{
			HandleRebind(keyboard);
		}
		else if (KeyMapper.WasPressed(keyboard, _previousKeyboard, "Escape"))
		{
			CancelResetProgress();
		}

		var mouse = VirtualInput.ToVirtualMouse(_context.Viewport, Mouse.GetState());
		if (_resetConfirmVisible)
		{
			_resetOverlayCooldownFrames = Math.Max(0, _resetOverlayCooldownFrames - 1);
			if (_awaitMouseReleaseAfterResetOpen)
			{
				if (mouse.LeftButton == ButtonState.Released)
				{
					_awaitMouseReleaseAfterResetOpen = false;
					_resetOverlay.ResetButtonPointerStates();
				}
			}
			else
			{
				_resetOverlay.Update(gameTime, mouse, keyboard);
			}
		}
		else
		{
			_screen.Update(gameTime, mouse, keyboard);
		}

		_previousKeyboard = keyboard;
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
		if (_resetConfirmVisible)
		{
			_context.SpriteBatch.Draw(_context.Pixel, new Rectangle(0, 0, _context.Viewport.VirtualWidth, _context.Viewport.VirtualHeight), new Color(0, 0, 0, 160));
			_resetOverlay.Draw(_context.SpriteBatch, _context.Pixel, _context.Text, glowPhase);
		}
	}

	private void HandleRebind(KeyboardState keyboard)
	{
		if (_context == null || _rebindTarget == RebindTarget.None)
		{
			return;
		}

		if (KeyMapper.WasPressed(keyboard, _previousKeyboard, "Escape"))
		{
			_rebindTarget = RebindTarget.None;
			RefreshLabels();
			return;
		}

		Keys[] pressed = keyboard.GetPressedKeys();
		foreach (Keys key in pressed)
		{
			if (key == Keys.Escape || !KeyMapper.TryFromKey(key, out string keyName))
			{
				continue;
			}

			InputBindingModel bindings = _context.Session.Settings.Bindings;
			switch (_rebindTarget)
			{
			case RebindTarget.Meditate:
				bindings.Meditate = keyName;
				break;
			case RebindTarget.Pause:
				bindings.Pause = keyName;
				break;
			}

			_context.Services.GetRequiredService<InputBindingService>().LoadBindings(bindings);
			_rebindTarget = RebindTarget.None;
			PersistSettings();
			RefreshLabels();
			return;
		}
	}

	private void BuildUi()
	{
		if (_context == null)
		{
			return;
		}

		_screen.Clear();
		_resetOverlay.Clear();
		_screen.Add(new UiLabel
		{
			Text = _context.Localization.Get("settings.title"),
			Bounds = new Rectangle(12, 8, 160, 14),
			Color = UiPalette.TextPrimary
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("hub.back"),
			Bounds = new Rectangle(230, 6, 78, 14),
			OnClick = () => _context.SceneManager.ChangeTo(_returnState)
		});
		_screen.Add(new UiDivider { Bounds = new Rectangle(12, 24, 296, 2) });
		_screen.Add(new UiLabel
		{
			Text = _context.Localization.Get("audio.title"),
			Bounds = new Rectangle(12, 30, 120, 11),
			Color = UiPalette.TextMuted
		});
		_volumeLabel = new UiLabel
		{
			Bounds = new Rectangle(12, 44, 160, 11),
			Color = UiPalette.TextSecondary
		};
		_screen.Add(_volumeLabel);
		_screen.Add(new UiButton
		{
			Text = "-",
			Bounds = new Rectangle(12, 58, 24, 14),
			OnClick = () => AdjustVolume(-0.1f)
		});
		_screen.Add(new UiButton
		{
			Text = "+",
			Bounds = new Rectangle(40, 58, 24, 14),
			OnClick = () => AdjustVolume(0.1f)
		});
		_muteCheckbox = new UiCheckbox
		{
			Label = _context.Localization.Get("settings.mute.label"),
			Bounds = new Rectangle(12, 76, 200, 12),
			OnChanged = SetMuted
		};
		_screen.Add(_muteCheckbox);
		_screen.Add(new UiDivider { Bounds = new Rectangle(12, 92, 296, 2) });
		_screen.Add(new UiLabel
		{
			Text = _context.Localization.Get("settings.display.title"),
			Bounds = new Rectangle(12, 98, 120, 11),
			Color = UiPalette.TextMuted
		});
		_fullscreenCheckbox = new UiCheckbox
		{
			Label = _context.Localization.Get("settings.fullscreen.label"),
			Bounds = new Rectangle(12, 112, 220, 12),
			OnChanged = SetFullscreen
		};
		_screen.Add(_fullscreenCheckbox);
		_screen.Add(new UiDivider { Bounds = new Rectangle(12, 128, 296, 2) });
		_screen.Add(new UiLabel
		{
			Text = _context.Localization.Get("settings.controls.title"),
			Bounds = new Rectangle(12, 134, 120, 11),
			Color = UiPalette.TextMuted
		});
		_meditateButton = new UiButton
		{
			Bounds = new Rectangle(12, 148, 144, 13),
			UseCompactFont = true,
			AlignLeft = true,
			OnClick = () => BeginRebind(RebindTarget.Meditate)
		};
		_pauseButton = new UiButton
		{
			Bounds = new Rectangle(160, 148, 148, 13),
			UseCompactFont = true,
			AlignLeft = true,
			OnClick = () => BeginRebind(RebindTarget.Pause)
		};
		_screen.Add(_meditateButton);
		_screen.Add(_pauseButton);
		_screen.Add(new UiDivider { Bounds = new Rectangle(12, 164, 296, 2) });
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("settings.reset.button"),
			Bounds = new Rectangle(12, 168, 160, 12),
			UseCompactFont = true,
			AlignLeft = true,
			OnClick = ShowResetConfirm
		});
		_resetOverlay.Add(new UiPanel
		{
			Bounds = new Rectangle(0, 0, 320, 180),
			BackgroundColor = Color.Transparent
		});
		_resetOverlay.Add(new UiPanel
		{
			Bounds = new Rectangle(28, 36, 264, 116),
			BackgroundColor = UiPalette.PanelDark
		});
		_resetOverlay.Add(new UiLabel
		{
			Text = _context.Localization.Get("settings.reset.warning"),
			Bounds = new Rectangle(40, 46, 240, 44),
			Color = UiPalette.PinkSoft,
			WrapText = true,
			LineHeight = 11
		});
		_resetOverlay.Add(new UiButton
		{
			Text = _context.Localization.Get("settings.reset.confirm"),
			Bounds = new Rectangle(40, 96, 216, 22),
			WrapText = true,
			UseCompactFont = true,
			AlignLeft = true,
			LineHeight = 8,
			OnClick = ConfirmResetProgress
		});
		_resetOverlay.Add(new UiButton
		{
			Text = _context.Localization.Get("settings.reset.cancel"),
			Bounds = new Rectangle(40, 122, 216, 18),
			UseCompactFont = true,
			AlignLeft = true,
			OnClick = CancelResetProgress
		});
		RefreshLabels();
	}

	private void RefreshLabels()
	{
		if (_context == null)
		{
			return;
		}

		var settings = _context.Session.Settings;
		int volumePercent = (int)Math.Round(settings.MasterVolume * 100f);
		_volumeLabel!.Text = string.Format(_context.Localization.Get("settings.volume.label"), volumePercent);
		_muteCheckbox!.IsChecked = settings.IsMuted;
		_fullscreenCheckbox!.IsChecked = settings.IsFullscreen;
		_meditateButton!.Text = _rebindTarget == RebindTarget.Meditate
			? _context.Localization.Get("settings.rebind.wait")
			: string.Format(_context.Localization.Get("settings.bind.meditate"), KeyMapper.FormatBindingLabel(settings.Bindings.Meditate));
		_pauseButton!.Text = _rebindTarget == RebindTarget.Pause
			? _context.Localization.Get("settings.rebind.wait")
			: string.Format(_context.Localization.Get("settings.bind.pause"), KeyMapper.FormatBindingLabel(settings.Bindings.Pause));
	}

	private void ShowResetConfirm()
	{
		_resetConfirmVisible = true;
		_awaitMouseReleaseAfterResetOpen = true;
		_resetOverlayCooldownFrames = 12;
		_resetOverlay.ResetButtonPointerStates();
		foreach (IUiElement element in _screen.Elements)
		{
			if (element is UiButton button)
			{
				button.ResetPointerState();
			}
		}
	}

	private void CancelResetProgress()
	{
		_resetConfirmVisible = false;
		_awaitMouseReleaseAfterResetOpen = false;
		_resetOverlayCooldownFrames = 0;
		_resetOverlay.ResetButtonPointerStates();
	}

	private void ConfirmResetProgress()
	{
		if (_context == null || _awaitMouseReleaseAfterResetOpen || _resetOverlayCooldownFrames > 0)
		{
			return;
		}

		var saveLoad = _context.Services.GetRequiredService<ISaveLoadService>();
		var fresh = saveLoad.ResetProgress(_context.Session.Save);
		_context.Session.ReplaceSave(fresh);
		_context.Services.GetRequiredService<InputBindingService>().LoadBindings(fresh.Settings.Bindings);
		_context.Services.GetRequiredService<DisplaySettingsApplier>().Apply(fresh.Settings);
		_context.Services.GetRequiredService<IAudioService>().ApplySettings(fresh.Settings);
		_resetConfirmVisible = false;
		_awaitMouseReleaseAfterResetOpen = false;
		_resetOverlayCooldownFrames = 0;
		_context.SceneManager.ChangeTo(GameStateType.MainMenu);
	}

	private void AdjustVolume(float delta)
	{
		if (_context == null)
		{
			return;
		}

		var settings = _context.Session.Settings;
		settings.MasterVolume = Math.Clamp(settings.MasterVolume + delta, 0f, 1f);
		ApplyAudioSettings();
		PersistSettings();
		RefreshLabels();
	}

	private void SetMuted(bool isMuted)
	{
		if (_context == null)
		{
			return;
		}

		_context.Session.Settings.IsMuted = isMuted;
		ApplyAudioSettings();
		PersistSettings();
		RefreshLabels();
	}

	private void SetFullscreen(bool isFullscreen)
	{
		if (_context == null)
		{
			return;
		}

		var applier = _context.Services.GetRequiredService<DisplaySettingsApplier>();
		applier.SetFullscreen(_context.Session.Settings, isFullscreen);
		PersistSettings();
		RefreshLabels();
	}

	private void BeginRebind(RebindTarget target)
	{
		_rebindTarget = target;
		RefreshLabels();
	}

	private void ApplyAudioSettings()
	{
		_context?.Services.GetRequiredService<IAudioService>().ApplySettings(_context.Session.Settings);
	}

	private void PersistSettings()
	{
		if (_context == null)
		{
			return;
		}

		_context.Services.GetRequiredService<ISaveLoadService>().SaveDefault(_context.Session.Save);
	}
}
