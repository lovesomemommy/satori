namespace Satori.Core.Models.Minigames;

public sealed class WheelOfDharmaState
{
	public WheelOfDharmaPhase Phase { get; set; } = WheelOfDharmaPhase.Idle;

	public int Difficulty { get; set; } = 1;

	public List<WheelDirection> Sequence { get; set; } = new List<WheelDirection>();

	public int ShowIndex { get; set; }

	public float ShowStepTimer { get; set; }

	public bool ShowHighlightActive { get; set; }

	public WheelDirection? ActiveShowDirection { get; set; }

	public WheelDirection? ActiveInputDirection { get; set; }

	public float InputHighlightTimer { get; set; }

	public int InputIndex { get; set; }
}
