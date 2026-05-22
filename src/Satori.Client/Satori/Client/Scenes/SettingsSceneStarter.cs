using Satori.Client.State;

namespace Satori.Client.Scenes;

public static class SettingsSceneStarter
{
	private static GameStateType _returnState = GameStateType.MainMenu;

	public static void Open(SceneContext context, GameStateType returnState)
	{
		_returnState = returnState;
		context.SceneManager.ChangeTo(GameStateType.Settings);
	}

	public static GameStateType ReturnState => _returnState;
}
