using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.UI.Framework;
using Satori.Client.Views.Hub;
using Satori.Core.Systems.Progression;

namespace Satori.Client.Scenes;

public sealed class GardenScene : IScene
{
	private SceneContext? _context;

	private readonly UiScreen _screen = new UiScreen();

	private string _lotusCountText = string.Empty;

	public void Load(SceneContext context)
	{
		_context = context;
		_lotusCountText = string.Format(
			context.Localization.Get("hub.lotuses_planted"),
			context.Session.Meta.PlantedLotuses.Count,
			GardenPlantingSystem.MaxSlots);
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
		var gardenOrigin = GardenView.GetCenteredOrigin(_context.Viewport.VirtualWidth);
		GardenView.Draw(
			_context.SpriteBatch,
			_context.Pixel,
			_context.Session.Meta,
			glowPhase,
			gardenOrigin,
			_context.GardenSprites.FloorTile,
			_context.PilgrimSprites.Lotus,
			_context.PilgrimSprites.WallTile);

		string title = _context.Localization.Get("hub.garden.title");
		_context.Text.DrawText(_context.SpriteBatch, title, new Vector2(12, 8), UiPalette.TextPrimary);
		_context.Text.DrawText(
			_context.SpriteBatch,
			_lotusCountText,
			new Vector2(12, 20),
			UiPalette.TextSecondary,
			compact: true);

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
			Text = _context.Localization.Get("hub.back"),
			Bounds = new Rectangle(230, 6, 78, 18),
			OnClick = () => _context.SceneManager.ChangeTo(GameStateType.Hub)
		});
	}
}
