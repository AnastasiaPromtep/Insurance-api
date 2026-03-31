namespace InsuranceApi.Models;

public class Policy
{
    public int Id { get; set; }

    public string PolicyNumber { get; set; } = string.Empty;

    public string SubscriberName { get; set; } = string.Empty;

    public decimal PremiumAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public PolicyStatus Status { get; set; }
}