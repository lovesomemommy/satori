namespace Satori.Core.Interfaces.Events.Events;

public sealed record QuoteUnlockedEvent(string QuoteId) : IGameEvent;
