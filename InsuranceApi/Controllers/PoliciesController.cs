using InsuranceApi.Models;
using InsuranceApi.Services.Policies;
using InsuranceApi.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Controllers;

[ApiController]
[Route("policies")]
public class PoliciesController : ControllerBase
{
    private readonly IPolicyRepository _repository;
    private readonly PolicyCreationService _creationService;

    public PoliciesController(
        IPolicyRepository repository,
        PolicyCreationService creationService)
    {
        _repository = repository;
        _creationService = creationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Policy>>> GetAll(CancellationToken cancellationToken)
    {
        var policies = await _repository.GetAllAsync(cancellationToken);
        return Ok(policies);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Policy>> GetById(int id, CancellationToken cancellationToken)
    {
        var policy = await _repository.GetByIdAsync(id, cancellationToken);

        if (policy is null)
        {
            return NotFound();
        }

        return Ok(policy);
    }

    [HttpPost]
    public async Task<ActionResult<Policy>> Create(
        [FromBody] CreatePolicyRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var policy = await _creationService.CreateAsync(
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
            return Conflict(new
            {
                code = "policy_number_already_exists",
                message = ex.Message
            });
        }
        catch (DbUpdateException)
        {
            return Conflict(new
            {
                code = "policy_number_already_exists",
                message = $"A policy with number '{request.PolicyNumber}' already exists."
            });
        }
    }
}