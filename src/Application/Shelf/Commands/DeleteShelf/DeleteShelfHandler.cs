using Application.Interfaces.IShelfRepository;
using MediatR;
using Id = Domain.Common.Id;

namespace Application.Shelfs.Commands.DeleteShelf;

public class DeleteShelfHandler : IRequestHandler<DeleteShelfCommand, bool>
{
    private readonly IShelfRepository _shelfRepository;

    public DeleteShelfHandler(IShelfRepository userRepository)
    {
        _shelfRepository = userRepository;
    }

    public async Task<bool> Handle(DeleteShelfCommand request, CancellationToken cancellationToken)
    {
        var userId = Id.From(request.Id);
        var user = await _shelfRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        await _shelfRepository.DeleteAsync(userId, cancellationToken);
        return true;
    }
}
