namespace InsuranceApi.Commands;

public sealed record CreateQuoteCommand(
    DateTime StartDate,
    DateTime? EndDate
);