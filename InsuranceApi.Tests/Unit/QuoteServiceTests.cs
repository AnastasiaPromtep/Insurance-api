using FluentAssertions;
using InsuranceApi.Commands;
using InsuranceApi.Models;
using InsuranceApi.Services.Quotes;

namespace InsuranceApi.Tests.Unit;

[Trait("Category", "Unit")]
public class QuoteServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreateQuote_WhenCommandIsValid()
    {
        var repository = new FakeQuoteRepository();
        var service = new QuoteService(repository);

        var command = new CreateQuoteCommand(
            new DateTime(2026, 04, 01),
            new DateTime(2026, 04, 30));

        var result = await service.CreateAsync(command);

        result.Should().NotBeNull();
        result.QuoteId.Should().Be(1);
        result.QuoteStatus.Should().Be(QuoteStatus.Draft);
        result.StartDate.Should().Be(new DateTime(2026, 04, 01));
        result.EndDate.Should().Be(new DateTime(2026, 04, 30));
        result.CreationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        repository.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentException_WhenStartDateIsDefault()
    {
        var repository = new FakeQuoteRepository();
        var service = new QuoteService(repository);

        var command = new CreateQuoteCommand(default, new DateTime(2026, 04, 30));

        var act = async () => await service.CreateAsync(command);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Start date is required*");
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowArgumentException_WhenEndDateIsEarlierThanStartDate()
    {
        var repository = new FakeQuoteRepository();
        var service = new QuoteService(repository);

        var command = new CreateQuoteCommand(
            new DateTime(2026, 04, 30),
            new DateTime(2026, 04, 01));

        var act = async () => await service.CreateAsync(command);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*End date cannot be earlier than start date*");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllQuotes_WhenQuotesExist()
    {
        var repository = new FakeQuoteRepository();
        repository.Items.AddRange(
        [
            new Quote
            {
                QuoteId = 1,
                QuoteStatus = QuoteStatus.Draft,
                CreationDate = new DateTime(2026, 01, 01),
                StartDate = new DateTime(2026, 02, 01),
                EndDate = new DateTime(2026, 02, 28)
            },
            new Quote
            {
                QuoteId = 2,
                QuoteStatus = QuoteStatus.Submitted,
                CreationDate = new DateTime(2026, 01, 02),
                StartDate = new DateTime(2026, 03, 01),
                EndDate = new DateTime(2026, 03, 31)
            }
        ]);

        var service = new QuoteService(repository);

        var result = await service.GetAllAsync();

        result.Should().HaveCount(2);
        result.Select(q => q.QuoteId).Should().Contain([1, 2]);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoQuotesExist()
    {
        var repository = new FakeQuoteRepository();
        var service = new QuoteService(repository);

        var result = await service.GetAllAsync();

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnQuote_WhenQuoteExists()
    {
        var repository = new FakeQuoteRepository();
        repository.Items.Add(new Quote
        {
            QuoteId = 1,
            QuoteStatus = QuoteStatus.Draft,
            CreationDate = new DateTime(2026, 01, 01),
            StartDate = new DateTime(2026, 02, 01),
            EndDate = new DateTime(2026, 02, 28)
        });

        var service = new QuoteService(repository);

        var result = await service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result.QuoteId.Should().Be(1);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowKeyNotFoundException_WhenQuoteDoesNotExist()
    {
        var repository = new FakeQuoteRepository();
        var service = new QuoteService(repository);

        var act = async () => await service.GetByIdAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteQuote_WhenQuoteExists()
    {
        var repository = new FakeQuoteRepository();
        repository.Items.Add(new Quote
        {
            QuoteId = 1,
            QuoteStatus = QuoteStatus.Draft,
            CreationDate = new DateTime(2026, 01, 01),
            StartDate = new DateTime(2026, 02, 01),
            EndDate = new DateTime(2026, 02, 28)
        });

        var service = new QuoteService(repository);

        await service.DeleteAsync(1);

        repository.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowKeyNotFoundException_WhenQuoteDoesNotExist()
    {
        var repository = new FakeQuoteRepository();
        var service = new QuoteService(repository);

        var act = async () => await service.DeleteAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task SubmitAsync_ShouldSetQuoteStatusToSubmitted_WhenQuoteExists()
    {
        var repository = new FakeQuoteRepository();
        repository.Items.Add(new Quote
        {
            QuoteId = 1,
            QuoteStatus = QuoteStatus.Draft,
            CreationDate = new DateTime(2026, 01, 01),
            StartDate = new DateTime(2026, 02, 01),
            EndDate = new DateTime(2026, 02, 28)
        });

        var service = new QuoteService(repository);

        var result = await service.SubmitAsync(1);

        result.QuoteStatus.Should().Be(QuoteStatus.Submitted);
        repository.Items.Single().QuoteStatus.Should().Be(QuoteStatus.Submitted);
    }

    [Fact]
    public async Task SubmitAsync_ShouldThrowKeyNotFoundException_WhenQuoteDoesNotExist()
    {
        var repository = new FakeQuoteRepository();
        var service = new QuoteService(repository);

        var act = async () => await service.SubmitAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task RejectAsync_ShouldSetQuoteStatusToRejected_WhenQuoteExists()
    {
        var repository = new FakeQuoteRepository();
        repository.Items.Add(new Quote
        {
            QuoteId = 1,
            QuoteStatus = QuoteStatus.Submitted,
            CreationDate = new DateTime(2026, 01, 01),
            StartDate = new DateTime(2026, 02, 01),
            EndDate = new DateTime(2026, 02, 28)
        });

        var service = new QuoteService(repository);

        var result = await service.RejectAsync(1);

        result.QuoteStatus.Should().Be(QuoteStatus.Rejected);
        repository.Items.Single().QuoteStatus.Should().Be(QuoteStatus.Rejected);
    }

    [Fact]
    public async Task RejectAsync_ShouldThrowKeyNotFoundException_WhenQuoteDoesNotExist()
    {
        var repository = new FakeQuoteRepository();
        var service = new QuoteService(repository);

        var act = async () => await service.RejectAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*999*");
    }

    [Fact]
    public async Task ExpireAsync_ShouldSetQuoteStatusToExpired_WhenQuoteExists()
    {
        var repository = new FakeQuoteRepository();
        repository.Items.Add(new Quote
        {
            QuoteId = 1,
            QuoteStatus = QuoteStatus.Submitted,
            CreationDate = new DateTime(2026, 01, 01),
            StartDate = new DateTime(2026, 02, 01),
            EndDate = new DateTime(2026, 02, 28)
        });

        var service = new QuoteService(repository);

        var result = await service.ExpireAsync(1);

        result.QuoteStatus.Should().Be(QuoteStatus.Expired);
        repository.Items.Single().QuoteStatus.Should().Be(QuoteStatus.Expired);
    }

    [Fact]
    public async Task ExpireAsync_ShouldThrowKeyNotFoundException_WhenQuoteDoesNotExist()
    {
        var repository = new FakeQuoteRepository();
        var service = new QuoteService(repository);

        var act = async () => await service.ExpireAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*999*");
    }

    private sealed class FakeQuoteRepository : IQuoteRepository
    {
        public List<Quote> Items { get; } = new();

        public Task<List<Quote>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Items.ToList());
        }

        public Task<Quote?> GetByIdAsync(int quoteId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Items.FirstOrDefault(q => q.QuoteId == quoteId));
        }

        public Task AddAsync(Quote quote, CancellationToken cancellationToken = default)
        {
            quote.QuoteId = Items.Count == 0 ? 1 : Items.Max(q => q.QuoteId) + 1;
            Items.Add(quote);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Quote quote, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Quote quote, CancellationToken cancellationToken = default)
        {
            Items.Remove(quote);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}