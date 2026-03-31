namespace InsuranceApi.Requests;

public record CreatePolicyRequest(
    string PolicyNumber,
    string SubscriberName,
    decimal PremiumAmount,
    DateTime StartDate
);