using Domain.Common;

namespace Domain.TradeOffers.Events;

public record TradeOfferAcceptedEvent(Id Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}