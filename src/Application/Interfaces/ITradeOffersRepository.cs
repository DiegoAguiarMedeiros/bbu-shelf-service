using Domain.Common;
using Domain.TradeOffers;
namespace Application.Interfaces.ITradeOfferRepository;
public interface ITradeOfferRepository
{
    Task<TradeOffer?> GetByIdAsync(Id id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TradeOffer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<TradeOffer>> GetAllSentAsync(Id userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TradeOffer>> GetAllReceivedAsync(Id userId, CancellationToken cancellationToken = default);
    Task AddAsync(TradeOffer tradeOffer, CancellationToken cancellationToken = default);
    Task UpdateAsync(TradeOffer tradeOffer, CancellationToken cancellationToken = default);
    Task DeleteAsync(Id id, CancellationToken cancellationToken = default);
}
