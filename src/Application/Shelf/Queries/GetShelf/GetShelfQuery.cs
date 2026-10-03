using Application.DTOs;
using MediatR;

namespace Application.Shelfs.Queries.GetShelf;

public record GetShelfQuery(Guid Id) : IRequest<ShelfDto?>;
