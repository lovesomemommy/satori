using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.UI.Framework;
using Satori.Core.Interfaces.Services;
using Satori.Core.Systems.Minigames;

namespace Satori.Client.Scenes.Minigames;

public sealed class RightSpeechScene : IScene
{
	private const int OptionX = 12;

	private const int OptionWidth = 296;

	private const int OptionInnerWidth = OptionWidth - 8;

	private const int OptionLineHeight = 9;

	private const int OptionMinHeight = 16;

	private const int NextButtonHeight = 16;

	private const int BottomMargin = 6;

	private static readonly Rectangle SituationBounds = new Rectangle(12, 36, 296, 30);

	private SceneContext? _context;

	private readonly UiScreen _screen = new UiScreen();

	private readonly Random _random = new Random();

	private RightSpeechSystem? _rightSpeech;

	private string _statusText = string.Empty;

	private bool _answered;

	private bool _showCompletion;

	private Rectangle _statusBannerBounds;

	public void Load(SceneContext context)
	{
		_context = context;
		_rightSpeech = context.Services.GetRequiredService<RightSpeechSystem>();
		_answered = false;
		_statusText = string.Empty;
		_statusBannerBounds = Rectangle.Empty;
		var progress = context.Session.Save.Precepts;
		string revisionBefore = progress.RightSpeechCatalogRevision ?? string.Empty;
		int completedBefore = progress.CompletedRightSpeechQuestionIds.Count;
		_rightSpeech.ReloadCatalog();
		_rightSpeech.SyncProgressWithCatalog(progress);
		if (progress.RightSpeechCatalogRevision != revisionBefore
			|| progress.CompletedRightSpeechQuestionIds.Count != completedBefore)
		{
			context.PersistSave();
		}

		_showCompletion = _rightSpeech.IsAllComplete(progress);
		if (_showCompletion)
		{
			BuildCompletionUi();
		}
		else
		{
			_rightSpeech.TryStartQuestion(_random, context.Session.Save.Precepts);
			BuildUi();
		}
	}

	public void Unload()
	{
		_screen.Clear();
		_rightSpeech?.ClearQuestion();
		_context = null;
		_rightSpeech = null;
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

		if (_showCompletion)
		{
			DrawCompletionMessage();
			return;
		}

		if (_rightSpeech?.CurrentQuestion == null)
		{
			return;
		}

		_context.Text.DrawWrappedText(
			_context.SpriteBatch,
			_rightSpeech.CurrentQuestion.Situation,
			SituationBounds,
			UiPalette.TextPrimary,
			OptionLineHeight,
			compact: true);

		if (!string.IsNullOrEmpty(_statusText) && _statusBannerBounds.Height > 0)
		{
			_context.SpriteBatch.Draw(_context.Pixel, _statusBannerBounds, UiPalette.PanelDark);
			_context.Text.DrawWrappedText(
				_context.SpriteBatch,
				_statusText,
				new Rectangle(_statusBannerBounds.X + 4, _statusBannerBounds.Y + 3, _statusBannerBounds.Width - 8, _statusBannerBounds.Height - 6),
				UiPalette.TextSecondary,
				OptionLineHeight,
				compact: true);
		}
	}

	private void DrawCompletionMessage()
	{
		if (_context == null)
		{
			return;
		}

		var panel = new Rectangle(20, 48, 280, 72);
		_context.SpriteBatch.Draw(_context.Pixel, panel, UiPalette.PanelDark);
		string message = _context.Localization.Get("minigames.right_speech.complete");
		_context.Text.DrawWrappedText(_context.SpriteBatch, message, new Rectangle(panel.X + 8, panel.Y + 8, panel.Width - 16, panel.Height - 16), UiPalette.TextPrimary, OptionLineHeight, compact: true);
	}

	private void BuildUi()
	{
		if (_context == null || _rightSpeech?.CurrentQuestion == null)
		{
			return;
		}

		_screen.Clear();
		AddHeader();

		var question = _rightSpeech.CurrentQuestion;
		int y = SituationBounds.Bottom + 6;
		for (int i = 0; i < question.Options.Count; i++)
		{
			int optionIndex = i;
			string optionText = question.Options[i];
			int optionHeight = Math.Max(
				OptionMinHeight,
				_context.Text.MeasureWrappedHeight(optionText, OptionInnerWidth, OptionLineHeight, compact: true) + 3);
			_screen.Add(new UiButton
			{
				Text = optionText,
				WrapText = true,
				AlignLeft = true,
				UseCompactFont = true,
				LineHeight = OptionLineHeight,
				Bounds = new Rectangle(OptionX, y, OptionWidth, optionHeight),
				OnClick = () => SubmitAnswer(optionIndex)
			});
			y += optionHeight + 4;
		}
	}

	private void BuildCompletionUi()
	{
		if (_context == null)
		{
			return;
		}

		_screen.Clear();
		AddHeader();
	}

	private void AddHeader()
	{
		if (_context == null)
		{
			return;
		}

		_screen.Add(new UiLabel
		{
			Text = _context.Localization.Get("minigames.right_speech.title"),
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

	private void SubmitAnswer(int optionIndex)
	{
		if (_context == null || _rightSpeech == null || _answered)
		{
			return;
		}

		_answered = true;
		var outcome = _rightSpeech.TryAnswer(_context.Session.Meta, _context.Session.Save.Precepts, optionIndex);
		SaveProgress();

		if (outcome.AllQuestionsComplete)
		{
			_showCompletion = true;
			BuildCompletionUi();
			return;
		}

		_statusText = outcome.IsCorrect
			? string.Format(_context.Localization.Get("minigames.right_speech.success"), outcome.KarmaDelta)
			: string.Format(_context.Localization.Get("minigames.right_speech.fail"), Math.Abs(outcome.KarmaDelta));

		RebuildAfterAnswer();
	}

	private void RebuildAfterAnswer()
	{
		if (_context == null)
		{
			return;
		}

		int nextButtonY = _context.Viewport.VirtualHeight - BottomMargin - NextButtonHeight;
		int statusTextHeight = _context.Text.MeasureWrappedHeight(_statusText, OptionInnerWidth, OptionLineHeight, compact: true);
		int statusBannerHeight = Math.Max(NextButtonHeight, statusTextHeight + 6);
		int statusBannerY = nextButtonY - 6 - statusBannerHeight;
		_statusBannerBounds = new Rectangle(OptionX, statusBannerY, OptionWidth, statusBannerHeight);

		_screen.Clear();
		AddHeader();
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("minigames.right_speech.next"),
			Bounds = new Rectangle(OptionX, nextButtonY, 120, NextButtonHeight),
			OnClick = StartNextQuestion
		});
	}

	private void StartNextQuestion()
	{
		if (_context == null || _rightSpeech == null)
		{
			return;
		}

		_answered = false;
		_statusText = string.Empty;
		_statusBannerBounds = Rectangle.Empty;
		if (!_rightSpeech.TryStartQuestion(_random, _context.Session.Save.Precepts))
		{
			_showCompletion = true;
			BuildCompletionUi();
			return;
		}

		BuildUi();
	}

	private void SaveProgress()
	{
		if (_context == null)
		{
			return;
		}

		_context.PersistSave();
	}
}
