using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.UI;

namespace Satori.Client.UI.Framework;

public abstract class UiElement : IUiElement
{
	public Rectangle Bounds { get; set; }

	public bool IsVisible { get; set; } = true;

	public abstract void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard);

	public abstract void Draw(SpriteBatch spriteBatch, Texture2D pixel, TextRenderingService text, float glowPhase);
}
