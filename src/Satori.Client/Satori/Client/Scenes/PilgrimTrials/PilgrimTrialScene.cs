using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Content;
using Satori.Client.Controllers;
using Satori.Client.Input;
using Satori.Client.Scenes;
using Satori.Client.Services.Wisdom;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.UI.Framework;
using Satori.Client.Views.Lotus;
using Satori.Client.Views.PilgrimTrials;
using Satori.Core.Interfaces.Events;
using Satori.Core.Interfaces.Events.Events;
using Satori.Core.Interfaces.Services;
using Satori.Core.Models.Input;
using Satori.Core.Models.Lotus;
using Satori.Core.Models.Minigames;
using Satori.Core.Models.PilgrimTrials;
using Satori.Core.Services.Wisdom;
using Satori.Core.Systems.Lotus;
using Satori.Core.Systems.PilgrimTrials;
using Satori.Core.Systems.Precepts;
using Satori.Core.Systems.Wisdom;
using Satori.Core.Utilities;

namespace Satori.Client.Scenes.PilgrimTrials;

public sealed class PilgrimTrialScene : IScene
{
	private enum PilgrimageUiMode
	{
		Playing,
		Meditating,
		ShowingLotusReveal,
		Defeat,
		Success
	}

	private const float MoveCooldownSeconds = 0.12f;

	private const float PreceptHintDurationSeconds = 3.2f;

	private const float DecoyHintDurationSeconds = 2f;

	private const float InterruptedHintDurationSeconds = 1.4f;

	private SceneContext? _context;

	private PilgrimPilgrimageSystem? _pilgrimage;

	private PreceptViolationSystem? _preceptSystem;

	private LotusCollectionSystem? _lotusCollection;

	private MeditationSystem? _meditation;

	private QuoteUnlockSystem? _quoteUnlock;

	private QuoteCatalog? _quoteCatalog;

	private QuoteImageCatalog? _quoteImages;

	private IInputController? _inputController;

	private KeyboardState _previousKeyboard;

	private int _monkTileX;

	private int _monkTileY;

	private float _moveCooldown;

	private string _segmentTitleText = string.Empty;

	private float _preceptHintTimer;

	private string _preceptHintText = string.Empty;

	private float _decoyHintTimer;

	private bool _resultHandled;

	private float _lotusRevealProgress;

	private string _lotusRevealQuoteId = string.Empty;

	private int _lotusRevealOriginTileX;

	private int _lotusRevealOriginTileY;

	private float _interruptedHintTimer;

	private float _portalLockedHintTimer;

	private PilgrimageUiMode _uiMode = PilgrimageUiMode.Playing;

	private readonly UiScreen _resultScreen = new UiScreen();

	private bool _pauseButtonHovered;

	private bool _pausePointerDown;

	public void Load(SceneContext context)
	{
		_context = context;
		_pilgrimage = context.Services.GetRequiredService<PilgrimPilgrimageSystem>();
		_preceptSystem = context.Services.GetRequiredService<PreceptViolationSystem>();
		_lotusCollection = context.Services.GetRequiredService<LotusCollectionSystem>();
		_meditation = context.Services.GetRequiredService<MeditationSystem>();
		_quoteUnlock = context.Services.GetRequiredService<QuoteUnlockSystem>();
		_quoteCatalog = context.Services.GetRequiredService<QuoteCatalog>();
		_quoteImages = context.Services.GetRequiredService<QuoteImageCatalog>();
		_inputController = context.Services.GetRequiredService<IInputController>();
		_previousKeyboard = Keyboard.GetState();
		IGameEventBus gameEventBus = (IGameEventBus)context.Services.GetRequiredService(typeof(IGameEventBus));
		gameEventBus.Subscribe<SegmentCompletedEvent>(OnSegmentCompleted);
		gameEventBus.Subscribe<DecoyTrailEnteredEvent>(OnDecoyTrailEntered);
		_resultHandled = false;
		_lotusRevealProgress = 0f;
		_lotusRevealQuoteId = string.Empty;
		_preceptHintTimer = 0f;
		_preceptHintText = string.Empty;
		_decoyHintTimer = 0f;
		_interruptedHintTimer = 0f;
		_portalLockedHintTimer = 0f;
		_meditation?.Reset();
		_uiMode = PilgrimageUiMode.Playing;
		_resultScreen.Clear();
		_pauseButtonHovered = false;
		_pausePointerDown = false;
		ResetMonkToCurrentSegment();
		ShowSegmentBanner();
	}

	public void Unload()
	{
		if (_context != null)
		{
			IGameEventBus gameEventBus = (IGameEventBus)_context.Services.GetRequiredService(typeof(IGameEventBus));
			gameEventBus.Unsubscribe<SegmentCompletedEvent>(OnSegmentCompleted);
			gameEventBus.Unsubscribe<DecoyTrailEnteredEvent>(OnDecoyTrailEntered);
		}
		_resultScreen.Clear();
		_context = null;
		_pilgrimage = null;
		_lotusCollection = null;
		_meditation = null;
		_quoteUnlock = null;
		_quoteCatalog = null;
		_preceptSystem = null;
		_quoteImages = null;
		_inputController = null;
	}

	public void Update(GameTime gameTime)
	{
		if (_context == null || _pilgrimage == null || _inputController == null)
		{
			return;
		}

		if (_context.SceneManager.HasOverlay)
		{
			_previousKeyboard = Keyboard.GetState();
			_pausePointerDown = false;
			return;
		}

		float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
		_preceptHintTimer = Math.Max(0f, _preceptHintTimer - num);
		_decoyHintTimer = Math.Max(0f, _decoyHintTimer - num);
		_interruptedHintTimer = Math.Max(0f, _interruptedHintTimer - num);
		_portalLockedHintTimer = Math.Max(0f, _portalLockedHintTimer - num);
		if (_uiMode == PilgrimageUiMode.Playing || _uiMode == PilgrimageUiMode.Meditating)
		{
			_pilgrimage.Update(TimeSpan.FromSeconds(num));
			UpdatePlaying(num);
			CheckRunOutcome();
		}
		else if (_uiMode == PilgrimageUiMode.ShowingLotusReveal)
		{
			UpdateLotusReveal(num);
			CheckRunOutcome();
		}
		else
		{
			MouseState mouse = VirtualInput.ToVirtualMouse(_context.Viewport, Mouse.GetState());
			_resultScreen.Update(gameTime, mouse, Keyboard.GetState());
		}
	}

	public void Draw(GameTime gameTime)
	{
		if (_context == null || _pilgrimage == null)
		{
			return;
		}

		TrialSegmentDefinition? currentSegment = _pilgrimage.GetCurrentSegment();
		if (currentSegment == null)
		{
			return;
		}

		float glowPhase = UiAnimator.GlowPhase(gameTime);
		double totalSeconds = gameTime.TotalGameTime.TotalSeconds;
		_context.SpriteBatch.Draw(_context.Pixel, new Rectangle(0, 0, _context.Viewport.VirtualWidth, _context.Viewport.VirtualHeight), new Color(14, 18, 24));
		SegmentLayoutView.Draw(_context.SpriteBatch, _context.Pixel, _context.ObstacleSprites, _context.PilgrimSprites, currentSegment, totalSeconds, _pilgrimage.Run.CollectedLotusIds);
		LotusModel? lotusAtMonk = GetLotusAtMonk();
		if (_meditation?.State.IsActive == true && lotusAtMonk != null)
		{
			MeditationView.DrawLotusGlow(
				_context.SpriteBatch,
				_context.Pixel,
				_context.PilgrimSprites.Lotus,
				lotusAtMonk.TileX,
				lotusAtMonk.TileY,
				_meditation.GetGlowStrength());
		}
		if (_uiMode != PilgrimageUiMode.ShowingLotusReveal && _meditation != null)
		{
			MeditationView.DrawMonkBreath(
				_context.SpriteBatch,
				_context.Pixel,
				_context.PilgrimSprites.MonkWalking,
				_context.PilgrimSprites.MonkMeditating,
				_monkTileX,
				_monkTileY,
				_meditation.GetBreathScale(),
				_meditation.State.Phase);
		}
		bool isLow = _pilgrimage.Run.RemainingTime <= TimeSpan.FromSeconds(30.0);
		bool showPauseButton = _uiMode == PilgrimageUiMode.Playing || _uiMode == PilgrimageUiMode.Meditating;
		TrialTimerView.Draw(
			_context.SpriteBatch,
			_context.Pixel,
			_context.Text,
			_pilgrimage.Run.RemainingTime,
			isLow,
			_context.Viewport.VirtualWidth,
			showPauseButton && _pauseButtonHovered);
		if (_uiMode == PilgrimageUiMode.ShowingLotusReveal)
		{
			float progress = Math.Min(1f, _lotusRevealProgress);
			Texture2D? quoteImage = _quoteImages?.GetQuoteImage(_lotusRevealQuoteId);
			LotusRevealView.Draw(
				_context.SpriteBatch,
				_context.Pixel,
				_context.Text,
				progress,
				_lotusRevealOriginTileX,
				_lotusRevealOriginTileY,
				_context.Viewport.VirtualWidth,
				_context.Viewport.VirtualHeight,
				quoteImage,
				_context.PilgrimSprites.Lotus,
				_context.Localization.Get("meditation.quote.dismiss"));
		}
		else if (lotusAtMonk != null && !_pilgrimage.Run.CollectedLotusIds.Contains(lotusAtMonk.Id) && _uiMode == PilgrimageUiMode.Playing)
		{
			DrawMeditationHint();
		}
		else 		if (_interruptedHintTimer > 0f)
		{
			DrawInterruptedHint();
		}
		if (_uiMode == PilgrimageUiMode.Playing || _uiMode == PilgrimageUiMode.Meditating)
		{
			DrawSegmentTitle();
		}
		if (_preceptHintTimer > 0f)
		{
			DrawPreceptHint();
		}
		if (_decoyHintTimer > 0f)
		{
			DrawDecoyHint();
		}
		if (_portalLockedHintTimer > 0f)
		{
			DrawPortalLockedHint();
		}
		PilgrimageUiMode uiMode = _uiMode;
		if ((uint)(uiMode - 3) <= 1u)
		{
			_context.SpriteBatch.Draw(_context.Pixel, new Rectangle(0, 0, _context.Viewport.VirtualWidth, _context.Viewport.VirtualHeight), new Color((byte)0, (byte)0, (byte)0, (byte)150));
			_resultScreen.Draw(_context.SpriteBatch, _context.Pixel, _context.Text, glowPhase);
		}
	}

	private void UpdateLotusReveal(float delta)
	{
		if (_inputController == null)
		{
			return;
		}

		if (_lotusRevealProgress < 1f)
		{
			_lotusRevealProgress = Math.Min(1f, _lotusRevealProgress + delta / 0.22f);
		}
		KeyboardState state = Keyboard.GetState();
		PlayerIntent intent = _inputController.GetIntent(state, _previousKeyboard);
		_previousKeyboard = state;
		if (intent.Pause)
		{
			CloseLotusReveal();
		}
	}

	private void CloseLotusReveal()
	{
		_uiMode = PilgrimageUiMode.Playing;
		_lotusRevealQuoteId = string.Empty;
		_lotusRevealProgress = 0f;
	}

	private void UpdatePlaying(float delta)
	{
		if (_context == null || _inputController == null)
		{
			return;
		}

		KeyboardState state = Keyboard.GetState();
		PlayerIntent intent = _inputController.GetIntent(state, _previousKeyboard);
		_previousKeyboard = state;
		HandlePauseButtonClick();
		if (intent.Pause && !_context.SceneManager.HasOverlay)
		{
			OpenPause();
			return;
		}
		if (HandleMeditation(delta, intent))
		{
			return;
		}
		_moveCooldown -= delta;
		if (!(_moveCooldown > 0f) && !intent.Move.IsZero)
		{
			int num = ((intent.Move.X > 0.25f) ? 1 : ((intent.Move.X < -0.25f) ? (-1) : 0));
			int num2 = ((intent.Move.Y > 0.25f) ? 1 : ((intent.Move.Y < -0.25f) ? (-1) : 0));
			if (num != 0 || num2 != 0)
			{
				TryMove(num, num2);
			}
		}
	}

	private void HandlePauseButtonClick()
	{
		if (_context == null || _context.SceneManager.HasOverlay)
		{
			_pauseButtonHovered = false;
			return;
		}

		if (_uiMode != PilgrimageUiMode.Playing && _uiMode != PilgrimageUiMode.Meditating)
		{
			_pauseButtonHovered = false;
			return;
		}

		var mouse = VirtualInput.ToVirtualMouse(_context.Viewport, Mouse.GetState());
		var pauseBounds = TrialTimerView.GetPauseButtonBounds(_context.Viewport.VirtualWidth);
		var mousePoint = new Point(mouse.X, mouse.Y);
		_pauseButtonHovered = pauseBounds.Contains(mousePoint);
		if (UiClickHelper.TryHandleClick(pauseBounds, mousePoint, mouse.LeftButton, ref _pausePointerDown))
		{
			OpenPause();
		}
	}

	private void OpenPause()
	{
		if (_context == null || _context.SceneManager.HasOverlay)
		{
			return;
		}

		_meditation?.Reset();
		_uiMode = PilgrimageUiMode.Playing;
		_pausePointerDown = false;
		_context.SceneManager.SetOverlay(new PauseOverlayScene());
		_context.StateMachine.TransitionTo(GameStateType.PauseOverlay);
	}

	private void TryMove(int deltaX, int deltaY)
	{
		if (_pilgrimage == null)
		{
			return;
		}

		TrialSegmentDefinition? currentSegment = _pilgrimage.GetCurrentSegment();
		if (currentSegment != null)
		{
			int num = _monkTileX + deltaX;
			int num2 = _monkTileY + deltaY;
			if (SegmentNavigation.IsWalkable(currentSegment, num, num2))
			{
				_monkTileX = num;
				_monkTileY = num2;
				_moveCooldown = 0.12f;
				OnTileEntered(currentSegment);
			}
		}
	}

	private void OnTileEntered(TrialSegmentDefinition segment)
	{
		_pilgrimage!.OnPlayerEnteredTile(_monkTileX, _monkTileY);
		if (!_pilgrimage.Run.IsActive || !SegmentNavigation.IsPortal(segment, _monkTileX, _monkTileY))
		{
			return;
		}

		if (!AreAllSegmentLotusesCollected(segment))
		{
			_portalLockedHintTimer = 2f;
			return;
		}

		_pilgrimage.OnPlayerReachedExitPortal();
		if (_pilgrimage.Run.IsActive)
		{
			ResetMonkToCurrentSegment();
			ShowSegmentBanner();
		}
	}

	private bool AreAllSegmentLotusesCollected(TrialSegmentDefinition segment)
	{
		if (_pilgrimage == null)
		{
			return false;
		}

		foreach (LotusModel lotus in segment.Lotuses)
		{
			if (!_pilgrimage.Run.CollectedLotusIds.Contains(lotus.Id))
			{
				return false;
			}
		}

		return true;
	}

	private void CheckRunOutcome()
	{
		if (!_resultHandled && _pilgrimage != null && _context != null)
		{
			switch (_pilgrimage.Run.Outcome)
			{
			case TrialOutcome.Defeat:
				_resultHandled = true;
				ShowDefeatUi();
				break;
			case TrialOutcome.Success:
				_resultHandled = true;
				SaveProgress();
				ShowSuccessUi();
				break;
			}
		}
	}

	private void ShowDefeatUi()
	{
		if (_context == null)
		{
			return;
		}

		_uiMode = PilgrimageUiMode.Defeat;
		string key = _pilgrimage?.Run.LastDefeatMessageKey ?? "pilgrim.defeat";
		BuildResultUi(_context.Localization.Get(key), StartNewPilgrimage, () =>
		{
			_context.SceneManager.ChangeTo(GameStateType.Hub);
		});
	}

	private void ShowSuccessUi()
	{
		if (_context == null)
		{
			return;
		}

		_uiMode = PilgrimageUiMode.Success;
		BuildResultUi(_context.Localization.Get("pilgrim.success"), null, () =>
		{
			_context.SceneManager.ChangeTo(GameStateType.Hub);
		});
	}

	private void BuildResultUi(string message, Action? retry, Action leave)
	{
		if (_context == null)
		{
			return;
		}

		_resultScreen.Clear();
		UiPanel element = new UiPanel
		{
			Bounds = new Rectangle(36, 44, 248, 98),
			BackgroundColor = new Color(24, 30, 42, 240)
		};
		UiLabel element2 = new UiLabel
		{
			Text = message,
			Bounds = new Rectangle(46, 52, 228, 36),
			Color = new Color(230, 220, 180),
			WrapText = true,
			LineHeight = 10
		};
		_resultScreen.Add(element);
		_resultScreen.Add(element2);
		if (retry != null)
		{
			_resultScreen.Add(new UiButton
			{
				Text = _context.Localization.Get("pilgrim.retry"),
				Bounds = new Rectangle(44, 94, 152, 18),
				UseCompactFont = true,
				OnClick = retry
			});
		}

		int hubX = retry == null ? 118 : 200;
		_resultScreen.Add(new UiButton
		{
			Text = _context.Localization.Get("pilgrim.return_hub"),
			Bounds = new Rectangle(hubX, 94, 80, 18),
			OnClick = leave
		});
	}

	private void StartNewPilgrimage()
	{
		if (_context != null && _pilgrimage != null)
		{
			_pilgrimage.BindPersistence(_context.Session.Meta, _context.Session.Save.Pilgrimage, _context.Session.Save.Wisdom);
			_pilgrimage.Start(PilgrimageContentFactory.Create());
			_meditation?.Reset();
			_resultHandled = false;
			_lotusRevealProgress = 0f;
			_lotusRevealQuoteId = string.Empty;
			_uiMode = PilgrimageUiMode.Playing;
			_resultScreen.Clear();
			ResetMonkToCurrentSegment();
			ShowSegmentBanner();
		}
	}

	private void SaveProgress()
	{
		if (_context == null)
		{
			return;
		}

		_context.PersistSave();
	}

	private void ResetMonkToCurrentSegment()
	{
		TrialSegmentDefinition? trialSegmentDefinition = _pilgrimage?.GetCurrentSegment();
		if (trialSegmentDefinition != null)
		{
			_monkTileX = trialSegmentDefinition.Spawn.X;
			_monkTileY = trialSegmentDefinition.Spawn.Y;
		}
	}

	private void ShowSegmentBanner()
	{
		TrialSegmentDefinition? trialSegmentDefinition = _pilgrimage?.GetCurrentSegment();
		if (trialSegmentDefinition != null && _context != null)
		{
			_segmentTitleText = _context.Localization.Get(trialSegmentDefinition.TitleKey);
			string? hintKey = _preceptSystem?.GetHintKey(trialSegmentDefinition);
			if (!string.IsNullOrWhiteSpace(hintKey) && _context.Localization.TryGet(hintKey, out var value))
			{
				_preceptHintText = value;
				_preceptHintTimer = 3.2f;
			}
			else
			{
				_preceptHintText = string.Empty;
				_preceptHintTimer = 0f;
			}
		}
	}

	private void DrawPreceptHint()
	{
		if (_context == null)
		{
			return;
		}

		var destinationRectangle = new Rectangle(8, 22, 200, 20);
		_context.Text.DrawWrappedText(
			_context.SpriteBatch,
			_preceptHintText,
			destinationRectangle,
			UiPalette.PinkSoft,
			8,
			compact: true);
	}

	private void DrawDecoyHint()
	{
		if (_context == null)
		{
			return;
		}

		string text = _context.Localization.Get("precept.decoy_trails.warning");
		_context.Text.DrawWrappedText(
			_context.SpriteBatch,
			text,
			new Rectangle(6, 158, 308, 18),
			UiPalette.TextSecondary,
			8,
			compact: true);
	}

	private void OnDecoyTrailEntered(DecoyTrailEnteredEvent evt)
	{
		_decoyHintTimer = 2f;
	}

	private void DrawSegmentTitle()
	{
		if (_context == null || string.IsNullOrEmpty(_segmentTitleText))
		{
			return;
		}

		_context.Text.DrawText(
			_context.SpriteBatch,
			_segmentTitleText,
			new Vector2(8, 8),
			UiPalette.PinkSoft,
			compact: true);
	}

	private void OnSegmentCompleted(SegmentCompletedEvent evt)
	{
		ShowSegmentBanner();
	}

	private bool HandleMeditation(float delta, PlayerIntent intent)
	{
		if (_pilgrimage == null || _meditation == null || _lotusCollection == null || _quoteUnlock == null || _quoteCatalog == null)
		{
			return false;
		}
		LotusModel? lotusModel = GetLotusAtMonk();
		TrialRunState run = _pilgrimage.Run;
		if (_meditation.State.Phase == MeditationPhase.Completed)
		{
			lotusModel ??= FindLotusById(_meditation.State.TargetLotusId);
			if (lotusModel != null)
			{
				_lotusCollection.TryCollect(run, lotusModel);
				if (lotusModel.HasQuote)
				{
					_quoteUnlock.UnlockForRun(run, lotusModel);
					_lotusRevealQuoteId = _quoteCatalog.GetQuoteIdForPilgrimageSegment(lotusModel.SegmentIndex);
					_lotusRevealOriginTileX = lotusModel.TileX;
					_lotusRevealOriginTileY = lotusModel.TileY;
					_lotusRevealProgress = 0f;
					_uiMode = PilgrimageUiMode.ShowingLotusReveal;
				}
			}
			_meditation.Reset();
			return _uiMode == PilgrimageUiMode.ShowingLotusReveal;
		}
		if (_meditation.State.Phase == MeditationPhase.Interrupted)
		{
			_meditation.Reset();
			_uiMode = PilgrimageUiMode.Playing;
			_interruptedHintTimer = 1.4f;
		}
		if (_uiMode == PilgrimageUiMode.ShowingLotusReveal)
		{
			return true;
		}
		if (_meditation.State.IsActive)
		{
			_uiMode = PilgrimageUiMode.Meditating;
			_meditation.Update(run, delta, intent.MeditateHold);
			return true;
		}
		if (lotusModel != null && !run.CollectedLotusIds.Contains(lotusModel.Id) && intent.MeditateHold && _meditation.TryBegin(run, lotusModel))
		{
			_uiMode = PilgrimageUiMode.Meditating;
			_meditation.Update(run, delta, meditateHold: true);
			return true;
		}
		_uiMode = PilgrimageUiMode.Playing;
		return false;
	}

	private LotusModel? GetLotusAtMonk()
	{
		var segment = _pilgrimage?.GetCurrentSegment();
		if (segment == null)
		{
			return null;
		}

		foreach (LotusModel lotus in segment.Lotuses)
		{
			if (lotus.TileX == _monkTileX && lotus.TileY == _monkTileY)
			{
				return lotus;
			}
		}

		return null;
	}

	private LotusModel? FindLotusById(int lotusId)
	{
		var segment = _pilgrimage?.GetCurrentSegment();
		if (segment == null)
		{
			return null;
		}

		foreach (LotusModel lotus in segment.Lotuses)
		{
			if (lotus.Id == lotusId)
			{
				return lotus;
			}
		}

		return null;
	}

	private void DrawMeditationHint()
	{
		if (_context == null)
		{
			return;
		}

		string text = _context.Localization.Get("meditation.hold");
		var destinationRectangle = new Rectangle(8, 168, 200, 12);
		_context.SpriteBatch.Draw(_context.Pixel, destinationRectangle, UiPalette.PanelDark);
		_context.Text.DrawText(_context.SpriteBatch, text, new Vector2(destinationRectangle.X + 4, destinationRectangle.Y + 1), UiPalette.PinkSoft);
	}

	private void DrawInterruptedHint()
	{
		if (_context == null)
		{
			return;
		}

		string text = _context.Localization.Get("meditation.interrupted");
		var destinationRectangle = new Rectangle(8, 160, 200, 16);
		_context.SpriteBatch.Draw(_context.Pixel, destinationRectangle, UiPalette.PanelDark);
		_context.Text.DrawText(_context.SpriteBatch, text, new Vector2(destinationRectangle.X + 4, destinationRectangle.Y + 2), UiPalette.TextSecondary);
	}

	private void DrawPortalLockedHint()
	{
		if (_context == null)
		{
			return;
		}

		const int panelWidth = 288;
		const int panelX = (320 - panelWidth) / 2;
		string text = _context.Localization.Get("pilgrim.portal.locked");
		int textHeight = _context.Text.MeasureWrappedHeight(text, panelWidth - 12, 9, compact: true);
		var destinationRectangle = new Rectangle(panelX, 10, panelWidth, textHeight + 10);
		_context.SpriteBatch.Draw(_context.Pixel, destinationRectangle, UiPalette.PanelDark);
		_context.Text.DrawWrappedText(
			_context.SpriteBatch,
			text,
			new Rectangle(destinationRectangle.X + 6, destinationRectangle.Y + 4, panelWidth - 12, textHeight + 2),
			UiPalette.TextSecondary,
			9,
			compact: true);
	}
}
