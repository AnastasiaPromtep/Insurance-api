using InsuranceApi.Commands;
using InsuranceApi.Errors;
using InsuranceApi.Models;
using InsuranceApi.Requests;
using InsuranceApi.Services.Quotes;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceApi.Controllers;

[ApiController]
[Route("quotes")]
public class QuotesController : ControllerBase
{
    private readonly QuoteService _quoteService;

    public QuotesController(QuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Quote>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var quotes = await _quoteService.GetAllAsync(cancellationToken);
        return Ok(quotes);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Quote), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        try
        {
            var quote = await _quoteService.GetByIdAsync(id, cancellationToken);
            return Ok(quote);
        }
        catch (KeyNotFoundException)
        {
            return ProblemFactory.NotFound(
                ErrorCodes.Quote.NotFound,
                $"Quote with id '{id}' was not found.");
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(Quote), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateQuoteRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var quote = await _quoteService.CreateAsync(
                new CreateQuoteCommand(
                    request.StartDate,
                    request.EndDate),
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = quote.QuoteId }, quote);
        }
        catch (ArgumentException ex)
        {
            var code = ex.ParamName switch
            {
                nameof(CreateQuoteCommand.StartDate) => ErrorCodes.Quote.InvalidStartDate,
                nameof(CreateQuoteCommand.EndDate) => ErrorCodes.Quote.InvalidEndDate,
                _ => ErrorCodes.Quote.InvalidStartDate
            };

            return ProblemFactory.BadRequest(code, ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _quoteService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return ProblemFactory.NotFound(
                ErrorCodes.Quote.NotFound,
                $"Quote with id '{id}' was not found.");
        }
    }

    [HttpPost("{id:int}/submit")]
    [ProducesResponseType(typeof(Quote), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Submit(int id, CancellationToken cancellationToken)
    {
        try
        {
            var quote = await _quoteService.SubmitAsync(id, cancellationToken);
            return Ok(quote);
        }
        catch (KeyNotFoundException)
        {
            return ProblemFactory.NotFound(
                ErrorCodes.Quote.NotFound,
                $"Quote with id '{id}' was not found.");
        }
    }

    [HttpPost("{id:int}/reject")]
    [ProducesResponseType(typeof(Quote), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(int id, CancellationToken cancellationToken)
    {
        try
        {
            var quote = await _quoteService.RejectAsync(id, cancellationToken);
            return Ok(quote);
        }
        catch (KeyNotFoundException)
        {
            return ProblemFactory.NotFound(
                ErrorCodes.Quote.NotFound,
                $"Quote with id '{id}' was not found.");
        }
    }

    [HttpPost("{id:int}/expire")]
    [ProducesResponseType(typeof(Quote), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Expire(int id, CancellationToken cancellationToken)
    {
        try
        {
            var quote = await _quoteService.ExpireAsync(id, cancellationToken);
            return Ok(quote);
        }
        catch (KeyNotFoundException)
        {
            return ProblemFactory.NotFound(
                ErrorCodes.Quote.NotFound,
                $"Quote with id '{id}' was not found.");
        }
    }
}