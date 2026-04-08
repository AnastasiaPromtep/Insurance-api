using InsuranceApi.Commands;
using InsuranceApi.Errors;
using InsuranceApi.Models;
using InsuranceApi.Requests;
using InsuranceApi.Services.Policies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Controllers;

[ApiController]
[Route("policies")]
public class PoliciesController : ControllerBase
{
    private readonly IPolicyRepository _repository;
    private readonly PolicyService _policyService;

    public PoliciesController(
        IPolicyRepository repository,
        PolicyService policyService)
    {
        _repository = repository;
        _policyService = policyService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Policy>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var policies = await _repository.GetAllAsync(cancellationToken);
        return Ok(policies);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Policy), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var policy = await _repository.GetByIdAsync(id, cancellationToken);

        if (policy is null)
        {
            return ProblemFactory.NotFound(
                ErrorCodes.Policy.NotFound,
                $"Policy with id '{id}' was not found.");
        }

        return Ok(policy);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Policy), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePolicyRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var policy = await _policyService.CreateAsync(
                new CreatePolicyCommand(
                    request.PolicyNumber,
                    request.SubscriberName,
                    request.PremiumAmount,
                    request.StartDate),
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = policy.Id }, policy);
        }
        catch (DuplicatePolicyNumberException ex)
        {
            return ProblemFactory.Conflict(
                ErrorCodes.Policy.Duplicate,
                ex.Message);
        }
        catch (DbUpdateException)
        {
            return ProblemFactory.Conflict(
                ErrorCodes.Policy.Duplicate,
                $"A policy with number '{request.PolicyNumber}' already exists.");
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Policy), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdatePolicyRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var policy = await _policyService.UpdateAsync(
                new UpdatePolicyCommand(
                    id,
                    request.PolicyNumber,
                    request.SubscriberName,
                    request.PremiumAmount,
                    request.StartDate),
                cancellationToken);

            return Ok(policy);
        }
        catch (ArgumentException ex)
        {
            var code = ex.ParamName switch
            {
                nameof(UpdatePolicyCommand.PolicyNumber) => ErrorCodes.Policy.InvalidPolicyNumber,
                nameof(UpdatePolicyCommand.SubscriberName) => ErrorCodes.Policy.InvalidSubscriberName,
                nameof(UpdatePolicyCommand.PremiumAmount) => ErrorCodes.Policy.InvalidPremiumAmount,
                _ => ErrorCodes.Policy.InvalidPolicyNumber
            };

            return ProblemFactory.BadRequest(code, ex.Message);
        }
        catch (KeyNotFoundException)
        {
            return ProblemFactory.NotFound(
                ErrorCodes.Policy.NotFound,
                $"Policy with id '{id}' was not found.");
        }
        catch (DuplicatePolicyNumberException ex)
        {
            return ProblemFactory.Conflict(
                ErrorCodes.Policy.Duplicate,
                ex.Message);
        }
        catch (DbUpdateException)
        {
            return ProblemFactory.Conflict(
                ErrorCodes.Policy.Duplicate,
                $"A policy with number '{request.PolicyNumber}' already exists.");
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _policyService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return ProblemFactory.NotFound(
                ErrorCodes.Policy.NotFound,
                $"Policy with id '{id}' was not found.");
        }
    }
}