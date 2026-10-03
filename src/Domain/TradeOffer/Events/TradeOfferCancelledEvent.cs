using Domain.Common;

namespace Domain.TradeOffers.Events;

public record TradeOfferCancelledEvent(Id Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}