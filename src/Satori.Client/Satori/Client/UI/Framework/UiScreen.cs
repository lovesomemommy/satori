using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.UI;

namespace Satori.Client.UI.Framework;

public sealed class UiScreen
{
	private readonly List<UiElement> _elements = [];

	public IReadOnlyList<UiElement> Elements => _elements;

	public void Add(UiElement element) => _elements.Add(element);

	public void Clear() => _elements.Clear();

	public void ResetButtonPointerStates()
	{
		foreach (UiElement element in _elements)
		{
			if (element is UiButton button)
			{
				button.ResetPointerState();
			}
		}
	}

	public void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
	{
		// Snapshot: button handlers may rebuild the screen during Update.
		var snapshot = _elements.ToArray();
		foreach (UiElement element in snapshot)
		{
			element.Update(gameTime, mouse, keyboard);
		}
	}

	public void Draw(SpriteBatch spriteBatch, Texture2D pixel, TextRenderingService text, float glowPhase)
	{
		foreach (UiElement element in _elements)
		{
			element.Draw(spriteBatch, pixel, text, glowPhase);
		}
	}
}
