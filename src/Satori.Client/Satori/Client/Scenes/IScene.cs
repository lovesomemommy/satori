using Microsoft.Xna.Framework;

namespace Satori.Client.Scenes;

public interface IScene
{
	void Load(SceneContext context);

	void Unload();

	void Update(GameTime gameTime);

	void Draw(GameTime gameTime);
}
