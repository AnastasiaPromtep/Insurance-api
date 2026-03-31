namespace InsuranceApi.Requests;

using InsuranceApi.Models;

public record PatchPolicyRequest(
    string? SubscriberName,
    decimal? PremiumAmount,
    PolicyStatus? Status
);