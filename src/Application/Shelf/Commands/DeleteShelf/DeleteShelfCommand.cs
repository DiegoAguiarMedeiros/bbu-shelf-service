using MediatR;

namespace Application.Shelfs.Commands.DeleteShelf;

public record DeleteShelfCommand(Guid Id) : IRequest<bool>;
