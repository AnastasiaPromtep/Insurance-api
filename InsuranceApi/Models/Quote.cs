namespace InsuranceApi.Models;

public sealed class Quote
{
    public int QuoteId { get; set; }

    public QuoteStatus QuoteStatus { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}