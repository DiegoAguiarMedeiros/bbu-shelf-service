using Domain.Common;
using Domain.TradeOfferEntrys.Events;

namespace Domain.TradeOfferEntrys;

public class TradeOfferEntry : AggregateRoot<Id>
{
    public Id TradeOfferId { get; private set; } = null!;
    public Id OwnerUserId { get; private set; } = null!;
    public Id ShelfId { get; private set; } = null!;

    private TradeOfferEntry() { } // Required for EF Core

    public TradeOfferEntry(Id id, Id tradeOfferId, Id ownerUserId, Id shelfId) : base(id)
    {
        TradeOfferId = tradeOfferId;
        OwnerUserId = ownerUserId;
        ShelfId = shelfId;

        AddDomainEvent(new TradeOfferEntryCreatedEvent(Id));
    }

    public static TradeOfferEntry Create(Id id, Id tradeOfferId, Id toUserId, Id shelfId)
    {
        return new TradeOfferEntry(id, tradeOfferId, toUserId, shelfId);
    }
}
