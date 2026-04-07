using System.ComponentModel.DataAnnotations;

namespace InsuranceApi.Requests;

public sealed record CreatePolicyRequest(
    [param: Required]
    [param: StringLength(50, MinimumLength = 1)]
    string PolicyNumber,

    [param: Required]
    [param: StringLength(200, MinimumLength = 1)]
    string SubscriberName,

    [param: Range(typeof(decimal), "0.01", "999999999")]
    decimal PremiumAmount,

    DateTime StartDate);