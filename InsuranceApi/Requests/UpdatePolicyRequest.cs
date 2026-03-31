namespace InsuranceApi.Requests;

using InsuranceApi.Models;

public record UpdatePolicyRequest(
    string PolicyNumber,
    string SubscriberName,
    decimal PremiumAmount,
    DateTime StartDate,
    DateTime? EndDate,
    PolicyStatus Status
);