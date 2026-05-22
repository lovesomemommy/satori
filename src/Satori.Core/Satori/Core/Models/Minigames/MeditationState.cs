namespace Satori.Core.Models.Minigames;

public sealed class MeditationState
{
	public MeditationPhase Phase { get; set; } = MeditationPhase.Idle;

	public int TargetLotusId { get; set; }

	public float PhaseElapsedSeconds { get; set; }

	public float TotalElapsedSeconds { get; set; }

	public bool IsActive
	{
		get
		{
			MeditationPhase phase = Phase;
			if ((uint)(phase - 1) <= 2u)
			{
				return true;
			}
			return false;
		}
	}
}
