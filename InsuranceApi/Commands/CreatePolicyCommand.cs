namespace InsuranceApi.Commands;

public sealed record CreatePolicyCommand(
    string PolicyNumber,
    string SubscriberName,
    decimal PremiumAmount,
    DateTime StartDate);