using Application.DTOs;
using MediatR;

namespace Application.Shelfs.Commands.CreateShelf;

public record CreateShelfCommand(Guid UserId, Guid BookId, string CopyType, string AcquiredSource) : IRequest<ShelfDto>;
