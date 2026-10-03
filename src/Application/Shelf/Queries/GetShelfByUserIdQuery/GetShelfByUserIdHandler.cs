using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.IShelfRepository;
using MediatR;
using Id = Domain.Common.Id;
using Microsoft.Extensions.Logging;

namespace Application.Shelfs.Queries.GetShelfByUserId;

public class GetShelfByUserIdHandler : IRequestHandler<GetShelfByUserIdQuery, IEnumerable<ShelfDto>>
{
    private readonly IShelfRepository _shelfRepository;
    private readonly ILogger<GetShelfByUserIdHandler> _logger;

    public GetShelfByUserIdHandler(IShelfRepository userRepository,
        ILogger<GetShelfByUserIdHandler> logger)
    {
        _shelfRepository = userRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<ShelfDto>> Handle(GetShelfByUserIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching shelf with userId {Id}", request.Id);
        var shelfTask = _shelfRepository.GetAllByUserIdAsync(Id.From(request.Id), cancellationToken);


        var shelf = await shelfTask;

        if (shelf == null)
        {
            _logger.LogInformation("Shelf with Id {Id} was not found", request.Id);
            return null;
        }
        _logger.LogInformation("Shelf with Id {Id} found", request.Id);

        return shelf.Select(shelf => new ShelfDto(
                shelf.Id.Value,
                shelf.UserId.Value,
                shelf.BookId.Value,
                shelf.AcquiredSource.ToString(),
                shelf.CopyType.ToString(),
                shelf.CreatedAt,
                shelf.UpdatedAt
            ));
    }
}
