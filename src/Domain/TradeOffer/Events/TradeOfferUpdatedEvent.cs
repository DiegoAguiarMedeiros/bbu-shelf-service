using Domain.Common;

namespace Domain.TradeOffers.Events;

public record TradeOfferUpdatedEvent(Id Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
