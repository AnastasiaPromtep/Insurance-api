using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using InsuranceApi.Models;

namespace InsuranceApi.Tests.Integration;

public class QuoteControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public QuoteControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetQuotes_ShouldReturnOkWithEmptyList_WhenNoQuotesExist()
    {
        var response = await _client.GetAsync("/quotes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var quotes = await response.Content.ReadFromJsonAsync<List<Quote>>();

        quotes.Should().NotBeNull();
        quotes.Should().BeEmpty();
    }

    [Fact]
    public async Task GetQuotes_ShouldReturnOk_WhenQuotesExist()
    {
        var request1 = new
        {
            startDate = "2026-04-01T00:00:00Z",
            endDate = "2026-04-30T00:00:00Z"
        };

        var request2 = new
        {
            startDate = "2026-05-01T00:00:00Z",
            endDate = "2026-05-31T00:00:00Z"
        };

        var createResponse1 = await _client.PostAsJsonAsync("/quotes", request1);
        var createResponse2 = await _client.PostAsJsonAsync("/quotes", request2);

        createResponse1.StatusCode.Should().Be(HttpStatusCode.Created);
        createResponse2.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await _client.GetAsync("/quotes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var quotes = await response.Content.ReadFromJsonAsync<List<Quote>>();

        quotes.Should().NotBeNull();
        quotes.Should().HaveCount(2);
    }

    [Fact]
    public async Task PostQuotes_ShouldReturnCreated_WhenRequestIsValid()
    {
        var request = new
        {
            startDate = "2026-04-01T00:00:00Z",
            endDate = "2026-04-30T00:00:00Z"
        };

        var response = await _client.PostAsJsonAsync("/quotes", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var quote = await response.Content.ReadFromJsonAsync<Quote>();

        quote.Should().NotBeNull();
        quote!.QuoteId.Should().BeGreaterThan(0);
        quote.QuoteStatus.Should().Be(QuoteStatus.Draft);
        quote.StartDate.Should().Be(DateTime.Parse("2026-04-01T00:00:00Z").ToUniversalTime());
        quote.EndDate.Should().Be(DateTime.Parse("2026-04-30T00:00:00Z").ToUniversalTime());
        quote.CreationDate.Should().NotBe(default);
    }

    [Fact]
    public async Task PostQuotes_ShouldReturnProblemDetails_WhenEndDateIsEarlierThanStartDate()
    {
        var request = new
        {
            startDate = "2026-04-30T00:00:00Z",
            endDate = "2026-04-01T00:00:00Z"
        };

        var response = await _client.PostAsJsonAsync("/quotes", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Bad request");
        problem.Status.Should().Be((int)HttpStatusCode.BadRequest);
        problem.Code.Should().Be("invalid_quote_end_date");
        problem.Detail.Should().Contain("End date");
    }

    [Fact]
    public async Task GetQuoteById_ShouldReturnOk_WhenQuoteExists()
    {
        var createRequest = new
        {
            startDate = "2026-05-01T00:00:00Z",
            endDate = "2026-05-31T00:00:00Z"
        };

        var createResponse = await _client.PostAsJsonAsync("/quotes", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdQuote = await createResponse.Content.ReadFromJsonAsync<Quote>();
        createdQuote.Should().NotBeNull();

        var response = await _client.GetAsync($"/quotes/{createdQuote!.QuoteId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var quote = await response.Content.ReadFromJsonAsync<Quote>();

        quote.Should().NotBeNull();
        quote!.QuoteId.Should().Be(createdQuote.QuoteId);
        quote.QuoteStatus.Should().Be(QuoteStatus.Draft);
    }

    [Fact]
    public async Task GetQuoteById_ShouldReturnProblemDetails_WhenQuoteDoesNotExist()
    {
        var response = await _client.GetAsync("/quotes/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Not found");
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Code.Should().Be("quote_not_found");
        problem.Detail.Should().Contain("999");
    }

    [Fact]
    public async Task DeleteQuotes_ShouldReturnNoContent_WhenQuoteExists()
    {
        var createRequest = new
        {
            startDate = "2026-06-01T00:00:00Z",
            endDate = "2026-06-30T00:00:00Z"
        };

        var createResponse = await _client.PostAsJsonAsync("/quotes", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdQuote = await createResponse.Content.ReadFromJsonAsync<Quote>();
        createdQuote.Should().NotBeNull();

        var deleteResponse = await _client.DeleteAsync($"/quotes/{createdQuote!.QuoteId}");

        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteQuotes_ShouldReturnProblemDetails_WhenQuoteDoesNotExist()
    {
        var response = await _client.DeleteAsync("/quotes/999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Not found");
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Code.Should().Be("quote_not_found");
        problem.Detail.Should().Contain("999");
    }

    [Fact]
    public async Task SubmitQuote_ShouldReturnOk_WhenQuoteExists()
    {
        var createRequest = new
        {
            startDate = "2026-07-01T00:00:00Z",
            endDate = "2026-07-31T00:00:00Z"
        };

        var createResponse = await _client.PostAsJsonAsync("/quotes", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdQuote = await createResponse.Content.ReadFromJsonAsync<Quote>();
        createdQuote.Should().NotBeNull();

        var response = await _client.PostAsync($"/quotes/{createdQuote!.QuoteId}/submit", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedQuote = await response.Content.ReadFromJsonAsync<Quote>();

        updatedQuote.Should().NotBeNull();
        updatedQuote!.QuoteId.Should().Be(createdQuote.QuoteId);
        updatedQuote.QuoteStatus.Should().Be(QuoteStatus.Submitted);
    }

    [Fact]
    public async Task SubmitQuote_ShouldReturnProblemDetails_WhenQuoteDoesNotExist()
    {
        var response = await _client.PostAsync("/quotes/999/submit", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Not found");
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Code.Should().Be("quote_not_found");
        problem.Detail.Should().Contain("999");
    }

    [Fact]
    public async Task RejectQuote_ShouldReturnOk_WhenQuoteExists()
    {
        var createRequest = new
        {
            startDate = "2026-08-01T00:00:00Z",
            endDate = "2026-08-31T00:00:00Z"
        };

        var createResponse = await _client.PostAsJsonAsync("/quotes", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdQuote = await createResponse.Content.ReadFromJsonAsync<Quote>();
        createdQuote.Should().NotBeNull();

        var response = await _client.PostAsync($"/quotes/{createdQuote!.QuoteId}/reject", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedQuote = await response.Content.ReadFromJsonAsync<Quote>();

        updatedQuote.Should().NotBeNull();
        updatedQuote!.QuoteId.Should().Be(createdQuote.QuoteId);
        updatedQuote.QuoteStatus.Should().Be(QuoteStatus.Rejected);
    }

    [Fact]
    public async Task RejectQuote_ShouldReturnProblemDetails_WhenQuoteDoesNotExist()
    {
        var response = await _client.PostAsync("/quotes/999/reject", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Not found");
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Code.Should().Be("quote_not_found");
        problem.Detail.Should().Contain("999");
    }

    [Fact]
    public async Task ExpireQuote_ShouldReturnOk_WhenQuoteExists()
    {
        var createRequest = new
        {
            startDate = "2026-09-01T00:00:00Z",
            endDate = "2026-09-30T00:00:00Z"
        };

        var createResponse = await _client.PostAsJsonAsync("/quotes", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdQuote = await createResponse.Content.ReadFromJsonAsync<Quote>();
        createdQuote.Should().NotBeNull();

        var response = await _client.PostAsync($"/quotes/{createdQuote!.QuoteId}/expire", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedQuote = await response.Content.ReadFromJsonAsync<Quote>();

        updatedQuote.Should().NotBeNull();
        updatedQuote!.QuoteId.Should().Be(createdQuote.QuoteId);
        updatedQuote.QuoteStatus.Should().Be(QuoteStatus.Expired);
    }

    [Fact]
    public async Task ExpireQuote_ShouldReturnProblemDetails_WhenQuoteDoesNotExist()
    {
        var response = await _client.PostAsync("/quotes/999/expire", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>();

        problem.Should().NotBeNull();
        problem!.Title.Should().Be("Not found");
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
        problem.Code.Should().Be("quote_not_found");
        problem.Detail.Should().Contain("999");
    }
}