using Microsoft.Xna.Framework;

namespace Satori.Client.UI.Framework;

public static class UiAnimator
{
	public static float GlowPhase(GameTime gameTime)
	{
		return (float)gameTime.TotalGameTime.TotalSeconds * 2.5f;
	}
}
