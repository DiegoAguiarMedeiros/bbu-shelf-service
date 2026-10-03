namespace Application.DTOs;

public record ShelfDto(
    Guid Id,
    Guid UserId,
    Guid BookId,
    string CopyType,
    string AcquiredSource,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
