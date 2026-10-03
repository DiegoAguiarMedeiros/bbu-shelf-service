using Domain.Common;

namespace Domain.TradeOfferEntrys.Events;

public record TradeOfferEntryCreatedEvent(Id Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
