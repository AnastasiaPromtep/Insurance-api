namespace InsuranceApi.Commands;

public sealed record UpdatePolicyCommand(
    int Id,
    string PolicyNumber,
    string SubscriberName,
    decimal PremiumAmount,
    DateTime StartDate);