using System.ComponentModel.DataAnnotations;
using InsuranceApi.Models;

namespace InsuranceApi.Requests;

public sealed class UpdatePolicyRequest
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string PolicyNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string SubscriberName { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal PremiumAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public PolicyStatus Status { get; set; }
}