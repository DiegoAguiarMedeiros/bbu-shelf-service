using Domain.Common;

namespace Domain.TradeOffers.Events;

public record TradeOfferDeclinedEvent(Id Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}