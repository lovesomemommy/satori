using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.UI.Framework;
using Satori.Client.Views.Finale;
using Satori.Core.Interfaces.Services;

namespace Satori.Client.Scenes;

public sealed class FinaleScene : IScene
{
	private SceneContext? _context;

	private readonly UiScreen _screen = new UiScreen();

	private float _elapsedSeconds;

	public void Load(SceneContext context)
	{
		_context = context;
		_elapsedSeconds = 0f;
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

		_elapsedSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
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
		_context.SpriteBatch.Draw(_context.Pixel, new Rectangle(0, 0, _context.Viewport.VirtualWidth, _context.Viewport.VirtualHeight), UiPalette.Black);
		FinaleView.Draw(
			_context.SpriteBatch,
			_context.Pixel,
			_context.Text,
			_elapsedSeconds,
			glowPhase,
			_context.Localization.Get("finale.title"),
			_context.Localization.Get("finale.subtitle"),
			_context.Localization.Get("finale.quote"));
		_screen.Draw(_context.SpriteBatch, _context.Pixel, _context.Text, glowPhase);
	}

	private void BuildUi()
	{
		if (_context == null)
		{
			return;
		}

		_screen.Clear();
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("finale.continue"),
			Bounds = new Rectangle(112, 158, 96, 18),
			OnClick = CompleteFinale
		});
	}

	private void CompleteFinale()
	{
		if (_context == null)
		{
			return;
		}

		_context.Session.Save.Pilgrimage.FinaleCompleted = true;
		_context.PersistSave();
		_context.SceneManager.ChangeTo(GameStateType.Hub);
	}
}
