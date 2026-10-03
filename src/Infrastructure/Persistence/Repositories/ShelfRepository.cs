using Application.Interfaces.IShelfRepository;
using Domain.Shelfs;
using Id = Domain.Common.Id;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence.Repositories;

public class ShelfRepository : IShelfRepository
{
    private readonly ShelfDbContext _context;

    public ShelfRepository(ShelfDbContext context)
    {
        _context = context;
    }

    public async Task<Shelf?> GetByIdAsync(Id id, CancellationToken cancellationToken = default)
    {
        return await _context.Shelfs.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Shelf>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Shelfs.ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<Shelf>> GetAllByUserIdAsync(Id userId, CancellationToken cancellationToken = default)
    {
        return await _context.Shelfs.Where(s => s.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Shelf shelf, CancellationToken cancellationToken = default)
    {
        await _context.Shelfs.AddAsync(shelf, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Shelf shelf, CancellationToken cancellationToken = default)
    {
        _context.Shelfs.Update(shelf);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Id id, CancellationToken cancellationToken = default)
    {
        var shelf = await GetByIdAsync(id, cancellationToken);
        if (shelf != null)
        {
            _context.Shelfs.Remove(shelf);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
