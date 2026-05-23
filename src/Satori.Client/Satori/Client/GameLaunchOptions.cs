namespace Satori.Client;

public sealed class GameLaunchOptions
{
	public State.GameStateType PostBootState { get; init; } = State.GameStateType.MainMenu;

	public State.GameStateType SettingsReturnState { get; init; } = State.GameStateType.MainMenu;

	public static GameLaunchOptions Default { get; } = new();

	public static GameLaunchOptions OpenSettings { get; } = new()
	{
		PostBootState = State.GameStateType.Settings,
		SettingsReturnState = State.GameStateType.MainMenu
	};
}
