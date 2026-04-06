using System.ComponentModel.DataAnnotations;

namespace InsuranceApi.Requests;

public sealed record CreatePolicyRequest(
    [property: Required]
    [property: StringLength(50, MinimumLength = 1)]
    string PolicyNumber,

    [property: Required]
    [property: StringLength(200, MinimumLength = 1)]
    string SubscriberName,

    [property: Range(typeof(decimal), "0.01", "999999999")]
    decimal PremiumAmount,

    [property: Required]
    DateTime StartDate);