using InsuranceApi.Commands;
using InsuranceApi.Models;

namespace InsuranceApi.Services.Policies;

public sealed class PolicyService
{
    private readonly IPolicyRepository _repository;

    public PolicyService(IPolicyRepository repository)
    {
        _repository = repository;
    }

    public async Task<Policy> CreateAsync(CreatePolicyCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.PolicyNumber))
        {
            throw new ArgumentException("Policy number is required.", nameof(command.PolicyNumber));
        }

        if (string.IsNullOrWhiteSpace(command.SubscriberName))
        {
            throw new ArgumentException("Subscriber name is required.", nameof(command.SubscriberName));
        }

        if (command.PremiumAmount <= 0)
        {
            throw new ArgumentException("Premium amount must be greater than zero.", nameof(command.PremiumAmount));
        }

        var alreadyExists = await _repository.ExistsByPolicyNumberAsync(
            command.PolicyNumber,
            cancellationToken);

        if (alreadyExists)
        {
            throw new DuplicatePolicyNumberException(command.PolicyNumber);
        }

        var policy = new Policy
        {
            PolicyNumber = command.PolicyNumber,
            SubscriberName = command.SubscriberName,
            PremiumAmount = command.PremiumAmount,
            StartDate = command.StartDate,
            Status = PolicyStatus.Draft
        };

        await _repository.AddAsync(policy, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return policy;
    }

    public async Task<Policy> UpdateAsync(UpdatePolicyCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.PolicyNumber))
        {
            throw new ArgumentException("Policy number is required.", nameof(command.PolicyNumber));
        }

        if (string.IsNullOrWhiteSpace(command.SubscriberName))
        {
            throw new ArgumentException("Subscriber name is required.", nameof(command.SubscriberName));
        }

        if (command.PremiumAmount <= 0)
        {
            throw new ArgumentException("Premium amount must be greater than zero.", nameof(command.PremiumAmount));
        }

        var policy = await _repository.GetByPolicyNumberAsync(command.PolicyNumber, cancellationToken);
        if (policy == null)
        {
            throw new KeyNotFoundException($"Policy with number {command.PolicyNumber} not found.");
        }

        policy.PolicyNumber = command.PolicyNumber;
        policy.SubscriberName = command.SubscriberName;
        policy.PremiumAmount = command.PremiumAmount;
        policy.StartDate = command.StartDate;

        await _repository.SaveChangesAsync(cancellationToken);

        return policy;
    }

    public async Task DeleteAsync(string policyNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(policyNumber))
        {
            throw new ArgumentException("Policy number is required.", nameof(policyNumber));
        }

        var policy = await _repository.GetByPolicyNumberAsync(policyNumber, cancellationToken);
        if (policy == null)
        {
            throw new KeyNotFoundException($"Policy with number {policyNumber} not found.");
        }

        await _repository.DeleteAsync(policy, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}