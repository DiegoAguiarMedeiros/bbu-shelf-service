using Domain.Common;

namespace Domain.TradeOffers.Events;

public record TradeOfferCreatedEvent(Id Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}