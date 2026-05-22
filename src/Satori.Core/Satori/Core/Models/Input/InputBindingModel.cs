namespace Satori.Core.Models.Input;

public sealed class InputBindingModel
{
	public string MoveUp { get; set; } = "W";

	public string MoveDown { get; set; } = "S";

	public string MoveLeft { get; set; } = "A";

	public string MoveRight { get; set; } = "D";

	public string Meditate { get; set; } = "Space";

	public string Pause { get; set; } = "Escape";

	public string Interact { get; set; } = "E";
}
