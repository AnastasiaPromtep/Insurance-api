using InsuranceApi.Commands;
using InsuranceApi.Models;
using InsuranceApi.Services.Policies;
using FluentAssertions;

namespace InsuranceApi.Tests.Unit;

public class PolicyCreationServiceTests
{
    [Fact]
    public async Task CreateAsync_ShouldCreatePolicy_WhenPolicyNumberDoesNotExist()
    {
        var repository = new FakePolicyRepository();
        var service = new PolicyService(repository);

        var command = new CreatePolicyCommand(
            "POL-001",
            "Alice Dupont",
            1200m,
            new DateTime(2026, 04, 01));

        var result = await service.CreateAsync(command);

        result.PolicyNumber.Should().Be("POL-001");
        result.SubscriberName.Should().Be("Alice Dupont");
        result.PremiumAmount.Should().Be(1200m);
        result.Status.Should().Be(PolicyStatus.Draft);

        repository.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenPolicyNumberAlreadyExists()
    {
        var repository = new FakePolicyRepository();
        repository.Items.Add(new Policy
        {
            Id = 1,
            PolicyNumber = "POL-001",
            SubscriberName = "Existing Customer",
            PremiumAmount = 1000m,
            StartDate = new DateTime(2026, 01, 01),
            Status = PolicyStatus.Active
        });

        var service = new PolicyService(repository);

        var command = new CreatePolicyCommand(
            "POL-001",
            "Alice Dupont",
            1200m,
            new DateTime(2026, 04, 01));

        var act = async () => await service.CreateAsync(command);

        await act.Should().ThrowAsync<DuplicatePolicyNumberException>()
            .WithMessage("*POL-001*");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePolicy_WhenCommandIsValid()
    {
        var repository = new FakePolicyRepository();
        repository.Items.Add(new Policy
        {
            Id = 1,
            PolicyNumber = "POL-001",
            SubscriberName = "Alice Dupont",
            PremiumAmount = 1200m,
            StartDate = new DateTime(2026, 04, 01),
            Status = PolicyStatus.Draft
        });

        var service = new PolicyService(repository);

        var command = new UpdatePolicyCommand(
            1,
            "POL-001",
            "Alice Dupont Updated",
            1500m,
            new DateTime(2026, 05, 01));

        var result = await service.UpdateAsync(command);

        result.Id.Should().Be(1);
        result.SubscriberName.Should().Be("Alice Dupont Updated");
        result.PremiumAmount.Should().Be(1500m);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowValidationException_WhenUpdateCommandIsInvalid()
    {
        var repository = new FakePolicyRepository();
        repository.Items.Add(new Policy
        {
            Id = 1,
            PolicyNumber = "POL-001",
            SubscriberName = "Alice Dupont",
            PremiumAmount = 1200m,
            StartDate = new DateTime(2026, 04, 01),
            Status = PolicyStatus.Draft
        });

        var service = new PolicyService(repository);

        var command = new UpdatePolicyCommand(
            1,
            "POL-001",
            "",
            -100m,
            new DateTime(2026, 05, 01));

        var act = async () => await service.UpdateAsync(command);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowNotFoundException_WhenUpdatingNonExistingPolicy()
    {
        var repository = new FakePolicyRepository();
        var service = new PolicyService(repository);

        var command = new UpdatePolicyCommand(
            999,
            "999",
            "Alice Dupont",
            1500m,
            new DateTime(2026, 05, 01));

        var act = async () => await service.UpdateAsync(command);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeletePolicy_WhenPolicyExists()
    {
        var repository = new FakePolicyRepository();
        repository.Items.Add(new Policy
        {
            Id = 1,
            PolicyNumber = "POL-001",
            SubscriberName = "Alice Dupont",
            PremiumAmount = 1200m,
            StartDate = new DateTime(2026, 04, 01),
            Status = PolicyStatus.Draft
        });

        var service = new PolicyService(repository);

        await service.DeleteAsync(1);

        repository.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentException_WhenPolicyNumberIsInvalid()
    {
        var repository = new FakePolicyRepository();
        var service = new PolicyService(repository);

        var act = async () => await service.DeleteAsync(0);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenPolicyDoesNotExist()
    {
        var repository = new FakePolicyRepository();
        var service = new PolicyService(repository);

        var act = async () => await service.DeleteAsync(999);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    private sealed class FakePolicyRepository : IPolicyRepository
    {
        public List<Policy> Items { get; } = new();

        public Task<bool> ExistsByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                Items.Any(p => p.PolicyNumber == policyNumber));
        }

        public Task AddAsync(Policy policy, CancellationToken cancellationToken = default)
        {
            policy.Id = Items.Count == 0 ? 1 : Items.Max(p => p.Id) + 1;
            Items.Add(policy);
            return Task.CompletedTask;
        }

        public Task<Policy?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Items.FirstOrDefault(p => p.Id == id));
        }

        public Task<Policy?> GetByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Items.FirstOrDefault(p => p.PolicyNumber == policyNumber));
        }

        public Task<List<Policy>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Items.ToList());
        }

        public Task DeleteAsync(Policy policy, CancellationToken cancellationToken = default)
        {
            Items.Remove(policy);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}