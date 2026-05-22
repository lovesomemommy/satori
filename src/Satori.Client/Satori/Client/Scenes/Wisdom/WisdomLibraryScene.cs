using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.Input;
using Satori.Client.Services.Wisdom;
using Satori.Client.State;
using Satori.Client.UI;
using Satori.Client.UI.Framework;
using Satori.Client.Views.Wisdom;
using Satori.Core.Models.Wisdom;
using Satori.Core.Systems.Progression;

namespace Satori.Client.Scenes.Wisdom;

public sealed class WisdomLibraryScene : IScene
{
	private SceneContext? _context;

	private readonly UiScreen _screen = new UiScreen();

	private QuoteImageCatalog? _quoteImages;

	private readonly Dictionary<string, QuoteModel> _quotesById = new();

	private int _selectedSlot = -1;

	private bool _pointerDown;

	public void Load(SceneContext context)
	{
		_context = context;
		_quoteImages = context.Services.GetRequiredService<QuoteImageCatalog>();
		_selectedSlot = -1;
		RefreshQuotes();
		BuildUi();
	}

	public void Unload()
	{
		_screen.Clear();
		_context = null;
		_quoteImages = null;
		_quotesById.Clear();
	}

	public void Update(GameTime gameTime)
	{
		if (_context == null)
		{
			return;
		}

		var mouse = VirtualInput.ToVirtualMouse(_context.Viewport, Mouse.GetState());
		HandleGalleryClick(new Point(mouse.X, mouse.Y), mouse.LeftButton);
		_screen.Update(gameTime, mouse, Keyboard.GetState());
	}

	public void Draw(GameTime gameTime)
	{
		if (_context == null)
		{
			return;
		}

		float glowPhase = UiAnimator.GlowPhase(gameTime);
		_context.SpriteBatch.Draw(
			_context.Pixel,
			new Rectangle(0, 0, _context.Viewport.VirtualWidth, _context.Viewport.VirtualHeight),
			UiPalette.Background);

		for (int slotIndex = 0; slotIndex < WisdomGalleryView.TotalSlots; slotIndex++)
		{
			string quoteId = WisdomGalleryView.SlotQuoteIds[slotIndex];
			bool isUnlocked = _quotesById.ContainsKey(quoteId);
			var cellBounds = WisdomGalleryView.GetSlotBounds(slotIndex);
			WisdomGalleryView.DrawSlotFrame(
				_context.SpriteBatch,
				_context.Pixel,
				cellBounds,
				isUnlocked,
				slotIndex == _selectedSlot);

			if (isUnlocked)
			{
				var image = _quoteImages?.GetQuoteImage(quoteId);
				if (image != null)
				{
					WisdomGalleryView.DrawSquareImage(
						_context.SpriteBatch,
						image,
						WisdomGalleryView.GetImageBounds(cellBounds));
				}
			}
		}

		_screen.Draw(_context.SpriteBatch, _context.Pixel, _context.Text, glowPhase);
	}

	private void HandleGalleryClick(Point mousePoint, ButtonState leftButton)
	{
		for (int slotIndex = 0; slotIndex < WisdomGalleryView.TotalSlots; slotIndex++)
		{
			if (!WisdomGalleryView.GetSlotBounds(slotIndex).Contains(mousePoint))
			{
				continue;
			}

			string quoteId = WisdomGalleryView.SlotQuoteIds[slotIndex];
			if (!_quotesById.ContainsKey(quoteId))
			{
				if (leftButton == ButtonState.Released)
				{
					_pointerDown = false;
				}

				return;
			}

			if (leftButton == ButtonState.Pressed)
			{
				_pointerDown = true;
			}

			if (_pointerDown && leftButton == ButtonState.Released)
			{
				_selectedSlot = slotIndex;
				_pointerDown = false;
			}

			return;
		}

		if (leftButton == ButtonState.Released)
		{
			_pointerDown = false;
		}
	}

	private void RefreshQuotes()
	{
		_quotesById.Clear();
		if (_context == null)
		{
			return;
		}

		foreach (QuoteModel quote in _context.Session.Save.Wisdom.Quotes)
		{
			_quotesById[quote.QuoteId] = quote;
		}

		if (_selectedSlot >= 0)
		{
			string selectedQuoteId = WisdomGalleryView.SlotQuoteIds[_selectedSlot];
			if (!_quotesById.ContainsKey(selectedQuoteId))
			{
				_selectedSlot = -1;
			}
		}
	}

	private void BuildUi()
	{
		if (_context == null)
		{
			return;
		}

		_screen.Clear();
		int foundCount = WisdomGalleryView.SlotQuoteIds.Count(id => _quotesById.ContainsKey(id));
		_screen.Add(new UiLabel
		{
			Text = _context.Localization.Get("wisdom.library.title"),
			Bounds = new Rectangle(12, 8, 200, 14)
		});
		_screen.Add(new UiLabel
		{
			Text = $"{foundCount}/{WinConditionSystem.RequiredQuoteCount}",
			Bounds = new Rectangle(12, 22, 48, 12),
			Color = UiPalette.TextMuted
		});
		_screen.Add(new UiButton
		{
			Text = _context.Localization.Get("hub.back"),
			Bounds = new Rectangle(230, 6, 78, 18),
			OnClick = () => _context.SceneManager.ChangeTo(GameStateType.Hub)
		});
	}
}
