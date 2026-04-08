using InsuranceApi.Commands;
using InsuranceApi.Models;

namespace InsuranceApi.Services.Quotes;

public sealed class QuoteService
{
    private readonly IQuoteRepository _repository;

    public QuoteService(IQuoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Quote>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }

    public async Task<Quote> GetByIdAsync(int quoteId, CancellationToken cancellationToken = default)
    {
        var quote = await _repository.GetByIdAsync(quoteId, cancellationToken);

        if (quote is null)
        {
            throw new KeyNotFoundException($"Quote with id '{quoteId}' was not found.");
        }

        return quote;
    }

    public async Task<Quote> CreateAsync(
    CreateQuoteCommand command,
    CancellationToken cancellationToken = default)
    {
        if (command.StartDate == default)
        {
            throw new ArgumentException("Start date is required.", nameof(command.StartDate));
        }

        if (command.EndDate.HasValue && command.EndDate.Value < command.StartDate)
        {
            throw new ArgumentException("End date cannot be earlier than start date.", nameof(command.EndDate));
        }

        var quote = new Quote
        {
            QuoteStatus = QuoteStatus.Draft,
            CreationDate = DateTime.UtcNow,
            StartDate = command.StartDate,
            EndDate = command.EndDate
        };

        await _repository.AddAsync(quote, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return quote;
    }

    public async Task DeleteAsync(int quoteId, CancellationToken cancellationToken = default)
    {
        var quote = await _repository.GetByIdAsync(quoteId, cancellationToken);

        if (quote is null)
        {
            throw new KeyNotFoundException($"Quote with id '{quoteId}' was not found.");
        }

        await _repository.DeleteAsync(quote, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task<Quote> SubmitAsync(int quoteId, CancellationToken cancellationToken = default)
    {
        var quote = await _repository.GetByIdAsync(quoteId, cancellationToken);

        if (quote is null)
        {
            throw new KeyNotFoundException($"Quote with id '{quoteId}' was not found.");
        }

        quote.QuoteStatus = QuoteStatus.Submitted;

        await _repository.UpdateAsync(quote, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return quote;
    }

    public async Task<Quote> RejectAsync(int quoteId, CancellationToken cancellationToken = default)
    {
        var quote = await _repository.GetByIdAsync(quoteId, cancellationToken);

        if (quote is null)
        {
            throw new KeyNotFoundException($"Quote with id '{quoteId}' was not found.");
        }

        quote.QuoteStatus = QuoteStatus.Rejected;

        await _repository.UpdateAsync(quote, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return quote;
    }

    public async Task<Quote> ExpireAsync(int quoteId, CancellationToken cancellationToken = default)
    {
        var quote = await _repository.GetByIdAsync(quoteId, cancellationToken);

        if (quote is null)
        {
            throw new KeyNotFoundException($"Quote with id '{quoteId}' was not found.");
        }

        quote.QuoteStatus = QuoteStatus.Expired;

        await _repository.UpdateAsync(quote, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return quote;
    }
}