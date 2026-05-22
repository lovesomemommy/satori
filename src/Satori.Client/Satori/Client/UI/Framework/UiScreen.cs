using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.UI;

namespace Satori.Client.UI.Framework;

public sealed class UiScreen
{
	private readonly List<IUiElement> _elements = new List<IUiElement>();

	public IReadOnlyList<IUiElement> Elements => _elements;

	public void Add(IUiElement element)
	{
		_elements.Add(element);
	}

	public void Clear()
	{
		_elements.Clear();
	}

	public void ResetButtonPointerStates()
	{
		foreach (IUiElement element in _elements)
		{
			if (element is UiButton button)
			{
				button.ResetPointerState();
			}
		}
	}

	public void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
	{
		var snapshot = _elements.ToArray();
		foreach (IUiElement element in snapshot)
		{
			element.Update(gameTime, mouse, keyboard);
		}
	}

	public void Draw(SpriteBatch spriteBatch, Texture2D pixel, TextRenderingService text, float glowPhase)
	{
		foreach (IUiElement element in _elements)
		{
			element.Draw(spriteBatch, pixel, text, glowPhase);
		}
	}
}
