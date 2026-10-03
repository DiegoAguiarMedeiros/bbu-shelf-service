using Domain.TradeOffers.Enums;

namespace Application.DTOs;

public record TradeOfferDto(
    Guid Id,
    Guid FromUserId,
    Guid ToUserId,
    Status Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
