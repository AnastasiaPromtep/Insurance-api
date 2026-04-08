using InsuranceApi.Models;

namespace InsuranceApi.Services.Policies;

public interface IPolicyRepository
{
    Task<bool> ExistsByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default);
    Task AddAsync(Policy policy, CancellationToken cancellationToken = default);
    Task<Policy?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Policy?> GetByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken = default);
    Task<List<Policy>> GetAllAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(Policy policy, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}