using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.IShelfRepository;
using MediatR;
using Id = Domain.Common.Id;
using Microsoft.Extensions.Logging;

namespace Application.Shelfs.Queries.GetShelf;

public class GetShelfHandler : IRequestHandler<GetShelfQuery, ShelfDto?>
{
    private readonly IShelfRepository _shelfRepository;
    private readonly ILogger<GetShelfHandler> _logger;

    public GetShelfHandler(IShelfRepository userRepository,
        ILogger<GetShelfHandler> logger)
    {
        _shelfRepository = userRepository;
        _logger = logger;
    }

    public async Task<ShelfDto?> Handle(GetShelfQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching user with Id {Id}", request.Id);
        var userTask = _shelfRepository.GetByIdAsync(Id.From(request.Id), cancellationToken);


        var user = await userTask;

        if (user == null)
        {
            _logger.LogInformation("Shelf with Id {Id} was not found", request.Id);
            return null;
        }
        _logger.LogInformation("Shelf with Id {Id} found", user.ToString());
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
