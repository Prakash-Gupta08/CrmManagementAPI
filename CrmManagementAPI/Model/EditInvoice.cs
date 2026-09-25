using System;

namespace CrmManagementAPI.Model
{
    public class EditInvoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateOnly InvoiceDate { get; set; }
        public int PoId { get; set; }
        public int? MilestoneId { get; set; }
        public int CustomerId { get; set; }
        public decimal TaxableAmountInr { get; set; }
        public decimal? GstPct { get; set; }
        public decimal? GstAmountInr { get; set; }
        public decimal TotalInvoiceValueInr { get; set; }
        public DateOnly? SubmissionDate { get; set; }
        public DateOnly? AcknowledgementDate { get; set; }
        public string? AcknowledgementRef { get; set; }
        public DateOnly? PaymentDueDate { get; set; }
        public decimal PaymentReceivedInr { get; set; } = 0;
        public decimal TdsAmountInr { get; set; } = 0;
        public decimal OtherDeductionsInr { get; set; } = 0;
        public decimal OutstandingAmountInr { get; set; }
        public string InvoiceStatus { get; set; } = "Draft";
        public string? DisputeNotes { get; set; }
        public string? Notes { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
