using Satori.Core.Models.Lotus;

namespace Satori.Core.Interfaces.Events.Events;

public sealed record LotusCollectedEvent(int LotusId, LotusType Type) : IGameEvent;
