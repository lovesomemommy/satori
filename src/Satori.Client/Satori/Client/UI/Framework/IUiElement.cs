using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Satori.Client.UI;

namespace Satori.Client.UI.Framework;

public interface IUiElement
{
	Rectangle Bounds { get; }

	bool IsVisible { get; set; }

	void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard);

	void Draw(SpriteBatch spriteBatch, Texture2D pixel, TextRenderingService text, float glowPhase);
}
