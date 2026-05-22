using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Client.Scenes.PilgrimTrials;
using Satori.Client.Services.Hub;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.UI.Framework;
using Satori.Client.Views.Hub;
using Satori.Core.Systems.Progression;
using System.Collections.Generic;

namespace Satori.Client.Scenes;

public sealed class HubScene : IScene
{
	private SceneContext? _context;

	private HubBackgroundCatalog? _background;

	private readonly UiScreen _screen = new UiScreen();

	private string _karmaText = string.Empty;

	private bool _isNight;

	private bool _finaleHintVisible;

	private Rectangle _finaleHintPanelBounds;

	private Rectangle _finaleHintCloseBounds;

	private bool _finaleHintClosePointerDown;

	private readonly List<(string Text, Color Color)> _finaleHintLines = new();

	public void Load(SceneContext context)
	{
		_context = context;
		_background = context.Services.GetRequiredService<HubBackgroundCatalog>();
		_finaleHintVisible = false;
		_finaleHintClosePointerDown = false;
		_finaleHintLines.Clear();
		RefreshMetaLabels();
		BuildUi();
	}

	public void Unload()
	{
		_screen.Clear();
		_context = null;
		_background = null;
	}

	public void Update(GameTime gameTime)
	{
		if (_context == null)
		{
			return;
		}

		var state = Keyboard.GetState();
		var mouse = VirtualInput.ToVirtualMouse(_context.Viewport, Mouse.GetState());
		_screen.Update(gameTime, mouse, state);
		UpdateFinaleHint(mouse);
	}

	public void Draw(GameTime gameTime)
	{
		if (_context == null)
		{
			return;
		}

		float glowPhase = UiAnimator.GlowPhase(gameTime);
		float timeSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
		Color background = _isNight ? UiPalette.BackgroundNight : UiPalette.Background;
		_context.SpriteBatch.Draw(_context.Pixel, new Rectangle(0, 0, _context.Viewport.VirtualWidth, _context.Viewport.VirtualHeight), background);
		_background?.Draw(_context.SpriteBatch, _context.Pixel, _context.Viewport.VirtualWidth, _context.Viewport.VirtualHeight, _isNight);
		HubBatsView.Draw(_context.SpriteBatch, _context.Pixel, timeSeconds, _context.Viewport.VirtualWidth);

		var buttonPanel = new Rectangle(HubLayout.ButtonX, HubLayout.ContentTop, HubLayout.ButtonWidth, HubLayout.ContentHeight);
		_context.SpriteBatch.Draw(_context.Pixel, buttonPanel, UiPalette.Panel);

		EnlightenmentAscensionView.Draw(
			_context.SpriteBatch,
			_context.Pixel,
			_context.Session.Meta.Enlightenment,
			glowPhase);

		var enlightenmentLabel = _context.Localization.Get("hub.enlightenment.short");
		var labelSize = _context.Text.MeasureText(enlightenmentLabel);
		var enlightenmentBounds = EnlightenmentAscensionView.GetBounds();
		_context.Text.DrawText(
			_context.SpriteBatch,
			enlightenmentLabel,
			new Vector2(
				enlightenmentBounds.X + (enlightenmentBounds.Width - labelSize.X) * 0.5f,
				enlightenmentBounds.Y + 3),
			UiPalette.TextMuted);

		_context.Text.DrawText(_context.SpriteBatch, _karmaText, new Vector2(12, 22), UiPalette.TextSecondary);
		_screen.Draw(_context.SpriteBatch, _context.Pixel, _context.Text, glowPhase);
		if (_finaleHintVisible && _finaleHintLines.Count > 0)
		{
			DrawFinaleHint();
		}
	}

	private void UpdateFinaleHint(MouseState mouse)
	{
		if (!_finaleHintVisible)
		{
			_finaleHintClosePointerDown = false;
			return;
		}

		var mousePoint = new Point(mouse.X, mouse.Y);
		if (UiClickHelper.TryHandleClick(_finaleHintCloseBounds, mousePoint, mouse.LeftButton, ref _finaleHintClosePointerDown))
		{
			_finaleHintVisible = false;
			_finaleHintLines.Clear();
		}
	}

	private void DrawFinaleHint()
	{
		if (_context == null)
		{
			return;
		}

		const int virtualWidth = 320;
		const int virtualHeight = 180;
		const int panelWidth = 268;
		const int lineHeight = 10;
		const int linePadding = 2;
		int panelHeight = 16 + _finaleHintLines.Count * (lineHeight + linePadding) + 6;
		_finaleHintPanelBounds = new Rectangle(
			(virtualWidth - panelWidth) / 2,
			(virtualHeight - panelHeight) / 2,
			panelWidth,
			panelHeight);
		_finaleHintCloseBounds = new Rectangle(_finaleHintPanelBounds.Right - 14, _finaleHintPanelBounds.Y + 3, 12, 12);

		_context.SpriteBatch.Draw(
			_context.Pixel,
			new Rectangle(0, 0, virtualWidth, virtualHeight),
			new Color(0, 0, 0, 120));
		_context.SpriteBatch.Draw(_context.Pixel, _finaleHintPanelBounds, new Color(10, 10, 14, 175));
		_context.SpriteBatch.Draw(_context.Pixel, _finaleHintCloseBounds, new Color(32, 32, 36, 200));
		_context.Text.DrawText(
			_context.SpriteBatch,
			"x",
			new Vector2(_finaleHintCloseBounds.X + 3, _finaleHintCloseBounds.Y + 1),
			UiPalette.TextPrimary,
			compact: true);

		int y = _finaleHintPanelBounds.Y + 8;
		foreach (var (text, color) in _finaleHintLines)
		{
			_context.Text.DrawText(
				_context.SpriteBatch,
				text,
				new Vector2(_finaleHintPanelBounds.X + 6, y),
				color,
				compact: true);
			y += lineHeight + linePadding;
		}
	}

	private void RefreshMetaLabels()
	{
		if (_context == null)
		{
			return;
		}

		_karmaText = string.Format(_context.Localization.Get("hub.karma"), _context.Session.Meta.Karma);
		_isNight = HubAmbienceSystem.IsNight(_context.Session.Meta);
	}

	private void TryOpenFinale()
	{
		if (_context == null)
		{
			return;
		}

		if (WinConditionSystem.Evaluate(_context.Session.Save).IsComplete)
		{
			_context.SceneManager.ChangeTo(GameStateType.Finale);
			return;
		}

		ShowFinaleLockedHint();
	}

	private void ShowFinaleLockedHint()
	{
		if (_context == null)
		{
			return;
		}

		var save = _context.Session.Save;
		var status = WinConditionSystem.Evaluate(save);
		int enlightenmentPercent = (int)MathF.Round(save.Meta.Enlightenment * 100f);

		_finaleHintLines.Clear();
		_finaleHintLines.Add((_context.Localization.Get("hub.finale.locked.title"), UiPalette.PinkSoft));
		_finaleHintLines.Add((_context.Localization.Get("hub.finale.locked.requirements"), UiPalette.TextSecondary));
		_finaleHintLines.Add((
			status.PilgrimageCompleted
				? _context.Localization.Get("hub.finale.progress.pilgrimage.done")
				: _context.Localization.Get("hub.finale.progress.pilgrimage.todo"),
			status.PilgrimageCompleted ? UiPalette.TextPrimary : UiPalette.TextMuted));
		_finaleHintLines.Add((
			string.Format(
				_context.Localization.Get("hub.finale.progress.garden"),
				save.Meta.PlantedLotuses.Count,
				GardenPlantingSystem.MaxSlots),
			status.GardenFull ? UiPalette.TextPrimary : UiPalette.TextMuted));
		_finaleHintLines.Add((
			string.Format(
				_context.Localization.Get("hub.finale.progress.wisdom"),
				save.Wisdom.Quotes.Count,
				WinConditionSystem.RequiredQuoteCount),
			status.WisdomGathered ? UiPalette.TextPrimary : UiPalette.TextMuted));
		_finaleHintLines.Add((
			string.Format(
				_context.Localization.Get("hub.finale.progress.enlightenment"),
				enlightenmentPercent),
			status.EnlightenmentReached ? UiPalette.TextPrimary : UiPalette.TextMuted));
		_finaleHintVisible = true;
		_finaleHintClosePointerDown = false;
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
			Text = _context.Localization.Get("hub.title"),
			Bounds = new Rectangle(12, 8, 200, 14),
			Color = UiPalette.TextPrimary
		});

		const int buttonCount = 6;
		int buttonHeight = 20;
		int rowHeight = (HubLayout.ContentHeight - buttonHeight) / (buttonCount - 1);
		int y = HubLayout.ContentTop;
		void AddHubButton(string label, Action onClick)
		{
			_screen.Add(new UiButton
			{
				Text = label,
				Bounds = new Rectangle(HubLayout.ButtonX + 2, y, HubLayout.ButtonWidth - 4, buttonHeight),
				WrapText = true,
				AlignLeft = true,
				UseCompactFont = true,
				LineHeight = 8,
				OnClick = onClick
			});
			y += rowHeight;
		}

		AddHubButton(_context.Localization.Get("pilgrim.start"), () => PilgrimageSceneStarter.StartFromHub(_context));
		AddHubButton(_context.Localization.Get("hub.garden.title"), () => _context.SceneManager.ChangeTo(GameStateType.Garden));
		AddHubButton(_context.Localization.Get("wisdom.library.title"), () => _context.SceneManager.ChangeTo(GameStateType.WisdomLibrary));
		AddHubButton(_context.Localization.Get("minigames.title"), () => _context.SceneManager.ChangeTo(GameStateType.MinigamesHub));
		AddHubButton(_context.Localization.Get("hub.finale.title"), TryOpenFinale);
		AddHubButton(_context.Localization.Get("settings.title"), () => SettingsSceneStarter.Open(_context, GameStateType.Hub));
	}
}
