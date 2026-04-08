using System.ComponentModel.DataAnnotations;

namespace InsuranceApi.Requests;

public sealed class CreateQuoteRequest
{
    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}