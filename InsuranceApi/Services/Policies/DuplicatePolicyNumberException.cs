namespace InsuranceApi.Services.Policies;

public sealed class DuplicatePolicyNumberException : Exception
{
    public DuplicatePolicyNumberException(string policyNumber)
        : base($"A policy with number '{policyNumber}' already exists.")
    {
    }
}