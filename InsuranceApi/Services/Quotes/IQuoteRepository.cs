using InsuranceApi.Models;

namespace InsuranceApi.Services.Quotes;

public interface IQuoteRepository
{
    Task<List<Quote>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Quote?> GetByIdAsync(int quoteId, CancellationToken cancellationToken = default);

    Task AddAsync(Quote quote, CancellationToken cancellationToken = default);

    Task UpdateAsync(Quote quote, CancellationToken cancellationToken = default);

    Task DeleteAsync(Quote quote, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}