using InsuranceApi.Models;
using InsuranceApi.Services.Quotes;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Data;

public sealed class QuoteRepository : IQuoteRepository
{
    private readonly InsuranceDbContext _dbContext;

    public QuoteRepository(InsuranceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<Quote>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Quotes.ToListAsync(cancellationToken);
    }

    public Task<Quote?> GetByIdAsync(int quoteId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Quotes.FirstOrDefaultAsync(q => q.QuoteId == quoteId, cancellationToken);
    }

    public async Task AddAsync(Quote quote, CancellationToken cancellationToken = default)
    {
        await _dbContext.Quotes.AddAsync(quote, cancellationToken);
    }

    public Task UpdateAsync(Quote quote, CancellationToken cancellationToken = default)
    {
        _dbContext.Quotes.Update(quote);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Quote quote, CancellationToken cancellationToken = default)
    {
        _dbContext.Quotes.Remove(quote);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}