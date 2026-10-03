using Domain.Common;
using Domain.TradeOffers.Enums;
using Domain.TradeOffers.Events;
using Domain.TradeOfferEntrys;

namespace Domain.TradeOffers;

public class TradeOffer : AggregateRoot<Id>
{
    public Id FromUserId { get; private set; } = null!;
    public Id ToUserId { get; private set; } = null!;
    public Status Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    private readonly List<TradeOfferEntry> _entries = new();
    public IReadOnlyCollection<TradeOfferEntry> Entries => _entries.AsReadOnly();
    private TradeOffer() { } // Required for EF Core

    public TradeOffer(Id id, Id fromUserId, Id toUserId, Status status) : base(id)
    {
        FromUserId = fromUserId;
        ToUserId = toUserId;
        Status = status;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new TradeOfferCreatedEvent(Id));
    }

    public static TradeOffer Create(Id id, Id fromUserId, Id toUserId, Status status)
    {
        return new TradeOffer(id, fromUserId, toUserId, status);
    }

    public void Update(Status status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new TradeOfferUpdatedEvent(Id));
    }
    public void AddEntry(Id ownerUserId, Id shelfId)
    {
        if (Status != Status.pending)
            throw new InvalidOperationException("Cannot add entries to a trade offer that is not pending.");

        if (ownerUserId != FromUserId && ownerUserId != ToUserId)
            throw new InvalidOperationException("Entry owner must be either the offer sender or receiver.");

        if (_entries.Any(e => e.ShelfId == shelfId))
            throw new InvalidOperationException($"Shelf '{shelfId.Value}' is already part of this trade offer.");

        _entries.Add(TradeOfferEntry.Create(Id.New(), Id, ownerUserId, shelfId));
    }

    public void Accept()
    {
        if (Status != Status.pending)
            throw new InvalidOperationException("Only pending trade offers can be accepted.");

        Status = Status.accepted;
        AddDomainEvent(new TradeOfferAcceptedEvent(Id));
    }

    public void Decline()
    {
        if (Status != Status.pending)
            throw new InvalidOperationException("Only pending trade offers can be declined.");

        Status = Status.declined;
        AddDomainEvent(new TradeOfferDeclinedEvent(Id));
    }

    public void Cancel(Id requestedByUserId)
    {
        if (requestedByUserId != FromUserId)
            throw new InvalidOperationException("Only the offer creator can cancel it.");

        if (Status != Status.pending)
            throw new InvalidOperationException("Only pending trade offers can be cancelled.");

        Status = Status.cancelled;
        AddDomainEvent(new TradeOfferCancelledEvent(Id));
    }

    public void Complete()
    {
        if (Status != Status.accepted)
            throw new InvalidOperationException("Only accepted trade offers can be completed.");

        Status = Status.completed;
        AddDomainEvent(new TradeOfferCompletedEvent(Id));
    }
}
