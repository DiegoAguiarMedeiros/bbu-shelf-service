using Application.Interfaces.ITradeOfferRepository;
using Domain.TradeOffers;
using Id = Domain.Common.Id;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence.Repositories;

public class TradeOfferRepository : ITradeOfferRepository
{
    private readonly ShelfDbContext _context;

    public TradeOfferRepository(ShelfDbContext context)
    {
        _context = context;
    }

    public async Task<TradeOffer?> GetByIdAsync(Id id, CancellationToken cancellationToken = default)
    {
        return await _context.TradeOffers.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<TradeOffer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.TradeOffers.ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<TradeOffer>> GetAllReceivedAsync(Id userId, CancellationToken cancellationToken = default)
    {
        return await _context.TradeOffers.Where(s => s.ToUserId == userId).ToListAsync(cancellationToken);
    }
    public async Task<IEnumerable<TradeOffer>> GetAllSentAsync(Id userId, CancellationToken cancellationToken = default)
    {
        return await _context.TradeOffers.Where(s => s.FromUserId == userId).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TradeOffer tradeOffer, CancellationToken cancellationToken = default)
    {
        await _context.TradeOffers.AddAsync(tradeOffer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task UpdateAsync(TradeOffer tradeOffer, CancellationToken cancellationToken = default)
    {
        _context.TradeOffers.Update(tradeOffer);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Id id, CancellationToken cancellationToken = default)
    {
        var shelf = await GetByIdAsync(id, cancellationToken);
        if (shelf != null)
        {
            _context.TradeOffers.Remove(shelf);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
