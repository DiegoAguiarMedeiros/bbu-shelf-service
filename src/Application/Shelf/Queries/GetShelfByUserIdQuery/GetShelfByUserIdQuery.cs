using Application.DTOs;
using MediatR;

namespace Application.Shelfs.Queries.GetShelfByUserId;

public record GetShelfByUserIdQuery(Guid Id) : IRequest<IEnumerable<ShelfDto>>;
