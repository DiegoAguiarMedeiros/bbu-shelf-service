using Domain.Common;

namespace Domain.Shelfs.Events;

public record ShelfUpdatedEvent(Id Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
