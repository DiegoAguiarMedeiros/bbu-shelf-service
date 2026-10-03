using Domain.Common;
using Domain.Shelfs;
namespace Application.Interfaces.IShelfRepository;
public interface IShelfRepository
{
    Task<Shelf?> GetByIdAsync(Id id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Shelf>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Shelf>> GetAllByUserIdAsync(Id userId, CancellationToken cancellationToken = default);
    Task AddAsync(Shelf shelf, CancellationToken cancellationToken = default);
    Task UpdateAsync(Shelf shelf, CancellationToken cancellationToken = default);
    Task DeleteAsync(Id id, CancellationToken cancellationToken = default);
}
