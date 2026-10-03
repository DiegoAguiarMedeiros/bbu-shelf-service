using Domain.Common;

namespace Domain.TradeOffers.Events;

public record TradeOfferCompletedEvent(Id Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}