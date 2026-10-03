using Domain.Common;
using Domain.Common.Enums;
using Domain.OwnershipHistorys.Events;

namespace Domain.OwnershipHistorys;

public class OwnershipHistory : AggregateRoot<Id>
{
    public Id ShelfId { get; private set; } = null!;
    public Id? FromUserId { get; private set; }
    public Id ToUserId { get; private set; } = null!;
    public Id? TradeOfferId { get; private set; }
    public AcquiredSource AcquiredSource { get; private set; }

    public DateTime TransferredAt { get; private set; }

    private OwnershipHistory() { } // Required for EF Core

    public OwnershipHistory(Id id, Id shelfId, Id fromUserId, Id toUserId, Id tradeOfferId, AcquiredSource acquiredSource) : base(id)
    {
        ShelfId = shelfId;
        FromUserId = fromUserId;
        ToUserId = toUserId;
        TradeOfferId = tradeOfferId;
        AcquiredSource = acquiredSource;
        TransferredAt = DateTime.UtcNow;

        AddDomainEvent(new OwnershipHistoryCreatedEvent(Id));
    }

    public static OwnershipHistory Create(Id id, Id shelfId, Id fromUserId, Id toUserId, Id tradeOfferId, AcquiredSource acquiredSource)
    {
        return new OwnershipHistory(id, shelfId, fromUserId, toUserId, tradeOfferId, acquiredSource);
    }
}
