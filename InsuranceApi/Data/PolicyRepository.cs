using InsuranceApi.Models;
using InsuranceApi.Services.Policies;
using Microsoft.EntityFrameworkCore;

namespace InsuranceApi.Data;

public sealed class PolicyRepository : IPolicyRepository
{
    private readonly InsuranceDbContext _dbContext;

    public PolicyRepository(InsuranceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default)
    {
        return _dbContext.Policies
            .AnyAsync(p => p.PolicyNumber == policyNumber, cancellationToken);
    }

    public async Task AddAsync(Policy policy, CancellationToken cancellationToken = default)
    {
        await _dbContext.Policies.AddAsync(policy, cancellationToken);
    }

    public Task<Policy?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Policies.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<Policy?> GetByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default)
    {
        return _dbContext.Policies.FirstOrDefaultAsync(p => p.PolicyNumber == policyNumber, cancellationToken);
    }

    public Task<List<Policy>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Policies.ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(Policy policy, CancellationToken cancellationToken = default)
    {
        _dbContext.Policies.Remove(policy);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}