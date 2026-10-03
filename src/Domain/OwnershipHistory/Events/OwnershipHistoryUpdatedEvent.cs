using Domain.Common;

namespace Domain.OwnershipHistorys.Events;

public record OwnershipHistoryUpdatedEvent(Id Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
