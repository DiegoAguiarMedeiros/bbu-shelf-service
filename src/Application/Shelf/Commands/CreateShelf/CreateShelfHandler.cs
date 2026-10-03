using Application.DTOs;
using Application.Interfaces.IShelfRepository;
using Domain.Common;
using MediatR;
using Shelf = Domain.Shelfs.Shelf;

namespace Application.Shelfs.Commands.CreateShelf;

public class CreateShelfHandler : IRequestHandler<CreateShelfCommand, ShelfDto>
{
    private readonly IShelfRepository _shelfRepository;

    public CreateShelfHandler(IShelfRepository userRepository)
    {
        _shelfRepository = userRepository;
    }

    public async Task<ShelfDto> Handle(CreateShelfCommand request, CancellationToken cancellationToken)
    {
        var userId = Id.From(request.UserId);
        var bookId = Id.From(request.BookId);
        var acquiredSource = Enum.Parse<Domain.Common.Enums.AcquiredSource>(request.AcquiredSource);
        var copyType = Enum.Parse<Domain.Shelfs.Enums.CopyType>(request.CopyType);

        var user = Shelf.Create(Id.New(), userId, bookId, acquiredSource, copyType);
        await _shelfRepository.AddAsync(user, cancellationToken);

        return new ShelfDto(
            user.Id.Value,
            user.UserId.Value,
            user.BookId.Value,
            user.AcquiredSource.ToString(),
            user.CopyType.ToString(),
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}
