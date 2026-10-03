using Domain.Common;

namespace Domain.OwnershipHistorys.Events;

public record OwnershipHistoryCreatedEvent(Id Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
