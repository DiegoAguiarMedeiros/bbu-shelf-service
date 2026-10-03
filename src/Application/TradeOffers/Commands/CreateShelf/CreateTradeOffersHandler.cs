using Application.DTOs;
using Application.Interfaces.IShelfRepository;
using Application.Interfaces.ITradeOfferRepository;
using Domain.Common;
using Domain.Shelfs;
using Domain.TradeOffers;
using Domain.TradeOffers.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.TradeOffers.Commands.CreateTradeOffer;

public class CreateTradeOfferHandler : IRequestHandler<CreateTradeOfferCommand, TradeOfferDto>
{
    private readonly IShelfRepository _shelfRepository;
    private readonly ITradeOfferRepository _tradeOfferRepository;
    private readonly ILogger<CreateTradeOfferHandler> _logger;

    public CreateTradeOfferHandler(
        IShelfRepository shelfRepository,
        ITradeOfferRepository tradeOfferRepository,
        ILogger<CreateTradeOfferHandler> logger)
    {
        _shelfRepository = shelfRepository;
        _tradeOfferRepository = tradeOfferRepository;
        _logger = logger;
    }

    public async Task<TradeOfferDto> Handle(CreateTradeOfferCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating trade offer request: {@Request}", request);

        var fromUserId = Id.From(request.FromUserId);
        var toUserId = Id.From(request.ToUserId);

        if (fromUserId == toUserId)
            throw new InvalidOperationException("Cannot create a trade offer with yourself.");

        // antes: exigia pelo menos 1 em CADA lado
        // agora: exige pelo menos 1 no TOTAL (não pode ser 0 e 0)
        if (!request.OfferedShelfIds.Any() && !request.RequestedShelfIds.Any())
            throw new InvalidOperationException("A trade offer must include at least one game from either side.");

        var tradeOffer = TradeOffer.Create(
            id: Id.New(),
            fromUserId: fromUserId,
            toUserId: toUserId,
            status: Status.pending
        );

        foreach (var shelfId in request.OfferedShelfIds)
        {
            var shelf = await _shelfRepository.GetByIdAsync(Id.From(shelfId), cancellationToken)
                ?? throw new InvalidOperationException($"Shelf with ID '{shelfId}' does not exist.");

            if (shelf.UserId != fromUserId)
                throw new InvalidOperationException($"You can only offer games that you own. Shelf '{shelfId}' is not yours.");

            ValidateTradeable(shelf);
            tradeOffer.AddEntry(fromUserId, shelf.Id);
        }

        foreach (var shelfId in request.RequestedShelfIds)
        {
            var shelf = await _shelfRepository.GetByIdAsync(Id.From(shelfId), cancellationToken)
                ?? throw new InvalidOperationException($"Shelf with ID '{shelfId}' does not exist.");

            if (shelf.UserId != toUserId)
                throw new InvalidOperationException($"The requested game '{shelfId}' does not belong to the specified user.");

            ValidateTradeable(shelf);
            tradeOffer.AddEntry(toUserId, shelf.Id);
        }

        await _tradeOfferRepository.AddAsync(tradeOffer, cancellationToken);


        return new TradeOfferDto(
            tradeOffer.Id.Value,
            tradeOffer.FromUserId.Value,
            tradeOffer.ToUserId.Value,
            tradeOffer.Status,
            tradeOffer.CreatedAt,
            tradeOffer.UpdatedAt
        );
    }

    private static void ValidateTradeable(Shelf shelf)
    {
        if (shelf.CopyType != Domain.Shelfs.Enums.CopyType.Physical)
            throw new InvalidOperationException($"O jogo '{shelf.Id.Value}' não é uma cópia física e não pode ser negociado.");
    }
}