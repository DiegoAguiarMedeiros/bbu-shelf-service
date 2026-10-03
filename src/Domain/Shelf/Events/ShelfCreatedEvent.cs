using Domain.Common;

namespace Domain.Shelfs.Events;

public record ShelfCreatedEvent(Id Id) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
