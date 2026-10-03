using Domain.Common.Enums;
using Domain.Shelfs.Enums;

namespace Api.Contracts;

public record RedeemBookRequest(
    Guid BookId,
    AcquiredSource AcquiredSource,
    CopyType CopyType
);
