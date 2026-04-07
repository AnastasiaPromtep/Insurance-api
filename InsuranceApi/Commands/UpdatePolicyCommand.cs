namespace InsuranceApi.Commands;

public sealed record UpdatePolicyCommand(
    string PolicyNumber,
    string SubscriberName,
    decimal PremiumAmount,
    DateTime StartDate);