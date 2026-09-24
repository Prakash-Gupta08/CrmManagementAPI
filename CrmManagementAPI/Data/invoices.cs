using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrmManagementAPI.Data
{
    public class invoices
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("invoice_number")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        [Column("invoice_date")]
        public DateOnly InvoiceDate { get; set; }

        [Required]
        [Column("po_id")]
        public int PoId { get; set; }

        [Column("milestone_id")]
        public int? MilestoneId { get; set; }

        [Required]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Required]
        [Column("taxable_amount_inr", TypeName = "numeric(18,2)")]
        public decimal TaxableAmountInr { get; set; }

        [Column("gst_pct", TypeName = "numeric(5,2)")]
        public decimal? GstPct { get; set; }

        [Column("gst_amount_inr", TypeName = "numeric(18,2)")]
        public decimal? GstAmountInr { get; set; }

        [Required]
        [Column("total_invoice_value_inr", TypeName = "numeric(18,2)")]
        public decimal TotalInvoiceValueInr { get; set; }

        [Column("submission_date")]
        public DateOnly? SubmissionDate { get; set; }

        [Column("acknowledgement_date")]
        public DateOnly? AcknowledgementDate { get; set; }

        [Column("acknowledgement_ref")]
        public string? AcknowledgementRef { get; set; }

        [Column("payment_due_date")]
        public DateOnly? PaymentDueDate { get; set; }

        [Column("payment_received_inr", TypeName = "numeric(18,2)")]
        public decimal PaymentReceivedInr { get; set; } = 0;

        [Column("tds_amount_inr", TypeName = "numeric(18,2)")]
        public decimal TdsAmountInr { get; set; } = 0;

        [Column("other_deductions_inr", TypeName = "numeric(18,2)")]
        public decimal OtherDeductionsInr { get; set; } = 0;

        [Required]
        [Column("outstanding_amount_inr", TypeName = "numeric(18,2)")]
        public decimal OutstandingAmountInr { get; set; }

        [Column("invoice_status")]
        public string InvoiceStatus { get; set; } = "Draft";

        [Column("dispute_notes")]
        public string? DisputeNotes { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
