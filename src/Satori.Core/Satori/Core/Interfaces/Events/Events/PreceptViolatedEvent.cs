using Satori.Core.Models.Precepts;

namespace Satori.Core.Interfaces.Events.Events;

public sealed record PreceptViolatedEvent(PreceptType PreceptType, string MessageKey, int SegmentIndex) : IGameEvent;
