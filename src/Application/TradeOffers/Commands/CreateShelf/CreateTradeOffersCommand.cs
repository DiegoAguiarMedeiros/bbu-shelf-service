using Application.DTOs;
using MediatR;

namespace Application.TradeOffers.Commands.CreateTradeOffer;

public record CreateTradeOfferCommand(
    Guid FromUserId,
    Guid ToUserId,
    List<Guid> OfferedShelfIds,
    List<Guid> RequestedShelfIds
) : IRequest<TradeOfferDto>;
