using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Client.Services.Wisdom;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.UI.Framework;
using Satori.Client.Views.Lotus;
using Satori.Client.Views.Minigames;
using Satori.Core.Interfaces.Services;
using Satori.Core.Models.Minigames;
using Satori.Core.Services.Wisdom;
using Satori.Core.Systems.Minigames;
using Satori.Core.Systems.Progression;

namespace Satori.Client.Scenes.Minigames;

public sealed class WheelOfDharmaScene : IScene
{
	private SceneContext? _context;

	private readonly UiScreen _screen = new UiScreen();

	private readonly Random _random = new Random();

	private WheelOfDharmaSystem? _wheel;

	private QuoteImageCatalog? _quoteImages;

	private string _statusText = string.Empty;

	private string _hintText = string.Empty;

	private bool _shortenNextRound;

	private bool _showingLotusReveal;

	private float _lotusRevealProgress;

	private string _lotusRevealQuoteId = string.Empty;

	private int _pendingResultDifficulty;

	private bool _pendingResultFailed;

	private bool _directionPointerDown;

	private KeyboardState _previousKeyboard;

	public void Load(SceneContext context)
	{
		_context = context;
		_wheel = context.Services.GetRequiredService<WheelOfDharmaSystem>();
		_quoteImages = context.Services.GetRequiredService<QuoteImageCatalog>();
		_wheel.Reset();
		_statusText = string.Empty;
		_hintText = string.Empty;
		_shortenNextRound = false;
		_showingLotusReveal = false;
		_lotusRevealProgress = 0f;
		_lotusRevealQuoteId = string.Empty;
		_directionPointerDown = false;
		_previousKeyboard = default;
		BuildDifficultyUi();
	}

	public void Unload()
	{
		_wheel?.Reset();
		_screen.Clear();
		_context = null;
		_wheel = null;
		_quoteImages = null;
	}

	public void Update(GameTime gameTime)
	{
		if (_context == null || _wheel == null)
		{
			return;
		}

		float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
		var keyboard = Keyboard.GetState();
		if (_showingLotusReveal)
		{
			UpdateLotusReveal(delta, keyboard);
			_previousKeyboard = keyboard;
			return;
		}

		_wheel.Update(delta);
		UpdateHintText();
		HandleDirectionInput(keyboard);
		var mouse = VirtualInput.ToVirtualMouse(_context.Viewport, Mouse.GetState());
		HandleDirectionClick(new Point(mouse.X, mouse.Y), mouse.LeftButton);
		_screen.Update(gameTime, mouse, keyboard);
		_previousKeyboard = keyboard;
	}

	public void Draw(GameTime gameTime)
	{
		if (_context == null || _wheel == null)
		{
			return;
		}

		float glowPhase = UiAnimator.GlowPhase(gameTime);
		_context.SpriteBatch.Draw(_context.Pixel, new Rectangle(0, 0, _context.Viewport.VirtualWidth, _context.Viewport.VirtualHeight), UiPalette.Background);

		if (_showingLotusReveal)
		{
			float progress = Math.Min(1f, _lotusRevealProgress);
			Texture2D? quoteImage = _quoteImages?.GetQuoteImage(_lotusRevealQuoteId);
			LotusRevealView.DrawCentered(
				_context.SpriteBatch,
				_context.Pixel,
				_context.Text,
				progress,
				_context.Viewport.VirtualWidth,
				_context.Viewport.VirtualHeight,
				quoteImage,
				_context.PilgrimSprites.Lotus,
				_context.Localization.Get("meditation.quote.dismiss"));
			return;
		}

		_screen.Draw(_context.SpriteBatch, _context.Pixel, _context.Text, glowPhase);

		if (_wheel.State.Phase != WheelOfDharmaPhase.Idle)
		{
			string? headlineLabel = _wheel.State.Phase == WheelOfDharmaPhase.Showing
				? string.Format(
					_context.Localization.Get("minigames.wheel.step"),
					Math.Min(_wheel.State.ShowIndex + 1, _wheel.State.Sequence.Count),
					_wheel.State.Sequence.Count)
				: null;
			WheelOfDharmaView.DrawPad(
				_context.SpriteBatch,
				_context.Pixel,
				_wheel.State,
				glowPhase,
				_context.Text,
				headlineLabel);
		}

		if (!string.IsNullOrEmpty(_hintText) && _wheel.State.Phase != WheelOfDharmaPhase.Showing)
		{
			var hintSize = _context.Text.MeasureText(_hintText);
			var hintPosition = new Vector2((_context.Viewport.VirtualWidth - hintSize.X) * 0.5f, 156f);
			_context.Text.DrawText(_context.SpriteBatch, _hintText, hintPosition, UiPalette.TextSecondary);
		}

		if (!string.IsNullOrEmpty(_statusText))
		{
			var banner = new Rectangle(12, 128, 296, 18);
			_context.SpriteBatch.Draw(_context.Pixel, banner, UiPalette.PanelDark);
			_context.Text.DrawText(_context.SpriteBatch, _statusText, new Vector2(banner.X + 6, banner.Y + 3), UiPalette.PinkSoft);
		}
	}

	private void UpdateLotusReveal(float delta, KeyboardState keyboard)
	{
		if (_lotusRevealProgress < 1f)
		{
			_lotusRevealProgress = Math.Min(1f, _lotusRevealProgress + delta / LotusRevealView.ExpandDurationSeconds);
		}

		if (keyboard.IsKeyDown(Keys.Escape) && !_previousKeyboard.IsKeyDown(Keys.Escape))
		{
			CloseLotusReveal();
		}
	}

	private void CloseLotusReveal()
	{
		_showingLotusReveal = false;
		_lotusRevealProgress = 0f;
		_lotusRevealQuoteId = string.Empty;
		BuildResultUi(_pendingResultDifficulty, _pendingResultFailed);
	}

	private void UpdateHintText()
	{
		if (_context == null || _wheel == null)
		{
			return;
		}

		_hintText = _wheel.State.Phase switch
		{
			WheelOfDharmaPhase.Input => _context.Localization.Get("minigames.wheel.follow"),
			_ => string.Empty
		};
	}

	private void HandleDirectionInput(KeyboardState keyboard)
	{
		if (_wheel == null || _wheel.State.Phase != WheelOfDharmaPhase.Input)
		{
			return;
		}

		if (WasKeyPressed(keyboard, _previousKeyboard, Keys.Up))
		{
			ProcessInput(WheelDirection.Up);
		}
		else if (WasKeyPressed(keyboard, _previousKeyboard, Keys.Down))
		{
			ProcessInput(WheelDirection.Down);
		}
		else if (WasKeyPressed(keyboard, _previousKeyboard, Keys.Left))
		{
			ProcessInput(WheelDirection.Left);
		}
		else if (WasKeyPressed(keyboard, _previousKeyboard, Keys.Right))
		{
			ProcessInput(WheelDirection.Right);
		}
	}

	private static bool WasKeyPressed(KeyboardState current, KeyboardState previous, Keys key) =>
		current.IsKeyDown(key) && !previous.IsKeyDown(key);

	private void HandleDirectionClick(Point mousePoint, ButtonState leftButton)
	{
		if (_wheel == null || _wheel.State.Phase != WheelOfDharmaPhase.Input)
		{
			if (leftButton == ButtonState.Released)
			{
				_directionPointerDown = false;
			}

			return;
		}

		foreach (WheelDirection direction in Enum.GetValues<WheelDirection>())
		{
			if (!WheelOfDharmaView.GetDirectionButtonBounds(direction).Contains(mousePoint))
			{
				continue;
			}

			if (leftButton == ButtonState.Pressed)
			{
				_directionPointerDown = true;
			}

			if (_directionPointerDown && leftButton == ButtonState.Released)
			{
				ProcessInput(direction);
				_directionPointerDown = false;
			}

			return;
		}

		if (leftButton == ButtonState.Released)
		{
			_directionPointerDown = false;
		}
	}

	private void ProcessInput(WheelDirection direction)
	{
		if (_context == null || _wheel == null)
		{
			return;
		}

		WheelInputResult result = _wheel.TryInput(direction);
		if (result == WheelInputResult.Advanced)
		{
			return;
		}

		int difficulty = _wheel.State.Difficulty;
		if (result == WheelInputResult.Completed)
		{
			var outcome = _wheel.ApplySuccess(_context.Session.Meta, _context.Session.Save.Precepts);
			_statusText = string.Format(_context.Localization.Get("minigames.wheel.success"), outcome.KarmaDelta);
			_shortenNextRound = false;
			var lotusOutcome = WheelLotusRewardSystem.TryGrantFirstClearReward(
				difficulty,
				_context.Session.Meta,
				_context.Session.Save.Precepts,
				_context.Session.Save.Wisdom,
				_context.Services.GetRequiredService<LotusCatalog>(),
				_context.Services.GetRequiredService<QuoteCatalog>(),
				_context.Services.GetRequiredService<GardenPlantingSystem>(),
				DateTimeOffset.UtcNow);
			SaveProgress();
			if (lotusOutcome.Granted)
			{
				BeginLotusReveal(lotusOutcome.QuoteId, difficulty, failed: false);
				return;
			}

			_wheel.Reset();
			BuildResultUi(difficulty, failed: false);
			return;
		}

		if (result == WheelInputResult.Failed)
		{
			var outcome = _wheel.ApplyFailure(_context.Session.Meta, _context.Session.Save.Precepts);
			_statusText = string.Format(_context.Localization.Get("minigames.wheel.fail"), Math.Abs(outcome.KarmaDelta));
			_shortenNextRound = true;
			SaveProgress();
			_wheel.Reset();
			BuildResultUi(difficulty, failed: true);
		}
	}

	private void BeginLotusReveal(string quoteId, int difficulty, bool failed)
	{
		_pendingResultDifficulty = difficulty;
		_pendingResultFailed = failed;
		_lotusRevealQuoteId = quoteId;
		_lotusRevealProgress = 0f;
		_showingLotusReveal = true;
		_wheel?.Reset();
		_screen.Clear();
	}

	private void StartRound(int difficulty)
	{
		if (_context == null || _wheel == null)
		{
			return;
		}

		_statusText = string.Empty;
		_showingLotusReveal = false;
		_directionPointerDown = false;
		_wheel.StartRound(difficulty, _random, _shortenNextRound);
		_shortenNextRound = false;
		BuildPlayingUi();
	}

	private void BuildDifficultyUi()
	{
		if (_context == null)
		{
			return;
		}

		_screen.Clear();
		_screen.Add(new UiLabel
		{
			Text = _context.Localization.Get("minigames.wheel.title"),
			Bounds = new Rectangle(12, 8, 200, 14),
			Color = new Color(230, 220, 180)
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("hub.back"),
			Bounds = new Rectangle(230, 6, 78, 18),
			OnClick = () => _context.SceneManager.ChangeTo(GameStateType.MinigamesHub)
		});

		int unlocked = _context.Session.Save.Precepts.WheelHighestDifficultyUnlocked;
		for (int difficulty = 1; difficulty <= WheelOfDharmaSystem.MaxDifficulty; difficulty++)
		{
			int level = difficulty;
			bool isLocked = level > unlocked;
			if (isLocked)
			{
				_screen.Add(new UiLabel
				{
					Text = string.Format(_context.Localization.Get("minigames.wheel.difficulty"), level) + " *",
					Bounds = new Rectangle(16, 36 + (level - 1) * 22, 200, 14),
					Color = new Color(110, 115, 120)
				});
				continue;
			}

			_screen.Add(new UiButton
			{
				Text = string.Format(_context.Localization.Get("minigames.wheel.difficulty"), level),
				Bounds = new Rectangle(12, 34 + (level - 1) * 22, 200, 18),
				OnClick = () => StartRound(level)
			});
		}
	}

	private void BuildPlayingUi()
	{
		if (_context == null)
		{
			return;
		}

		_screen.Clear();
		_screen.Add(new UiLabel
		{
			Text = string.Format(_context.Localization.Get("minigames.wheel.difficulty"), _wheel!.State.Difficulty),
			Bounds = new Rectangle(12, 8, 200, 14),
			Color = new Color(230, 220, 180)
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("hub.back"),
			Bounds = new Rectangle(230, 6, 78, 18),
			OnClick = () =>
			{
				_wheel?.Reset();
				BuildDifficultyUi();
			}
		});
	}

	private void BuildResultUi(int difficulty, bool failed)
	{
		if (_context == null || _wheel == null)
		{
			return;
		}

		_wheel.Reset();
		_screen.Clear();
		_screen.Add(new UiLabel
		{
			Text = _context.Localization.Get("minigames.wheel.title"),
			Bounds = new Rectangle(12, 8, 200, 14),
			Color = new Color(230, 220, 180)
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("hub.back"),
			Bounds = new Rectangle(230, 6, 78, 18),
			OnClick = () => _context.SceneManager.ChangeTo(GameStateType.MinigamesHub)
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("pilgrim.retry"),
			Bounds = new Rectangle(12, 152, 120, 18),
			OnClick = () => StartRound(difficulty)
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("minigames.wheel.choose_level"),
			Bounds = new Rectangle(140, 152, 120, 18),
			OnClick = () =>
			{
				_shortenNextRound = failed;
				_statusText = string.Empty;
				BuildDifficultyUi();
			}
		});
	}

	private void SaveProgress()
	{
		if (_context == null)
		{
			return;
		}

		_context.Services.GetRequiredService<ISaveLoadService>().SaveDefault(_context.Session.Save);
	}
}
