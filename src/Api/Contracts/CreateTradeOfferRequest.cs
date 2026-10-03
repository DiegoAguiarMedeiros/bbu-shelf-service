using Domain.Common.Enums;
using Domain.Shelfs.Enums;

namespace Api.Contracts;

public record CreateTradeOfferRequest(
    Guid ToUserId,
    List<Guid> OfferedShelfIds,
    List<Guid> RequestedShelfIds
);
