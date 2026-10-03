using Domain.Common;
using Domain.Common.Enums;
using Domain.Shelfs.Enums;
using Domain.Shelfs.Events;

namespace Domain.Shelfs;

public class Shelf : AggregateRoot<Id>
{
    public Id UserId { get; private set; } = null!;
    public Id BookId { get; private set; } = null!;
    public AcquiredSource AcquiredSource { get; private set; }
    public CopyType CopyType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Shelf() { } // Required for EF Core

    public Shelf(Id id, Id userId, Id bookId, AcquiredSource acquiredSource, CopyType copyType) : base(id)
    {
        UserId = userId;
        BookId = bookId;
        AcquiredSource = acquiredSource;
        CopyType = copyType;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new ShelfCreatedEvent(Id));
    }

    public static Shelf Create(Id id, Id userId, Id bookId, AcquiredSource acquiredSource, CopyType copyType)
    {
        return new Shelf(id, userId, bookId, acquiredSource, copyType);
    }

    public void Update(CopyType copyType)
    {
        CopyType = copyType;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new ShelfUpdatedEvent(Id));
    }
}
