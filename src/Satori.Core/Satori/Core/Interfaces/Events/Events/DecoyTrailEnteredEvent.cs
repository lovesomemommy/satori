namespace Satori.Core.Interfaces.Events.Events;

public sealed record DecoyTrailEnteredEvent(int SegmentIndex) : IGameEvent;
