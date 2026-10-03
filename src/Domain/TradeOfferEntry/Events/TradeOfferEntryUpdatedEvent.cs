using Domain.Common;

namespace Domain.TradeOfferEntrys.Events;

public record TradeOfferEntryUpdatedEvent(Id Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
